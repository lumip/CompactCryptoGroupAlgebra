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

using System;
using System.Numerics;
using CompactCryptoGroupAlgebra.LibCrypto.Internal.Native;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.LibCrypto
{
    public class BigNumberTests
    {

        [Test]
        public void TestConstructor()
        {
            var number = new BigNumber();
            Assert.That(!number.Handle.IsInvalid);
            Assert.That(!number.Handle.IsClosed);
        }

        [Test]
        public void TestBigIntegerConstructor()
        {
            var rawValue = 936758;
            var bigInteger = new BigInteger(rawValue);
            var expected = new BigNumber(rawValue);

            var number = new BigNumber(bigInteger);

            Assert.That(number.Equals(expected));
        }

        [Test]
        public void TestIntegerConstructor()
        {
            var rawValue = 869235;
            var number = new BigNumber(rawValue);

            using (var expectedHandle = BigNumberHandle.Create())
            {
                BigNumberHandle.SetWord(expectedHandle, (ulong)rawValue);
                Assert.That(BigNumberHandle.Compare(number.Handle, expectedHandle) == 0);
            }
        }

        [Test]
        public void TestBytesConstructor()
        {
            var rawValue = new byte[] { 0x07, 0x8f, 0xa4 };
            var expected = new BigNumber(0xa48f07);

            var number = new BigNumber(rawValue);

            Assert.That(number.Equals(expected));
        }

        [Test]
        public void TestToBytes()
        {
            var rawValue = 0xa48f07;
            var expected = new byte[] { 0x07, 0x8f, 0xa4 };

            var number = new BigNumber(rawValue);
            var result = number.ToBytes();

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromRawHandleFailsOnInvalid()
        {
            var invalidHandle = new BigNumberHandle();
            Assert.That(invalidHandle.IsInvalid);
            Assert.Throws<ArgumentException>(() => BigNumber.FromRawHandle(invalidHandle));
        }

        [Test]
        public void TestFromRawHandleFailsWithSecure()
        {
            var secureHandle = BigNumberHandle.CreateSecure();
            Assert.Throws<ArgumentException>(() => BigNumber.FromRawHandle(secureHandle));
        }

        [Test]
        public void TestFromRawHandle()
        {
            using (var handle = BigNumberHandle.Create())
            {
                BigNumberHandle.SetWord(handle, 3);
                var number = BigNumber.FromRawHandle(handle);
                Assert.That(BigNumberHandle.Compare(number.Handle, handle) == 0);
            }
        }

        [Test]
        public void TestToBigInteger()
        {
            byte[] bigIntBuffer = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde, 0xf0, 0x00 };
            var expected = new BigInteger(bigIntBuffer);

            var number = new BigNumber(bigIntBuffer);
            var result = number.ToBigInteger();

            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestDispose()
        {
            var number = new BigNumber();
            Assert.That(!number.Handle.IsClosed);
            number.Dispose();
            Assert.That(number.Handle.IsClosed);
            Assert.DoesNotThrow(number.Dispose);
        }

        [Test]
        public void TestOne()
        {
            var number = BigNumber.One;
            Assert.That(number.Equals(new BigNumber(1)));
        }

        [Test]
        public void TestZero()
        {
            var number = BigNumber.Zero;
            Assert.That(number.Equals(new BigNumber(0)));
        }

        [Test]
        public void TestLength()
        {
            var number = new BigNumber(0x548f07);
            var expected = NumberLength.FromBitLength(23); ;

            Assert.That(number.Length.Equals(expected));
        }

        [Test]
        public void TestEqualsAndHashCode()
        {
            var number = new BigNumber(0x548f07);
            var equalNumber = new BigNumber(0x548f07);
            var nonequalNumber = new BigNumber(823935);

            Assert.That(number.Equals(equalNumber));
            Assert.That(number.GetHashCode().Equals(equalNumber.GetHashCode()));
            Assert.That(!number.Equals(nonequalNumber));
        }

        [Test]
        public void TestEqualsWithNull()
        {
            var number = new BigNumber(86935);
            Assert.That(!number.Equals(null));
        }

        [Test]
        public void TestToString()
        {
            int rawValue = 0x96235;
            var expected = (new BigInteger(rawValue)).ToString();
            var number = new BigNumber(rawValue);

            Assert.That(number.ToString().Equals(expected));
        }

        [Test]
        public void TestModMul()
        {
            ulong firstNumberRaw = 69345;
            ulong secondNumberRaw = 97628;
            ulong moduloRaw = 100001;
            ulong resultRaw = (firstNumberRaw * secondNumberRaw) % moduloRaw;
            var expected = new BigNumber(resultRaw);

            var firstNumber = new BigNumber(firstNumberRaw);
            var secondNumber = new BigNumber(secondNumberRaw);
            var modulo = new BigNumber(moduloRaw);

            var result = firstNumber.ModMul(secondNumber, modulo);
            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestSecureModExp()
        {
            var baseRaw = 96235;
            var exponentRaw = 7354;
            var moduloRaw = 200001;
            var resultRaw = BigInteger.ModPow(baseRaw, exponentRaw, moduloRaw);
            var expected = new BigNumber(resultRaw);

            var basis = new BigNumber(baseRaw);
            var exponent = SecureBigNumber.FromBigNumber(new BigNumber(exponentRaw));
            var modulo = new BigNumber(moduloRaw);

            var result = basis.ModExp(exponent, modulo);

            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestModExp()
        {
            var baseRaw = 96235;
            var exponentRaw = 7354;
            var moduloRaw = 200001;
            var resultRaw = BigInteger.ModPow(baseRaw, exponentRaw, moduloRaw);
            var expected = new BigNumber(resultRaw);

            var basis = new BigNumber(baseRaw);
            var exponent = new BigNumber(exponentRaw);
            var modulo = new BigNumber(moduloRaw);

            var result = basis.ModExp(exponent, modulo);

            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestModReciprocal()
        {
            var numberRaw = 7652;
            var modulo = BigPrime.CreateWithoutChecks(89237);
            var number = new BigNumber(numberRaw);
            var expected = number.ModExp(new BigNumber(modulo - 2), new BigNumber(modulo));
            var result = number.ModReciprocal(new BigNumber(modulo));

            Assert.That(result.Equals(expected));
        }
    }
}
