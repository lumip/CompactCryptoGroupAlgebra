using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompactEC.UnitTests
{
    [TestClass]
    public class CurveAndPointTests
    {
        // reference results from https://trustica.cz/en/2018/04/26/elliptic-curves-prime-order-curves/
        // curve has order 32, i.e., OrderSize = 6 bits

        [TestMethod]
        public void TestClone()
        {
            var p = new RawPoint(5, 5);
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }

        [TestMethod]
        public void TestAdd()
        {
            var curve = new CurveAlgebra(23, -2, 2, 6);
            var p = new RawPoint(5, 5);

            var expectedQ = new RawPoint(15, 14);

            var q = curve.Add(p, p);
            Assert.AreEqual(expectedQ, q, "add same");

            q = curve.Add(p, p.Clone());
            Assert.AreEqual(expectedQ, q, "add equal");

            var expectedR = new RawPoint(16, 15);
            var r = curve.Add(q, p);

            Assert.AreEqual(expectedR, r, "add different");
            r = curve.Add(p, q);
            Assert.AreEqual(expectedR, r, "add different reversed");
        }

        [TestMethod]
        public void TestAreNegated()
        {
            var curve = new CurveAlgebra(23, -2, 2, 6);
            var p = new RawPoint(5, 5);
            Assert.IsFalse(curve.ArePointsAdditiveInverse(p, p), "same");
            Assert.IsFalse(curve.ArePointsAdditiveInverse(p, p.Clone()), "equal");
            Assert.IsTrue(curve.ArePointsAdditiveInverse(p, curve.Invert(p)), "inverse");
            Assert.IsTrue(curve.ArePointsAdditiveInverse(curve.Invert(p), p), "inverse reversed");

            p = new RawPoint(11, 0);
            Assert.IsTrue(curve.ArePointsAdditiveInverse(p, p), "zero y");
        }

        [TestMethod]
        public void TestAddInfinity()
        {
            var curve = new CurveAlgebra(23, -2, 2, 6);
            var p = new RawPoint(5, 5);

            var q = curve.Add(p, RawPoint.PointAtInfinity);
            Assert.AreEqual(p, q);

            q = curve.Add(RawPoint.PointAtInfinity, p);
            Assert.AreEqual(p, q);

            q = curve.Add(RawPoint.PointAtInfinity, RawPoint.PointAtInfinity);
            Assert.AreEqual(RawPoint.PointAtInfinity, q);
        }

        [TestMethod]
        public void TestAddNegated()
        {
            var curve = new CurveAlgebra(23, -2, 2, 6);
            var p = new RawPoint(5, 5);

            var nP = curve.Invert(p);
            Assert.IsTrue(curve.ArePointsAdditiveInverse(p, nP));

            var q = curve.Add(p, nP);
            Assert.AreEqual(RawPoint.PointAtInfinity, q);

            q = curve.Add(nP, p);
            Assert.AreEqual(RawPoint.PointAtInfinity, q);
        }

        [TestMethod]
        public void TestAddAffine()
        {
            var curve = new CurveAlgebra(23, -2, 2, 6);
            var p = new RawPoint(11, 0);

            var q = curve.Add(p, p);

            Assert.AreEqual(RawPoint.PointAtInfinity, q);
        }

        [TestMethod]
        public void TestEquals()
        {
            var p = new RawPoint(11, 0);
            var q = new RawPoint(3, 4);

            Assert.AreEqual(p, p, "same");
            Assert.AreEqual(p, p.Clone(), "equal");
            Assert.AreEqual(RawPoint.PointAtInfinity, RawPoint.PointAtInfinity, "at inf");

            Assert.AreNotEqual(p, q, "not equal");
            Assert.AreNotEqual(q, p, "not equal reversed");
        }

        [TestMethod]
        public void TestPointValid()
        {
            var curve = new CurveAlgebra(23, -2, 2, 6);
            Assert.IsTrue(curve.IsValidPoint(new RawPoint(5, 5)), "(5,5)");
            Assert.IsTrue(curve.IsValidPoint(new RawPoint(11, 0)), "(11, 0)");
            Assert.IsTrue(curve.IsValidPoint(new RawPoint(16, 15)), "(16, 15)");
            Assert.IsTrue(curve.IsValidPoint(RawPoint.PointAtInfinity), "(atInf)");

            Assert.IsFalse(curve.IsValidPoint(new RawPoint(16, 1)), "not (16, 1)");
            Assert.IsFalse(curve.IsValidPoint(new RawPoint(5, 2)), "not (5, 2)");
        }

        [TestMethod]
        public void TestPointEquality()
        {
            Assert.AreEqual(RawPoint.PointAtInfinity, RawPoint.PointAtInfinity);

            var p = new RawPoint(14, 3);
            var q = new RawPoint(14, 3);
            Assert.AreEqual(p, q);
            Assert.AreEqual(q, p);
        }

        [TestMethod]
        public void TestMultiply()
        {
            var curve = new CurveAlgebra(23, -2, 2, 6);
            var p = new RawPoint(5, 5);

            var q = curve.Multiply(p, 2);
            var expectedQ = curve.Add(p, p);
            Assert.AreEqual(expectedQ, q);

            q = curve.Multiply(p, 5);
            expectedQ = new RawPoint(16, 8);
            Assert.AreEqual(expectedQ, q);

            q = curve.Multiply(p, 7);
            expectedQ = new RawPoint(5, 18);
            Assert.AreEqual(expectedQ, q);

            q = curve.Multiply(p, 8);
            expectedQ = RawPoint.PointAtInfinity;
            Assert.AreEqual(expectedQ, q);

            q = curve.Multiply(p, 9);
            expectedQ = p;
            Assert.AreEqual(expectedQ, q);
        }
    }
}
