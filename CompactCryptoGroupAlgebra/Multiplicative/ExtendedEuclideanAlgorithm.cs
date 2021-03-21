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

using System.Numerics;

namespace CompactCryptoGroupAlgebra.Multiplicative
{
    /// <summary>
    /// Contains an implementation of the extended Euclidean algorithm for <see cref="BigInteger"/>.
    /// </summary>
    public static class ExtendedEuclideanAlgorithm
    {

        /// <summary>
        /// Returns the greatest common divisor and factors <c>x</c> and
        /// <c>y</c> such that <c>gcd(a,b) = a*x + b*y</c>.
        /// </summary>
        /// <returns>
        /// Tuple <c>(gcd(a,b), x, y)</c>.
        /// </returns>
        /// <param name="a">A <see cref="BigInteger" /> instance.</param>
        /// <param name="b">A <see cref="BigInteger" /> instance.</param>
        /// <remarks>
        /// Factors <c>x</c> or <c>y</c> may be negative.
        /// </remarks>
        public static (BigInteger, BigInteger, BigInteger) GreatestCommonDivisor(BigInteger a, BigInteger b)
        {
            // following: https://cp-algorithms.com/algebra/extended-euclid-algorithm.html
            if (b > a)
            {
                (var gcd, var y, var x) = GreatestCommonDivisor(b, a);
                return (gcd, x, y);
            }

            (BigInteger x0, BigInteger x1, BigInteger y0, BigInteger y1) = (1, 0, 0, 1);
            while (!b.IsZero)
            {
                BigInteger r;
                var q = BigInteger.DivRem(a, b, out r);
                a = b;
                b = r;
                (x0, x1) = (x1, x0 - q * x1);
                (y0, y1) = (y1, y0 - q * y1);
            }
            return (a, x0, y0);

        }

    }
}