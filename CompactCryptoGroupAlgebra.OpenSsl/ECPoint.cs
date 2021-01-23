using System;
using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;
using CompactCryptoGroupAlgebra.OpenSsl.Internal;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.OpenSsl
{

    public class ECPoint : IDisposable
    {
        internal ECPointHandle Handle
        {
            get;
            private set;
        }

        private ECGroupHandle _ecGroup;

        internal ECPoint(ECGroupHandle ecGroupHandle)
        {
            Debug.Assert(!ecGroupHandle.IsInvalid);
            _ecGroup = ecGroupHandle;
            Handle = ECPointHandle.Create(_ecGroup);
        }

        internal ECPoint(ECGroupHandle ecGroupHandle, ECPointHandle pointHandle) : this(ecGroupHandle)
        {
            Debug.Assert(!pointHandle.IsInvalid);
            ECPointHandle.Copy(Handle, pointHandle);
        }

        public (BigNumber, BigNumber) GetCoordinates()
        {
            if (IsAtInfinity)
            {
                throw new InvalidOperationException("Cannot get coordinates from point at infinity!");
            }
            using (var ctx = BigNumberContextHandle.Create())
            {
                var x = new BigNumber();
                var y = new BigNumber();
                ECPointHandle.GetAffineCoordinates(_ecGroup, Handle, x.Handle, y.Handle, ctx);
                return (x, y);
            }
        }

        public bool IsAtInfinity
        {
            get
            {
                return ECPointHandle.IsAtInfinity(_ecGroup, Handle);
            }
        }

        public override bool Equals(object? obj)
        {
            ECPoint? other = obj as ECPoint;
            if (other == null) return false;

            using (var ctx = BigNumberContextHandle.Create())
            {
                return ECPointHandle.Compare(_ecGroup, this.Handle, other.Handle, ctx);
            }
        }
        
        public override int GetHashCode()
        {
            return new BigInteger(ToBytes()).GetHashCode();
        }

        public byte[] ToBytes(PointEncoding representation = PointEncoding.Compressed)
        {
            using (var ctx = BigNumberContextHandle.Create())
            { 
                int requiredBufferLength = ECPointHandle.ToByteBuffer(_ecGroup, Handle, representation, null, ctx);
                byte[] buffer = new byte[requiredBufferLength];
                int writtenBytes = ECPointHandle.ToByteBuffer(_ecGroup, Handle, representation, buffer, ctx);
                Debug.Assert(writtenBytes == requiredBufferLength);
                return buffer;
            }
        }

        internal static ECPoint CreateFromBytes(ECGroupHandle group, byte[] buffer)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var point = new ECPoint(group);
                ECPointHandle.FromByteBuffer(point._ecGroup, point.Handle, buffer, ctx);
                return point;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
            }

        }

        /// <inheritdocs />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
