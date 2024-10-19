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

using System.Numerics;

namespace CompactCryptoGroupAlgebra.LibCrypto.EllipticCurves
{

    class NISTP256Reference
    {
        // Curve parameters, Appendix D: https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.186-4.pdf

        public static BigPrime Prime = BigPrime.CreateWithoutChecks(BigInteger.Parse("0115792089210356248762697446949407573530086143415290314195533631308867097853951", System.Globalization.NumberStyles.Integer));

        public static BigPrime Order = BigPrime.CreateWithoutChecks(BigInteger.Parse("0115792089210356248762697446949407573529996955224135760342422259061068512044369", System.Globalization.NumberStyles.Integer));

        public static BigInteger generatorX = BigInteger.Parse("06B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", System.Globalization.NumberStyles.HexNumber);

        public static BigInteger generatorY = BigInteger.Parse("04FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", System.Globalization.NumberStyles.HexNumber);

        public static BigInteger Cofactor = BigInteger.One;

        public static int ElementBitLength = NumberLength.GetLength(Prime).InBits;

        public static int OrderBitLength = NumberLength.GetLength(Order).InBits;
    }
}
