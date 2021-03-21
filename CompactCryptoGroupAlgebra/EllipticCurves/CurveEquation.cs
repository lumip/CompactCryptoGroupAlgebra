// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2020-2021 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra is distributed in the hope that it will be useful,
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
    /// Algebraic primitives for a specific elliptic curve equation type (e.g. Weierstrass or Montgomery curve equations).
    ///
    /// The curve equation is an equation of form <c>f(y) = g(x)</c> for some functions <c>f</c> and <c>g</c> that
    /// specifies the characteristics of an elliptic curve as well as the addition rule.
    /// </summary>
    public abstract class CurveEquation
    {
        /// <summary>
        /// The parameter A in the curve equation.
        /// </summary>
        public BigInteger A { get; }

        /// <summary>
        /// The parameter B in the curve equation.
        /// </summary>
        public BigInteger B { get; }

        /// <summary>
        /// The prime field over which the curve operates.
        /// </summary>
        public BigIntegerField Field { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CurveEquation"/> with given parameters.
        /// </summary>
        /// <param name="prime">Prime characteristic of the underlying scalar field.</param>
        /// <param name="a">The parameter A in the curve equation.</param>
        /// <param name="b">The parameter B in the curve equation.</param>
        protected CurveEquation(BigPrime prime, BigInteger a, BigInteger b)
        {
            A = a;
            B = b;
            Field = new BigIntegerField(prime);
        }

        /// <summary>
        /// Adds two points on the curve according to the addition rule specific
        /// for this <see cref="CurveEquation"/> instance.
        /// </summary>
        /// <param name="left">First point to add.</param>
        /// <param name="right">Second point to add.</param>
        /// <returns>Point on the curve that is the result of adding <paramref name="left"/> and <paramref name="right"/>.</returns>
        public abstract CurvePoint Add(CurvePoint left, CurvePoint right);

        /// <summary>
        /// Tests whether a given point lies on the curve.
        /// </summary>
        /// <param name="point"><see cref="CurvePoint"/> to test.</param>
        /// <returns><c>true</c> if <paramref name="point"/> satisfies the curve equation; otherwise <c>false</c></returns>
        public abstract bool IsPointOnCurve(CurvePoint point);

        /// <summary>
        /// Negates a given point on the curve.
        /// </summary>
        /// <param name="point"><see cref="CurvePoint"/> to negate.</param>
        /// <returns><see cref="CurvePoint"/> instance of the negation of <paramref name="point"/> on the curve.</returns>
        public virtual CurvePoint Negate(CurvePoint point)
        {
            if (point.Equals(CurvePoint.PointAtInfinity))
                return point;
            return new CurvePoint(point.X, Field.Mod(-point.Y));
        }

        /// <summary>
        /// Tests whether two given points on the curve are negations of each other.
        /// </summary>
        /// <param name="left">First point to check.</param>
        /// <param name="right">Second point to check.</param>
        /// <returns><c>true</c> if <paramref name="left"/> is a negation of <paramref name="right"/>; otherwise <c>false</c></returns>
        public bool AreNegations(CurvePoint left, CurvePoint right)
        {
            return Negate(right).Equals(left);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is CurveEquation other && A.Equals(other.A) && B.Equals(other.B) && Field.Modulo.Equals(other.Field.Modulo);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -986235350;
            hashCode = hashCode * -175131752 + A.GetHashCode();
            hashCode = hashCode * -175131752 + B.GetHashCode();
            hashCode = hashCode * -175131752 + Field.Modulo.GetHashCode();
            return hashCode;
        }

    }
}
