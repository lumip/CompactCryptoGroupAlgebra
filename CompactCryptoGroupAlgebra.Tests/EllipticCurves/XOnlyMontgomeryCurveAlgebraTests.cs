using System;
using System.Numerics;
using System.Security.Cryptography;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class XOnlyMontgomeryCurveAlgebraTests
    {
        private readonly CurveParameters ecParams;
        private readonly CurveParameters largeParams;

        public XOnlyMontgomeryCurveAlgebraTests()
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
        [TestCase(0, 2, 0)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 2, 6)]
        [TestCase(3, 2, 23)]
        [TestCase(4, 2, 38)]
        [TestCase(5, 2, 8)]
        [TestCase(6, 2, 15)]
        [TestCase(7, 2, 20)]
        [TestCase(8, 2, 30)]
        [TestCase(9, 2, 18)]
        [TestCase(10, 2, 28)]
        [TestCase(11, 2, 0)]
        public void TestMultiplyScalar(int k, int x, int expectedX)
        {
            var algebra = new XOnlyMontgomeryCurveAlgebra(ecParams);

            var p = new BigInteger(x);
            var result = algebra.MultiplyScalar(p, k);

            Assert.AreEqual(new BigInteger(expectedX), result);
        }

        [Test]
        public void TestConstructorAndProperties()
        {
            var algebra = new XOnlyMontgomeryCurveAlgebra(ecParams);

            Assert.AreEqual(ecParams.Generator.X, algebra.Generator);
            Assert.AreEqual(ecParams.Order, algebra.Order);
            Assert.AreEqual(ecParams.Cofactor, algebra.Cofactor);
            Assert.AreEqual(NumberLength.GetLength(ecParams.P).InBits, algebra.ElementBitLength);
        }

        [Test]
        public void TestAddThrowsNotSupported()
        {
            var point = new BigInteger(2);
            var otherPoint = new BigInteger(6);
            var algebra = new XOnlyMontgomeryCurveAlgebra(ecParams);

            Assert.Throws<NotSupportedException>(
                () => algebra.Add(point, otherPoint)
            );
        }

        [Test]
        public void TestNegateThrowsNotSupported()
        {
            var point = new BigInteger(2);
            var algebra = new XOnlyMontgomeryCurveAlgebra(ecParams);

            Assert.Throws<NotSupportedException>(
                () => algebra.Negate(point)
            );
        }


        [Test]
        public void TestFromBytes()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(largeParams);
            var expected = new BigInteger(5);
            var buffer = new byte[] { 5 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytes()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(largeParams);
            var p = new BigInteger(5);
            var expected = new byte[] { 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);
            var expected = new BigInteger(5);
            var buffer = new byte[] { 5 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);
            var p = new BigInteger(5);
            var expected = new byte[] { 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestIsElement()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);
            var p = new BigInteger(7);

            Assert.IsTrue(curve.IsElement(p));
        }

        [Test]
        public void TestIsElementFalseForLowOrderElements()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);
            var p = new BigInteger(0);
            
            Assert.IsFalse(curve.IsElement(p));
        }

        [Test]
        public void TestEqualsTrueForEqual()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);
            var otherCurve = new XOnlyMontgomeryCurveAlgebra(ecParams);

            Assert.IsTrue(curve.Equals(otherCurve));
        }

        [Test]
        public void TestEqualsFalseForDifferent()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);
            var otherCurve = new XOnlyMontgomeryCurveAlgebra(largeParams);

            Assert.IsFalse(curve.Equals(otherCurve));
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);

            Assert.IsFalse(curve.Equals(null));
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(ecParams);
            var otherCurve = new XOnlyMontgomeryCurveAlgebra(ecParams);

            Assert.AreEqual(curve.GetHashCode(), otherCurve.GetHashCode());
        }

        

    }
}
