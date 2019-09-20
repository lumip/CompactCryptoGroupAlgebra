using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public class CryptoGroupElement<E> : ICryptoGroupElement where E : struct
    {
        protected ICryptoGroupAlgebra<E> Algebra { get; }
        public E Value { get; private set; }

        public CryptoGroupElement(E value, ICryptoGroupAlgebra<E> groupAlgebra)
        {
            if (groupAlgebra == null)
                throw new ArgumentNullException(nameof(groupAlgebra));
            Algebra = groupAlgebra;
            
            if (!Algebra.IsValid(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        public CryptoGroupElement(byte[] valueBuffer, ICryptoGroupAlgebra<E> groupAlgebra)
        {
            if (groupAlgebra == null)
                throw new ArgumentNullException(nameof(groupAlgebra));
            Algebra = groupAlgebra;

            E value = Algebra.FromBytes(valueBuffer);
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

        public byte[] ToBytes()
        {
            return Algebra.ToBytes(Value);
        }

        protected CryptoGroupElement<E> CloneInternal()
        {
            return new CryptoGroupElement<E>(Value, Algebra);
        }

        public ICryptoGroupElement Clone()
        {
            return CloneInternal();
        }

        public bool Equals(CryptoGroupElement<E> other)
        {
            return other != null && Algebra == other.Algebra && Value.Equals(other.Value);
        }

        public bool Equals(ICryptoGroupElement other)
        {
            return Equals(other as CryptoGroupElement<E>);
        }

        public override bool Equals(object other)
        {
            return Equals(other as CryptoGroupElement<E>);
        }

        public override int GetHashCode()
        {
            var hashCode = -1217399511;
            hashCode = hashCode * -1521134295 + EqualityComparer<ICryptoGroupAlgebra<E>>.Default.GetHashCode(Algebra);
            hashCode = hashCode * -1521134295 + EqualityComparer<E>.Default.GetHashCode(Value);
            return hashCode;
        }

        public static CryptoGroupElement<E> operator +(CryptoGroupElement<E> left, ICryptoGroupElement right)
        {
            var result = left.CloneInternal();
            result.Add(right);
            return result;
        }

        public static CryptoGroupElement<E> operator -(CryptoGroupElement<E> e)
        {
            var result = e.CloneInternal();
            result.Negate();
            return result;
        }

        public static CryptoGroupElement<E> operator -(ICryptoGroupElement left, CryptoGroupElement<E> right)
        {
            var result = right.CloneInternal();
            result.Negate();
            result.Add(left);
            return result;
        }

        public static CryptoGroupElement<E> operator *(CryptoGroupElement<E> e, BigInteger k)
        {
            var result = e.CloneInternal();
            result.MultiplyScalar(k);
            return result;
        }

        public static CryptoGroupElement<E> operator *(BigInteger k, CryptoGroupElement<E> e)
        {
            return e * k;
        }
    }
}
