using System.Numerics;
using System.Globalization;

namespace CompactCryptoGroupAlgebra.Tests.TestUtils
{
    public static class BigIntegerUtils
    {
        /// <summary>
        /// Parses a hex-formatted integer string as a <see cref="BigInteger" />.
        /// </summary>
        /// <param name="hexEncodedInteger">Hexadecimal string representation of the integer.</param>
        /// <returns>The <see cref="BigInteger"/> determined by the given hex string.</returns>
        public static BigInteger ParseHex(string hexEncodedInteger)
        {
            return BigInteger.Parse(hexEncodedInteger, NumberStyles.AllowHexSpecifier);
        } 
    }
}
