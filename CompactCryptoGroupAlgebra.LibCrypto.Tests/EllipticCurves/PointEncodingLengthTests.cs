// CompactCryptoGroupAlgebra.LibCrypto - OpenSSL libcrypto implementation of CompactCryptoGroupAlgebra interfaces

// SPDX-FileCopyrightText: 2021 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later WITH GPL-3.0-linking-exception
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra.LibCrypto is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra.LibCrypto is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Additional permission under GNU GPL version 3 section 7
// 
// If you modify CompactCryptoGroupAlgebra.LibCrypto, or any covered work, by linking or combining it
// with the OpenSSL library (or a modified version of that library), containing parts covered by the
// terms of the OpenSSL License and the SSLeay License, the licensors of CompactCryptoGroupAlgebra.LibCrypto
// grant you additional permission to convey the resulting work.

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.LibCrypto.EllipticCurves
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
