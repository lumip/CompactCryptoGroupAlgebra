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

    [TestClass]
    public class CrptoGroupTests
    {
        [TestMethod]
        public void TestAddCallsAlgebraAndWrapsAsElement()
        {
            var algMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algMock.Setup(algebra => algebra.Add(2, 6)).Returns(8);
            algMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algMock.Object) { CallBase = true };

            var leftStub = new Mock<CryptoGroupElement<int>>(MockBehavior.Strict, 2, algMock.Object) { CallBase = true };
            var rightStub = new Mock<CryptoGroupElement<int>>(MockBehavior.Strict, 6, algMock.Object) { CallBase = true };
            var resultStub = new Mock<CryptoGroupElement<int>>(MockBehavior.Strict, 8, algMock.Object) { CallBase = true };

            groupMock.Protected().As<CryptoGroupProtectedMembers>().Setup(group => group.CreateGroupElement(It.Is<int>(i => i == 8))).Returns(resultStub.Object);

            Assert.AreSame(resultStub.Object, groupMock.Object.Add(leftStub.Object, rightStub.Object));
            algMock.Verify(algebra => algebra.Add(2, 6), Times.Once());
            groupMock.Protected().As<CryptoGroupProtectedMembers>().Verify(group => group.CreateGroupElement(It.Is<int>(i => i == 8)), Times.Once());
        }

        [TestMethod]
        public void TestFromBytesWrapsAsElement()
        {
            var algMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            byte[] inputBuffer = new byte[0];
            var resultStub = new Mock<CryptoGroupElement<int>>(MockBehavior.Strict, 0, algMock.Object);
            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Strict, algMock.Object) { CallBase = true };
            groupMock.Protected().As<CryptoGroupProtectedMembers>().Setup(group => group.CreateGroupElement(It.Is<byte[]>(b => b == inputBuffer))).Returns(resultStub.Object);

            Assert.AreSame(resultStub.Object, groupMock.Object.FromBytes(inputBuffer));
            groupMock.Protected().As<CryptoGroupProtectedMembers>().Verify(group => group.CreateGroupElement(It.Is<byte[]>(b => b == inputBuffer)), Times.Once());
        }

        //[TestMethod]
        //public void TestGenerateCallsAlgebraAndWrapsElement()
        //{
        //    var index = new BigInteger(7);
        //    var expectedRaw = 3;

        //    var algMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict);
        //    algMock.Setup(algebra => algebra.GenerateElement(It.Is<BigInteger>(i => i == index))).Returns(expectedRaw);
        //    algMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

        //    var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algMock.Object) { CallBase = true };
        //    var expectedStub = new Mock<CryptoGroupElementImplementation<int>>(MockBehavior.Strict, expectedRaw, algMock.Object) { CallBase = true };

        //    groupMock.Protected().As<CryptoGroupProtectedMembers>().Setup(group => group.CreateGroupElement(It.Is<int>(i => i == expectedRaw))).Returns(expectedStub.Object);

        //    Assert.AreSame(expectedStub.Object, groupMock.Object.Generate(It.Is<BigInteger>(i => i == index)));
        //    algMock.Verify(algebra => algebra.GenerateElement(index), Times.Once());
        //    groupMock.Protected().As<CryptoGroupProtectedMembers>().Verify(group => group.CreateGroupElement(It.Is<int>(i => i == expectedRaw)), Times.Once());
        //}
    }
}
