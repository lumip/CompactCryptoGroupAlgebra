// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2022 Lukas Prediger <lumip@lumip.de>
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
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;

namespace CompactCryptoGroupAlgebra.Multiplicative
{
    /// <summary>
    /// Algebraic group operations based on multiplications in the finite field of a prime number <c>P</c>.
    /// 
    /// Through the <see cref="CryptoGroup{BigInteger, BigInteger}"/> interface, the addition
    /// represents multiplication of two integers modulo <c>P</c>, while scalar multiplication
    /// is exponentiation of an integer modulo <c>P</c>.
    /// </summary>
    public class MultiplicativeGroupAlgebra : CryptoGroupAlgebra<BigInteger>
    {
        /// <summary>
        /// Accessor to the prime modulo defining the underlying number field.
        /// </summary>
        /// <value>The prime modulo of the group.</value>
        public BigPrime Prime { get; }

        /// <summary>
        /// Computes the security level of the multiplicative group.
        ///
        /// The security level is determined as the minimum of the respective
        /// security levels granted by the <paramref name="prime"/> modulus of
        /// the group and the <paramref name="order"/>.
        ///
        /// The first is estimated from the cost of computing the discrete logarithm (cf. [1])
        /// using the number field sieve, the second results from Pollard's Rho algorithm
        /// for discrete logarithms [2].
        ///
        /// [1]: D. Gordon: Discrete Logarithms in GF(P) Using the Number Field Sieve, https://doi.org/10.1137/0406010
        /// [2]: J. Pollard: Monte Carlo Methods For Index Computation (mod p), https://doi.org/10.1090/S0025-5718-1978-0491431-9 
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group.</param>
        /// <returns>The security level (in bits).</returns>
        public static int ComputeSecurityLevel(BigPrime prime, BigPrime order)
        {
            // number field sieve strength
            int primeBits = NumberLength.GetLength(prime).InBits;
            double natLogPrime = Math.Log(2) * primeBits;
            double sieveConstant = 1.9; // note(lumip): references give ~1.91+o(1); using 1.9 for a tiny bit of slack
                                        // (cf.L. Grémy, Sieve Algorithms for the Discrete Logarithm in Medium Characteristic Finite Fields,
                                        //               https://tel.archives-ouvertes.fr/tel-01647623/document )

            double natLogSieveLevel = sieveConstant * Math.Pow(natLogPrime, 1.0/3.0) * Math.Pow(Math.Log(natLogPrime), 2.0/3.0);
            int sieveLevel = (int)(natLogSieveLevel/Math.Log(2));

            // pollard rho strength
            int rhoLevel = NumberLength.GetLength(order).InBits * 2;

            return Math.Min(sieveLevel, rhoLevel);
        }

        /// <summary>
        /// Computes the required bit length for prime modulus to achieve a desired security level.
        ///
        /// The required bit length for the modulus is found by solving the equation for expected
        /// runtime of the number field sieve algorithm [1] for the security parameter. 
        ///
        /// /// [1]: D. Gordon: Discrete Logarithms in GF(P) Using the Number Field Sieve, https://doi.org/10.1137/0406010
        /// </summary>
        /// <param name="securityLevel">The desired security level (measured in bits).</param>
        public static NumberLength ComputePrimeLengthForSecurityLevel(int securityLevel)
        {
            // find minimum bit length l for Number field sieve
            // 1. solve number field sieve for z = ln ln (2^l) via Newton method
            double c = Math.Log(2)*securityLevel/1.9;
            Func<double, double> f = (double z) =>
                c - Math.Exp((1.0/3.0) * z) * Math.Pow(z, 2.0/3.0);
            Func<double, double> df1 = (double z) => 
                -Math.Exp((1.0/3.0) * z) * ((1.0/3.0) * Math.Pow(z, 2.0/3.0) + 2.0/(3.0 * Math.Pow(z, 1.0/3.0)));

            double[] z = new double[] { Math.Log(20 * securityLevel), double.PositiveInfinity };
            int i = 0;
            while (Math.Abs(z[1-i] - z[i]) > 1e-7)
            {
                z[1 - i] =  z[i] - f(z[i]) / df1(z[i]);
                i = 1-i;
            }
            // 2. compute l from z
            int l = (int)Math.Ceiling(Math.Exp(z[i]) / Math.Log(2));

            l = Math.Max(l, 2 * securityLevel); // at least 2*securityLevel from Pollard Rho
            return NumberLength.FromBitLength(l);
        }

