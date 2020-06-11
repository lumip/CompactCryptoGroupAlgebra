using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// An element of a (cryptographic) algebraic group.
    /// 
    /// Every <see cref="ICryptoGroupElement"/> instance is associated with a <see cref="ICryptoGroup"/>
    /// and exposes algebraic operations to modify the element according to the group definition,
    /// such as addition with other elements from the same group as well as multiplication with a scalar.
    /// </summary>
    /// <remarks>
    /// ICryptoGroupElement abstracts from any underlying implementation and allows production code
    /// to be oblivious to the exact group implementation and the specific data types required to store group elements.
    /// </remarks>
    public interface ICryptoGroupElement : IEquatable<ICryptoGroupElement>
    {
        /// <summary>
        /// Performs a group addition with another element of the associated group.
        /// 
        /// This is an in-place operation, i.e., this istance is modified.
        /// 
        /// This is only a valid operation if passed in element is of the same
        /// group.
        /// </summary>
        /// <param name="other">The group element to add to the one represented by this instance.</param>
        void Add(ICryptoGroupElement other);

        /// <summary>
        /// Performs multiplication with a scalar within the associated group.
        /// 
        /// This is an in-place operation, i.e., this instance is modified.
        /// </summary>
        /// <param name="k">The scalar by which to multiply the element represented by this instance.</param>
        void MultiplyScalar(BigInteger k);

        /// <summary>
        /// Performs negation in the associated group.
        /// </summary>
        void Negate();

        /// <summary>
        /// Converts this group element into a unique byte representation.
        /// </summary>
        /// <returns>Byte representation of the element represented by this instance.</returns>
        byte[] ToBytes();

        /// <summary>
        /// Creates an exact clone of this instance.
        /// </summary>
        /// <returns>A new ICryptoGroupElement instance that is equal to this one.</returns>
        ICryptoGroupElement Clone();
    }
}
