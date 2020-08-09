using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class WeierstrassCurveEquationTests
    {
        [Test]
        public void TestAddDoublePoint()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = p.Clone();

            var expectedQ = new CurvePoint(15, 14);
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDoublePointAtInfinity()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = CurvePoint.PointAtInfinity;
            var other = p.Clone();

            var expectedQ = CurvePoint.PointAtInfinity;
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDifferentPoints()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = new CurvePoint(15, 14);

            var expected = new CurvePoint(16, 15);
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddPointAtInfinityLeft()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = CurvePoint.PointAtInfinity;

            var result = curve.Add(other, p);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddPointAtInfinityRight()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = CurvePoint.PointAtInfinity;

            var result = curve.Add(p, other);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddNegated()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = curve.Negate(p);

            var expected = CurvePoint.PointAtInfinity;
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }
        

        [Test]
        public void TestAddAffine()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
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
        public void TestIsElementTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsPointOnCurve(point));
        }

        [Test]
        public void TestIsElementFalseForPointAtInfinity()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            Assert.IsFalse(curve.IsPointOnCurve(CurvePoint.PointAtInfinity));
        }

        [Test]
        [TestCase(16, 1)]
        [TestCase(5, 2)]
        [TestCase(-2, 1)]
        [TestCase(16, -15)]
        [TestCase(78, 4)]
        [TestCase(4, 78)]
        public void TestIsElementFalseForPointNotOnCurve(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsPointOnCurve(point));
        }

        [Test]
        [TestCase(10, 0)]
        public void TestIsElementTrueForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsPointOnCurve(point));
        }

        [Test]
        public void TestEqualsTrue()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = TestCurveParameters.WeierstrassParameters.Equation;

            bool result = curve.Equals(otherCurve);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;

            bool result = curve.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = new object();

            bool result = curve.Equals(otherCurve);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = new WeierstrassCurveEquation(prime: BigPrime.CreateWithoutChecks(11), 0, 0);

            bool result = curve.Equals(otherCurve);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = TestCurveParameters.WeierstrassParameters.Equation;

            Assert.AreEqual(curve.GetHashCode(), otherCurve.GetHashCode());
        }

    }
}
