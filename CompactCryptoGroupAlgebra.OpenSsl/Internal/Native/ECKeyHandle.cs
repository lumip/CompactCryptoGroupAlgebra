using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{

    sealed class ECKeyHandle : NativeHandle
    {

#region Native Methods Imports
        [DllImport("libcrypto", CallingConvention=CallingConvention.Cdecl)]
        private extern static int EC_KEY_set_private_key(ECKeyHandle key, BigNumberHandle prv);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_KEY_set_group(ECKeyHandle key, ECGroupHandle group);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static BigNumberHandle EC_KEY_get0_private_key(ECKeyHandle key);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static ECPointHandle EC_KEY_get0_public_key(ECKeyHandle key);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_KEY_generate_key(ECKeyHandle key);
#endregion

#region Checked Native Methods
        public static void SetPrivateKey(ECKeyHandle key, BigNumberHandle privateKey)
        {
            Debug.Assert(!key.IsInvalid, $"Accessed an invalid ECKeyHandle! <{nameof(key)}>");
            Debug.Assert(!key.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(privateKey)}>");
            if (EC_KEY_set_private_key(key, privateKey) == 0) throw new OpenSslNativeException();
        }

        public static void SetGroup(ECKeyHandle key, ECGroupHandle group)
        {
            Debug.Assert(!key.IsInvalid, $"Accessed an invalid ECKeyHandle! <{nameof(key)}>");
            Debug.Assert(!key.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            if (EC_KEY_set_group(key, group) == 0) throw new OpenSslNativeException();
        }

        public static BigNumberHandle GetPrivateKey(ECKeyHandle key)
        {
            Debug.Assert(!key.IsInvalid, $"Accessed an invalid ECKeyHandle! <{nameof(key)}>");
            return EC_KEY_get0_private_key(key);
        }

        public static ECPointHandle GetPublicKey(ECKeyHandle key)
        {
            Debug.Assert(!key.IsInvalid, $"Accessed an invalid ECKeyHandle! <{nameof(key)}>");
            return EC_KEY_get0_public_key(key);
        }

        public static void GenerateKey(ECKeyHandle key)
        {
            Debug.Assert(!key.IsInvalid, $"Accessed an invalid ECKeyHandle! <{nameof(key)}>");
            if (EC_KEY_generate_key(key) == 0) throw new OpenSslNativeException();
        }
#endregion

#region Memory Handling Methods
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr EC_KEY_new();

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void EC_KEY_free(IntPtr key);
#endregion

        ///
        /// <remarks>Guaranteed to result in a valid handle if no exception.</remarks>
        public static ECKeyHandle Create()
        {
            var key = new ECKeyHandle(EC_KEY_new(), ownsHandle: true);
            if (key.IsInvalid) throw new OpenSslNativeException();
            return key;
        }

        public static ECKeyHandle Null = new ECKeyHandle();

        internal ECKeyHandle() : base(ownsHandle: false)
        {

        }

        internal ECKeyHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
            if (IsInvalid)
            {
                throw new ArgumentException("received an invalid handle", nameof(handle), new OpenSslNativeException());
            }
        }

        protected override bool ReleaseHandle()
        {
            Debug.Assert(!IsInvalid);
            EC_KEY_free(this.handle);
            return true;
        }
    }
}