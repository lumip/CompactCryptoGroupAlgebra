using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public class MultiplicativeCryptoGroup : CryptoGroup<BigInteger>
    {
        public MultiplicativeCryptoGroup(MultiplicativeGroupAlgebra algebra) : base(algebra)
        { }

        public MultiplicativeCryptoGroup(BigInteger prime, BigInteger order, BigInteger generator, int groupElementSize, int orderSize)
          : this(new MultiplicativeGroupAlgebra(prime, order, generator, groupElementSize, orderSize))
        { }

        protected override ICryptoGroupElement CreateGroupElement(BigInteger value)
        {
            return new MultiplicativeGroupElementImplementation(value, (MultiplicativeGroupAlgebra)Algebra);
        }

        protected override ICryptoGroupElement CreateGroupElement(byte[] buffer)
        {
            return new MultiplicativeGroupElementImplementation(buffer, (MultiplicativeGroupAlgebra)Algebra);
        }
    }
}
