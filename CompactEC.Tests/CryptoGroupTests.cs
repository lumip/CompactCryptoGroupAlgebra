using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

using CompactEC.CryptoAlgebra;
using System.Numerics;

namespace CompactEC.Tests.CryptoAlgebra
{
    interface CryptoGroupProtectedMembers
    {
        ICryptoGroupElement CreateGroupElement(int value);
        ICryptoGroupElement CreateGroupElement(byte[] buffer);
    }

    public class CryptoGroupFake : CryptoGroup<int>
    {
        public CryptoGroupFake(CryptoGroupAlgebra<int> algebra) : base(algebra)
        { }

        protected override ICryptoGroupElement CreateGroupElement(int value)
        {
            throw new NotImplementedException();
        }

        protected override ICryptoGroupElement CreateGroupElement(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class CrptoGroupTests
    {
        [TestMethod]
        public void TestConstructorRejectsNullAlgebra()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new CryptoGroupFake(null)
            );
        }

        [TestMethod]
        public void TestSpecificAddCallsAlgebraAndWrapsAsElement()
        {
            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.Add(2, 6)).Returns(8);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            var leftStub = new CryptoGroupElement<int>(2, algebraMock.Object);
            var rightStub = new CryptoGroupElement<int>(6, algebraMock.Object);
            var resultStub = new CryptoGroupElement<int>(8, algebraMock.Object);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object) { CallBase = true };
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(resultStub);

