using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.Tests
{
    [TestFixture]
    public class ECGroupAlgebraTests
    {

        private readonly ECParameters ecParams;

        public ECGroupAlgebraTests()
        {
            ecParams = new ECParameters(
                p: 23,
                a: -2,
                b: 9,
                generator: new ECPoint(5, 3),
                order: 11,
                cofactor: 2,
                rng: new SeededRandomNumberGenerator()
            );
        }

        [Test]
        public void TestAddDoublePoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var other = p.Clone();

            var expectedQ = new ECPoint(15, 14);
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDoublePointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = ECPoint.PointAtInfinity;
            var other = p.Clone();

            var expectedQ = ECPoint.PointAtInfinity;
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDifferentPoints()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var other = new ECPoint(15, 14);

            var expected = new ECPoint(16, 15);
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddPointAtInfinityLeft()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var other = ECPoint.PointAtInfinity;

            var result = curve.Add(other, p);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddPointAtInfinityRight()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var other = ECPoint.PointAtInfinity;

            var result = curve.Add(p, other);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddNegated()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var other = curve.Negate(p);

            var expected = ECPoint.PointAtInfinity;
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void TestAreNegationsFalseForEqualPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var other = p.Clone();

            Assert.IsFalse(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAreNegationsTrueForNegation()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var other = curve.Negate(p);

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAreNegationsTrueForZeroYPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(11, 0);
            var other = p.Clone();

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [Test]
        public void TestAddAffine()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(11, 0);

            var q = curve.Add(p, p);

            Assert.AreEqual(ECPoint.PointAtInfinity, q);
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
            var curve = new ECGroupAlgebra(ecParams);
            var point = new ECPoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsValid(point));
        }

        [Test]
        public void TestIsValidTrueForPointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.IsTrue(curve.IsValid(ECPoint.PointAtInfinity));
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
            var curve = new ECGroupAlgebra(ecParams);
            var point = new ECPoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsValid(point));
        }

        [Test]
        [TestCase(10, 0)]
        public void TestIsValidFalseForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curve = new ECGroupAlgebra(ecParams);
            var point = new ECPoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsValid(point));
        }

        [Test]
        public void TestNegate()
        {
            var curve = new ECGroupAlgebra(ecParams);

            var p = new ECPoint(5, 5);
            var expected = new ECPoint(5, 23 - 5);

            var result = curve.Negate(p);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForZeroYPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(11, 0);

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestNegatePointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = ECPoint.PointAtInfinity;

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
            var p = new ECPoint(5, 5);

            var q = curve.MultiplyScalar(p, k);
            var expectedQ = new ECPoint(expectedX, expectedY);
            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
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
            Assert.AreEqual(4, curve.OrderBitLength);
        }
        
        [Test]
        public void TestNeutralElement()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(ECPoint.PointAtInfinity, curve.NeutralElement);
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
            ECParameters largeParams = new ECParameters(
                p: 18392027, // 25 bits
                generator: ECPoint.PointAtInfinity,
                order: 3,
                a: 0, b: 0, cofactor: 1, rng: new SeededRandomNumberGenerator()
            );

            var curve = new ECGroupAlgebra(largeParams);
            var expected = new ECPoint(5, 3);
            var buffer = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

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
            ECParameters largeParams = new ECParameters(
                p: 18392027, // 25 bits
                generator: ECPoint.PointAtInfinity,
                order: 3,
                a: 0, b: 0, cofactor: 1, rng: new SeededRandomNumberGenerator()
            );
            var curve = new ECGroupAlgebra(largeParams);
            var buffer = new byte[7];
            Assert.Throws<ArgumentException>(
                () => curve.FromBytes(buffer)
            );
        }

        [Test]
        public void TestToBytes()
        {
            ECParameters largeParams = new ECParameters(
                p: 18392027, // 25 bits
                generator: ECPoint.PointAtInfinity,
                order: 3,
                a: 0, b: 0, cofactor: 1, rng: new SeededRandomNumberGenerator()
            );

            var curve = new ECGroupAlgebra(largeParams);
            var p = new ECPoint(5, 3);
            var expected = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var expected = new ECPoint(5, 3);
            var buffer = new byte[] { 5, 3 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new ECPoint(5, 5);
            var expected = new byte[] { 5, 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var invalidGenerator = new ECPoint(16, 1);
            ECParameters invalidParams = new ECParameters(
                generator: invalidGenerator,
                p: ecParams.P,
                a: ecParams.A,
                b: ecParams.B,
                order: ecParams.Order,
                cofactor: ecParams.Cofactor,
                rng: new SeededRandomNumberGenerator()
            );
                
            Assert.Throws<ArgumentException>(
                () => new ECGroupAlgebra(invalidParams)
            );
        }

        [Test]
        public void TestProperties()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);

            Assert.AreEqual(ECPoint.PointAtInfinity, groupAlgebra.NeutralElement, "verifying neutral element");
            Assert.AreEqual(ecParams.Generator, groupAlgebra.Generator, "verifying generator");
            Assert.AreEqual(ecParams.Order, groupAlgebra.Order, "verifying order");
            Assert.AreEqual(ecParams.Cofactor, groupAlgebra.Cofactor, "verifying cofactor");
        }


        [Test]
        public void TestEqualsTrue()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            var otherAlgebra = new ECGroupAlgebra(ecParams);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            MultiplicativeGroupAlgebra otherAlgebra = null;

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            var otherAlgebra = new object { };

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            ECParameters otherParams = new ECParameters(
                p: 18392027, // 25 bits
                generator: ECPoint.PointAtInfinity,
                order: 3,
                a: 0, b: 0, cofactor: 1, rng: new SeededRandomNumberGenerator()
            );
            var otherAlgebra = new ECGroupAlgebra(otherParams);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            var otherAlgebra = new ECGroupAlgebra(ecParams);

            Assert.AreEqual(groupAlgebra.GetHashCode(), otherAlgebra.GetHashCode());
        }
    }
}
