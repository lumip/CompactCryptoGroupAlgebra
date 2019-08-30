using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

using CompactEC.CryptoAlgebra;

namespace CompactEC.Tests.CryptoAlgebra
{
    public class CryptoGroupElementFake : CryptoGroupElement<int>
    {
        public CryptoGroupElementFake(int value, CryptoGroupAlgebra<int> algebra)
            : base(value, algebra)
        { }

        public override ICryptoGroupElement Clone()
        {
            throw new NotImplementedException();
        }

        public override byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class CryptoGroupElementTests
    {

        [TestMethod]
        public void TestConstructorRejectsInvalidValue()
        {
            int element = -3;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(false);
            
            Assert.ThrowsException<ArgumentException>(
                () => new CryptoGroupElementFake(element, algebraMock.Object)
            );
            algebraMock.Verify(alg => alg.IsValid(It.Is<int>(x => x == element)), Times.Once());
        }

        [TestMethod]
        public void TestConstructorRejectsNullAlgebra()
        {
            int element = -3;

            Assert.ThrowsException<ArgumentNullException>(
                () => new CryptoGroupElementFake(element, null)
            );
        }

        [TestMethod]
        public void TestConstructorSetsValueCorrectly()
        {
            int element = -3;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElementFake(element, algebraMock.Object);

            Assert.AreEqual(element, elementStub.Value);
            algebraMock.Verify(alg => alg.IsValid(It.Is<int>(x => x == element)), Times.Once());
        }

        [TestMethod]
        public void TestAddRejectsOtherCryptoGroupElementSubclasses()
        {
            var otherElementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElementFake(0, algebraStub.Object);

            Assert.ThrowsException<ArgumentException>(
                () => elementStub.Add(otherElementStub.Object)
            );
        }

        [TestMethod]
        public void TestAddRejectsNull()
        {
            ICryptoGroupElement otherElement = null;

            var algebraStub = new Mock<CryptoGroupAlgebra<int>>();
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElementFake(0, algebraStub.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => elementStub.Add(otherElement)
            );
        }

        [TestMethod]
        public void TestAddRejectsElementFromDifferentGroup()
        {
            var otherAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            var otherElementStub = new CryptoGroupElementFake(0, otherAlgebraStub.Object);

            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElementFake(0, algebraStub.Object);

            Assert.ThrowsException<ArgumentException>(
                () => elementStub.Add(otherElementStub)
            );
        }

        [TestMethod]
        public void TestAdd()
        {
            int otherValue = 3;
            int value = 7;
            int expected = value + otherValue;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);

            var elementStub = new CryptoGroupElementFake(value, algebraMock.Object);
            var otherElementStub = new CryptoGroupElementFake(otherValue, algebraMock.Object);

            elementStub.Add(otherElementStub);

            Assert.AreEqual(expected, elementStub.Value);
            Assert.AreEqual(otherValue, otherElementStub.Value);
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == value), It.Is<int>(x => x == otherValue)), Times.Once());
        }

        [TestMethod]
        public void TestMultiplyScalar()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;
            var order = new BigInteger(10);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().
                Setup(alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar), It.IsAny<int>())).Returns(expected);

            var elementStub = new CryptoGroupElementFake(value, algebraMock.Object);

            elementStub.MultiplyScalar(scalar);

            Assert.AreEqual(expected, elementStub.Value);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void TestNegate()
        {
            int value = 3;
            int expected = -value;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var elementStub = new CryptoGroupElementFake(value, algebraMock.Object);

            elementStub.Negate();

            Assert.AreEqual(expected, elementStub.Value);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
        }

        [TestMethod]
        public void TestEqualsFalseForOtherCryptoGroupElementSubclasses()
        {
            var otherElementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElementFake(0, algebraStub.Object);
            bool result = elementStub.Equals(otherElementStub.Object);
            
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsFalseForNull()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElementFake(0, algebraStub.Object);
            bool result = elementStub.Equals(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsFalseForDifferentValues()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var otherElementStub = new CryptoGroupElementFake(3, algebraStub.Object);
            var elementStub = new CryptoGroupElementFake(8, algebraStub.Object);

            bool result = elementStub.Equals(otherElementStub);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsFalseForDifferentAlgebras()
        {
            var otherAlgebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            var otherElementStub = new CryptoGroupElementFake(0, otherAlgebraStub.Object);

            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElementFake(0, algebraStub.Object);

            bool result = elementStub.Equals(otherElementStub);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestEqualsTrue()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var otherElementStub = new CryptoGroupElementFake(5, algebraStub.Object);
            var elementStub = new CryptoGroupElementFake(5, algebraStub.Object);

            bool result = elementStub.Equals(otherElementStub);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestOperatorPlus()
        {
            int otherValue = 3;
            int value = 7;
            int expected = value + otherValue;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);

            var otherElementStub = new CryptoGroupElementFake(otherValue, algebraMock.Object);
            var elementStub = new Mock<CryptoGroupElementFake>(MockBehavior.Strict, value, algebraMock.Object);
            elementStub.Setup(element => element.Clone()).Returns(new CryptoGroupElementFake(value, algebraMock.Object));

            var result = elementStub.Object + otherElementStub;

            Assert.AreEqual(otherValue, otherElementStub.Value);
            Assert.AreEqual(value, elementStub.Object.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Add(It.Is<int>(x => x == value), It.Is<int>(x => x == otherValue)), Times.Once());
        }

        [TestMethod]
        public void TestOperatorUnaryMinus()
        {
            int value = 3;
            int expected = -value;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var elementStub = new Mock<CryptoGroupElementFake>(MockBehavior.Strict, value, algebraMock.Object);
            elementStub.Setup(element => element.Clone()).Returns(new CryptoGroupElementFake(value, algebraMock.Object));

            var result = -elementStub.Object;

            Assert.AreEqual(value, elementStub.Object.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Verify(alg => alg.Negate(It.Is<int>(x => x == value)), Times.Once());
        }

        [TestMethod]
        public void TestOperatorMinus()
        {
            int otherValue = 7;
            int value = 3;
            int expected = otherValue - value;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Add(It.IsAny<int>(), It.IsAny<int>())).Returns(expected);
            algebraMock.Setup(alg => alg.Negate(It.IsAny<int>())).Returns((int x) => -x);

            var otherElementStub = new CryptoGroupElementFake(otherValue, algebraMock.Object);
            var elementStub = new Mock<CryptoGroupElementFake>(MockBehavior.Strict, value, algebraMock.Object);
            elementStub.Setup(element => element.Clone()).Returns(new CryptoGroupElementFake(value, algebraMock.Object));

            var result = otherElementStub - elementStub.Object;

            Assert.AreEqual(otherValue, otherElementStub.Value);
            Assert.AreEqual(value, elementStub.Object.Value);
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

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().
                Setup(alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar), It.IsAny<int>())).Returns(expected);

            var elementStub = new Mock<CryptoGroupElementFake>(MockBehavior.Strict, value, algebraMock.Object);
            elementStub.Setup(element => element.Clone()).Returns(new CryptoGroupElementFake(value, algebraMock.Object));

            var result = elementStub.Object * scalar;

            Assert.AreEqual(value, elementStub.Object.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void TestOperatorMultiplyRight()
        {
            int value = 3;
            var scalar = new BigInteger(7);
            int expected = value * (int)scalar;
            var order = new BigInteger(10);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().
                Setup(alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar), It.IsAny<int>())).Returns(expected);

            var elementStub = new Mock<CryptoGroupElementFake>(MockBehavior.Strict, value, algebraMock.Object);
            elementStub.Setup(element => element.Clone()).Returns(new CryptoGroupElementFake(value, algebraMock.Object));

            var result = scalar * elementStub.Object;

            Assert.AreEqual(value, elementStub.Object.Value);
            Assert.AreEqual(expected, result.Value);
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Verify(alg => alg.MultiplyScalarUnsafe(It.Is<int>(x => x == value), It.Is<BigInteger>(x => x == scalar), It.IsAny<int>()), Times.Once());
        }
    }
}
