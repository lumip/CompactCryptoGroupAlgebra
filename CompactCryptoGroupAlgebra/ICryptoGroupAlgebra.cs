﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Algebraic group operations provider.
    /// 
    /// Used by <see cref="CryptoGroup{T}"/> as the implementation of
    /// the respective group operations.
    /// </summary>
    /// <typeparam name="T">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    /// <remarks>
    /// ICryptoGroupAlgebra should not used directly by productive code. Use <see cref="ICryptoGroup{T}"/>
    /// instead.
    /// 
    /// ICryptoGroupAlgebra is intended to facilitate implementing the <see cref="ICryptoGroup{T}"/>
    /// interface by allowing an implementer to focus on the actual group operations without
    /// having to deal with boilerplate constructs. The aim is mostly to avoid code duplication.
    /// In the same manner, <see cref="CryptoGroupAlgebra{T}"/> implements this interface and
    /// provides useful default implementations for certain operations. It is the class that
    /// an implementer should extend to implement this interface.
    /// </remarks>
    public interface ICryptoGroupAlgebra<T> where T : notnull
    {
        /// <summary>
        /// The order of the group.
        /// 
        /// The order expresses the number of unique group elements (as generated by the group's generator).
        /// </summary>
        BigInteger Order { get; }

        /// <summary>
        /// The generator of the group.
        /// 
        /// The generator is a group element that allows to generate the entire group by scalar multiplication.
        /// </summary>
        T Generator { get; }

        /// <summary>
        /// The neutral element of the group with respect to the addition operation.
        /// </summary>
        T NeutralElement { get; }

        /// <summary>
        /// The cofactor of the defined cryptographic group algebra curve.
        ///
        /// The cofactor is the ratio of the order of the embedding and
        /// the embedded subgroup represented by the current <see cref="ICryptoGroupAlgebra{T}"/>.
        /// </summary>
        BigInteger Cofactor { get; }

        /// <summary>
        /// The maximum bit length of elements of the group.
        /// 
        /// This is the number of bits required to represent any element of the group.
        /// </summary>
        int ElementBitLength { get; }

        /// <summary>
        /// The bit length of the group's order.
        /// 
        /// This is the number of bits required to represent any scalar factor.
        /// </summary>
        int OrderBitLength { get; }

        /// <summary>
        /// Generates a group element.
        /// 
        /// The element is obtained by scalar multiplication of the generator with the
        /// provided index.
        /// </summary>
        /// <param name="index">A positive scalar factor less than the group's order
        ///     that uniquely identifies the element to generate.
        /// </param>
        /// <returns>The group element uniquely identified by the index.</returns>
        T GenerateElement(BigInteger index);

        /// <summary>
        /// Negates a group element.
        /// 
        /// The returned element added to the given element will result in the neutral element of the group.
        /// </summary>
        /// <param name="element">The group element to negate.</param>
        /// <returns>The negation of the given element in the group.</returns>
        T Negate(T element);

        /// <summary>
        /// Multiplies a group element with a scalar factor.
        /// 
        /// Scalar multiplication is understood as adding the group element to itself
        /// as many times as dictated by the scalar factor.
        /// </summary>
        /// <param name="e">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <returns>The given element multiplied with the given scalar.</returns>
        T MultiplyScalar(T e, BigInteger k);

        /// <summary>
        /// Adds two group elements according to the addition semantics
        /// defined for this group.
        /// 
        /// The operation is commutative, (i.e., symmetric in its arguments).
        /// </summary>
        /// <param name="left">Group element to add.</param>
        /// <param name="right">Group element to add.</param>
        /// <returns>The result of the group addition.</returns>
        T Add(T left, T right);

        /// <summary>
        /// Checks whether an input of group element type is a valid and safe element of the group.
        /// 
        /// To be valid, an element has to verify two conditions:
        /// - be an element of the mathematical structure the group is defined in,
        /// - be of group order (to prevent subgroup attacks).
        /// </summary>
        /// <param name="element">The group element type object to check for validity.</param>
        /// <returns><c>true</c> if the given input is a valid group element.</returns>
        /// <remarks>
        /// Note that an element is not required to be generated by the group
        /// generator to be considered valid as long as its order is identical
        /// to the group order.
        /// </remarks>
        bool IsValid(T element); // todo: rename to IsGroupElement or Contains

        /// <summary>
        /// Restores a group element from a byte representation.
        /// </summary>
        /// <param name="buffer">Byte array holding a representation of a group element.</param>
        /// <returns>The loaded group element.</returns>
        T FromBytes(byte[] buffer);

        /// <summary>
        /// Converts a group element into a byte representation.
        /// </summary>
        /// <param name="element">The group element to convert.</param>
        /// <returns>A byte array holding a representation of the group element.</returns>
        byte[] ToBytes(T element);
    }
}
