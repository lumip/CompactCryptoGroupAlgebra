using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    
    /// <summary>
    /// Cryptographic group based on point addition in elliptic curves in Montgomery form
    /// using x-coordinate-only arithmetics on projected coordinates.
    /// 
    /// Montgomery curves are of form <c>By² = x³ + Ax² + x</c>, with all numbers from the finite field with
    /// characteristic <c>P</c>. Elements of the groups are all points (<c>x mod P</c>, <c>y mod P</c>) that satisfy
    /// the curve equation (and the additional "point at infinity" as neutral element).
    ///
    /// The exact parameters of the curve (<c>A</c>, <c>B</c>, <c>P</c>) are encoded in a <see cref="CurveParameters"/> object.
    ///
    /// The x-coordinate-only specification is computationally more efficient (and elements require less
    /// storage) but does not have a well-defined addition for arbitrary points.
    /// <see cref="ICryptoGroupAlgebra{E}.Add(E, E)"/> and <see cref="ICryptoGroupAlgebra{E}.Negate(E)"/>
    /// are therefore not implemented (however, 
    /// <see cref="ICryptoGroupAlgebra{E}.MultiplyScalar(E, BigInteger)"/> is).
    /// If you require full addition on arbitrary points, use <see cref="MontgomeryCurveAlgebra"/>.
    /// 
    /// <see cref="XOnlyMontgomeryCurveAlgebra"/> returns <c>0</c> as its neutral element.
    /// This is not technically correct as a point with x-coordinate <c>0</c> exists on the curve,
    /// but that point is of low order and thus not admittable as safe curve element. For
    /// all implementation related considerations, <c>0</c> serves as representation of 
    /// the neutral element.
    ///
    /// Note that <see cref="XOnlyMontgomeryCurveAlgebra"/> does not implement RFC 7748 ( https://tools.ietf.org/html/rfc7748 )
    /// due to different handling/encoding of scalars.
    /// </summary>
    /// <remarks>
    /// Implementation based on https://eprint.iacr.org/2017/212.pdf .
    /// </remarks>
    public class XOnlyMontgomeryCurveAlgebra : CryptoGroupAlgebra<BigInteger>
    {
        private readonly CurveParameters _parameters;
        private BigIntegerField _field;
        private BigInteger _aConstant;

        /// <summary>
        /// Initializes a new instance of <see cref="XOnlyMontgomeryCurveAlgebra"/> 
        /// with given curve parameters.
        /// </summary>
        /// <param name="parameters">Curve parameters.</param>
        public XOnlyMontgomeryCurveAlgebra(CurveParameters parameters)
            : base(
                parameters.Generator.X,
                parameters.Order
            )
        {
            _parameters = parameters;
            _field = new BigIntegerField(_parameters.P);
            _aConstant = _field.Mod((_parameters.A + 2) * _field.InvertMult(4));
        }

        /// <inheritdoc/>
        public override BigInteger Cofactor { get { return _parameters.Cofactor; } }

        /// <inheritdoc/>
        public override int ElementBitLength { get { return NumberLength.GetLength(_parameters.P).InBits; } }

        /// <inheritdoc/>
        public override BigInteger NeutralElement { get { return BigInteger.Zero; } }

        /// <summary>
        /// Sums two projected Montgomery point using only x- and z-coordinates and knowledge of 
        /// the difference.
        /// </summary>
        /// <returns>The sum of <paramref name="left"/> and
        /// <paramref name="right"/>.</returns>
        /// <param name="left">Curve point to add.</param>
        /// <param name="right">Curve point to add.</param>
        /// <param name="diff">Difference of <paramref name="left"/> and
        /// <paramref name="right"/>.</param>
        private MontgomeryCurvePoint XOnlyAdd(MontgomeryCurvePoint left, MontgomeryCurvePoint right, MontgomeryCurvePoint diff)
        {
            if (right.IsAtInfinity)
                return left;
            if (left.IsAtInfinity)
                return right;

            BigInteger xp = left.X;
            BigInteger xq = right.X;
            BigInteger zp = left.Z;
            BigInteger zq = right.Z;

            var xpPlusZp = _field.Mod(xp + zp);
            var xpMinusZp = _field.Mod(xp - zp);

            var xqPlusZq = _field.Mod(xq + zq);
            var xqMinusZq = _field.Mod(xq - zq);

            var firstProduct = _field.Mod(xpMinusZp * xqPlusZq);
            var secondProduct = _field.Mod(xpPlusZp * xqMinusZq);

            var xNew = _field.Mod(diff.Z * _field.Square(
                firstProduct + secondProduct
            ));

            var zNew = _field.Mod(diff.X * _field.Square(
                firstProduct - secondProduct
            ));

            return new MontgomeryCurvePoint(xNew, zNew);
        }

        /// <summary>
        /// Doubles a projected Montgomery point using only x- and z-coordinates.
        /// </summary>
        /// <returns>The curve point.</returns>
        /// <param name="point">The doubled curve point.</param>
        private MontgomeryCurvePoint XOnlyDouble(MontgomeryCurvePoint point)
        {
            BigInteger xp = point.X;
            BigInteger zp = point.Z;

            var xpPlusZpSquared = _field.Square(xp + zp);
            var xpMinusZpSquared = _field.Square(xp - zp);
            var xpzp4 = xpPlusZpSquared - xpMinusZpSquared;

            var xNew = _field.Mod(xpPlusZpSquared * xpMinusZpSquared);
            var zNew = _field.Mod(xpzp4 * (xpMinusZpSquared + _aConstant * xpzp4));

            return new MontgomeryCurvePoint(xNew, zNew);
        }

        /// <summary>
        /// Renormalizes a Montgomery point by setting its z-coordinate to 1
        /// and scaling its x-coordinate accordingly.
        /// </summary>
        /// <returns>The curve point point.</returns>
        /// <param name="point">The renormalized curve point.</param>
        private MontgomeryCurvePoint RenormalizePoint(MontgomeryCurvePoint point)
        {
            return new MontgomeryCurvePoint(
                _field.Mod(_field.InvertMult(point.Z) * point.X)
            );
        }

        /// <inheritdoc/>
        public override BigInteger Add(BigInteger left,BigInteger right)
        {
            throw new NotSupportedException("An x-only Montgomery curve " +
            	"has no definition for the standard addition. Use the " +
            	"standard Montgomery curve implementation instead.");
        }

        /// <inheritdoc/>
        public override BigInteger FromBytes(byte[] buffer)
        {
            return new BigInteger(buffer);
        }

        /// <inheritdoc/>
        public override byte[] ToBytes(BigInteger element)
        {
            return element.ToByteArray();
        }

        /// <inheritdoc/>
        public override BigInteger Negate(BigInteger point)
        {
            throw new NotSupportedException("An x-only Montgomery curve " +
            	"has no definition for the standard negation. Use the " +
            	"standard Montgomery curve implementation instead.");
        }

        /// <inheritdoc/>
        protected override bool IsElementDerived(BigInteger point)
        {
            return _field.IsElement(point);
            
            // In Montgomery form, every x coordinate corresponds to a valid
            // point either on the curve or on its twist, i.e., another valid
            // curve. We do not really care about which of these two we compute
            // on (the projected x-only addition and multiplication work for
            // both), so we do not need to perform additional validity checks
            // here.
            // Note that subgroup attacks could be problematic, but those
            // are caught already in CryptoGroupAlgebra's IsElement implementation.
        }

        /// <summary>
        /// Selects one of two given BigInteger scalars.
        /// 
        /// This allows side-channel resistant selection by avoiding branching.
        /// The selection is made based on the value of the parameter
        /// <paramref name="selection"/>. A value of <c>BigInteger.Zero</c> selects the BigInteger
        /// given as <paramref name="first"/>, a value of <c>BigInteger.One</c> selects <paramref name="second"/>.
        /// </summary>
        /// <returns>The selected BigInteger.</returns>
        /// <param name="selection">Selection indicator.</param>
        /// <param name="first">First selection option.</param>
        /// <param name="second">Second selection option.</param>
        protected override BigInteger Multiplex(BigInteger selection, BigInteger first, BigInteger second)
        {
            Debug.Assert(selection.IsOne || selection.IsZero);
            return first + selection * (second - first);
        }

        /// <summary>
        /// Selects one of two given curve points.
        /// 
        /// This allows side-channel resistant selection by avoiding branching.
        /// The selection is made based on the value of the parameter
        /// <paramref name="selection"/>. A value of <c>BigInteger.Zero</c> selects the curve point
        /// given as <paramref name="first"/>, a value of <c>BigInteger.One</c> selects <paramref name="second"/>.
        /// </summary>
        /// <returns>The selected boolean.</returns>
        /// <param name="selection">Selection indicator.</param>
        /// <param name="first">First selection option.</param>
        /// <param name="second">First selection option.</param>
        private MontgomeryCurvePoint Multiplex(
            BigInteger selection, MontgomeryCurvePoint first, MontgomeryCurvePoint second
        )
        {
            Debug.Assert(selection.IsOne || selection.IsZero);
            var sel = !selection.IsZero;
            return new MontgomeryCurvePoint(
                Multiplex(selection, first.X, second.X),
                Multiplex(selection, first.Z, second.Z)
            );
        }

        /// <summary>
        /// Conditionally swaps the two given curve points.
        /// 
        /// This allows side-channel resistant selection by avoiding branching.
        /// The swap is made based on the value of the parameter
        /// <paramref name="selector"/>. A value of <c>BigInteger.Zero</c> means
        /// no swapping takes place, a value of <c>BigInteger.One</c> causes
        /// a swap.
        /// </summary>
        /// <returns>
        /// <c>(<paramref name="first"/>, <paramref name="second"/>)</c>
        /// if <paramref name="selector"/> is <c>0</c>; otherwise
        /// <c>(<paramref name="second"/>, <paramref name="first"/>)</c>
        /// </returns>
        /// <param name="selector">Swapping indicator.</param>
        /// <param name="first">Curve point.</param>
        /// <param name="second">Curve point.</param>
        private (MontgomeryCurvePoint, MontgomeryCurvePoint) ConditionalSwap(
            BigInteger selector, MontgomeryCurvePoint first, MontgomeryCurvePoint second
        )
        {
            Debug.Assert(selector.IsOne || selector.IsZero);
            return (
                Multiplex(selector, first, second),
                Multiplex(selector, second, first)
            );
        }

        /// <inheritdoc/>
        protected override BigInteger MultiplyScalarUnchecked(BigInteger e, BigInteger k, int factorBitLength)
        {
            BigInteger maxFactor = BigInteger.One << factorBitLength;
            int i = factorBitLength - 1;

            MontgomeryCurvePoint point = new MontgomeryCurvePoint(e);
            MontgomeryCurvePoint r0 = MontgomeryCurvePoint.PointAtInfinity;
            MontgomeryCurvePoint r1 = point;

            // Montgomery ladder maintains invariant r1 = r0 + e
            // and can thus use x-only addition that requires knowledge of r1-r0 (= e)
            for (BigInteger mask = maxFactor >> 1; !mask.IsZero; mask = mask >> 1, --i)
            {
                BigInteger bitI = (k & mask) >> i;

                (r0, r1) = ConditionalSwap(bitI, r0, r1);
                r1 = XOnlyAdd(r0, r1, point);
                r0 = XOnlyDouble(r0);
                (r0, r1) = ConditionalSwap(bitI, r0, r1);
                //if (bitI.IsZero)
                //{
                //    r1 = AddInternal(r0, r1, e);
                //    r0 = DoubleInternal(r0);
                //}
                //else
                //{
                //    r0 = AddInternal(r0, r1, e);
                //    r1 = DoubleInternal(r1);
                //}
            }
            Debug.Assert(i == -1);
            return RenormalizePoint(r0).X;
        }

        /// <inheritdoc/>
        public override bool Equals(CryptoGroupAlgebra<BigInteger>? other)
        {
            var algebra = other as XOnlyMontgomeryCurveAlgebra;
            return other != null && _parameters.Equals(algebra!._parameters);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -2051777468 + _parameters.GetHashCode();
            return hashCode;
        }
    }
}
