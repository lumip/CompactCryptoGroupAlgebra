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

using System;
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
    /// The exact parameters of the curve (<c>A</c>, <c>B</c>, <c>P</c>) are
    /// encoded in a <see cref="CurveParameters"/> object.
    ///
    /// The x-coordinate-only specification is computationally more efficient (and elements require less
    /// storage) but does not have a well-defined addition for arbitrary points.
    /// <see cref="ICryptoGroupAlgebra{BigInteger, BigInteger}.Add(BigInteger, BigInteger)"/> and
    /// <see cref="ICryptoGroupAlgebra{BigInteger, BigInteger}.Negate(BigInteger)"/>
    /// are therefore not implemented (however, 
    /// <see cref="ICryptoGroupAlgebra{BigInteger, BigInteger}.MultiplyScalar(BigInteger, BigInteger)"/> is).
    /// If you require full addition on arbitrary points, use <see cref="CurveGroupAlgebra"/> with
    /// a <see cref="MontgomeryCurveEquation"/> instance.
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
        private readonly BigInteger _aConstant;
        private BigIntegerField Field { get { return _parameters.Equation.Field; } }

        /// <summary>
        /// Initializes a new instance of <see cref="XOnlyMontgomeryCurveAlgebra"/> 
        /// with given curve parameters.
        /// </summary>
        /// <param name="parameters">The parameters of the elliptic curve.</param>
        public XOnlyMontgomeryCurveAlgebra(CurveParameters parameters)
            : base(
                parameters.Generator.X,
                parameters.Order,
                parameters.Cofactor,
                BigInteger.Zero,
                NumberLength.GetLength(parameters.Equation.Field.Modulo).InBits
            )
        {
            _parameters = parameters;
            _aConstant = Field.Mod((_parameters.Equation.A + 2) * Field.InvertMult(4));
        }

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

            var xpPlusZp = Field.Mod(xp + zp);
            var xpMinusZp = Field.Mod(xp - zp);

            var xqPlusZq = Field.Mod(xq + zq);
            var xqMinusZq = Field.Mod(xq - zq);

            var firstProduct = Field.Mod(xpMinusZp * xqPlusZq);
            var secondProduct = Field.Mod(xpPlusZp * xqMinusZq);

            var xNew = Field.Mod(diff.Z * Field.Square(
                firstProduct + secondProduct
            ));

            var zNew = Field.Mod(diff.X * Field.Square(
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

            var xpPlusZpSquared = Field.Square(xp + zp);
            var xpMinusZpSquared = Field.Square(xp - zp);
            var xpzp4 = xpPlusZpSquared - xpMinusZpSquared;

            var xNew = Field.Mod(xpPlusZpSquared * xpMinusZpSquared);
            var zNew = Field.Mod(xpzp4 * (xpMinusZpSquared + _aConstant * xpzp4));

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
                Field.Mod(Field.InvertMult(point.Z) * point.X)
            );
        }

        /// <inheritdoc/>
        public override BigInteger Add(BigInteger left, BigInteger right)
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
            return Field.IsElement(point);
            
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
            }
            Debug.Assert(i == -1);
            return RenormalizePoint(r0).X;
        }

        /// <inheritdoc/>
        public override bool Equals(CryptoGroupAlgebra<BigInteger>? other)
        {
            return other is XOnlyMontgomeryCurveAlgebra algebra && _parameters.Equals(algebra._parameters);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -2051777468 + _parameters.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{BigInteger, BigInteger}" /> instance using a <see cref="XOnlyMontgomeryCurveAlgebra" />
        /// instance with the given <see cref="CurveParameters"/>.
        /// </summary>
        /// <param name="parameters">The parameters of the elliptic curve.</param>
        public static CryptoGroup<BigInteger, BigInteger> CreateCryptoGroup(CurveParameters parameters)
        {
            return new CryptoGroup<BigInteger, BigInteger>(
                new XOnlyMontgomeryCurveAlgebra(parameters)
            );
        }
    }
}
