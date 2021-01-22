using System;
using System.Runtime.CompilerServices;

using System.Numerics;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

[assembly:InternalsVisibleTo("CompactCryptoGroupAlgebra.OpenSsl.Tests")]
namespace CompactCryptoGroupAlgebra.OpenSsl.Internal
{

    public class BigNumber : IDisposable
    {

        private BigNumberHandle _bn;

        internal BigNumberHandle Handle
        {
            get
            {
                return _bn;
            }
        }

        public NumberLength Length
        {
            get
            {
                return NumberLength.FromBitLength(BigNumberHandle.GetNumberOfBits(_bn));
            }
        }

        public static BigNumber Zero = new BigNumber(0);

        public static BigNumber One = new BigNumber(1);

        internal static BigNumber FromRawHandle(BigNumberHandle bigNumberHandle, bool secure = false)
        {
            if (bigNumberHandle.IsInvalid)
            {
                throw new ArgumentException("The provided handle is invalid.", nameof(bigNumberHandle));
            }
            var bn = new BigNumber(secure);
            BigNumberHandle.Copy(bn.Handle, bigNumberHandle);
            return bn;
        }

        private BigNumber(BigNumberHandle handle)
        {
            _bn = handle;
            Debug.Assert(!handle.IsInvalid, "BigNumber received an invalid handle internally!");
        }

        private static BigNumberHandle CreateHandle(bool secure)
        {
            if (secure) return BigNumberHandle.CreateSecure();
            return BigNumberHandle.Create();
        }


        public BigNumber(bool secure = false) : this(CreateHandle(secure)) { }

        public BigNumber(BigInteger x, bool secure = false) : this(x.ToByteArray(), secure) { }

        public BigNumber(byte[] buffer, bool secure = false) : this(secure)
        {
            BigNumberHandle.FromBytes(buffer, _bn);
        }

        public BigNumber(ulong x, bool secure = false) : this(secure)
        {
            BigNumberHandle.SetWord(_bn, x);
        }

        public byte[] ToBytes(uint backPadding = 0)
        {
            byte[] buffer = new byte[Length.InBytes + backPadding];
            BigNumberHandle.ToBytes(_bn, buffer);
            return buffer;
        }

        public BigInteger ToBigInteger()
        {
            byte[] buffer = ToBytes(backPadding: 1);
            var integer = new BigInteger(buffer);
            return integer; // BigInteger.Abs(new BigInteger(ToBytes()));
        }

        private BigNumberFlags Flags => BigNumberHandle.GetFlags(_bn);

        public bool IsSecure
        {
            get
            {
                return Flags.HasFlag(BigNumberFlags.Secure);
            }
        }
        public bool IsConstantTime
        {
            get
            {
                return Flags.HasFlag(BigNumberFlags.ConstantTime);
            }
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
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