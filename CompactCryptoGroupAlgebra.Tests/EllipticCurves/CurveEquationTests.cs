using System.Numerics;

using NUnit.Framework;
using Moq;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{

    [TestFixture]
    public class CurveEquationTests
    {

        [Test]
        public void TestNegate()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(5, 5);
            var expected = new CurvePoint(5, 18);

            var result = equation.Object.Negate(testElement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForPointAtInfinity()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = CurvePoint.PointAtInfinity;
            var expected = CurvePoint.PointAtInfinity;

            var result = equation.Object.Negate(testElement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAreNegationsFalseForEqualPoint()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(5, 5);
            var otherElement = testElement.Clone();

            Assert.IsFalse(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForNegation()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(5, 5);
            var otherElement = equation.Object.Negate(testElement);

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForZeroYPoint()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(11, 0);
            var otherElement = testElement.Clone();

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForPointAtInfinity()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = CurvePoint.PointAtInfinity;
            var otherElement = testElement.Clone();

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestEqualsTrue()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            Assert.IsTrue(equationMock.Object.Equals(otherMock.Object));
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            Assert.IsFalse(equationMock.Object.Equals(null));
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            Assert.IsFalse(equationMock.Object.Equals(new object()));
        }

        [Test]
        public void TestEqualsFalseForDifferentObject()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(11),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };
            Assert.IsFalse(equationMock.Object.Equals(otherMock.Object));

            otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.One, BigInteger.One
            ) { CallBase = true };
            Assert.IsFalse(equationMock.Object.Equals(otherMock.Object));

            otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.Zero
            ) { CallBase = true };
            Assert.IsFalse(equationMock.Object.Equals(otherMock.Object));
        }
    }
}
