using System;
using System.Numerics;

using NUnit.Framework;
using Moq;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class CryptoGroupElementTests
    {

        [Test]
        public void TestConstructorRejectsInvalidValue()
        {
            int element = -3;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(false);
            
            Assert.Throws<ArgumentException>(
                () => new CryptoGroupElement<BigInteger, int>(element, algebraMock.Object)
            );
            algebraMock.Verify(alg => alg.IsPotentialElement(It.Is<int>(x => x == element)), Times.Once());
        }

        [Test]
        public void TestConstructorSetsValueCorrectly()
        {
            int elementRaw = -3;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(elementRaw, algebraMock.Object);

            Assert.AreEqual(elementRaw, element.Value);
            algebraMock.Verify(alg => alg.IsPotentialElement(It.Is<int>(x => x == elementRaw)), Times.Once());
        }

        [Test]
        public void TestConstructorFromBytesCorrect()
        {
            byte[] buffer = new byte[0];
            int expectedValue = 9;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.FromBytes(It.IsAny<byte[]>())).Returns(expectedValue);

            var element = new CryptoGroupElement<BigInteger, int>(buffer, algebraMock.Object);

            Assert.AreEqual(expectedValue, element.Value);
            algebraMock.Verify(alg => alg.FromBytes(It.Is<byte[]>(x => x == buffer)));
        }

        [Test]
        public void TestConstructorFromBytesRejectsInvalidValue()
        {
            byte[] buffer = new byte[0];

            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.FromBytes(It.IsAny<byte[]>())).Returns(0);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(false);

            Assert.Throws<ArgumentException>(
                () => new CryptoGroupElement<BigInteger, int>(buffer, algebraStub.Object)
            );
        }

        [Test]
        public void TestAddRejectsElementFromDifferentGroup()
        {
            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            var otherElement = new CryptoGroupElement<BigInteger, int>(0, otherAlgebraStub.Object);

            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(0, algebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => element.Add(otherElement)
            );
        }

        [Test]
        public void TestAdd()
        {
            int otherValue = 3;
            int value = 7;
            int expected = value + otherValue;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);
            var otherElement = new CryptoGroupElement<BigInteger, int>(otherValue, algebraMock.Object);

            var result = element.Add(otherElement);
            var expectedElement = new CryptoGroupElement<BigInteger, int>(expected, algebraMock.Object);

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(otherValue, otherElement.Value);
            Assert.AreEqual(expectedElement, result);
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == value), It.Is<int>(x => x == otherValue)), Times.Once());
        }

        [Test]
        public void TestMultiplyScalar()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expected);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);

            var result = element.MultiplyScalar(scalar);
            var expectedElement = new CryptoGroupElement<BigInteger, int>(expected, algebraMock.Object);

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expectedElement, result);
            algebraMock.Verify(alg => alg.MultiplyScalar(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar)), Times.Once());
        }

        [Test]
        public void TestNegate()
        {
            int value = 3;
            int expected = -value;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);

            var result = element.Negate();
            var expectedElement = new CryptoGroupElement<BigInteger, int>(expected, algebraMock.Object);

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expectedElement, result);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(0, algebraStub.Object);
            bool result = element.Equals(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForDifferentValues()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var otherElement = new CryptoGroupElement<BigInteger, int>(3, algebraStub.Object);
            var element = new CryptoGroupElement<BigInteger, int>(8, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForDifferentAlgebras()
        {
            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            var otherElement = new CryptoGroupElement<BigInteger, int>(0, otherAlgebraStub.Object);

            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraStub.Setup(alg => alg.Equals(It.IsAny<ICryptoGroupAlgebra<BigInteger, int>>())).Returns(false);

            var element = new CryptoGroupElement<BigInteger, int>(0, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsTrueForEqualAlgebras()
        {
            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            var otherElement = new CryptoGroupElement<BigInteger, int>(0, otherAlgebraStub.Object);

            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraStub.Setup(alg => alg.Equals(It.IsAny<ICryptoGroupAlgebra<BigInteger, int>>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(0, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsTrue()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var otherElement = new CryptoGroupElement<BigInteger, int>(5, algebraStub.Object);
            var element = new CryptoGroupElement<BigInteger, int>(5, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsUnrelatedObject()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var otherElement = new object();
            var element = new CryptoGroupElement<BigInteger, int>(5, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsFalse(result);
        }

        [Test]
        public void TestOperatorPlus()
        {
            int otherValue = 3;
            int value = 7;
            int expected = value + otherValue;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);

            var otherElement = new CryptoGroupElement<BigInteger, int>(otherValue, algebraMock.Object);
            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);

            var result = element + otherElement;

            Assert.AreEqual(otherValue, otherElement.Value);
            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == value), It.Is<int>(x => x == otherValue)), Times.Once());
        }

        [Test]
        public void TestOperatorUnaryMinus()
        {
            int value = 3;
            int expected = -value;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);

            var result = -element;

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
        }

        [Test]
        public void TestOperatorMinus()
        {
            int otherValue = 7;
            int value = 3;
            int expected = otherValue - value;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var otherElement = new CryptoGroupElement<BigInteger, int>(otherValue, algebraMock.Object);
            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);

            var result = otherElement - element;

            Assert.AreEqual(otherValue, otherElement.Value);
            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == otherValue), It.Is<int>(x => x == -value)), Times.Once());
        }
        
        [Test]
        public void TestOperatorMultiplyLeft()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expected);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);

            var result = element * scalar;

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.MultiplyScalar(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar)), Times.Once());
        }

        [Test]
        public void TestOperatorMultiplyRight()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expected);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);

            var result = scalar * element;

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.MultiplyScalar(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar)), Times.Once());
        }

        [Test]
        public void TestCloneConstructor()
        {
            int value = 3;

            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraStub.Object);
            var clone = new CryptoGroupElement<BigInteger, int>(element);

            Assert.AreEqual(element, clone);
            Assert.AreNotSame(element, clone);
        }

        [Test]
        public void TestToBytesCallsAlgebra()
        {
            var value = 3;
            var expected = new byte[0];

            var algebraMock = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.ToBytes(It.IsAny<int>())).Returns(expected);

            var element = new CryptoGroupElement<BigInteger, int>(value, algebraMock.Object);
            element.ToBytes();

            algebraMock.Verify(alg => alg.ToBytes(It.Is<int>(x => x == value)), Times.Once());
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var otherElement = new CryptoGroupElement<BigInteger, int>(5, algebraStub.Object);
            var element = new CryptoGroupElement<BigInteger, int>(5, algebraStub.Object);

            Assert.AreEqual(element.GetHashCode(), otherElement.GetHashCode());
        }

        [Test]
        public void TestToString()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<BigInteger, int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsPotentialElement(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<BigInteger, int>(5, algebraStub.Object);

            var expected = "<CryptoGroupElement: 5>";
            Assert.AreEqual(expected, element.ToString());
        }
    }
}
