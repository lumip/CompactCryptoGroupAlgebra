using System;
using System.Numerics;
using System.Security.Cryptography;

using Moq;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class PrimalityTestTests
    {
        [Test]
        [TestCase(7, 7, 2)] // fails a^q    % p !=  1
        [TestCase(5, 7, 2)] // fails a^q    % p != -1
        [TestCase(3, 7, 2)] // fails a^(2q) % p != -1
        public void TestIsCompositeWitnessForPrime(int a, int q, int k)
        {
            var result = PrimalityTest.IsCompositeWitness(a, q, k);
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(8, 5, 2)]
        public void TestIsCompositeWitnessForComposite(int a, int q, int k)
        {
            var result = PrimalityTest.IsCompositeWitness(a, q, k);
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(11)]
        [TestCase(3041)]
        [TestCase(8052311)]
        [TestCase(13132877)]
        public void TestMillerRabinWithPrimes(int rawN)
        {
            BigInteger n = new BigInteger(rawN);
            Random random = new Random(0);

            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(random.NextBytes);

            Assert.IsTrue(n.IsProbablyPrime(rngMock.Object));
        }

        [Test]
        [TestCase(32)]
        [TestCase(2 * 29)]
        [TestCase(8052311 * 17)]
        [TestCase(1709 * 2713)]
        public void TestMillerRabinWithComposites(int rawN)
        {
            BigInteger n = new BigInteger(rawN);
            Random random = new Random(0);

            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(random.NextBytes);

            Assert.IsFalse(n.IsProbablyPrime(rngMock.Object));
        }
    }
}
