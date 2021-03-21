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

namespace CompactCryptoGroupAlgebra.LibCrypto.EllipticCurves
{

    /// <summary>
    /// Specifies the encoding of a point into bytes.
    /// </summary>
    /// <remarks>
    /// The enum values correspond directly to OpenSSL.
    /// </remarks>
    public enum PointEncoding : int
    {
        /// <summary>
        /// The point is encoded as <c>z||x</c>, where the octet <c>z</c> specifies
        /// which solution of the quadratic equation <c>y</c> is.
        /// </summary>
        Compressed = 2,
        
        /// <summary>
        /// The point is encoded as <c>z||x||y</c>, where <c>z</c> is the octet <c>0x04</c>.
        /// </summary>
        Uncompressed = 4,

        /// <summary>
        /// The point is encoded as <c>z||x||y</c>, where the octet <c>z</c> specifies
        /// which solution of the quadratic equation <c>y</c>.
        /// </summary>
        Hybrid = 6
    }

    /// <summary>
    /// Utility functions to compute the bits requires for encoding elliptic curve points.
    /// </summary>
    public static class PointEncodingLength
    {
        /// <summary>
        /// Computes the bits required for a given encoding of an elliptic curve point.
        /// </summary>
        /// <returns>The integer bit length of the encoded points.</returns>
        /// <param name="encoding">The point encoding.</param>
        /// <param name="elementBitLength">The bit length elements in the underlying field of the curve.</param>
        public static int GetEncodingBitLength(PointEncoding encoding, int elementBitLength)
        {
            switch (encoding)
            {
                case PointEncoding.Compressed:
                    return elementBitLength + 8;
                default:
                    return 2 * elementBitLength + 8;
            }
        }
    }

}