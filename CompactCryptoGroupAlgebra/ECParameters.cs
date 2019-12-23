using System.Numerics;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// A set of parameters of an eliptic curve y² = x³ + Ax + B over the finite field defined by prime P.
    /// </summary>
    public struct ECParameters
    {
        /// <summary>
        /// The prime P definining the finite field underlying the eliptic curve.
        /// </summary>
        public BigInteger P;

        /// <summary>
        /// The parameter A in the curve equation.
        /// </summary>
        public BigInteger A;

        /// <summary>
        /// The parameter B in the curve equation.
        /// </summary>
        public BigInteger B;

        /// <summary>
        /// The order of the defined eliptic curve.
        /// </summary>
        public BigInteger Order;

        /// <summary>
        /// A generator for the defined eliptic curve.
        /// </summary>
        public ECPoint Generator;

        /// <summary>
        /// Creates a parameter set for the NIST P-256 eliptic curve.
        /// </summary>
        /// <remarks>
        /// As defined in https://csrc.nist.gov/csrc/media/publications/fips/186/2/archive/2000-01-27/documents/fips186-2.pdf , p. 34.
        /// </remarks>
        /// <returns>An instance of ECParameters for the NIST P-256 curve.</returns>
        public static ECParameters CreateNISTP256()
        {
            var B = new BigInteger(new byte[] {
                0x4b, 0x60, 0xd2, 0x27, 0x3e, 0x3c, 0xce, 0x3b,
                0xf6, 0xb0, 0x53, 0xcc, 0xb0, 0x06, 0x1d, 0x65,
                0xbc, 0x86, 0x98, 0x76, 0x55, 0xbd, 0xeb, 0xb3,
                0xe7, 0x93, 0x3a, 0xaa, 0xd8, 0x35, 0xc6, 0x5a, 
            });

            var genX = new BigInteger(new byte[]
            {
                0x96, 0xc2, 0x98, 0xd8, 0x45, 0x39, 0xa1, 0xf4,
                0xa0, 0x33, 0xeb, 0x2d, 0x81, 0x7d, 0x03, 0x77,
                0xf2, 0x40, 0xa4, 0x63, 0xe5, 0xe6, 0xbc, 0xf8,
                0x47, 0x42, 0x2c, 0xe1, 0xf2, 0xd1, 0x17, 0x6b, 
            });
            var genY = new BigInteger(new byte[]
            {
                0xf5, 0x51, 0xbf, 0x37, 0x68, 0x40, 0xb6, 0xcb,
                0xce, 0x5e, 0x31, 0x6b, 0x57, 0x33, 0xce, 0x2b,
                0x16, 0x9e, 0x0f, 0x7c, 0x4a, 0xeb, 0xe7, 0x8e,
                0x9b, 0x7f, 0x1a, 0xfe, 0xe2, 0x42, 0xe3, 0x4f, 
            });
            return new ECParameters()
            {
                P = BigInteger.Parse("115792089210356248762697446949407573530086143415290314195533631308867097853951"),
                A = -3,
                B = B,
                Generator = new ECPoint(genX, genY),
                Order = BigInteger.Parse("115792089210356248762697446949407573529996955224135760342422259061068512044369")
            };
        }
    }
}
