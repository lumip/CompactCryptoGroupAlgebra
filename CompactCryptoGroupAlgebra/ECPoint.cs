using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// A point on an eliptic curve.
    /// 
    /// A point on an eliptic curve is a two-dimensional point with integer coordinates
    /// from the (finite) field underlying the curve or lies at infinity.
    /// 
    /// Raw group element of <see cref="ECGroupAlgebra"/>. This struct is mainly a
    /// information container and implements no algebraic operations. To perform
    /// eliptic curve operations directly with ECPoint, use the methods of
    /// <see cref="ECGroupAlgebra"/>. If you only need generic cryptographic
    /// group algebra, use <see cref="CryptoGroup{ECPoint}"/> instead.
    /// </summary>
    public struct ECPoint : IEquatable<ECPoint>
    {
        /// <summary>
        /// Whether this point is a point at infinity.
        /// </summary>
        public bool IsAtInfinity { get; }

        /// <summary>
        /// X-coordinate of this point.
        /// </summary>
        public BigInteger X { get; }

        /// <summary>
        /// Y-coordingate of this point.
        /// </summary>
        public BigInteger Y { get; }

        /// <summary>
        /// Instantiates a new ECPoint with the given values.
        /// 
        /// The integer coordinates are only relevant is the point is not at infinity.
        /// </summary>
        /// <param name="x">X-coordinate of the point.</param>
        /// <param name="y">Y-coordinate of the point.</param>
        /// <param name="isAtInfinity">Whether the point is a point at infinity</param>
        public ECPoint(BigInteger x, BigInteger y, bool isAtInfinity = false)
        {
            X = x;
            Y = y;
            IsAtInfinity = isAtInfinity;
        }

        /// <summary>
        /// Creates a point at infninity.
        /// </summary>
        /// <returns>A ECPoint instance representing a point at infinity.</returns>
        public static ECPoint PointAtInfinity { get { return new ECPoint(BigInteger.Zero, BigInteger.Zero, true); } }

        /// <summary>
        /// Clones this point.
        /// </summary>
        /// <returns>A new ECPoint instance that is an identical copy of the original instance.</returns>
        public ECPoint Clone()
        {
            return new ECPoint(X, Y, IsAtInfinity);
        }
        
        /// <summary>
        /// Creates a string representation of this point for displaying.
        /// </summary>
        /// <returns>A string displaying relevant information (coordinates or that it is at infinity) of this point.</returns>
        public override string ToString()
        {
            if (IsAtInfinity)
                return string.Format("(atInf)");
            return string.Format("({0}, {1})", X, Y);
        }

        /// <summary>
        /// Compares this point for equality with another one.
        /// </summary>
        /// <param name="other">The point to compare this one to.</param>
        /// <returns>True, if either both points are points at infinity or have identical coordinates.</returns>
        public bool Equals(ECPoint other)
        {
            return (IsAtInfinity && other.IsAtInfinity) || ((X == other.X) && (Y == other.Y));
        }
    }
}
