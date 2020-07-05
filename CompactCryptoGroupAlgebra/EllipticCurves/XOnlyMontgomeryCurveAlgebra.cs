﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    
    /// <summary>
    /// An implementation of <see cref="ICryptoGroupAlgebra{E}"/> for
    /// Montgomery curves using x-only arithmetic on projected coordinates.
    /// 
    /// Note: Does not implement the standard addition of <see cref="ICryptoGroupAlgebra{E}.Add(E, E)"/>
    /// but only <see cref="ICryptoGroupAlgebra{E}.MultiplyScalar(E, BigInteger)"/>.
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
            throw new NotSupportedException("A projected Montgomery curve" +
            	"has no definition for the standard addition. Use the" +
            	"standard Montgomery curve implementation instead.");
        }

        /// <inheritdoc/>
        public override BigInteger FromBytes(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < _field.ElementByteLength)
                throw new ArgumentException("The given buffer is too short to" +
                	"contain a valid element representation.", nameof(buffer));

            byte[] xBytes = new byte[_field.ElementByteLength];

            Buffer.BlockCopy(buffer, 0, xBytes, 0, _field.ElementByteLength);

            BigInteger x = new BigInteger(xBytes);
            return x;
        }

        /// <inheritdoc/>
        public override byte[] ToBytes(BigInteger element)
        {
            byte[] xBytes = element.ToByteArray();

            Debug.Assert(xBytes.Length <= _field.ElementByteLength);

            return xBytes;
        }

        /// <inheritdoc/>
        public override BigInteger Negate(BigInteger point)
        {
            return point;
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
        /// Conditionally swaps the two given curve points.
        /// 
        /// This allows side-channel resistant selection by avoiding branching.
        /// The swap is made based on the value of the parameter
        /// <paramref name="swap"/>. A value of <c>BigInteger.Zero</c> means
        /// no swapping takes place, a value of <c>BigInteger.One</c> causes
        /// a swap.
        /// </summary>
        /// <returns>
        /// <c>(<paramref name="first"/>, <paramref name="second"/>)</c>
        /// if <paramref name="swap"/> is <c>false</c>; otherwise
        /// <c>(<paramref name="second"/>, <paramref name="first"/>)</c>
        /// </returns>
        /// <param name="swap">Swapping indicator.</param>
        /// <param name="first">Curve point.</param>
        /// <param name="second">Curve point.</param>
        private (MontgomeryCurvePoint, MontgomeryCurvePoint) ConditionalSwap(
            bool swap, MontgomeryCurvePoint first, MontgomeryCurvePoint second
        )
        {
            if (swap) return (second, first);
            return (first, second);
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

                (r0, r1) = ConditionalSwap(bitI.IsOne, r0, r1);
                r1 = XOnlyAdd(r0, r1, point);
                r0 = XOnlyDouble(r0);
                (r0, r1) = ConditionalSwap(bitI.IsOne, r0, r1);
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
            return other != null &&
                   EqualityComparer<CurveParameters>.Default.Equals(_parameters, algebra!._parameters);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -2051777468 + EqualityComparer<CurveParameters>.Default.GetHashCode(_parameters);
            return hashCode;
        }
    }
}
