using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// A point on an eliptic curve.
    /// 
    /// A point on an eliptic curve is a two-dimensional point with integer coordinates
    /// from the (finite) field underlying the curve or lies at infinity.
    /// 
    /// Raw group element of <see cref="CurveGroupAlgebra"/>. This struct is mainly a
    /// information container and implements no algebraic operations. To perform
    /// eliptic curve operations directly with CurvePoint, use the methods of
    /// <see cref="CurveGroupAlgebra"/>. If you only need generic cryptographic
    /// group algebra, use <see cref="CryptoGroup{CurvePoint}"/> instead.
    /// </summary>
    public readonly struct CurvePoint : IEquatable<CurvePoint>
    {
        /// <summary>
        /// Creates a point at infinity.
        /// </summary>
        /// <returns>A <see cref="CurvePoint" /> instance representing a point at infinity.</returns>
        public static readonly CurvePoint PointAtInfinity = new CurvePoint(0, 0, true);

        /// <summary>
        /// Whether this point is a point at infinity.
        /// </summary>
        public bool IsAtInfinity { get; }

        /// <summary>
        /// X-coordinate of this point.
        /// </summary>
        public BigInteger X { get; }

        /// <summary>
        /// Y-coordinate of this point.
        /// </summary>
        public BigInteger Y { get; }

        /// <summary>
        /// Instantiates a new <see cref="CurvePoint" /> with the given values.
        /// 
        /// The integer coordinates are only relevant is the point is not at infinity.
        /// </summary>
        /// <param name="x">X-coordinate of the point.</param>
        /// <param name="y">Y-coordinate of the point.</param>
        /// <param name="isAtInfinity">Whether the point is a point at infinity</param>
        private CurvePoint(BigInteger x, BigInteger y, bool isAtInfinity)
        {
            X = x;
            Y = y;
            IsAtInfinity = isAtInfinity;
        }

        /// <summary>
        /// Instantiates a new <see cref="CurvePoint" /> with the given coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the point.</param>
        /// <param name="y">Y-coordinate of the point.</param>
        public CurvePoint(BigInteger x, BigInteger y)
            : this(x, y, false)
        { }

        /// <summary>
        /// Clones this point.
        /// </summary>
        /// <returns>A new <see cref="CurvePoint" /> instance that is an identical copy of the original instance.</returns>
        public CurvePoint Clone()
        {
            return new CurvePoint(X, Y, IsAtInfinity);
        }
        
        /// <summary>
        /// Creates a string representation of this point for displaying.
        /// </summary>
        /// <returns>A string displaying relevant information (coordinates or that it is at infinity) of this point.</returns>
        public override string ToString()
        {
            if (IsAtInfinity)
                return "(at infinity)";
            return string.Format($"({X}, {Y})");
        }

        /// <summary>
        /// Compares this point for equality with another one.
        /// </summary>
        /// <param name="other">The point to compare this one to.</param>
        /// <returns>True, if either both points are points at infinity or have identical coordinates.</returns>
        public bool Equals(CurvePoint other)
        {
            return (IsAtInfinity && other.IsAtInfinity) || (!IsAtInfinity && !other.IsAtInfinity && (X == other.X) && (Y == other.Y));
        }

    }
}
