using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{

    [Flags]
    internal enum BigNumberFlags : int
    {
        None            = 0,
        Malloced        = 0x01,
        StaticData      = 0x02,
        ConstantTime    = 0x04,
        Secure          = 0x08
    }


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

        ///
        /// <remarks>Guaranteed to result in a valid handle if no exception.</remarks>
        public static BigNumberHandle Create()
        {
            var handle = new BigNumberHandle(BN_new(), ownsHandle: true);
            if (handle.IsInvalid) throw new OpenSslNativeException();
            return handle;
        }

        ///
        /// <remarks>Guaranteed to result in a valid handle if no exception.</remarks>
        public static BigNumberHandle CreateSecure()
        {
            var handle = new BigNumberHandle(BN_secure_new(), ownsHandle: true);
            if (handle.IsInvalid) throw new OpenSslNativeException();
            BN_set_flags(handle, BigNumberFlags.ConstantTime);
            return handle;
        }

        public static BigNumberHandle Null = new BigNumberHandle();

        internal BigNumberHandle() : base(ownsHandle: false)
        {

        }

        private BigNumberHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
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
            BN_clear_free(this.handle);
            return true;
        }

        
    }
}