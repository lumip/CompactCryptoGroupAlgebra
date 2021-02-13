using NUnit.Framework;
using System;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;
using System.Runtime.InteropServices;

namespace CompactCryptoGroupAlgebra.OpenSsl
{
    public class OpenSslNativeExceptionTests
    {

        [Test]
        public void TestConstructorWithErrorCode()
        {
            ulong errorCode = 268877932;
            var ex = new OpenSslNativeException(errorCode);

            Assert.That(ex.Code == errorCode);
            Assert.That(ex.Message.Contains("elliptic curve routines:EC_GROUP_new:"));
        }

        [DllImport("libcrypto", CallingConvention = CallingConvention.Cdecl)]
        private extern static ECGroupHandle EC_GROUP_new(IntPtr method);

        [Test]
        public void TestConstructorFromLastError()
        {
            // make invalid call to force an error condition
            var handle = EC_GROUP_new(IntPtr.Zero);
            Assert.That(handle.IsInvalid);

            ulong expectedErrorCode = 268877932;

            var ex = new OpenSslNativeException();
            Assert.That(ex.Code == expectedErrorCode);
            Assert.That(ex.Message.Contains("elliptic curve routines:EC_GROUP_new:"));
        }
    }
}