        /// <inheritdoc/>
        public override int SecurityLevel { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplicativeGroupAlgebra"/> class
        /// given the group's parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group.</param>
        /// <param name="generator">The generator of the group.</param>
        public MultiplicativeGroupAlgebra(BigPrime prime, BigPrime order, BigInteger generator)
            : base(
                generator,
                order,
                (prime - 1) / order,
                BigInteger.One,
                NumberLength.GetLength(prime).InBits
            )
        {
            Prime = prime;
            if (!IsSafeElement(generator))
                throw new ArgumentException("The generator must be an element of the group.", nameof(generator));
            SecurityLevel = ComputeSecurityLevel(prime, order);
        }

        /// <inheritdoc/>
        public override BigInteger Add(BigInteger left, BigInteger right)
        {
            return (left * right) % Prime;
        }

        /// <inheritdoc/>
        public override BigInteger Negate(BigInteger e)
        {
            (_, _, BigInteger r) = ExtendedEuclideanAlgorithm.GreatestCommonDivisor(Prime, e);
            r = (r + Prime) % Prime;
            return r;
        }

        /// <inheritdoc/>
        protected override bool IsElementDerived(BigInteger element)
        {
            return (element > BigInteger.Zero && element < Prime);
        }

        /// <inheritdoc/>
        public override BigInteger FromBytes(byte[] buffer)
        {
            return new BigInteger(buffer);
        }

        /// <inheritdoc/>
        public override byte[] ToBytes(BigInteger element)
        {
            return element.ToByteArray();
        }

        /// <inheritdoc/>
        public override bool Equals(CryptoGroupAlgebra<BigInteger>? other)
        {
            return other is MultiplicativeGroupAlgebra algebra &&
                   Prime.Equals(algebra.Prime) &&
                   Order.Equals(algebra.Order) &&
                   Generator.Equals(algebra.Generator);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 630028201;
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Prime);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Order);
            hashCode = hashCode * -1521134295 + EqualityComparer<BigInteger>.Default.GetHashCode(Generator);
            return hashCode;
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{BigInteger, BigInteger}" /> instance using a <see cref="MultiplicativeGroupAlgebra" />
        /// instance with the given parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public static CryptoGroup<BigInteger, BigInteger> CreateCryptoGroup(BigPrime prime, BigPrime order, BigInteger generator)
        {
            return new CryptoGroup<BigInteger, BigInteger>(new MultiplicativeGroupAlgebra(
                prime, order, generator
            ));
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{BigInteger, BigInteger}" /> instance that satisfies at least a given security level.
        /// 
        /// Finds random primes q, p such that p = 2q+1 and the bit length of p satisfies the security level requirements.
        /// Then finds an element of the q-order subgroup of the multiplicative group defined by p to use as the generator.
        /// 
        /// This process may take some time, depending on the security level chosen.
        /// </summary>
        /// <param name="securityLevel">The minimal security level for the group to be created.</param>
        /// <param name="randomNumberGenerator">A random number generator (used for sampling primes).</param>
        public static CryptoGroup<BigInteger, BigInteger> CreateCryptoGroup(int securityLevel, RandomNumberGenerator randomNumberGenerator)
        {
            var primeLength = ComputePrimeLengthForSecurityLevel(securityLevel);
            var sgPrimeLength = NumberLength.FromBitLength(primeLength.InBits - 1);
            BigInteger sgCandidate = randomNumberGenerator.GetBigIntegerWithLength(sgPrimeLength);
            sgCandidate |= BigInteger.One; // ensure sgCandidate is odd
            BigInteger primeCandidate = 2 * sgCandidate + 1;
            while ( !PrimalityTest.IsProbablyPrime(sgCandidate, randomNumberGenerator) ||
                    !PrimalityTest.IsProbablyPrime(primeCandidate, randomNumberGenerator) )
            {
                sgCandidate += 2;
                primeCandidate += 4;
            }
            Debug.Assert(NumberLength.GetLength(sgCandidate).InBits == sgPrimeLength.InBits);
            Debug.Assert(NumberLength.GetLength(primeCandidate).InBits == primeLength.InBits);

            var groupAlgebra = new MultiplicativeGroupAlgebra(
                BigPrime.CreateWithoutChecks(primeCandidate), BigPrime.CreateWithoutChecks(sgCandidate), new BigInteger(4)
            );

            return new CryptoGroup<BigInteger, BigInteger>(groupAlgebra);
        }
        
    }
}
