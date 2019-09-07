using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompactEC.Tests
{
    [TestClass]
    public class ECGroupAlgebraTest
    {
        // reference results from https://trustica.cz/en/2018/04/26/elliptic-curves-prime-order-curves/
        // generator has order 16, i.e., OrderSize = 5 bits

        private readonly ECParameters ecParams;

        public ECGroupAlgebraTest()
        {
            ecParams = new ECParameters()
            {
                P = 23,
                A = -2,
                B = 2,
                Generator = new CompactEC.ECPoint(1, 1),
                Order = 16
            };
        }

        [TestMethod]
        public void TestAddDoublePoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = p.Clone();

            var expectedQ = new CompactEC.ECPoint(15, 14);
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [TestMethod]
        public void TestAddDoublePointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = CompactEC.ECPoint.PointAtInfinity;
            var other = p.Clone();

            var expectedQ = CompactEC.ECPoint.PointAtInfinity;
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [TestMethod]
        public void TestAddDifferentPoints()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = new CompactEC.ECPoint(15, 14);

            var expected = new CompactEC.ECPoint(16, 15);
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAddPointAtInfinityLeft()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = CompactEC.ECPoint.PointAtInfinity;

            var result = curve.Add(other, p);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [TestMethod]
        public void TestAddPointAtInfinityRight()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = CompactEC.ECPoint.PointAtInfinity;

            var result = curve.Add(p, other);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [TestMethod]
        public void TestAddNegated()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = curve.Negate(p);

            var expected = CompactEC.ECPoint.PointAtInfinity;
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }
        
        [TestMethod]
        public void TestAreNegationsFalseForEqualPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = p.Clone();

            Assert.IsFalse(curve.AreNegations(p, other));
        }

        [TestMethod]
        public void TestAreNegationsTrueForNegation()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var other = curve.Negate(p);

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [TestMethod]
        public void TestAreNegationsTrueForZeroYPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(11, 0);
            var other = p.Clone();

            Assert.IsTrue(curve.AreNegations(p, other));
        }

        [TestMethod]
        public void TestAddAffine()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(11, 0);

            var q = curve.Add(p, p);

            Assert.AreEqual(CompactEC.ECPoint.PointAtInfinity, q);
        }
        
        [TestMethod]
        [DataRow(5, 5)]
        [DataRow(11, 0)]
        [DataRow(16, 15)]
        public void TestPointValidTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = new ECGroupAlgebra(ecParams);
            var point = new CompactEC.ECPoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsValid(point));
        }

        [TestMethod]
        public void TestIsValidTrueForPointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.IsTrue(curve.IsValid(CompactEC.ECPoint.PointAtInfinity));
        }

        [TestMethod]
        [DataRow(16, 1)]
        [DataRow(5, 2)]
        [DataRow(-2, 1)]
        [DataRow(16, -15)]
        [DataRow(78, 4)]
        [DataRow(4, 78)]
        public void TestIsvalidFalseForInvalidPoint(int xRaw, int yRaw)
        {
            var curve = new ECGroupAlgebra(ecParams);
            var point = new CompactEC.ECPoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsValid(point));
        }

        [TestMethod]
        public void TestNegate()
        {
            var curve = new ECGroupAlgebra(ecParams);

            var p = new CompactEC.ECPoint(5, 5);
            var expected = new CompactEC.ECPoint(5, 23 - 5);

            var result = curve.Negate(p);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestNegateForZeroYPoint()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(11, 0);

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [TestMethod]
        public void TestNegatePointAtInfinity()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = CompactEC.ECPoint.PointAtInfinity;

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [TestMethod]
        [DataRow(2, 15, 14)]
        [DataRow(5, 16, 8)]
        [DataRow(7, 5, 18)]
        [DataRow(9, 5, 5)]
        public void TestMultiplyScalar(int kRaw, int expectedX, int expectedY)
        {
            var k = new BigInteger(kRaw);
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);

            var q = curve.MultiplyScalar(p, k);
            var expectedQ = new CompactEC.ECPoint(expectedX, expectedY);
            Assert.AreEqual(expectedQ, q);
        }

        [TestMethod]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var result = curve.MultiplyScalar(p, 8);

            Assert.AreEqual(curve.NeutralElement, result);
        }

        [TestMethod]
        public void TestGroupElementBitLength()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(2*5, curve.ElementBitLength);
        }

        [TestMethod]
        public void TestOrderIsAsSet()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(ecParams.Order, curve.Order);
        }

        [TestMethod]
        public void TestOrderBitLength()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(5, curve.OrderBitLength);
        }
        
        [TestMethod]
        public void TestNeutralElement()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(CompactEC.ECPoint.PointAtInfinity, curve.NeutralElement);
        }

        [TestMethod]
        public void TestGeneratorIsAsSet()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.AreEqual(ecParams.Generator, curve.Generator);
        }

        [TestMethod]
        public void TestFromBytes()
        {
            ECParameters largeParams = new ECParameters()
            {
                P = 134217728, // == 2 ^ 27
                Generator = CompactEC.ECPoint.PointAtInfinity,
                Order = 1
            };

            var curve = new ECGroupAlgebra(largeParams);
            var expected = new CompactEC.ECPoint(5, 5);
            var buffer = new byte[] { 5, 0, 0, 0, 5, 0, 0, 0 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFromBytesRejectsNullArgument()
        {
            var curve = new ECGroupAlgebra(ecParams);
            Assert.ThrowsException<ArgumentNullException>(
                () => curve.FromBytes(null)
            );
        }

        [TestMethod]
        public void TestFromBytesRejectsTooShortBuffer()
        {
            ECParameters largeParams = new ECParameters()
            {
                P = 134217728, // == 2 ^ 27
                Generator = CompactEC.ECPoint.PointAtInfinity,
                Order = 1
            };
            var curve = new ECGroupAlgebra(largeParams);
            var buffer = new byte[7];
            Assert.ThrowsException<ArgumentException>(
                () => curve.FromBytes(buffer)
            );
        }

        [TestMethod]
        public void TestToBytes()
        {
            ECParameters largeParams = new ECParameters()
            {
                P = 134217728, // == 2 ^ 27
                Generator = CompactEC.ECPoint.PointAtInfinity,
                Order = 1
            };

            var curve = new ECGroupAlgebra(largeParams);
            var p = new CompactEC.ECPoint(5, 5);
            var expected = new byte[] { 5, 0, 0, 0, 5, 0, 0, 0 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var expected = new CompactEC.ECPoint(5, 5);
            var buffer = new byte[] { 5, 5 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new ECGroupAlgebra(ecParams);
            var p = new CompactEC.ECPoint(5, 5);
            var expected = new byte[] { 5, 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var generator = new CompactEC.ECPoint(16, 1);
            var invalidParams = ecParams;
            invalidParams.Generator = generator;
            Assert.ThrowsException<ArgumentException>(
                () => new ECGroupAlgebra(invalidParams)
            );
        }
    }
}
