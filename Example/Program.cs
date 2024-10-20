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
using CompactCryptoGroupAlgebra;
using CompactCryptoGroupAlgebra.Multiplicative;

namespace Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Instantiating a strong random number generator
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

            // Choosing parameters for multiplicative group
            // order 11 subgroup with generator 4 of characteristic 23 multiplicative group 
            BigPrime prime = BigPrime.Create(23, randomNumberGenerator);
            BigPrime order = BigPrime.Create(11, randomNumberGenerator);
            BigInteger generator = 4;

            // Creating the group instance
            var group = MultiplicativeGroupAlgebra.CreateCryptoGroup(prime, order, generator);
            DoDiffieHellman(group, randomNumberGenerator);
        }

        private static void DoDiffieHellman<TScalar, TElement>(
            CryptoGroup<TScalar, TElement> group, RandomNumberGenerator randomNumberGenerator
        ) where TScalar : notnull where TElement : notnull
        {
            // Generating DH secret and public key for Alice
            (TScalar dhSecretAlice, CryptoGroupElement<TScalar, TElement> dhPublicAlice) =
                group.GenerateRandom(randomNumberGenerator);

            // Generating DH secret and public key for Bob
            (TScalar dhSecretBob, CryptoGroupElement<TScalar, TElement> dhPublicBob) =
                group.GenerateRandom(randomNumberGenerator);

            // Computing shared secret for Alice and Bob
            CryptoGroupElement<TScalar, TElement> sharedSecretBob = dhPublicAlice * dhSecretBob;
            CryptoGroupElement<TScalar, TElement> sharedSecretAlice = dhPublicBob * dhSecretAlice;

            // Confirm that it's the same
            Debug.Assert(sharedSecretAlice.Equals(sharedSecretBob));

            Console.WriteLine($"Alice - Secret: {dhSecretAlice}, Public: {dhPublicAlice}");
            Console.WriteLine($"Bob   - Secret: {dhSecretBob}, Public: {dhPublicBob}");

            Console.WriteLine($"Alice - Result: {sharedSecretAlice}");
            Console.WriteLine($"Bob   - Result: {sharedSecretBob}");
        }
    }
}
