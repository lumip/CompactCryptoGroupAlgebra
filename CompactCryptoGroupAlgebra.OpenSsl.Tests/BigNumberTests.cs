using NUnit.Framework;

using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal
{
    public class BigNumberTests
    {

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestConstructor(bool useSecure)
        {
            var number = new BigNumber(useSecure);
            Assert.That(number.IsSecure == useSecure);
            Assert.That(number.IsConstantTime == useSecure);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestBigIntegerConstructor(bool useSecure)
        {
            var rawValue = 936758;
            var bigInteger = new BigInteger(rawValue);
            var expected = new BigNumber(rawValue);

            var number = new BigNumber(bigInteger, useSecure);
            Assert.That(number.IsSecure == useSecure);
            Assert.That(number.IsConstantTime == useSecure);

            Assert.That(number.Equals(expected));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestIntegerConstructor(bool useSecure)
        {
            var rawValue = 869235;
            var number = new BigNumber(rawValue, useSecure);
            Assert.That(number.IsSecure == useSecure);
            Assert.That(number.IsConstantTime == useSecure);

            using (var expectedHandle = Native.BigNumberHandle.Create())
            {
                Native.BigNumberHandle.SetWord(expectedHandle, (ulong)rawValue);
                Assert.That(Native.BigNumberHandle.Compare(number.Handle, expectedHandle) == 0);
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestBytesConstructor(bool useSecure)
        {
            var rawValue = new byte[] { 0x07, 0x8f, 0xa4 };
            var expected = new BigNumber(0xa48f07);

            var number = new BigNumber(rawValue, useSecure);
            Assert.That(number.IsSecure == useSecure);
            Assert.That(number.IsConstantTime == useSecure);

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
            var invalidHandle = new Native.BigNumberHandle();
            Assert.That(invalidHandle.IsInvalid);
            Assert.Throws<ArgumentException>(() => BigNumber.FromRawHandle(invalidHandle));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestFromRawHandle(bool useSecure)
        {
            using (var handle = Native.BigNumberHandle.Create())
            {
                Native.BigNumberHandle.SetWord(handle, 3);
                var number = BigNumber.FromRawHandle(handle, useSecure);
                Assert.That(Native.BigNumberHandle.Compare(number.Handle, handle) == 0);
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
            var expected = NumberLength.FromBitLength(23);;

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
    }
}
