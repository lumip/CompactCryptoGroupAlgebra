using System;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

namespace CompactCryptoGroupAlgebra.OpenSsl
{

    /// <summary>
    /// An arbitrarily large integer number in OpenSSL secure heap memory.
    ///
    /// Used to store numbers considered to be secrets and thus does not
    /// allow extracting values. If this is required 
    /// consider using <see cref="BigNumber" />.
    /// </summary>
    public sealed class SecureBigNumber : IDisposable
    {

        internal BigNumberHandle Handle { get; private set; }

        /// <summary>
        /// Create a <see cref="SecureBigNumber" /> with a value
        /// sampled uniformly at random less than <paramref name="range"/>.
        /// </summary>
        /// <param name="range">The upper bound for the randomly generated value.</param>
        /// <returns>
        /// New <see cref="SecureBigNumber" /> instance containing
        /// the random value.
        /// </returns>
        public static SecureBigNumber Random(BigNumber range)
        {
            var result = new SecureBigNumber();
            BigNumberHandle.SecureRandom(result.Handle, range.Handle);
            return result;
        }

        /// <summary>
        /// Creates a new uninitialized <see cref="SecureBigNumber" /> instance.
        /// </summary>
        public SecureBigNumber()
        {
            Handle = BigNumberHandle.CreateSecure();
        }

        /// <summary>
        /// Creates a <see cref="SecureBigNumber" /> instance from a valid <see cref="BigNumberHandle" />
        /// to a secure OpenSSL <c>BIGNUM</c> structure. A copy of the pointed to <c>BIGNUM</c> structure
        /// is made for the created instance.
        /// </summary>
        /// <param name="bigNumberHandle">
        /// A handle to a raw OpenSSL <c>BIGNUM</c> structure with which to initialize the new <see cref="SecureBigNumber" />.
        /// </param>
        /// <returns>
        /// A new <see cref="SecureBigNumber" /> instance with the same value as
        /// referred to by <paramref name="bigNumberHandle"/>.
        /// </returns>
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

        /// <summary>
        /// Creates a <see cref="SecureBigNumber" /> instance from
        /// a non-secret <see cref="BigNumber" />.
        /// </summary>
        /// <param name="number">
        /// The <see cref="BigNumber" /> to convert into to a <see cref="SecureBigNumber" />
        /// </param>
        /// <returns>
        /// A new <see cref="SecureBigNumber" /> instance with a value
        /// equal to <paramref name="number"/>.
        /// </returns>
        internal static SecureBigNumber FromBigNumber(BigNumber number)
        {
            return FromRawHandle(number.Handle);
        }

        /// <summary>
        /// The length of the integer in bits.
        /// </summary>
        public NumberLength Length
        {
            get
            {
                return NumberLength.FromBitLength(BigNumberHandle.GetNumberOfBits(Handle));
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
            }
        }

    }

}