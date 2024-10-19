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

using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class DiffieHellmanIntegrationTests
    {
        [Test]
        public void TestDiffieHellmanWithMultiplicativeGroup()
        {
            string primeHex = @"0
                FFFFFFFF FFFFFFFF C90FDAA2 2168C234 C4C6628B 80DC1CD1
                29024E08 8A67CC74 020BBEA6 3B139B22 514A0879 8E3404DD
                EF9519B3 CD3A431B 302B0A6D F25F1437 4FE1356D 6D51C245
                E485B576 625E7EC6 F44C42E9 A63A3620 FFFFFFFF FFFFFFFF";

            BigPrime prime = BigPrime.CreateWithoutChecks(
                BigInteger.Parse(Regex.Replace(primeHex, @"\s+", ""), NumberStyles.AllowHexSpecifier)
            );
            BigPrime order = BigPrime.CreateWithoutChecks((prime - 1) / 2);
            BigInteger generator = 4;

            var group = Multiplicative.MultiplicativeGroupAlgebra.CreateCryptoGroup(
                prime, order, generator
            );

            DoDiffieHellman(group);
        }

        [Test]
        public void TestDiffieHellmanWithNISTP256Curve()
        {
            var group = EllipticCurves.CurveGroupAlgebra.CreateCryptoGroup(
                EllipticCurves.CurveParameters.Curve25519
            );

            DoDiffieHellman(group);
        }

        [Test]
        public void TestDiffieHellmanWithCurve25519Curve()
        {
            var group = EllipticCurves.CurveGroupAlgebra.CreateCryptoGroup(
                EllipticCurves.CurveParameters.Curve25519
            );

            DoDiffieHellman(group);
        }

        [Test]
        public void TestDiffieHellmanWithXOnlyCurve25519Curve()
        {
            var group = EllipticCurves.XOnlyMontgomeryCurveAlgebra.CreateCryptoGroup(
                EllipticCurves.CurveParameters.Curve25519
            );

            DoDiffieHellman(group);
        }

        public static void DoDiffieHellman<TScalar, TElement>(
            CryptoGroup<TScalar, TElement> group
        ) where TScalar : notnull where TElement : notnull
        {
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

            // Generating DH secret and public key for Alice
            (var dhSecretAlice, var dhPublicAlice) =
                group.GenerateRandom(randomNumberGenerator);

            // Generating DH secret and public key for Bob
            (var dhSecretBob, var dhPublicBob) =
                group.GenerateRandom(randomNumberGenerator);

            // Computing shared secret for Alice and Bob
            var sharedSecretBob = dhPublicAlice * dhSecretBob;
            var sharedSecretAlice = dhPublicBob * dhSecretAlice;

            Assert.AreEqual(sharedSecretAlice, sharedSecretBob);
        }
    }
}
