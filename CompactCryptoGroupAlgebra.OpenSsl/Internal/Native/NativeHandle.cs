using System;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
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
