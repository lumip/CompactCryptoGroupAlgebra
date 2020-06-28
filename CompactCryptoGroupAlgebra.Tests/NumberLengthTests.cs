using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.Tests
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
