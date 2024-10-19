// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2022 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Numerics;
using System.Security.Cryptography;

using Moq;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra
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
                   .Callback<byte[]>(buffer =>
                        {
                            Buffer.BlockCopy(rngDeltaBuffer, 0, buffer, 0, rngDeltaBuffer.Length);
                        }
                    );

            var result = rngMock.Object.GetBigIntegerBetween(lowerBound, upperBound);
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
                   .Callback<byte[]>(buffer =>
                        {
                            if (firstCall)
                                Buffer.BlockCopy(invalidRngDeltaBuffer, 0, buffer, 0, invalidRngDeltaBuffer.Length);
                            else
                                Buffer.BlockCopy(validRngDeltaBuffer, 0, buffer, 0, validRngDeltaBuffer.Length);
                            firstCall = false;
                        }
                   );

            var result = rngMock.Object.GetBigIntegerBetween(lowerBound, upperBound);
            Assert.AreEqual(expected, result);
            rngMock.Verify(rng => rng.GetBytes(It.IsAny<byte[]>()), Times.Exactly(2));
        }

        [Test]
        public void TestRandomWithLengthNonByteBoundary()
        {
            var length = NumberLength.FromBitLength(17);

            var leadingOneRngBuffer = ((BigInteger.One << (3 * 8)) - 1).ToByteArray(); // 3 bytes of ones
            var leadingZeroRngBuffer = ((BigInteger.One << (length.InBits - 2))).ToByteArray(); // only bit 16 set
            var expectedFirst = (BigInteger.One << length.InBits) - 1;
            var expectedSecond = ((BigInteger.One << (length.InBits - 2)) ^ (BigInteger.One << (length.InBits - 1)));

            bool firstCall = true;
            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(buffer =>
                        {
                            if (firstCall)
                                Buffer.BlockCopy(leadingOneRngBuffer, 0, buffer, 0, Math.Min(leadingOneRngBuffer.Length, buffer.Length));
                            else
                                Buffer.BlockCopy(leadingZeroRngBuffer, 0, buffer, 0, Math.Min(leadingZeroRngBuffer.Length, buffer.Length));
                            firstCall = false;
                        }
                    );

            var firstResult = rngMock.Object.GetBigIntegerWithLength(length);
            Assert.AreEqual(1, firstResult.Sign, "GetBigIntegerWithLength returned negative number");
            Assert.AreEqual(length, NumberLength.GetLength(firstResult));
            Assert.AreEqual(expectedFirst, firstResult);

            var secondResult = rngMock.Object.GetBigIntegerWithLength(length); ;
            Assert.AreEqual(1, secondResult.Sign, "GetBigIntegerWithLength returned negative number");
            Assert.AreEqual(length, NumberLength.GetLength(secondResult));
            Assert.AreEqual(expectedSecond, secondResult);

            rngMock.Verify(rng => rng.GetBytes(It.IsAny<byte[]>()), Times.Exactly(2));
        }


        [Test]
        public void TestRandomWithLengthByteBoundary()
        {
            var length = NumberLength.FromBitLength(16);

            var leadingOneRngBuffer = ((BigInteger.One << (3 * 8)) - 1).ToByteArray(); // 3 bytes of ones
            var leadingZeroRngBuffer = ((BigInteger.One << (length.InBits - 2))).ToByteArray(); // only bit 15 set
            var expectedFirst = (BigInteger.One << length.InBits) - 1;
            var expectedSecond = ((BigInteger.One << (length.InBits - 2)) ^ (BigInteger.One << (length.InBits - 1)));

            bool firstCall = true;
            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(buffer =>
                        {
                            if (firstCall)
                                Buffer.BlockCopy(leadingOneRngBuffer, 0, buffer, 0, Math.Min(leadingOneRngBuffer.Length, buffer.Length));
                            else
                                Buffer.BlockCopy(leadingZeroRngBuffer, 0, buffer, 0, Math.Min(leadingZeroRngBuffer.Length, buffer.Length));
                            firstCall = false;
                        }
                   );

            var firstResult = rngMock.Object.GetBigIntegerWithLength(length);
            Assert.AreEqual(1, firstResult.Sign, "GetBigIntegerWithLength returned negative number");
            Assert.AreEqual(length, NumberLength.GetLength(firstResult));
            Assert.AreEqual(expectedFirst, firstResult);

            var secondResult = rngMock.Object.GetBigIntegerWithLength(length); ;
            Assert.AreEqual(1, secondResult.Sign, "GetBigIntegerWithLength returned negative number");
            Assert.AreEqual(length, NumberLength.GetLength(secondResult));
            Assert.AreEqual(expectedSecond, secondResult);

            rngMock.Verify(rng => rng.GetBytes(It.IsAny<byte[]>()), Times.Exactly(2));
        }
    }
}
