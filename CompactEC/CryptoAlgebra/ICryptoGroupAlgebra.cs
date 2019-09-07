using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public interface ICryptoGroupAlgebra<E> where E : struct
    {
        BigInteger Order { get; }
        E Generator { get; }
        int ElementBitLength { get; }
        int OrderBitLength { get; }

        E GenerateElement(BigInteger index);
        E Negate(E e);
        E MultiplyScalar(E e, BigInteger k);
        E Add(E left, E right);
        bool IsValid(E element);
        E FromBytes(byte[] buffer);
        byte[] ToBytes(E element);
    }
}
