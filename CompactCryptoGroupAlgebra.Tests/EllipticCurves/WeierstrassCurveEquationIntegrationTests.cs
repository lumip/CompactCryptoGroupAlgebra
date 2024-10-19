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
    public class WeierstrassCurveEquationIntegrationTests
    {
        private readonly CurveParameters curveParameters = TestCurveParameters.WeierstrassParameters;

        [Test]
        [TestCase(1, 10)]
        [TestCase(2, 17)]
        [TestCase(6, 11)]
        [TestCase(13, 8)]
        [TestCase(18, 3)]
        [TestCase(0, 20)]
        public void TestIsElementForValidPoint(int rawX, int rawY)
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(rawX, rawY);
            Assert.IsTrue(curve.IsPotentialElement(point), "IsPotentialElement not true for valid element!");
            Assert.IsTrue(curve.IsSafeElement(point), "IsSafeElement not true for valid element!");
        }

        [Test]
        public void TestIsElementForPointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.IsTrue(curve.IsPotentialElement(CurvePoint.PointAtInfinity), "IsPotentialElement not true for point at infinity!");
            Assert.IsFalse(curve.IsSafeElement(CurvePoint.PointAtInfinity), "IsSafeElement not false for point at infinity!");
        }

        [Test]
        [TestCase(16, 1)]
        [TestCase(5, 2)]
        [TestCase(-2, 1)]
        [TestCase(16, -15)]
        [TestCase(78, 4)]
        [TestCase(4, 78)]
        public void TestIsElementForPointNotOnCurve(int rawX, int rawY)
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(rawX, rawY);
            Assert.IsFalse(curve.IsPotentialElement(point), "IsPotentialElement not true for point not on curve!");
            Assert.IsFalse(curve.IsSafeElement(point), "IsSafeElement not true for point not on curve!");
        }

        [Test]
        public void TestIsElementForLowOrderCurvePoint()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(10, 0);
            Assert.IsTrue(curve.IsPotentialElement(point), "IsPotentialElement not true for low order point!");
            Assert.IsFalse(curve.IsSafeElement(point), "IsSafeElement not false for low order point!");
        }

        [Test]
        [TestCase(2, 15, 14)]
        [TestCase(5, 16, 8)]
        [TestCase(7, 5, 18)]
        [TestCase(9, 5, 5)]
        public void TestMultiplyScalar(int kRaw, int expectedX, int expectedY)
        {
            var k = new BigInteger(kRaw);
            var curve = new CurveGroupAlgebra(curveParameters);
            var p = new CurvePoint(5, 5);

            var q = curve.MultiplyScalar(p, k);
            var expectedQ = new CurvePoint(expectedX, expectedY);
            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestMultiplyScalarOrderResultsInNeutralElement()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var p = new CurvePoint(5, 3);
            var result = curve.MultiplyScalar(p, 11);

            Assert.AreEqual(curve.NeutralElement, result);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            var invalidGenerator = new CurvePoint(16, 1);
            CurveParameters invalidParameters = new CurveParameters(
                curveEquation: TestCurveParameters.WeierstrassParameters.Equation,
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
