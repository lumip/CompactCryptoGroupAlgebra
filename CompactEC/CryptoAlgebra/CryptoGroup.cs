using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroup<E> : ICryptoGroup where E : struct
    {
        protected CryptoGroupAlgebra<E> Algebra { get; }

        public CryptoGroup(CryptoGroupAlgebra<E> algebra)
        {
            if (algebra == null)
                throw new ArgumentNullException(nameof(algebra));
            Algebra = algebra;
        }

        public ICryptoGroupElement NeutralElement { get { return CreateGroupElement(Algebra.NeutralElement); } }
        public ICryptoGroupElement Generator { get { return CreateGroupElement(Algebra.Generator); } }

        public int OrderBitLength { get { return Algebra.OrderBitLength; } }

        public int OrderByteLength { get { return (int)Math.Ceiling((double)OrderBitLength / 8); } }

        public BigInteger Order { get { return Algebra.Order; } }

        public int ElementBitLength { get { return Algebra.ElementBitLength; } }

        public int ElementByteLength { get { return (int)Math.Ceiling((double)ElementBitLength / 8); } }

        protected abstract ICryptoGroupElement CreateGroupElement(E value);
        protected abstract ICryptoGroupElement CreateGroupElement(byte[] buffer);

        public ICryptoGroupElement Add(CryptoGroupElement<E> left, CryptoGroupElement<E> right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return CreateGroupElement(Algebra.Add(left.Value, right.Value));
        }

        public ICryptoGroupElement FromBytes(byte[] buffer)
        {
            return CreateGroupElement(buffer);
        }

        public ICryptoGroupElement Generate(BigInteger index)
        {
            return CreateGroupElement(Algebra.GenerateElement(index));
        }

        public ICryptoGroupElement MultiplyScalar(CryptoGroupElement<E> element, BigInteger k)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return CreateGroupElement(Algebra.MultiplyScalar(element.Value, k));
        }

        public ICryptoGroupElement Negate(CryptoGroupElement<E> element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return CreateGroupElement(Algebra.Negate(element.Value));
        }

        public ICryptoGroupElement Add(ICryptoGroupElement left, ICryptoGroupElement right)
        {
            CryptoGroupElement<E> lhs = left as CryptoGroupElement<E>;
            CryptoGroupElement<E> rhs = right as CryptoGroupElement<E>;
            if (lhs == null)
                throw new ArgumentException("The left summand is not an element of the group.", nameof(left));
            if (rhs == null)
                throw new ArgumentException("The right summand is not an element of the group.", nameof(left));

            return Add(lhs, rhs);
        }

        public ICryptoGroupElement MultiplyScalar(ICryptoGroupElement element, BigInteger k)
        {
            CryptoGroupElement<E> e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return MultiplyScalar(e, k);
        }

        public ICryptoGroupElement Negate(ICryptoGroupElement element)
        {
            CryptoGroupElement<E> e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return Negate(e);
        }
    }
}
