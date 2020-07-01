using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// A prime integer ring using BigIntegers.
    /// </summary>
    /// <remarks>
    /// Uses by elliptic curve implementations in <see cref="CurveGroupAlgebra"/>.
    /// </remarks>
    class BigIntegerRing
    {
        /// <summary>
        /// The prime that serves as module for operations.
        /// </summary>
        public BigInteger Modulo { get; }

        /// <summary>
        /// The maximal bit length of ring elements.
        /// 
        /// This is the number of bits required to represent any element of the group.
        /// </summary>
        public int ElementByteLength { get; }

        /// <summary>
        /// Instantiates a new BigIntegerRing given a prime modulo.
        /// </summary>
        /// <param name="primeModulo">The characteristic prime/modulo of the ring.</param>
        public BigIntegerRing(BigInteger primeModulo)
        {
            Modulo = primeModulo;
            ElementByteLength = primeModulo.ToByteArray().Length;
        }

        /// <summary>
        /// Exponentiates an integer within the ring.
        /// </summary>
        /// <param name="x">Number to exponentiate.</param>
        /// <param name="e">Exponent.</param>
        /// <returns>x to the power of e in the ring (i.e., modulo the ring's prime).</returns>
        public BigInteger Pow(BigInteger x, BigInteger e)
        {
            Debug.Assert(e >= BigInteger.Zero);
            return BigInteger.ModPow(Mod(x), e, Modulo);
        }

        /// <summary>
        /// Squares an integer within the ring.
        /// </summary>
        /// <param name="x">Number to square.</param>
        /// <returns>x squared in the ring (i.e., modulo the ring's prime).</returns>
        public BigInteger Square(BigInteger x)
        {
            return Pow(x, 2);
        }

        /// <summary>
        /// Multiplicatively inverts an element within the ring.
        /// 
        /// The multiplicative inverse for any x is the unique ring element e such that
        /// the product x*e equals 1 in the ring.
        /// </summary>
        /// <param name="x">Number to invert.</param>
        /// <returns>the multiplicative inverse of x in the ring.</returns>
        public BigInteger InvertMult(BigInteger x)
        {
            return Pow(x, Modulo - 2);
        }

        /// <summary>
        /// Finds the corresponding ring element for any integer.
        /// 
        /// Applies the modulo operation to map any given integer to the corresponding ring element.
        /// The result is guaranteed to be a positive integer in the ring (as opposed to the
        /// modulo operation of <see cref="BigInteger"/>, which is signed.
        /// </summary>
        /// <param name="x">Any number to be mapped on the ring.</param>
        /// <returns>The ring element corresponding to the input.</returns>
        public BigInteger Mod(BigInteger x)
        {
            return (x % Modulo + Modulo) % Modulo; // to prevent negative results of BigInteger modulo
        }
    }
}
