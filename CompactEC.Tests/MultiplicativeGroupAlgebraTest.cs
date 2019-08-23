using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CompactEC.CryptoAlgebra;

// Best Practices: https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
namespace CompactEC.Tests.CryptoAlgebra
{
    [TestClass]
    public class MultiplicativeGroupAlgebraTest
    {
        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 3)]
        [DataRow(2, 9)]
        [DataRow(3, 5)]
        [DataRow(4, 4)]
        [DataRow(5, 1)]
        [DataRow(6, 3)]
        [DataRow(12, 9)]
        public void TestMultiplyScalar(int scalarInt, int expectedInt)
        {
            var k = new BigInteger(scalarInt);
            var expected = new BigInteger(expectedInt);

            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            var x = new BigInteger(3);
            var result = groupAlgebra.MultiplyScalar(x, k);
            Assert.AreEqual(expected, result);
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
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(7)]
        public void TestGeneratorIsAsSet(int generatorInt)
        {
            var generator = new BigInteger(generatorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, generator);
            Assert.AreEqual(generator, groupAlgebra.Generator);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-3)]
        [DataRow(11)]
        [DataRow(136)]
        public void TestInvalidElementRejectedAsGenerator(int generatorInt)
        {
            var generator = new BigInteger(generatorInt);
            Assert.ThrowsException<ArgumentException>(
                () => new MultiplicativeGroupAlgebra(11, 10, generator)
            );
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(10)]
        public void TestIsValidAcceptsValidElements(int elementInt)
        {
            var element = new BigInteger(elementInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.IsTrue(groupAlgebra.IsValid(element));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-3)]
        [DataRow(11)]
        [DataRow(136)]
        public void TestIsValidRejectsInvalidElements(int elementInt)
        {
            var element = new BigInteger(elementInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.IsFalse(groupAlgebra.IsValid(element));
        }

        [TestMethod]
        public void TestGroupElementBitlength()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.AreEqual(4, groupAlgebra.GroupElementBitlength);
        }

        [TestMethod]
        public void TestOrderBitlength()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 5, 3);
            Assert.AreEqual(3, groupAlgebra.OrderBitlength);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(4)]
        [DataRow(7)]
        public void TestMultiplyScalarWithSmallFactorSizeEqualToOrderFactorSize(int factorInt)
        {
            int factorBitlength = 3;
            var k = new BigInteger(factorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);
            Assert.AreEqual(groupAlgebra.MultiplyScalar(x, k), groupAlgebra.MultiplyScalar(x, k, factorBitlength));
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
            int factorBitlength = 3;
            var k = new BigInteger(factorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var x = new BigInteger(6);
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => groupAlgebra.MultiplyScalar(x, k, factorBitlength)
            );
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(10)]
        public void TestNegate(int elementInt)
        {
            var x = new BigInteger(elementInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.AreEqual(groupAlgebra.MultiplyScalar(x, groupAlgebra.Order - 1), groupAlgebra.Negate(x));
        }

        [TestMethod]
        public void TestNeutralElement()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.AreEqual(BigInteger.One, groupAlgebra.NeutralElement);
        }
    }
}
