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

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class NumberLengthTests
    {
        [Test]
        [TestCase(0)]
        [TestCase(6)]
        public void TestFromBitLength(int bl)
        {
            var l = NumberLength.FromBitLength(bl);
            Assert.AreEqual(bl, l.InBits);
        }

        [Test]
        [TestCase(0)]
        [TestCase(6)]
        public void TestFromByteLength(int bl)
        {
            var l = NumberLength.FromByteLength(bl);
            Assert.AreEqual(bl * 8, l.InBits);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(5, 1)]
        [TestCase(6, 1)]
        [TestCase(7, 1)]
        [TestCase(8, 1)]
        [TestCase(9, 2)]
        [TestCase(13, 2)]
        [TestCase(16, 2)]
        [TestCase(17, 3)]
        public void TestInBytes(int bl, int expected)
        {
            var l = NumberLength.FromBitLength(bl);
            var result = l.InBytes;
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(15, 4)]
        [TestCase(16, 5)]
        public void TestGetLengthBigInteger(int valueInt, int expectedBitLength)
        {
            var value = new BigInteger(valueInt);
            var result = NumberLength.GetLength(value).InBits;

            Assert.AreEqual(expectedBitLength, result);
        }

        [Test]
        public void TestEqualsTrueForEqual()
        {
            var e1 = NumberLength.FromBitLength(10);
            var e2 = NumberLength.FromBitLength(10);

            Assert.AreEqual(e1, e2);
        }

        [Test]
        public void TestEqualsFalseForDifferent()
        {
            var e1 = NumberLength.FromBitLength(10);
            var e2 = NumberLength.FromBitLength(12);

            Assert.AreNotEqual(e1, e2);
        }
    }
}
