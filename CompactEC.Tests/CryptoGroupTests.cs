using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

using CompactEC.CryptoAlgebra;
using System.Numerics;
using System.Security.Cryptography;
using System.Linq;

namespace CompactEC.Tests.CryptoAlgebra
{
    interface CryptoGroupProtectedMembers
    {
        ICryptoGroupElement CreateGroupElement(int value);
        ICryptoGroupElement CreateGroupElement(byte[] buffer);
    }

    class CryptoGroupFake : CryptoGroup<int>
    {
        public CryptoGroupFake(ICryptoGroupAlgebra<int> algebra) : base(algebra)
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
            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.Add(2, 6)).Returns(8);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            var leftStub = new CryptoGroupElement<int>(2, algebraMock.Object);
            var rightStub = new CryptoGroupElement<int>(6, algebraMock.Object);
            var resultStub = new CryptoGroupElement<int>(8, algebraMock.Object);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object);
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
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
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

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            var resultStub = new Mock<CryptoGroupElement<int>>(MockBehavior.Strict, 0, algebraMock.Object);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Strict, algebraMock.Object);
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

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.GenerateElement(It.IsAny<BigInteger>())).Returns(expectedRaw);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object);
            var expectedStub = new CryptoGroupElement<int>(expectedRaw, algebraMock.Object);

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expectedStub);

            Assert.AreSame(expectedStub, groupMock.Object.Generate(index));

            algebraMock.Verify(
                algebra => algebra.GenerateElement(It.Is<BigInteger>(x => x == index)),
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

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.MultiplyScalar(It.IsAny<int>(), It.IsAny<BigInteger>())).Returns(expectedRaw);
            
            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object);
            var expectedStub = new CryptoGroupElement<int>(expectedRaw, algebraMock.Object);
            var elementStub = new CryptoGroupElement<int>(elementRaw, algebraMock.Object);

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expectedStub);

            Assert.AreSame(expectedStub, groupMock.Object.MultiplyScalar(elementStub, k));

            algebraMock.Verify(
                algebra => algebra.MultiplyScalar(It.Is<int>(x => x == elementRaw), It.Is<BigInteger>(x => x == k)),
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
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
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

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.Negate(It.IsAny<int>())).Returns(expectedRaw);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Loose, algebraMock.Object);
            var expectedStub = new CryptoGroupElement<int>(expectedRaw, algebraMock.Object);
            var elementStub = new CryptoGroupElement<int>(elementRaw, algebraMock.Object);

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expectedStub);

            Assert.AreSame(expectedStub, groupMock.Object.Negate(elementStub));

            algebraMock.Verify(
                algebra => algebra.Negate(It.Is<int>(x => x == elementRaw)),
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
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            Assert.ThrowsException<ArgumentNullException>(
                () => groupMock.Negate(null)
            );
        }

        [TestMethod]
        public void TestAddRejectsDifferentGroupElementLeft()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
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
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
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
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            var elementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            Assert.ThrowsException<ArgumentException>(
                () => groupMock.MultiplyScalar(elementStub.Object, new BigInteger(8))
            );
        }

        [TestMethod]
        public void TestNegateRejectsDifferentGroupElement()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            var groupMock = new CryptoGroupFake(algebraStub.Object);
            var elementStub = new Mock<ICryptoGroupElement>(MockBehavior.Strict);

            Assert.ThrowsException<ArgumentException>(
                () => groupMock.Negate(elementStub.Object)
            );
        }

        [TestMethod]
        public void TestOrderCallsAlgebra()
        {
            var order = new BigInteger(29);

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);

            var groupMock = new CryptoGroupFake(algebraMock.Object);
            var result = groupMock.Order;

            Assert.AreEqual(order, result);
        }

        [TestMethod]
        public void TestOrderBitLengthCallsAlgebra()
        {
            int rawBitLength = 11;

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.OrderBitLength).Returns(rawBitLength);

            var groupMock = new CryptoGroupFake(algebraStub.Object);

            int result = groupMock.OrderBitLength;
            int expected = algebraStub.Object.OrderBitLength;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(8, 1)]
        [DataRow(9, 2)]
        public void TestOrderByteLengthCallsAlgebra(int bitLength, int expectedByteLength)
        {
            var order = new BigInteger(1 << (bitLength - 1));

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.OrderBitLength).Returns(bitLength);

            var groupMock = new CryptoGroupFake(algebraStub.Object);

            int result = groupMock.OrderByteLength;
            Assert.AreEqual(expectedByteLength, result);
        }

        [TestMethod]
        public void TestElementBitLengthCallsAlgebra()
        {
            int expected = 11;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.ElementBitLength).Returns(expected);

            var groupMock = new CryptoGroupFake(algebraMock.Object);

            int result = groupMock.ElementBitLength;
            Assert.AreEqual(expected, result);

            algebraMock.Verify(alg => alg.ElementBitLength, Times.Once());
        }

        [TestMethod]
        [DataRow(8, 1)]
        [DataRow(9, 2)]
        public void TestElementByteLengthCallsAlgebra(int bitLength, int expectedByteLength)
        {
            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.ElementBitLength).Returns(bitLength);

            var groupMock = new CryptoGroupFake(algebraMock.Object);

            int result = groupMock.ElementByteLength;
            Assert.AreEqual(expectedByteLength, result);

            algebraMock.Verify(alg => alg.ElementBitLength, Times.Once());
        }

        [TestMethod]
        public void TestGenerateRandom()
        {
            var order = new BigInteger(1021);
            int orderByteLength = 2;

            var expected = new Mock<ICryptoGroupElement>();
            var expectedRaw = 7;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Setup(alg => alg.OrderBitLength).Returns(orderByteLength * 8);
            algebraMock.Setup(alg => alg.GenerateElement(It.IsAny<BigInteger>())).Returns(expectedRaw);
            
            var groupMock = new Mock<CryptoGroup<int>>(algebraMock.Object);
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expected.Object);

            var index = new BigInteger(301);
            byte[] rngResponse = index.ToByteArray();

            var rngMock = new Mock<RandomNumberGenerator>();
            rngMock
                .Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                .Callback(
                    new Action<byte[]>(
                        (buffer) => { Buffer.BlockCopy(rngResponse, 0, buffer, 0, orderByteLength); }
                    )
                );

            var result = groupMock.Object.GenerateRandom(rngMock.Object);
            var resultIndex = result.Item1;
            var resultElement = result.Item2;
            Assert.AreEqual(index, resultIndex);
            Assert.AreSame(expected.Object, resultElement);
            
            algebraMock.Verify(
                alg => alg.GenerateElement(It.Is<BigInteger>(x => x == index)),
                Times.Once()
            );

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Verify(group => group.CreateGroupElement(It.Is<int>(x => x == expectedRaw)),
                        Times.Once());

            rngMock.Verify(rng => rng.GetBytes(It.Is<byte[]>(x => x.Length == orderByteLength)), Times.Once());
        }

        [TestMethod]
        [DataRow(-3)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(1020)]
        [DataRow(1022)]
        public void TestGenerateRandomDoesNotSampleInvalidIndices(int invalidIndexRaw)
        {
            // tests that invalid indices returned by rng are skipped:
            // negative, 0, 1, order -1, order +1
            var order = new BigInteger(1021);
            int orderByteLength = 2;

            var expected = new Mock<ICryptoGroupElement>();
            var expectedRaw = 7;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Setup(alg => alg.OrderBitLength).Returns(orderByteLength * 8);
            algebraMock.Setup(alg => alg.GenerateElement(It.IsAny<BigInteger>())).Returns(expectedRaw);

            var groupMock = new Mock<CryptoGroup<int>>(algebraMock.Object);
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expected.Object);

            var invalidIndex = new BigInteger(invalidIndexRaw);
            byte[] invalidRngResponse = invalidIndex.ToByteArray();
            if (invalidRngResponse.Length < orderByteLength)
            {
                byte[] buffer = new byte[orderByteLength];
                Buffer.BlockCopy(invalidRngResponse, 0, buffer, 0, invalidRngResponse.Length);
                if (invalidIndex < 0)
                {
                    buffer[1] = 0xFF;
                }
                else
                {
                    buffer[1] = 0x00;
                }
                invalidRngResponse = buffer;
            }

            var validIndex = new BigInteger(301);
            byte[] validRngResponse = validIndex.ToByteArray();

            bool firstTime = true;
            var rngMock = new Mock<RandomNumberGenerator>();
            rngMock
                .Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                .Callback(
                    new Action<byte[]>(
                        (buffer) => {
                            if (firstTime)
                            {
                                Buffer.BlockCopy(invalidRngResponse, 0, buffer, 0, orderByteLength);
                                firstTime = false;
                            }
                            else
                            {
                                Buffer.BlockCopy(validRngResponse, 0, buffer, 0, orderByteLength);
                            }
                        }
                    )
                );

            var result = groupMock.Object.GenerateRandom(rngMock.Object);
            var resultIndex = result.Item1;
            var resultElement = result.Item2;
            Assert.AreEqual(validIndex, resultIndex);
            Assert.AreSame(expected.Object, resultElement);
            
            algebraMock.Verify(
                alg => alg.GenerateElement(It.Is<BigInteger>(x => x == validIndex)),
                Times.Once()
            );

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Verify(group => group.CreateGroupElement(It.Is<int>(x => x == expectedRaw)),
                        Times.Once());

            rngMock.Verify(rng => rng.GetBytes(It.Is<byte[]>(x => x.Length == orderByteLength)), Times.Exactly(2));
        }

    }
}
