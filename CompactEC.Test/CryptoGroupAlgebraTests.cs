using System;
using System.Numerics;

using NUnit.Framework;
using Moq;
using Moq.Protected;

using CompactEC;

namespace CompactEC.Tests.CryptoAlgebra
{
    interface CryptoGroupAlgebraProtectedMembers
    {
        int Multiplex(BigInteger selection, int left, int right);
        int MultiplyScalarUnsafe(int e, BigInteger k, int factorBitLength);
    }

    [TestFixture]
    public class CryptoGroupAlgebraTests
    {
        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(15, 4)]
        [TestCase(16, 5)]
        public void TestBitLength(int valueInt, int expectedBitLength)
        {
            var value = new BigInteger(valueInt);
            var result = CryptoGroupAlgebra<int>.GetBitLength(value);

            Assert.AreEqual(expectedBitLength, result);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(15, 4)]
        [TestCase(16, 5)]
        public void TestOrderBitLength(int orderInt, int expectedBitLength)
        {
            var order = new BigInteger(orderInt);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);

            var result = algebraMock.Object.OrderBitLength;

            Assert.AreEqual(expectedBitLength, result);
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(12)]
        public void TestMultiplyScalar(int scalarInt)
        {
            // this also tests the implementation in MultiplyScalarUnsafe

            var k = new BigInteger(scalarInt);
            var order = new BigInteger(5);
            int element = 3;
            int expected = 3 * (scalarInt % 5);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Setup(alg => alg.NeutralElement).Returns(0);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns((int x, int y) => x + y);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsZero), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => x);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsOne), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => y);

            var result = algebraMock.Object.MultiplyScalar(element, k);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestMultiplyScalarRejectsNegativeScalar()
        {
            int element = 5;
            var index = BigInteger.MinusOne;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.MultiplyScalar(element, index)
            );
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(7)]
        public void TestMultiplyScalarWithSmallFactor(int factorInt)
        {
            int element = 5;
            var k = new BigInteger(factorInt);
            int factorBitLength = 3;
            int expected = 5 * factorInt;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.NeutralElement).Returns(0);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns((int x, int y) => x + y);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsZero), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => x);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.Multiplex(
                    It.Is<BigInteger>(s => s.IsOne), It.IsAny<int>(), It.IsAny<int>())
                )
                .Returns((BigInteger s, int x, int y) => y);

            var result = algebraMock.Object.MultiplyScalar(element, k, factorBitLength);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsNegativeScalars()
        {
            int element = 5;
            var index = BigInteger.MinusOne;
            int factorBitLength = 3;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.MultiplyScalar(element, index, factorBitLength)
            );
        }

        [Test]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(123)]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsLargerFactors(int factorInt)
        {
            var k = new BigInteger(factorInt);
            int element = 5;
            int factorBitLength = 3;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.MultiplyScalar(element, k, factorBitLength)
            );
        }


        [Test]
        public void TestGenerateElement()
        {
            var order = new BigInteger(7);
            int orderBitLength = 3;
            int generator = 2;
            var index = new BigInteger(3);
            int expected = 3 * generator;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Setup(alg => alg.Generator).Returns(generator);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == index), It.Is<int>(i => i == orderBitLength))
                )
                .Returns(expected);

            var result = algebraMock.Object.GenerateElement(index);

            Assert.AreEqual(expected, result);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == index), It.Is<int>(i => i == orderBitLength)),
                    Times.Once()
                );
        }

        [Test]
        public void TestGenerateElementRejectsNegativeIndex()
        {
            var index = BigInteger.MinusOne;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.Generator).Returns(1);

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => algebraMock.Object.GenerateElement(index)
            );
        }

        [Test]
        public void TestNegate()
        {
            var order = new BigInteger(17);
            int element = 7;
            int expected = -7;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>() { CallBase = true };
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == order - 1), It.Is<int>(i => i == 5))
                )
                .Returns(expected);

            int result = algebraMock.Object.Negate(element);

            Assert.AreEqual(expected, result);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnsafe(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == order - 1), It.Is<int>(i => i == 5)),
                    Times.Once()
                );
        }

    }
}
