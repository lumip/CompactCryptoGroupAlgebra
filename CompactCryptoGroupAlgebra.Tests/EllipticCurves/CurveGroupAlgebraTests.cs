using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class CurveGroupAlgebraTests
    {

        private readonly CurveParameters ecParams;
        private readonly CurveParameters largeParams;

        public CurveGroupAlgebraTests()
        {
            ecParams = new CurveParameters(
                p: BigPrime.CreateWithoutChecks(23),
                a: -2,
                b: 9,
                generator: new CurvePoint(5, 3),
                order: BigPrime.CreateWithoutChecks(11),
                cofactor: 2
            );
            largeParams = new CurveParameters(
                p: BigPrime.CreateWithoutChecks(18392027), // 25 bits
                generator: CurvePoint.PointAtInfinity,
                order: BigPrime.CreateWithoutChecks(3),
                a: 0, b: 0, cofactor: 1
            );
        }

        [Test]
        public void TestAddDoublePoint()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var other = p.Clone();

            var expectedQ = new CurvePoint(15, 14);
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDoublePointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = CurvePoint.PointAtInfinity;
            var other = p.Clone();

            var expectedQ = CurvePoint.PointAtInfinity;
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDifferentPoints()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var other = new CurvePoint(15, 14);

            var expected = new CurvePoint(16, 15);
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddPointAtInfinityLeft()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var other = CurvePoint.PointAtInfinity;

            var result = curve.Add(other, p);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddPointAtInfinityRight()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var other = CurvePoint.PointAtInfinity;

            var result = curve.Add(p, other);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddNegated()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var other = curve.Negate(p);

            var expected = CurvePoint.PointAtInfinity;
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void TestAreNegationsFalseForEqualPoint()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var other = p.Clone();

            Assert.IsFalse(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAreNegationsTrueForNegation()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var other = curve.Negate(p);

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAreNegationsTrueForZeroYPoint()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(11, 0);
            var other = p.Clone();

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAddAffine()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(11, 0);

            var q = curve.Add(p, p);

            Assert.AreEqual(CurvePoint.PointAtInfinity, q);
        }
        
        [Test]
        [TestCase(1, 10)]
        [TestCase(2, 17)]
        [TestCase(6, 11)]
        [TestCase(13, 8)]
        [TestCase(18, 3)]
        [TestCase(0, 20)]
        public void TestPointValidTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsValid(point));
        }

        [Test]
        public void TestIsValidFalseForPointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            Assert.IsFalse(curve.IsValid(CurvePoint.PointAtInfinity));
        }

        [Test]
        [TestCase(16, 1)]
        [TestCase(5, 2)]
        [TestCase(-2, 1)]
        [TestCase(16, -15)]
        [TestCase(78, 4)]
        [TestCase(4, 78)]
        public void TestIsValidFalseForPointNotOnCurve(int xRaw, int yRaw)
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsValid(point));
        }

        [Test]
        [TestCase(10, 0)]
        public void TestIsValidFalseForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsValid(point));
        }

        [Test]
        public void TestNegate()
        {
            var curve = new CurveGroupAlgebra(ecParams);

            var p = new CurvePoint(5, 5);
            var expected = new CurvePoint(5, 23 - 5);

            var result = curve.Negate(p);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForZeroYPoint()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(11, 0);

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestNegatePointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = CurvePoint.PointAtInfinity;

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
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);

            var q = curve.MultiplyScalar(p, k);
            var expectedQ = new CurvePoint(expectedX, expectedY);
            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var result = curve.MultiplyScalar(p, 8);

            Assert.AreEqual(curve.NeutralElement, result);
        }

        [Test]
        public void TestGroupElementBitLength()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            Assert.AreEqual(2*5, curve.ElementBitLength);
        }

        [Test]
        public void TestOrderIsAsSet()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            Assert.AreEqual(ecParams.Order, curve.Order);
        }

        [Test]
        public void TestOrderBitLength()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            Assert.AreEqual(4, curve.OrderBitLength);
        }
        
        [Test]
        public void TestNeutralElement()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            Assert.AreEqual(CurvePoint.PointAtInfinity, curve.NeutralElement);
        }

        [Test]
        public void TestGeneratorIsAsSet()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            Assert.AreEqual(ecParams.Generator, curve.Generator);
        }

        [Test]
        public void TestFromBytes()
        {
            var curve = new CurveGroupAlgebra(largeParams);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesRejectsTooShortBuffer()
        {
            var curve = new CurveGroupAlgebra(largeParams);
            var buffer = new byte[7];
            Assert.Throws<ArgumentException>(
                () => curve.FromBytes(buffer)
            );
        }

        [Test]
        public void TestToBytes()
        {
            var curve = new CurveGroupAlgebra(largeParams);
            var p = new CurvePoint(5, 3);
            var expected = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 3 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new CurveGroupAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var expected = new byte[] { 5, 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var invalidGenerator = new CurvePoint(16, 1);
            CurveParameters invalidParams = new CurveParameters(
                generator: invalidGenerator,
                p: ecParams.P,
                a: ecParams.A,
                b: ecParams.B,
                order: ecParams.Order,
                cofactor: ecParams.Cofactor
            );
                
            Assert.Throws<ArgumentException>(
                () => new CurveGroupAlgebra(invalidParams)
            );
        }

        [Test]
        public void TestProperties()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);

            Assert.AreEqual(CurvePoint.PointAtInfinity, groupAlgebra.NeutralElement, "verifying neutral element");
            Assert.AreEqual(ecParams.Generator, groupAlgebra.Generator, "verifying generator");
            Assert.AreEqual(ecParams.Order, groupAlgebra.Order, "verifying order");
            Assert.AreEqual(ecParams.Cofactor, groupAlgebra.Cofactor, "verifying cofactor");
        }


        [Test]
        public void TestEqualsTrue()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);
            var otherAlgebra = new CurveGroupAlgebra(ecParams);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);

            bool result = groupAlgebra.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);
            var otherAlgebra = new object { };

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);

            var otherParams = largeParams;
            var otherAlgebra = new CurveGroupAlgebra(otherParams);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);
            var otherAlgebra = new CurveGroupAlgebra(ecParams);

            Assert.AreEqual(groupAlgebra.GetHashCode(), otherAlgebra.GetHashCode());
        }
    }
}
