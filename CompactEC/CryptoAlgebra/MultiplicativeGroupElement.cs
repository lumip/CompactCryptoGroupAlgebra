using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public class MultiplicativeGroupElement : CryptoGroupElement<BigInteger>
    {
        public MultiplicativeGroupElement(BigInteger value, MultiplicativeGroupAlgebra groupAlgebra)
            : base(value, groupAlgebra)
        { }

        public MultiplicativeGroupElement(MultiplicativeGroupElement other)
            : this(other.Value, (MultiplicativeGroupAlgebra)other.Algebra)
        { }

        public MultiplicativeGroupElement(byte[] buffer, MultiplicativeGroupAlgebra groupAlgebra)
            : base(new BigInteger(buffer), groupAlgebra)
        { }

        public static MultiplicativeGroupElement FromBytes(byte[] buffer, MultiplicativeGroupAlgebra groupAlgebra)
        {
            return new MultiplicativeGroupElement(new BigInteger(buffer), groupAlgebra);
        }

        public override ICryptoGroupElement Clone()
        {
            return new MultiplicativeGroupElement(this);
        }

        public override byte[] ToBytes()
        {
            return Value.ToByteArray();
        }
    }
}
