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