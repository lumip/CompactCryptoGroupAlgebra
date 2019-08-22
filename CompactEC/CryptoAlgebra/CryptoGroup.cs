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

        protected abstract ICryptoGroupElement CreateGroupElement(E value);
        protected abstract ICryptoGroupElement CreateGroupElement(byte[] buffer);

        //private ICryptoGroupElement CreateGroupElementWithOperators(E value)
        //{
        //    return new CryptoGroupElementOperatorDecorator(CreateGroupElement(value));
        //}

        //private ICryptoGroupElement CreateGroupElementWithOperators(byte[] buffer)
        //{
        //    return new CryptoGroupElementOperatorDecorator(CreateGroupElement(buffer));
        //}

        public ICryptoGroupElement Add(CryptoGroupElementImplementation<E> left, CryptoGroupElementImplementation<E> right)
        {
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

        public ICryptoGroupElement MultiplyScalar(CryptoGroupElementImplementation<E> element, BigInteger k)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return CreateGroupElement(Algebra.MultiplyScalar(element.Value, k));
        }

        public ICryptoGroupElement Negate(CryptoGroupElementImplementation<E> element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return CreateGroupElement(Algebra.Negate(element.Value));
        }

        public ICryptoGroupElement Add(ICryptoGroupElement left, ICryptoGroupElement right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            CryptoGroupElementImplementation<E> lhs = left as CryptoGroupElementImplementation<E>;
            CryptoGroupElementImplementation<E> rhs = right as CryptoGroupElementImplementation<E>;
            if (lhs == null)
                throw new ArgumentException("The left summand is not an element of the group.", nameof(left));
            if (rhs == null)
                throw new ArgumentException("The right summand is not an element of the group.", nameof(left));

            return Add(lhs, rhs);
        }

        public ICryptoGroupElement MultiplyScalar(ICryptoGroupElement element, BigInteger k)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            CryptoGroupElementImplementation<E> e = element as CryptoGroupElementImplementation<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return MultiplyScalar(e, k);
        }

        public ICryptoGroupElement Negate(ICryptoGroupElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            CryptoGroupElementImplementation<E> e = element as CryptoGroupElementImplementation<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return Negate(e);
        }
    }
    //public abstract class CryptoGroup : ICryptoGroup
    //{
    //    public abstract ICryptoGroupElement NeutralElement { get; }
    //    public abstract ICryptoGroupElement Generator { get; }
    //    public abstract BigInteger Order { get; }
    //    public abstract int OrderBitlength { get; }

    //    public abstract ICryptoGroupElement Add(ICryptoGroupElement left, ICryptoGroupElement right);
    //    public abstract ICryptoGroupElement FromBytes(byte[] buffer);
    //    protected abstract ICryptoGroupElement Multiplex(BigInteger selection, ICryptoGroupElement left, ICryptoGroupElement right);

    //    public virtual ICryptoGroupElement Generate(BigInteger index)
    //    {
    //        return MultiplyScalar(Generator, index);
    //    }

    //    protected virtual ICryptoGroupElement Multiplex(bool selection, ICryptoGroupElement left, ICryptoGroupElement right)
    //    {
    //        var sel = new BigInteger(Convert.ToByte(selection));
    //        return Multiplex(sel, left, right);
    //    }

    //    public virtual ICryptoGroupElement MultiplyScalar(ICryptoGroupElement element, BigInteger k)
    //    {
    //        // note(lumip): double-and-add (in this case: square-and-multiply)
    //        //  implementation that issues the same amount of adds no matter
    //        //  the value of k and has no conditional control flow. It is thus
    //        //  safe(r) against timing/power/cache/branch prediction(?)
    //        //  side channel attacks.

    //        BigInteger initialMask = BigInteger.One << OrderBitlength;

    //        k = k % Order; // k * e is at least periodic in Order
    //        var r0 = NeutralElement;

    //        int i = OrderBitlength - 1;
    //        for (BigInteger mask = initialMask >> 1; !mask.IsZero; mask = mask >> 1, --i)
    //        {
    //            BigInteger bitI = (k & mask) >> i;
    //            r0 = Add(r0, r0);
    //            var r1 = Add(r0, element);

    //            r0 = Multiplex(bitI, r1, r0);
    //        }
    //        Debug.Assert(i == -1);
    //        return r0;
    //    }

    //    public virtual ICryptoGroupElement Negate(ICryptoGroupElement element)
    //    {
    //        return MultiplyScalar(element, Order - 1);
    //    }
    //}
}
