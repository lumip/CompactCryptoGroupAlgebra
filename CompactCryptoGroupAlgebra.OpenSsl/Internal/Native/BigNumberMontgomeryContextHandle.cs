using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{

    sealed class BigNumberMontgomeryContextHandle : NativeHandle
    {
#region Native Methods Imports

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr BN_MONT_CTX_new();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void BN_MONT_CTX_free(IntPtr montCtx);
#endregion

        public static BigNumberMontgomeryContextHandle Create()
        {
            var ctx = new BigNumberMontgomeryContextHandle(BN_MONT_CTX_new(), ownsHandle: true);
            if (ctx.IsInvalid) throw new OpenSslNativeException();
            return ctx;
        }

        public static BigNumberMontgomeryContextHandle Null = new BigNumberMontgomeryContextHandle();

        internal BigNumberMontgomeryContextHandle() : base(ownsHandle: false) { }

        private BigNumberMontgomeryContextHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
            if (IsInvalid)
            {
                throw new ArgumentException("received an invalid handle!", nameof(handle));
            }
        }

        protected override bool ReleaseHandle()
        {
            Debug.Assert(!IsInvalid);
            BN_MONT_CTX_free(this.handle);
            return true;
        }

    }

}