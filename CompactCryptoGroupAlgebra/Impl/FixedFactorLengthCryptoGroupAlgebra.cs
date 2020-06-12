using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra
{
    // todo: problem: CryptoGroup.GenerateRandom does not work with FixedFactorLengthCryptoGroupAlgebra 

    /// <summary>
    /// An <see cref="ICryptoGroupAlgebra{E}"/> variants that only allows
    /// multiplication with scalar factors - and thus generation with indices -
    /// of a predefined bitlength.
    /// 
    /// This serves the purpose of allowing for more efficient group implementations
    /// by allowing the multiplication to stop early. Since all factors are of
    /// the same bitlength, side-channel resistance is maintained.
    /// </summary>
    public class FixedFactorLengthCryptoGroupAlgebra<E> : ICryptoGroupAlgebra<E> where E : struct
    {
        private CryptoGroupAlgebra<E> _baseAlgebra;

        /// <summary>
        /// Initializes a new instance of <see cref="FixedFactorLengthCryptoGroupAlgebra{E}"/>.
        /// </summary>
        /// <param name="baseAlgebra">The <see cref="CryptoGroupAlgebra{E}"/> instance to decorate to fixed-length factors.</param>
        /// <param name="fixedFactorBitLength">Permissible bit length of scalar factors.</param>
        public FixedFactorLengthCryptoGroupAlgebra(CryptoGroupAlgebra<E> baseAlgebra, int fixedFactorBitLength)
        {
            if (baseAlgebra == null)
                throw new ArgumentNullException(nameof(baseAlgebra));
            _baseAlgebra = baseAlgebra;

            if (fixedFactorBitLength <= 0 || fixedFactorBitLength > _baseAlgebra.OrderBitLength)
                throw new ArgumentOutOfRangeException("Factor bit length must be a positive integer not larger than the group order.", nameof(fixedFactorBitLength));
            FactorBitLength = fixedFactorBitLength;
        }

        /// <inheritdoc/>
        public BigInteger Order => _baseAlgebra.Order;

        /// <inheritdoc/>
        public E Generator => _baseAlgebra.Generator;

        /// <inheritdoc/>
        public E NeutralElement => _baseAlgebra.NeutralElement;

        /// <inheritdoc/>
        public int ElementBitLength => _baseAlgebra.ElementBitLength;

        /// <inheritdoc/>
        public int OrderBitLength => _baseAlgebra.OrderBitLength;

        /// <inheritdoc/>
        public int FactorBitLength { get; }

        /// <inheritdoc/>
        public E Add(E left, E right)
        {
            return _baseAlgebra.Add(left, right);
        }

        /// <inheritdoc/>
        public E FromBytes(byte[] buffer)
        {
            return _baseAlgebra.FromBytes(buffer);
        }

        /// <inheritdoc/>
        public E GenerateElement(BigInteger index)
        {
            int indexBitLength = CryptoGroupAlgebra<E>.GetBitLength(index);
            if (indexBitLength != FactorBitLength)
                throw new ArgumentException("Index does have an incorrect bit length!");
            return _baseAlgebra.MultiplyScalar(Generator, index, FactorBitLength);
        }

        /// <inheritdoc/>
        public bool IsValid(E element)
        {
            return _baseAlgebra.IsValid(element);
        }

        /// <inheritdoc/>
        public E MultiplyScalar(E e, BigInteger k)
        {
            int factorBitLength = CryptoGroupAlgebra<E>.GetBitLength(k);
            if (factorBitLength != FactorBitLength)
                throw new ArgumentException("Index does have an incorrect bit length!");
            return _baseAlgebra.MultiplyScalar(e, k, FactorBitLength);
        }

        /// <inheritdoc/>
        public E Negate(E e)
        {
            return _baseAlgebra.Negate(e);
        }

        /// <inheritdoc/>
        public byte[] ToBytes(E element)
        {
            return _baseAlgebra.ToBytes(element);
        }
    }
}
