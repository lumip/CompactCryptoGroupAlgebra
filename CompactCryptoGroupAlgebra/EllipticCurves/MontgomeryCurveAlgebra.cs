using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    public class MontgomeryCurveAlgebra : CryptoGroupAlgebra<CurvePoint>
    {
        private readonly CurveParameters _parameters;
        private readonly BigIntegerField _field;

        public MontgomeryCurveAlgebra(CurveParameters parameters)
            : base(parameters.Generator, parameters.Order)
        {
            _parameters = parameters;
            _field = new BigIntegerField(_parameters.P);
        }

        /// <inheritdoc/>
        public override BigInteger Cofactor { get { return _parameters.Cofactor; } }

        /// <inheritdoc/>
        public override int ElementBitLength { get { return 2 * NumberLength.GetLength(_parameters.P).InBits; } }

        /// <inheritdoc/>
        public override CurvePoint NeutralElement { get { return CurvePoint.PointAtInfinity; } }

        /// <inheritdoc/>
        public override CurvePoint Add(CurvePoint left, CurvePoint right)
        {
            BigInteger x1 = left.X;
            BigInteger x2 = right.X;
            BigInteger y1 = left.Y;
            BigInteger y2 = right.Y;

            BigInteger lambdaSame = _field.Mod((3 * _field.Square(x1) + 2 * _parameters.A * x1+ 1) * _field.InvertMult(2 * _parameters.B * y1));
            BigInteger lambdaDiff = _field.Mod((y2 - y1) * _field.InvertMult(x2 - x1));
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
            BigInteger x3 = _field.Mod(_parameters.B * _field.Square(lambda) - x1 - x2 - _parameters.A);
            BigInteger y3 = _field.Mod(lambda * (x1 - x3) - y1);

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

        /// <summary>
        /// Checks whether two given points are negations of each other, i.e.,
        /// adding them results in the zero element (point at infinity).
        /// </summary>
        /// <returns><c>true</c>, if the given points are negations of each other, <c>false</c> otherwise.</returns>
        /// <param name="left">Curve point</param>
        /// <param name="right">Curve point</param>
        public bool AreNegations(CurvePoint left, CurvePoint right)
        {
            var inv = Negate(right);
            return left.Equals(inv);
        }

        /// <inheritdoc/>
        public override CurvePoint FromBytes(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override byte[] ToBytes(CurvePoint element)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override CurvePoint Negate(CurvePoint e)
        {
            if (e.IsAtInfinity)
                return e;
            return new CurvePoint(e.X, -e.Y);
        }

        /// <inheritdoc/>
        protected override bool IsElementDerived(CurvePoint point)
        {
            if (!_field.IsElement(point.X))
                return false;
            if (!_field.IsElement(point.Y))
                return false;

            // verifying that the point satisfies the curve equation
            BigInteger r = _field.Mod(_field.Pow(point.X, 3) + _parameters.A * _field.Square(point.X) + point.X);
            BigInteger ySquared = _field.Mod(_parameters.B * _field.Square(point.Y));
            return (r == ySquared);
        }

        /// <summary>
        /// Selects one of two given curve points.
        /// 
        /// This allows side-channel resistant selection by avoiding branching.
        /// The selection is made based on the value of the parameter
        /// <paramref name="selection"/>. A value of <c>BigInteger.Zero</c>selects the curve point
        /// given as <paramref name="first"/>, a value of <c>BigInteger.One</c> selects <paramref name="second"/>.
        /// </summary>
        /// <returns>The selected boolean.</returns>
        /// <param name="selection">Selection indicator.</param>
        /// <param name="first">First selection option.</param>
        /// <param name="second">First selection option.</param>
        protected override CurvePoint Multiplex(BigInteger selection, CurvePoint first, CurvePoint second)
        {
            return CurvePoint.Multiplex(selection, first, second);
        }

        /// <inheritdoc/>
        protected override CurvePoint MultiplyScalarUnchecked(CurvePoint e, BigInteger k, int factorBitLength)
        {
            return base.MultiplyScalarUnchecked(e, k, factorBitLength);
        }

        /// <inheritdoc/>
        public override bool Equals(CryptoGroupAlgebra<CurvePoint>? other)
        {
            var algebra = other as MontgomeryCurveAlgebra;
            return other != null &&
                   EqualityComparer<CurveParameters>.Default.Equals(_parameters, algebra!._parameters);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -2051777468 + EqualityComparer<CurveParameters>.Default.GetHashCode(_parameters);
        }
    }
}
