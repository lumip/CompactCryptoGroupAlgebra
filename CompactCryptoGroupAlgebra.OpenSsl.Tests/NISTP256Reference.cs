using System.Numerics;

namespace CompactCryptoGroupAlgebra.OpenSsl
{

    class NISTP256Reference
    {
        // Curve parameters, Appendix D: https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.186-4.pdf

        public static BigPrime Prime = BigPrime.CreateWithoutChecks(BigInteger.Parse("0115792089210356248762697446949407573530086143415290314195533631308867097853951", System.Globalization.NumberStyles.Integer));

        public static BigPrime Order = BigPrime.CreateWithoutChecks(BigInteger.Parse("0115792089210356248762697446949407573529996955224135760342422259061068512044369", System.Globalization.NumberStyles.Integer));

        public static BigInteger generatorX = BigInteger.Parse("06B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", System.Globalization.NumberStyles.HexNumber);

        public static BigInteger generatorY = BigInteger.Parse("04FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", System.Globalization.NumberStyles.HexNumber);

        public static BigInteger Cofactor = BigInteger.One;

        public static int ElementBitLength = NumberLength.GetLength(Prime).InBits + 1;

        public static int OrderBitLength = NumberLength.GetLength(Order).InBits;
    }
}