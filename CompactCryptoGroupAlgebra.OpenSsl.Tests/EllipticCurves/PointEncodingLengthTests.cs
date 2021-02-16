using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.OpenSsl.EllipticCurves
{

    class PointEncodingLengthTests
    {
        [Test]
        public void TestGetEncodingBitLengthCompressed()
        {
            var elementBitLength = 10;
            var expected = 18;

            var result = PointEncodingLength.GetEncodingBitLength(PointEncoding.Compressed, elementBitLength);
            Assert.That(result == expected);
        }

        [Test]
        public void TestGetEncodingBitLengthUncompressed()
        {
            var elementBitLength = 10;
            var expected = 28;

            var result = PointEncodingLength.GetEncodingBitLength(PointEncoding.Uncompressed, elementBitLength);
            Assert.That(result == expected);
        }

        [Test]
        public void TestGetEncodingBitLengthHybrid()
        {
            var elementBitLength = 10;
            var expected = 28;

            var result = PointEncodingLength.GetEncodingBitLength(PointEncoding.Hybrid, elementBitLength);
            Assert.That(result == expected);
        }
    }

}