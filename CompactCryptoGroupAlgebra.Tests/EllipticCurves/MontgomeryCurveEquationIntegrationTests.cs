// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2020-2021 Lukas Prediger <lumip@lumip.de>
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

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class MontgomeryCurveEquationIntegrationTests
    {
        private static readonly CurveParameters curveParameters = TestCurveParameters.MontgomeryParameters;

        [Test]
        [TestCase(1, 2, 6, 2, 6)]
        [TestCase(2, 2, 6, 6, 9)]
        [TestCase(6, 2, 6, 15, 6)]
        [TestCase(8, 2, 6, 30, 40)]
        [TestCase(9, 2, 6, 18, 39)]
        [TestCase(10, 2, 6, 28, 7)]
        public void TestMultiplyScalar(int k, int x, int y, int expectedX, int expectedY)
        {
            var curveAlgebra = new CurveGroupAlgebra(curveParameters);
            var expected = new CurvePoint(new BigInteger(expectedX), new BigInteger(expectedY));

            var p = new CurvePoint(new BigInteger(x), new BigInteger(y));
            var result = curveAlgebra.MultiplyScalar(p, k);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curveAlgebra = new CurveGroupAlgebra(curveParameters);
            var p = new CurvePoint(2, 6);
            var result = curveAlgebra.MultiplyScalar(p, 11);

            Assert.AreEqual(curveAlgebra.NeutralElement, result);
        }

        [Test]
        [TestCase(38, 24)]
        [TestCase(2, 6)]
        [TestCase(18, 39)]
        public void TestIsElementForValidPoint(int xRaw, int yRaw)
        {
            var curveAlgebra = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curveAlgebra.IsPotentialElement(point), "IsPotentialElement not true for valid element!");
            Assert.IsTrue(curveAlgebra.IsSafeElement(point), "IsSafeElement not true for valid element!");
        }

        [Test]
        public void TestIsElementForPointAtInfinity()
        {
            var curveAlgebra = new CurveGroupAlgebra(curveParameters);
            Assert.IsTrue(curveAlgebra.IsPotentialElement(CurvePoint.PointAtInfinity), "IsPotentialElement not true for point at infinity!");
            Assert.IsFalse(curveAlgebra.IsSafeElement(CurvePoint.PointAtInfinity), "IsSafeElement not false for point at infinity!");
        }

        [Test]
        [TestCase(3, 1)]
        [TestCase(1, 6)]
        [TestCase(-2, 1)]
        [TestCase(1, -4)]
        [TestCase(25, 7)]
        public void TestIsElementForPointNotOnCurve(int xRaw, int yRaw)
        {
            var curveAlgebra = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curveAlgebra.IsPotentialElement(point), "IsPotentialElement not true for point not on curve!");
            Assert.IsFalse(curveAlgebra.IsSafeElement(point), "IsSafeElement not true for point not on curve!");
        }

        [Test]
        [TestCase(0, 0)]
        public void TestIsElementForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curveAlgebra = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curveAlgebra.IsPotentialElement(point), "IsPotentialElement not true for low order point!");
            Assert.IsFalse(curveAlgebra.IsSafeElement(point), "IsSafeElement not false for low order point!");
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var invalidGenerator = new CurvePoint(0, 0);
            CurveParameters invalidParameters = new CurveParameters(
                curveEquation: curveParameters.Equation,
                generator: invalidGenerator,
                order: curveParameters.Order,
                cofactor: curveParameters.Cofactor
            );
                
            Assert.Throws<ArgumentException>(
                () => new CurveGroupAlgebra(invalidParameters)
            );
        }
    }
}
