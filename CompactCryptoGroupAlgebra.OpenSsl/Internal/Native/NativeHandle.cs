using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl
{
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

        public override bool IsInvalid
        {
            get
            {
                return (handle == IntPtr.Zero || handle == new IntPtr(-1));
            }
        }

    }
}
