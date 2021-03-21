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
    public class CurveParametersTests
    {

        private readonly Mock<CurveEquation> equationMock;

        public CurveParametersTests()
        {
            equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(11),
                new BigInteger(2),
                new BigInteger(-3)
            );
        }

        [SetUp]
        public void Setup()
        {
            equationMock.Reset();
            equationMock.Setup(eq => eq.Equals(It.IsAny<CurveEquation>())).Returns(true);
        }

        [Test]
        public void TestEqualsIsTrueForEqualObjects()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreEqual(parameters, otherParameters);
        }

        [Test]
        public void TestEqualsIsFalseForDifferentObjects()
        {
            var equation = equationMock.Object;
            equationMock.Reset();
            equationMock.Setup(eq => eq.Equals(It.IsAny<CurveEquation>())).Returns(false);

            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equation, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                equation, generator, order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            equationMock.Reset();
            equationMock.Setup(eq => eq.Equals(It.IsAny<CurveEquation>())).Returns(true);

            otherParameters = new CurveParameters(
                equation, new CurvePoint(1, 1), order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                equation, generator, BigPrime.CreateWithoutChecks(3), cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                equation, generator, order, BigInteger.One
            );
            Assert.AreNotEqual(parameters, otherParameters);
        }

        [Test]
        public void TestEqualsIsFalseForNull()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreNotEqual(parameters, null);
        }

        [Test]
        public void TestEqualsIsFalseForUnrelatedObject()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreNotEqual(parameters, new object());
        }

        [Test]
        public void TestHashCodeIsSameForEqualObjects()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreEqual(parameters.GetHashCode(), otherParameters.GetHashCode());
        }

    }
}
