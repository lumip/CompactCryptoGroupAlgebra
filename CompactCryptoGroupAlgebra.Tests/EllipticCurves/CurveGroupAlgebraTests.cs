using System;
using System.Numerics;

using NUnit.Framework;
using Moq;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class CurveGroupAlgebraTests
    {

        private readonly CurveParameters curveParams = TestCurveParameters.WeierstrassParameters;
        private readonly CurveParameters largeParams = TestCurveParameters.LargeParameters;

        private Mock<CurveEquation> curveEquationMock;
        private Mock<CurveEquation> largeCurveEquationMock;

        public CurveGroupAlgebraTests()
        {
            curveEquationMock = new Mock<CurveEquation>(curveParams) { CallBase = true };
            largeCurveEquationMock = new Mock<CurveEquation>(largeParams) { CallBase = true };
        }

        [SetUp]
        public void Setup()
        {
            curveEquationMock = new Mock<CurveEquation>(curveParams) { CallBase = true };
            curveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(true);
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));

            largeCurveEquationMock = new Mock<CurveEquation>(largeParams) { CallBase = true };
            largeCurveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(true);
            largeCurveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));
        }

        [Test]
        public void TestAdd()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);

            var p = new CurvePoint(5, 5);
            var other = new CurvePoint(15, 14);
            var expected = new CurvePoint(20, 19);

            curveEquationMock.Reset();
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));

            var result = curve.Add(p, other);
            Assert.AreEqual(expected, result);
            curveEquationMock.Verify(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()), Times.Once);
        }

        [Test]
        public void TestIsElementTrueForValidPoint()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            var point = new CurvePoint(2, 17);
            Assert.IsTrue(curve.IsElement(point));
        }

        [Test]
        public void TestIsElementFalseForPointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>())).Returns(CurvePoint.PointAtInfinity);
            Assert.IsFalse(curve.IsElement(CurvePoint.PointAtInfinity));
        }

        [Test]
        public void TestIsElementFalseForPointNotOnCurve()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            curveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(false);
            var point = new CurvePoint(16, 1);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        public void TestIsElementFalseForLowOrderCurvePoint()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            var point = new CurvePoint(10, 0);

            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>())).Returns(CurvePoint.PointAtInfinity);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        public void TestNegate()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);

            var p = new CurvePoint(5, 5);
            var expected = new CurvePoint(5, 18);

            var result = curve.Negate(p);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForZeroYPoint()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            var p = new CurvePoint(11, 0);

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestNegatePointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            var p = CurvePoint.PointAtInfinity;

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestMultiplyScalar()
        {
            var k = new BigInteger(5);
            
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            var p = new CurvePoint(5, 5);

            curveEquationMock.Reset();
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));

            var expectedQ = new CurvePoint(25, 25);
            var q = curve.MultiplyScalar(p, k);

            Assert.AreEqual(expectedQ, q);
            curveEquationMock.Verify(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()), Times.Exactly(2*4)); // 2 * Order bit length
        }

        [Test]
        public void TestGroupElementBitLength()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            Assert.AreEqual(2*5, curve.ElementBitLength);
        }

        [Test]
        public void TestOrderIsAsSet()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            Assert.AreEqual(curveParams.Order, curve.Order);
        }

        [Test]
        public void TestOrderBitLength()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            Assert.AreEqual(4, curve.OrderBitLength);
        }
        
        [Test]
        public void TestNeutralElement()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            Assert.AreEqual(CurvePoint.PointAtInfinity, curve.NeutralElement);
        }

        [Test]
        public void TestGeneratorIsAsSet()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            Assert.AreEqual(curveParams.Generator, curve.Generator);
        }

        [Test]
        public void TestCofactor()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            Assert.AreEqual(curveParams.Cofactor, curve.Cofactor);
        }

        [Test]
        public void TestFromBytes()
        {
            var curve = new CurveGroupAlgebra(largeCurveEquationMock.Object);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesRejectsTooShortBuffer()
        {
            var curve = new CurveGroupAlgebra(largeCurveEquationMock.Object);
            var buffer = new byte[7];
            Assert.Throws<ArgumentException>(
                () => curve.FromBytes(buffer)
            );
        }

        [Test]
        public void TestToBytes()
        {
            var curve = new CurveGroupAlgebra(largeCurveEquationMock.Object);
            var p = new CurvePoint(5, 3);
            var expected = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 3 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new CurveGroupAlgebra(curveEquationMock.Object);
            var p = new CurvePoint(5, 5);
            var expected = new byte[] { 5, 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            curveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(false);
                
            Assert.Throws<ArgumentException>(
                () => new CurveGroupAlgebra(curveEquationMock.Object)
            );
        }

        [Test]
        public void TestEqualsTrue()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);
            var otherAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);

            bool result = groupAlgebra.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);
            var otherAlgebra = new object { };

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);
            var otherAlgebra = new CurveGroupAlgebra(largeCurveEquationMock.Object);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);
            var otherAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);

            Assert.AreEqual(groupAlgebra.GetHashCode(), otherAlgebra.GetHashCode());
        }

        [Test]
        public void TestCreateCryptoGroup()
        {
            var expectedGroupAlgebra = new CurveGroupAlgebra(curveEquationMock.Object);
            var group = CurveGroupAlgebra.CreateCryptoGroup(curveEquationMock.Object);
            Assert.AreEqual(expectedGroupAlgebra, group.Algebra);
        }
    }
}
