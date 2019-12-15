using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactEC.Tests
{
    [TestFixture]
    public class ECGroupAlgebraTests
    {
        // reference results from https://trustica.cz/en/2018/04/26/elliptic-curves-prime-order-curves/
        // generator has order 16, i.e., OrderSize = 5 bits

        private readonly ECParameters ecParams;

        public ECGroupAlgebraTests()
        {
            ecParams = new ECParameters()
            {
                P = 23,
                A = -2,
                B = 2,
                Generator = new CompactEC.ECPoint(1, 1),
                Order = 16
            };
        }

        [Test]
        public void TestAddDoublePoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = p.Clone();

            var expectedQ = new CompactEC.ECPoint(15, 14);
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDoublePointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = CompactEC.ECPoint.PointAtInfinity;
            var other = p.Clone();

            var expectedQ = CompactEC.ECPoint.PointAtInfinity;
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDifferentPoints()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = new CompactEC.ECPoint(15, 14);

            var expected = new CompactEC.ECPoint(16, 15);
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddPointAtInfinityLeft()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = CompactEC.ECPoint.PointAtInfinity;

            var result = curve.Add(other, p);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddPointAtInfinityRight()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = CompactEC.ECPoint.PointAtInfinity;

            var result = curve.Add(p, other);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddNegated()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = curve.Negate(p);

            var expected = CompactEC.ECPoint.PointAtInfinity;
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void TestAreNegationsFalseForEqualPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = p.Clone();

            Assert.IsFalse(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAreNegationsTrueForNegation()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = curve.Negate(p);

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAreNegationsTrueForZeroYPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(11, 0);
            var other = p.Clone();

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAddAffine()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(11, 0);

            var q = curve.Add(p, p);

            Assert.AreEqual(CompactEC.ECPoint.PointAtInfinity, q);
        }
        
        [Test]
        [TestCase(5, 5)]
        [TestCase(11, 0)]
        [TestCase(16, 15)]
        public void TestPointValidTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = new ECGroupAlgebra(ecParams);
            var point = new CompactEC.ECPoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsValid(point));
        }

        [Test]
        public void TestIsValidTrueForPointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.IsTrue(curve.IsValid(CompactEC.ECPoint.PointAtInfinity));
        }

        [Test]
        [TestCase(16, 1)]
        [TestCase(5, 2)]
        [TestCase(-2, 1)]
        [TestCase(16, -15)]
        [TestCase(78, 4)]
        [TestCase(4, 78)]
        public void TestIsvalidFalseForInvalidPoint(int xRaw, int yRaw)
        {
            var curve = new ECGroupAlgebra(ecParams);
            var point = new CompactEC.ECPoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsValid(point));
        }

        [Test]
        public void TestNegate()
        {
            var curve = new ECGroupAlgebra(ecParams);

            var p = new CompactEC.ECPoint(5, 5);
            var expected = new CompactEC.ECPoint(5, 23 - 5);

            var result = curve.Negate(p);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForZeroYPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(11, 0);

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestNegatePointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = CompactEC.ECPoint.PointAtInfinity;

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        [TestCase(2, 15, 14)]
        [TestCase(5, 16, 8)]
        [TestCase(7, 5, 18)]
        [TestCase(9, 5, 5)]
        public void TestMultiplyScalar(int kRaw, int expectedX, int expectedY)
        {
            var k = new BigInteger(kRaw);
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);

            var q = curve.MultiplyScalar(p, k);
            var expectedQ = new CompactEC.ECPoint(expectedX, expectedY);
            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var result = curve.MultiplyScalar(p, 8);

            Assert.AreEqual(curve.NeutralElement, result);
        }

        [Test]
        public void TestGroupElementBitLength()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(2*5, curve.ElementBitLength);
        }

        [Test]
        public void TestOrderIsAsSet()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(ecParams.Order, curve.Order);
        }

        [Test]
        public void TestOrderBitLength()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(5, curve.OrderBitLength);
        }
        
        [Test]
        public void TestNeutralElement()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(CompactEC.ECPoint.PointAtInfinity, curve.NeutralElement);
        }

        [Test]
        public void TestGeneratorIsAsSet()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(ecParams.Generator, curve.Generator);
        }

        [Test]
        public void TestFromBytes()
        {
            ECParameters largeParams = new ECParameters()
            {
                P = 134217728, // == 2 ^ 27
                Generator = CompactEC.ECPoint.PointAtInfinity,
                Order = 1
            };

            var curve = new ECGroupAlgebra(largeParams);
            var expected = new CompactEC.ECPoint(5, 5);
            var buffer = new byte[] { 5, 0, 0, 0, 5, 0, 0, 0 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesRejectsNullArgument()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.Throws<ArgumentNullException>(
                () => curve.FromBytes(null)
            );
        }

        [Test]
        public void TestFromBytesRejectsTooShortBuffer()
        {
            ECParameters largeParams = new ECParameters()
            {
                P = 134217728, // == 2 ^ 27
                Generator = CompactEC.ECPoint.PointAtInfinity,
                Order = 1
            };
            var curve = new ECGroupAlgebra(largeParams);
            var buffer = new byte[7];
            Assert.Throws<ArgumentException>(
                () => curve.FromBytes(buffer)
            );
        }

        [Test]
        public void TestToBytes()
        {
            ECParameters largeParams = new ECParameters()
            {
                P = 134217728, // == 2 ^ 27
                Generator = CompactEC.ECPoint.PointAtInfinity,
                Order = 1
            };

            var curve = new ECGroupAlgebra(largeParams);
            var p = new CompactEC.ECPoint(5, 5);
            var expected = new byte[] { 5, 0, 0, 0, 5, 0, 0, 0 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var expected = new CompactEC.ECPoint(5, 5);
            var buffer = new byte[] { 5, 5 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var expected = new byte[] { 5, 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var generator = new CompactEC.ECPoint(16, 1);
            var invalidParams = ecParams;
            invalidParams.Generator = generator;
            Assert.Throws<ArgumentException>(
                () => new ECGroupAlgebra(invalidParams)
            );
        }
    }
}
