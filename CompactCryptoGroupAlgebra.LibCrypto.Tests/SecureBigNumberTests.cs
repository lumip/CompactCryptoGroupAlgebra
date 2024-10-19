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
using CompactCryptoGroupAlgebra.LibCrypto.Internal.Native;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.LibCrypto
{
    public class SecureBigNumberTests
    {

        [Test]
        public void TestConstructor()
        {
            var number = new SecureBigNumber();
            Assert.That(BigNumberHandle.GetFlags(
                number.Handle, BigNumberFlags.Secure).HasFlag(BigNumberFlags.Secure)
            );
            Assert.That(BigNumberHandle.GetFlags(
                number.Handle, BigNumberFlags.ConstantTime).HasFlag(BigNumberFlags.ConstantTime)
            );
            Assert.That(!number.Handle.IsInvalid);
            Assert.That(!number.Handle.IsClosed);
        }

        [Test]
        public void TestFromRawHandleFailsOnInvalid()
        {
            var invalidHandle = new BigNumberHandle();
            Assert.That(invalidHandle.IsInvalid);
            Assert.Throws<ArgumentException>(() => SecureBigNumber.FromRawHandle(invalidHandle));
        }

        [Test]
        public void TestFromRawHandle()
        {
            using (var handle = BigNumberHandle.Create())
            {
                BigNumberHandle.SetWord(handle, 3);
                var number = SecureBigNumber.FromRawHandle(handle);
                Assert.That(BigNumberHandle.Compare(number.Handle, handle) == 0);
                Assert.That(BigNumberHandle.GetFlags(
                    number.Handle, BigNumberFlags.Secure).HasFlag(BigNumberFlags.Secure)
                );
                Assert.That(BigNumberHandle.GetFlags(
                    number.Handle, BigNumberFlags.ConstantTime).HasFlag(BigNumberFlags.ConstantTime)
                );
            }
        }

        [Test]
        public void TestFromBigNumber()
        {
            var rawValue = 0x548f07;
            var number = SecureBigNumber.FromBigNumber(new BigNumber(rawValue));

            using (var expectedHandle = BigNumberHandle.Create())
            {
                BigNumberHandle.SetWord(expectedHandle, (ulong)rawValue);
                Assert.That(BigNumberHandle.Compare(number.Handle, expectedHandle) == 0);
            }
        }

        [Test]
        public void TestDispose()
        {
            var number = new SecureBigNumber();
            Assert.That(!number.Handle.IsClosed);
            number.Dispose();
            Assert.That(number.Handle.IsClosed);
            Assert.DoesNotThrow(number.Dispose);
        }

        [Test]
        public void TestLength()
        {
            var number = SecureBigNumber.FromBigNumber(new BigNumber(0x548f07));
            var expected = NumberLength.FromBitLength(23);

            Assert.That(number.Length.Equals(expected));
        }

        [Test]
        public void TestRandom()
        {
            var NumTests = 100;
            var range = new BigNumber(0x869375a76);
            for (var k = 0; k < NumTests; k++)
            {
                using (var number = SecureBigNumber.Random(range))
                {
                    Assert.That(BigNumberHandle.GetFlags(
                        number.Handle, BigNumberFlags.Secure).HasFlag(BigNumberFlags.Secure)
                    );
                    Assert.That(BigNumberHandle.GetFlags(
                        number.Handle, BigNumberFlags.ConstantTime).HasFlag(BigNumberFlags.ConstantTime)
                    );
                    Assert.That(BigNumberHandle.Compare(number.Handle, range.Handle) < 0);
                }
            }
        }

    }
}
