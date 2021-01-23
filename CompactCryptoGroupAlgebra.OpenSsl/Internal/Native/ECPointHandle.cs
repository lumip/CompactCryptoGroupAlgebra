using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{

    sealed class ECPointHandle : NativeHandle
    {

#region Native Methods Imports
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_copy(ECPointHandle dst, ECPointHandle src);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_get_affine_coordinates(ECGroupHandle group, ECPointHandle p, BigNumberHandle x, BigNumberHandle y, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_add(ECGroupHandle group, ECPointHandle r, ECPointHandle a, ECPointHandle b, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_mul(ECGroupHandle group, ECPointHandle r, BigNumberHandle n, ECPointHandle q, BigNumberHandle m, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_cmp(ECGroupHandle group, ECPointHandle a, ECPointHandle b, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_set_to_infinity(ECGroupHandle group, ECPointHandle point);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_is_on_curve(ECGroupHandle group, ECPointHandle point, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_invert(ECGroupHandle group, ECPointHandle a, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_point2oct(ECGroupHandle group, ECPointHandle p, PointEncoding form, byte[]? buffer, int bufferLen, BigNumberContextHandle contextHandle);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_oct2point(ECGroupHandle group, ECPointHandle p, byte[] buffer, int bufferLen, BigNumberContextHandle ctx);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static int EC_POINT_is_at_infinity(ECGroupHandle group, ECPointHandle p);
#endregion

#region Checked Native Methods
        public static void Copy(ECPointHandle to, ECPointHandle from)
        {
            Debug.Assert(!to.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(to)}>");
            Debug.Assert(!from.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(from)}>");
            if (EC_POINT_copy(to, from) == 0) throw new OpenSslNativeException();
        }

        public static void GetAffineCoordinates(ECGroupHandle group, ECPointHandle p, BigNumberHandle x, BigNumberHandle y, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!p.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(p)}>");
            Debug.Assert(!x.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(x)}>");
            Debug.Assert(!y.IsInvalid, $"Accessed an invalid BigNumberHandle! <{nameof(y)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (EC_POINT_get_affine_coordinates(group, p, x, y, ctx) == 0) throw new OpenSslNativeException();
        }

        public static void Add(ECGroupHandle group, ECPointHandle result, ECPointHandle a, ECPointHandle b, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!result.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(result)}>");
            Debug.Assert(!a.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(a)}>");
            Debug.Assert(!b.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(b)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (EC_POINT_add(group, result, a, b, ctx) == 0) throw new OpenSslNativeException();
        }

        public static void Multiply(ECGroupHandle group, ECPointHandle result, BigNumberHandle n, ECPointHandle q, BigNumberHandle m, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!result.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(result)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (EC_POINT_mul(group, result, n, q, m, ctx) == 0) throw new OpenSslNativeException();
        }

        public static void InvertInPlace(ECGroupHandle group, ECPointHandle p, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!p.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(p)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            if (EC_POINT_invert(group, p, ctx) == 0) throw new OpenSslNativeException();
        }

        public static bool Compare(ECGroupHandle group, ECPointHandle a, ECPointHandle b, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!a.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(a)}>");
            Debug.Assert(!b.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(b)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            var result = EC_POINT_cmp(group, a, b, ctx);
            if (result < 0) throw new OpenSslNativeException();
            return result == 0;
        }

        public static int ToByteBuffer(ECGroupHandle group, ECPointHandle p, PointEncoding form, byte[]? buffer, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!p.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(p)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            int bufferLen = 0;
            if (buffer != null)
            {
                bufferLen = buffer.Length;
            }
            bufferLen = EC_POINT_point2oct(group, p, form, buffer, bufferLen, ctx);
            if (bufferLen == 0) throw new OpenSslNativeException();
            return bufferLen;
        }

        public static void FromByteBuffer(ECGroupHandle group, ECPointHandle p, byte[] buffer, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            var result = EC_POINT_oct2point(group, p, buffer, buffer.Length, ctx);
            if (result == 0) throw new OpenSslNativeException();
        }

        public static void SetToInfinity(ECGroupHandle group, ECPointHandle p)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!p.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(p)}>");
            if (EC_POINT_set_to_infinity(group, p) == 0) throw new OpenSslNativeException();
        }

        public static bool IsOnCurve(ECGroupHandle group, ECPointHandle p, BigNumberContextHandle ctx)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!p.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(p)}>");
            Debug.Assert(!ctx.IsInvalid, $"Accessed an invalid BigNumberContextHandle! <{nameof(ctx)}>");
            var result = EC_POINT_is_on_curve(group, p, ctx);
            if (result < 0) throw new OpenSslNativeException();
            return result == 1;
        }

        public static bool IsAtInfinity(ECGroupHandle group, ECPointHandle p)
        {
            Debug.Assert(!group.IsInvalid, $"Accessed an invalid ECGroupHandle! <{nameof(group)}>");
            Debug.Assert(!p.IsInvalid, $"Accessed an invalid ECPointHandle! <{nameof(p)}>");
            var result = EC_POINT_is_at_infinity(group, p);
            return (result == 1);
        }
#endregion

#region Memory Handling Methods
        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr EC_POINT_new(ECGroupHandle group);

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static void EC_POINT_clear_free(IntPtr point);
#endregion

        ///
        /// <remarks>Guaranteed to result in a valid handle if no exception.</remarks>
        public static ECPointHandle Create(ECGroupHandle group)
        {
            var point = new ECPointHandle(EC_POINT_new(group), ownsHandle: true);
            if (point.IsInvalid) throw new OpenSslNativeException();
            return point;
        }

        public static ECPointHandle Null = new ECPointHandle();

        internal ECPointHandle() : base(ownsHandle: false)
        {

        }

        internal ECPointHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
            if (IsInvalid)
            {
                throw new ArgumentException("received an invalid handle", nameof(handle), new OpenSslNativeException());
            }
        }

        protected override bool ReleaseHandle()
        {
            EC_POINT_clear_free(this.handle);
            return true;
        }
    }
}