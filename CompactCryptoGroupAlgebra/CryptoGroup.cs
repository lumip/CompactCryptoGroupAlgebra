// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography
// Copyright (C) 2020  Lukas <lumip> Prediger
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
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
    /// generators of <see cref="CryptoGroup{TScalar, TElement}"/> instances
    /// are always of prime order.
    ///
    /// The interface is intended to facilitate group operations as required
    /// e.g. in a Diffie-Hellman key exchange and related protocols.
    /// </summary>
    /// <typeparam name="TScalar">
    /// The data type used to represent a scalar number for indexing group elements.
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The data type used for raw group elements the algebraic operations operate on.
    /// </typeparam>
    /// <remarks>
    /// <see cref="CryptoGroup{TScalar, TElement}"/> provides a high-level API
    /// of algebraic operations implemented in a <see cref="ICryptoGroupAlgebra{TScalar, TElement}"/>
    /// instance, wrapping all basic element type values in
    /// <see cref="CryptoGroupElement{TScalar, TElement}" /> instances that
    /// offer operator overloading.
    /// </remarks>
    public class CryptoGroup<TScalar, TElement> where TScalar : notnull where TElement : notnull
    {

        /// <summary>
        /// The <see cref="ICryptoGroupAlgebra{TScalar, TElement}"/> that provides
        /// implementations of underlying group operations on raw group element type
        /// <typeparamref name="TElement"/>.
        /// </summary>
        public ICryptoGroupAlgebra<TScalar, TElement> Algebra { get; }

        /// <summary>
        /// The security level of the group.
        ///
        /// The security level is defined as the value λ such that the expected time it takes
        /// an adversary's to solve the discrete logarithm, i.e., finding the secret scalar
        /// that generates a group element with probability at least <c>p</c> is
        /// is at least <c>p 2^λ</c>.
        /// </summary>
        public int SecurityLevel => Algebra.SecurityLevel;

        /// <summary>
        /// Initializes a <see cref="CryptoGroup{TScalar, TElement}"/> instance using a
        /// given <see cref="ICryptoGroupAlgebra{TScalar, TElement}"/> instance
        /// for algebraic operations.
        /// </summary>
        /// <param name="algebra">Group algebra implementation on raw data type <typeparamref name="TElement"/></param>
        public CryptoGroup(ICryptoGroupAlgebra<TScalar, TElement> algebra)
        {
            Algebra = algebra;
        }

        /// <summary>
        /// The generator of the group.
        /// 
        /// The generator is a group element that allows to generate the entire group by scalar multiplication.
        /// </summary>
        public CryptoGroupElement<TScalar, TElement> Generator { get { return CreateGroupElement(Algebra.Generator); } }

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
        /// Creates a new <see cref="CryptoGroupElement{TScalar, TElement}"/> instance for a
        /// given raw group element of type <typeparamref name="TElement"/>.
        /// </summary>
        /// <param name="value">Value of the group element as raw type.</param>
        /// <returns>
        /// A <see cref="CryptoGroupElement{TScalar, TElement}"/> instance
        /// representing the given value.
        /// </returns>
        /// <remarks>
        /// Used to convert outputs of <see cref="ICryptoGroupAlgebra{TScalar, TElement}"/>
        /// operations to <see cref="CryptoGroupElement{TScalar, TElement}"/>
        /// instances by methods within this class.
        /// </remarks>
        private CryptoGroupElement<TScalar, TElement> CreateGroupElement(TElement value)
        {
            return new CryptoGroupElement<TScalar, TElement>(value, Algebra);
        }

        /// <summary>
        /// Creates a new <see cref="CryptoGroupElement{TScalar, TElement}"/> instance from a
        /// byte representation of the group element.
        /// </summary>
        /// <param name="buffer">Byte representation of the group elements.</param>
        /// <returns>
        /// An <see cref="CryptoGroupElement{TScalar, TElement}"/> instance
        /// representing the given value.
        /// </returns>
        private CryptoGroupElement<TScalar, TElement> CreateGroupElement(byte[] buffer)
        {
            return new CryptoGroupElement<TScalar, TElement>(buffer, Algebra);
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
        public CryptoGroupElement<TScalar, TElement> Add(
            CryptoGroupElement<TScalar, TElement> left, CryptoGroupElement<TScalar, TElement> right
        )
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
        public CryptoGroupElement<TScalar, TElement> FromBytes(byte[] buffer)
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
        public CryptoGroupElement<TScalar, TElement> Generate(TScalar index)
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
        public CryptoGroupElement<TScalar, TElement> MultiplyScalar(
            CryptoGroupElement<TScalar, TElement> element, TScalar k
        )
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
        public CryptoGroupElement<TScalar, TElement> Negate(CryptoGroupElement<TScalar, TElement> element)
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
        /// To obtain a group element for a given index, see <see cref="Generate(TScalar)"/>.
        /// </summary>
        /// <param name="randomNumberGenerator">A (cryptographically strong) random number generator instance from
        /// which the index will be drawn.
        /// </param>
        /// <returns>A tuple of the random index and the corresponding group element.</returns>
        public (TScalar, CryptoGroupElement<TScalar, TElement>) GenerateRandom(RandomNumberGenerator randomNumberGenerator)
        {
            (var index, var element) = Algebra.GenerateRandomElement(randomNumberGenerator);
            return (index, CreateGroupElement(element));
        }
    }
}
