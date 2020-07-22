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
    public class CryptoGroupElement<T> where T : notnull
    {
        /// <summary>
        /// Accessor to the <see cref="ICryptoGroupAlgebra{T}"/> that provides
        /// implementations of the group's underlying operations.
        /// </summary>
        /// <value>The <see cref="ICryptoGroupAlgebra{T}"/> that provides implementations of 
        /// the group's underlying operations.</value>
        public ICryptoGroupAlgebra<T> Algebra { get; }

        /// <summary>
        /// Raw value accessor.
        /// </summary>
        /// <value>The raw value.</value>
        public T Value { get; }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{T}" /> instance
        /// based on a raw <paramref name="value" /> and a <see cref="ICryptoGroupAlgebra{T}" />
        /// instance providing algebraic operations.
        /// </summary>
        /// <param name="value">Raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(T value, ICryptoGroupAlgebra<T> groupAlgebra)
        {
            Algebra = groupAlgebra;
            
            if (!Algebra.IsElement(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{T}"/> instance
        /// based on a byte representation <paramref name="valueBuffer"/> and a
        /// <see cref="ICryptoGroupAlgebra{T}"/> instance providing algebraic operations.
        /// </summary>
        /// <param name="valueBuffer">Byte representation of raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(byte[] valueBuffer, ICryptoGroupAlgebra<T> groupAlgebra)
        {
            Algebra = groupAlgebra;

            T value = Algebra.FromBytes(valueBuffer);
            if (!Algebra.IsElement(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{T}"/> instance
        /// that is an exact copy of a given <paramref name="other"/> 
        /// instance of <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="other">The <see cref="CryptoGroupElement{T}"/> instance to clone.static</param>
        public CryptoGroupElement(CryptoGroupElement<T> other)
        {
            Algebra = other.Algebra;
            Value = other.Value;
        }

        /// <summary>
        /// Adds a <see cref="CryptoGroupElement{T}"/> to the current instance following
        /// the addition law of the associated group.
        /// </summary>
        /// <param name="element">The <see cref="CryptoGroupElement{T}"/> to add.</param>
        /// <returns>The <see cref="CryptoGroupElement{T}"/> that is the group addition result of the values of
        /// <c>this</c> and <c>element</c>.</returns>
        public CryptoGroupElement<T> Add(CryptoGroupElement<T> element)
        {
            if (Algebra != element.Algebra)
                throw new ArgumentException("Added group element must be from the same group!", nameof(element));
            return new CryptoGroupElement<T>(Algebra.Add(Value, element.Value), Algebra);
        }

        /// <summary>
        /// Multiplication the current instance with a scalar within the associated group.
        /// </summary>
        /// <param name="k">The scalar by which to multiply the element represented by this instance.</param>
        /// <returns>The <see cref="CryptoGroupElement{T}"/> that is the product of <c>this</c> * <c>k</c>.</returns>
        public CryptoGroupElement<T> MultiplyScalar(BigInteger k)
        {
            return new CryptoGroupElement<T>(Algebra.MultiplyScalar(Value, k), Algebra);
        }

        /// <summary>
        /// Negates the current instance in the associated group.
        /// </summary>
        /// <returns>The <see cref="CryptoGroupElement{T}"/> that is the negation of <c>this</c>.</returns>
        public CryptoGroupElement<T> Negate()
        {
            return new CryptoGroupElement<T>(Algebra.Negate(Value), Algebra);
        }

        /// <summary>
        /// Converts this group element into a unique byte representation.
        /// </summary>
        /// <returns>Byte representation of the element represented by this instance.</returns>
        public byte[] ToBytes()
        {
            return Algebra.ToBytes(Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="CryptoGroupElement{T}"/> is equal to
        /// the current <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="other">The <see cref="CryptoGroupElement{T}"/> to compare with the current <see cref="CryptoGroupElement{T}"/>.</param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public bool Equals(CryptoGroupElement<T>? other)
        {
            return other != null && Algebra.Equals(other.Algebra) && Value.Equals(other.Value);
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
        /// Adds a <see cref="CryptoGroupElement{T}"/> to a
        /// <see cref="CryptoGroupElement{T}"/>, yielding a new <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="left">The first <see cref="CryptoGroupElement{T}"/> to add.</param>
        /// <param name="right">The second <see cref="CryptoGroupElement{T}"/> to add.</param>
        /// <returns>The <see cref="CryptoGroupElement{T}"/> that is the group addition result of the values of
        /// <c>left</c> and <c>right</c>.</returns>
        public static CryptoGroupElement<T> operator +(CryptoGroupElement<T> left, CryptoGroupElement<T> right)
        {
            return left.Add(right);
        }

        /// <summary>
        /// Negates a <see cref="CryptoGroupElement{T}"/>, yielding a new <see cref="CryptoGroupElement{T}"/> instance.
        /// </summary>
        /// <param name="e">The <see cref="CryptoGroupElement{T}"/> to negate.</param>
        /// <returns>The group negation of <see cref="CryptoGroupElement{T}"/>.</returns>
        public static CryptoGroupElement<T> operator -(CryptoGroupElement<T> e)
        {
            return e.Negate();
        }

        /// <summary>
        /// Subtracts a <see cref="CryptoGroupElement{T}"/> from a
        /// <see cref="CryptoGroupElement{T}"/>, yielding a new <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="left">The <see cref="CryptoGroupElement{T}"/> to subtract from (the minuend).</param>
        /// <param name="right">The <see cref="CryptoGroupElement{T}"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="CryptoGroupElement{T}"/> that is results from the subtraction.</returns>
        public static CryptoGroupElement<T> operator -(CryptoGroupElement<T> left, CryptoGroupElement<T> right)
        {
            return left.Add(right.Negate());
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CryptoGroupElement{T}"/>
        /// and a scalar <see cref="System.Numerics.BigInteger"/>, yielding a new <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="e">The <see cref="CryptoGroupElement{T}"/> to multiply.</param>
        /// <param name="k">The <see cref="System.Numerics.BigInteger"/> to multiply.</param>
        /// <returns>The <see cref="CryptoGroupElement{T}"/> that is the product of <c>e</c> * <c>k</c>.</returns>
        public static CryptoGroupElement<T> operator *(CryptoGroupElement<T> e, BigInteger k)
        {
            return e.MultiplyScalar(k);
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CryptoGroupElement{T}"/>
        /// and a scalar <see cref="BigInteger"/>, yielding a new <see cref="CryptoGroupElement{T}"/>.
        /// </summary>
        /// <param name="k">The <see cref="BigInteger"/> to multiply.</param>
        /// <param name="e">The <see cref="CryptoGroupElement{T}"/> to multiply.</param>
        /// <returns>The <see cref="CryptoGroupElement{T}"/> that is the product of <c>e</c> * <c>k</c>.</returns>
        public static CryptoGroupElement<T> operator *(BigInteger k, CryptoGroupElement<T> e)
        {
            return e * k;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"<CryptoGroupElement: {Value.ToString()}>";
        }
    }
}
