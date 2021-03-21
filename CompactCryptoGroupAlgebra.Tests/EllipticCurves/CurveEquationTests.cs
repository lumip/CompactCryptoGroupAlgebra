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

using System.Numerics;

using NUnit.Framework;
using Moq;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{

    [TestFixture]
    public class CurveEquationTests
    {

        [Test]
        public void TestNegate()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(5, 5);
            var expected = new CurvePoint(5, 18);

            var result = equation.Object.Negate(testElement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestNegateForPointAtInfinity()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = CurvePoint.PointAtInfinity;
            var expected = CurvePoint.PointAtInfinity;

            var result = equation.Object.Negate(testElement);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestAreNegationsFalseForEqualPoint()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(5, 5);
            var otherElement = testElement.Clone();

            Assert.IsFalse(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForNegation()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(5, 5);
            var otherElement = equation.Object.Negate(testElement);

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForZeroYPoint()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = new CurvePoint(11, 0);
            var otherElement = testElement.Clone();

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestAreNegationsTrueForPointAtInfinity()
        {
            var equation = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var testElement = CurvePoint.PointAtInfinity;
            var otherElement = testElement.Clone();

            Assert.IsTrue(equation.Object.AreNegations(testElement, otherElement));
        }

        [Test]
        public void TestEqualsTrue()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            Assert.IsTrue(equationMock.Object.Equals(otherMock.Object));
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            Assert.IsFalse(equationMock.Object.Equals(null));
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            Assert.IsFalse(equationMock.Object.Equals(new object()));
        }

        [Test]
        public void TestEqualsFalseForDifferentObject()
        {
            var equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };

            var otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(11),
                BigInteger.Zero, BigInteger.One
            ) { CallBase = true };
            Assert.IsFalse(equationMock.Object.Equals(otherMock.Object));

            otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.One, BigInteger.One
            ) { CallBase = true };
            Assert.IsFalse(equationMock.Object.Equals(otherMock.Object));

            otherMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(23),
                BigInteger.Zero, BigInteger.Zero
            ) { CallBase = true };
            Assert.IsFalse(equationMock.Object.Equals(otherMock.Object));
        }
    }
}
