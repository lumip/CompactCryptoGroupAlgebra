namespace CompactCryptoGroupAlgebra.OpenSsl
{

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