// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography
// Copyright (C) 2020  Lukas <lumip> Prediger
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
