using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Base implementation of <see cref="ICryptoGroupAlgebra{T}"/>.
    /// 
    /// Provides common implementations independent of the actual group details
    /// based on basic operations. Basic operations are left abstract and must
    /// be implemented by deriving classes.
    /// 
    /// Group elements passed into algebraic operations are assumed to be valid values,
    /// this is not explicitely checked for and should be done by calling classes.
    /// </summary>
    /// <typeparam name="T">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    public abstract class CryptoGroupAlgebra<T> : ICryptoGroupAlgebra<T> where T : notnull
    {
        /// <inheritdoc/>
        public BigPrime Order { get; }

        /// <inheritdoc/>
        public T Generator { get; }

        /// <inheritdoc/>
        public abstract BigInteger Cofactor { get; }

        /// <inheritdoc/>
        public abstract int ElementBitLength { get; }

        /// <inheritdoc/>
        public int OrderBitLength { get { return NumberLength.GetLength(Order).InBits; } }

        /// <summary>
        /// Initializes a new instance of <see cref="CryptoGroupAlgebra{T}"/>
        /// with a given generator <paramref name="order"/>.
        /// 
        /// Checks that the given <paramref name="order"/> is prime and throws
        /// a <exception cref="ArgumentException"/> if that is not the case.
        /// </summary>
        /// <param name="generator">Generator of the group.</param>
        /// <param name="order">Order of the group's generator.</param>
        protected CryptoGroupAlgebra(T generator, BigPrime order)
        {
            // todo: would be nice to do IsValid(generator) here - but that is virtual
            Generator = generator;
            Order = order;
        }

        /// <inheritdoc/>
        public T GenerateElement(BigInteger index)
        {
            return MultiplyScalar(Generator, index);
        }

        /// <inheritdoc/>
        public virtual T Negate(T e)
        {
            return MultiplyScalarUnchecked(e, Order - 1, OrderBitLength);
        }

        /// <summary>
        /// Internal implementation of scalar multiplication.
        /// 
        /// Uses a double-and-add approach relying on the <see cref="Add(T, T)"/>
        /// method. Since no data dependent branching occurs, the implementation
        /// can be considered reasonably safe against timing/power/branch prediction
        /// side channel attacks if the same is true for implementations of
        /// <see cref="Add(T, T)"/> and <see cref="Multiplex(BigInteger, T, T)"/>.
        /// 
        /// To ensure this, the double-and-add implementation assumes that the scalar
        /// factor's bit length is limited by a given upper bound and always executes
        /// exactly that many iterations. To ensure resilience against side channel
        /// attacks, this bit length should therefore be held constant over subsequent
        /// invocations.
        /// </summary>
        /// <remarks>
        /// This method is intended to be overriden with more specific implementations
        /// if necessary. It is labelled "Unchecked" since it performs no validity checks
        /// for arguments to keep it simple. These checks are done by the public facing
        /// methods <see cref="MultiplyScalar(T, BigInteger)"/>
        /// and <see cref="MultiplyScalar(T, BigInteger, int)"/>, which then call
        /// MultiplyScalarUnchecked. Overriding implementations can therefore also benefit
        /// from the argument checks performed in these and need not check their arguments.
        /// </remarks>
        /// <param name="e">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <param name="factorBitLength">Maximum bit length of a scalar.</param>
        /// <returns>The given element multiplied with the given scalar.</returns>
        protected virtual T MultiplyScalarUnchecked(T e, BigInteger k, int factorBitLength)
        {
            BigInteger maxFactor = BigInteger.One << factorBitLength;
            int i = factorBitLength - 1;

            T r0 = NeutralElement;
            for (BigInteger mask = maxFactor >> 1; !mask.IsZero; mask = mask >> 1, --i)
            {
                BigInteger bitI = (k & mask) >> i;
                r0 = Add(r0, r0);
                T r1 = Add(r0, e);

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
        public T MultiplyScalar(T e, BigInteger k, int factorBitLength)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(k), "The given factor must be non-negative.");

            if (NumberLength.GetLength(k).InBits > factorBitLength)
                throw new ArgumentOutOfRangeException(nameof(k), "The given factor must not exceed the admittable factor bit length.");

            return MultiplyScalarUnchecked(e, k, factorBitLength);
        }

        /// <inheritdoc/>
        public T MultiplyScalar(T e, BigInteger k)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(k), "The given factor must be non-negative."); // todo: check other occurrences for argument order
            k = k % Order; // k * e is at least periodic in Order
            return MultiplyScalarUnchecked(e, k, OrderBitLength);
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
        /// <param name="selection">Selection bit (as <see cref="BigInteger"/>). Takes values 0 or 1.</param>
        /// <param name="left">Element to be returned when the selection bit is 0.</param>
        /// <param name="right">Element to be returned when the selection bit is 1.</param>
        /// <returns>The element determined by the selection bit.</returns>
        protected abstract T Multiplex(BigInteger selection, T left, T right);

        /// <inheritdoc/>
        public abstract T NeutralElement { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// Implementers should take care to provide a data-independent, branch-free
        /// implementation to be resistant to side channel attacks.
        /// </remarks>
        public abstract T Add(T left, T right);

        /// <inheritdoc/>
        public bool IsValid(T element)
        {
            // implementation specific checks
            if (!IsValidDerived(element)) return false;

            // verifying that the point is not from a small subgroup of the whole curve (and thus outside
            // of the safe subgroup over which operations are considered)
            if (Cofactor > 1)
            {
                T check = MultiplyScalarUnchecked(element, Cofactor, NumberLength.GetLength(Cofactor).InBits);
                if (check.Equals(NeutralElement))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Implementation specific checks for validity of group elements.
        /// 
        /// Must be provided by inheriting classes and is called by
        /// <see cref="IsValid"/>.
        /// </summary>
        /// <param name="element">The group element candidate to be checked for validity.</param>
        /// <returns><c>true</c>, if the candidate is valid, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// Implementations of <see cref="IsValidDerived(T)"/> do not need to
        /// check whether the element candidate has too small order, as that
        /// check is performed in <see cref="IsValid(T)"/>.
        /// </remarks>
        protected abstract bool IsValidDerived(T element);

        /// <inheritdoc/>
        public abstract T FromBytes(byte[] buffer);

        /// <inheritdoc/>
        public abstract byte[] ToBytes(T element);

        /// <summary>
        /// Determines whether the given <see cref="CryptoGroupAlgebra{T}"/> is equal to the current <see cref="CryptoGroupAlgebra{T}"/>
        /// in the sense that both operate on the same algebraic group.
        /// </summary>
        /// <param name="other">The <see cref="CryptoGroupAlgebra{T}"/> to compare with the current <see cref="CryptoGroupAlgebra{T}"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="CryptoGroupAlgebra{T}"/> is equal to the current
        /// <see cref="CompactCryptoGroupAlgebra.MultiplicativeGroupAlgebra"/>; otherwise, <c>false</c>.</returns>
        public abstract bool Equals(CryptoGroupAlgebra<T>? other);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as CryptoGroupAlgebra<T>);
        }

        /// <inheritdoc/>
        public abstract override int GetHashCode();
    }
}
