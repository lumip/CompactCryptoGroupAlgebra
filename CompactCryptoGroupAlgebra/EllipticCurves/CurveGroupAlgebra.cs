using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// CurveGroupAlgebra provides algebraic operations for groups based on elliptic curves
    /// in Weierstrass form, i.e., <c>y² = x³ + Ax + B</c> over the finite field defined by a prime number <c>P</c>.
    ///
    /// The exact parameters of the curve (A, B, P) are encoded in a <see cref="CurveParameters"/> object.
    /// </summary>
    public sealed class CurveGroupAlgebra : CryptoGroupAlgebra<CurvePoint>
    {
        private readonly CurveParameters _parameters;
        private readonly BigIntegerRing _ring;

        /// <summary>
        /// The neutral element of the group i.e., the point at infinity of the elliptic curve.
        /// </summary>
        public override CurvePoint NeutralElement { get { return CurvePoint.PointAtInfinity; } }

        /// <summary>
        /// The cofactor of the defined elliptic curve.
        ///
        /// The cofactor is the ratio of the number of points on the curve
        /// and the order of the subgroup of safe points generated by the generator.
        /// </summary>
        public override BigInteger Cofactor { get { return _parameters.Cofactor; } }

        /// <summary>
        /// The maximum bit length of elements of the group.
        /// 
        /// This is the number of bits required to represent any element of the group.
        /// </summary>
        public override int ElementBitLength { get { return 2 * NumberLength.GetLength(_ring.Modulo).InBits; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveGroupAlgebra"/> class.
        /// </summary>
        /// <param name="parameters">Parameters for the curve.</param>
        public CurveGroupAlgebra(CurveParameters parameters)
            : base(parameters.Generator, parameters.Order)
        {
            _parameters = parameters;
            _ring = new BigIntegerRing(_parameters.P);
            if (!IsValid(Generator))
                throw new ArgumentException("The point given as generator is" +
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

            BigInteger lambdaSame = _ring.Mod((3 * _ring.Square(x1) + _parameters.A) * _ring.InvertMult(2 * y1));
            BigInteger lambdaDiff = _ring.Mod((y2 - y1) * _ring.InvertMult(x2 - x1));
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
            BigInteger x3 = _ring.Mod(_ring.Square(lambda) - x1 - x2);
            BigInteger y3 = _ring.Mod(lambda * (x1 - x3) - y1);

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
            return new CurvePoint(p.X, _ring.Mod(-p.Y));
        }

        /// <summary>
        /// Selects one of two given curve points.
        /// 
        /// This allows side-channel resistant selection by avoiding branching.
        /// The selection is made based on the value of the parameter
        /// <paramref name="selection"/>. A value of <c>BigInteger.Zero</c>selects the curve point
        /// given as <paramref name="first"/>, a value of <c>BigInteger.One</c> selects <paramref name="second"/>.
        /// </summary>
        /// <returns>The selected curve point.</returns>
        /// <param name="selection">Selection indicator.</param>
        /// <param name="first">First selection option.</param>
        /// <param name="second">First selection option.</param>
        protected override CurvePoint Multiplex(BigInteger selection, CurvePoint first, CurvePoint second)
        {
            return CurvePoint.Multiplex(selection, first, second);
        }

        /// <inheritdocs/>
        protected override bool IsValidDerived(CurvePoint point)
        {
            if (!(BigInteger.Zero <= point.X && point.X < _parameters.P))
                return false;
            if (!(BigInteger.Zero <= point.Y && point.Y < _parameters.P))
                return false;

            // verifying that the point satisfies the curve equation
            BigInteger r = _ring.Mod(_ring.Pow(point.X, 3) + _parameters.A * point.X + _parameters.B);
            BigInteger ySquared = _ring.Square(point.Y);
            return (r == ySquared);
        }

        /// <summary>
        /// Restores a curve point from a byte representation.
        /// </summary>
        /// <param name="buffer">Byte array holding a representation of the curve point to restore.</param>
        /// <returns>The loaded curve point.</returns>
        public override CurvePoint FromBytes(byte[] buffer)
        {
            if (buffer.Length < 2 * _ring.ElementByteLength)
                throw new ArgumentException("The given buffer is too short to contain a valid element representation.", nameof(buffer));

            byte[] xBytes = new byte[_ring.ElementByteLength];
            byte[] yBytes = new byte[_ring.ElementByteLength];

            Buffer.BlockCopy(buffer, 0, xBytes, 0, _ring.ElementByteLength);
            Buffer.BlockCopy(buffer, _ring.ElementByteLength, yBytes, 0, _ring.ElementByteLength);

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

            Debug.Assert(xBytes.Length <= _ring.ElementByteLength);
            Debug.Assert(yBytes.Length <= _ring.ElementByteLength);

            byte[] result = new byte[2 * _ring.ElementByteLength];
            Buffer.BlockCopy(xBytes, 0, result, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, result, _ring.ElementByteLength, yBytes.Length);
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
    }
}
