namespace CompactCryptoGroupAlgebra.OpenSsl
{

    /// <summary>
    /// Specifies the encoding of a point into bytes.
    /// </summary>
    /// <remarks>
    /// The enum values correspond directly to OpenSSL.
    /// </remarks>
    public enum PointEncoding : int
    {
        /** the point is encoded as z||x, where the octet z specifies
         *  which solution of the quadratic equation y is  */
        Compressed = 2,
        /** the point is encoded as z||x||y, where z is the octet 0x04  */
        Uncompressed = 4,
        /** the point is encoded as z||x||y, where the octet z specifies
         *  which solution of the quadratic equation y is  */
        Hybrid = 6
    }

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