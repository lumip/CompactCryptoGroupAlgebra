using System;
using System.Numerics;
using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra.OpenSsl
{

    public class EllipticCurveAlgebra : ICryptoGroupAlgebra<SecureBigNumber, ECPoint>, IDisposable
    {

        private static readonly PointEncoding GroupPointEncoding = PointEncoding.Compressed;

        internal ECGroupHandle Handle
        {
            get;
            private set;
        }

        private BigNumberContextHandle _ctxHandle;

        public EllipticCurveAlgebra(EllipticCurveID curveId)
        {
            Handle = ECGroupHandle.CreateByCurveNID((int)curveId);

            _ctxHandle = BigNumberContextHandle.Create();

            ECGroupHandle.PrecomputeGeneratorMultiples(Handle, _ctxHandle);
        }

        private BigPrime? _order;

        /// <inheritdocs />
        public BigPrime Order
        {
            get
            {
                if (!_order.HasValue)
                {
                    using (var order = new BigNumber())
                    {
                        ECGroupHandle.GetOrder(Handle, order.Handle, _ctxHandle);
                        var orderInt = order.ToBigInteger();

                        Debug.Assert(orderInt.IsProbablyPrime(RandomNumberGenerator.Create()));
                        _order = BigPrime.CreateWithoutChecks(orderInt);
                    }
                }
                return _order.Value;
            }
        }

        /// <inheritdocs />
        public ECPoint Generator
        {
            get
            {
                var rawGenerator = ECGroupHandle.GetGenerator(Handle);
                return new ECPoint(Handle, rawGenerator);
            }
        }

        private ECPoint? _neutralElement;

        /// <inheritdocs />
        public ECPoint NeutralElement
        {
            get
            {
                if (_neutralElement == null)
                {
                    _neutralElement = new ECPoint(Handle);
                    ECPointHandle.SetToInfinity(Handle, _neutralElement.Handle);
                }
                return _neutralElement;
            }
        }

        private BigInteger? _cofactor;

        /// <inheritdocs />
        public BigInteger Cofactor
        {
            get
            {
                if (!_cofactor.HasValue)
                {
                    using (var cofactor = new BigNumber())
                    {
                        ECGroupHandle.GetCofactor(Handle, cofactor.Handle, _ctxHandle);
                        _cofactor = cofactor.ToBigInteger();
                    }
                }
                return _cofactor.Value;
            }
        }

        /// <inheritdocs />
        public int ElementBitLength 
        {
            get
            {
                return PointEncodingLength.GetEncodingBitLength(GroupPointEncoding, ECGroupHandle.GetDegree(Handle));
            }
        }

        /// <inheritdocs />
        public int OrderBitLength
        {
            get
            {
                return ECGroupHandle.GetOrderNumberOfBits(Handle);
            }
        }

        /// <inheritdocs />
        public ECPoint Add(ECPoint left, ECPoint right)
        {
            var res = new ECPoint(Handle);
            ECPointHandle.Add(Handle, res.Handle, left.Handle, right.Handle, _ctxHandle);
            return res;
        }

        /// <inheritdocs />
        public ECPoint FromBytes(byte[] buffer)
        {
            return ECPoint.CreateFromBytes(Handle, buffer);
        }

        /// <inheritdocs />
        public ECPoint GenerateElement(SecureBigNumber index)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var res = new ECPoint(Handle);
                ECPointHandle.Multiply(Handle, res.Handle, index.Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
                return res;
            }
        }

        /// <inheritdocs />
        public bool IsElement(ECPoint element)
        {
            return ECPointHandle.IsOnCurve(Handle, element.Handle, _ctxHandle);
        }

        /// <inheritdocs />
        public ECPoint MultiplyScalar(ECPoint e, SecureBigNumber k)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var res = new ECPoint(Handle);
                ECPointHandle.Multiply(Handle, res.Handle, BigNumberHandle.Null, e.Handle, k.Handle, ctx);
                return res;
            }
        }

        /// <inheritdocs />
        public ECPoint Negate(ECPoint element)
        {
            var p = new ECPoint(Handle, element.Handle);
            ECPointHandle.InvertInPlace(Handle, p.Handle, _ctxHandle);
            return p;
        }

        /// <inheritdocs />
        public byte[] ToBytes(ECPoint element)
        {
            return element.ToBytes(GroupPointEncoding);
        }

        /// <inheritdocs />
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

        /// <inheritdocs />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
                _ctxHandle.Dispose();
            }
        }

        /// <inheritdocs />
        public override bool Equals(object? obj)
        {
            EllipticCurveAlgebra? other = obj as EllipticCurveAlgebra;
            if (other == null) return false;

            return ECGroupHandle.Compare(this.Handle, other.Handle, _ctxHandle);
        }
        
        /// <inheritdocs />
        public override int GetHashCode()
        {
            int hashCode = 55837;
            hashCode = hashCode * 233 + Order.GetHashCode();
            hashCode = hashCode * 233 + Generator.GetHashCode();
            return hashCode;
        }

    }

}
