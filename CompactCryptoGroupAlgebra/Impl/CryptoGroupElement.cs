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
    /// Every <see cref="CryptoGroupElement{T}"/>instance is associated with a <see cref="CryptoGroup{T}"/>
    /// and exposes algebraic operations to modify the element according to the group definition,
    /// such as addition with other elements from the same group as well as multiplication with a scalar.
    /// </summary>
    /// <typeparam name="T">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    /// <inheritdoc/>
    public class CryptoGroupElement<T> : ICryptoGroupElement<T> where T : notnull // todo: consider making immutable
    {
        /// <summary>
        /// Subclass accessor to the <see cref="ICryptoGroupAlgebra{T}"/> that provides
        /// implementations of the group's underlying operations.
        /// </summary>
        /// <value>The <see cref="ICryptoGroupAlgebra{T}"/> that provides implementations of 
        /// the group's underlying operations.</value>
        protected ICryptoGroupAlgebra<T> Algebra { get; }

        /// <summary>
        /// Raw value accessor.
        /// </summary>
        /// <value>The raw value.</value>
        public T Value { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{T}"/> instance
        /// based on a raw <paramref name="value"/> and a <see cref="ICryptoGroupAlgebra{T}"/>
        /// instance providing algebraic operations.
        /// </summary>
        /// <param name="value">Raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(T value, ICryptoGroupAlgebra<T> groupAlgebra)
        {
            Algebra = groupAlgebra;
            
            if (!Algebra.IsValid(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{T}"/> instance
        /// based on a byte representation <paramref name="valueBuffer"/> and a
        /// <see cref="ICryptoGroupAlgebra{T}"/> instance providing algebraic operations.
        /// </summary>
        /// <param name="valueBuffer">Byte represenation of raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(byte[] valueBuffer, ICryptoGroupAlgebra<T> groupAlgebra)
        {
            Algebra = groupAlgebra;

            T value = Algebra.FromBytes(valueBuffer);
            if (!Algebra.IsValid(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <inheritdoc/>
        public void Add(ICryptoGroupElement<T> element)
        {
            CryptoGroupElement<T>? e = element as CryptoGroupElement<T>;
            if (e == null)
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(Value));

            Add(e);
        }

        /// <summary>
        /// <see cref="ICryptoGroupElement{T}.Add(ICryptoGroupElement{T})"/>
        /// </summary>
        /// <remarks>
        /// Overloaded variant for <see cref="CryptoGroupElement{T}"/>
        /// </remarks>
        public void Add(CryptoGroupElement<T> element)
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
        /// Creates a clone of this <see cref="CryptoGroup{T}"/> instance.
        /// </summary>
        /// <returns>A new copy of the current <see cref="CryptoGroup{T}"/>.</returns>
        /// <remarks>Used extensively in the operator overload implementations.</remarks>
        protected CryptoGroupElement<T> CloneInternal()
        {
            return new CryptoGroupElement<T>(Value, Algebra);
        }

        /// <inheritdoc/>
        public ICryptoGroupElement<T> Clone()
        {
            return CloneInternal();
        }

        /// <summary>
        /// Determines whether the specified <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> is equal to
        /// the current <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="other">The <see cref="CryptoGroupElement{T}"/> to compare with the current <see cref="CryptoGroupElement{T}"/>.</param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public bool Equals(CryptoGroupElement<T>? other)
        {
            return other != null && Algebra.Equals(other.Algebra) && Value.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="ICryptoGroupElement{T}"/> is equal to the
        /// the current <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="other">The <see cref="ICryptoGroupElement{T}"/> to compare with the current <see cref="CryptoGroupElement{T}"/>.</param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public bool Equals(ICryptoGroupElement<T> other)
        {
            return Equals(other as CryptoGroupElement<T>);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to
        /// the current <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="CryptoGroupElement{T}"/>.</param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as CryptoGroupElement<T>);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -1217399511;
            hashCode = hashCode * -1521134295 + EqualityComparer<ICryptoGroupAlgebra<T>>.Default.GetHashCode(Algebra);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
            return hashCode;
        }

        /// <summary>
        /// Adds a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> to a
        /// <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement{T}"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="left">The first <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> to add.</param>
        /// <param name="right">The second <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement{T}"/> to add.</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> that is the group addition result of the values of
        /// <c>left</c> and <c>right</c>.</returns>
        public static CryptoGroupElement<T> operator +(CryptoGroupElement<T> left, ICryptoGroupElement<T> right)
        {
            var result = left.CloneInternal();
            result.Add(right);
            return result;
        }

        /// <summary>
        /// Negates a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> instance.
        /// </summary>
        /// <param name="e">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> to negate.</param>
        /// <returns>The group negation of <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>.</returns>
        public static CryptoGroupElement<T> operator -(CryptoGroupElement<T> e)
        {
            var result = e.CloneInternal();
            result.Negate();
            return result;
        }

        /// <summary>
        /// Subtracts a <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement{T}"/> from a
        /// <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="left">The <see cref="CompactCryptoGroupAlgebra.ICryptoGroupElement{T}"/> to subtract from (the minuend).</param>
        /// <param name="right">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> that is results from the subtraction.</returns>
        public static CryptoGroupElement<T> operator -(ICryptoGroupElement<T> left, CryptoGroupElement<T> right)
        {
            var result = right.CloneInternal();
            result.Negate();
            result.Add(left);
            return result;
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>
        /// and a scalar <see cref="System.Numerics.BigInteger"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="e">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> to multiply.</param>
        /// <param name="k">The <see cref="System.Numerics.BigInteger"/> to multiply.</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> that is the product of <c>e</c> * <c>k</c>.</returns>
        public static CryptoGroupElement<T> operator *(CryptoGroupElement<T> e, BigInteger k)
        {
            var result = e.CloneInternal();
            result.MultiplyScalar(k);
            return result;
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>
        /// and a scalar <see cref="System.Numerics.BigInteger"/>, yielding a new <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="k">The <see cref="System.Numerics.BigInteger"/> to multiply.</param>
        /// <param name="e">The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> to multiply.</param>
        /// <returns>The <see cref="CompactCryptoGroupAlgebra.CryptoGroupElement{T}"/> that is the product of <c>e</c> * <c>k</c>.</returns>
        public static CryptoGroupElement<T> operator *(BigInteger k, CryptoGroupElement<T> e)
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
