using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// An algebraic cyclic group for cryptographic purposes.
    /// 
    /// Expressed from an additive perspective, i.e., with element addition
    /// and scalar multiplication as group operations. Scalars are integers
    /// from a finite field underlying the group whose characteristic is 
    /// the order of the group.
    /// 
    /// A group features a generator from which each of its elements can
    /// be obtained by scalar multiplication with a unique scalar identifier.
    /// For safety reasons (i.e., resistance against small subgroup attacks)
    /// generators of <see cref="CryptoGroup{T}"/> instances are always of prime
    /// order.
    ///
    /// The interface is intended to facilitate group operations as required
    /// e.g. in a Diffie-Helman key exchange and related protocols.
    /// </summary>
    /// <remarks>
    /// Implementers of <see cref="CryptoGroup{T}"/> should provide an implementation of
    /// <see cref="ICryptoGroupAlgebra{T}"/> (preferrably by extending <see cref="CryptoGroupAlgebra{T}"/>)
    /// to realize basic algebraic operations and an extension of <see cref="CryptoGroupElement{T}"/> 
    /// which can then be employed in a specialization of <see cref="CryptoGroup{T}"/>.
    /// 
    /// All these classes are designed to have a minimum of fully virtual/abstract methods
    /// in need of implementation to provide full algebraic group functionality.
    /// </remarks>
    public abstract class CryptoGroup<T> where T: notnull
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

        /// <summary>
        /// The generator of the group.
        /// 
        /// The generator is a group element that allows to generate the entire group by scalar multiplication.
        /// </summary>
        public ICryptoGroupElement<T> Generator { get { return CreateGroupElement(Algebra.Generator); } }

        /// <summary>
        /// The bit length of the group's order.
        /// 
        /// This is the number of bits required to represent any scalar factor.
        /// </summary>
        public int OrderBitLength { get { return Algebra.OrderBitLength; } }

        /// <summary>
        /// The length of the group's order in bytes.
        /// 
        /// This is the number of bytes required to represent any scalar factor.
        /// </summary>
        public int OrderByteLength { get { return (int)Math.Ceiling((double)OrderBitLength / 8); } }

        /// <summary>
        /// The order of the group's generator.
        /// 
        /// The order expresses the number of unique group elements.
        /// </summary>
        public BigInteger Order { get { return Algebra.Order; } }

        /// <summary>
        /// The maximum bit length of elements of the group.
        /// 
        /// This is the number of bits required to represent any element of the group.
        /// </summary>
        public int ElementBitLength { get { return Algebra.ElementBitLength; } }

        /// <summary>
        /// The maximum length of elements of the group in bytes.
        /// 
        /// This is the number of bytes required to represent any element of the group.
        /// </summary>
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

        /// <summary>
        /// Adds two group elements according to the addition semantics
        /// defined for this group.
        /// 
        /// The operation is commutative, (i.e., symmetric in its arguments).
        /// </summary>
        /// <param name="left">Group element to add.</param>
        /// <param name="right">Group element to add.</param>
        /// <returns>The result of the group addition.</returns>
        public ICryptoGroupElement<T> Add(CryptoGroupElement<T> left, CryptoGroupElement<T> right)
        {
            return CreateGroupElement(Algebra.Add(left.Value, right.Value));
        }

        /// <summary>
        /// Restores a group element from a byte representation.
        /// </summary>
        /// <param name="buffer">Byte array holding a representation of a group element.</param>
        /// <returns>The group element as an instance of ICryptoGroupElement.</returns>
        public ICryptoGroupElement<T> FromBytes(byte[] buffer)
        {
            return CreateGroupElement(buffer);
        }

        /// <summary>
        /// Obtains the group element for a given index.
        /// 
        /// The element is obtained by scalar multiplication of the generator with the
        /// provided index.
        /// 
        /// To obtain a random group element, see <see cref="GenerateRandom(RandomNumberGenerator)"/>.
        /// </summary>
        /// <param name="index">A positive scalar factor less than the group's order
        ///  that uniquely identifies the element to generate.
        /// </param>
        /// <returns>The group element uniquely identified by the index.</returns>
        public ICryptoGroupElement<T> Generate(BigInteger index)
        {
            return CreateGroupElement(Algebra.GenerateElement(index));
        }

        /// <summary>
        /// Multiplies a group element with a scalar factor.
        /// 
        /// Scalar multiplication is understood as adding the group element to itself
        /// as many times as dictated by the scalar factor.
        /// </summary>
        /// <param name="element">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <returns>The result of the multiplication.</returns>
        public ICryptoGroupElement<T> MultiplyScalar(CryptoGroupElement<T> element, BigInteger k)
        {
            return CreateGroupElement(Algebra.MultiplyScalar(element.Value, k));
        }

        /// <summary>
        /// Negates a group element.
        /// 
        /// The returned element added to the given element will thus result in the neutral element of the group.
        /// </summary>
        /// <param name="element">The group element to negate.</param>
        /// <returns>The negation of the given element in the group.</returns>
        public ICryptoGroupElement<T> Negate(CryptoGroupElement<T> element)
        {
            return CreateGroupElement(Algebra.Negate(element.Value));
        }

        /// <summary>
        /// Adds two group elements according to the addition semantics
        /// defined for this group.
        /// 
        /// The operation is commutative, (i.e., symmetric in its arguments).
        /// </summary>
        /// <param name="left">Group element to add.</param>
        /// <param name="right">Group element to add.</param>
        /// <returns>The result of the group addition.</returns>
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

        /// <summary>
        /// Multiplies a group element with a scalar factor.
        /// 
        /// Scalar multiplication is understood as adding the group element to itself
        /// as many times as dictated by the scalar factor.
        /// </summary>
        /// <param name="element">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <returns>The result of the multiplication.</returns>
        public ICryptoGroupElement<T> MultiplyScalar(ICryptoGroupElement<T> element, BigInteger k)
        {
            CryptoGroupElement<T>? e = element as CryptoGroupElement<T>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return MultiplyScalar(e, k);
        }

        /// <summary>
        /// Negates a group element.
        /// 
        /// The returned element added to the given element will thus result in the neutral element of the group.
        /// </summary>
        /// <param name="element">The group element to negate.</param>
        /// <returns>The negation of the given element in the group.</returns>
        public ICryptoGroupElement<T> Negate(ICryptoGroupElement<T> element)
        {
            CryptoGroupElement<T>? e = element as CryptoGroupElement<T>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return Negate(e);
        }

        /// <summary>
        /// Generates a random group element.
        /// 
        /// The group element is obtained by first drawing an index less than the group's
        /// order uniformly at random and then multiplying the group's generator with
        /// that index.
        ///
        /// To obtain a group element for a given index, see <see cref="Generate(BigInteger)"/>.
        /// </summary>
        /// <param name="rng">A (cryptographically strong) random number generator instance from
        /// which the index will be drawn.
        /// </param>
        /// <returns>A tuple of the random index and the corresponding group element.</returns>
        public (BigInteger, ICryptoGroupElement<T>) GenerateRandom(RandomNumberGenerator rng)
        {
            BigInteger index = rng.RandomBetween(1, Order - 1);
            ICryptoGroupElement<T> element = Generate(index);
            return (index, element);
        }
    }
}
