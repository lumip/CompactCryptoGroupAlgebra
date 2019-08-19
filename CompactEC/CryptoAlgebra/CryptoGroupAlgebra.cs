using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroupAlgebra<E>
    {
        public BigInteger Order { get; }
        public E Generator { get; }
        public int GroupElementSize { get; }
        public int OrderSize { get; }
        public int FactorSize { get; }

        public CryptoGroupAlgebra(E generator, BigInteger order, int groupElementSize, int orderSize, int factorSize)
        {
            if (generator == null)
                throw new ArgumentNullException(nameof(generator));
            Generator = generator;

            if (order == null)
                throw new ArgumentNullException(nameof(order));
            Order = order;

            GroupElementSize = groupElementSize;
            OrderSize = orderSize;
            FactorSize = factorSize;
        }

        public CryptoGroupAlgebra(E generator, BigInteger order, int groupElementSize, int orderSize)
            : this(generator, order, groupElementSize, orderSize, orderSize) { }

        public E GenerateElement(BigInteger index)
        {
            return MultiplyScalar(Generator, index);
        }

        public virtual E Negate(E e)
        {
            return MultiplyScalar(e, Order - 1, OrderSize);
        }

        protected abstract E Multiplex(BigInteger selection, E left, E right);
        //{
        //    Debug.Assert(selection.IsOne || selection.IsZero);
        //    return right + selection * (left - right);
        //}

        protected virtual E Multiplex(bool selection, E left, E right)
        {
            var sel = new BigInteger(Convert.ToByte(selection));
            return Multiplex(sel, left, right);
        }

        protected virtual E MultiplyScalar(E e, BigInteger k, int factorSize)
        {
            // note(lumip): double-and-add (in this case: square-and-multiply)
            //  implementation that issues the same amount of adds no matter
            //  the value of k and has no conditional control flow. It is thus
            //  safe(r) against timing/power/cache/branch prediction(?)
            //  side channel attacks.

            int factorBitlen = 8 * factorSize;
            BigInteger maxFactor = BigInteger.One << factorBitlen;
            if (k >= maxFactor)
                throw new ArgumentException("The given factor is larger than the maximum admittable factor.", nameof(k));

            k = k % Order; // k * e is at least periodic in Order
            E r0 = IdentityElement;

            int i = factorBitlen - 1;
            for (BigInteger mask = maxFactor >> 1; !mask.IsZero; mask = mask >> 1, --i)
            {
                BigInteger bitI = (k & mask) >> i;
                r0 = Add(r0, r0);
                E r1 = Add(r0, e);

                r0 = Multiplex(bitI, r1, r0);
            }
            Debug.Assert(i == -1);
            return r0;
        }

        public E MultiplyScalar(E e, BigInteger k)
        {
            return MultiplyScalar(e, k, FactorSize);
        }

        public abstract E IdentityElement { get; }
        public abstract E Add(E left, E right);
        
    }
}
