using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public class MultiplicativeGroupAlgebra : CryptoGroupAlgebra<BigInteger>
    {
        public BigInteger Prime { get; }

        public override BigInteger NeutralElement { get { return BigInteger.One; } }

        public MultiplicativeGroupAlgebra(BigInteger prime, BigInteger order, BigInteger generator, int groupElementSize, int orderSize)
            : base(generator, order, groupElementSize, orderSize)
        {
            Prime = prime;
        }

        public override BigInteger Add(BigInteger left, BigInteger right)
        {
            return (left * right) % Prime;
        }

        protected override BigInteger Multiplex(BigInteger selection, BigInteger left, BigInteger right)
        {
            return left + selection * (right - left);
        }

        public override BigInteger Negate(BigInteger e)
        {
            return BigInteger.ModPow(e, Order - 1, Prime);
        }

        public override bool IsValid(BigInteger element)
        {
            return element >= BigInteger.Zero && element < Prime;
        }
    }
}
