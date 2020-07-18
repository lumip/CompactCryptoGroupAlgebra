using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    public static class TestCurveParameters
    {
        /// <summary>Valid parameters for a small prime-order Weierstrass curve</summary>
        public static readonly CurveParameters WeierstrassParameters = new CurveParameters(
            p: BigPrime.CreateWithoutChecks(23),
            a: -2,
            b: 9,
            generator: new CurvePoint(5, 3),
            order: BigPrime.CreateWithoutChecks(11),
            cofactor: 2
        );

        /// <summary>A parameter set for testing serialization of elements larger than one byte;
        /// not a valid set of a curve</summary>
        public static readonly CurveParameters LargeParameters = new CurveParameters(
            p: BigPrime.CreateWithoutChecks(18392027), // 25 bits
            generator: CurvePoint.PointAtInfinity,
            order: BigPrime.CreateWithoutChecks(3),
            a: 0, b: 0, cofactor: 1
        );

        /// <summary>Valid parameters for a small prime-order Montgomery curve</summary>
        public static readonly CurveParameters MontgomeryParameters = new CurveParameters(
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
}