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
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Contains extension methods implementing a <see cref="BigInteger"/> primality test.
    /// </summary>
    public static class PrimalityTest
    {
        /// <summary>
        /// Checks whether <paramref name="a"/> is a strong witness of compositeness
        /// of <c>n</c> = <paramref name="q"/> * 2^<paramref name="k"/> + 1.
        /// </summary>
        /// <returns>
        ///   <c>true</c>, if <paramref name="a"/> is a witness of
        ///   compositeness of <c>n</c>, <c>false</c> otherwise.
        /// </returns>
        /// <param name="a">The number that could be a witness.</param>
        /// <param name="q">Odd factor of <c>n</c>.</param>
        /// <param name="k">Power of two exponent of <c>n</c>.</param>
        public static bool IsCompositeWitness(BigInteger a, BigInteger q, int k)
        {
            BigInteger n = q * (1 << k) + 1;
            if (BigInteger.ModPow(a, q, n).IsOne) return false;
            BigInteger e = q;
            for (int i = 0; i < k; i++)
            {
                if (BigInteger.ModPow(a, e, n) == n - 1) return false;
                e <<= 1;
            }
            return true;
        }

        /// <summary>
        /// List of all primes from 3 to 113 for quick compositeness check
        /// in <see cref="IsProbablyPrime(BigInteger, RandomNumberGenerator, double)"/>
        /// </summary>
        private static readonly int[] SmallPrimes = {
                   3,   5,   7,  11,  13,  17,  19,  23,  29,
             31,  37,  41,  43,  47,  53,  59,  61,  67,  71,
             73,  79,  83,  89,  97, 101, 103, 107, 109, 113
        }; // excluding 2, we check IsEven for that

        /// <summary>
        /// Checks whether a given <paramref name="n"/> is a prime number.
        ///
        /// Implements the Miller-Rabin primality test, which is a probabilistic
        /// algorithm. If the given <paramref name="n"/> is a prime,
        /// <see cref="IsProbablyPrime(BigInteger, RandomNumberGenerator, double)"/>
        /// will always return <c>true</c>, but if <paramref name="n"/> is
        /// a composite number, the algorithm may return <c>true</c> with probability
        /// less than <paramref name="errorProbability"/>.
        /// </summary>
        /// <param name="n">The number to check for primality.</param>
        /// <param name="randomNumberGenerator">Random number generator instance.</param>
        /// <param name="errorProbability">Acceptable probability of <c>true</c> being returned when <paramref name="n"/> is actually not a prime.</param>
        /// <returns><c>true</c>, if <paramref name="n"/> is prime with probability 1 - <paramref name="errorProbability"/>, <c>false</c> otherwise.</returns>
        public static bool IsProbablyPrime(
            this BigInteger n, RandomNumberGenerator randomNumberGenerator, double errorProbability = 1e-10
        )
        {
            if (n.IsEven || n.IsPowerOfTwo) return false;

            // shortcut: see if small primes are factors of n. if so, n not prime
            foreach (int p in SmallPrimes)
            {
                if (n == p) return true;
                if (n % p == 0) return false;
            }

            // if shortcut did not discard n, do Miller-Rabin:

            // determine amount of basic Miller-Rabin tests:
            // P(Miller-Rabin states "n prime" | n composite) <= 1/4
            // and we want:
            //      errorProbability <= (1/4)^testCount
            // <->  ln(errorProbability) <= -testCount*ln(4)
            // <-> -ln(errorProbability)/ln(4) >= testCount
            int testCount = 1 + (int)(-Math.Log(errorProbability) / Math.Log(4));

            // isolate uneven factor of n-1
            BigInteger q = n - 1;
            Debug.Assert(q.IsEven);
            int k = 0;
            do
            {
                q >>= 1;
                k++;
            }
            while (q.IsEven);

            // actual Miller-Rabin test
            for (int i = 0; i < testCount; ++i)
            {
                BigInteger a = randomNumberGenerator.GetBigIntegerBetween(2, n - 1);
                if (IsCompositeWitness(a, q, k)) return false;
            }
            return true;
        }
    }
}
