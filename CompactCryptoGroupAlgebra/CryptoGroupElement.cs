// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2020-2021 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CompactCryptoGroupAlgebra.Tests")]
namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// An element of a (cryptographic) algebraic group.
    /// 
    /// Every <see cref="CryptoGroupElement{TScalar, TElement}"/> instance is associated with
    /// a <see cref="CryptoGroup{TScalar, TElement}"/> and exposes algebraic operations to
    /// modify the element according to the group definition, such as addition with other
    /// elements from the same group as well as multiplication with a scalar.
    /// </summary>
    /// <typeparam name="TScalar">
    /// The data type used to represent a scalar number for indexing group elements.
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The data type used for raw group elements the algebraic operations operate on.
    /// </typeparam>
    public class CryptoGroupElement<TScalar, TElement> where TScalar : notnull where TElement: notnull
    {
        /// <summary>
        /// Accessor to the <see cref="ICryptoGroupAlgebra{TScalar, TElement}"/> that provides
        /// implementations of the group's underlying operations.
        /// </summary>
        /// <value>The <see cref="ICryptoGroupAlgebra{TScalar, TElement}"/> that provides
        /// implementations of the group's underlying operations.</value>
        public ICryptoGroupAlgebra<TScalar, TElement> Algebra { get; }

        /// <summary>
        /// Raw value accessor.
        /// </summary>
        /// <value>The raw value.</value>
        public TElement Value { get; }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{TScalar, TElement}" /> instance
        /// based on a raw <paramref name="value" /> and a <see cref="ICryptoGroupAlgebra{TScalar, TElement}" />
        /// instance providing algebraic operations.
        /// </summary>
        /// <param name="value">Raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(TElement value, ICryptoGroupAlgebra<TScalar, TElement> groupAlgebra)
        {
            Algebra = groupAlgebra;
            
            if (!Algebra.IsPotentialElement(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{TScalar, TElement}"/>
        /// instance based on a byte representation <paramref name="valueBuffer"/> and a
        /// <see cref="ICryptoGroupAlgebra{TScalar, TElement}"/> instance providing algebraic operations.
        /// </summary>
        /// <param name="valueBuffer">Byte representation of raw value.</param>
        /// <param name="groupAlgebra">Corresponding group algebra instance.</param>
        protected internal CryptoGroupElement(byte[] valueBuffer, ICryptoGroupAlgebra<TScalar, TElement> groupAlgebra)
        {
            Algebra = groupAlgebra;

            TElement value = Algebra.FromBytes(valueBuffer);
            if (!Algebra.IsPotentialElement(value))
                throw new ArgumentException("The provided value is not a valid element of the group.", nameof(value));
            Value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoGroupElement{TScalar, TElement}"/> instance
        /// that is an exact copy of a given <paramref name="other"/> 
        /// instance of <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> instance to clone.
        /// </param>
        public CryptoGroupElement(CryptoGroupElement<TScalar, TElement> other)
        {
            Algebra = other.Algebra;
            Value = other.Value;
        }

        /// <summary>
        /// Whether the element is a safe member of the group.
        /// 
        /// To be considered safe, an element must have the same order as
        /// the group itself.
        /// </summary>
        /// <remarks>
        /// The neutral element is a trivial example of an element that is a group
        /// member but not safe.
        /// </remarks>
        /// <value>True, if the element is safe.</value>
        public bool IsSafe => Algebra.IsSafeElement(Value);

        /// <summary>
        /// Adds a <see cref="CryptoGroupElement{TScalar, TElement}"/> to the current instance following
        /// the addition law of the associated group.
        /// </summary>
        /// <param name="element">The <see cref="CryptoGroupElement{TScalar, TElement}"/> to add.</param>
        /// <returns>
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> that is the group addition
        /// result of the values of <c>this</c> and <c>element</c>.
        /// </returns>
        public CryptoGroupElement<TScalar, TElement> Add(CryptoGroupElement<TScalar, TElement> element)
        {
            if (Algebra != element.Algebra)
                throw new ArgumentException("Added group element must be from the same group!", nameof(element));
            return new CryptoGroupElement<TScalar, TElement>(Algebra.Add(Value, element.Value), Algebra);
        }

        /// <summary>
        /// Multiplication the current instance with a scalar within the associated group.
        /// </summary>
        /// <param name="k">The scalar by which to multiply the element represented by this instance.</param>
        /// <returns>
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> that is the
        /// product of <c>this</c> * <c>k</c>.
        /// </returns>
        public CryptoGroupElement<TScalar, TElement> MultiplyScalar(TScalar k)
        {
            return new CryptoGroupElement<TScalar, TElement>(Algebra.MultiplyScalar(Value, k), Algebra);
        }

        /// <summary>
        /// Negates the current instance in the associated group.
        /// </summary>
        /// <returns>
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> that is
        /// the negation of <c>this</c>.
        /// </returns>
        public CryptoGroupElement<TScalar, TElement> Negate()
        {
            return new CryptoGroupElement<TScalar, TElement>(Algebra.Negate(Value), Algebra);
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
        /// Determines whether the specified <see cref="CryptoGroupElement{TScalar, TElement}"/>
        /// is equal to the current <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </summary>
        /// <param name="other">
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> to compare with the
        /// current <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public bool Equals(CryptoGroupElement<TScalar, TElement>? other)
        {
            return other != null && Algebra.Equals(other.Algebra) && Value.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to
        /// the current <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="object"/> to compare with the current
        /// <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </param>
        /// <returns><c>true</c> no equality; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as CryptoGroupElement<TScalar, TElement>);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -1217399511;
            hashCode = hashCode * -1521134295 + EqualityComparer<ICryptoGroupAlgebra<TScalar, TElement>>.Default.GetHashCode(Algebra);
            hashCode = hashCode * -1521134295 + EqualityComparer<TElement>.Default.GetHashCode(Value);
            return hashCode;
        }

        /// <summary>
        /// Adds a <see cref="CryptoGroupElement{TScalar, TElement}"/> to a
        /// <see cref="CryptoGroupElement{TScalar, TElement}"/>, yielding
        /// a new <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </summary>
        /// <param name="left">The first <see cref="CryptoGroupElement{TScalar, TElement}"/> to add.</param>
        /// <param name="right">The second <see cref="CryptoGroupElement{TScalar, TElement}"/> to add.</param>
        /// <returns>
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> that is the group addition result of 
        /// the values of<c>left</c> and <c>right</c>.
        /// </returns>
        public static CryptoGroupElement<TScalar, TElement> operator +(
            CryptoGroupElement<TScalar, TElement> left, CryptoGroupElement<TScalar, TElement> right
        )
        {
            return left.Add(right);
        }

        /// <summary>
        /// Negates a <see cref="CryptoGroupElement{TScalar, TElement}"/>, yielding
        /// a new <see cref="CryptoGroupElement{TScalar, TElement}"/> instance.
        /// </summary>
        /// <param name="e">The <see cref="CryptoGroupElement{TScalar, TElement}"/> to negate.</param>
        /// <returns>The group negation of <see cref="CryptoGroupElement{TScalar, TElement}"/>.</returns>
        public static CryptoGroupElement<TScalar, TElement> operator -(CryptoGroupElement<TScalar, TElement> e)
        {
            return e.Negate();
        }

        /// <summary>
        /// Subtracts a <see cref="CryptoGroupElement{TScalar, TElement}"/> from a
        /// <see cref="CryptoGroupElement{TScalar, TElement}"/>, yielding
        /// a new <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> to subtract from (the minuend).
        /// </param>
        /// <param name="right">
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> to subtract (the subtrahend).
        /// </param>
        /// <returns>
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> that is results from the subtraction.
        /// </returns>
        public static CryptoGroupElement<TScalar, TElement> operator -(
            CryptoGroupElement<TScalar, TElement> left, CryptoGroupElement<TScalar, TElement> right
        )
        {
            return left.Add(right.Negate());
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CryptoGroupElement{TScalar, TElement}"/>
        /// and a scalar <typenameref name="TScalar"/>, yielding
        /// a new <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </summary>
        /// <param name="e">The <see cref="CryptoGroupElement{TScalar, TElement}"/> to multiply.</param>
        /// <param name="k">The <typeparamref name="TScalar"/> to multiply.</param>
        /// <returns>
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> that is the product of <c>e</c> * <c>k</c>.
        /// </returns>
        public static CryptoGroupElement<TScalar, TElement> operator *(CryptoGroupElement<TScalar, TElement> e, TScalar k)
        {
            return e.MultiplyScalar(k);
        }

        /// <summary>
        /// Computes the multiplication of a <see cref="CryptoGroupElement{TScalar, TElement}"/>
        /// and a scalar <typenameref name="TScalar"/>, yielding
        /// a new <see cref="CryptoGroupElement{TScalar, TElement}"/>.
        /// </summary>
        /// <param name="k">The <typenameref name="TScalar"/> to multiply.</param>
        /// <param name="e">The <see cref="CryptoGroupElement{TScalar, TElement}"/> to multiply.</param>
        /// <returns>
        /// The <see cref="CryptoGroupElement{TScalar, TElement}"/> that is the product of <c>e</c> * <c>k</c>.
        /// </returns>
        public static CryptoGroupElement<TScalar, TElement> operator *(TScalar k, CryptoGroupElement<TScalar, TElement> e)
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
