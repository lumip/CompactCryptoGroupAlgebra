﻿// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2022-2024 Lukas Prediger <lumip@lumip.de>
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

using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

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

        /// <summary>
        /// Returns a random positive <see cref="BigInteger"/> with the given bit length.
        /// </summary>
        /// <param name="randomNumberGenerator">Random number generator.</param>
        /// <param name="length">The bit length of the generated number.</param>
        /// <returns>The random <see cref="BigInteger"/>.</returns>
        public static BigInteger GetBigIntegerWithLength(
            this RandomNumberGenerator randomNumberGenerator, NumberLength length
        )
        {
            byte[] buffer = new byte[length.InBytes + 1];
            randomNumberGenerator.GetBytes(buffer);
            buffer[buffer.Length - 1] = 0; // ensure that there is an additional zero byte so that BigInteger does not treat it as negative

            var candidate = new BigInteger(buffer);
            candidate &= (BigInteger.One << length.InBits) - 1; // cut off additional bits generated in the highest-order byte
            candidate |= BigInteger.One << (length.InBits - 1); // ensure msb is set to achieve desired length
            return candidate;
        }

        /// <summary>
        /// Returns a random <see cref="BigPrime"/> with the given bit length.
        /// </summary>
        /// <param name="randomNumberGenerator">Random number generator.</param>
        /// <param name="length">The bit length of the generated prime number.</param>
        /// <returns>The random <see cref="BigPrime"/>.</returns>
        public static BigPrime GetBigPrime(
            this RandomNumberGenerator randomNumberGenerator, NumberLength length
        )
        {
            BigInteger primeCandidate = randomNumberGenerator.GetBigIntegerWithLength(length);

            // Ensure primeCandidate is odd.
            primeCandidate |= 1;

            // Ensure primeCandidate mod 6 == 1 or 5. Any prime p can only have p mod 6 == 1 or p mod 6 == 5.
            // Relying on bit operations to avoid any branching statements.
            var residue = primeCandidate % 6;
            primeCandidate += residue & 2;  // <-> primeCandidate += (residue == 3) ? 2 : 0;
            residue += residue & 2; // <-> residue += (residue == 3) ? 2 : 0;
            int step = 2 << (int)((residue ^ (residue >> 2)) & 1); // <-> step = (residue == 1) ? 4 : 2;
            int shiftDir = -1 + (step & 2); // <-> shiftDir = (step == 2) ? 1 : -1;

            while (!PrimalityTest.IsProbablyPrime(primeCandidate, randomNumberGenerator))
            {
                primeCandidate += step;

                // Below is equivalent to step = (step == 2) ? 4 : 2;
                step = (step << shiftDir) | (step >> -shiftDir);
                shiftDir = -shiftDir;
            }

            return BigPrime.CreateWithoutChecks(primeCandidate);
        }
    }
}
