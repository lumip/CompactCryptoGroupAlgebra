using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroupElement<E>
    {
        private CryptoGroupAlgebra<E> _groupAlgebra;
        public E Value { get; private set; }

        public CryptoGroupElement(E value, CryptoGroupAlgebra<E> groupAlgebra)
        {
            if (groupAlgebra == null)
                throw new ArgumentNullException(nameof(groupAlgebra));
            _groupAlgebra = groupAlgebra;

            if (value == null)
                throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public CryptoGroupElement<E> Clone()
        {
            return Create(Value, _groupAlgebra);
        }

        public void Add(CryptoGroupElement<E> e)
        {
            if (_groupAlgebra != e._groupAlgebra)
                throw new ArgumentException("Added group element must be from the same group!", nameof(e));
            Value = _groupAlgebra.Add(Value, e.Value);
        }

        public void MultiplyScalar(BigInteger k)
        {
            Value = _groupAlgebra.MultiplyScalar(Value, k);
        }

        public void Invert()
        {
            Value = _groupAlgebra.Negate(Value);
        }

        public static CryptoGroupElement<E> operator +(CryptoGroupElement<E> left, CryptoGroupElement<E> right)
        {
            var result = left.Clone();
            result.Add(right);
            return result;
        }

        public static CryptoGroupElement<E> operator -(CryptoGroupElement<E> e)
        {
            var result = e.Clone();
            result.Invert();
            return result;
        }

        public static CryptoGroupElement<E> operator -(CryptoGroupElement<E> left, CryptoGroupElement<E> right)
        {
            var result = right.Clone();
            result.Invert();
            result.Add(left);
            return result;
        }

        public static CryptoGroupElement<E> operator *(CryptoGroupElement<E> e, BigInteger k)
        {
            var result = e.Clone();
            result.MultiplyScalar(k);
            return result;
        }

        public static CryptoGroupElement<E> operator *(BigInteger k, CryptoGroupElement<E> e)
        {
            return e * k;
        }

        protected abstract CryptoGroupElement<E> Create(E value, CryptoGroupAlgebra<E> groupAlgebra);
        public abstract byte[] ToByteArray();
    }
}
