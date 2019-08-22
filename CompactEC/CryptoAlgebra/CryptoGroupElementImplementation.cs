using System;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroupElementImplementation<E> : ICryptoGroupElement where E : struct
    {
        protected CryptoGroupAlgebra<E> Algebra { get; }
        public E Value { get; private set; }

        public CryptoGroupElementImplementation(E value, CryptoGroupAlgebra<E> groupAlgebra)
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
            CryptoGroupElementImplementation<E> e = element as CryptoGroupElementImplementation<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(Value));

            Add(e);
        }

        public void Add(CryptoGroupElementImplementation<E> element)
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
            var o = other as CryptoGroupElementImplementation<E>;
            return o != null && Value.Equals(o.Value);
        }

        public static CryptoGroupElementImplementation<E> operator +(CryptoGroupElementImplementation<E> left, ICryptoGroupElement right)
        {
            var result = (CryptoGroupElementImplementation<E>)left.Clone();
            result.Add(right);
            return result;
        }

        public static CryptoGroupElementImplementation<E> operator +(ICryptoGroupElement left, CryptoGroupElementImplementation<E> right)
        {
            return right + left;
        }

        public static CryptoGroupElementImplementation<E> operator -(CryptoGroupElementImplementation<E> e)
        {
            var result = (CryptoGroupElementImplementation<E>)e.Clone();
            result.Negate();
            return result;
        }

        public static CryptoGroupElementImplementation<E> operator -(ICryptoGroupElement left, CryptoGroupElementImplementation<E> right)
        {
            var result = (CryptoGroupElementImplementation<E>)right.Clone();
            result.Negate();
            result.Add(left);
            return result;
        }

        public static CryptoGroupElementImplementation<E> operator -(CryptoGroupElementImplementation<E> left, ICryptoGroupElement right)
        {
            return right - left;
        }

        public static CryptoGroupElementImplementation<E> operator *(CryptoGroupElementImplementation<E> e, BigInteger k)
        {
            var result = (CryptoGroupElementImplementation<E>)e.Clone();
            result.MultiplyScalar(k);
            return result;
        }

        public static CryptoGroupElementImplementation<E> operator *(BigInteger k, CryptoGroupElementImplementation<E> e)
        {
            return e * k;
        }
    }
}
