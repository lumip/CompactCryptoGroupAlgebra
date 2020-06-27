using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Base implementation of <see cref="ICryptoGroup"/>.
    /// 
    /// Relies on <see cref="ICryptoGroupAlgebra{E}"/> to provide basic algebraic group
    /// opertions on group elements represented the template type. The implementation of
    /// CryptoGroup mainly provides abstraction from this specifics for production code,
    /// wrapping the template type into <see cref="CryptoGroupElement{E}"/>
    /// instances, thus allowing production code to interface only with the
    /// <see cref="ICryptoGroup"/> and <see cref="ICryptoGroupElement"/> interfaces,
    /// irrespective of the specific group implementation.
    /// </summary>
    /// <typeparam name="E">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    /// <remarks>
    /// Implementers of <see cref="ICryptoGroup"/> should provide an implementation of
    /// <see cref="ICryptoGroupAlgebra{E}"/> (preferrably by extending <see cref="CryptoGroupAlgebra{E}"/>)
    /// to realize basic algebraic operations and an extension of <see cref="CryptoGroupElement{E}"/> 
    /// which can then be employed in a specialization of <see cref="CryptoGroup{E}"/>.
    /// 
    /// All these classes are designed to have a minimum of fully virtual/abstract methods in need of implementation
    /// to provide full algebraic group functionality.
    /// </remarks>
    public abstract class CryptoGroup<E> : ICryptoGroup where E : struct
    {
        /// <summary>
        /// Subclass accessor for the <see cref="ICryptoGroupAlgebra{E}"/> that provides
        /// implementations of underlying group operatiosn on raw group element type
        /// <typeparamref name="E"/>.
        /// </summary>
        /// <value>The <see cref="ICryptoGroupAlgebra{E}"/> that provides
        /// implementations of underlying group operatiosn on raw group element type
        /// <typeparamref name="E"/>.</value>
        protected ICryptoGroupAlgebra<E> Algebra { get; }

        /// <summary>
        /// Initializes a <see cref="CryptoGroup{E}"/> instance using a given <see cref="ICryptoGroupAlgebra{E}"/> instance
        /// for algebraic operations.
        /// </summary>
        /// <param name="algebra">Group algebra implementation on raw data type <typeparamref name="E"/></param>
        protected CryptoGroup(ICryptoGroupAlgebra<E> algebra)
        {
            Algebra = algebra;
        }

        /// <inheritdoc/>
        public ICryptoGroupElement Generator { get { return CreateGroupElement(Algebra.Generator); } }

        /// <inheritdoc/>
        public int OrderBitLength { get { return Algebra.OrderBitLength; } }

        /// <inheritdoc/>
        public int OrderByteLength { get { return (int)Math.Ceiling((double)OrderBitLength / 8); } }

        /// <inheritdoc/>
        public BigInteger Order { get { return Algebra.Order; } }

        /// <summary>
        /// <see cref="ICryptoGroup.ElementBitLength"/>
        /// </summary>
        public int ElementBitLength { get { return Algebra.ElementBitLength; } }

        /// <inheritdoc/>
        public int ElementByteLength { get { return (int)Math.Ceiling((double)ElementBitLength / 8); } }

        /// <summary>
        /// Creates a new <see cref="ICryptoGroupElement"/> instance for a given raw group element of type <typeparamref name="E"/>.
        /// </summary>
        /// <param name="value">Value of the group element as raw type.</param>
        /// <returns>An <see cref="ICryptoGroupElement"/>instance representing the given value.</returns>
        /// <remarks>
        /// Used to convert outputs of <see cref="ICryptoGroupAlgebra{E}"/> operations to <see cref="ICryptoGroupElement"/>
        /// instances by methods within this class.
        /// 
        /// Needs to be implemented by specializations.
        /// </remarks>
        protected abstract CryptoGroupElement<E> CreateGroupElement(E value);

        /// <summary>
        /// Creates a new <see cref="ICryptoGroupElement"/> instance from a byte representation of the group element.
        /// </summary>
        /// <param name="buffer">Byte representation of the group elements.</param>
        /// <returns>An <see cref="ICryptoGroupElement"/>instance representing the given value.</returns>
        /// <remarks>
        /// Needs to be implemented by specializations.
        /// </remarks>
        protected abstract CryptoGroupElement<E> CreateGroupElement(byte[] buffer);

        /// <inheritdoc/>
        public ICryptoGroupElement Add(CryptoGroupElement<E> left, CryptoGroupElement<E> right)
        {
            return CreateGroupElement(Algebra.Add(left.Value, right.Value));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement FromBytes(byte[] buffer)
        {
            return CreateGroupElement(buffer);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement Generate(BigInteger index)
        {
            return CreateGroupElement(Algebra.GenerateElement(index));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement MultiplyScalar(CryptoGroupElement<E> element, BigInteger k)
        {
            return CreateGroupElement(Algebra.MultiplyScalar(element.Value, k));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement Negate(CryptoGroupElement<E> element)
        {
            return CreateGroupElement(Algebra.Negate(element.Value));
        }

        /// <inheritdoc/>
        public ICryptoGroupElement Add(ICryptoGroupElement left, ICryptoGroupElement right)
        {
            CryptoGroupElement<E>? lhs = left as CryptoGroupElement<E>;
            CryptoGroupElement<E>? rhs = right as CryptoGroupElement<E>;
            if (lhs == null)
                throw new ArgumentException("The left summand is not an element of the group.", nameof(left));
            if (rhs == null)
                throw new ArgumentException("The right summand is not an element of the group.", nameof(left));

            return Add(lhs, rhs);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement MultiplyScalar(ICryptoGroupElement element, BigInteger k)
        {
            CryptoGroupElement<E>? e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return MultiplyScalar(e, k);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement Negate(ICryptoGroupElement element)
        {
            CryptoGroupElement<E>? e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return Negate(e);
        }

        /// <inheritdoc/>
        public Tuple<BigInteger, ICryptoGroupElement> GenerateRandom(RandomNumberGenerator rng)
        {
            BigInteger index = rng.RandomBetween(1, Order - 1);
            ICryptoGroupElement element = Generate(index);
            return new Tuple<BigInteger, ICryptoGroupElement>(index, element);
        }
    }
}
