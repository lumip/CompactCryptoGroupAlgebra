using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class WeierstrassCurveEquationIntegrationTests
    {
        private readonly CurveParameters curveParameters = TestCurveParameters.WeierstrassParameters;

        [Test]
        [TestCase(1, 10)]
        [TestCase(2, 17)]
        [TestCase(6, 11)]
        [TestCase(13, 8)]
        [TestCase(18, 3)]
        [TestCase(0, 20)]
        public void TestIsElementTrueForValidPoint(int rawX, int rawY)
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(rawX, rawY);
            Assert.IsTrue(curve.IsElement(point));
        }

        [Test]
        public void TestIsElementFalseForPointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.IsFalse(curve.IsElement(CurvePoint.PointAtInfinity));
        }

        [Test]
        [TestCase(16, 1)]
        [TestCase(5, 2)]
        [TestCase(-2, 1)]
        [TestCase(16, -15)]
        [TestCase(78, 4)]
        [TestCase(4, 78)]
        public void TestIsElementFalseForPointNotOnCurve(int rawX, int rawY)
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(rawX, rawY);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        public void TestIsElementFalseForLowOrderCurvePoint()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(10, 0);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        [TestCase(2, 15, 14)]
        [TestCase(5, 16, 8)]
        [TestCase(7, 5, 18)]
        [TestCase(9, 5, 5)]
        public void TestMultiplyScalar(int kRaw, int expectedX, int expectedY)
        {
            var k = new BigInteger(kRaw);
            var curve = new CurveGroupAlgebra(curveParameters);
            var p = new CurvePoint(5, 5);

            var q = curve.MultiplyScalar(p, k);
            var expectedQ = new CurvePoint(expectedX, expectedY);
            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var p = new CurvePoint(5, 3);
            var result = curve.MultiplyScalar(p, 11);

            Assert.AreEqual(curve.NeutralElement, result);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var invalidGenerator = new CurvePoint(16, 1);
            CurveParameters invalidParameters = new CurveParameters(
                curveEquation: TestCurveParameters.WeierstrassParameters.Equation,
                generator: invalidGenerator,
                order: curveParameters.Order,
                cofactor: curveParameters.Cofactor
            );
                
            Assert.Throws<ArgumentException>(
                () => new CurveGroupAlgebra(invalidParameters)
            );
        }

    }
}