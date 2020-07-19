using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    
    /// <summary>
    /// Cryptographic group based on point addition in elliptic curves.
    /// 
    /// The exact form of the curve and its characteristics are determined by an implementation
    /// of <see cref="CurveEquation"/>. 
    /// 
    /// Elements of the group are all points (<c>x mod P</c>, <c>y mod P</c>) that satisfy
    /// the curve equation and are of group order (to prevent small subgroup attacks).
    /// </summary>
    public sealed class CurveGroupAlgebra : CryptoGroupAlgebra<CurvePoint>
    {
        
        private readonly CurveEquation _curveEquation;
        private BigIntegerField Field { get { return _curveEquation.Field; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveGroupAlgebra"/> class.
        /// </summary>
        /// <param name="parameters">The parameters of the elliptic curve.</param>
        public CurveGroupAlgebra(CurveParameters parameters)
            : base(
                parameters.Generator,
                parameters.Order,
                parameters.Cofactor,
                CurvePoint.PointAtInfinity,
                2 * NumberLength.GetLength(parameters.Equation.Field.Modulo).InBits
            )
        {
            _curveEquation = parameters.Equation;
            if (!IsElement(Generator))
                throw new ArgumentException("The point given as generator is " +
                	"not a valid point on the curve.", nameof(parameters));
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
            return _curveEquation.Add(left, right);
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
            return _curveEquation.Negate(p);
        }

        /// <inheritdocs/>
        protected override bool IsElementDerived(CurvePoint point)
        {
            if (!Field.IsElement(point.X) || !Field.IsElement(point.Y))
                return false;

            // verifying that the point satisfies the curve equation
            return _curveEquation.IsPointOnCurve(point);
        }

        /// <summary>
        /// Restores a curve point from a byte representation.
        /// </summary>
        /// <param name="buffer">Byte array holding a representation of the curve point to restore.</param>
        /// <returns>The loaded curve point.</returns>
        public override CurvePoint FromBytes(byte[] buffer)
        {
            if (buffer.Length < 2 * Field.ElementByteLength)
                throw new ArgumentException("The given buffer is too short to contain a valid element representation.", nameof(buffer));

            byte[] xBytes = new byte[Field.ElementByteLength];
            byte[] yBytes = new byte[Field.ElementByteLength];

            Buffer.BlockCopy(buffer, 0, xBytes, 0, Field.ElementByteLength);
            Buffer.BlockCopy(buffer, Field.ElementByteLength, yBytes, 0, Field.ElementByteLength);

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

            Debug.Assert(xBytes.Length <= Field.ElementByteLength);
            Debug.Assert(yBytes.Length <= Field.ElementByteLength);

            byte[] result = new byte[2 * Field.ElementByteLength];
            Buffer.BlockCopy(xBytes, 0, result, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, result, Field.ElementByteLength, yBytes.Length);
            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(CryptoGroupAlgebra<CurvePoint>? other)
        {
            var algebra = other as CurveGroupAlgebra;
            return algebra != null! && _curveEquation.Equals(algebra._curveEquation);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -1453010210 + _curveEquation.GetHashCode();
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{CurvePoint}" /> instance using a <see cref="CurveGroupAlgebra" />
        /// instance with the given <see cref="CurveParameters"/>.
        /// </summary>
        /// <param name="parameters">The parameters of the elliptic curve.</param>
        public static CryptoGroup<CurvePoint> CreateCryptoGroup(CurveParameters parameters)
        {
            return new CryptoGroup<CurvePoint>(new CurveGroupAlgebra(parameters));
        }
    }
}
