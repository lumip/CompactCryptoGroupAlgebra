﻿using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

using CompactEC.CryptoAlgebra;

namespace CompactEC.Tests.CryptoAlgebra
{
    interface CryptoAlgebraProtectedMembers
    {
        int Multiplex(BigInteger selection, int left, int right);
        int MultiplyScalarUnsafe(int e, BigInteger k, int factorBitlength);
    }

    [TestClass]
    public class CryptoGroupAlgebraTests
    {
        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(15, 4)]
        [DataRow(16, 5)]
        public void TestBitLength(int valueInt, int expectedBitlength)
        {
            var value = new BigInteger(valueInt);
            var result = CryptoGroupAlgebra<int>.GetBitLength(value);
            Assert.AreEqual(expectedBitlength, result);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(15, 4)]
        [DataRow(16, 5)]
        public void TestOrderBitLength(int orderInt, int expectedBitlength)
        {
            var order = new BigInteger(orderInt);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Loose) { CallBase = true };
            algebraMock.Setup(alg => alg.Order).Returns(order);

            var result = algebraMock.Object.OrderBitlength;
            Assert.AreEqual(expectedBitlength, result);
        }
        
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(12)]
        public void TestMultiplyScalar(int scalarInt)
        {
            var k = new BigInteger(scalarInt);
            var order = new BigInteger(5);
            int element = 3;
            int expected = 3 * (scalarInt % 5);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Setup(alg => alg.NeutralElement).Returns(0);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns((int x, int y) => x + y);

            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsZero), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => x);

            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsOne), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => y);

            var result = algebraMock.Object.MultiplyScalar(element, k);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMultiplyScalarRejectsNegativeScalar()
        {
            int element = 5;
            var index = BigInteger.MinusOne;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.MultiplyScalar(element, index)
            );
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(4)]
        [DataRow(7)]
        public void TestMultiplyScalarWithSmallFactor(int factorInt)
        {
            int element = 5;
            var k = new BigInteger(factorInt);
            int factorBitlength = 3;
            int expected = 5 * factorInt;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.NeutralElement).Returns(0);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns((int x, int y) => x + y);

            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsZero), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => x);

            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsOne), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => y);

            var result = algebraMock.Object.MultiplyScalar(element, k, factorBitlength);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsNegativeScalars()
        {
            int element = 5;
            var index = BigInteger.MinusOne;
            int factorBitlength = 3;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.MultiplyScalar(element, index, factorBitlength)
            );
        }

        [TestMethod]
        [DataRow(8)]
        [DataRow(9)]
        [DataRow(123)]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsLargerFactors(int factorInt)
        {
            var k = new BigInteger(factorInt);
            int element = 5;
            int factorBitlength = 3;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.MultiplyScalar(element, k, factorBitlength)
            );
        }


        [TestMethod]
        public void TestGenerateElement()
        {
            var order = new BigInteger(7);
            int orderBitlength = 3;
            int generator = 2;
            var index = new BigInteger(3);
            int expected = 3 * generator;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Setup(alg => alg.Generator).Returns(generator);

            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == index), It.Is<int>(i => i == orderBitlength))
                )
                .Returns(expected);

            var result = algebraMock.Object.GenerateElement(index);
            Assert.AreEqual(expected, result);
            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == index), It.Is<int>(i => i == orderBitlength)),
                    Times.Once()
                );
        }

        [TestMethod]
        public void TestGenerateElementRejectsNegativeIndex()
        {
            var index = BigInteger.MinusOne;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.GenerateElement(index)
            );
        }

        [TestMethod]
        public void TestNegate()
        {
            var order = new BigInteger(17);
            int element = 7;
            int expected = -7;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == order - 1), It.Is<int>(i => i == 5))
                )
                .Returns(expected);

            int result = algebraMock.Object.Negate(element);
            Assert.AreEqual(expected, result);
            algebraMock.Protected().As<CryptoAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == order - 1), It.Is<int>(i => i == 5)),
                    Times.Once()
                );
        }

    }
}
