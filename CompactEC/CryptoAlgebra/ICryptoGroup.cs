using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;

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
        Tuple<BigInteger, ICryptoGroupElement> GenerateRandom(RandomNumberGenerator rng);

        int OrderBitLength { get; }
        int OrderByteLength { get; }
        BigInteger Order { get; }
        int ElementBitLength { get; }
        int ElementByteLength { get; }
    }
}
