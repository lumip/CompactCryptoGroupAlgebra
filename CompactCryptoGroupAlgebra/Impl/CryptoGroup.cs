using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Base implementation of <see cref="ICryptoGroup{T}"/>.
    /// 
    /// Relies on <see cref="ICryptoGroupAlgebra{T}"/> to provide basic algebraic group
    /// opertions on group elements represented the template type. The implementation of
    /// CryptoGroup mainly provides abstraction from this specifics for production code,
    /// wrapping the template type into <see cref="CryptoGroupElement{T}"/>
    /// instances, thus allowing production code to interface only with the
    /// <see cref="ICryptoGroup{T}"/> and <see cref="ICryptoGroupElement{T}"/> interfaces,
    /// irrespective of the specific group implementation.
    /// </summary>
    /// <typeparam name="T">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    /// <remarks>
    /// Implementers of <see cref="ICryptoGroup{T}"/> should provide an implementation of
    /// <see cref="ICryptoGroupAlgebra{T}"/> (preferrably by extending <see cref="CryptoGroupAlgebra{T}"/>)
    /// to realize basic algebraic operations and an extension of <see cref="CryptoGroupElement{T}"/> 
    /// which can then be employed in a specialization of <see cref="CryptoGroup{T}"/>.
    /// 
    /// All these classes are designed to have a minimum of fully virtual/abstract methods in need of implementation
    /// to provide full algebraic group functionality.
    /// </remarks>
    public abstract class CryptoGroup<T> : ICryptoGroup<T> where T: notnull
    {
        /// <summary>
        /// Subclass accessor for the <see cref="ICryptoGroupAlgebra{T}"/> that provides
        /// implementations of underlying group operatiosn on raw group element type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <value>The <see cref="ICryptoGroupAlgebra{T}"/> that provides
        /// implementations of underlying group operatiosn on raw group element type
        /// <typeparamref name="T"/>.</value>
        protected ICryptoGroupAlgebra<T> Algebra { get; }

        /// <summary>
        /// Initializes a <see cref="CryptoGroup{T}"/> instance using a given <see cref="ICryptoGroupAlgebra{T}"/> instance
        /// for algebraic operations.
        /// </summary>
        /// <param name="algebra">Group algebra implementation on raw data type <typeparamref name="T"/></param>
        protected CryptoGroup(ICryptoGroupAlgebra<T> algebra)
        {
            Algebra = algebra;
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> Generator { get { return CreateGroupElement(Algebra.Generator); } }

        /// <inheritdoc/>
        public int OrderBitLength { get { return Algebra.OrderBitLength; } }

        /// <inheritdoc/>
        public int OrderByteLength { get { return (int)Math.Ceiling((double)OrderBitLength / 8); } }

        /// <inheritdoc/>
        public BigInteger Order { get { return Algebra.Order; } }

        /// <summary>
        /// <see cref="ICryptoGroup{T}.ElementBitLength"/>
        /// </summary>
        public int ElementBitLength { get { return Algebra.ElementBitLength; } }

        /// <inheritdoc/>
        public int ElementByteLength { get { return (int)Math.Ceiling((double)ElementBitLength / 8); } } // todo: use NumberLength

        /// <summary>
        /// Creates a new <see cref="CryptoGroupElement{T}"/> instance for a given raw group element of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">Value of the group element as raw type.</param>
        /// <returns>An <see cref="CryptoGroupElement{T}"/>instance representing the given value.</returns>
        /// <remarks>
        /// Used to convert outputs of <see cref="ICryptoGroupAlgebra{T}"/> operations to <see cref="ICryptoGroupElement{T}"/>
        /// instances by methods within this class.
        /// 
        /// Needs to be implemented by specializations.
        /// </remarks>
        protected abstract CryptoGroupElement<T> CreateGroupElement(T value);

        /// <summary>
        /// Creates a new <see cref="CryptoGroupElement{T}"/> instance from a byte representation of the group element.
        /// </summary>
        /// <param name="buffer">Byte representation of the group elements.</param>
        /// <returns>An <see cref="CryptoGroupElement{T}"/>instance representing the given value.</returns>
        /// <remarks>
        /// Needs to be implemented by specializations.
        /// </remarks>
        protected abstract CryptoGroupElement<T> CreateGroupElement(byte[] buffer);

        /// <inheritdoc/>
        public ICryptoGroupElement<T> Add(CryptoGroupElement<T> left, CryptoGroupElement<T> right)
        {
            return CreateGroupElement(Algebra.Add(left.Value, right.Value));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> FromBytes(byte[] buffer)
        {
            return CreateGroupElement(buffer);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> Generate(BigInteger index)
        {
            return CreateGroupElement(Algebra.GenerateElement(index));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> MultiplyScalar(CryptoGroupElement<T> element, BigInteger k)
        {
            return CreateGroupElement(Algebra.MultiplyScalar(element.Value, k));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> Negate(CryptoGroupElement<T> element)
        {
            return CreateGroupElement(Algebra.Negate(element.Value));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> Add(ICryptoGroupElement<T> left, ICryptoGroupElement<T> right)
        {
            CryptoGroupElement<T>? lhs = left as CryptoGroupElement<T>;
            CryptoGroupElement<T>? rhs = right as CryptoGroupElement<T>;
            if (lhs == null)
                throw new ArgumentException("The left summand is not an element of the group.", nameof(left));
            if (rhs == null)
                throw new ArgumentException("The right summand is not an element of the group.", nameof(left));

            return Add(lhs, rhs);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> MultiplyScalar(ICryptoGroupElement<T> element, BigInteger k)
        {
            CryptoGroupElement<T>? e = element as CryptoGroupElement<T>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return MultiplyScalar(e, k);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> Negate(ICryptoGroupElement<T> element)
        {
            CryptoGroupElement<T>? e = element as CryptoGroupElement<T>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return Negate(e);
        }

        /// <inheritdoc/>
        public (BigInteger, ICryptoGroupElement<T>) GenerateRandom(RandomNumberGenerator rng)
        {
            BigInteger index = rng.RandomBetween(1, Order - 1);
            ICryptoGroupElement<T> element = Generate(index);
            return (index, element);
        }
    }
}
