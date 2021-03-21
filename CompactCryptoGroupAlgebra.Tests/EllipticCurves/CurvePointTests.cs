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
    public class CurvePointTests
    {

        [Test]
        public void TestClone()
        {
            var p = new CurvePoint(5, 5);
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }

        [Test]
        public void TestClonePointAtInfinity()
        {
            var p = CurvePoint.PointAtInfinity;
            var q = p.Clone();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }
        
        [Test]
        public void TestEquals()
        {
            var p = new CurvePoint(11, 0);
            var q = new CurvePoint(3, 4);

            Assert.AreEqual(p, p, "same");
            Assert.AreEqual(p, p.Clone(), "equal");
            Assert.AreEqual(CurvePoint.PointAtInfinity, CurvePoint.PointAtInfinity, "at inf");

            Assert.AreNotEqual(p, q, "not equal");
            Assert.AreNotEqual(q, p, "not equal reversed");
        }
        
        [Test]
        public void TestPointEquality()
        {
            Assert.AreEqual(CurvePoint.PointAtInfinity, CurvePoint.PointAtInfinity);

            var p = new CurvePoint(14, 3);
            var q = new CurvePoint(14, 3);
            Assert.AreEqual(p, q);
            Assert.AreEqual(q, p);
        }

        [Test]
        public void TestToStringRegularPoint()
        {
            var p = new CurvePoint(1, 2);
            var expected = "(1, 2)";
            Assert.AreEqual(expected, p.ToString());
        }

        [Test]
        public void TestToStringPointAtInfinity()
        {
            var p = CurvePoint.PointAtInfinity;
            var expected = "(at infinity)";
            Assert.AreEqual(expected, p.ToString());
        }
    }
}
