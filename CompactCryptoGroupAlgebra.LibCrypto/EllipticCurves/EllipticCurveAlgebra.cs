// CompactCryptoGroupAlgebra.LibCrypto - OpenSSL libcrypto implementation of CompactCryptoGroupAlgebra interfaces

// SPDX-FileCopyrightText: 2022 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later WITH GPL-3.0-linking-exception
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra.LibCrypto is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra.LibCrypto is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Additional permission under GNU GPL version 3 section 7
// 
// If you modify CompactCryptoGroupAlgebra.LibCrypto, or any covered work, by linking or combining it
// with the OpenSSL library (or a modified version of that library), containing parts covered by the
// terms of the OpenSSL License and the SSLeay License, the licensors of CompactCryptoGroupAlgebra.LibCrypto
// grant you additional permission to convey the resulting work.

using System;
using System.Numerics;
using System.Diagnostics;
using System.Security.Cryptography;

using CompactCryptoGroupAlgebra.LibCrypto.Internal.Native;

namespace CompactCryptoGroupAlgebra.LibCrypto.EllipticCurves
{

    /// <summary>
    /// OpenSSL-based cryptographic group algebra based on point addition in elliptic curves.
    /// 
    /// Elements of the group are all points (<c>x mod P</c>, <c>y mod P</c>) that satisfy
    /// the curve equation and are of group order (to prevent small subgroup attacks).
    ///
    /// <see cref="EllipticCurveAlgebra" /> uses OpenSSL based <see cref="ECPoint" />s
    /// as raw group elements and <see cref="SecureBigNumber" />s as scalar factors and private keys.
    /// </summary>
    public sealed class EllipticCurveAlgebra : ICryptoGroupAlgebra<SecureBigNumber, ECPoint>, IDisposable
    {
        private object instanceLock = new object();

        private static readonly PointEncoding GroupPointEncoding = PointEncoding.Compressed;

        internal ECGroupHandle Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new <see cref="EllipticCurveAlgebra" /> instance
        /// for the curve identified by <paramref name="curveId"/>.
        /// </summary>
        /// <param name="curveId">Identifier of the elliptic curve.</param>
        public EllipticCurveAlgebra(EllipticCurveID curveId)
        {
            Handle = ECGroupHandle.CreateByCurveNID((int)curveId);

            using (var ctx = BigNumberContextHandle.Create())
            {
                ECGroupHandle.PrecomputeGeneratorMultiples(Handle, ctx);
            }
        }

        private BigPrime? _order;

        /// <inheritdoc />
        public BigPrime Order
        {
            get
            {
                if (!_order.HasValue) // read is atomic; if _order.HasValue we don't need to lock because it will not be written again
                {
                    // if _order has no value, we create and store one for faster lookup in later calls
                    lock (instanceLock)
                    {
                        if (!_order.HasValue) // check that _order has not been set while we were waiting for lock
                        {
                            using (var order = new BigNumber())
                            using (var ctx = BigNumberContextHandle.Create())
                            {
                                ECGroupHandle.GetOrder(Handle, order.Handle, ctx);
                                var orderInt = order.ToBigInteger();

                                Debug.Assert(orderInt.IsProbablyPrime(RandomNumberGenerator.Create()));
                                _order = BigPrime.CreateWithoutChecks(orderInt);
                            }
                        }
                    }
                }
                return _order.Value;
            }
        }

        /// <inheritdoc />
        public ECPoint Generator
        {
            get
            {
                var rawGenerator = ECGroupHandle.GetGenerator(Handle);
                return new ECPoint(Handle, rawGenerator);
            }
        }

        private ECPoint? _neutralElement;

        /// <inheritdoc />
        public ECPoint NeutralElement
        {
            get
            {
                if (_neutralElement == null)
                {
                    lock (instanceLock)
                    {
                        if (_neutralElement == null)
                        {
                            _neutralElement = new ECPoint(Handle);
                            ECPointHandle.SetToInfinity(Handle, _neutralElement.Handle);
                        }
                    }
                }
                return _neutralElement;
            }
        }

        private BigInteger? _cofactor;

        /// <inheritdoc />
        public BigInteger Cofactor
        {
            get
            {
                if (!_cofactor.HasValue)
                {
                    lock (instanceLock)
                    {
                        if (!_cofactor.HasValue)
                        {
                            using (var cofactor = new BigNumber())
                            using (var ctx = BigNumberContextHandle.Create())
                            {
                                ECGroupHandle.GetCofactor(Handle, cofactor.Handle, ctx);
                                _cofactor = cofactor.ToBigInteger();
                            }
                        }
                    }
                }
                return _cofactor.Value;
            }
        }

        /// <inheritdoc />
        public int ElementBitLength 
        {
            get
            {
                return PointEncodingLength.GetEncodingBitLength(GroupPointEncoding, ECGroupHandle.GetDegree(Handle));
            }
        }

        /// <inheritdoc />
        public int OrderBitLength
        {
            get
            {
                return ECGroupHandle.GetOrderNumberOfBits(Handle);
            }
        }

        /// <inheritdoc/>
        public int SecurityLevel => OrderBitLength / 2;

