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

namespace CompactCryptoGroupAlgebra.LibCrypto.Internal.Native
{

    /// <summary>
    /// Base class for native handles of OpenSSL structures.
    /// </summary>
    abstract class NativeHandle : SafeHandle
    {

        protected NativeHandle(bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
        }

#if FEATURE_CORECLR
        protected NativeHandle()
        {
            throw new NotImplementedException();
        }
#endif // FEATURE_CORECLR

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get
            {
                return (handle == IntPtr.Zero || handle == new IntPtr(-1));
            }
        }

    }
}
