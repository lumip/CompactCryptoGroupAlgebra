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

        private ECPoint? _generator;

        public ECPoint Generator
        {
            get
            {
                if (_generator == null)
                {
                    var rawGenerator = ECGroupHandle.GetGenerator(Handle);

                    _generator = new ECPoint(Handle, rawGenerator);
                }
                return _generator;
            }
        }

        private ECPoint? _neutralElement;

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

        public int ElementBitLength 
        {
            get
            {
                return PointEncodingLength.GetEncodingBitLength(GroupPointEncoding, ECGroupHandle.GetDegree(Handle));
            }
        }

        public int OrderBitLength
        {
            get
            {
                return ECGroupHandle.GetOrderNumberOfBits(Handle);
            }
        }

        public ECPoint Add(ECPoint left, ECPoint right)
        {
            var res = new ECPoint(Handle);
            ECPointHandle.Add(Handle, res.Handle, left.Handle, right.Handle, _ctxHandle);
            return res;
        }

        public ECPoint FromBytes(byte[] buffer)
        {
            return ECPoint.CreateFromBytes(Handle, buffer);
        }

        public ECPoint GenerateElement(SecureBigNumber index)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var res = new ECPoint(Handle);
                ECPointHandle.Multiply(Handle, res.Handle, index.Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
                return res;
            }
        }

        public bool IsElement(ECPoint element)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                return ECPointHandle.IsOnCurve(Handle, element.Handle, ctx);
            }
        }

        public ECPoint MultiplyScalar(ECPoint e, SecureBigNumber k)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var res = new ECPoint(Handle);
                ECPointHandle.Multiply(Handle, res.Handle, BigNumberHandle.Null, e.Handle, k.Handle, ctx);
                return res;
            }
        }

        public ECPoint Negate(ECPoint element)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var p = new ECPoint(Handle, element.Handle);
                ECPointHandle.InvertInPlace(Handle, p.Handle, ctx);
                return p;
            }
        }

        public byte[] ToBytes(ECPoint element)
        {
            return element.ToBytes(GroupPointEncoding);
        }

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
    }

}
