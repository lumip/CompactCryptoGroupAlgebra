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
    public class XOnlyMontgomeryCurveAlgebraTests
    {
        private readonly CurveParameters curveParameters = TestCurveParameters.MontgomeryParameters;
        private readonly CurveParameters largeParameters;

        public XOnlyMontgomeryCurveAlgebraTests()
        {
            largeParameters = new CurveParameters(
                curveEquation: new MontgomeryCurveEquation(
                        prime: BigPrime.CreateWithoutChecks(18392027), // 25 bits
                        a: 0, b: 0
                    ),
                generator: CurvePoint.PointAtInfinity,
                order: BigPrime.CreateWithoutChecks(3),
                cofactor: 1
            );
        }

        [Test]
        [TestCase(0, 2, 0)]
        [TestCase(1, 2, 2)]
        [TestCase(2, 2, 6)]
        [TestCase(3, 2, 23)]
        [TestCase(4, 2, 38)]
        [TestCase(5, 2, 8)]
        [TestCase(6, 2, 15)]
        [TestCase(7, 2, 20)]
        [TestCase(8, 2, 30)]
        [TestCase(9, 2, 18)]
        [TestCase(10, 2, 28)]
        [TestCase(11, 2, 0)]
        public void TestMultiplyScalar(int k, int x, int expectedX)
        {
            var algebra = new XOnlyMontgomeryCurveAlgebra(curveParameters);

            var p = new BigInteger(x);
            var result = algebra.MultiplyScalar(p, k);

            Assert.AreEqual(new BigInteger(expectedX), result);
        }

        [Test]
        public void TestConstructorAndProperties()
        {
            var algebra = new XOnlyMontgomeryCurveAlgebra(curveParameters);

            Assert.AreEqual(curveParameters.Generator.X, algebra.Generator);
            Assert.AreEqual(curveParameters.Order, algebra.Order);
            Assert.AreEqual(curveParameters.Cofactor, algebra.Cofactor);
            Assert.AreEqual(NumberLength.GetLength(curveParameters.Equation.Field.Modulo).InBits, algebra.ElementBitLength);
            Assert.AreEqual(algebra.OrderBitLength / 2, algebra.SecurityLevel);
        }

        [Test]
        public void TestAddThrowsNotSupported()
        {
            var point = new BigInteger(2);
            var otherPoint = new BigInteger(6);
            var algebra = new XOnlyMontgomeryCurveAlgebra(curveParameters);

            Assert.Throws<NotSupportedException>(
                () => algebra.Add(point, otherPoint)
            );
        }

        [Test]
        public void TestNegateThrowsNotSupported()
        {
            var point = new BigInteger(2);
            var algebra = new XOnlyMontgomeryCurveAlgebra(curveParameters);

            Assert.Throws<NotSupportedException>(
                () => algebra.Negate(point)
            );
        }


        [Test]
        public void TestFromBytes()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(largeParameters);
            var expected = new BigInteger(5);
            var buffer = new byte[] { 5 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytes()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(largeParameters);
            var p = new BigInteger(5);
            var expected = new byte[] { 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var expected = new BigInteger(5);
            var buffer = new byte[] { 5 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var p = new BigInteger(5);
            var expected = new byte[] { 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestIsElement()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var p = new BigInteger(7);

            Assert.IsTrue(curve.IsPotentialElement(p));
            Assert.IsTrue(curve.IsSafeElement(p));
        }

        [Test]
        public void TestIsElementForLowOrderElements()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var p = new BigInteger(0);
            
            Assert.IsTrue(curve.IsPotentialElement(p));
            Assert.IsFalse(curve.IsSafeElement(p));
        }

        [Test]
        public void TestEqualsTrueForEqual()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var otherCurve = new XOnlyMontgomeryCurveAlgebra(curveParameters);

            Assert.IsTrue(curve.Equals(otherCurve));
        }

        [Test]
        public void TestEqualsFalseForDifferent()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var otherCurve = new XOnlyMontgomeryCurveAlgebra(largeParameters);

            Assert.IsFalse(curve.Equals(otherCurve));
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);

            Assert.IsFalse(curve.Equals(null));
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var curve = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var otherCurve = new XOnlyMontgomeryCurveAlgebra(curveParameters);

            Assert.AreEqual(curve.GetHashCode(), otherCurve.GetHashCode());
        }

        [Test]
        public void TestCreateCryptoGroup()
        {
            var expectedGroupAlgebra = new XOnlyMontgomeryCurveAlgebra(curveParameters);
            var group = XOnlyMontgomeryCurveAlgebra.CreateCryptoGroup(curveParameters);
            Assert.AreEqual(expectedGroupAlgebra, group.Algebra);
        }        

    }
}
