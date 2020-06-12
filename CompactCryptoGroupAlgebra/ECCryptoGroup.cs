using System;
namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Cryptographic group based on point addition in elliptic curves in Weierstrass form.
    /// 
    /// Weierstrass curves are of form <c>y² = x³ + Ax + B</c>, with all numbers from finite field defined
    /// by a prime number <c>P</c>. Elements of the groups are all points (x mod P, y mod P) that satisfy
    /// the curve equation (and the addtional "point at infinity" as neutral element).
    /// </summary>
    public class ECCryptoGroup : CryptoGroup<ECPoint>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ECCryptoGroup"/>
        /// given a <see cref="ICryptoGroupAlgebra{ECPoint}"/> which provides an
        /// implementation for underlying group operations and has already been
        /// initialized group parameters.
        /// </summary>
        /// <param name="algebra">The <see cref="ICryptoGroupAlgebra{E}"/> instance.</param>
        public ECCryptoGroup(ICryptoGroupAlgebra<ECPoint> algebra) : base(algebra)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="MultiplicativeCryptoGroup"/>
        /// given the curve's parameters.
        /// </summary>
        /// <param name="curveParameters">A <see cref="ECParameters"/> instance holding valid curve parameters.</param>
        public ECCryptoGroup(ECParameters curveParameters)
          : this(new ECGroupAlgebra(curveParameters))
        { }

        /// <inheritdoc/>
        protected override CryptoGroupElement<ECPoint> CreateGroupElement(ECPoint value)
        {
            return new CryptoGroupElement<ECPoint>(value, (ECGroupAlgebra)Algebra);
        }

        /// <inheritdoc/>
        protected override CryptoGroupElement<ECPoint> CreateGroupElement(byte[] buffer)
        {
            return new CryptoGroupElement<ECPoint>(buffer, (ECGroupAlgebra)Algebra);
        }
    }
}
