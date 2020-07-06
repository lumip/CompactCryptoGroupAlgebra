using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// A prime integer field using BigIntegers.
    /// </summary>
    /// <remarks>
    /// Used by elliptic curve implementations in <see cref="CurveGroupAlgebra"/>.
    /// </remarks>
    class BigIntegerField
    {
        /// <summary>
        /// The prime that serves as module for operations.
        /// </summary>
        public BigPrime Modulo { get; }

        /// <summary>
        /// The maximal bit length of field elements.
        /// 
        /// This is the number of bits required to represent any element of the group.
        /// </summary>
        public int ElementByteLength { get; }

        /// <summary>
        /// Instantiates a new BigIntegerfield given a prime modulo.
        /// </summary>
        /// <param name="primeModulo">The characteristic prime/modulo of the field.</param>
        public BigIntegerField(BigPrime primeModulo)
        {
            Modulo = primeModulo;
            ElementByteLength = ((BigInteger)primeModulo).ToByteArray().Length;
        }

        /// <summary>
        /// Exponentiates an integer within the field.
        /// </summary>
        /// <param name="x">Number to exponentiate.</param>
        /// <param name="e">Exponent.</param>
        /// <returns>x to the power of e in the field (i.e., modulo the field's prime).</returns>
        public BigInteger Pow(BigInteger x, BigInteger e)
        {
            if (e < BigInteger.Zero)
                throw new ArgumentException("Exponent cannot be negative!", nameof(e));
            return BigInteger.ModPow(Mod(x), e, Modulo);
        }

        /// <summary>
        /// Squares an integer within the field.
        /// </summary>
        /// <param name="x">Number to square.</param>
        /// <returns>x squared in the field (i.e., modulo the field's prime).</returns>
        public BigInteger Square(BigInteger x)
        {
            return Pow(x, 2);
        }

        /// <summary>
        /// Multiplicatively inverts an element within the field.
        /// 
        /// The multiplicative inverse for any x is the unique field element e such that
        /// the product x*e equals 1 in the field.
        /// </summary>
        /// <param name="x">Number to invert.</param>
        /// <returns>the multiplicative inverse of x in the field.</returns>
        public BigInteger InvertMult(BigInteger x)
        {
            return Pow(x, Modulo - 2);
        }

        /// <summary>
        /// Finds the corresponding field element for any integer.
        /// 
        /// Applies the modulo operation to map any given integer to the corresponding field element.
        /// The result is guaranteed to be a positive integer in the field (as opposed to the
        /// modulo operation of <see cref="BigInteger"/>, which is signed.
        /// </summary>
        /// <param name="x">Any number to be mapped on the field.</param>
        /// <returns>The field element corresponding to the input.</returns>
        public BigInteger Mod(BigInteger x)
        {
            return (x % Modulo + Modulo) % Modulo; // to prevent negative results of BigInteger modulo
        }

        public bool IsElement(BigInteger x)
        {
            return x >= 0 && x < Modulo;
        }
    }
}
