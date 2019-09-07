using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public interface ICryptoGroup
    {
        ICryptoGroupElement Add(ICryptoGroupElement left, ICryptoGroupElement right);
        ICryptoGroupElement MultiplyScalar(ICryptoGroupElement element, BigInteger k);
        ICryptoGroupElement Negate(ICryptoGroupElement element);
        ICryptoGroupElement Generate(BigInteger index);
        ICryptoGroupElement NeutralElement { get; }
        ICryptoGroupElement Generator { get; }
        ICryptoGroupElement FromBytes(byte[] buffer);

        int OrderBitLength { get; }
        int OrderByteLength { get; }
        BigInteger Order { get; }
        int ElementBitLength { get; }
        int ElementByteLength { get; }
    }
}
