using System;
using System.Numerics;
using System.Security.Cryptography;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class MontgomeryCurveAlgebraTests
    {
        private CurveParameters ecParams;

        public MontgomeryCurveAlgebraTests()
        {
            ecParams = new CurveParameters(
                p: BigPrime.CreateWithoutChecks(41),
                a: 4,
                b: 3,
                generator: new CurvePoint(2, 6),
                order: BigPrime.CreateWithoutChecks(11),
                cofactor: 4
            );
            // generated points
            //1: (2, 6)
            //2: (6, 9)
            //3: (23, 9)
            //4: (38, 24)
            //5: (8, 32)
            //6: (15, 6)
            //7: (20, 35)
            //8: (30, 40)
            //9: (18, 39)
            //10: (28, 7)
            //11: (atInf)

            // all curve points
            //(44,
            // 11,
            // 4,
            // 3,
            // [(0, (array([0]),)),
            //  (1, (array([17, 24]),)),
            //  (2, (array([6, 35]),)),
            //  (6, (array([9, 32]),)),
            //  (7, (array([10, 31]),)),
            //  (8, (array([9, 32]),)),
            //  (11, (array([12, 29]),)),
            //  (15, (array([6, 35]),)),
            //  (16, (array([20, 21]),)),
            //  (18, (array([2, 39]),)),
            //  (20, (array([6, 35]),)),
            //  (21, (array([19, 22]),)),
            //  (22, (array([15, 26]),)),
            //  (23, (array([9, 32]),)),
            //  (25, (array([8, 33]),)),
            //  (26, (array([20, 21]),)),
            //  (27, (array([11, 30]),)),
            //  (28, (array([7, 34]),)),
            //  (30, (array([1, 40]),)),
            //  (36, (array([20, 21]),)),
            //  (38, (array([17, 24]),)),
            //  (39, (array([17, 24]),))]
            //)


        }

        [Test]
        public void TestAddSame()
        {
            var point = new CurvePoint(2, 6);
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = new CurvePoint(6, 9);
            var result = algebra.Add(point, point);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddDifferent()
        {
            var point = new CurvePoint(2, 6);
            var otherPoint = new CurvePoint(8, 32);
            var algebra = new MontgomeryCurveAlgebra(ecParams);

            var expected = new CurvePoint(15, 6);
            var result = algebra.Add(otherPoint, point);

            Assert.AreEqual(expected, result);
        }


        [Test]
        [TestCase(1, 2, 6, 2, 6)]
        [TestCase(2, 2, 6, 6, 9)]
        [TestCase(6, 2, 6, 15, 6)]
        [TestCase(8, 2, 6, 30, 40)]
        [TestCase(9, 2, 6, 18, 39)]
        [TestCase(10, 2, 6, 28, 7)]
        public void TestMultiplyScalar(int k, int x, int y, int expectedX, int expectedY)
        {
            var algebra = new MontgomeryCurveAlgebra(ecParams);
            var expected = new CurvePoint(new BigInteger(expectedX), new BigInteger(expectedY));

            var p = new CurvePoint(new BigInteger(x), new BigInteger(y));
            var result = algebra.MultiplyScalar(p, k);

            Assert.AreEqual(expected, result);
        }

        //[Test]
        //public void GetOrder()
        //{
        //    var point = new CurvePoint(2, 6);
        //    var algebra = new MontgomeryCurveAlgebra(ecParams);

        //    for (int i = 1; i < ecParams.Order; ++i)
        //    {
        //        var r = algebra.MultiplyScalar(point, i);
        //        Console.WriteLine(String.Format("{0}: {1}", i, r));
        //        //Assert.IsTrue(algebra.IsElement(r));
        //    }

        //    //Assert.IsTrue(algebra.IsElement(point), "not valid");
        //    var point2 = algebra.MultiplyScalar(point, 2);
        //    Assert.AreNotEqual(point2, CurvePoint.PointAtInfinity, "order 2");
        //    var point4 = algebra.MultiplyScalar(point, 4);
        //    Assert.AreNotEqual(point4, CurvePoint.PointAtInfinity, "order 4");
        //    var point11 = algebra.MultiplyScalar(point, 11);
        //    Assert.AreEqual(point11, CurvePoint.PointAtInfinity, "order not 11");

        //}

    }
}
