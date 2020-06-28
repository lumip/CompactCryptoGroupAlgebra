using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Linq;

using NUnit.Framework;
using Moq;
using Moq.Protected;

using CompactCryptoGroupAlgebra;

namespace CompactCryptoGroupAlgebra.Tests
{
    interface CryptoGroupProtectedMembers
    {
        CryptoGroupElement<int> CreateGroupElement(int value);
        CryptoGroupElement<int> CreateGroupElement(byte[] buffer);
    }

    // todo: remove this?
    class CryptoGroupFake : CryptoGroup<int>
    {
        public CryptoGroupFake(ICryptoGroupAlgebra<int> algebra) : base(algebra)
        { }

        protected override CryptoGroupElement<int> CreateGroupElement(int value)
        {
            throw new NotImplementedException();
        }

        protected override CryptoGroupElement<int> CreateGroupElement(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class CryptoGroupTests
    {

        [Test]
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

        [Test]
        public void TestFromBytesWrapsAsElement()
        {
            byte[] inputBuffer = new byte[0];

            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, 1, new BigInteger(3), new SeededRandomNumberGenerator());
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>()
                .Setup(algebra => algebra.IsValidDerived(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(algebra => algebra.Cofactor).Returns(1);

            var resultStub = new Mock<CryptoGroupElement<int>>(MockBehavior.Strict, 0, algebraMock.Object);

            var groupMock = new Mock<CryptoGroup<int>>(MockBehavior.Strict, algebraMock.Object);
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<byte[]>()))
                .Returns(resultStub.Object);

            Assert.AreSame(resultStub.Object, groupMock.Object.FromBytes(inputBuffer));
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Verify(group => group.CreateGroupElement(It.Is<byte[]>(b => b == inputBuffer)), Times.Once());
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void TestAddRejectsDifferentGroupElementLeft()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);

            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElement<int>(3, algebraStub.Object);
            var otherElementStub = new CryptoGroupElement<int>(3, otherAlgebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => groupMock.Add(otherElementStub, elementStub)
            );
        }

        [Test]
        public void TestAddRejectsDifferentGroupElementRight()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var groupMock = new CryptoGroupFake(algebraStub.Object);

            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);

            var elementStub = new CryptoGroupElement<int>(3, algebraStub.Object);
            var otherElementStub = new CryptoGroupElement<int>(3, otherAlgebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => groupMock.Add(elementStub, otherElementStub)
            );
        }

        [Test]
        public void TestMultiplyScalarRejectsDifferentGroupElement()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            var groupMock = new CryptoGroupFake(algebraStub.Object);

            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var elementStub = new CryptoGroupElement<int>(3, otherAlgebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => groupMock.MultiplyScalar(elementStub, new BigInteger(8))
            );
        }

        [Test]
        public void TestNegateRejectsDifferentGroupElement()
        {
            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            var groupMock = new CryptoGroupFake(algebraStub.Object);

            var otherAlgebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            otherAlgebraStub.Setup(algebra => algebra.IsValid(It.IsAny<int>())).Returns(true);
            var elementStub = new CryptoGroupElement<int>(3, otherAlgebraStub.Object);

            Assert.Throws<ArgumentException>(
                () => groupMock.Negate(elementStub)
            );
        }

        [Test]
        public void TestOrderCallsAlgebra()
        {
            var order = new BigInteger(29);

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);

            var groupMock = new CryptoGroupFake(algebraMock.Object);
            var result = groupMock.Order;

            Assert.AreEqual(order, result);
        }

        [Test]
        public void TestOrderBitLengthCallsAlgebra()
        {
            int rawBitLength = 11;

            var algebraStub = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraStub.Setup(alg => alg.OrderBitLength).Returns(rawBitLength);

            var groupMock = new CryptoGroupFake(algebraStub.Object);

            var result = groupMock.OrderLength;
            var expected = NumberLength.FromBitLength(algebraStub.Object.OrderBitLength);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestElementBitLengthCallsAlgebra()
        {
            int expectedRaw = 11;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.ElementBitLength).Returns(expectedRaw);

            var groupMock = new CryptoGroupFake(algebraMock.Object);

            var result = groupMock.ElementLength;
            var expected = NumberLength.FromBitLength(expectedRaw);
            Assert.AreEqual(expected, result);

            algebraMock.Verify(alg => alg.ElementBitLength, Times.Once());
        }

        [Test]
        public void TestGenerateRandom()
        {
            var order = new BigInteger(1021);
            int orderByteLength = 2;

            var expectedRaw = 7;

            var algebraMock = new Mock<ICryptoGroupAlgebra<int>>(MockBehavior.Strict);
            algebraMock.Setup(alg => alg.Order).Returns(order);
            algebraMock.Setup(alg => alg.OrderBitLength).Returns(orderByteLength * 8);
            algebraMock.Setup(alg => alg.GenerateElement(It.IsAny<BigInteger>())).Returns(expectedRaw);
            algebraMock.Setup(alg => alg.IsValid(It.IsAny<int>())).Returns(true);

            var expected = new Mock<CryptoGroupElement<int>>(expectedRaw, algebraMock.Object);

            var groupMock = new Mock<CryptoGroup<int>>(algebraMock.Object);
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expected.Object);

            var index = new BigInteger(301);
            byte[] rngResponse = (index - 1).ToByteArray();

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

        [Test]
        public void TestGeneratorAccessor()
        {
            var expectedRaw = 3;
            var algebraMock = new Mock<CryptoGroupAlgebra<int>>(MockBehavior.Strict, expectedRaw, new BigInteger(3), new SeededRandomNumberGenerator()) { CallBase = true };
            algebraMock.Protected().As<CryptoGroupAlgebraProtectedMembers>().
                Setup(alg => alg.IsValidDerived(It.IsAny<int>())).Returns(true);
            algebraMock.Setup(alg => alg.Cofactor).Returns(1);

            var expected = new Mock<CryptoGroupElement<int>>(expectedRaw, algebraMock.Object);
            var groupMock = new Mock<CryptoGroup<int>>(algebraMock.Object);
            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Setup(group => group.CreateGroupElement(It.IsAny<int>()))
                .Returns(expected.Object);

            Assert.AreEqual(expected.Object, groupMock.Object.Generator);

            groupMock.Protected().As<CryptoGroupProtectedMembers>()
                .Verify(group => group.CreateGroupElement(It.Is<int>(x => x == expectedRaw)));
        }

    }

}
