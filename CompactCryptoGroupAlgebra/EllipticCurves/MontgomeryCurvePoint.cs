using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// A point on a Montgomery elliptic curve in projected coordinates
    /// and without y-coordinate.
    /// 
    /// Raw group element of <see cref="XOnlyMontgomeryCurveAlgebra"/>. This struct
    /// is mainly a information container and implements no algebraic operations.
    /// </summary>
    public readonly struct MontgomeryCurvePoint
    {
        /// <summary>
        /// X-coordinate of this point.
        /// </summary>
        public BigInteger X { get; }

        /// <summary>
        /// Z-coordinate of this point.
        /// </summary>
        public BigInteger Z { get; }

        /// <summary>
        /// Whether this point is a point at infinity.
        /// </summary>
        public bool IsAtInfinity { get { return Z.IsZero; } }

        /// <summary>
        /// Instantiates a new <see cref="MontgomeryCurvePoint" /> with the given coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the point.</param>
        /// <param name="z">Z-coordinate of the point.</param>
        public MontgomeryCurvePoint(BigInteger x, BigInteger z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Instantiates a new <see cref="MontgomeryCurvePoint" /> with the given X-coordinate.
        /// </summary>
        /// <param name="x">X-coordinate of the point.</param>
        public MontgomeryCurvePoint(BigInteger x)
            : this(x, BigInteger.One) { }

        /// <summary>
        /// Creates a point at infinity.
        /// </summary>
        /// <returns>A CurvePoint instance representing a point at infinity.</returns>
        public static MontgomeryCurvePoint PointAtInfinity = new MontgomeryCurvePoint();

        /// <summary>
        /// Compares this point for equality with another one.
        /// </summary>
        /// <param name="other">The point to compare this one to.</param>
        /// <returns>True, if either both points are points at infinity or have identical coordinates.</returns>
        public bool Equals(MontgomeryCurvePoint other)
        {
            return X == other.X && Z == other.Z;
        }

        /// <summary>
        /// Whether this point is normalized (i.e. has z-coordinate <c>1</c>)
        /// </summary>
        public bool IsNormalized { get { return Z.IsOne; } }
    }
}