        /// <inheritdoc />
        public ECPoint Add(ECPoint left, ECPoint right)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var res = new ECPoint(Handle);
                ECPointHandle.Add(Handle, res.Handle, left.Handle, right.Handle, ctx);
                return res;
            }
        }

        /// <inheritdoc />
        public ECPoint FromBytes(byte[] buffer)
        {
            return ECPoint.CreateFromBytes(Handle, buffer);
        }

        /// <inheritdoc />
        public ECPoint GenerateElement(SecureBigNumber index)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var res = new ECPoint(Handle);
                ECPointHandle.Multiply(Handle, res.Handle, index.Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
                return res;
            }
        }

        /// <inheritdoc />
        public bool IsPotentialElement(ECPoint element)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                return ECPointHandle.IsOnCurve(Handle, element.Handle, ctx);
            }
        }

        /// <inheritdoc />
        public bool IsSafeElement(ECPoint element)
        {
            if (!IsPotentialElement(element)) return false;

            // verifying that the point is not from a small subgroup of the whole curve (and thus outside
            // of the safe subgroup over which operations are considered)
            using (var cofactorBignum = new BigNumber(Cofactor))
            {
                var check = MultiplyScalar(element, SecureBigNumber.FromBigNumber(new BigNumber(Cofactor)));
                if (check.IsAtInfinity)
                    return false;
            }
            return true;
        }

        /// <inheritdoc />
        public ECPoint MultiplyScalar(ECPoint e, SecureBigNumber k)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var res = new ECPoint(Handle);
                ECPointHandle.Multiply(Handle, res.Handle, BigNumberHandle.Null, e.Handle, k.Handle, ctx);
                return res;
            }
        }

        /// <inheritdoc />
        public ECPoint Negate(ECPoint element)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var p = new ECPoint(Handle, element.Handle);
                ECPointHandle.InvertInPlace(Handle, p.Handle, ctx);
                return p;
            }
        }

        /// <inheritdoc />
        public byte[] ToBytes(ECPoint element)
        {
            return element.ToBytes(GroupPointEncoding);
        }

        /// <inheritdoc />
        public (SecureBigNumber, ECPoint) GenerateRandomElement(RandomNumberGenerator randomNumberGenerator)
        {
            using (var keyHandle = ECKeyHandle.Create())
            {
                ECKeyHandle.SetGroup(keyHandle, Handle);

                // note(lumip): OpenSSL up to version 1.1.1 does not generate private keys for EC
                //  as secure BIGNUM. Workaround by setting an empty secure private key BIGNUM before
                //  generation. (cf. https://github.com/openssl/openssl/issues/13892)
                using (var privKeyTemplateHandle = BigNumberHandle.CreateSecure())
                {
                    ECKeyHandle.SetPrivateKey(keyHandle, privKeyTemplateHandle);
                }
                ECKeyHandle.GenerateKey(keyHandle);

                // note(lumip): ensure the workaround worked
                var privKeyHandle = ECKeyHandle.GetPrivateKey(keyHandle);
                Debug.Assert(!privKeyHandle.IsInvalid);
                Debug.Assert(BigNumberHandle.GetFlags(privKeyHandle).HasFlag(BigNumberFlags.Secure));

                var pubKeyHandle = ECKeyHandle.GetPublicKey(keyHandle);
                Debug.Assert(!pubKeyHandle.IsInvalid);
                var point = new ECPoint(Handle, pubKeyHandle);

                var index = SecureBigNumber.FromRawHandle(privKeyHandle);
                return (index, point);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            EllipticCurveAlgebra? other = obj as EllipticCurveAlgebra;
            if (other == null) return false;

            using (var ctx = BigNumberContextHandle.Create())
            {
                return ECGroupHandle.Compare(this.Handle, other.Handle, ctx);
            }
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = 55837;
            hashCode = hashCode * 233 + Order.GetHashCode();
            hashCode = hashCode * 233 + Generator.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{SecureBigNumber, ECPoint}" /> instance using a <see cref="EllipticCurveAlgebra" />
        /// for the given curve identifier.
        /// </summary>
        /// <param name="curveId">Identifier of the elliptic curve.</param>
        public static CryptoGroup<SecureBigNumber, ECPoint> CreateCryptoGroup(EllipticCurveID curveId)
        {
            return new CryptoGroup<SecureBigNumber, ECPoint>(new EllipticCurveAlgebra(curveId));
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{SecureBigNumber, ECPoint}" /> instance at least satisfying the given security level.
        /// </summary>
        /// <param name="securityLevel">The minimal security level for the curve to be created.</param>
        public static CryptoGroup<SecureBigNumber, ECPoint> CreateCryptoGroup(int securityLevel)
        {
            EllipticCurveID curveId;
            if (securityLevel <= 128)
            {
                curveId = EllipticCurveID.Prime256v1; // == Secp256r1
            }
            else if (securityLevel <= 192)
            {
                curveId = EllipticCurveID.Secp384r1;
            }
            else if (securityLevel <= 260)
            {
                curveId = EllipticCurveID.Secp521r1;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    $"There are no curves that satisfy a security level of {securityLevel}.", nameof(securityLevel)
                );
            }
            return CreateCryptoGroup(curveId);
        }

    }

}
