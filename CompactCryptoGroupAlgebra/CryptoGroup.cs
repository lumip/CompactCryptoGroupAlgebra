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
    /// e.g. in a Diffie-Hellman key exchange and related protocols.
    /// </summary>
    /// <typeparam name="T">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    /// <remarks>
    /// <see cref="CryptoGroup{T}"/> provides a high-level API of algebraic operations
    /// implemented in a <see cref="ICryptoGroupAlgebra{T}"/> instance, wrapping all
    /// basic element type values in <see cref="CryptoGroupElement{T}" /> instances that
    /// offer operator overloading.
    /// </remarks>
    public class CryptoGroup<T> where T: notnull
    {
        /// <summary>
        /// The <see cref="ICryptoGroupAlgebra{T}"/> that provides
        /// implementations of underlying group operations on raw group element type
        /// <typeparamref name="T"/>.
        /// </summary>
        public ICryptoGroupAlgebra<T> Algebra { get; }

        /// <summary>
        /// Initializes a <see cref="CryptoGroup{T}"/> instance using a given <see cref="ICryptoGroupAlgebra{T}"/> instance
        /// for algebraic operations.
        /// </summary>
        /// <param name="algebra">Group algebra implementation on raw data type <typeparamref name="T"/></param>
        public CryptoGroup(ICryptoGroupAlgebra<T> algebra)
        {
            Algebra = algebra;
        }

        /// <summary>
        /// The generator of the group.
        /// 
        /// The generator is a group element that allows to generate the entire group by scalar multiplication.
        /// </summary>
        public CryptoGroupElement<T> Generator { get { return CreateGroupElement(Algebra.Generator); } }

        /// <summary>
        /// The length (in binary digits) of the group's order.
        /// 
        /// This is the number of bytes required to represent any scalar factor.
        /// </summary>
        public NumberLength OrderLength { get { return NumberLength.FromBitLength(Algebra.OrderBitLength); } }

        /// <summary>
        /// The order of the group's generator.
        /// 
        /// The order expresses the number of unique group elements.
        /// </summary>
        public BigPrime Order { get { return Algebra.Order; } }

        /// <summary>
        /// The maximum length (in binary digits) required to represent elements of the group.
        /// </summary>
        public NumberLength ElementLength { get { return NumberLength.FromBitLength(Algebra.ElementBitLength); } }

        /// <summary>
        /// Creates a new <see cref="CryptoGroupElement{T}"/> instance for a given raw group element of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">Value of the group element as raw type.</param>
        /// <returns>An <see cref="CryptoGroupElement{T}"/>instance representing the given value.</returns>
        /// <remarks>
        /// Used to convert outputs of <see cref="ICryptoGroupAlgebra{T}"/> operations to <see cref="CryptoGroupElement{T}"/>
        /// instances by methods within this class.
        /// </remarks>
        protected virtual CryptoGroupElement<T> CreateGroupElement(T value)
        {
            return new CryptoGroupElement<T>(value, Algebra);
        }

        /// <summary>
        /// Creates a new <see cref="CryptoGroupElement{T}"/> instance from a byte representation of the group element.
        /// </summary>
        /// <param name="buffer">Byte representation of the group elements.</param>
        /// <returns>An <see cref="CryptoGroupElement{T}"/>instance representing the given value.</returns>
        protected virtual CryptoGroupElement<T> CreateGroupElement(byte[] buffer)
        {
            return new CryptoGroupElement<T>(buffer, Algebra);
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
        public CryptoGroupElement<T> Add(CryptoGroupElement<T> left, CryptoGroupElement<T> right)
        {
            if (!left.Algebra.Equals(Algebra))
                throw new ArgumentException("The argument is not an element of the group.", nameof(left));
            if (!right.Algebra.Equals(Algebra))
                throw new ArgumentException("The argument is not an element of the group.", nameof(right));
            return CreateGroupElement(Algebra.Add(left.Value, right.Value));
        }

        /// <summary>
        /// Restores a group element from a byte representation.
        /// </summary>
        /// <param name="buffer">Byte array holding a representation of a group element.</param>
        /// <returns>The group element as an instance of CryptoGroupElement.</returns>
        public CryptoGroupElement<T> FromBytes(byte[] buffer)
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
        public CryptoGroupElement<T> Generate(BigInteger index)
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
        public CryptoGroupElement<T> MultiplyScalar(CryptoGroupElement<T> element, BigInteger k)
        {
            if (!element.Algebra.Equals(Algebra))
                throw new ArgumentException("The argument is not an element of the group.", nameof(element));
            return CreateGroupElement(Algebra.MultiplyScalar(element.Value, k));
        }

        /// <summary>
        /// Negates a group element.
        /// 
        /// The returned element added to the given element will thus result in the neutral element of the group.
        /// </summary>
        /// <param name="element">The group element to negate.</param>
        /// <returns>The negation of the given element in the group.</returns>
        public CryptoGroupElement<T> Negate(CryptoGroupElement<T> element)
        {
            if (!element.Algebra.Equals(Algebra))
                throw new ArgumentException("The argument is not an element of the group.", nameof(element));
            return CreateGroupElement(Algebra.Negate(element.Value));
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
        /// <param name="randomNumberGenerator">A (cryptographically strong) random number generator instance from
        /// which the index will be drawn.
        /// </param>
        /// <returns>A tuple of the random index and the corresponding group element.</returns>
        public (BigInteger, CryptoGroupElement<T>) GenerateRandom(RandomNumberGenerator randomNumberGenerator)
        {
            BigInteger index = randomNumberGenerator.RandomBetween(1, Order - 1);
            CryptoGroupElement<T> element = Generate(index);
            return (index, element);
        }
    }
}
