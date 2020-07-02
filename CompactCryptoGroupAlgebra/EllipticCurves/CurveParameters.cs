﻿using System;
using System.Numerics;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    /// <summary>
    /// A set of parameters of an elliptic curve with prime order over the
    /// finite field defined by prime P.
    /// </summary>
    public class CurveParameters
    {
        /// <summary>
        /// The prime P definining the finite field underlying the elliptic curve.
        /// </summary>
        public BigPrime P { get; }

        /// <summary>
        /// The parameter A in the curve equation.
        /// </summary>
        public BigInteger A { get; }

        /// <summary>
        /// The parameter B in the curve equation.
        /// </summary>
        public BigInteger B { get; }

        /// <summary>
        /// The order of the generator for the defined elliptic curve.
        /// </summary>
        public BigPrime Order { get; }

        /// <summary>
        /// A generator for the defined elliptic curve.
        /// </summary>
        public CurvePoint Generator { get; }

        /// <summary>
        /// The cofactor of the defined elliptic curve.
        ///
        /// The cofactor is the ratio of the number of points on the curve
        /// and the order of the subgroup of safe points generated by the generator.
        /// </summary>
        public BigInteger Cofactor { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveParameters"/> struct
        /// with the given values.
        /// </summary>
        /// <returns><see cref="CurveParameters"/> instance with the given
        /// parameters.</returns>
        /// <param name="p">Prime of the underlying field.</param>
        /// <param name="a">Curve parameter A.</param>
        /// <param name="b">Curve parameter B.</param>
        /// <param name="generator">Curve generator point.</param>
        /// <param name="order">Generator order.</param>
        /// <param name="cofactor">Curve cofactor.</param>
        public CurveParameters(
            BigPrime p,
            BigInteger a,
            BigInteger b,
            CurvePoint generator,
            BigPrime order,
            BigInteger cofactor
        )
        {
            P = p;
            A = a;
            B = b;
            Generator = generator;
            Order = order;
            Cofactor = cofactor;
        }

        /// <summary>
        /// Creates a parameter set for the NIST P-256 elliptic curve
        /// of form <c>y² = x³ + Ax + B</c>.
        /// </summary>
        /// <remarks>
        /// As defined in https://csrc.nist.gov/csrc/media/publications/fips/186/2/archive/2000-01-27/documents/fips186-2.pdf , p. 34.
        /// </remarks>
        /// <returns>An instance of CurveParameters for the NIST P-256 curve.</returns>
        public static CurveParameters NISTP256 = new CurveParameters(
            p:  BigPrime.CreateWithoutChecks(BigInteger.Parse(
                    "115792089210356248762697446949407573530086143415290314195533631308867097853951"
                )),
            a:  new BigInteger(-3),
            b:  new BigInteger(new byte[] {
                    0x4b, 0x60, 0xd2, 0x27, 0x3e, 0x3c, 0xce, 0x3b,
                    0xf6, 0xb0, 0x53, 0xcc, 0xb0, 0x06, 0x1d, 0x65,
                    0xbc, 0x86, 0x98, 0x76, 0x55, 0xbd, 0xeb, 0xb3,
                    0xe7, 0x93, 0x3a, 0xaa, 0xd8, 0x35, 0xc6, 0x5a,
                }),
            generator: new CurvePoint(
                    new BigInteger(new byte[] {
                        0x96, 0xc2, 0x98, 0xd8, 0x45, 0x39, 0xa1, 0xf4,
                        0xa0, 0x33, 0xeb, 0x2d, 0x81, 0x7d, 0x03, 0x77,
                        0xf2, 0x40, 0xa4, 0x63, 0xe5, 0xe6, 0xbc, 0xf8,
                        0x47, 0x42, 0x2c, 0xe1, 0xf2, 0xd1, 0x17, 0x6b,
                    }),
                    new BigInteger(new byte[] {
                        0xf5, 0x51, 0xbf, 0x37, 0x68, 0x40, 0xb6, 0xcb,
                        0xce, 0x5e, 0x31, 0x6b, 0x57, 0x33, 0xce, 0x2b,
                        0x16, 0x9e, 0x0f, 0x7c, 0x4a, 0xeb, 0xe7, 0x8e,
                        0x9b, 0x7f, 0x1a, 0xfe, 0xe2, 0x42, 0xe3, 0x4f,
                    })
                ),
            order: BigPrime.CreateWithoutChecks(BigInteger.Parse(
                    "115792089210356248762697446949407573529996955224135760342422259061068512044369")
                ),
            cofactor: 1
        );

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var parameters = obj as CurveParameters;
            return parameters != null &&
                   P.Equals(parameters.P) &&
                   A.Equals(parameters.A) &&
                   B.Equals(parameters.B) &&
                   Order.Equals(parameters.Order) &&
                   Generator.Equals(parameters.Generator) &&
                   Cofactor.Equals(parameters.Cofactor);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -1791766799;
            hashCode = hashCode * -1521134295 + P.GetHashCode();
            hashCode = hashCode * -1521134295 + A.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            hashCode = hashCode * -1521134295 + Order.GetHashCode();
            hashCode = hashCode * -1521134295 + Generator.GetHashCode();
            hashCode = hashCode * -1521134295 + Cofactor.GetHashCode();
            return hashCode;
        }
    }
}