namespace CompactCryptoGroupAlgebra.OpenSsl
{

    /// <summary>
    /// A unique identifier of a known and commonly used elliptic curve.
    /// </summary>
    /// <remarks>
    /// Corresponds directly to <c>NID</c> values of OpenSSL.
    /// </remarks>
    public enum EllipticCurveID : int
    {
        // todo(lumip): complete list AND provide lookup function from strings
        /// <summary>
        /// NIST Curve P-239 v3
        /// </summary>
        Prime239v3 = 414,

        /// <summary>
        /// NIST Curve P-256
        /// </summary>
        Prime256v1 = 415
    };

}