using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class DefaultECParrameterTests
    {

        [Test]
        public void TestEqualsIsTrueForEqualObjects()
        {
            var p = BigPrime.CreateWithoutChecks(11);
            var order = BigPrime.CreateWithoutChecks(7);
            var a = new BigInteger(2);
            var b = new BigInteger(-3);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                p, a, b, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                p, a, b, generator, order, cofactor
            );

            Assert.AreEqual(parameters, otherParameters);
        }

        [Test]
        public void TestEqualsIsFalseForDifferentObjects()
        {
            var p = BigPrime.CreateWithoutChecks(11);
            var order = BigPrime.CreateWithoutChecks(7);
            var a = new BigInteger(2);
            var b = new BigInteger(-3);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                p, a, b, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                BigPrime.CreateWithoutChecks(7), a, b, generator, order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                p, new BigInteger(3), b, generator, order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                p, a, new BigInteger(5), generator, order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                p, a, b, new CurvePoint(1, 1), order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                p, a, b, generator, BigPrime.CreateWithoutChecks(3), cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                p, a, b, generator, order, BigInteger.One
            );
            Assert.AreNotEqual(parameters, otherParameters);
        }

        [Test]
        public void TestEqualsIsFalseForNull()
        {
            var p = BigPrime.CreateWithoutChecks(11);
            var order = BigPrime.CreateWithoutChecks(7);
            var a = new BigInteger(2);
            var b = new BigInteger(-3);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                p, a, b, generator, order, cofactor
            );

            Assert.AreNotEqual(parameters, null);
        }

        [Test]
        public void TestEqualsIsFalseForUnrelatedObject()
        {
            var p = BigPrime.CreateWithoutChecks(11);
            var order = BigPrime.CreateWithoutChecks(7);
            var a = new BigInteger(2);
            var b = new BigInteger(-3);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                p, a, b, generator, order, cofactor
            );

            Assert.AreNotEqual(parameters, new object { });
        }

        [Test]
        public void TestHashCodeIsSameForEqualObjects()
        {
            var p = BigPrime.CreateWithoutChecks(11);
            var order = BigPrime.CreateWithoutChecks(7);
            var a = new BigInteger(2);
            var b = new BigInteger(-3);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                p, a, b, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                p, a, b, generator, order, cofactor
            );

            Assert.AreEqual(parameters.GetHashCode(), otherParameters.GetHashCode());
        }

    }
}
