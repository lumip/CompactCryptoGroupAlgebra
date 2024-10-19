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

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class BigIntegerFieldTests
    {
        [Test]
        public void TestConstructor()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            Assert.AreEqual(prime, field.Modulo);
            Assert.AreEqual(1, field.ElementByteLength);
        }

        [Test]
        public void TestPow()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            var result = field.Pow(5, 3);
            var expected = new BigInteger(4);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestPowRejectsNegativeExponent()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            Assert.Throws<ArgumentException>(
                () => field.Pow(5, -1)
            );
        }

        [Test]
        public void TestSquare()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            var result = field.Square(5);
            var expected = new BigInteger(3);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestInvertMult()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            var result = field.InvertMult(5);
            var expected = new BigInteger(9);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(BigInteger.One, field.Mod(result * 5));
        }

        [Test]
        [TestCase(24, 2)]
        [TestCase(-2, 9)]
        public void TestMod(int value, int expectedRaw)
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            var result = field.Mod(value);
            var expected = new BigInteger(expectedRaw);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestIsElementTrue()
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            Assert.IsTrue(field.IsElement(7));
        }

        [Test]
        [TestCase(-2)]
        [TestCase(12)]
        public void TestIsElementFalse(int value)
        {
            var prime = BigPrime.CreateWithoutChecks(11);
            var field = new BigIntegerField(prime);

            Assert.IsFalse(field.IsElement(new BigInteger(value)));
        }
    }
}
