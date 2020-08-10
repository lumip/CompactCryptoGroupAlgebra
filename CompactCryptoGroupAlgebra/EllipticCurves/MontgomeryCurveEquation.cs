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
    /// Curve equation implementation for Montgomery curves
    /// of form <c>By² = x³ + Ax² + x</c>.
    /// </summary>
    public class MontgomeryCurveEquation : CurveEquation
    {

        /// <summary>
        /// Initializes a new instance of <see cref="MontgomeryCurveEquation"/>
        /// with the given parameters.
        /// </summary>
        /// <param name="prime">Prime characteristic of the underlying scalar field.</param>
        /// <param name="a">The parameter A in the curve equation.</param>
        /// <param name="b">The parameter B in the curve equation.</param>
        public MontgomeryCurveEquation(BigPrime prime, BigInteger a, BigInteger b)
            : base(prime, a, b) { }

        /// <inheritdoc/>
        public override CurvePoint Add(CurvePoint left, CurvePoint right)
        {
            BigInteger x1 = left.X;
            BigInteger x2 = right.X;
            BigInteger y1 = left.Y;
            BigInteger y2 = right.Y;

            BigInteger lambda;
            if (left.Equals(right))
            {
                lambda = Field.Mod((3 * Field.Square(x1) + 2 * A * x1+ 1) * Field.InvertMult(2 * B * y1));
            }
            else
            {
                lambda = Field.Mod((y2 - y1) * Field.InvertMult(x2 - x1));
            }
            BigInteger x3 = Field.Mod(B * Field.Square(lambda) - x1 - x2 - A);
            BigInteger y3 = Field.Mod(lambda * (x1 - x3) - y1);

            CurvePoint result = CurvePoint.PointAtInfinity;
            bool pointsAreNegations = AreNegations(left, right);
            if (left.IsAtInfinity && right.IsAtInfinity)
                result = CurvePoint.PointAtInfinity;
            if (left.IsAtInfinity && !right.IsAtInfinity)
                result = right.Clone();
            if (right.IsAtInfinity && !left.IsAtInfinity)
                result = left.Clone();
            if (!left.IsAtInfinity && !right.IsAtInfinity && pointsAreNegations)
                result = CurvePoint.PointAtInfinity;
            if (!left.IsAtInfinity && !right.IsAtInfinity && !pointsAreNegations)
                result = new CurvePoint(x3, y3);
            return result;
        }

        /// <inheritdoc/>
        public override bool IsPointOnCurve(CurvePoint point)
        {
            BigInteger r = Field.Mod(Field.Pow(point.X, 3) + A * Field.Square(point.X) + point.X);
            BigInteger ySquared = Field.Mod(B * Field.Square(point.Y));
            return (r == ySquared);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is MontgomeryCurveEquation other && base.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -752386232 + base.GetHashCode();
        }

    }
}
