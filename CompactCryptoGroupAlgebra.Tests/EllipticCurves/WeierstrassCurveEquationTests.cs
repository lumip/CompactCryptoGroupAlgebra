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

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class WeierstrassCurveEquationTests
    {
        [Test]
        public void TestAddDoublePoint()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = p.Clone();

            var expectedQ = new CurvePoint(15, 14);
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDoublePointAtInfinity()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = CurvePoint.PointAtInfinity;
            var other = p.Clone();

            var expectedQ = CurvePoint.PointAtInfinity;
            var q = curve.Add(p, other);

            Assert.AreEqual(expectedQ, q);
        }

        [Test]
        public void TestAddDifferentPoints()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = new CurvePoint(15, 14);

            var expected = new CurvePoint(16, 15);
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAddPointAtInfinityLeft()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = CurvePoint.PointAtInfinity;

            var result = curve.Add(other, p);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddPointAtInfinityRight()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = CurvePoint.PointAtInfinity;

            var result = curve.Add(p, other);

            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestAddNegated()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(5, 5);
            var other = curve.Negate(p);

            var expected = CurvePoint.PointAtInfinity;
            var result = curve.Add(p, other);

            Assert.AreEqual(expected, result);
        }
        

        [Test]
        public void TestAddAffine()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var p = new CurvePoint(11, 0);

            var q = curve.Add(p, p);

            Assert.AreEqual(CurvePoint.PointAtInfinity, q);
        }

        [Test]
        [TestCase(1, 10)]
        [TestCase(2, 17)]
        [TestCase(6, 11)]
        [TestCase(13, 8)]
        [TestCase(18, 3)]
        [TestCase(0, 20)]
        public void TestIsElementTrueForValidPoint(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsPointOnCurve(point));
        }

        [Test]
        public void TestIsElementFalseForPointAtInfinity()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            Assert.IsFalse(curve.IsPointOnCurve(CurvePoint.PointAtInfinity));
        }

        [Test]
        [TestCase(16, 1)]
        [TestCase(5, 2)]
        [TestCase(-2, 1)]
        [TestCase(16, -15)]
        [TestCase(78, 4)]
        [TestCase(4, 78)]
        public void TestIsElementFalseForPointNotOnCurve(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsFalse(curve.IsPointOnCurve(point));
        }

        [Test]
        [TestCase(10, 0)]
        public void TestIsElementTrueForLowOrderCurvePoint(int xRaw, int yRaw)
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var point = new CurvePoint(xRaw, yRaw);
            Assert.IsTrue(curve.IsPointOnCurve(point));
        }

        [Test]
        public void TestEqualsTrue()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = TestCurveParameters.WeierstrassParameters.Equation;

            bool result = curve.Equals(otherCurve);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;

            bool result = curve.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = new object();

            bool result = curve.Equals(otherCurve);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = new WeierstrassCurveEquation(prime: BigPrime.CreateWithoutChecks(11), 0, 0);

            bool result = curve.Equals(otherCurve);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var curve = TestCurveParameters.WeierstrassParameters.Equation;
            var otherCurve = TestCurveParameters.WeierstrassParameters.Equation;

            Assert.AreEqual(curve.GetHashCode(), otherCurve.GetHashCode());
        }

    }
}
