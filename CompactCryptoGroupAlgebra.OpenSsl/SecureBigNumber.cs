using System;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

namespace CompactCryptoGroupAlgebra.OpenSsl
{

    public sealed class SecureBigNumber : IDisposable
    {

        internal BigNumberHandle Handle { get; private set; }

        public static SecureBigNumber Random(BigNumber range)
        {
            var result = new SecureBigNumber();
            BigNumberHandle.SecureRandom(result.Handle, range.Handle);
            return result;
        }

        public SecureBigNumber()
        {
            Handle = BigNumberHandle.CreateSecure();
        }

        internal static SecureBigNumber FromRawHandle(BigNumberHandle bigNumberHandle)
        {
            if (bigNumberHandle.IsInvalid)
            {
                throw new ArgumentException("The provided handle is invalid.", nameof(bigNumberHandle));
            }
            var bn = new SecureBigNumber();
            BigNumberHandle.Copy(bn.Handle, bigNumberHandle);
            return bn;
        }

        internal static SecureBigNumber FromBigNumber(BigNumber number)
        {
            return FromRawHandle(number.Handle);
        }

        public NumberLength Length
        {
            get
            {
                return NumberLength.FromBitLength(BigNumberHandle.GetNumberOfBits(Handle));
            }
        }

        /// <inheritdocs />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
            }
        }

    }

}