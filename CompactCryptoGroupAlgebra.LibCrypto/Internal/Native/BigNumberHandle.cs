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
    /// A handle for OpenSSL <c>BIGNUM</c> structures.
    /// </summary>
    sealed class BigNumberHandle : NativeHandle
    {

        #region Native Methods Imports

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_num_bits(BigNumberHandle a);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_set_word(BigNumberHandle a, ulong w);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static BigNumberHandle BN_copy(BigNumberHandle to, BigNumberHandle from);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void BN_set_flags(BigNumberHandle a, BigNumberFlags flags);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static BigNumberFlags BN_get_flags(BigNumberHandle a, BigNumberFlags flags);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_ucmp(BigNumberHandle a, BigNumberHandle b);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_bn2lebinpad(BigNumberHandle a, byte[] buffer, int bufferLen);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static BigNumberHandle BN_lebin2bn(byte[] buffer, int len, BigNumberHandle ret);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_mod_mul(BigNumberHandle r, BigNumberHandle a, BigNumberHandle b, BigNumberHandle m, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_mod_exp(BigNumberHandle r, BigNumberHandle a, BigNumberHandle p, BigNumberHandle m, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_priv_rand_range(BigNumberHandle rnd, BigNumberHandle range);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int BN_mod_exp_mont_consttime(BigNumberHandle result, BigNumberHandle a, BigNumberHandle exponent,
                              BigNumberHandle modulo, BigNumberContextHandle ctx, BigNumberMontgomeryContextHandle montCtx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static BigNumberHandle BN_mod_inverse(BigNumberHandle r, BigNumberHandle a, BigNumberHandle m, BigNumberContextHandle ctx);
        #endregion

        #region Checked Native Methods
        public static int GetNumberOfBits(BigNumberHandle x)
        {
            Debug.Assert(!x.IsInvalid, "Accessed an invalid BigNumberHandle");
            return BN_num_bits(x);
        }

        public static void SetWord(BigNumberHandle x, ulong w)
        {
            Debug.Assert(!x.IsInvalid, "Accessed an invalid BigNumberHandle");
            if (BN_set_word(x, w) == 0) throw new OpenSslNativeException();
        }

        public static BigNumberHandle Copy(BigNumberHandle from, BigNumberHandle to)
        {
            Debug.Assert(!from.IsInvalid, $"Accessed an invalid BigNumberHandle");
            BigNumberHandle result = BN_copy(from, to);
            if (result.IsInvalid) throw new OpenSslNativeException();
            return result;
        }

        public static void SetFlags(BigNumberHandle x, BigNumberFlags flags)
        {
            Debug.Assert(!x.IsInvalid, "Accessed an invalid BigNumberHandle");
            BN_set_flags(x, flags);
        }

        public static BigNumberFlags GetFlags(BigNumberHandle x, BigNumberFlags filter)
        {
            Debug.Assert(!x.IsInvalid, "Accessed an invalid BigNumberHandle");
            return BN_get_flags(x, filter);
        }

        public static BigNumberFlags GetFlags(BigNumberHandle x)
        {
            return GetFlags(x, BigNumberFlags.ConstantTime | BigNumberFlags.Malloced | BigNumberFlags.Secure | BigNumberFlags.StaticData);
        }

        public static int Compare(BigNumberHandle x, BigNumberHandle y)
        {
            Debug.Assert(!x.IsInvalid, "Accessed an invalid BigNumberHandle");
            Debug.Assert(!y.IsInvalid, "Accessed an invalid BigNumberHandle");
            return BN_ucmp(x, y);
        }

        public static void ToBytes(BigNumberHandle x, byte[] buffer)
        {
            Debug.Assert(!x.IsInvalid, "Accessed an invalid BigNumberHandle");
            int result = BN_bn2lebinpad(x, buffer, buffer.Length);
            if (result == -1) throw new OpenSslNativeException();
        }

        public static BigNumberHandle FromBytes(byte[] buffer, BigNumberHandle target)
        {
            var result = BN_lebin2bn(buffer, buffer.Length, target);
            if (result.IsInvalid) throw new OpenSslNativeException();
            return result;
        }

        public static void ModMul(BigNumberHandle r, BigNumberHandle a, BigNumberHandle b, BigNumberHandle m, BigNumberContextHandle ctx)
        {
            Debug.Assert(!r.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(r)}>");
            Debug.Assert(!a.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(a)}>");
            Debug.Assert(!b.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(b)}>");
            Debug.Assert(!m.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(m)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (BN_mod_mul(r, a, b, m, ctx) == 0) throw new OpenSslNativeException();
        }

        public static void ModExp(BigNumberHandle r, BigNumberHandle a, BigNumberHandle e, BigNumberHandle m, BigNumberContextHandle ctx)
        {
            Debug.Assert(!r.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(r)}>");
            Debug.Assert(!a.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(a)}>");
            Debug.Assert(!e.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(e)}>");
            Debug.Assert(!m.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(m)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (BN_mod_exp(r, a, e, m, ctx) == 0) throw new OpenSslNativeException();
        }

        public static void SecureModExp(BigNumberHandle r, BigNumberHandle a, BigNumberHandle e, BigNumberHandle m, BigNumberContextHandle ctx)
        {
            Debug.Assert(!r.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(r)}>");
            Debug.Assert(!a.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(a)}>");
            Debug.Assert(!e.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(e)}>");
            Debug.Assert(!m.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(m)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (BN_mod_exp_mont_consttime(r, a, e, m, ctx, BigNumberMontgomeryContextHandle.Null) == 0) throw new OpenSslNativeException();
        }

        public static void SecureRandom(BigNumberHandle rnd, BigNumberHandle range)
        {
            Debug.Assert(!rnd.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(rnd)}>");
            Debug.Assert(!range.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(range)}>");
            if (BN_priv_rand_range(rnd, range) == 0) throw new OpenSslNativeException();
        }

        public static void ModInverse(BigNumberHandle r, BigNumberHandle a, BigNumberHandle m, BigNumberContextHandle ctx)
        {
            Debug.Assert(!r.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(r)}>");
            Debug.Assert(!a.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(a)}>");
            Debug.Assert(!m.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(m)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            var result = BN_mod_inverse(r, a, m, ctx);
            if (result.IsInvalid) throw new OpenSslNativeException();
        }
        #endregion

        #region Native Memory Handling
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr BN_new();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr BN_secure_new();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void BN_clear_free(IntPtr val);
        #endregion

        /// <summary>
        /// Allocates a new OpenSSL <c>BIGNUM</c> structure
        /// and returns a handle to it.
        ///
        /// The returned handle is guaranteed to point to be valid,
        /// i.e., point to a valid <c>BIGNUM</c> structure.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BigNumberHandle" /> pointing to a
        /// freshly allocated <c>BIGNUM</c> structure.
        /// </returns>
        public static BigNumberHandle Create()
        {
            var handle = new BigNumberHandle(BN_new(), ownsHandle: true);
            if (handle.IsInvalid) throw new OpenSslNativeException();
            return handle;
        }

        /// <summary>
        /// Allocates a new OpenSSL <c>BIGNUM</c> structure
        /// in OpenSSL secure heap and returns a handle to it.
        ///
        /// The returned handle is guaranteed to point to be valid,
        /// i.e., point to a valid <c>BIGNUM</c> structure with
        /// <c>BN_FLG_SECURE</c> and <c>BN_FLG_CONSTTIME</c> flags set.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BigNumberHandle" /> pointing to a
        /// freshly allocated <c>BIGNUM</c> structure.
        /// </returns>
        public static BigNumberHandle CreateSecure()
        {
            var handle = new BigNumberHandle(BN_secure_new(), ownsHandle: true);
            if (handle.IsInvalid) throw new OpenSslNativeException();
            BN_set_flags(handle, BigNumberFlags.ConstantTime);
            return handle;
        }

        /// <summary>
        /// An unintialized handle. A null pointer.
        /// </summary>
        public static BigNumberHandle Null = new BigNumberHandle();

        internal BigNumberHandle() : base(ownsHandle: false)
        {

        }

        /// <summary>
        /// Creates a new <see cref="BigNumberHandle" /> instance for
        /// the given raw <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle">A valid pointer to a OpenSSL <c>BIGNUM</c> structure.</param>
        /// <param name="ownsHandle">
        /// If <c>true</c>, the referred <c>BIGNUM</c> will be deleted from memory on disposal.
        /// </param>
        private BigNumberHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
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
            BN_clear_free(this.handle);
            return true;
        }


    }
}
