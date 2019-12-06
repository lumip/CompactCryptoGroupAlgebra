using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace CompactEC
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
        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.Order"/>
        /// </summary>
        public abstract BigInteger Order { get; }

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.Generator"/>
        /// </summary>
        public abstract E Generator { get; }

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.ElementBitLength"/>
        /// </summary>
        public abstract int ElementBitLength { get; }

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.OrderBitLength"/>
        /// </summary>
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

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.GenerateElement(BigInteger)"/>
        /// </summary>
        public E GenerateElement(BigInteger index)
        {
            return MultiplyScalar(Generator, index);
        }

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.Negate(E)"/>
        /// </summary>
        public virtual E Negate(E e)
        {
            return MultiplyScalarUnsafe(e, Order - 1, OrderBitLength);
        }

        /// <summary>
        /// Internal implementation of scalar multiplication.
        /// 
        /// Uses a double-and-add approach relying on the <see cref="Add(E, E)"/>
        /// method. Since no data dependent branching occurs, the implementation
        /// can be considered reasonably safe against timing/power/branch prediction
        /// side channel attacks if the same is true for implementations of
        /// <see cref="Add(E, E)"/> and <see cref="Multiplex(BigInteger, E, E)"/>.
        /// 
        /// To ensure this, the double-and-add implementation assumes that the scalar
        /// factor's bit length is limited by a given upper bound and always executes
        /// exactly that many iterations. To ensure resilience against side channel
        /// attacks, this bit length should therefore be held constant over subsequent
        /// invocations.
        /// </summary>
        /// <remarks>
        /// This method is intended to be overriden with more specific implementations
        /// if necessary. It is labelled "Unsafe" since it performs no validity checks
        /// for arguments to keep it simple. These checks are done by the public facing
        /// methods <see cref="MultiplyScalar(E, BigInteger)"/>
        /// and <see cref="MultiplyScalar(E, BigInteger, int)"/>, which then call
        /// MultiplyScalarUnsafe. Overriding implementations can therefore also benefit
        /// from the argument checks performed there.
        /// </remarks>
        /// <param name="e">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <param name="factorBitLength">Maximum bit length of a scalar.</param>
        /// <returns>The given element multiplied with the given scalar.</returns>
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

        /// <summary>
        /// Multiplies a group element with a scalar factor.
        /// 
        /// Scalar multiplication is understood as adding the group element to itself
        /// as many times as dictated by the scalar factor.
        ///
        /// The optional parameter factorBitLength allows to specify the bit length
        /// of the scalar, which increases performance if it is significantly below
        /// that of the order. However, to be resistant against side channel attacks,
        /// this value should be held constant over subsequent calls to this method.
        /// </summary>
        /// <param name="e">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <param name="factorBitLength">Maximum bit length of a scalar.</param>
        /// <returns>The given element multiplied with the given scalar.</returns>
        public E MultiplyScalar(E e, BigInteger k, int factorBitLength)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException("The given factor must be non-negative.", nameof(k));

            if (GetBitLength(k) > factorBitLength)
                throw new ArgumentOutOfRangeException("The given factor must not exceed the admittable factor bit length.", nameof(k));

            return MultiplyScalarUnsafe(e, k, factorBitLength);
        }

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.MultiplyScalar(E, BigInteger)"/>
        /// </summary>
        public E MultiplyScalar(E e, BigInteger k)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException("The given factor must be non-negative.", nameof(k));
            k = k % Order; // k * e is at least periodic in Order
            return MultiplyScalarUnsafe(e, k, OrderBitLength);
        }

        /// <summary>
        /// Multiplexes two elements based on a selection bit.
        /// 
        /// Returns the second given group element (right) if the selection bit is 1, and the first
        /// given element (left) otherwise.
        /// </summary>
        /// <remarks>
        /// Implementers should take care to provide a data-independent, branch-free
        /// implementation to be resistant to side channel attacks.
        /// </remarks>
        /// <param name="selection">Selection bit (as BigInteger). Takes values 0 or 1.</param>
        /// <param name="left">Element to be returned when the selection bit is 0.</param>
        /// <param name="right">Element to be returned when the selection bit is 1.</param>
        /// <returns>The element determined by the selection bit.</returns>
        protected abstract E Multiplex(BigInteger selection, E left, E right);

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.NeutralElement"/>
        /// </summary>
        public abstract E NeutralElement { get; }

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.Add(E, E)"/>
        /// </summary>
        /// <remarks>
        /// Implementers should take care to provide a data-independent, branch-free
        /// implementation to be resistant to side channel attacks.
        /// </remarks>
        public abstract E Add(E left, E right);

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.IsValid(E)"/>
        /// </summary>
        public abstract bool IsValid(E element);

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.FromBytes(byte[])"/>
        /// </summary>
        public abstract E FromBytes(byte[] buffer);

        /// <summary>
        /// <see cref="ICryptoGroupAlgebra{E}.ToBytes(E)"/>
        /// </summary>
        public abstract byte[] ToBytes(E element);
    }
}
