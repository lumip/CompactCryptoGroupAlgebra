using NUnit.Framework;

using System.Numerics;

namespace CompactCryptoGroupAlgebra.Multiplicative
{
    class ExtendedEuclideanAlgorithmTests
    {
        
        [Test]
        [TestCase(17, 7, -2, 5)]
        [TestCase(7, 17, 5, -2)]
        public void TestExtendedGreatestCommonDivisor(int rawA, int rawB, int rawX, int rawY)
        {
            var a = new BigInteger(rawA);
            var b = new BigInteger(rawB);

            var expectedX = new BigInteger(rawX);
            var expectedY = new BigInteger(rawY);

            (var gcd, var x, var y) = ExtendedEuclideanAlgorithm.GreatestCommonDivisor(a, b);
            Assert.That(x.Equals(expectedX));
            Assert.That(y.Equals(expectedY));
        }

    }
}
