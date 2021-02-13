using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl
{
    /// <summary>
    /// Exception type for errors in native OpenSSL calls.
    /// </summary>
    class OpenSslNativeException : Exception
    {

#region Native Methods
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static ulong ERR_get_error();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr ERR_error_string(ulong e, byte[]? buf);
#endregion

        /// <summary>
        /// OpenSSL error code.
        /// </summary>
        public ulong Code { get; private set; }

        /// <summary>
        /// Returns the OpenSSL error message for a given error code.
        /// </summary>
        /// <param name="errorCode">The OpenSsl error code.</param>
        /// <returns>
        /// The OpenSSL error message corresponding to <paramref name="errorCode"/>.
        /// </returns>
        private static string GetErrorMessage(ulong errorCode)
        {
            return Marshal.PtrToStringAnsi(ERR_error_string(errorCode, null)); 
        }

        /// <summary>
        /// Creates a new <see cref="OpenSslNativeException" /> for a
        /// given error code.
        /// </summary>
        /// <param name="code">An OpenSSL error code.</param>
        public OpenSslNativeException(ulong code) : base(GetErrorMessage(code))
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new <see cref="OpenSslNativeException" /> for the
        /// most recent OpenSSL error.
        /// </summary>
        internal OpenSslNativeException() : this(ERR_get_error())
        {
        }
    }
}