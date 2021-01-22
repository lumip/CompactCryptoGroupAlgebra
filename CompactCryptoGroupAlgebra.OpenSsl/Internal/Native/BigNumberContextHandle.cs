using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{

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

        ///
        /// <remarks>Guaranteed to result in a valid handle if no exception.</remarks>
        public static BigNumberContextHandle Create()
        {
            var ctx = new BigNumberContextHandle(BN_CTX_new(), ownsHandle: true);
            if (ctx.IsInvalid) throw new OpenSslNativeException();
            return ctx;
        }

        ///
        /// <remarks>Guaranteed to result in a valid handle if no exception.</remarks>
        public static BigNumberContextHandle CreateSecure()
        {
            var ctx = new BigNumberContextHandle(BN_CTX_secure_new(), ownsHandle: true);
            if (ctx.IsInvalid) throw new OpenSslNativeException();
            return ctx;
        }

        public static BigNumberContextHandle Null = new BigNumberContextHandle();

        internal BigNumberContextHandle() : base(ownsHandle: false)
        {

        }

        internal BigNumberContextHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
            if (IsInvalid)
            {
                throw new ArgumentException("received an invalid handle", nameof(handle));
            }
        }

        protected override bool ReleaseHandle()
        {
            Debug.Assert(!IsInvalid);
            BN_CTX_free(this.handle);
            return true;
        }
    }
}