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
    /// A unique identifier of a known and commonly used elliptic curve.
    /// </summary>
    /// <remarks>
    /// Corresponds directly to <c>NID</c> values of OpenSSL.
    /// </remarks>
    public enum EllipticCurveID : int
    {
        /// <summary>
        /// NIST/X9.62/SECG curve over a 192 bit prime field
        /// </summary>
        Prime192v1 = 409,
        /// <summary>
        /// X9.62 curve over a 192 bit prime field
        /// </summary>
        Prime192v2 = 410,
        /// <summary>
        /// X9.62 curve over a 192 bit prime field
        /// </summary>
        Prime192v3 = 411,
        /// <summary>
        /// X9.62 curve over a 239 bit prime field
        /// </summary>
        Prime239v1 = 412,
        /// <summary>
        /// X9.62 curve over a 239 bit prime field
        /// </summary>
        Prime239v2 = 413,
        /// <summary>
        /// X9.62 curve over a 239 bit prime field
        /// </summary>
        Prime239v3 = 414,
        /// <summary>
        /// X9.62/SECG curve over a 256 bit prime field
        /// </summary>
        Prime256v1 = 415,
        /// <summary>
        /// SECG/WTLS curve over a 112 bit prime field
        /// </summary>
        Secp112r1 = 704,
        /// <summary>
        /// SECG curve over a 112 bit prime field
        /// </summary>
        Secp112r2 = 705,
        /// <summary>
        /// SECG curve over a 128 bit prime field
        /// </summary>
        Secp128r1 = 706,
        /// <summary>
        /// SECG curve over a 128 bit prime field
        /// </summary>
        Secp128r2 = 707,
        /// <summary>
        /// SECG curve over a 160 bit prime field
        /// </summary>
        Secp160k1 = 708,
        /// <summary>
        /// SECG curve over a 160 bit prime field
        /// </summary>
        Secp160r1 = 709,
        /// <summary>
        /// SECG/WTLS curve over a 160 bit prime field
        /// </summary>
        Secp160r2 = 710,
        /// <summary>
        /// SECG curve over a 192 bit prime field
        /// </summary>
        Secp192k1 = 711,
        /// <summary>
        /// SECG curve over a 224 bit prime field
        /// </summary>
        Secp224k1 = 712,
        /// <summary>
        /// NIST/SECG curve over a 224 bit prime field
        /// </summary>
        Secp224r1 = 713,
        /// <summary>
        /// SECG curve over a 256 bit prime field
        /// </summary>
        Secp256k1 = 714,
        /// <summary>
        /// NIST/SECG curve over a 384 bit prime field
        /// </summary>
        Secp384r1 = 715,
        /// <summary>
        /// NIST/SECG curve over a 521 bit prime field
        /// </summary>
        Secp521r1 = 716,
        /// <summary>
        /// SECG curve over a 113 bit binary field
        /// </summary>
        Sect113r1 = 717,
        /// <summary>
        /// SECG curve over a 113 bit binary field
        /// </summary>
        Sect113r2 = 718,
        /// <summary>
        /// SECG/WTLS curve over a 131 bit binary field
        /// </summary>
        Sect131r1 = 719,
        /// <summary>
        /// SECG curve over a 131 bit binary field
        /// </summary>
        Sect131r2 = 720,
        /// <summary>
        /// NIST/SECG/WTLS curve over a 163 bit binary field
        /// </summary>
        Sect163k1 = 721,
        /// <summary>
        /// SECG curve over a 163 bit binary field
        /// </summary>
        Sect163r1 = 722,
        /// <summary>
        /// NIST/SECG curve over a 163 bit binary field
        /// </summary>
        Sect163r2 = 723,
        /// <summary>
        /// SECG curve over a 193 bit binary field
        /// </summary>
        Sect193r1 = 724,
        /// <summary>
        /// SECG curve over a 193 bit binary field
        /// </summary>
        Sect193r2 = 725,
        /// <summary>
        /// NIST/SECG/WTLS curve over a 233 bit binary field
        /// </summary>
        Sect233k1 = 726,
        /// <summary>
        /// NIST/SECG/WTLS curve over a 233 bit binary field
        /// </summary>
        Sect233r1 = 727,
        /// <summary>
        /// SECG curve over a 239 bit binary field
        /// </summary>
        Sect239k1 = 728,
        /// <summary>
        /// NIST/SECG curve over a 283 bit binary field
        /// </summary>
        Sect283k1 = 729,
        /// <summary>
        /// NIST/SECG curve over a 283 bit binary field
        /// </summary>
        Sect283r1 = 730,
        /// <summary>
        /// NIST/SECG curve over a 409 bit binary field
        /// </summary>
        Sect409k1 = 731,
        /// <summary>
        /// NIST/SECG curve over a 409 bit binary field
        /// </summary>
        Sect409r1 = 732,
        /// <summary>
        /// NIST/SECG curve over a 571 bit binary field
        /// </summary>
        Sect571k1 = 733,
        /// <summary>
        /// NIST/SECG curve over a 571 bit binary field
        /// </summary>
        Sect571r1 = 734,
        /// <summary>
        /// RFC 5639 curve over a 160 bit prime field
        /// </summary>
        BrainpoolP160r1 = 921,
        /// <summary>
        /// RFC 5639 curve over a 160 bit prime field
        /// </summary>
        BrainpoolP160t1 = 922,
        /// <summary>
        /// RFC 5639 curve over a 192 bit prime field
        /// </summary>
        BrainpoolP192r1 = 923,
        /// <summary>
        /// RFC 5639 curve over a 192 bit prime field
        /// </summary>
        BrainpoolP192t1 = 924,
        /// <summary>
        /// RFC 5639 curve over a 224 bit prime field
        /// </summary>
        BrainpoolP224r1 = 925,
        /// <summary>
        /// RFC 5639 curve over a 224 bit prime field
        /// </summary>
        BrainpoolP224t1 = 926,
        /// <summary>
        /// RFC 5639 curve over a 256 bit prime field
        /// </summary>
        BrainpoolP256r1 = 927,
        /// <summary>
        /// RFC 5639 curve over a 256 bit prime field
        /// </summary>
        BrainpoolP256t1 = 928,
        /// <summary>
        /// RFC 5639 curve over a 320 bit prime field
        /// </summary>
        BrainpoolP320r1 = 929,
        /// <summary>
        /// RFC 5639 curve over a 320 bit prime field
        /// </summary>
        BrainpoolP320t1 = 930,
        /// <summary>
        /// RFC 5639 curve over a 384 bit prime field
        /// </summary>
        BrainpoolP384r1 = 931,
        /// <summary>
        /// RFC 5639 curve over a 384 bit prime field
        /// </summary>
        BrainpoolP384t1 = 932,
        /// <summary>
        /// RFC 5639 curve over a 512 bit prime field
        /// </summary>
        BrainpoolP512r1 = 933,
        /// <summary>
        /// RFC 5639 curve over a 512 bit prime field
        /// </summary>
        BrainpoolP512t1 = 934
    };

}
