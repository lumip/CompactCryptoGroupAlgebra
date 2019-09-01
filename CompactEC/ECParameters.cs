using System.Numerics;

namespace CompactEC
{
    /// <summary>
    /// An eliptic curve y² = x³ + Ax + B over the finite field defined by P
    /// </summary>
    public struct ECParameters
    {
        public BigInteger P;
        public BigInteger A;
        public BigInteger B;
        public BigInteger Order;
        public ECPoint Generator;
    }
}
