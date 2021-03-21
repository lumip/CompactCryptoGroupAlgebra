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
using System.Diagnostics;

namespace CompactCryptoGroupAlgebra.LibCrypto.Internal.Native
{

    /// <summary>
    /// A handle for OpenSSL <c>EC_GROUP</c> structures.
    /// </summary>
    sealed class ECGroupHandle : NativeHandle
    {

#region Native Methods Imports
        [DllImport("libcrypto", CallingConvention=CallingConvention.Cdecl)]
        private extern static int EC_GROUP_order_bits(ECGroupHandle group);

        [DllImport("libcrypto", CallingConvention=CallingConvention.Cdecl)]
        private extern static int EC_GROUP_get_order(ECGroupHandle group, BigNumberHandle order, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_GROUP_get_cofactor(ECGroupHandle group, BigNumberHandle cofactor, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static  ECPointHandle EC_GROUP_get0_generator(ECGroupHandle group);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_GROUP_precompute_mult(ECGroupHandle group, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_GROUP_have_precompute_mult(ECGroupHandle group);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_GROUP_get_degree(ECGroupHandle group);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_GROUP_cmp(ECGroupHandle a, ECGroupHandle b, BigNumberContextHandle ctx);
#endregion

#region Checked Native Methods
        public static void GetOrder(ECGroupHandle group, BigNumberHandle order, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!order.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(order)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (EC_GROUP_get_order(group, order, ctx) == 0) throw new OpenSslNativeException();
        }

        public static void GetCofactor(ECGroupHandle group, BigNumberHandle cofactor, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!cofactor.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(cofactor)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (EC_GROUP_get_cofactor(group, cofactor, ctx) == 0) throw new OpenSslNativeException();
        }

        public static int GetOrderNumberOfBits(ECGroupHandle group)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            return EC_GROUP_order_bits(group);
        }

        public static ECPointHandle GetGenerator(ECGroupHandle group)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            var generator = EC_GROUP_get0_generator(group);
            if (generator.IsInvalid) throw new OpenSslNativeException();
            return generator;
        }

        public static void PrecomputeGeneratorMultiples(ECGroupHandle group, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (EC_GROUP_precompute_mult(group, ctx) == 0) throw new OpenSslNativeException();
        }

        public static bool HasPrecomputedGeneratorMultiples(ECGroupHandle group)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            return (EC_GROUP_have_precompute_mult(group) == 1);
        }

        public static int GetDegree(ECGroupHandle group)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            var result = EC_GROUP_get_degree(group);
            if (result == 0) throw new OpenSslNativeException();
            return result;
        }

        public static bool Compare(ECGroupHandle a, ECGroupHandle b, BigNumberContextHandle ctx)
        {
            Debug.Assert(!a.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(a)}>");
            Debug.Assert(!b.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(b)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            var result = EC_GROUP_cmp(a, b, ctx);
            if (result == -1) throw new OpenSslNativeException();
            else if (result == 0) return true;
            return false;
        }
#endregion

#region Memory Handling Methods
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr EC_GROUP_new_by_curve_name(int nid);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr EC_GROUP_new(); 

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void EC_GROUP_clear_free(IntPtr curve);
#endregion

        /// <summary>
        /// Allocates a new OpenSSL <c>EC_GROUP</c> structure for
        /// the curve indicated by <paramref name="nid"/> and
        /// returns a handle to it.
        ///
        /// The returned handle is guaranteed to point to be valid,
        /// i.e., point to a valid <c>EC_GROUP</c> structure.
        /// </summary>
        /// <returns>
        /// A valid <see cref="ECGroupHandle" /> pointing to a freshly allocated
        /// <c>EC_GROUP</c> structure.
        /// </returns>
        public static ECGroupHandle CreateByCurveNID(int nid)
        {
            var group = new ECGroupHandle(EC_GROUP_new_by_curve_name(nid), ownsHandle: true);
            if (group.IsInvalid) throw new OpenSslNativeException();
            return group;
        }

        /// <summary>
        /// An unintialized handle. A null pointer.
        /// </summary>
        public static ECGroupHandle Null = new ECGroupHandle();

        internal ECGroupHandle() : base(ownsHandle: false)
        {

        }

        /// <summary>
        /// Creates a new <see cref="ECGroupHandle" /> instance for
        /// the given raw <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle">A valid pointer to a OpenSSL <c>EC_GROUP</c> structure.</param>
        /// <param name="ownsHandle">
        /// If <c>true</c>, the referred <c>EC_GROUP</c> will be deleted from memory on disposal.
        /// </param>
        internal ECGroupHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
            if (IsInvalid)
            {
                throw new ArgumentException("received an invalid handle", nameof(handle), new OpenSslNativeException());
            }
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            Debug.Assert(!IsInvalid);
            EC_GROUP_clear_free(this.handle);
            return true;
        }
    }
}