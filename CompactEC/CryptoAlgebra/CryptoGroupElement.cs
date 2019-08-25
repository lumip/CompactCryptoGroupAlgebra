using System;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroupElement<E> : ICryptoGroupElement where E : struct
    {
        protected CryptoGroupAlgebra<E> Algebra { get; }
        public E Value { get; private set; }

        public CryptoGroupElement(E value, CryptoGroupAlgebra<E> groupAlgebra)
        {
            if (groupAlgebra == null)
                throw new ArgumentNullException(nameof(groupAlgebra));
            Algebra = groupAlgebra;
            
            if (!Algebra.IsValid(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        public void Add(ICryptoGroupElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            CryptoGroupElement<E> e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(Value));

            Add(e);
        }

        public void Add(CryptoGroupElement<E> element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (Algebra != element.Algebra)
                throw new ArgumentException("Added group element must be from the same group!", nameof(element));
            Value = Algebra.Add(Value, element.Value);
        }

        public void MultiplyScalar(BigInteger k)
        {
            Value = Algebra.MultiplyScalar(Value, k);
        }

        public void Negate()
        {
            Value = Algebra.Negate(Value);
        }

        public abstract byte[] ToBytes();
        public abstract ICryptoGroupElement Clone();

        public bool Equals(ICryptoGroupElement other)
        {
            var o = other as CryptoGroupElement<E>;
            return o != null && Algebra == o.Algebra && Value.Equals(o.Value);
        }

        public static CryptoGroupElement<E> operator +(CryptoGroupElement<E> left, ICryptoGroupElement right)
        {
            var result = (CryptoGroupElement<E>)left.Clone();
            result.Add(right);
            return result;
        }

        public static CryptoGroupElement<E> operator -(CryptoGroupElement<E> e)
        {
            var result = (CryptoGroupElement<E>)e.Clone();
            result.Negate();
            return result;
        }

        public static CryptoGroupElement<E> operator -(ICryptoGroupElement left, CryptoGroupElement<E> right)
        {
            var result = (CryptoGroupElement<E>)right.Clone();
            result.Negate();
            result.Add(left);
            return result;
        }

        public static CryptoGroupElement<E> operator *(CryptoGroupElement<E> e, BigInteger k)
        {
            var result = (CryptoGroupElement<E>)e.Clone();
            result.MultiplyScalar(k);
            return result;
        }

        public static CryptoGroupElement<E> operator *(BigInteger k, CryptoGroupElement<E> e)
        {
            return e * k;
        }
    }
}
