using System;
using System.Numerics;
using System.Diagnostics;

using NUnit.Framework;
using Moq;
using Moq.Protected;

namespace CompactCryptoGroupAlgebra.Tests
{

    [TestFixture]
    public class FixedFactorLengthGroupAlgebraTests
    {
        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        public void TestConstructorRejectsInvalidFactorBitLength(int factorBitLength)
        {
            var order = new BigInteger(8);
            int orderBitLength = 4;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength)
            );
        }

        [Test]
        public void TestConstructorRejectsNullBaseAlgebra()
        {
            Assert.Throws<ArgumentNullException>(
                () => new FixedFactorLengthCryptoGroupAlgebra<int>(null, 2)
            );
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void TestCofactorCallsBaseAlgebra()
        {
            int factorBitLength = 1;
            var order = new BigInteger(24);
            var cofactor = new BigInteger(8);

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            baseAlgebraStub.Setup(alg => alg.Order).Returns(order);
            baseAlgebraStub.Setup(alg => alg.Cofactor).Returns(cofactor);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(cofactor, fixedAlgebra.Cofactor);
        }

        [Test]
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

        [Test]
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

        [Test]
        [TestCase(6)]
        [TestCase(8)]
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
            Assert.Throws<ArgumentException>(
                () => fixedAlgebra.GenerateElement(index)
            );
        }

        [Test]
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
                .Setup(alg => alg.MultiplyScalarUnchecked(It.IsAny<int>(), It.IsAny<BigInteger>(), It.IsAny<int>()))
                .Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.GenerateElement(index);

            Assert.AreEqual(expected, result);

            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().Verify(
                alg => alg.MultiplyScalarUnchecked(It.Is<int>(x => x == generator), 
                                                It.Is<BigInteger>(x => x == index),
                                                It.Is<int>(x => x == factorBitLength)),
                Times.Once()
            );
        }

        [Test]
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

        [Test]
        [TestCase(6)]
        [TestCase(8)]
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
            Assert.Throws<ArgumentException>(
                () => fixedAlgebra.MultiplyScalar(element, k)
            );
        }

        [Test]
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
                .Setup(alg => alg.MultiplyScalarUnchecked(It.IsAny<int>(), It.IsAny<BigInteger>(), It.IsAny<int>()))
                .Returns(expected);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            var result = fixedAlgebra.MultiplyScalar(element, factor);

            Assert.AreEqual(expected, result);
            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().Verify(
                alg => alg.MultiplyScalarUnchecked(It.Is<int>(x => x == element),
                                                It.Is<BigInteger>(x => x == factor),
                                                It.Is<int>(x => x == factorBitLength)),
                Times.Once()
            );
        }

        [Test]
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

        [Test]
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
