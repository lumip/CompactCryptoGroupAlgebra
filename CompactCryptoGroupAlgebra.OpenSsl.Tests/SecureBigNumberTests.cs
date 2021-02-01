using NUnit.Framework;

using System;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

namespace CompactCryptoGroupAlgebra.OpenSsl
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
            var expected = NumberLength.FromBitLength(23);;

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
