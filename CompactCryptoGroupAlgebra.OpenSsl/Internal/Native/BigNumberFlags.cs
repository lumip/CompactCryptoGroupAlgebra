using System;

namespace CompactCryptoGroupAlgebra.OpenSsl.Internal.Native
{
    /// <summary>
    /// Flags of the OpenSSL <c>BIGNUM</c> structure.
    /// </summary>
    [Flags]
    internal enum BigNumberFlags : int
    {
        None            = 0,
        Malloced        = 0x01,
        StaticData      = 0x02,
        ConstantTime    = 0x04,
        Secure          = 0x08
    }
}