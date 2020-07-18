using System;
using System.Numerics;

using NUnit.Framework;
using Moq;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{

    [TestFixture]
    public class CurveEquationTests
    {
        private readonly CurveParameters curveParameters = TestCurveParameters.WeierstrassParameters;

        [Test]
        public void TestCurveParametersProperty()
        {
            var equation = new Mock<CurveEquation>(curveParameters) { CallBase = true };
            Assert.AreEqual(curveParameters, equation.Object.CurveParameters);
        }

        [Test]
        public void TestNegate()
        {
            var equation = new Mock<CurveEquation>(curveParameters) { CallBase = true };

            var testElement = new CurvePoint(5, 5);
            var expected = new CurvePoint(5, 18);

            var result = equation.Object.Negate(testElement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForPointAtInfinity()
        {
            var equation = new Mock<CurveEquation>(curveParameters) { CallBase = true };

            var testElement = CurvePoint.PointAtInfinity;
            var expected = CurvePoint.PointAtInfinity;

            var result = equation.Object.Negate(testElement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAreNegationsFalseForEqualPoint()
        {
            var equation = new Mock<CurveEquation>(curveParameters) { CallBase = true };
            var testElement = new CurvePoint(5, 5);
            var otherElement = testElement.Clone();

            Assert.IsFalse(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForNegation()
        {
            var equation = new Mock<CurveEquation>(curveParameters) { CallBase = true };
            var testElement = new CurvePoint(5, 5);
            var otherElement = equation.Object.Negate(testElement);

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForZeroYPoint()
        {
            var equation = new Mock<CurveEquation>(curveParameters) { CallBase = true };
            var testElement = new CurvePoint(11, 0);
            var otherElement = testElement.Clone();

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForPointAtInfinity()
        {
            var equation = new Mock<CurveEquation>(curveParameters) { CallBase = true };
            var testElement = CurvePoint.PointAtInfinity;
            var otherElement = testElement.Clone();

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }
    }
}