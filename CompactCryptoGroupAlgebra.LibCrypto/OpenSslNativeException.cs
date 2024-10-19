// CompactCryptoGroupAlgebra.LibCrypto - OpenSSL libcrypto implementation of CompactCryptoGroupAlgebra interfaces

// SPDX-FileCopyrightText: 2021 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later WITH GPL-3.0-linking-exception
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra.LibCrypto is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra.LibCrypto is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Additional permission under GNU GPL version 3 section 7
// 
// If you modify CompactCryptoGroupAlgebra.LibCrypto, or any covered work, by linking or combining it
// with the OpenSSL library (or a modified version of that library), containing parts covered by the
// terms of the OpenSSL License and the SSLeay License, the licensors of CompactCryptoGroupAlgebra.LibCrypto
// grant you additional permission to convey the resulting work.

using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.LibCrypto
{
    /// <summary>
    /// Exception type for errors in native OpenSSL calls.
    /// </summary>
    class OpenSslNativeException : Exception
    {

        private static object NativeExceptionLock = new object();

        #region Native Methods
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static ulong ERR_get_error();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr ERR_error_string(ulong e, byte[]? buf);
        #endregion

        /// <summary>
        /// Thread-safe wrapper around <see cref="ERR_get_error()" />.
        /// </summary>
        private static ulong GetLastError()
        {
            lock (NativeExceptionLock)
            {
                return ERR_get_error();
            }
        }

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
        internal OpenSslNativeException() : this(GetLastError())
        {
        }
    }
}
