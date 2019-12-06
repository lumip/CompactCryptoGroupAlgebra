using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC
{
    public class MultiplicativeCryptoGroup : CryptoGroup<BigInteger>
    {
        public MultiplicativeCryptoGroup(MultiplicativeGroupAlgebra algebra) : base(algebra)
        { }

        public MultiplicativeCryptoGroup(BigInteger prime, BigInteger order, BigInteger generator)
          : this(new MultiplicativeGroupAlgebra(prime, order, generator))
        { }

        protected override CryptoGroupElement<BigInteger> CreateGroupElement(BigInteger value)
        {
            return new CryptoGroupElement<BigInteger>(value, (MultiplicativeGroupAlgebra)Algebra);
        }

        protected override CryptoGroupElement<BigInteger> CreateGroupElement(byte[] buffer)
        {
            return new CryptoGroupElement<BigInteger>(buffer, (MultiplicativeGroupAlgebra)Algebra);
        }
    }
}
