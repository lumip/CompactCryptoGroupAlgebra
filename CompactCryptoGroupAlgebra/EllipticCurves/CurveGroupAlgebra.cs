// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2022 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Numerics;
using System.Diagnostics;

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

        /// <inheritdoc/>
        public override int SecurityLevel => OrderBitLength / 2;

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
            if (!IsSafeElement(Generator))
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

        /// <inheritdoc/>
        protected override bool IsElementDerived(CurvePoint point)
        {
            if (!Field.IsElement(point.X) || !Field.IsElement(point.Y))
                return false;

            // verifying that the point satisfies the curve equation
            return point.IsAtInfinity || _curveEquation.IsPointOnCurve(point);
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
        /// Creates a <see cref="CryptoGroup{BigInteger, CurvePoint}" /> instance using a <see cref="CurveGroupAlgebra" />
        /// instance with the given <see cref="CurveParameters"/>.
        /// </summary>
        /// <param name="parameters">The parameters of the elliptic curve.</param>
        public static CryptoGroup<BigInteger, CurvePoint> CreateCryptoGroup(CurveParameters parameters)
        {
            return new CryptoGroup<BigInteger, CurvePoint>(new CurveGroupAlgebra(parameters));
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{BigInteger, CurvePoint}" /> instance at least satisfying a given security level.
        /// </summary>
        /// <param name="securityLevel">The minimal security level for the curve to be created.</param>
        public static CryptoGroup<BigInteger, CurvePoint> CreateCryptoGroup(int securityLevel)
        {
            CurveParameters parameters;
            if (securityLevel <= 126)
            {
                parameters = CurveParameters.Curve25519;
            }
            else if (securityLevel <= 128)
            {
                parameters = CurveParameters.NISTP256;
            }
            else if (securityLevel <= 190)
            {
                parameters = CurveParameters.M383;
            }
            else if (securityLevel <= 254)
            {
                parameters = CurveParameters.M511;
            }
            else if (securityLevel <= 260)
            {
                parameters = CurveParameters.NISTP521;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    $"There are no pre-configured curves that satisfy a security level of {securityLevel}.", nameof(securityLevel)
                );
            }

            return CreateCryptoGroup(parameters);
        }

    }
}
