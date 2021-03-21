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
using NUnit.Framework;

using CompactCryptoGroupAlgebra.TestUtils;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class BigPrimeTests
    {
        [Test]
        public void TestCreateWithoutChecks()
        {
            var rawValue = new BigInteger(1236);
            var value = BigPrime.CreateWithoutChecks(rawValue);
            Assert.AreEqual(rawValue, (BigInteger)value);
        }

        [Test]
        public void TestCreate()
        {
            var rawValue = new BigInteger(8052311);
            var value = BigPrime.Create(rawValue, new SeededRandomNumberGenerator());
            Assert.AreEqual(rawValue, (BigInteger)value);
        }

        [Test]
        public void TestCreateThrowsForNonPrimeValue()
        {
            var rawValue = new BigInteger(124);
            Assert.Throws<ArgumentException>(
                () => BigPrime.Create(rawValue, new SeededRandomNumberGenerator())
            );
        }

        [Test]
        public void TestAdditionLeft()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var other = new BigInteger(5);

            var expected = new BigInteger(16);
            var result = prime + other;
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAdditionRight()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var other = new BigInteger(5);

            var expected = new BigInteger(16);
            var result = other + prime;
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestSubtractionLeft()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var other = new BigInteger(5);

            var expected = new BigInteger(6);
            var result = prime - other;
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestSubtractionRight()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var other = new BigInteger(5);

            var expected = new BigInteger(-6);
            var result = other - prime;
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegation()
        {
            var prime = BigPrime.CreateWithoutChecks(11);

            var expected = new BigInteger(-11);
            var result = -prime;
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void MultiplicationLeft()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var other = new BigInteger(5);

            var expected = new BigInteger(55);
            var result = prime * other;
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void MultiplicationRight()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var other = new BigInteger(5);

            var expected = new BigInteger(55);
            var result = other * prime;
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestEqualsIsTrueForEqual()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var otherPrime = BigPrime.CreateWithoutChecks(11);

            Assert.AreEqual(prime, otherPrime);
        }

        [Test]
        public void TestEqualsIsFalseForDifferent()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var otherPrime = BigPrime.CreateWithoutChecks(13);

            Assert.AreNotEqual(prime, otherPrime);
        }

        [Test]
        public void TestEqualsIsFalseForNull()
        {
            var prime = BigPrime.CreateWithoutChecks(11);

            Assert.AreNotEqual(prime, null);
        }

        [Test]
        public void TestEqualsIsFalseForUnrelatedObject()
        {
            var prime = BigPrime.CreateWithoutChecks(11);

            Assert.AreNotEqual(prime, new object());
        }

        [Test]
        public void TestGetHashCodeIsEqualForEqual()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var otherPrime = BigPrime.CreateWithoutChecks(11);

            Assert.AreEqual(prime.GetHashCode(), otherPrime.GetHashCode());
        }

        [Test]
        public void TestToString()
        {
            var prime = BigPrime.CreateWithoutChecks(13);
            var expected = new BigInteger(13).ToString();
            var result = prime.ToString();

            Assert.AreEqual(expected, result);
        }
    }
}
