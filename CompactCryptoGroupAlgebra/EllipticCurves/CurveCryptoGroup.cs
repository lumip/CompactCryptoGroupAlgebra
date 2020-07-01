using System;
namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// Cryptographic group based on point addition in elliptic curves in Weierstrass form.
    /// 
    /// Weierstrass curves are of form <c>y² = x³ + Ax + B</c>, with all numbers from finite field defined
    /// by a prime number <c>P</c>. Elements of the groups are all points (x mod P, y mod P) that satisfy
    /// the curve equation (and the addtional "point at infinity" as neutral element).
    /// </summary>
    public class CurveCryptoGroup : CryptoGroup<CurvePoint>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CurveCryptoGroup"/>
        /// given a <see cref="ICryptoGroupAlgebra{CurvePoint}"/> which provides an
        /// implementation for underlying group operations and has already been
        /// initialized group parameters.
        /// </summary>
        /// <param name="algebra">The <see cref="ICryptoGroupAlgebra{T}"/> instance.</param>
        public CurveCryptoGroup(ICryptoGroupAlgebra<CurvePoint> algebra) : base(algebra)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="MultiplicativeCryptoGroup"/>
        /// given the curve's parameters.
        /// </summary>
        /// <param name="curveParameters">A <see cref="CurveParameters"/> instance holding valid curve parameters.</param>
        public CurveCryptoGroup(CurveParameters curveParameters)
          : this(new CurveGroupAlgebra(curveParameters))
        { }

        /// <inheritdoc/>
        protected override CryptoGroupElement<CurvePoint> CreateGroupElement(CurvePoint value)
        {
            return new CryptoGroupElement<CurvePoint>(value, (CurveGroupAlgebra)Algebra);
        }

        /// <inheritdoc/>
        protected override CryptoGroupElement<CurvePoint> CreateGroupElement(byte[] buffer)
        {
            return new CryptoGroupElement<CurvePoint>(buffer, (CurveGroupAlgebra)Algebra);
        }
    }
}
