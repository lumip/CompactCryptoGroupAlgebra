using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public class MultiplicativeGroupElementImplementation : CryptoGroupElementImplementation<BigInteger>
    {
        public MultiplicativeGroupElementImplementation(BigInteger value, MultiplicativeGroupAlgebra groupAlgebra)
            : base(value, groupAlgebra)
        { }

        public MultiplicativeGroupElementImplementation(MultiplicativeGroupElementImplementation other)
            : this(other.Value, (MultiplicativeGroupAlgebra)other.Algebra)
        { }

        public MultiplicativeGroupElementImplementation(byte[] buffer, MultiplicativeGroupAlgebra groupAlgebra)
            : base(new BigInteger(buffer), groupAlgebra)
        { }

        public static MultiplicativeGroupElementImplementation FromBytes(byte[] buffer, MultiplicativeGroupAlgebra groupAlgebra)
        {
            return new MultiplicativeGroupElementImplementation(new BigInteger(buffer), groupAlgebra);
        }

        public override ICryptoGroupElement Clone()
        {
            return new MultiplicativeGroupElementImplementation(this);
        }

        public override byte[] ToBytes()
        {
            return Value.ToByteArray();
        }
    }
}
