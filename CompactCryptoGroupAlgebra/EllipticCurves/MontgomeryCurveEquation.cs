using System;
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

            BigInteger lambdaSame = Field.Mod((3 * Field.Square(x1) + 2 * A * x1+ 1) * Field.InvertMult(2 * B * y1));
            BigInteger lambdaDiff = Field.Mod((y2 - y1) * Field.InvertMult(x2 - x1));
            BigInteger lambda;
            // note: branching is side-channel vulnerable
            if (left.Equals(right)) // Equals probably not constant time
            {
                lambda = lambdaSame;
            }
            else
            {
                lambda = lambdaDiff;
            }
            BigInteger x3 = Field.Mod(B * Field.Square(lambda) - x1 - x2 - A);
            BigInteger y3 = Field.Mod(lambda * (x1 - x3) - y1);

            CurvePoint result = CurvePoint.PointAtInfinity;
            bool pointsAreNegations = AreNegations(left, right);
            // note: branching is side-channel vulnerable
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
            MontgomeryCurveEquation? other = obj as MontgomeryCurveEquation;
            return other != null && base.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -752386232 + base.GetHashCode();
        }

    }
}