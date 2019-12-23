﻿using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.Tests
{
    [TestFixture]
    public class ECPointTests
    {
        // reference results from https://trustica.cz/en/2018/04/26/elliptic-curves-prime-order-curves/
        // curve has order 32, i.e., OrderSize = 6 bits

        [Test]
        public void TestClone()
        {
            var p = new CompactCryptoGroupAlgebra.ECPoint(5, 5);
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }

        [Test]
        public void TestClonePointAtInfinity()
        {
            var p = CompactCryptoGroupAlgebra.ECPoint.PointAtInfinity;
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }
        
        [Test]
        public void TestEquals()
        {
            var p = new CompactCryptoGroupAlgebra.ECPoint(11, 0);
            var q = new CompactCryptoGroupAlgebra.ECPoint(3, 4);

            Assert.AreEqual(p, p, "same");
            Assert.AreEqual(p, p.Clone(), "equal");
            Assert.AreEqual(CompactCryptoGroupAlgebra.ECPoint.PointAtInfinity, CompactCryptoGroupAlgebra.ECPoint.PointAtInfinity, "at inf");

            Assert.AreNotEqual(p, q, "not equal");
            Assert.AreNotEqual(q, p, "not equal reversed");
        }
        
        [Test]
        public void TestPointEquality()
        {
            Assert.AreEqual(CompactCryptoGroupAlgebra.ECPoint.PointAtInfinity, CompactCryptoGroupAlgebra.ECPoint.PointAtInfinity);

            var p = new CompactCryptoGroupAlgebra.ECPoint(14, 3);
            var q = new CompactCryptoGroupAlgebra.ECPoint(14, 3);
            Assert.AreEqual(p, q);
            Assert.AreEqual(q, p);
        }
    }
}