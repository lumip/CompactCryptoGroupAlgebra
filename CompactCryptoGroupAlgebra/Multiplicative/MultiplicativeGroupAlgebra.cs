// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography
// Copyright (C) 2020  Lukas <lumip> Prediger
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.Multiplicative
{
    /// <summary>
    /// Algebraic group operations based on multiplications in the finite field of a prime number <c>P</c>.
    /// 
    /// Through the <see cref="CryptoGroup{BigInteger, BigInteger}"/> interface, the addition
    /// represents multiplication of two integers modulo <c>P</c>, while scalar multiplication
    /// is exponentiation of an integer modulo <c>P</c>.
    /// </summary>
    public class MultiplicativeGroupAlgebra : CryptoGroupAlgebra<BigInteger>
    {
        /// <summary>
        /// Accessor to the prime modulo defining the underlying number field.
        /// </summary>
        /// <value>The prime modulo of the group.</value>
        public BigPrime Prime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplicativeGroupAlgebra"/> class
        /// given the group's parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public MultiplicativeGroupAlgebra(BigPrime prime, BigPrime order, BigInteger generator)
            : base(generator, order, (prime - 1) / order, BigInteger.One, NumberLength.GetLength(prime).InBits)
        {
            Prime = prime;
            if (!IsElement(generator))
                throw new ArgumentException("The generator must be an element of the group.", nameof(generator));
        }

        /// <inheritdoc/>
        public override BigInteger Add(BigInteger left, BigInteger right)
        {
            return (left * right) % Prime;
        }

        /// <inheritdoc/>
        protected override bool IsElementDerived(BigInteger element)
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
            return other is MultiplicativeGroupAlgebra algebra &&
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

        /// <summary>
        /// Creates a <see cref="CryptoGroup{BigInteger, BigInteger}" /> instance using a <see cref="MultiplicativeGroupAlgebra" />
        /// instance with the given parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public static CryptoGroup<BigInteger, BigInteger> CreateCryptoGroup(BigPrime prime, BigPrime order, BigInteger generator)
        {
            return new CryptoGroup<BigInteger, BigInteger>(new MultiplicativeGroupAlgebra(
                prime, order, generator
            ));
        }
        
    }
}
