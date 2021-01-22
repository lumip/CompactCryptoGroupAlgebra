using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{
    class OpenSslNativeException : Exception
    {

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static ulong ERR_get_error();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr ERR_error_string(ulong e, byte[]? buf);


        public ulong Code { get; private set; }

        public OpenSslNativeException() : this(ERR_get_error())
        {
        }

        private static string GetErrorMessage(ulong errorCode)
        {
            return Marshal.PtrToStringAnsi(ERR_error_string(errorCode, null)); 
        }

        public OpenSslNativeException(ulong code) : base(GetErrorMessage(code)) { }
    }
}