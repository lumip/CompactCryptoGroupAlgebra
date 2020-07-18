namespace CompactCryptoGroupAlgebra.EllipticCurves
{

    /// <summary>
    /// Algebraic primitives for a specific elliptic curve equation type (e.g. Weierstrass or Montgomery curve equations).
    ///
    /// The curve equation is an equation of form <c>f(y) = g(x)</c> for some functions <c>f</c> and <c>g</c> that
    /// specifies the characteristics of an elliptic curve as well as the addition rule.
    /// </summary>
    public abstract class CurveEquation
    {
        // todo: manage the split of CurveParameters and CurveEquation better. Right now they are weirdly interdependent (in code and conceptually)
        // todo: could move all actually curve related parameters (A, B, P) into CurveEquation, keep Generator, Order, Cofactor in (Crypto)CurveParamaters which would also then know the CurveEquation instance

        /// <summary>
        /// The <see cref="CurveParameters"/> defining this <see cref="CurveEquation"/> instance.
        /// </summary>
        public CurveParameters CurveParameters { get; }
        
        /// <summary>
        /// The prime field over which the curve operates.
        /// </summary>
        public BigIntegerField Field { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CurveEquation"/> with given parameters.
        /// </summary>
        protected CurveEquation(CurveParameters parameters)
        {
            CurveParameters = parameters;
            Field = new BigIntegerField(CurveParameters.P);
        }

        /// <summary>
        /// Adds two points on the curve according to the addition rule specific
        /// for this <see cref="CurveEquation"/> instance.
        /// </summary>
        /// <param name="left">First point to add.</param>
        /// <param name="right">Second point to add.</param>
        /// <returns>Point on the curve that is the result of adding <paramref name="left"/> and <paramref name="right"/>.</returns>
        public abstract CurvePoint Add(CurvePoint left, CurvePoint right);

        /// <summary>
        /// Tests whether a given point lies on the curve.
        /// </summary>
        /// <param name="point"><see cref="CurvePoint"/> to test.</param>
        /// <returns><c>true</c> if <paramref name="point"/> satisifies the curve equation; otherwise <c>false</c></returns>
        public abstract bool IsPointOnCurve(CurvePoint point);

        /// <summary>
        /// Negates a given point on the curve.
        /// </summary>
        /// <param name="point"><see cref="CurvePoint"/> to negate.</param>
        /// <returns><see cref="CurvePoint"/> instance of the negation of <paramref name="point"/> on the curve.</returns>
        public virtual CurvePoint Negate(CurvePoint point)
        {
            BigIntegerField field = new BigIntegerField(CurveParameters.P);
            if (point.Equals(CurvePoint.PointAtInfinity))
                return point;
            return new CurvePoint(point.X, field.Mod(-point.Y));
        }

        /// <summary>
        /// Tests whether two given points on the curve are negations of each other.
        /// </summary>
        /// <param name="left">First point to check.</param>
        /// <param name="right">Second point to check.</param>
        /// <returns><c>true</c> if <paramref name="left"/> is a negation of <paramref name="right"/>; otherwise <c>false</c></returns>
        public bool AreNegations(CurvePoint left, CurvePoint right)
        {
            return Negate(right).Equals(left);
        }

        /// <summary>
        /// <see cref="CurveEquation"/> of the NIST P-256 Weierstrass-type curve.
        /// </summary>
        public static readonly CurveEquation NISTP256 = new WeierstrassCurveEquation(CurveParameters.NISTP256);

        /// <summary>
        /// <see cref="CurveEquation"/> of the Curve25519 Montgomery-type curve.
        /// </summary>
        public static readonly CurveEquation Curve25519 = new MontgomeryCurveEquation(CurveParameters.Curve25519);

    }
}