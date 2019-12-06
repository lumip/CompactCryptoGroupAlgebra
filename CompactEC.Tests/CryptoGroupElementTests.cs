using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using CompactEC;

namespace CompactEC.Tests.CryptoAlgebra
{
    [TestClass]
    public class CryptoGroupElementTests
    {

        [TestMethod]
        public void TestConstructorRejectsInvalidValue()
        {
            int element = -3;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(false);
            
            Assert.ThrowsException<ArgumentException>(
                () => new CryptoGroupElement<int>(element, algebraMock.Object)
            );
            algebraMock.Verify(alg => alg.IsValid(It.Is<int>(x => x == element)), Times.Once());
        }

        [TestMethod]
        public void TestConstructorRejectsNullAlgebra()
        {
            int element = -3;

            Assert.ThrowsException<ArgumentNullException>(
                () => new CryptoGroupElement<int>(element, null)
            );
        }

        [TestMethod]
        public void TestConstructorSetsValueCorrectly()
        {
            int elementRaw = -3;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(elementRaw, algebraMock.Object);

            Assert.AreEqual(elementRaw, element.Value);
            algebraMock.Verify(alg => alg.IsValid(It.Is<int>(x => x == elementRaw)), Times.Once());
        }

        [TestMethod]
        public void TestConstructorFromBytesCorrect()
        {
            byte[] buffer = new byte[0];
            int expectedValue = 9;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.FromBytes(It.IsAny<byte[]>())).Returns(expectedValue);

            var element = new CryptoGroupElement<int>(buffer, algebraMock.Object);

            Assert.AreEqual(expectedValue, element.Value);
            algebraMock.Verify(alg => alg.FromBytes(It.Is<byte[]>(x => x == buffer)));
        }

        [TestMethod]
        public void TestConstructorFromBytesRejectsNullAlgebra()
        {
            byte[] buffer = new byte[0];

            Assert.ThrowsException<ArgumentNullException>(
                () => new CryptoGroupElement<int>(buffer, null)
            );
        }

        [TestMethod]
        public void TestConstructorFromBytesRejectsInvalidValue()
        {
            byte[] buffer = new byte[0];

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.FromBytes(It.IsAny<byte[]>())).Returns(0);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(false);

            Assert.ThrowsException<ArgumentException>(
                () => new CryptoGroupElement<int>(buffer, algebraStub.Object)
            );
        }

        [TestMethod]
        public void TestAddRejectsOtherCryptoGroupElementSubclasses()
        {
            var otherElementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(0, algebraStub.Object);

            Assert.ThrowsException<ArgumentException>(
                () => element.Add(otherElementStub.Object)
            );
        }

        [TestMethod]
        public void TestAddRejectsNull()
        {
            ICryptoGroupElement otherElement = null;

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>();
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(0, algebraStub.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => element.Add(otherElement)
            );
        }

        [TestMethod]
        public void TestAddRejectsElementFromDifferentGroup()
        {
            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            var otherElement = new CryptoGroupElement<int>(0, otherAlgebraStub.Object);

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(0, algebraStub.Object);

            Assert.ThrowsException<ArgumentException>(
                () => element.Add(otherElement)
            );
        }

        [TestMethod]
        public void TestAdd()
        {
            int otherValue = 3;
            int value = 7;
            int expected = value + otherValue;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);

            var element = new CryptoGroupElement<int>(value, algebraMock.Object);
            var otherElement = new CryptoGroupElement<int>(otherValue, algebraMock.Object);

            element.Add(otherElement);

            Assert.AreEqual(expected, element.Value);
            Assert.AreEqual(otherValue, otherElement.Value);
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == value), It.Is<int>(x => x == otherValue)), Times.Once());
        }

        [TestMethod]
        public void TestMultiplyScalar()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;
            var order = new BigInteger(10);

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expected);

            var element = new CryptoGroupElement<int>(value, algebraMock.Object);

            element.MultiplyScalar(scalar);

            Assert.AreEqual(expected, element.Value);
            algebraMock.Verify(alg => alg.MultiplyScalar(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar)), Times.Once());
        }

        [TestMethod]
        public void TestNegate()
        {
            int value = 3;
            int expected = -value;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var element = new CryptoGroupElement<int>(value, algebraMock.Object);

            element.Negate();

            Assert.AreEqual(expected, element.Value);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
        }

        [TestMethod]
        public void TestEqualsFalseForOtherCryptoGroupElementSubclasses()
        {
            var otherElementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(0, algebraStub.Object);
            bool result = element.Equals(otherElementStub.Object);
            
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsFalseForNull()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(0, algebraStub.Object);
            bool result = element.Equals(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsFalseForDifferentValues()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var otherElement = new CryptoGroupElement<int>(3, algebraStub.Object);
            var element = new CryptoGroupElement<int>(8, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsFalseForDifferentAlgebras()
        {
            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            var otherElement = new CryptoGroupElement<int>(0, otherAlgebraStub.Object);

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(0, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsTrue()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var otherElement = new CryptoGroupElement<int>(5, algebraStub.Object);
            var element = new CryptoGroupElement<int>(5, algebraStub.Object);

            bool result = element.Equals(otherElement);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestOperatorPlus()
        {
            int otherValue = 3;
            int value = 7;
            int expected = value + otherValue;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);

            var otherElement = new CryptoGroupElement<int>(otherValue, algebraMock.Object);
            var element = new CryptoGroupElement<int>(value, algebraMock.Object);

            var result = element + otherElement;

            Assert.AreEqual(otherValue, otherElement.Value);
            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == value), It.Is<int>(x => x == otherValue)), Times.Once());
        }

        [TestMethod]
        public void TestOperatorUnaryMinus()
        {
            int value = 3;
            int expected = -value;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var element = new CryptoGroupElement<int>(value, algebraMock.Object);

            var result = -element;

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
        }

        [TestMethod]
        public void TestOperatorMinus()
        {
            int otherValue = 7;
            int value = 3;
            int expected = otherValue - value;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var otherElement = new CryptoGroupElement<int>(otherValue, algebraMock.Object);
            var element = new CryptoGroupElement<int>(value, algebraMock.Object);

            var result = otherElement - element;

            Assert.AreEqual(otherValue, otherElement.Value);
            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == -value), It.Is<int>(x => x == otherValue)), Times.Once());
        }
        
        [TestMethod]
        public void TestOperatorMulitplyLeft()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;
            var order = new BigInteger(10);

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expected);

            var element = new CryptoGroupElement<int>(value, algebraMock.Object);

            var result = element * scalar;

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.MultiplyScalar(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar)), Times.Once());
        }

        [TestMethod]
        public void TestOperatorMultiplyRight()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;
            var order = new BigInteger(10);

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expected);

            var element = new CryptoGroupElement<int>(value, algebraMock.Object);

            var result = scalar * element;

            Assert.AreEqual(value, element.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.MultiplyScalar(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar)), Times.Once());
        }

        [TestMethod]
        public void TestCloneResultsInEqualButNotSameElement()
        {
            int value = 3;

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var element = new CryptoGroupElement<int>(value, algebraStub.Object);
            var clone = element.Clone();

            Assert.AreEqual(element, clone);
            Assert.AreNotSame(element, clone);
        }

        [TestMethod]
        public void TestToBytesCallsAlgebra()
        {
            var value = 3;
            var expected = new byte[0];

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.ToBytes(It.IsAny<int>())).Returns(expected);

            var element = new CryptoGroupElement<int>(value, algebraMock.Object);
            var result = element.ToBytes();

            algebraMock.Verify(alg => alg.ToBytes(It.Is<int>(x => x == value)), Times.Once());
        }
    }
}
