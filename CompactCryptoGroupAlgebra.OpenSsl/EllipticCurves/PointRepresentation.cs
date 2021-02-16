namespace CompactCryptoGroupAlgebra.OpenSsl.EllipticCurves
{

    /// <summary>
    /// Specifies the encoding of a point into bytes.
    /// </summary>
    /// <remarks>
    /// The enum values correspond directly to OpenSSL.
    /// </remarks>
    public enum PointEncoding : int
    {
        /// <summary>
        /// The point is encoded as <c>z||x</c>, where the octet <c>z</c> specifies
        /// which solution of the quadratic equation <c>y</c> is.
        /// </summary>
        Compressed = 2,
        
        /// <summary>
        /// The point is encoded as <c>z||x||y</c>, where <c>z</c> is the octet <c>0x04</c>.
        /// </summary>
        Uncompressed = 4,

        /// <summary>
        /// The point is encoded as <c>z||x||y</c>, where the octet <c>z</c> specifies
        /// which solution of the quadratic equation <c>y</c>.
        /// </summary>
        Hybrid = 6
    }

    /// <summary>
    /// Utility functions to compute the bits requires for encoding elliptic curve points.
    /// </summary>
    public static class PointEncodingLength
    {
        /// <summary>
        /// Computes the bits required for a given encoding of an elliptic curve point.
        /// </summary>
        /// <returns>The integer bit length of the encoded points.</returns>
        /// <param name="encoding">The point encoding.</param>
        /// <param name="elementBitLength">The bit length elements in the underlying field of the curve.</param>
        public static int GetEncodingBitLength(PointEncoding encoding, int elementBitLength)
        {
            switch (encoding)
            {
                case PointEncoding.Compressed:
                    return elementBitLength + 8;
                default:
                    return 2 * elementBitLength + 8;
            }
        }
    }

}