using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{

    /// <summary>
    /// A handle for OpenSSL <c>BN_CTX</c> structures.
    /// </summary>
    sealed class BigNumberContextHandle : NativeHandle
    {

#region MemoryHandling
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr BN_CTX_new();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr BN_CTX_secure_new();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void BN_CTX_free(IntPtr c);
#endregion

        /// <summary>
        /// Allocates a new OpenSSL <c>BN_CTX</c> structure
        /// and returns a handle to it.
        ///
        /// The returned handle is guaranteed to point to be valid,
        /// i.e., point to a valid <c>BN_CTX</c> structure.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BigNumberContextHandle" /> pointing to a
        /// freshly allocated <c>BN_CTX</c> structure.
        /// </returns>
        public static BigNumberContextHandle Create()
        {
            var ctx = new BigNumberContextHandle(BN_CTX_new(), ownsHandle: true);
            if (ctx.IsInvalid) throw new OpenSslNativeException();
            return ctx;
        }

        /// <summary>
        /// Allocates a new OpenSSL <c>BN_CTX</c> structure
        /// in OpenSSL secure heap and returns a handle to it.
        ///
        /// The returned handle is guaranteed to point to be valid,
        /// i.e., point to a valid <c>BN_CTX</c> structure.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BigNumberContextHandle" /> pointing to a
        /// freshly allocated <c>BN_CTX</c> structure.
        /// </returns>
        public static BigNumberContextHandle CreateSecure()
        {
            var ctx = new BigNumberContextHandle(BN_CTX_secure_new(), ownsHandle: true);
            if (ctx.IsInvalid) throw new OpenSslNativeException();
            return ctx;
        }

        /// <summary>
        /// An unintialized handle. A null pointer.
        /// </summary>
        public static BigNumberContextHandle Null = new BigNumberContextHandle();

        internal BigNumberContextHandle() : base(ownsHandle: false)
        {

        }

        /// <summary>
        /// Creates a new <see cref="BigNumberContextHandle" /> instance for
        /// the given raw <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle">A valid pointer to a OpenSSL <c>BN_CTX</c> structure.</param>
        /// <param name="ownsHandle">
        /// If <c>true</c>, the referred <c>BN_CTX</c> will be deleted from memory on disposal.
        /// </param>
        internal BigNumberContextHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
            if (IsInvalid)
            {
                throw new ArgumentException("received an invalid handle", nameof(handle));
            }
        }

        /// <inheritdocs />
        protected override bool ReleaseHandle()
        {
            Debug.Assert(!IsInvalid);
            BN_CTX_free(this.handle);
            return true;
        }
    }
}