using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CompactCryptoGroupAlgebra.Tests")]
namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// An element of a (cryptographic) algebraic group.
    /// 
    /// Every <see cref="CryptoGroupElement{E}"/>instance is associated with a <see cref="CryptoGroup{E}"/>
    /// and exposes algebraic operations to modify the element according to the group definition,
    /// such as addition with other elements from the same group as well as multiplication with a scalar.
    /// </summary>
    /// <typeparam name="E">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    /// <inheritdoc/>
    public class CryptoGroupElement<E> : ICryptoGroupElement where E : struct
    {
        /// <summary>
        /// Subclass accessor to the <see cref="ICryptoGroupAlgebra{E}"/> that provides
        /// implementations of the group's underlying operations.
        /// </summary>
        /// <value>The <see cref="ICryptoGroupAlgebra{E}"/> that provides implementations of 
        /// the group's underlying operations.</value>
        protected ICryptoGroupAlgebra<E> Algebra { get; }

        /// <summary>
        /// Raw value accessor.
        /// </summary>
        /// <value>The raw value.</value>
        public E Value { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{E}"/> instance
        /// based on a raw <paramref name="value"/> and a <see cref="ICryptoGroupAlgebra{E}"/>
        /// instance providing algebraic operations.
        /// </summary>
        /// <param name="value">Raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(E value, ICryptoGroupAlgebra<E> groupAlgebra)
        {
            Algebra = groupAlgebra;
            
            if (!Algebra.IsValid(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{E}"/> instance
        /// based on a byte representation <paramref name="valueBuffer"/> and a
        /// <see cref="ICryptoGroupAlgebra{E}"/> instance providing algebraic operations.
        /// </summary>
        /// <param name="valueBuffer">Byte represenation of raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(byte[] valueBuffer, ICryptoGroupAlgebra<E> groupAlgebra)
        {
            Algebra = groupAlgebra;

            E value = Algebra.FromBytes(valueBuffer);
            if (!Algebra.IsValid(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <inheritdoc/>
        public void Add(ICryptoGroupElement element)
        {
            CryptoGroupElement<E>? e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(Value));

            Add(e);
        }

        /// <summary>
        /// <see cref="ICryptoGroupElement.Add(ICryptoGroupElement)"/>
        /// </summary>
        /// <remarks>
        /// Overloaded variant for <see cref="CryptoGroupElement{E}"/>
        /// </remarks>
        public void Add(CryptoGroupElement<E> element)
        {
            if (Algebra != element.Algebra)
                throw new ArgumentException("Added group element must be from the same group!", nameof(element));
            Value = Algebra.Add(Value, element.Value);
        }

        /// <inheritdoc/>
        public void MultiplyScalar(BigInteger k)
        {
            Value = Algebra.MultiplyScalar(Value, k);
        }

        /// <inheritdoc/>
        public void Negate()
        {
            Value = Algebra.Negate(Value);
        }

        /// <inheritdoc/>
        public byte[] ToBytes()
        {
            return Algebra.ToBytes(Value);
        }

        /// <summary>
        /// Creates a clone of this <see cref="CryptoGroup{E}"/> instance.
        /// </summary>
        /// <returns>A new copy of the current <see cref="CryptoGroup{E}"/>.</returns>
        /// <remarks>Used extensively in the operator overload implementations.</remarks>
        protected CryptoGroupElement<E> CloneInternal()
        {
            return new CryptoGroupElement<E>(Value, Algebra);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement Clone()
        {
            return CloneInternal();
        }

        /// <summary>
        /// Determines whether the specified <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> is equal to
        /// the current <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.
        /// </summary>
        /// <param name="other">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> to compare with the current <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.</param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public bool Equals(CryptoGroupElement<E>? other)
        {
            return other != null && Algebra.Equals(other.Algebra) && Value.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement"/> is equal to the
        /// the current <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.
        /// </summary>
        /// <param name="other">The <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement"/> to compare with the current <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.</param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public bool Equals(ICryptoGroupElement other)
        {
            return Equals(other as CryptoGroupElement<E>);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to
        /// the current <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.</param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as CryptoGroupElement<E>);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -1217399511;
            hashCode = hashCode * -1521134295 + EqualityComparer<ICryptoGroupAlgebra<E>>.Default.GetHashCode(Algebra);
            hashCode = hashCode * -1521134295 + EqualityComparer<E>.Default.GetHashCode(Value);
            return hashCode;
        }

        /// <summary>
        /// Adds a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> to a
        /// <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.
        /// </summary>
        /// <param name="left">The first <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> to add.</param>
        /// <param name="right">The second <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement"/> to add.</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> that is the group addition result of the values of
        /// <c>left</c> and <c>right</c>.</returns>
        public static CryptoGroupElement<E> operator +(CryptoGroupElement<E> left, ICryptoGroupElement right)
        {
            var result = left.CloneInternal();
            result.Add(right);
            return result;
        }

        /// <summary>
        /// Negates a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> instance.
        /// </summary>
        /// <param name="e">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> to negate.</param>
        /// <returns>The group negation of <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.</returns>
        public static CryptoGroupElement<E> operator -(CryptoGroupElement<E> e)
        {
            var result = e.CloneInternal();
            result.Negate();
            return result;
        }

        /// <summary>
        /// Subtracts a <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement"/> from a
        /// <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.
        /// </summary>
        /// <param name="left">The <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement"/> to subtract from (the minuend).</param>
        /// <param name="right">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> that is results from the subtraction.</returns>
        public static CryptoGroupElement<E> operator -(ICryptoGroupElement left, CryptoGroupElement<E> right)
        {
            var result = right.CloneInternal();
            result.Negate();
            result.Add(left);
            return result;
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>
        /// and a scalar <see cref="System.Numerics.BigInteger"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.
        /// </summary>
        /// <param name="e">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> to multiply.</param>
        /// <param name="k">The <see cref="System.Numerics.BigInteger"/> to multiply.</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> that is the product of <c>e</c> * <c>k</c>.</returns>
        public static CryptoGroupElement<E> operator *(CryptoGroupElement<E> e, BigInteger k)
        {
            var result = e.CloneInternal();
            result.MultiplyScalar(k);
            return result;
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>
        /// and a scalar <see cref="System.Numerics.BigInteger"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/>.
        /// </summary>
        /// <param name="k">The <see cref="System.Numerics.BigInteger"/> to multiply.</param>
        /// <param name="e">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> to multiply.</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{E}"/> that is the product of <c>e</c> * <c>k</c>.</returns>
        public static CryptoGroupElement<E> operator *(BigInteger k, CryptoGroupElement<E> e)
        {
            return e * k;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("<CryptoGroupElement: {0}>", Value.ToString());
        }
    }
}
