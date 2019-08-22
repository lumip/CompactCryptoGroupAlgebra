using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public interface ICryptoGroupElement : IEquatable<ICryptoGroupElement>
    {
        void Add(ICryptoGroupElement other);
        void MultiplyScalar(BigInteger k);
        void Negate();
        byte[] ToBytes();
        ICryptoGroupElement Clone();
    }
}
