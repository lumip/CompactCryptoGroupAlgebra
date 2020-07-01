using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.Multiplicative
{
    /// <summary>
    /// Algebraic group operations based on multiplications in the finite field of a prime number <c>P</c>.
    /// 
    /// Through the <see cref="CryptoGroup{T}"/> interface, the addition represents
    /// multiplication of two integers modulo <c>P</c>, while scalar multiplication
    /// is exponentiation of an integer modulo <c>P</c>.
    /// </summary>
    public class MultiplicativeGroupAlgebra : CryptoGroupAlgebra<BigInteger>
    {
        /// <summary>
        /// Accessor to the prime modulo definining the underlying number field.
        /// </summary>
        /// <value>The prime modulo of the group.</value>
        public BigPrime Prime { get; }

        /// <inheritdoc/>
        public override BigInteger NeutralElement { get { return BigInteger.One; } }

        /// <inheritdoc/>
        public override BigInteger Cofactor
        {
            get
            {
                return (Prime - 1) / Order;
            }
        }

        /// <inheritdoc/>
        public override int ElementBitLength { get { return NumberLength.GetLength(Prime).InBits; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplicativeGroupAlgebra"/> class
        /// given the group's parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public MultiplicativeGroupAlgebra(BigPrime prime, BigPrime order, BigInteger generator)
            : base(generator, order)
        {
            Prime = prime;
            if (!IsValid(generator))
                throw new ArgumentException("The generator must be an element of the group.", nameof(generator));
        }

        /// <inheritdoc/>
        public override BigInteger Add(BigInteger left, BigInteger right)
        {
            return (left * right) % Prime;
        }

        /// <inheritdoc/>
        protected override BigInteger Multiplex(BigInteger selection, BigInteger left, BigInteger right)
        {
            Debug.Assert(selection == BigInteger.Zero || selection == BigInteger.One);
            return left + selection * (right - left);
        }

        /// <inheritdoc/>
        public override BigInteger Negate(BigInteger e)
        {
            return BigInteger.ModPow(e, Order - 1, Prime);
        }

        /// <inheritdoc/>
        protected override bool IsValidDerived(BigInteger element)
        {
            return (element > BigInteger.Zero && element < Prime);
        }

        /// <inheritdoc/>
        public override BigInteger FromBytes(byte[] buffer)
        {
            return new BigInteger(buffer);
        }

        /// <inheritdoc/>
        public override byte[] ToBytes(BigInteger element)
        {
            return element.ToByteArray();
        }

        /// <inheritdoc/>
        public override bool Equals(CryptoGroupAlgebra<BigInteger>? other)
        {
            var algebra = other as MultiplicativeGroupAlgebra;
            return algebra != null &&
                   Prime.Equals(algebra.Prime) &&
                   Order.Equals(algebra.Order) &&
                   Generator.Equals(algebra.Generator);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 630028201;
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Prime);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Order);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Generator);
            return hashCode;
        }
    }
}
