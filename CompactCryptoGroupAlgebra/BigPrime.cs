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
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// A big prime number.
    /// </summary>
    public readonly struct BigPrime
    {
        private readonly BigInteger _value;

        /// <summary>
        /// Initializes a new instance of <see cref="BigPrime"/>.
        /// </summary>
        /// <param name="primeValue">Prime <see cref="BigInteger"/>.</param>
        private BigPrime(BigInteger primeValue)
        {
            _value = primeValue;
        }

        /// <summary>
        /// Creates a new <see cref="BigPrime"/> instance, foregoing primality
        /// checks.
        /// 
        /// CAUTION: This foregoes some computation at the risk of being UNSAFE
        /// by potentially introducing non-prime values into routines that rely on primes
        /// for security. Prefer using
        /// <see cref="BigPrime.Create(BigInteger, RandomNumberGenerator, double)" />.
        /// </summary>
        /// <returns>New <see cref="BigPrime"/> instance of <paramref name="primeValue"/>.</returns>
        /// <param name="primeValue">Prime value.</param>
        public static BigPrime CreateWithoutChecks(BigInteger primeValue)
        {
            return new BigPrime(primeValue);
        }

        /// <summary>
        /// Creates a new <see cref="BigPrime"/> instance.
        /// 
        /// Performs a primality test to ensure that the given <paramref name="primeValue"/>
        /// is indeed prime. The test is probabilistic and may accept a non-prime
        /// value with a percentage of <paramref name="errorTolerance"/>.
        /// Throws a <see cref="ArgumentException"/> if <paramref name="primeValue"/>
        /// is found to be not prime.
        /// </summary>
        /// <returns>New <see cref="BigPrime"/> instance of</returns>
        /// <param name="primeValue">Prime value.</param>
        /// <param name="randomNumberGenerator">Random number generator.</param>
        /// <param name="errorTolerance">Error tolerance for accepting non-prime values.</param>
        public static BigPrime Create(BigInteger primeValue, RandomNumberGenerator randomNumberGenerator, double errorTolerance = 1E-40)
        {
            if (!primeValue.IsProbablyPrime(randomNumberGenerator, errorTolerance))
                throw new ArgumentException("Provided value is not a prime number!", nameof(primeValue));

            return new BigPrime(primeValue);
        }

        /// <summary>
        /// Implicitly converts to a <see cref="BigInteger"/>.
        /// </summary>
        public static implicit operator BigInteger(BigPrime p) => p._value;

        /// <summary>
        /// Adds a <see cref="BigPrime"/> to a <see cref="BigInteger"/>,
        /// yielding a new <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="p">The first <see cref="BigPrime"/> to add.</param>
        /// <param name="x">The second <see cref="BigInteger"/> to add.</param>
        /// <returns>The <see cref="BigInteger"/> that is the sum of the values of <c>p</c> and <c>x</c>.</returns>
        public static BigInteger operator+(BigPrime p, BigInteger x)
        {
            return p._value + x;
        }

        /// <summary>
        /// Adds a <see cref="BigInteger"/> to a <see cref="BigPrime"/>,
        /// yielding a new <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="x">The first <see cref="BigInteger"/> to add.</param>
        /// <param name="p">The second <see cref="BigPrime"/> to add.</param>
        /// <returns>The <see cref="BigInteger"/> that is the sum of the values of <c>x</c> and <c>p</c>.</returns>
        public static BigInteger operator+(BigInteger x, BigPrime p)
        {
            return p + x;
        }

        /// <summary>
        /// Subtracts a <see cref="CompactCryptoGroupAlgebra.BigPrime"/> from a
        /// <see cref="System.Numerics.BigInteger"/>, yielding a new <see cref="System.Numerics.BigInteger"/>.
        /// </summary>
        /// <param name="p">The <see cref="CompactCryptoGroupAlgebra.BigPrime"/> to subtract from (the minuend).</param>
        /// <param name="x">The <see cref="System.Numerics.BigInteger"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="System.Numerics.BigInteger"/> that is <c>p</c> minus <c>x</c>.</returns>
        public static BigInteger operator-(BigPrime p, BigInteger x)
        {
            return p._value - x;
        }

        /// <summary>
        /// Subtracts a <see cref="System.Numerics.BigInteger"/> from a
        /// <see cref="CompactCryptoGroupAlgebra.BigPrime"/>, yielding a new <see cref="System.Numerics.BigInteger"/>.
        /// </summary>
        /// <param name="x">The <see cref="System.Numerics.BigInteger"/> to subtract from (the minuend).</param>
        /// <param name="p">The <see cref="CompactCryptoGroupAlgebra.BigPrime"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="System.Numerics.BigInteger"/> that is <c>x</c> minus <c>p</c>.</returns>
        public static BigInteger operator-(BigInteger x, BigPrime p)
        {
            return x - p._value;
        }

        /// <summary>
        /// Negates a <see cref="BigPrime"/>.
        /// </summary>
        /// <param name="p">The <see cref="BigPrime"/> to negate.</param>
        /// <returns>The <see cref="BigInteger"/> that is the negation of <c>p</c></returns>.
        public static BigInteger operator-(BigPrime p)
        {
            return -p._value;
        }

        /// <summary>
        /// Computes the product of a <see cref="BigInteger"/> and a <see cref="BigPrime"/>,
        /// yielding a new <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="x">The <see cref="System.Numerics.BigInteger"/> to multiply.</param>
        /// <param name="p">The <see cref="CompactCryptoGroupAlgebra.BigPrime"/> to multiply.</param>
        /// <returns>The <see cref="System.Numerics.BigInteger"/> that is <c>x</c> * <c>p</c>.</returns>
        public static BigInteger operator*(BigInteger x, BigPrime p)
        {
            return x * p._value;
        }

        /// <summary>
        /// Computes the product of a <see cref="BigPrime"/> and a <see cref="BigInteger"/>,
        /// yielding a new <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="p">The <see cref="CompactCryptoGroupAlgebra.BigPrime"/> to multiply.</param>
        /// <param name="x">The <see cref="System.Numerics.BigInteger"/> to multiply.</param>
        /// <returns>The <see cref="System.Numerics.BigInteger"/> that is <c>p</c> * <c>x</c>.</returns>
        public static BigInteger operator*(BigPrime p, BigInteger x)
        {
            return x * p._value;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is BigPrime prime && _value.Equals(prime._value);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -1937169414 + _value.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
