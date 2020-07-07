using System;
using System.Numerics;
using System.Reflection;

using NUnit.Framework;
using Moq;
using Moq.Protected;

namespace CompactCryptoGroupAlgebra.Tests
{
    interface CryptoGroupAlgebraProtectedMembers
    {
        int MultiplyScalarUnchecked(int e, BigInteger k, int factorBitLength);
        bool IsElementDerived(int e);
    }

    [TestFixture]
    public class CryptoGroupAlgebraTests
    {

        [Test]
        [TestCase(13, 4)]
        public void TestOrderBitLength(int orderInt, int expectedBitLength)
        {
            var order = BigPrime.CreateWithoutChecks(orderInt);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generatorStub, order, cofactor, neutralElement, elementBitLength);
            var result = algebraMock.Object.OrderBitLength;

            Assert.AreEqual(expectedBitLength, result);
        }

        [Test]
        public void TestProperties()
        {
            var order = BigPrime.CreateWithoutChecks(13);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generatorStub, order, cofactor, neutralElement, elementBitLength);

            Assert.AreEqual(generatorStub, algebraMock.Object.Generator);
            Assert.AreEqual(cofactor, algebraMock.Object.Cofactor);
            Assert.AreEqual(neutralElement, algebraMock.Object.NeutralElement);
            Assert.AreEqual(elementBitLength, algebraMock.Object.ElementBitLength);
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
            var order = BigPrime.CreateWithoutChecks(5);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            int element = 3;
            int expected = 3 * (scalarInt % 5);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(generatorStub, order, cofactor, neutralElement, elementBitLength) { CallBase = true };
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns((int x, int y) => x + y);

            var result = algebraMock.Object.MultiplyScalar(element, k);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestMultiplyScalarRejectsNegativeScalar()
        {
            int element = 5;
            var index = BigInteger.MinusOne;
            var order = BigPrime.CreateWithoutChecks(5);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generatorStub, order, cofactor, neutralElement, elementBitLength);

            Assert.Throws<ArgumentOutOfRangeException>(
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

            var order = BigPrime.CreateWithoutChecks(11);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            int factorBitLength = 3;
            int expected = 5 * factorInt;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(generatorStub, order, cofactor, neutralElement, elementBitLength) { CallBase = true };
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns((int x, int y) => x + y);

            var result = algebraMock.Object.MultiplyScalar(element, k, factorBitLength);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestMultiplyScalarWithSmallFactorSizeRejectsNegativeScalars()
        {
            int element = 5;
            var index = BigInteger.MinusOne;
            int factorBitLength = 3;

            var order = BigPrime.CreateWithoutChecks(5);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;


            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generatorStub, order, cofactor, neutralElement, elementBitLength);

            Assert.Throws<ArgumentOutOfRangeException>(
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

            var order = BigPrime.CreateWithoutChecks(5);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generatorStub, order, cofactor, neutralElement, elementBitLength);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => algebraMock.Object.MultiplyScalar(element, k, factorBitLength)
            );
        }


        [Test]
        public void TestGenerateElement()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            int orderBitLength = 3;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;
            int generator = 2;
            var index = new BigInteger(3);
            int expected = 3 * generator;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, generator, order, cofactor, neutralElement, elementBitLength);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnchecked(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == index), It.Is<int>(i => i == orderBitLength))
                )
                .Returns(expected);

            var result = algebraMock.Object.GenerateElement(index);

            Assert.AreEqual(expected, result);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnchecked(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == index), It.Is<int>(i => i == orderBitLength)),
                    Times.Once()
                );
        }

        [Test]
        public void TestGenerateElementRejectsNegativeIndex()
        {
            var index = BigInteger.MinusOne;

            var order = BigPrime.CreateWithoutChecks(5);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(generatorStub, order, cofactor, neutralElement, elementBitLength) { CallBase = true };

            Assert.Throws<ArgumentOutOfRangeException>(
                () => algebraMock.Object.GenerateElement(index)
            );
        }

        [Test]
        public void TestNegate()
        {
            var order = BigPrime.CreateWithoutChecks(17);
            int element = 7;
            int expected = -7;

            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(generatorStub, order, cofactor, neutralElement, elementBitLength) { CallBase = true };
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(alg => alg.MultiplyScalarUnchecked(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == order - 1), It.Is<int>(i => i == 5))
                )
                .Returns(expected);

            int result = algebraMock.Object.Negate(element);

            Assert.AreEqual(expected, result);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnchecked(
                    It.IsAny<int>(), It.Is<BigInteger>(i => i == order - 1), It.Is<int>(i => i == 5)),
                    Times.Once()
                );
        }

        [Test]
        public void TestEqualsCallsSpecificEquals()
        {
            var order = BigPrime.CreateWithoutChecks(5);
            var generatorStub = 1;
            var cofactor = BigInteger.One;
            var neutralElement = 0;
            var elementBitLength = 8;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(generatorStub, order, cofactor, neutralElement, elementBitLength) { CallBase = true };
            algebraMock.Setup(alg => alg.Equals(It.IsAny<CryptoGroupAlgebra<int>>())).Returns(true);

            var otherAlgebraMock = new Mock<CryptoGroupAlgebra<int>>(generatorStub, order, cofactor, neutralElement, elementBitLength);

            Assert.IsTrue(algebraMock.Object.Equals((object)(otherAlgebraMock.Object)));

            algebraMock.Verify(
                alg => alg.Equals(It.Is<CryptoGroupAlgebra<int>>(x => x == otherAlgebraMock.Object)),
                Times.Once()
            );
        }

    }
}
