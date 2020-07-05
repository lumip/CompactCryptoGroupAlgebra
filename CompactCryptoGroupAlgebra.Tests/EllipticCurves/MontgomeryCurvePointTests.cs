using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class MontgomeryCurvePointTests
    {
        [Test]
        public void TestPointAtInfinity()
        {
            var point = MontgomeryCurvePoint.PointAtInfinity;

            Assert.True(point.IsAtInfinity);
        }

        [Test]
        public void TestConstructor()
        {
            var x = new BigInteger(2);
            var z = new BigInteger(7);
            var point = new MontgomeryCurvePoint(x, z);

            Assert.AreEqual(x, point.X);
            Assert.AreEqual(z, point.Z);
            Assert.IsFalse(point.IsNormalized);
            Assert.IsFalse(point.IsAtInfinity);
        }

        [Test]
        public void TestConstructorXOnly()
        {
            var x = new BigInteger(5);
            var point = new MontgomeryCurvePoint(x);

            Assert.AreEqual(x, point.X);
            Assert.AreEqual(BigInteger.One, point.Z);
            Assert.IsTrue(point.IsNormalized);
            Assert.IsFalse(point.IsAtInfinity);
        }

        [Test]
        public void TestEqualsTrueForEqual()
        {
            var x = new BigInteger(3);
            var z = new BigInteger(67);
            var point = new MontgomeryCurvePoint(x, z);
            var otherPoint = new MontgomeryCurvePoint(x, z);

            Assert.IsTrue(point.Equals(otherPoint));
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var x = new BigInteger(3);
            var z = new BigInteger(67);
            var point = new MontgomeryCurvePoint(x, z);
            var otherPoint = new MontgomeryCurvePoint(x, z);

            Assert.AreEqual(point.GetHashCode(), otherPoint.GetHashCode());
        }

        [Test]
        public void TestEqualsFalseForDifferent()
        {
            var x = new BigInteger(3);
            var z = new BigInteger(67);
            var point = new MontgomeryCurvePoint(x, z);

            var otherPoint = new MontgomeryCurvePoint(1, z);
            Assert.IsFalse(point.Equals(otherPoint));

            otherPoint = new MontgomeryCurvePoint(8, z);
            Assert.IsFalse(point.Equals(otherPoint));
        }

        [Test]
        public void TestEqualsFalseForX0AndPointAtInfinity()
        {
            var point = new MontgomeryCurvePoint(0);
            var otherPoint = MontgomeryCurvePoint.PointAtInfinity;

            Assert.IsFalse(point.Equals(otherPoint));
        }

    }
}