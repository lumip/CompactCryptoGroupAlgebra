using System;
using System.Numerics;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

using CompactEC;

namespace CompactEC.Tests.CryptoAlgebra
{

    [TestClass]
    public class FixedFactorLengthGroupAlgebraTests
    {
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(5)]
        public void TestConstructorRejectsInvalidFactorBitLength(int factorBitLength)
        {
            var order = new BigInteger(8);
            int orderBitLength = 4;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength)
            );
        }

        [TestMethod]
        public void TestConstructorRejectsNullBaseAlgebra()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new FixedFactorLengthCryptoGroupAlgebra<int>(null, 2)
            );
        }

        [TestMethod]
        public void TestFactorBitLengthCorrect()
        {
            var order = new BigInteger(8);
            int orderBitLength = 4;
            int factorBitLength = 2;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            Debug.Assert(baseAlgebraStub.Object.OrderBitLength == orderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(factorBitLength, fixedAlgebra.FactorBitLength);
        }

        [TestMethod]
        public void TestOrderCallsBaseAlgebra()
        {
            var order = new BigInteger(7);
            int orderBitLength = 3;
            int factorBitLength = 2;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(order, fixedAlgebra.Order);
        }

        [TestMethod]
        public void TestOrderBitLengthCallsBaseAlgebra()
        {
            var order = new BigInteger(7);
            int orderBitLength = 3;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(baseAlgebraStub.Object.OrderBitLength, fixedAlgebra.OrderBitLength);
        }

        [TestMethod]
        public void TestGeneratorCallsBaseAlgebra()
        {
            int generator = 23;
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            baseAlgebraStub.Setup(alg => alg.Generator).Returns(generator);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(generator, fixedAlgebra.Generator);
        }

        [TestMethod]
        public void TestNeutralElementCallsBaseAlgebra()
        {
            int neutralElement = 3;
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            baseAlgebraStub.Setup(alg => alg.NeutralElement).Returns(neutralElement);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(neutralElement, fixedAlgebra.NeutralElement);
        }

        [TestMethod]
        public void TestElementBitLengthCallsBaseAlgebra()
        {
            int elementBitLength = 3;
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            baseAlgebraStub.Setup(alg => alg.ElementBitLength).Returns(elementBitLength);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(elementBitLength, fixedAlgebra.ElementBitLength);
        }

        [TestMethod]
        public void TestAddCallsBaseAlgebra()
        {
            int leftElement = 11;
            int rightElement = 6;
            int expected = 17;
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraMock.Setup(alg => alg.Order).Returns(order);
            baseAlgebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.Add(leftElement, rightElement);

            Assert.AreEqual(expected, result);
            baseAlgebraMock.Verify(
                alg => alg.Add(It.Is<int>(x => x == leftElement), It.Is<int>(x => x == rightElement)),
                Times.Once()
            );
        }

        [TestMethod]
        public void TestFromBytesCallsBaseAlgebra()
        {
            byte[] buffer = new byte[0];
            int expected = 17;
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraMock.Setup(alg => alg.Order).Returns(order);
            baseAlgebraMock.Setup(alg => alg.FromBytes(It.IsAny<byte[]>())).Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.FromBytes(buffer);

            Assert.AreEqual(expected, result);
            baseAlgebraMock.Verify(
                alg => alg.FromBytes(It.Is<byte[]>(x => x == buffer)),
                Times.Once()
            );
        }

        [TestMethod]
        [DataRow(6)]
        [DataRow(8)]
        public void TestGenerateElementRejectsIndexWithDeviatingBitLength(int indexBitLength)
        {
            var order = new BigInteger(1024);
            int orderBitLength = 11;
            int factorBitLength = 7;
            var index = new BigInteger(1 << (indexBitLength - 1));
            Debug.Assert(CryptoGroupAlgebra<int>.GetBitLength(index) == indexBitLength);

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.ThrowsException<ArgumentException>(
                () => fixedAlgebra.GenerateElement(index)
            );
        }

        [TestMethod]
        public void TestGenerateElementCallsBaseAlgebra()
        {
            var order = new BigInteger(1024);
            int orderBitLength = 11;
            int factorBitLength = 7;
            var index = new BigInteger(1 << (factorBitLength - 1));
            int expected = 236;
            int generator = 4;

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraMock.Setup(alg => alg.Order).Returns(order);
            baseAlgebraMock.Setup(alg => alg.Generator).Returns(generator);
            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnsafe(It.IsAny<int>(), It.IsAny<BigInteger>(), It.IsAny<int>()))
                .Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.GenerateElement(index);

            Assert.AreEqual(expected, result);

            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().Verify(
                alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == generator), 
                                                It.Is<BigInteger>(x => x == index),
                                                It.Is<int>(x => x == factorBitLength)),
                Times.Once()
            );
        }

        [TestMethod]
        public void TestIsValidCallsBaseAlgebra()
        {
            int element = 9;
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;


            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraMock.Setup(alg => alg.Order).Returns(order);
            baseAlgebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            Assert.IsTrue(fixedAlgebra.IsValid(element));

            baseAlgebraMock.Verify(
                alg => alg.IsValid(It.Is<int>(x => x == element)),
                Times.Once()
            );
        }

        [TestMethod]
        [DataRow(6)]
        [DataRow(8)]
        public void TestMultiplyScalarRejectsFactorWithDeviationBitLength(int bitLength)
        {
            var order = new BigInteger(1024);
            int orderBitLength = 11;
            int factorBitLength = 7;
            int element = 2;
            var k = new BigInteger(1 << (bitLength - 1));
            Debug.Assert(CryptoGroupAlgebra<int>.GetBitLength(k) == bitLength);

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.ThrowsException<ArgumentException>(
                () => fixedAlgebra.MultiplyScalar(element, k)
            );
        }

        [TestMethod]
        public void TestMultiplyScalarCallsBaseAlgebra()
        {
            var order = new BigInteger(1024);
            int orderBitLength = 11;
            int element = 2;
            int expected = 124;
            int factorBitLength = 7;
            var factor = new BigInteger(1 << (factorBitLength - 1));
            Debug.Assert(CryptoGroupAlgebra<int>.GetBitLength(factor) == factorBitLength);

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraMock.Setup(alg => alg.Order).Returns(order);
            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnsafe(It.IsAny<int>(), It.IsAny<BigInteger>(), It.IsAny<int>()))
                .Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.MultiplyScalar(element, factor);

            Assert.AreEqual(expected, result);
            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().Verify(
                alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == element),
                                                It.Is<BigInteger>(x => x == factor),
                                                It.Is<int>(x => x == factorBitLength)),
                Times.Once()
            );
        }

        [TestMethod]
        public void TestNegateCallsBaseAlgebra()
        {
            int element = 9;
            int expected = 35;
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;


            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraMock.Setup(alg => alg.Order).Returns(order);
            baseAlgebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.Negate(element);

            Assert.AreEqual(expected, result);

            baseAlgebraMock.Verify(
                alg => alg.Negate(It.Is<int>(x => x == element)),
                Times.Once()
            );
        }

        [TestMethod]
        public void TestToBytesCallsBaseAlgebra()
        {
            int element = 9;
            var expected = new byte[0];
            var order = new BigInteger(24);
            int orderBitLength = 5;
            int factorBitLength = 1;


            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraMock.Setup(alg => alg.Order).Returns(order);
            baseAlgebraMock.Setup(alg => alg.ToBytes(It.IsAny<int>())).Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.ToBytes(element);

            CollectionAssert.AreEqual(expected, result);

            baseAlgebraMock.Verify(
                alg => alg.ToBytes(It.Is<int>(x => x == element)),
                Times.Once()
            );
        }
    }
}
