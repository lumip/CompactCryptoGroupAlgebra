using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class MontgomeryCurveEquationTests
    {

        [Test]
        public void TestAddSame()
        {
            var point = new CurvePoint(2, 6);
            var curve = TestCurveParameters.MontgomeryParameters.Equation;

            var expected = new CurvePoint(6, 9);
            var result = curve.Add(point, point);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddDifferent()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = new CurvePoint(8, 32);
            var curve = TestCurveParameters.MontgomeryParameters.Equation;

            var expected = new CurvePoint(15, 6);
            var result = curve.Add(otherPoint, point);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNeutralElementRight()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = CurvePoint.PointAtInfinity;
            var curve = TestCurveParameters.MontgomeryParameters.Equation;

            var expected = point;
            var result = curve.Add(point, otherPoint);
            
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNeutralElementLeft()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = CurvePoint.PointAtInfinity;
            var curve = TestCurveParameters.MontgomeryParameters.Equation;

            var expected = point;
            var result = curve.Add(otherPoint, point);
            
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNeutralElements()
        {
            var point = CurvePoint.PointAtInfinity;
            var otherPoint = CurvePoint.PointAtInfinity;
            var curve = TestCurveParameters.MontgomeryParameters.Equation;

            var expected = CurvePoint.PointAtInfinity;
            var result = curve.Add(otherPoint, point);
            
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddNegatedElements()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = new CurvePoint(2, 41 - 6);
            var curve = TestCurveParameters.MontgomeryParameters.Equation;

            var expected = CurvePoint.PointAtInfinity;
            var result = curve.Add(otherPoint, point);
            
            Assert.AreEqual(expected, result);
        }


        [Test]
        [TestCase(38, 24)]
        [TestCase(2, 6)]
        [TestCase(18, 39)]
        public void TestIsElementTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsPointOnCurve(point));
        }

        [Test]
        public void TestIsElementTrueForPointAtInfinity()
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            Assert.IsTrue(curve.IsPointOnCurve(CurvePoint.PointAtInfinity));
        }

        [Test]
        [TestCase(3, 1)]
        [TestCase(1, 6)]
        [TestCase(-2, 1)]
        [TestCase(1, -4)]
        [TestCase(25, 7)]
        public void TestIsElementFalseForPointNotOnCurve(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsPointOnCurve(point));
        }

        [Test]
        [TestCase(0, 0)]
        public void TestIsElementTrueForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsPointOnCurve(point));
        }

        [Test]
        public void TestEqualsTrue()
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            var otherCurve = TestCurveParameters.MontgomeryParameters.Equation;

            bool result = curve.Equals(otherCurve);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;

            bool result = curve.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            var otherCurve = new object();

            bool result = curve.Equals(otherCurve);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            var otherCurve = new MontgomeryCurveEquation(prime: BigPrime.CreateWithoutChecks(11), 0, 0);

            bool result = curve.Equals(otherCurve);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var curve = TestCurveParameters.MontgomeryParameters.Equation;
            var otherCurve = TestCurveParameters.MontgomeryParameters.Equation;

            Assert.AreEqual(curve.GetHashCode(), otherCurve.GetHashCode());
        }

    }
}
