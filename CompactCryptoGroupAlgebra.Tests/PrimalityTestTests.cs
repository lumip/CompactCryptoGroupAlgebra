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

using Moq;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class PrimalityTestTests
    {
        [Test]
        [TestCase(7, 7, 2)] // fails a^q    % p !=  1
        [TestCase(5, 7, 2)] // fails a^q    % p != -1
        [TestCase(3, 7, 2)] // fails a^(2q) % p != -1
        public void TestIsCompositeWitnessForPrime(int a, int q, int k)
        {
            var result = PrimalityTest.IsCompositeWitness(a, q, k);
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(8, 5, 2)]
        public void TestIsCompositeWitnessForComposite(int a, int q, int k)
        {
            var result = PrimalityTest.IsCompositeWitness(a, q, k);
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(11)]
        [TestCase(3041)]
        [TestCase(8052311)]
        [TestCase(13132877)]
        public void TestMillerRabinWithPrimes(int rawN)
        {
            BigInteger n = new BigInteger(rawN);
            Random random = new Random(0);

            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(random.NextBytes);

            Assert.IsTrue(n.IsProbablyPrime(rngMock.Object));
        }

        [Test]
        [TestCase(32)]
        [TestCase(2 * 29)]
        [TestCase(8052311 * 17)]
        [TestCase(1709 * 2713)]
        public void TestMillerRabinWithComposites(int rawN)
        {
            BigInteger n = new BigInteger(rawN);
            Random random = new Random(0);

            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(random.NextBytes);

            Assert.IsFalse(n.IsProbablyPrime(rngMock.Object));
        }
    }
}
