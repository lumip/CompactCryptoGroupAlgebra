using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace CompactEC.CryptoAlgebra
{
    /// <summary>
    /// Base implementation of <see cref="ICryptoGroupAlgebra{E}"/>.
    /// 
    /// Provides common implementations independent of the actual group details
    /// based on basic operations. Basic operations are left abstract and must
    /// be implemented by deriving classes.
    /// 
    /// Group elements passed into algebraic operations are assumed to be valid values,
    /// this is not explicitely checked for and should be done by calling classes.
    /// </summary>
    /// <typeparam name="E">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    public abstract class CryptoGroupAlgebra<E> : ICryptoGroupAlgebra<E> where E : struct
    {
        public abstract BigInteger Order { get; }
        public abstract E Generator { get; }

        public abstract int ElementBitLength { get; }
        public int OrderBitLength { get { return GetBitLength(Order); } }

        /// <summary>
        /// Returns the bit length of a BigInteger.
        /// </summary>
        /// <param name="x">Any BigInteger instance.</param>
        /// <returns>The bit length of the input.</returns>
        public static int GetBitLength(BigInteger x)
        {
            if (x.IsZero)
                return 0;
            return (int)Math.Floor(BigInteger.Log(x, 2) + 1);
        }

        public E GenerateElement(BigInteger index)
        {
            return MultiplyScalar(Generator, index);
        }

        public virtual E Negate(E e)
        {
            return MultiplyScalar(e, Order - 1, OrderBitLength);
        }

        protected virtual E MultiplyScalarUnsafe(E e, BigInteger k, int factorBitLength)
        {
            // note(lumip): double-and-add (in this case: square-and-multiply)
            //  implementation that issues the same amount of adds no matter
            //  the value of k and has no conditional control flow. It is thus
            //  safe(r) against timing/power/cache/branch prediction(?)
            //  side channel attacks.

            BigInteger maxFactor = BigInteger.One << factorBitLength;
            int i = factorBitLength - 1;

            E r0 = NeutralElement;
            for (BigInteger mask = maxFactor >> 1; !mask.IsZero; mask = mask >> 1, --i)
            {
                BigInteger bitI = (k & mask) >> i;
                r0 = Add(r0, r0);
                E r1 = Add(r0, e);

                r0 = Multiplex(bitI, r0, r1);
            }
            Debug.Assert(i == -1);
            return r0;
        }

        public E MultiplyScalar(E e, BigInteger k, int factorBitLength)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException("The given factor must be non-negative.", nameof(k));
            
            if (GetBitLength(k) > factorBitLength)
                throw new ArgumentOutOfRangeException("The given factor must not exceed the admittable factor bit length.", nameof(k));

            return MultiplyScalarUnsafe(e, k, factorBitLength);
        }

        public E MultiplyScalar(E e, BigInteger k)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException("The given factor must be non-negative.", nameof(k));
            k = k % Order; // k * e is at least periodic in Order
            return MultiplyScalarUnsafe(e, k, OrderBitLength);
        }

        protected abstract E Multiplex(BigInteger selection, E left, E right);
        public abstract E NeutralElement { get; }
        public abstract E Add(E left, E right);
        public abstract bool IsValid(E element);
        public abstract E FromBytes(byte[] buffer);
        public abstract byte[] ToBytes(E element);
    }
}