            Assert.AreSame(resultStub, groupMock.Object.Add(leftStub, rightStub));
            algebraMock.Verify(algebra => algebra.Add(2, 6), Times.Once());
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Verify(group => group.CreateGroupElement(It.Is<int>(i => i == 8)), Times.Once());
        }

        [TestMethod]
        public void TestAddRejectsNullArgumentLeft()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            var otherElementStub = new CryptoGroupElement<int>(3, algebraStub.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => groupMock.Add(null, otherElementStub)
            );
        }

        [TestMethod]
        public void TestAddRejectsNullArgumentRight()
        {
            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraMock.Object);
            var otherElementStub = new CryptoGroupElement<int>(3, algebraMock.Object);

            Assert.ThrowsException<ArgumentNullException>(
                () => groupMock.Add(otherElementStub, null)
            );
        }

        [TestMethod]
        public void TestFromBytesWrapsAsElement()
        {
            byte[] inputBuffer = new byte[0];

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            var resultStub = new Mock<CryptoGroupElement<int>>(MockBehavior.Strict, 0, algebraMock.Object);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Strict, algebraMock.Object) { CallBase = true };
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<byte[]>()))
                .Returns(resultStub.Object);

            Assert.AreSame(resultStub.Object, groupMock.Object.FromBytes(inputBuffer));
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Verify(group => group.CreateGroupElement(It.Is<byte[]>(b => b == inputBuffer)), Times.Once());
        }

        [TestMethod]
        public void TestGenerateCallsAlgebraAndWrapsElement()
        {
            var index = new BigInteger(7);
            int expectedRaw = 3;
            int generator = 2;
            var order = new BigInteger(16);
            int orderBitlength = 5;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.Generator).Returns(generator);
            algebraMock.Setup(algebra => algebra.Order).Returns(order);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(algebra => algebra.MultiplyScalarUnsafe(It.IsAny<int>(), It.IsAny<BigInteger>(), It.IsAny<int>()))
                .Returns(expectedRaw);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object) { CallBase = true };
            var expectedStub = new CryptoGroupElement<int>(expectedRaw, algebraMock.Object);

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expectedStub);

            Assert.AreSame(expectedStub, groupMock.Object.Generate(index));

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().Verify(
                algebra => algebra.MultiplyScalarUnsafe(
                    It.Is<int>(x => x == generator), It.Is<BigInteger>(x => x == index), It.Is<int>(x => x == orderBitlength)
                ),
                Times.Once()
            );
            groupMock.Protected().As<CryptoGroupProtectedMembers>().Verify(
                group => group.CreateGroupElement(It.Is<int>(i => i == expectedRaw)),
                Times.Once()
            );
        }

        [TestMethod]
        public void TestSpecificMultiplyScalarCallsAlgebraAndWrapsElement()
        {
            var k = new BigInteger(7);
            int expectedRaw = 3;
            int elementRaw = 8;
            var order = new BigInteger(16);
            int orderBitlength = 5;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.Order).Returns(order);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(algebra => algebra.MultiplyScalarUnsafe(It.IsAny<int>(), It.IsAny<BigInteger>(), It.IsAny<int>()))
                .Returns(expectedRaw);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object) { CallBase = true };
            var expectedStub = new CryptoGroupElement<int>(expectedRaw, algebraMock.Object);
            var elementStub = new CryptoGroupElement<int>(elementRaw, algebraMock.Object);

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expectedStub);

            Assert.AreSame(expectedStub, groupMock.Object.MultiplyScalar(elementStub, k));

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().Verify(
                algebra => algebra.MultiplyScalarUnsafe(
                    It.Is<int>(x => x == elementRaw), It.Is<BigInteger>(x => x == k), It.Is<int>(x => x == orderBitlength)
                ),
                Times.Once()
            );
            groupMock.Protected().As<CryptoGroupProtectedMembers>().Verify(
                group => group.CreateGroupElement(It.Is<int>(i => i == expectedRaw)),
                Times.Once()
            );
        }

        [TestMethod]
        public void TestMultiplyScalarRejectsNullElement()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            Assert.ThrowsException<ArgumentNullException>(
                () => groupMock.MultiplyScalar(null, new BigInteger(1))
            );
        }

        [TestMethod]
        public void TestSpecificNegateCallsAlgebraAndWrapsElement()
        {
            int expectedRaw = 3;
            int elementRaw = 8;
            var order = new BigInteger(16);
            int orderBitlength = 5;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Loose) { CallBase = true };
            algebraMock.Setup(algebra => algebra.Order).Returns(order);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(algebra => algebra.MultiplyScalarUnsafe(It.IsAny<int>(), It.IsAny<BigInteger>(), It.IsAny<int>()))
                .Returns(expectedRaw);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object) { CallBase = true };
            var expectedStub = new CryptoGroupElement<int>(expectedRaw, algebraMock.Object);
            var elementStub = new CryptoGroupElement<int>(elementRaw, algebraMock.Object);

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expectedStub);

            Assert.AreSame(expectedStub, groupMock.Object.Negate(elementStub));

            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().Verify(
                algebra => algebra.MultiplyScalarUnsafe(
                    It.Is<int>(x => x == elementRaw), It.Is<BigInteger>(x => x == order - 1), It.Is<int>(x => x == orderBitlength)
                ),
                Times.Once()
            );
            groupMock.Protected().As<CryptoGroupProtectedMembers>().Verify(
                group => group.CreateGroupElement(It.Is<int>(i => i == expectedRaw)),
                Times.Once()
            );
        }

        [TestMethod]
        public void TestNegateRejectsNullElement()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            Assert.ThrowsException<ArgumentNullException>(
                () => groupMock.Negate(null)
            );
        }

        [TestMethod]
        public void TestAddRejectsDifferentGroupElementLeft()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            var elementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);
            var otherElementStub = new CryptoGroupElement<int>(3, algebraStub.Object);

            Assert.ThrowsException<ArgumentException>(
                () => groupMock.Add(elementStub.Object, otherElementStub)
            );
        }

        [TestMethod]
        public void TestAddRejectsDifferentGroupElementRight()
        {
            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            var elementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);
            var otherElementStub = new CryptoGroupElement<int>(3, algebraStub.Object);

            Assert.ThrowsException<ArgumentException>(
                () => groupMock.Add(otherElementStub, elementStub.Object)
            );
        }

        [TestMethod]
        public void TestMultiplyScalarRejectsDifferentGroupElement()
        {
            var algebraStub = new CryptoGroupAlgebraFake();
            var groupMock = new CryptoGroupFake(algebraStub);
            var elementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            Assert.ThrowsException<ArgumentException>(
                () => groupMock.MultiplyScalar(elementStub.Object, new BigInteger(8))
            );
        }

        [TestMethod]
        public void TestNegateRejectsDifferentGroupElement()
        {
            var algebraStub = new CryptoGroupAlgebraFake();
            var groupMock = new CryptoGroupFake(algebraStub);
            var elementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            Assert.ThrowsException<ArgumentException>(
                () => groupMock.Negate(elementStub.Object)
            );
        }

        [TestMethod]
        public void TestOrderCallsAlgebra()
        {
            var order = new BigInteger(29);

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);

            var groupMock = new CryptoGroupFake(algebraMock.Object);
            var result = groupMock.Order;

            Assert.AreEqual(order, result);
        }

        [TestMethod]
        public void TestOrderBitLengthCallsAlgebra()
        {
            int rawBitLength = 11;
            var order = new BigInteger(1 << (rawBitLength - 1));

            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.Order).Returns(order);

            var groupMock = new CryptoGroupFake(algebraStub.Object);

            int result = groupMock.OrderBitLength;
            int expected = algebraStub.Object.OrderBitlength;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(8, 1)]
        [DataRow(9, 2)]
        public void TestOrderByteLengthCallsAlgebra(int bitLength, int expectedByteLength)
        {
            var order = new BigInteger(1 << (bitLength - 1));

            var algebraStub = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.Order).Returns(order);

            var groupMock = new CryptoGroupFake(algebraStub.Object);

            int result = groupMock.OrderByteLength;
            Assert.AreEqual(expectedByteLength, result);
        }

        [TestMethod]
        public void TestElementBitLengthCallsAlgebra()
        {
            int expected = 11;

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.GroupElementBitlength).Returns(expected);

            var groupMock = new CryptoGroupFake(algebraMock.Object);

            int result = groupMock.ElementBitLength;
            Assert.AreEqual(expected, result);

            algebraMock.Verify(alg => alg.GroupElementBitlength, Times.Once());
        }

        [TestMethod]
        [DataRow(8, 1)]
        [DataRow(9, 2)]
        public void TestElementByteLengthCallsAlgebra(int bitLength, int expectedByteLength)
        {
            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.GroupElementBitlength).Returns(bitLength);

            var groupMock = new CryptoGroupFake(algebraMock.Object);

            int result = groupMock.ElementByteLength;
            Assert.AreEqual(expectedByteLength, result);

            algebraMock.Verify(alg => alg.GroupElementBitlength, Times.Once());
        }

    }
}
