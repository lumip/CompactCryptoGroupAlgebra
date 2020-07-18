using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class MontgomeryCurveEquationIntegrationTests
    {
        private static readonly CurveParameters curveParameters = TestCurveParameters.MontgomeryParameters;

        [Test]
        [TestCase(1, 2, 6, 2, 6)]
        [TestCase(2, 2, 6, 6, 9)]
        [TestCase(6, 2, 6, 15, 6)]
        [TestCase(8, 2, 6, 30, 40)]
        [TestCase(9, 2, 6, 18, 39)]
        [TestCase(10, 2, 6, 28, 7)]
        public void TestMultiplyScalar(int k, int x, int y, int expectedX, int expectedY)
        {
            var algebra = new CurveGroupAlgebra(new MontgomeryCurveEquation(curveParameters));
            var expected = new CurvePoint(new BigInteger(expectedX), new BigInteger(expectedY));

            var p = new CurvePoint(new BigInteger(x), new BigInteger(y));
            var result = algebra.MultiplyScalar(p, k);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curve = new CurveGroupAlgebra(new MontgomeryCurveEquation(curveParameters));
            var p = new CurvePoint(2, 6);
            var result = curve.MultiplyScalar(p, 11);

            Assert.AreEqual(curve.NeutralElement, result);
        }

        [Test]
        [TestCase(38, 24)]
        [TestCase(2, 6)]
        [TestCase(18, 39)]
        public void TestIsElementTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = new CurveGroupAlgebra(new MontgomeryCurveEquation(curveParameters));
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsElement(point));
        }

        [Test]
        public void TestIsElementFalseForPointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(new MontgomeryCurveEquation(curveParameters));
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
            var curve = new CurveGroupAlgebra(new MontgomeryCurveEquation(curveParameters));
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        [TestCase(0, 0)]
        public void TestIsElementFalseForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curve = new CurveGroupAlgebra(new MontgomeryCurveEquation(curveParameters));
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsElement(point));
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var invalidGenerator = new CurvePoint(0, 0);
            CurveParameters invalidParameters = new CurveParameters(
                generator: invalidGenerator,
                p: curveParameters.P,
                a: curveParameters.A,
                b: curveParameters.B,
                order: curveParameters.Order,
                cofactor: curveParameters.Cofactor
            );
                
            Assert.Throws<ArgumentException>(
                () => new CurveGroupAlgebra(new MontgomeryCurveEquation(invalidParameters))
            );
        }
    }
}