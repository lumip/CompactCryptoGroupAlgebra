using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{

    /// <summary>
    /// A handle for OpenSSL <c>BN_MONT_CTX</c> structures.
    /// </summary>
    sealed class BigNumberMontgomeryContextHandle : NativeHandle
    {
#region Native Methods Imports

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr BN_MONT_CTX_new();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void BN_MONT_CTX_free(IntPtr montCtx);
#endregion

        /// <summary>
        /// Allocates a new OpenSSL <c>BN_MONT_CTX</c> structure
        /// and returns a handle to it.
        ///
        /// The returned handle is guaranteed to point to be valid,
        /// i.e., point to a valid <c>BN_MONT_CTX</c> structure.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BigNumberMontgomeryContextHandle" /> pointing to a
        /// freshly allocated <c>BN_MONT_CTX</c> structure.
        /// </returns>
        public static BigNumberMontgomeryContextHandle Create()
        {
            var ctx = new BigNumberMontgomeryContextHandle(BN_MONT_CTX_new(), ownsHandle: true);
            if (ctx.IsInvalid) throw new OpenSslNativeException();
            return ctx;
        }

        /// <summary>
        /// An unintialized handle. A null pointer.
        /// </summary>
        public static BigNumberMontgomeryContextHandle Null = new BigNumberMontgomeryContextHandle();

        internal BigNumberMontgomeryContextHandle() : base(ownsHandle: false) { }

        /// <summary>
        /// Creates a new <see cref="BigNumberMontgomeryContextHandle" /> instance for
        /// the given raw <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle">A valid pointer to a OpenSSL <c>BN_MONT_CTX</c> structure.</param>
        /// <param name="ownsHandle">
        /// If <c>true</c>, the referred <c>BN_MONT_CTX</c> will be deleted from memory on disposal.
        /// </param>
        private BigNumberMontgomeryContextHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
            if (IsInvalid)
            {
                throw new ArgumentException("received an invalid handle!", nameof(handle));
            }
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            Debug.Assert(!IsInvalid);
            BN_MONT_CTX_free(this.handle);
            return true;
        }

    }

}