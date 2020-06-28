using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Algebraic group based on multiplications in the finite field of a prime number <c>P</c>.
    /// 
    /// Through the <see cref="CryptoGroup{T}"/> interface, the addition represents
    /// multiplication of two integers modulo <c>P</c>, while scalar multiplication
    /// is exponentiation of an integer modulo <c>P</c>.
    /// </summary>
    public class MultiplicativeCryptoGroup : CryptoGroup<BigInteger>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultiplicativeCryptoGroup"/>
        /// given a <see cref="ICryptoGroupAlgebra{BigInteger}"/> which provides an
        /// implementation for underlying group operations and has already been
        /// initialized group parameters.
        /// </summary>
        /// <param name="algebra">The <see cref="ICryptoGroupAlgebra{BigInteger}"/> instance.</param>
        public MultiplicativeCryptoGroup(ICryptoGroupAlgebra<BigInteger> algebra) : base(algebra)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="MultiplicativeCryptoGroup"/>
        /// given its parameters.
        /// </summary>
        /// <param name="prime">The prime number which is the modulo of the finite field the group is based on.</param>
        /// <param name="order">The order of the group.</param>
        /// <param name="generator">A generator of the group.</param>
        public MultiplicativeCryptoGroup(BigInteger prime, BigInteger order, BigInteger generator)
          : this(new MultiplicativeGroupAlgebra(prime, order, generator))
        { }

        /// <inheritdoc/>
        protected override CryptoGroupElement<BigInteger> CreateGroupElement(BigInteger value)
        {
            return new CryptoGroupElement<BigInteger>(value, (MultiplicativeGroupAlgebra)Algebra);
        }

        /// <inheritdoc/>
        protected override CryptoGroupElement<BigInteger> CreateGroupElement(byte[] buffer)
        {
            return new CryptoGroupElement<BigInteger>(buffer, (MultiplicativeGroupAlgebra)Algebra);
        }
    }
}
