using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class CurvePointTests
    {
        // reference results from https://trustica.cz/en/2018/04/26/elliptic-curves-prime-order-curves/
        // curve has order 32, i.e., OrderSize = 6 bits

        [Test]
        public void TestClone()
        {
            var p = new CurvePoint(5, 5);
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }

        [Test]
        public void TestClonePointAtInfinity()
        {
            var p = CurvePoint.PointAtInfinity;
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }
        
        [Test]
        public void TestEquals()
        {
            var p = new CurvePoint(11, 0);
            var q = new CurvePoint(3, 4);

            Assert.AreEqual(p, p, "same");
            Assert.AreEqual(p, p.Clone(), "equal");
            Assert.AreEqual(CurvePoint.PointAtInfinity, CurvePoint.PointAtInfinity, "at inf");

            Assert.AreNotEqual(p, q, "not equal");
            Assert.AreNotEqual(q, p, "not equal reversed");
        }
        
        [Test]
        public void TestPointEquality()
        {
            Assert.AreEqual(CurvePoint.PointAtInfinity, CurvePoint.PointAtInfinity);

            var p = new CurvePoint(14, 3);
            var q = new CurvePoint(14, 3);
            Assert.AreEqual(p, q);
            Assert.AreEqual(q, p);
        }

        [Test]
        public void TestToStringRegularPoint()
        {
            var p = new CurvePoint(1, 2);
            var expected = "(1, 2)";
            Assert.AreEqual(expected, p.ToString());
        }

        [Test]
        public void TestToStringPointAtInfinity()
        {
            var p = CurvePoint.PointAtInfinity;
            var expected = "(at infinity)";
            Assert.AreEqual(expected, p.ToString());
        }
    }
}
