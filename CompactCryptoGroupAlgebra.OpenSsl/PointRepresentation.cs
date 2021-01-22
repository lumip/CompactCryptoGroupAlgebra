namespace CompactCryptoGroupAlgebra.OpenSsl
{

    public enum PointRepresentation : int
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

}