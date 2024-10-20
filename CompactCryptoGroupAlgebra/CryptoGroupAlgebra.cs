﻿// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

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
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Base implementation of <see cref="ICryptoGroupAlgebra{BigInteger, T}"/>
    /// with <see cref="BigInteger"/> scalars.
    /// 
    /// Provides common implementations independent of the actual group details
    /// based on basic operations. Basic operations are left abstract and must
    /// be implemented by deriving classes.
    /// 
    /// Group elements passed into algebraic operations are assumed to be valid values,
    /// this is not explicitly checked for and should be done by calling classes.
    /// </summary>
    /// <typeparam name="T">
    /// The data type used for raw group elements the algebraic operations operate on.
    /// </typeparam>
    public abstract class CryptoGroupAlgebra<T> : ICryptoGroupAlgebra<BigInteger, T> where T : notnull
    {
        /// <inheritdoc/>
        public BigPrime Order { get; }

        /// <inheritdoc/>
        public abstract int SecurityLevel { get; }

        /// <inheritdoc/>
        public T Generator { get; }

        /// <inheritdoc/>
        public BigInteger Cofactor { get; }

        /// <inheritdoc/>
        public int ElementBitLength { get; }

        /// <inheritdoc/>
        public int OrderBitLength { get { return NumberLength.GetLength(Order).InBits; } }

        /// <inheritdoc/>
        public T NeutralElement { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CryptoGroupAlgebra{T}"/>
        /// with a given generator <paramref name="order"/>.
        /// 
        /// Checks that the given <paramref name="order"/> is prime and throws
        /// a <exception cref="ArgumentException"/> if that is not the case.
        /// </summary>
        /// <param name="generator">Generator of the group.</param>
        /// <param name="order">Order of the group's generator.</param>
        /// <param name="cofactor">Cofactor of the group's generator.</param>
        /// <param name="neutralElement">The neutral element of the group.</param>
        /// <param name="elementBitLength">The maximum bit length of any group element.</param>

        protected CryptoGroupAlgebra(T generator, BigPrime order, BigInteger cofactor, T neutralElement, int elementBitLength)
        {
            Generator = generator;
            Order = order;
            Cofactor = cofactor;
            NeutralElement = neutralElement;
            ElementBitLength = elementBitLength;
        }

        /// <inheritdoc/>
        public T GenerateElement(BigInteger index)
        {
            return MultiplyScalar(Generator, index);
        }

        /// <inheritdoc/>
        public virtual T Negate(T e)
        {
            return MultiplyScalarUnchecked(e, Order - 1, OrderBitLength);
        }

        /// <summary>
        /// Internal implementation of scalar multiplication.
        /// 
        /// Uses a double-and-add approach relying on the <see cref="Add(T, T)"/>
        /// method.
        /// 
        /// To achieve some resistance against side channel attacks, the
        /// double-and-add implementation assumes that the scalar
        /// factor's bit length is limited by a given upper bound and always executes
        /// exactly that many iterations. <paramref name="factorBitLength"/>
        /// should therefore be held constant over subsequent invocations.
        ///
        /// Note that true side channel resistance cannot be guaranteed at this level.
        /// That would require all underlying implementations to be side channel resistant,
        /// which is not the case e.g. for <see cref="BigInteger"/>, which is often
        /// used throughout implementations.
        /// </summary>
        /// <remarks>
        /// This method is intended to be overriden with more specific implementations
        /// if necessary. It is labelled "Unchecked" since it performs no validity checks
        /// for arguments to keep it simple. These checks are done by the public facing
        /// methods <see cref="MultiplyScalar(T, BigInteger)"/>
        /// and <see cref="MultiplyScalar(T, BigInteger, int)"/>, which then call
        /// MultiplyScalarUnchecked. Overriding implementations can therefore also benefit
        /// from the argument checks performed in these and need not check their arguments.
        /// </remarks>
        /// <param name="e">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <param name="factorBitLength">Maximum bit length of a scalar.</param>
        /// <returns>The given element multiplied with the given scalar.</returns>
        protected virtual T MultiplyScalarUnchecked(T e, BigInteger k, int factorBitLength)
        {
            BigInteger maxFactor = BigInteger.One << factorBitLength;
            int i = factorBitLength - 1;

            T r0 = NeutralElement;
            for (BigInteger mask = maxFactor >> 1; !mask.IsZero; mask = mask >> 1, --i)
            {
                BigInteger bitI = (k & mask) >> i;
                r0 = Add(r0, r0);
                T r1 = Add(r0, e);

                r0 = Select(bitI.IsOne, r0, r1);
            }
            Debug.Assert(i == -1);
            return r0;
        }

        /// <summary>
        /// Selects one of two elements based on a selection bit.
        /// </summary>
        /// <param name="selectSecond">Whether to select the second choice or not.</param>
        /// <param name="first">The first selection choice.</param>
        /// <param name="second">The second selection choice.</param>
        /// <returns>
        /// <paramref name="second"/> if <paramref name="selectSecond"/> is
        /// <c>true</c>; otherwise <paramref name="first"/>
        /// </returns>
        private T Select(bool selectSecond, T first, T second)
        {
            if (selectSecond) return second;
            return first;
        }

        /// <summary>
        /// Multiplies a group element with a scalar factor.
        /// 
        /// Scalar multiplication is understood as adding the group element to itself
        /// as many times as dictated by the scalar factor.
        ///
        /// The optional parameter factorBitLength allows to specify the bit length
        /// of the scalar, which increases performance if it is significantly below
        /// that of the order. However, this value should be held constant over
        /// subsequent calls to this method to discourage timing and other side channel
        /// attacks.
        /// </summary>
        /// <param name="e">A group element.</param>
        /// <param name="k">A scalar.</param>
        /// <param name="factorBitLength">Maximum bit length of a scalar.</param>
        /// <returns>The given element multiplied with the given scalar.</returns>
        public T MultiplyScalar(T e, BigInteger k, int factorBitLength)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(k), "The given factor must be non-negative.");

            if (NumberLength.GetLength(k).InBits > factorBitLength)
                throw new ArgumentOutOfRangeException(nameof(k), "The given factor must not exceed the admittable factor bit length.");

            return MultiplyScalarUnchecked(e, k, factorBitLength);
        }

        /// <inheritdoc/>
        public T MultiplyScalar(T e, BigInteger k)
        {
            if (k < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(k), "The given factor must be non-negative.");
            k = k % Order; // k * e is at least periodic in Order
            return MultiplyScalarUnchecked(e, k, OrderBitLength);
        }

        /// <inheritdoc/>
        public bool IsPotentialElement(T element)
        {
            // implementation-specific checks
            return IsElementDerived(element);
        }

        /// <inheritdoc/>
        public bool IsSafeElement(T element)
        {
            if (!IsPotentialElement(element)) return false;

            // verifying that the point is not from a small subgroup of the whole curve (and thus outside
            // of the safe subgroup over which operations are considered)
            T check = MultiplyScalarUnchecked(element, Cofactor, NumberLength.GetLength(Cofactor).InBits);
            if (check.Equals(NeutralElement))
                return false;
            return true;
        }

        /// <inheritdoc/>
        public abstract T Add(T left, T right);

        /// <summary>
        /// Implementation specific checks for validity of group elements.
        /// 
        /// Must be provided by inheriting classes and is called by
        /// <see cref="IsPotentialElement"/> to determine whether the given element
        /// value is an element of the mathematical structure on which
        /// the group's algebra operates.
        /// </summary>
        /// <param name="element">The group element candidate to be checked for validity.</param>
        /// <returns><c>true</c>, if the candidate is valid, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// Implementations of <see cref="IsElementDerived(T)"/> must return
        /// <c>true</c> for the neutral element.
        /// </remarks>
        protected abstract bool IsElementDerived(T element);

        /// <inheritdoc/>
        public abstract T FromBytes(byte[] buffer);

        /// <inheritdoc/>
        public abstract byte[] ToBytes(T element);

        /// <summary>
        /// Determines whether the given <see cref="CryptoGroupAlgebra{T}"/> is equal to the current <see cref="CryptoGroupAlgebra{T}"/>
        /// in the sense that both operate on the same algebraic group.
        /// </summary>
        /// <param name="other">The <see cref="CryptoGroupAlgebra{T}"/> to compare with the current <see cref="CryptoGroupAlgebra{T}"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="CryptoGroupAlgebra{T}"/> is equal to the current
        /// <see cref="CryptoGroupAlgebra{T}"/>; otherwise, <c>false</c>.</returns>
        public abstract bool Equals(CryptoGroupAlgebra<T>? other);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as CryptoGroupAlgebra<T>);
        }

        /// <inheritdoc/>
        public abstract override int GetHashCode();


        /// <inheritdoc/>
        public (BigInteger, T) GenerateRandomElement(RandomNumberGenerator randomNumberGenerator)
        {
            BigInteger index = randomNumberGenerator.GetBigIntegerBetween(1, Order - 1);
            T element = GenerateElement(index);
            return (index, element);
        }
    }
}
