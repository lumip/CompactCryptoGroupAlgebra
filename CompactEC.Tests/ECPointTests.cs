using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompactEC.Tests
{
    [TestClass]
    public class ECPointTests
    {
        // reference results from https://trustica.cz/en/2018/04/26/elliptic-curves-prime-order-curves/
        // curve has order 32, i.e., OrderSize = 6 bits

        [TestMethod]
        public void TestClone()
        {
            var p = new CompactEC.ECPoint(5, 5);
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }

        [TestMethod]
        public void TestClonePointAtInfinity()
        {
            var p = CompactEC.ECPoint.PointAtInfinity;
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }
        
        [TestMethod]
        public void TestEquals()
        {
            var p = new CompactEC.ECPoint(11, 0);
            var q = new CompactEC.ECPoint(3, 4);

            Assert.AreEqual(p, p, "same");
            Assert.AreEqual(p, p.Clone(), "equal");
            Assert.AreEqual(CompactEC.ECPoint.PointAtInfinity, CompactEC.ECPoint.PointAtInfinity, "at inf");

            Assert.AreNotEqual(p, q, "not equal");
            Assert.AreNotEqual(q, p, "not equal reversed");
        }
        
        [TestMethod]
        public void TestPointEquality()
        {
            Assert.AreEqual(CompactEC.ECPoint.PointAtInfinity, CompactEC.ECPoint.PointAtInfinity);

            var p = new CompactEC.ECPoint(14, 3);
            var q = new CompactEC.ECPoint(14, 3);
            Assert.AreEqual(p, q);
            Assert.AreEqual(q, p);
        }
    }
}
