using System;
using System.Numerics;
using System.Security.Cryptography;

using Moq;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.Tests
{
    [TestFixture]
    public class RandomNumberGeneratorExtensionsTests
    {
        [Test]
        public void TestRandomBetween()
        {
            var lowerBound = new BigInteger(-10);
            var upperBound = new BigInteger(10);

            var expected = new BigInteger(-7);
            var rngDeltaBuffer = (expected - lowerBound).ToByteArray();

            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(buffer => {
                       Buffer.BlockCopy(rngDeltaBuffer, 0, buffer, 0, rngDeltaBuffer.Length);
                   });

            var result = rngMock.Object.RandomBetween(lowerBound, upperBound);
            Assert.AreEqual(expected, result);
            rngMock.Verify(rng => rng.GetBytes(It.IsAny<byte[]>()), Times.Once);
        }

        [Test]
        public void TestRandomBetweenDoesNotExceedUpperBound()
        {
            var lowerBound = new BigInteger(-10);
            var upperBound = new BigInteger(10);

            var expected = new BigInteger(-7);
            var invalidRngDeltaBuffer = (upperBound - lowerBound + 1).ToByteArray();
            var validRngDeltaBuffer = (expected - lowerBound).ToByteArray();

            bool firstCall = true;
            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(buffer => {
                       if (firstCall)
                           Buffer.BlockCopy(invalidRngDeltaBuffer, 0, buffer, 0, invalidRngDeltaBuffer.Length);
                       else
                           Buffer.BlockCopy(validRngDeltaBuffer, 0, buffer, 0, validRngDeltaBuffer.Length);
                       firstCall = false;
                   });

            var result = rngMock.Object.RandomBetween(lowerBound, upperBound);
            Assert.AreEqual(expected, result);
            rngMock.Verify(rng => rng.GetBytes(It.IsAny<byte[]>()), Times.Exactly(2));
        }
    }
}
