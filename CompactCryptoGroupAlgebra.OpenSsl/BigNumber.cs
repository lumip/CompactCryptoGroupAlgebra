using System;
using System.Runtime.CompilerServices;

using System.Numerics;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

[assembly:InternalsVisibleTo("CompactCryptoGroupAlgebra.OpenSsl.Tests")]
namespace CompactCryptoGroupAlgebra.OpenSsl
{

    public sealed class BigNumber : IDisposable
    {

        internal BigNumberHandle Handle
        {
            get;
            private set;
        }

        public NumberLength Length
        {
            get
            {
                return NumberLength.FromBitLength(BigNumberHandle.GetNumberOfBits(Handle));
            }
        }

        public static BigNumber Zero = new BigNumber(0);

        public static BigNumber One = new BigNumber(1);

        internal static BigNumber FromRawHandle(BigNumberHandle bigNumberHandle)
        {
            if (bigNumberHandle.IsInvalid)
            {
                throw new ArgumentException("The provided handle is invalid.", nameof(bigNumberHandle));
            }
            if (BigNumberHandle.GetFlags(bigNumberHandle).HasFlag(BigNumberFlags.Secure))
            {
                throw new ArgumentException(
                    "The provided handle is that of a secure big number. Converting secure into regular big numbers is not supported.",
                    nameof(bigNumberHandle)
                );
            }
            var bn = new BigNumber();
            BigNumberHandle.Copy(bn.Handle, bigNumberHandle);
            return bn;
        }

        private BigNumber(BigNumberHandle handle)
        {
            Debug.Assert(!handle.IsInvalid, "BigNumberBase received an invalid handle internally!");
            Handle = handle;
        }

        public BigNumber() : this(BigNumberHandle.Create()) { }

        public BigNumber(BigInteger x) : this(x.ToByteArray()) { }

        public BigNumber(byte[] buffer) : this()
        {
            BigNumberHandle.FromBytes(buffer, Handle);
        }

        public BigNumber(ulong x) : this()
        {
            BigNumberHandle.SetWord(Handle, x);
        }

        public byte[] ToBytes(uint backPadding = 0)
        {
            byte[] buffer = new byte[Length.InBytes + backPadding];
            BigNumberHandle.ToBytes(Handle, buffer);
            return buffer;
        }

        public BigInteger ToBigInteger()
        {
            byte[] buffer = ToBytes(backPadding: 1);
            var integer = new BigInteger(buffer);
            return integer;
        }

        /// <inheritdocs />
        public override bool Equals(object? obj)
        {        
            var other = obj as BigNumber;
            if (other == null)
            {
                return false;
            }

            return (BigNumberHandle.Compare(this.Handle, other.Handle) == 0);
        }
        
        /// <inheritdocs />
        public override int GetHashCode()
        {
            return ToBigInteger().GetHashCode();
        }

        /// <inheritdocs />
        public override string ToString()
        {
            return ToBigInteger().ToString();
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
            }
        }

        public BigNumber ModMul(BigNumber other, BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var result = new BigNumber();
                BigNumberHandle.ModMul(result.Handle, Handle, other.Handle, modulo.Handle, ctx);
                return result;
            }
        }

        public BigNumber ModExp(SecureBigNumber exponent, BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var result = new BigNumber();
                BigNumberHandle.SecureModExp(result.Handle, Handle, exponent.Handle, modulo.Handle, ctx);
                return result;
            }
        }

        public BigNumber ModExp(BigNumber exponent, BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var result = new BigNumber();
                BigNumberHandle.ModExp(result.Handle, Handle, exponent.Handle, modulo.Handle, ctx);
                return result;
            }
        }

        public BigNumber ModReciprocal(BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var result = new BigNumber();
                BigNumberHandle.ModInverse(result.Handle, Handle, modulo.Handle, ctx);
                return result;
            }
        }

        /// <inheritdocs />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}