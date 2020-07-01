using System;
using System.Numerics;

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
            var generator = 3;
            var order = BigPrime.CreateWithoutChecks(11);
            int orderBitLength = 4;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength)
            );
        }

        [Test]
        public void TestFactorBitLengthCorrect()
        {
            var generator = 3;
            var order = BigPrime.CreateWithoutChecks(11);
            int orderBitLength = 4;
            int factorBitLength = 2;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            Debug.Assert(baseAlgebraStub.Object.OrderBitLength == orderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(factorBitLength, fixedAlgebra.FactorBitLength);
        }

        [Test]
        public void TestOrderCallsBaseAlgebra()
        {
            var generator = 3;
            var order = BigPrime.CreateWithoutChecks(7);
            int orderBitLength = 3;
            int factorBitLength = 2;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(order, fixedAlgebra.Order);
        }

        [Test]
        public void TestOrderBitLengthCallsBaseAlgebra()
        {
            var generator = 3;
            var order = BigPrime.CreateWithoutChecks(7);
            int orderBitLength = 3;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(baseAlgebraStub.Object.OrderBitLength, fixedAlgebra.OrderBitLength);
        }

        [Test]
        public void TestGeneratorCallsBaseAlgebra()
        {
            int generator = 23;
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(generator, fixedAlgebra.Generator);
        }

        [Test]
        public void TestNeutralElementCallsBaseAlgebra()
        {
            var generator = 3;
            int neutralElement = 3;
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            baseAlgebraStub.Setup(alg => alg.NeutralElement).Returns(neutralElement);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(neutralElement, fixedAlgebra.NeutralElement);
        }

        [Test]
        public void TestElementBitLengthCallsBaseAlgebra()
        {
            var generator = 3;
            int elementBitLength = 3;
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            baseAlgebraStub.Setup(alg => alg.ElementBitLength).Returns(elementBitLength);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(elementBitLength, fixedAlgebra.ElementBitLength);
        }

        [Test]
        public void TestCofactorCallsBaseAlgebra()
        {
            var generator = 3;
            int factorBitLength = 1;
            var order = BigPrime.CreateWithoutChecks(23);
            var cofactor = new BigInteger(8);

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            baseAlgebraStub.Setup(alg => alg.Cofactor).Returns(cofactor);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.AreEqual(cofactor, fixedAlgebra.Cofactor);
        }

        [Test]
        public void TestAddCallsBaseAlgebra()
        {
            var generator = 3;
            int leftElement = 11;
            int rightElement = 6;
            int expected = 17;
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
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
            var generator = 3;
            byte[] buffer = new byte[0];
            int expected = 17;
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
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
            var generator = 3;
            var order = BigPrime.CreateWithoutChecks(1031);
            int orderBitLength = 11;
            int factorBitLength = 7;
            var index = new BigInteger(1 << (indexBitLength - 1));
            Debug.Assert(NumberLength.GetLength(index).InBits == indexBitLength);

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.Throws<ArgumentException>(
                () => fixedAlgebra.GenerateElement(index)
            );
        }

        [Test]
        public void TestGenerateElementCallsBaseAlgebra()
        {
            var order = BigPrime.CreateWithoutChecks(1031);
            int orderBitLength = 11;
            int factorBitLength = 7;
            var index = new BigInteger(1 << (factorBitLength - 1));
            int expected = 236;
            int generator = 4;

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
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
        public void TestIsElementCallsBaseAlgebra()
        {
            var generator = 3;
            int element = 9;
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;


            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.IsElementDerived(It.IsAny<int>())).Returns(true);
            baseAlgebraMock.Setup(alg => alg.Cofactor).Returns(1);
            Debug.Assert(orderBitLength == baseAlgebraMock.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraMock.Object, factorBitLength);
            Assert.IsTrue(fixedAlgebra.IsElement(element));

            baseAlgebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(
                    alg => alg.IsElementDerived(It.Is<int>(x => x == element)),
                    Times.Once()
            );
        }

        [Test]
        [TestCase(6)]
        [TestCase(8)]
        public void TestMultiplyScalarRejectsFactorWithDeviationBitLength(int bitLength)
        {
            var generator = 3;
            var order = BigPrime.CreateWithoutChecks(1031);
            int orderBitLength = 11;
            int factorBitLength = 7;
            int element = 2;
            var k = new BigInteger(1 << (bitLength - 1));
            Debug.Assert(NumberLength.GetLength(k).InBits == bitLength);

            var baseAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
            Debug.Assert(orderBitLength == baseAlgebraStub.Object.OrderBitLength);

            var fixedAlgebra = new FixedFactorLengthCryptoGroupAlgebra<int>(baseAlgebraStub.Object, factorBitLength);
            Assert.Throws<ArgumentException>(
                () => fixedAlgebra.MultiplyScalar(element, k)
            );
        }

        [Test]
        public void TestMultiplyScalarCallsBaseAlgebra()
        {
            var generator = 3;
            var order = BigPrime.CreateWithoutChecks(1031);
            int orderBitLength = 11;
            int element = 2;
            int expected = 124;
            int factorBitLength = 7;
            var factor = new BigInteger(1 << (factorBitLength - 1));
            Debug.Assert(NumberLength.GetLength(factor).InBits == factorBitLength);

            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
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
            var generator = 3;
            int element = 9;
            int expected = 35;
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;


            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
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
            var generator = 3;
            int element = 9;
            var expected = new byte[0];
            var order = BigPrime.CreateWithoutChecks(23);
            int orderBitLength = 5;
            int factorBitLength = 1;


            var baseAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order);
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
