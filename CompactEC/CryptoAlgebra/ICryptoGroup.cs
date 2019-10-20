using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactEC.CryptoAlgebra
{
    /// <summary>
    /// An algebraic group for cryptographic purposes.
    /// 
    /// Expressed from an additive perspective, i.e., with element addition
    /// and scalar multiplication as group operations. Scalars are integers
    /// from a finite field underlying the group whose characteristic is 
    /// the order of the group.
    /// 
    /// A group features a generator from which each of its elements can
    /// be obtained by scalar multiplication with a unique scalar identifier.
    ///
    /// The interface is intended to facilitate group operations as required
    /// e.g. in a Diffie-Helman key exchange and related protocols.
    /// </summary>
    /// <remarks>
    /// Note to implementers: To implement ICryptoGroup, provide an implementation
    /// of <see cref="CryptoGroupAlgebra{E}"/> and use it with
    /// <see cref="CryptoGroup{E}"/>, which is a base implementation of ICryptoGroup
    /// around <see cref="CryptoGroupAlgebra{E}"/> requiring only minimal
    /// adaptation.
    /// </remarks>
    public interface ICryptoGroup
    {
        /// <summary>
        /// Adds two group elements according to the addition semantics
        /// defined for this group.
        /// 
        /// The operation is commutative, (i.e., symmetric in its arguments).
        /// </summary>
        /// <param name="left">Group element to add.</param>
        /// <param name="right">Group element to add.</param>
        /// <returns>The result of the group addition.</returns>
        ICryptoGroupElement Add(ICryptoGroupElement left, ICryptoGroupElement right);

        /// <summary>
        /// Multiplies a group element with a scalar factor.
        /// 
        /// Scalar multiplication is understood as adding the group element to itself
        /// as many times as dictated by the scalar factor.
        /// </summary>
        /// <param name="element">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <returns>The result of the multiplication.</returns>
        ICryptoGroupElement MultiplyScalar(ICryptoGroupElement element, BigInteger k);

        /// <summary>
        /// Negates a group element.
        /// 
        /// The returned element added to the given element will thus result in the neutral element of the group.
        /// </summary>
        /// <param name="element">The group element to negate.</param>
        /// <returns>The negation of the given element in the group.</returns>
        ICryptoGroupElement Negate(ICryptoGroupElement element);

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
        ICryptoGroupElement Generate(BigInteger index);

        /// <summary>
        /// The neutral element of the group with respect to the addition operation.
        /// </summary>
        ICryptoGroupElement NeutralElement { get; }

        /// <summary>
        /// The generator of the group.
        /// 
        /// The generator is a group element that allows to generate the entire group by scalar multiplication.
        /// </summary>
        ICryptoGroupElement Generator { get; }

        /// <summary>
        /// Restores a group element from a byte representation.
        /// </summary>
        /// <param name="buffer">Byte array holding a representation of a group element.</param>
        /// <returns>The group element as an instance of ICryptoGroupElement.</returns>
        ICryptoGroupElement FromBytes(byte[] buffer);

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
        Tuple<BigInteger, ICryptoGroupElement> GenerateRandom(RandomNumberGenerator rng);

        /// <summary>
        /// The bit length of the group's order.
        /// 
        /// This is the number of bits required to represent any scalar factor.
        /// </summary>
        int OrderBitLength { get; }

        /// <summary>
        /// The length of the group's order in bytes.
        /// 
        /// This is the number of bytes required to represent any scalar factor.
        /// </summary>
        int OrderByteLength { get; }

        /// <summary>
        /// The order of the group.
        /// 
        /// The order expresses the number of unique group elements (as generated by the group's generator).
        /// </summary>
        BigInteger Order { get; }

        /// <summary>
        /// The maximum bit length of elements of the group.
        /// 
        /// This is the number of bits required to represent any element of the group.
        /// </summary>
        int ElementBitLength { get; }

        /// <summary>
        /// The maximum length of elements of the group in bytes.
        /// 
        /// This is the number of bytes required to represent any element of the group.
        /// </summary>
        int ElementByteLength { get; }
    }
}
