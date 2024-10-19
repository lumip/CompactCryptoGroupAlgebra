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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.LibCrypto.Internal.Native
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

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            Debug.Assert(!IsInvalid);
            BN_CTX_free(this.handle);
            return true;
        }
    }
}
