using System;
using System.Numerics;
using System.Security.Cryptography;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class MontgomeryCurveAlgebraTests
    {
        private readonly CurveParameters ecParams;
        private readonly CurveParameters largeParams;

        public MontgomeryCurveAlgebraTests()
        {
            ecParams = new CurveParameters(
                p: BigPrime.CreateWithoutChecks(41),
                a: 4,
                b: 3,
                generator: new CurvePoint(2, 6),
                order: BigPrime.CreateWithoutChecks(11),
                cofactor: 4
            );
            // generated points
            //1: (2, 6)
            //2: (6, 9)
            //3: (23, 9)
            //4: (38, 24)
            //5: (8, 32)
            //6: (15, 6)
            //7: (20, 35)
            //8: (30, 40)
            //9: (18, 39)
            //10: (28, 7)
            //11: (atInf)

            // all curve points
            //(44,
            // 11,
            // 4,
            // 3,
            // [(0, (array([0]),)),
            //  (1, (array([17, 24]),)),
            //  (2, (array([6, 35]),)),
            //  (6, (array([9, 32]),)),
            //  (7, (array([10, 31]),)),
            //  (8, (array([9, 32]),)),
            //  (11, (array([12, 29]),)),
            //  (15, (array([6, 35]),)),
            //  (16, (array([20, 21]),)),
            //  (18, (array([2, 39]),)),
            //  (20, (array([6, 35]),)),
            //  (21, (array([19, 22]),)),
            //  (22, (array([15, 26]),)),
            //  (23, (array([9, 32]),)),
            //  (25, (array([8, 33]),)),
            //  (26, (array([20, 21]),)),
            //  (27, (array([11, 30]),)),
            //  (28, (array([7, 34]),)),
            //  (30, (array([1, 40]),)),
            //  (36, (array([20, 21]),)),
            //  (38, (array([17, 24]),)),
            //  (39, (array([17, 24]),))]
            //)

            largeParams = new CurveParameters(
                p: BigPrime.CreateWithoutChecks(18392027), // 25 bits
                generator: CurvePoint.PointAtInfinity,
                order: BigPrime.CreateWithoutChecks(3),
                a: 0, b: 0, cofactor: 1
            );
        }

        [Test]
        public void TestConstructorAndProperties()
        {
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            Assert.AreEqual(ecParams.Cofactor, algebra.Cofactor);
            Assert.AreEqual(CurvePoint.PointAtInfinity, algebra.NeutralElement);
            Assert.AreEqual(2*NumberLength.GetLength(ecParams.P).InBits, algebra.ElementBitLength);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var invalidGenerator = new CurvePoint(0, 0);
            CurveParameters invalidParams = new CurveParameters(
                generator: invalidGenerator,
                p: ecParams.P,
                a: ecParams.A,
                b: ecParams.B,
                order: ecParams.Order,
                cofactor: ecParams.Cofactor
            );
                
            Assert.Throws<ArgumentException>(
                () => new MontgomeryCurveAlgebra(invalidParams)
            );
        }

        [Test]
        public void TestAddSame()
        {
            var point = new CurvePoint(2, 6);
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = new CurvePoint(6, 9);
            var result = algebra.Add(point, point);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddDifferent()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = new CurvePoint(8, 32);
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = new CurvePoint(15, 6);
            var result = algebra.Add(otherPoint, point);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNeutralElementRight()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = CurvePoint.PointAtInfinity;
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = point;
            var result = algebra.Add(point, otherPoint);
            
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNeutralElementLeft()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = CurvePoint.PointAtInfinity;
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = point;
            var result = algebra.Add(otherPoint, point);
            
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNeutralElements()
        {
            var point = CurvePoint.PointAtInfinity;
            var otherPoint = CurvePoint.PointAtInfinity;
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = CurvePoint.PointAtInfinity;
            var result = algebra.Add(otherPoint, point);
            
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNegatedElements()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = new CurvePoint(2, 41 - 6);
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = CurvePoint.PointAtInfinity;
            var result = algebra.Add(otherPoint, point);
            
            Assert.AreEqual(expected, result);
        }


        [Test]
        [TestCase(1, 2, 6, 2, 6)]
        [TestCase(2, 2, 6, 6, 9)]
        [TestCase(6, 2, 6, 15, 6)]
        [TestCase(8, 2, 6, 30, 40)]
        [TestCase(9, 2, 6, 18, 39)]
        [TestCase(10, 2, 6, 28, 7)]
        public void TestMultiplyScalar(int k, int x, int y, int expectedX, int expectedY)
        {
            var algebra = new MontgomeryCurveAlgebra(ecParams);
            var expected = new CurvePoint(new BigInteger(expectedX), new BigInteger(expectedY));

            var p = new CurvePoint(new BigInteger(x), new BigInteger(y));
            var result = algebra.MultiplyScalar(p, k);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegate()
        {
            var point = new CurvePoint(2, 6);
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = new CurvePoint(2, 41 - 6);
            var result = algebra.Negate(point);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateNeutralElement()
        {
            var algebra = new MontgomeryCurveAlgebra(ecParams);
            var point = algebra.NeutralElement;

            var expected = point;
            var result = algebra.Negate(point);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var groupAlgebra = new MontgomeryCurveAlgebra(ecParams);

            bool result = groupAlgebra.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var groupAlgebra = new MontgomeryCurveAlgebra(ecParams);
            var otherAlgebra = new object { };

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var groupAlgebra = new MontgomeryCurveAlgebra(ecParams);

            var otherParams = largeParams;
            var otherAlgebra = new MontgomeryCurveAlgebra(otherParams);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var groupAlgebra = new MontgomeryCurveAlgebra(ecParams);
            var otherAlgebra = new MontgomeryCurveAlgebra(ecParams);

            Assert.AreEqual(groupAlgebra.GetHashCode(), otherAlgebra.GetHashCode());
        }

        [Test]
        public void TestFromBytes()
        {
            var curve = new MontgomeryCurveAlgebra(largeParams);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytes()
        {
            var curve = new MontgomeryCurveAlgebra(largeParams);
            var p = new CurvePoint(5, 3);
            var expected = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new MontgomeryCurveAlgebra(ecParams);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 3 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new MontgomeryCurveAlgebra(ecParams);
            var p = new CurvePoint(5, 5);
            var expected = new byte[] { 5, 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesRejectsTooShortBuffer()
        {
            var curve = new MontgomeryCurveAlgebra(largeParams);
            var buffer = new byte[7];
            Assert.Throws<ArgumentException>(
                () => curve.FromBytes(buffer)
            );
        }


        [Test]
        [TestCase(38, 24)]
        [TestCase(2, 6)]
        [TestCase(18, 39)]
        public void TestIsElementTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = new MontgomeryCurveAlgebra(ecParams);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsElement(point));
        }

        [Test]
        public void TestIsElementFalseForPointAtInfinity()
        {
            var curve = new MontgomeryCurveAlgebra(ecParams);
            Assert.IsFalse(curve.IsElement(CurvePoint.PointAtInfinity));
        }

        [Test]
        [TestCase(3, 1)]
        [TestCase(1, 6)]
        [TestCase(-2, 1)]
        [TestCase(1, -4)]
        [TestCase(25, 7)]
        public void TestIsElementFalseForPointNotOnCurve(int xRaw, int yRaw)
        {
            var curve = new MontgomeryCurveAlgebra(ecParams);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        [TestCase(0, 0)]
        public void TestIsElementFalseForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curve = new MontgomeryCurveAlgebra(ecParams);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        public void TestCreateCryptoGroup()
        {
            var groupAlgebra = new MontgomeryCurveAlgebra(ecParams);
            var group = MontgomeryCurveAlgebra.CreateCryptoGroup(ecParams);
            Assert.AreEqual(groupAlgebra, group.Algebra);
        }

    }
}
