using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CompactEC.CryptoAlgebra;

namespace CompactEC.Tests.CryptoAlgebra
{
    [TestClass]
    public class MultiplicativeGroupAlgebraIntegratedTest
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(6)]
        [DataRow(9)]
        [DataRow(10)]
        [DataRow(11)]
        [DataRow(232)]
        public void TestGenerateIsGeneratorMultiplied(int idInt)
        {
            var id = new BigInteger(idInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            Assert.AreEqual(groupAlgebra.MultiplyScalar(groupAlgebra.Generator, id), groupAlgebra.GenerateElement(id));
        }

        [TestMethod]
        public void TestGenerateRejectsNegativeIds()
        {
            var id = new BigInteger(-1);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => groupAlgebra.GenerateElement(id)
            );
        }

        [TestMethod]
        public void TestMultiplyScalarRejectsNegativeScalars()
        {
            var k = new BigInteger(-1);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(3);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => groupAlgebra.MultiplyScalar(x, k)
            );
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(4)]
        [DataRow(7)]
        public void TestMultiplyScalarWithSmallFactorSizeEqualToOrderFactorSize(int factorInt)
        {
            int factorBitLength = 3;
            var k = new BigInteger(factorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);
            Assert.AreEqual(groupAlgebra.MultiplyScalar(x, k), groupAlgebra.MultiplyScalar(x, k, factorBitLength));
        }

        [TestMethod]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsNegativeScalars()
        {
            int factorBitLength = 3;
            var k = new BigInteger(-1);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => groupAlgebra.MultiplyScalar(x, k, factorBitLength)
            );
        }

        [TestMethod]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(123)]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsLargerFactors(int factorInt)
        {
            int factorBitLength = 3;
            var k = new BigInteger(factorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => groupAlgebra.MultiplyScalar(x, k, factorBitLength)
            );
        }
    }
}
