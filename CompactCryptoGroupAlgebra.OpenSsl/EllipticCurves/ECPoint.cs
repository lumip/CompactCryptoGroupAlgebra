using System;
using System.Numerics;
using System.Diagnostics;
using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

namespace CompactCryptoGroupAlgebra.OpenSsl.EllipticCurves
{

    /// <summary>
    /// A point on an elliptic curve.
    /// 
    /// A point on an elliptic curve is a two-dimensional point with integer coordinates
    /// from the (finite) field underlying the curve or lies at infinity.
    /// </summary>
    /// <remarks>
    /// This is essentially a managed code wrapper class for OpenSSL's <c>EC_POINT</c> structure.
    /// </remarks>
    public sealed class ECPoint : IDisposable
    {
        internal ECPointHandle Handle
        {
            get;
            private set;
        }

        private ECGroupHandle _ecGroup;

        /// <summary>
        /// Creates a new default <see cref="ECPoint" />
        /// on the curve defined by a valid <see cref="ECGroupHandle" />.
        ///
        /// The coordinates of the point are unspecified.
        /// </summary>
        /// <param name="ecGroupHandle">Native handle for the group the point lies on.</param>
        internal ECPoint(ECGroupHandle ecGroupHandle)
        {
            Debug.Assert(!ecGroupHandle.IsInvalid);
            _ecGroup = ecGroupHandle;
            Handle = ECPointHandle.Create(_ecGroup);
        }

        /// <summary>
        /// Creates a new <see cref="ECPoint" /> instance for a given
        /// valid <see cref="ECPointHandle" />
        /// on the curve defined by a valid <see cref="ECGroupHandle" />.
        ///
        /// The structure pointed to by <paramref name="pointHandle"/> is copied into
        /// this <see cref="ECPoint" /> instance.
        /// </summary>
        /// <param name="ecGroupHandle">Native handle for the group the point lies on.</param>
        /// <param name="pointHandle">Native handle for the point on the curve.</param>
        internal ECPoint(ECGroupHandle ecGroupHandle, ECPointHandle pointHandle) : this(ecGroupHandle)
        {
            Debug.Assert(!pointHandle.IsInvalid);
            ECPointHandle.Copy(Handle, pointHandle);
        }

        /// <summary>
        /// Returns the affine coordinates of the curve point.
        /// </summary>
        /// <returns>Tuple containing x- and y-coordinate.</returns>
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

        /// <summary>
        /// Whether this point is a point at infinity.
        /// </summary>
        public bool IsAtInfinity
        {
            get
            {
                return ECPointHandle.IsAtInfinity(_ecGroup, Handle);
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            ECPoint? other = obj as ECPoint;
            if (other == null) return false;

            using (var ctx = BigNumberContextHandle.Create())
            {
                return ECPointHandle.Compare(_ecGroup, this.Handle, other.Handle, ctx);
            }
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return new BigInteger(ToBytes()).GetHashCode();
        }

        /// <summary>
        /// Encodes the curve point in a byte buffer.
        /// </summary>
        /// <param name="encoding">The encoding scheme used to encode the point in the buffer.</param>
        /// <returns>Byte buffer containing the encoded curve point.</returns>
        public byte[] ToBytes(PointEncoding encoding = PointEncoding.Compressed)
        {
            using (var ctx = BigNumberContextHandle.Create())
            { 
                int requiredBufferLength = ECPointHandle.ToByteBuffer(_ecGroup, Handle, encoding, null, ctx);
                byte[] buffer = new byte[requiredBufferLength];
                int writtenBytes = ECPointHandle.ToByteBuffer(_ecGroup, Handle, encoding, buffer, ctx);
                Debug.Assert(writtenBytes == requiredBufferLength);
                return buffer;
            }
        }

        /// <summary>
        /// Decodes a <see cref="ECPoint" /> instance from a byte buffer.
        /// </summary>
        /// <param name="ecGroupHandle">Native handle for the group the point lies on.</param>
        /// <param name="buffer">Byte array containing an encoding of the curve point.</param>
        /// <returns><see cref="ECPoint" /> instance of the point encoded in the buffer.</returns>
        internal static ECPoint CreateFromBytes(ECGroupHandle ecGroupHandle, byte[] buffer)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var point = new ECPoint(ecGroupHandle);
                ECPointHandle.FromByteBuffer(point._ecGroup, point.Handle, buffer, ctx);
                return point;
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
            }

        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
