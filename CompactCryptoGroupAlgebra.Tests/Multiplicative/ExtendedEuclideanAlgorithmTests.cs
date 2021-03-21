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

using System.Numerics;

namespace CompactCryptoGroupAlgebra.Multiplicative
{
    class ExtendedEuclideanAlgorithmTests
    {
        
        [Test]
        [TestCase(17, 7, -2, 5)]
        [TestCase(7, 17, 5, -2)]
        public void TestExtendedGreatestCommonDivisor(int rawA, int rawB, int rawX, int rawY)
        {
            var a = new BigInteger(rawA);
            var b = new BigInteger(rawB);

            var expectedX = new BigInteger(rawX);
            var expectedY = new BigInteger(rawY);

            (var gcd, var x, var y) = ExtendedEuclideanAlgorithm.GreatestCommonDivisor(a, b);
            Assert.That(x.Equals(expectedX));
            Assert.That(y.Equals(expectedY));
        }

    }
}
