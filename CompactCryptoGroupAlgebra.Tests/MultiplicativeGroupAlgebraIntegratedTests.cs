﻿using System;
using System.Numerics;

using NUnit.Framework;

using CompactCryptoGroupAlgebra;

namespace CompactCryptoGroupAlgebra.Tests.CryptoAlgebra
{
    [TestFixture]
    public class MultiplicativeGroupAlgebraIntegratedTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(6)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(232)]
        public void TestGenerateIsGeneratorMultiplied(int idInt)
        {
            var id = new BigInteger(idInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            Assert.AreEqual(groupAlgebra.MultiplyScalar(groupAlgebra.Generator, id), groupAlgebra.GenerateElement(id));
        }

        [Test]
        public void TestGenerateRejectsNegativeIds()
        {
            var id = new BigInteger(-1);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => groupAlgebra.GenerateElement(id)
            );
        }

        [Test]
        public void TestMultiplyScalarRejectsNegativeScalars()
        {
            var k = new BigInteger(-1);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(3);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => groupAlgebra.MultiplyScalar(x, k)
            );
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(7)]
        public void TestMultiplyScalarWithSmallFactorSizeEqualToOrderFactorSize(int factorInt)
        {
            int factorBitLength = 3;
            var k = new BigInteger(factorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);
            Assert.AreEqual(groupAlgebra.MultiplyScalar(x, k), groupAlgebra.MultiplyScalar(x, k, factorBitLength));
        }

        [Test]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsNegativeScalars()
        {
            int factorBitLength = 3;
            var k = new BigInteger(-1);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => groupAlgebra.MultiplyScalar(x, k, factorBitLength)
            );
        }

        [Test]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(123)]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsLargerFactors(int factorInt)
        {
            int factorBitLength = 3;
            var k = new BigInteger(factorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => groupAlgebra.MultiplyScalar(x, k, factorBitLength)
            );
        }
    }
}