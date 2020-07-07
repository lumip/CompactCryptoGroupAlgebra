using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// Cryptographic group based on point addition in elliptic curves in Weierstrass form.
    /// 
    /// Weierstrass curves are of form <c>y² = x³ + Ax + B</c>, with all numbers from finite field
    /// with characteristic <c>P</c>. Elements of the groups are all points (<c>x mod P</c>, <c>y mod P</c>) that satisfy
    /// the curve equation (and the additional "point at infinity" as neutral element).
    ///
    /// The exact parameters of the curve (<c>A</c>, <c>B</c>, <c>P</c>) are encoded in a <see cref="CurveParameters"/> object.
    /// </summary>
    public sealed class CurveGroupAlgebra : CryptoGroupAlgebra<CurvePoint>
    {
        private readonly CurveParameters _parameters;
        private readonly BigIntegerField _field;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveGroupAlgebra"/> class.
        /// </summary>
        /// <param name="parameters">Parameters for the curve.</param>
        public CurveGroupAlgebra(CurveParameters parameters)
            : base(
                parameters.Generator,
                parameters.Order,
                parameters.Cofactor,
                CurvePoint.PointAtInfinity,
                2 * NumberLength.GetLength(parameters.P).InBits
            )
        {
            _parameters = parameters;
            _field = new BigIntegerField(_parameters.P);
            if (!IsElement(Generator))
                throw new ArgumentException("The point given as generator is " +
                	"not a valid point on the curve.", nameof(parameters));
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

        /// <summary>
        /// Adds two given curve points.
        /// 
        /// The operation is commutative, (i.e., symmetric in its arguments).
        /// 
        /// This is not a constant implementation. Side channel attacks might
        /// be able to leak whether one (or both) of the arguments is the
        /// point at infinity or if both arguments are identical (i.e., a single
        /// points gets doubled).
        /// </summary>
        /// <returns>The result of adding the two given points.</returns>
        /// <param name="left">Curve point to add.</param>
        /// <param name="right">Curve point to add.</param>
        public override CurvePoint Add(CurvePoint left, CurvePoint right)
        {
            BigInteger x1 = left.X;
            BigInteger x2 = right.X;
            BigInteger y1 = left.Y;
            BigInteger y2 = right.Y;

            BigInteger lambdaSame = _field.Mod((3 * _field.Square(x1) + _parameters.A) * _field.InvertMult(2 * y1));
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
            BigInteger x3 = _field.Mod(_field.Square(lambda) - x1 - x2);
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
        /// Negates a curve point.
        /// 
        /// The returned element added to the given element will result in the point at infinity (neutral element).
        /// </summary>
        /// <param name="p">The curve point o negate.</param>
        /// <returns>The negation of the given curve point.</returns>
        public override CurvePoint Negate(CurvePoint p)
        {
            if (p.Equals(CurvePoint.PointAtInfinity))
                return p;
            return new CurvePoint(p.X, _field.Mod(-p.Y));
        }

        /// <inheritdocs/>
        protected override bool IsElementDerived(CurvePoint point)
        {
            if (!_field.IsElement(point.X) || !_field.IsElement(point.Y))
                return false;

            // verifying that the point satisfies the curve equation
            BigInteger r = _field.Mod(_field.Pow(point.X, 3) + _parameters.A * point.X + _parameters.B);
            BigInteger ySquared = _field.Square(point.Y);
            return (r == ySquared);
        }

        /// <summary>
        /// Restores a curve point from a byte representation.
        /// </summary>
        /// <param name="buffer">Byte array holding a representation of the curve point to restore.</param>
        /// <returns>The loaded curve point.</returns>
        public override CurvePoint FromBytes(byte[] buffer)
        {
            if (buffer.Length < 2 * _field.ElementByteLength)
                throw new ArgumentException("The given buffer is too short to contain a valid element representation.", nameof(buffer));

            byte[] xBytes = new byte[_field.ElementByteLength];
            byte[] yBytes = new byte[_field.ElementByteLength];

            Buffer.BlockCopy(buffer, 0, xBytes, 0, _field.ElementByteLength);
            Buffer.BlockCopy(buffer, _field.ElementByteLength, yBytes, 0, _field.ElementByteLength);

            BigInteger x = new BigInteger(xBytes);
            BigInteger y = new BigInteger(yBytes);
            return new CurvePoint(x, y);
        }

        /// <summary>
        /// Converts a curve point into a byte representation.
        /// </summary>
        /// <param name="element">The curve point to convert.</param>
        /// <returns>A byte array holding a representation of the given curve point.</returns>
        public override byte[] ToBytes(CurvePoint element)
        {
            byte[] xBytes = element.X.ToByteArray();
            byte[] yBytes = element.Y.ToByteArray();

            Debug.Assert(xBytes.Length <= _field.ElementByteLength);
            Debug.Assert(yBytes.Length <= _field.ElementByteLength);

            byte[] result = new byte[2 * _field.ElementByteLength];
            Buffer.BlockCopy(xBytes, 0, result, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, result, _field.ElementByteLength, yBytes.Length);
            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(CryptoGroupAlgebra<CurvePoint>? other)
        {
            var algebra = other as CurveGroupAlgebra;
            return algebra != null! && EqualityComparer<CurveParameters>.Default.Equals(_parameters, algebra._parameters);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -1453010210 + EqualityComparer<CurveParameters>.Default.GetHashCode(_parameters);
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{CurvePoint}" /> instance using a <see cref="CurveGroupAlgebra" />
        /// instance with the given parameters.
        /// </summary>
        /// <param name="curveParameters">Parameters for the curve.</param>
        public static CryptoGroup<CurvePoint> CreateCryptoGroup(CurveParameters curveParameters)
        {
            return new CryptoGroup<CurvePoint>(new CurveGroupAlgebra(curveParameters));
        }
    }
}
