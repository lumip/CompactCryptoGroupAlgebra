﻿using System;
using System.Numerics;
using System.Security.Cryptography;

using NUnit.Framework;
using Moq;

namespace CompactCryptoGroupAlgebra
{

    [TestFixture]
    public class CryptoGroupTests
    {

        [Test]
        public void TestAddCallsAlgebra()
        {
            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.Add(2, 6)).Returns(8);
            algebraMock.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var left = new CryptoGroupElement<BigInteger, int>(2, algebraMock.Object);
            var right = new CryptoGroupElement<BigInteger, int>(6, algebraMock.Object);
            var expected = new CryptoGroupElement<BigInteger, int>(8, algebraMock.Object);

            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);

            Assert.AreEqual(expected, group.Add(left, right));
            algebraMock.Verify(algebra => algebra.Add(2, 6), Times.Once());
        }

        [Test]
        public void TestFromBytesCallsAlgebra()
        {
            byte[] inputBuffer = new byte[0];

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.FromBytes(It.IsAny<byte[]>())).Returns(0);

            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);

            var expected = new CryptoGroupElement<BigInteger, int>(0, algebraMock.Object);

            Assert.AreEqual(expected, group.FromBytes(inputBuffer));
            algebraMock.Verify(algebra => algebra.FromBytes(It.Is<byte[]>(b => b == inputBuffer)), Times.Once());
        }

        [Test]
        public void TestGenerateCallsAlgebra()
        {
            var index = new BigInteger(7);
            int expectedRaw = 3;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.GenerateElement(It.IsAny<BigInteger>())).Returns(expectedRaw);

            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);
            var expected = new CryptoGroupElement<BigInteger, int>(expectedRaw, algebraMock.Object);

            Assert.AreEqual(expected, group.Generate(index));

            algebraMock.Verify(
                algebra => algebra.GenerateElement(It.Is<BigInteger>(x => x == index)),
                Times.Once()
            );
        }

        [Test]
        public void TestMultiplyScalarCallsAlgebra()
        {
            var k = new BigInteger(7);
            int expectedRaw = 3;
            int elementRaw = 8;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expectedRaw);

            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);
            var expected = new CryptoGroupElement<BigInteger, int>(expectedRaw, algebraMock.Object);
            var element = new CryptoGroupElement<BigInteger, int>(elementRaw, algebraMock.Object);

            Assert.AreEqual(expected, group.MultiplyScalar(element, k));

            algebraMock.Verify(
                algebra => algebra.MultiplyScalar(It.Is<int>(x => x == elementRaw), It.Is<BigInteger>(x => x == k)),
                Times.Once()
            );
        }

        [Test]
        public void TestSpecificNegateCallsAlgebra()
        {
            int expectedRaw = 3;
            int elementRaw = 8;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.Negate(It.IsAny<int>())).Returns(expectedRaw);

            var groupMock = new CryptoGroup<BigInteger, int>(algebraMock.Object);
            var expected = new CryptoGroupElement<BigInteger, int>(expectedRaw, algebraMock.Object);
            var element = new CryptoGroupElement<BigInteger, int>(elementRaw, algebraMock.Object);

            Assert.AreEqual(expected, groupMock.Negate(element));

            algebraMock.Verify(
                algebra => algebra.Negate(It.Is<int>(x => x == elementRaw)),
                Times.Once()
            );
        }

        [Test]
        public void TestAddRejectsDifferentGroupElementLeft()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroup<BigInteger, int>(algebraStub.Object);

            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(3, algebraStub.Object);
            var otherElement = new CryptoGroupElement<BigInteger, int>(3, otherAlgebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => groupMock.Add(otherElement, element)
            );
        }

        [Test]
        public void TestAddRejectsDifferentGroupElementRight()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            var group = new CryptoGroup<BigInteger, int>(algebraStub.Object);

            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(3, algebraStub.Object);
            var otherElement = new CryptoGroupElement<BigInteger, int>(3, otherAlgebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => group.Add(element, otherElement)
            );
        }

        [Test]
        public void TestMultiplyScalarRejectsDifferentGroupElement()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            var groupMock = new Mock<CryptoGroup<BigInteger, int>>(MockBehavior.Strict, algebraStub.Object);

            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            var elementStub = new CryptoGroupElement<BigInteger, int>(3, otherAlgebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => groupMock.Object.MultiplyScalar(elementStub, new BigInteger(8))
            );
        }

        [Test]
        public void TestNegateRejectsDifferentGroupElement()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            var group = new CryptoGroup<BigInteger, int>(algebraStub.Object);

            var otherAlgebra = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            otherAlgebra.Setup(algebra => algebra.IsPotentialElement(It.IsAny<int>())).Returns(true);
            var element = new CryptoGroupElement<BigInteger, int>(3, otherAlgebra.Object);

            Assert.Throws<ArgumentException>(
                () => group.Negate(element)
            );
        }

        [Test]
        public void TestOrderCallsAlgebra()
        {
            var order = BigPrime.CreateWithoutChecks(29);

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);

            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);
            var result = group.Order;

            Assert.AreEqual(order, result);
        }

        [Test]
        public void TestOrderBitLengthCallsAlgebra()
        {
            int rawBitLength = 11;

            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.OrderBitLength).Returns(rawBitLength);

            var group = new CryptoGroup<BigInteger, int>(algebraStub.Object);

            var result = group.OrderLength;
            var expected = NumberLength.FromBitLength(algebraStub.Object.OrderBitLength);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestElementBitLengthCallsAlgebra()
        {
            int expectedRaw = 11;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.ElementBitLength).Returns(expectedRaw);

            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);

            var result = group.ElementLength;
            var expected = NumberLength.FromBitLength(expectedRaw);
            Assert.AreEqual(expected, result);

            algebraMock.Verify(alg => alg.ElementBitLength, Times.Once());
        }

        [Test]
        public void TestGenerateRandom()
        {
            var order = BigPrime.CreateWithoutChecks(1021);

            var expectedIndex = new BigInteger(19);
            int expectedRaw = 7;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.GenerateRandomElement(It.IsAny<RandomNumberGenerator>())).Returns((expectedIndex, expectedRaw));

            var expectedElement = new CryptoGroupElement<BigInteger, int>(expectedRaw, algebraMock.Object);

            var groupMock = new CryptoGroup<BigInteger, int>(algebraMock.Object);

            var rngMock = new Mock<RandomNumberGenerator>();

            var result = groupMock.GenerateRandom(rngMock.Object);
            var resultIndex = result.Item1;
            var resultElement = result.Item2;
            Assert.AreEqual(expectedIndex, resultIndex);
            Assert.AreEqual(expectedElement, resultElement);
        }

        [Test]
        public void TestGeneratorAccessor()
        {
            var expectedRaw = 3;
            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.IsSafeElement(It.Is<int>(x => x == expectedRaw))).Returns(true);
            algebraMock.Setup(alg => alg.Generator).Returns(expectedRaw);

            var expected = new CryptoGroupElement<BigInteger, int>(expectedRaw, algebraMock.Object);
            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);
            Assert.AreEqual(expected, group.Generator);
        }

        [Test]
        public void TestConstructor()
        {
            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            var group = new CryptoGroup<BigInteger, int>(algebraMock.Object);
            Assert.AreSame(algebraMock.Object, group.Algebra);
        }

    }

}
