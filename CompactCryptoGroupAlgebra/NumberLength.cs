using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// The lengths of a number in binary representation.
    /// </summary>
    public readonly struct NumberLength
    {
        /// <summary>
        /// Returns the length of the number in bits.
        /// </summary>
        /// <value>The length of the number in bits.</value>
        public int InBits { get; }

        /// <summary>
        /// Returns the length of the number in bytes.
        /// 
        /// If the length in bits is not a full byte multiple, the length
        /// in bytes is rounded up to the next full byte.
        /// </summary>
        /// <value>The length of the number in bytes.</value>
        /// <remarks>
        /// It holds that ByteLength / 8 >= BitLength
        /// </remarks>
        public int InBytes
        {
            get
            {
                return (InBits >> 3) +
                    ((InBits & 1) | (InBits >> 1 & 1) | (InBits >> 2 & 1));
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NumberLength"/> with
        /// a given bit length.
        /// </summary>
        /// <param name="bitLength">Bit length.</param>
        private NumberLength(int bitLength)
        {
            InBits = bitLength;
        }

        /// <summary>
        /// Returns the length for the given <see cref="BigInteger"/>.
        /// </summary>
        /// <returns>The length of <paramref name="x"/>.</returns>
        /// <param name="x"><see cref="BigInteger"/> to return the length for.</param>
        public static NumberLength GetLength(BigInteger x)
        {
            if (x.IsZero)
                return new NumberLength(0);
            return new NumberLength((int)Math.Floor(BigInteger.Log(x, 2) + 1));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NumberLength"/> for 
        /// a given byte length.
        /// </summary>
        /// <returns>The byte length.</returns>
        /// <param name="byteLength">Byte length.</param>
        public static NumberLength FromByteLength(int byteLength)
        {
            return new NumberLength(byteLength << 3);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NumberLength"/> with
        /// a given bit length.
        /// </summary>
        /// <param name="bitLength">Bit length.</param>
        public static NumberLength FromBitLength(int bitLength)
        {
            return new NumberLength(bitLength);
        }
    }
}
