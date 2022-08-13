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

using NUnit.Framework;
using Moq;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class CurveGroupAlgebraTests
    {

        private readonly CurveParameters curveParameters;
        private readonly CurveParameters largeParameters;

        private readonly Mock<CurveEquation> curveEquationMock;
        private readonly Mock<CurveEquation> largeCurveEquationMock;

        public CurveGroupAlgebraTests()
        {
            curveEquationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero,
                BigInteger.One
            ) { CallBase = true };
            curveParameters = new CurveParameters(
                curveEquation: curveEquationMock.Object,
                generator: TestCurveParameters.WeierstrassParameters.Generator,
                order: TestCurveParameters.WeierstrassParameters.Order,
                cofactor: TestCurveParameters.WeierstrassParameters.Cofactor
            );

            largeCurveEquationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(18392027),
                BigInteger.Zero,
                BigInteger.One
            ) { CallBase = true };  // 25 bits prime
            largeParameters = new CurveParameters(
                curveEquation: largeCurveEquationMock.Object,
                generator: new CurvePoint(),
                order: BigPrime.CreateWithoutChecks(2),
                cofactor: BigInteger.One
            );
        }

        [SetUp]
        public void Setup()
        {
            curveEquationMock.Reset();
            curveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(true);
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));

            largeCurveEquationMock.Reset();
            largeCurveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(true);
            largeCurveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));
        }

        [Test]
        public void TestAdd()
        {
            var curve = new CurveGroupAlgebra(curveParameters);

            var point = new CurvePoint(5, 5);
            var other = new CurvePoint(15, 14);
            var expected = new CurvePoint(20, 19);

            curveEquationMock.Reset();
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));

            var result = curve.Add(point, other);
            Assert.AreEqual(expected, result);
            curveEquationMock.Verify(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()), Times.Once);
        }

        [Test]
        public void TestIsElementForValidPoint()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(2, 17);
            Assert.IsTrue(curve.IsPotentialElement(point), "IsPotentialElement not true for valid element!");
            Assert.IsTrue(curve.IsSafeElement(point), "IsSafeElement not true for valid element!");
        }

        [Test]
        public void TestIsElementForPointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>())).Returns(CurvePoint.PointAtInfinity);
            Assert.IsTrue(curve.IsPotentialElement(CurvePoint.PointAtInfinity), "IsPotentialElement not true for point at infinity!");
            Assert.IsFalse(curve.IsSafeElement(CurvePoint.PointAtInfinity), "IsSafeElement not false for point at infinity!");
        }

        [Test]
        public void TestIsElementForPointAtInfinityCofactorOne()
        {
            var parameters = new CurveParameters(
                curveParameters.Equation,
                curveParameters.Generator,
                curveParameters.Order,
                BigInteger.One
            );
            var curve = new CurveGroupAlgebra(parameters);
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>())).Returns(CurvePoint.PointAtInfinity);
            Assert.IsTrue(curve.IsPotentialElement(CurvePoint.PointAtInfinity), "IsPotentialElement not true for point at infinity!");
            Assert.IsFalse(curve.IsSafeElement(CurvePoint.PointAtInfinity), "IsSafeElement not false for point at infinity!");
        }

        [Test]
        public void TestIsElementForPointNotOnCurve()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            curveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(false);
            var point = new CurvePoint(16, 1);
            Assert.IsFalse(curve.IsPotentialElement(point), "IsPotentialElement not true for point not on curve!");
            Assert.IsFalse(curve.IsSafeElement(point), "IsSafeElement not true for point not on curve!");
        }

        [Test]
        public void TestIsElementForLowOrderCurvePoint()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(10, 0);

            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>())).Returns(CurvePoint.PointAtInfinity);
            Assert.IsTrue(curve.IsPotentialElement(point), "IsPotentialElement not true for low order point!");
            Assert.IsFalse(curve.IsSafeElement(point), "IsSafeElement not false for low order point!");
        }

        [Test]
        public void TestNegate()
        {
            var curve = new CurveGroupAlgebra(curveParameters);

            var p = new CurvePoint(5, 5);
            var expected = new CurvePoint(5, 18);

            var result = curve.Negate(p);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForZeroYPoint()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var p = new CurvePoint(11, 0);

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestNegatePointAtInfinity()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var p = CurvePoint.PointAtInfinity;

            var result = curve.Negate(p);
            Assert.AreEqual(p, result);
            Assert.AreNotSame(p, result);
        }

        [Test]
        public void TestMultiplyScalar()
        {
            var k = new BigInteger(5);
            
            var curve = new CurveGroupAlgebra(curveParameters);
            var point = new CurvePoint(5, 5);

            curveEquationMock.Reset();
            curveEquationMock.Setup(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()))
                .Returns((CurvePoint p, CurvePoint q) => new CurvePoint(p.X + q.X, p.Y + q.Y));

            var expectedQ = new CurvePoint(25, 25);
            var otherPoint = curve.MultiplyScalar(point, k);

            Assert.AreEqual(expectedQ, otherPoint);
            curveEquationMock.Verify(eq => eq.Add(It.IsAny<CurvePoint>(), It.IsAny<CurvePoint>()), Times.Exactly(2*4)); // 2 * Order bit length
        }

        [Test]
        public void TestGroupElementBitLength()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.AreEqual(2*5, curve.ElementBitLength);
        }

        [Test]
        public void TestOrderIsAsSet()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.AreEqual(curveParameters.Order, curve.Order);
        }

        [Test]
        public void TestOrderBitLength()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.AreEqual(4, curve.OrderBitLength);
        }
        
        [Test]
        public void TestSecurityLevel()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.AreEqual(curve.OrderBitLength / 2, curve.SecurityLevel);
        }

        [Test]
        public void TestNeutralElement()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.AreEqual(CurvePoint.PointAtInfinity, curve.NeutralElement);
        }

        [Test]
        public void TestGeneratorIsAsSet()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.AreEqual(curveParameters.Generator, curve.Generator);
        }

        [Test]
        public void TestCofactor()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            Assert.AreEqual(curveParameters.Cofactor, curve.Cofactor);
        }

        [Test]
        public void TestFromBytes()
        {
            var curve = new CurveGroupAlgebra(largeParameters);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesRejectsTooShortBuffer()
        {
            var curve = new CurveGroupAlgebra(largeParameters);
            var buffer = new byte[7];
            Assert.Throws<ArgumentException>(
                () => curve.FromBytes(buffer)
            );
        }

        [Test]
        public void TestToBytes()
        {
            var curve = new CurveGroupAlgebra(largeParameters);
            var p = new CurvePoint(5, 3);
            var expected = new byte[] { 5, 0, 0, 0, 3, 0, 0, 0 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesWithLessThanOneByteLargeElements()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var expected = new CurvePoint(5, 3);
            var buffer = new byte[] { 5, 3 };

            var result = curve.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytesWithLessThanOneByteLargeElements()
        {
            var curve = new CurveGroupAlgebra(curveParameters);
            var p = new CurvePoint(5, 5);
            var expected = new byte[] { 5, 5 };

            var result = curve.ToBytes(p);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestInvalidElementRejectedAsGenerator()
        {
            curveEquationMock.Setup(eq => eq.IsPointOnCurve(It.IsAny<CurvePoint>())).Returns(false);
                
            Assert.Throws<ArgumentException>(
                () => new CurveGroupAlgebra(curveParameters)
            );
        }

        [Test]
        public void TestEqualsTrue()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveParameters);
            var otherAlgebra = new CurveGroupAlgebra(curveParameters);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveParameters);

            bool result = groupAlgebra.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveParameters);
            var otherAlgebra = new object();

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveParameters);
            var otherAlgebra = new CurveGroupAlgebra(largeParameters);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var groupAlgebra = new CurveGroupAlgebra(curveParameters);
            var otherAlgebra = new CurveGroupAlgebra(curveParameters);

            Assert.AreEqual(groupAlgebra.GetHashCode(), otherAlgebra.GetHashCode());
        }

        [Test]
        public void TestCreateCryptoGroup()
        {
            var expectedGroupAlgebra = new CurveGroupAlgebra(curveParameters);
            var group = CurveGroupAlgebra.CreateCryptoGroup(curveParameters);
            Assert.AreEqual(expectedGroupAlgebra, group.Algebra);
        }

        [Test]
        [TestCase(126)]
        [TestCase(128)]
        [TestCase(140)]
        [TestCase(190)]
        [TestCase(200)]
        [TestCase(254)]
        [TestCase(260)]
        public void TestCreateCryptoGroupWithSecurityLevel(int securityLevel)
        {
            var group = CurveGroupAlgebra.CreateCryptoGroup(securityLevel);
            Assert.That(group.SecurityLevel >= securityLevel);
        }

        [Test]
        public void TestCreateCryptoGroupWithTooHighSecurityLevel()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CurveGroupAlgebra.CreateCryptoGroup(261));
        }

    }
}
