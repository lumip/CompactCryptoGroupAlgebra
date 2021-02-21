// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography
// Copyright (C) 2020  Lukas <lumip> Prediger
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Extends <see cref="RandomNumberGenerator"/> with a convenient way to
    /// sample a random <see cref="BigInteger"/>.
    /// </summary>
    public static class RandomNumberGeneratorExtensions
    {
        /// <summary>
        /// Returns a random <see cref="BigInteger"/> between (and including)
        /// <paramref name="lower"/> and <paramref name="upper"/>.
        /// </summary>
        /// <returns>The <see cref="T:System.Numerics.BigInteger"/>.</returns>
        /// <param name="randomNumberGenerator">Random number generator.</param>
        /// <param name="lower">Inclusive lower bound.</param>
        /// <param name="upper">Inclusive upper bound.</param>
        public static BigInteger GetBigIntegerBetween(
            this RandomNumberGenerator randomNumberGenerator, BigInteger lower, BigInteger upper
        )
        {
            NumberLength length = NumberLength.GetLength(upper - lower);
            BigInteger delta;
            do
            {
                byte[] buffer = new byte[length.InBytes];
                randomNumberGenerator.GetBytes(buffer);
                delta = new BigInteger(buffer);
            }
            while (delta >= upper - lower);
            delta *= delta.Sign;
            Debug.Assert(delta >= BigInteger.Zero);
            return lower + delta;
        }
    }
}
