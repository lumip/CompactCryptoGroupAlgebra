using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public struct MultiplicativeGroupParameters
    {
        public BigInteger Prime;
        public BigInteger Generator;
        public BigInteger Order;
    }

    public class MultiplicativeGroupAlgebra : CryptoGroupAlgebra<BigInteger>
    {
        public BigInteger Prime { get; }
        public override BigInteger Order { get; }
        public override BigInteger NeutralElement { get { return BigInteger.One; } }
        public override BigInteger Generator { get; }
        public override int GroupElementBitlength { get { return GetBitLength(Prime); } }

        public MultiplicativeGroupAlgebra(BigInteger prime, BigInteger order, BigInteger generator)
            : base()
        {
            Prime = prime;
            if (!IsValid(generator))
                throw new ArgumentException("The generator must be an element of the group.", nameof(generator));
            Generator = generator;
            Order = order;
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
            return element > BigInteger.Zero && element < Prime;
        }

        public override BigInteger FromBytes(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public override byte[] ToBytes(BigInteger element)
        {
            throw new NotImplementedException();
        }
    }
}
