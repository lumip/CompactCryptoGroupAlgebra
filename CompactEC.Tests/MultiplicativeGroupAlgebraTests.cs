using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CompactEC;

// Best Practices: https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
namespace CompactEC.Tests.CryptoAlgebra
{
    [TestClass]
    public class MultiplicativeGroupAlgebraTests
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
        public void TestGroupElementBitLength()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.AreEqual(4, groupAlgebra.ElementBitLength);
        }

        [TestMethod]
        public void TestOrderBitLength()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 5, 3);
            Assert.AreEqual(3, groupAlgebra.OrderBitLength);
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

        [TestMethod]
        public void TestFromBytes()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var expected = new BigInteger(7);
            var buffer = expected.ToByteArray();

            var result = groupAlgebra.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFromBytesRejectsNullArgument()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.ThrowsException<ArgumentNullException>(
                () => groupAlgebra.FromBytes(null)
            );
        }

        [TestMethod]
        public void TestToBytes()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var element = new BigInteger(7);
            var expected = element.ToByteArray();

            var result = groupAlgebra.ToBytes(element);

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
