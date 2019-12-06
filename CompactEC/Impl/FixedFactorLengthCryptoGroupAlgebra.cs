using System;
using System.Numerics;

namespace CompactEC
{
    public class FixedFactorLengthCryptoGroupAlgebra<E> : ICryptoGroupAlgebra<E> where E : struct
    {
        private CryptoGroupAlgebra<E> _baseAlgebra;

        public FixedFactorLengthCryptoGroupAlgebra(CryptoGroupAlgebra<E> baseAlgebra, int fixedFactorBitLength)
        {
            if (baseAlgebra == null)
                throw new ArgumentNullException(nameof(baseAlgebra));
            _baseAlgebra = baseAlgebra;

            if (fixedFactorBitLength <= 0 || fixedFactorBitLength > _baseAlgebra.OrderBitLength)
                throw new ArgumentOutOfRangeException("Factor bit length must be a positive integer not larger than the group order.", nameof(fixedFactorBitLength));
            FactorBitLength = fixedFactorBitLength;
        }

        public BigInteger Order => _baseAlgebra.Order;

        public E Generator => _baseAlgebra.Generator;

        public E NeutralElement => _baseAlgebra.NeutralElement;

        public int ElementBitLength => _baseAlgebra.ElementBitLength;

        public int OrderBitLength => _baseAlgebra.OrderBitLength;

        public int FactorBitLength { get; }

        public E Add(E left, E right)
        {
            return _baseAlgebra.Add(left, right);
        }

        public E FromBytes(byte[] buffer)
        {
            return _baseAlgebra.FromBytes(buffer);
        }

        public E GenerateElement(BigInteger index)
        {
            int indexBitLength = CryptoGroupAlgebra<E>.GetBitLength(index);
            if (indexBitLength != FactorBitLength)
                throw new ArgumentException("Index does have an incorrect bit length!");
            return _baseAlgebra.MultiplyScalar(Generator, index, FactorBitLength);
        }

        public bool IsValid(E element)
        {
            return _baseAlgebra.IsValid(element);
        }

        public E MultiplyScalar(E e, BigInteger k)
        {
            int factorBitLength = CryptoGroupAlgebra<E>.GetBitLength(k);
            if (factorBitLength != FactorBitLength)
                throw new ArgumentException("Index does have an incorrect bit length!");
            return _baseAlgebra.MultiplyScalar(e, k, FactorBitLength);
        }

        public E Negate(E e)
        {
            return _baseAlgebra.Negate(e);
        }

        public byte[] ToBytes(E element)
        {
            return _baseAlgebra.ToBytes(element);
        }
    }
}
