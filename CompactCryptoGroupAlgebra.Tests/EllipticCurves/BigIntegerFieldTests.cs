using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
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
            Assert.AreEqual(BigInteger.One, field.Mod(result*5));
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
    }
}