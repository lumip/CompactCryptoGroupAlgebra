using System;
using System.Collections.Generic;
using System.Text;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroupElement<E, S>
    {
        private CryptoGroupAlgebra<E, S> _groupAlgebra;
        public E Value { get; private set; }

        public CryptoGroupElement(E value, CryptoGroupAlgebra<E, S> groupAlgebra)
        {
            if (groupAlgebra == null)
                throw new ArgumentNullException(nameof(groupAlgebra));
            _groupAlgebra = groupAlgebra;

            if (value == null)
                throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public CryptoGroupElement<E, S> Clone()
        {
            return Create(Value, _groupAlgebra);
        }

        public void Add(CryptoGroupElement<E, S> e)
        {
            if (_groupAlgebra != e._groupAlgebra)
                throw new ArgumentException("Added group element must be from the same group!", nameof(e));
            Value = _groupAlgebra.Add(Value, e.Value);
        }

        public void MultiplyScalar(S k)
        {
            Value = _groupAlgebra.MultiplyScalar(Value, k);
        }

        public void Invert()
        {
            Value = _groupAlgebra.Invert(Value);
        }

        public static CryptoGroupElement<E, S> operator +(CryptoGroupElement<E, S> left, CryptoGroupElement<E, S> right)
        {
            var result = left.Clone();
            result.Add(right);
            return result;
        }

        public static CryptoGroupElement<E, S> operator -(CryptoGroupElement<E, S> e)
        {
            var result = e.Clone();
            result.Invert();
            return result;
        }

        public static CryptoGroupElement<E, S> operator -(CryptoGroupElement<E, S> left, CryptoGroupElement<E, S> right)
        {
            var result = right.Clone();
            result.Invert();
            result.Add(left);
            return result;
        }

        public static CryptoGroupElement<E, S> operator *(CryptoGroupElement<E, S> e, S k)
        {
            var result = e.Clone();
            result.MultiplyScalar(k);
            return result;
        }

        public static CryptoGroupElement<E, S> operator *(S k, CryptoGroupElement<E, S> e)
        {
            return e * k;
        }

        protected abstract CryptoGroupElement<E, S> Create(E value, CryptoGroupAlgebra<E, S> groupAlgebra);
        public abstract byte[] ToByteArray();
    }
}
