// CompactCryptoGroupAlgebra.LibCrypto - OpenSSL libcrypto implementation of CompactCryptoGroupAlgebra interfaces

// SPDX-FileCopyrightText: 2021 Lukas Prediger <lumip@lumip.de>
// SPDX-License-Identifier: GPL-3.0-or-later WITH GPL-3.0-linking-exception
// SPDX-FileType: SOURCE

// CompactCryptoGroupAlgebra.LibCrypto is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CompactCryptoGroupAlgebra.LibCrypto is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Additional permission under GNU GPL version 3 section 7
// 
// If you modify CompactCryptoGroupAlgebra.LibCrypto, or any covered work, by linking or combining it
// with the OpenSSL library (or a modified version of that library), containing parts covered by the
// terms of the OpenSSL License and the SSLeay License, the licensors of CompactCryptoGroupAlgebra.LibCrypto
// grant you additional permission to convey the resulting work.

using System;
using CompactCryptoGroupAlgebra.LibCrypto.Internal.Native;
using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.LibCrypto.EllipticCurves
{
    class ECPointTests
    {

        private ECGroupHandle groupHandle = ECGroupHandle.Null;
        private ECPointHandle rawPointHandle = ECPointHandle.Null;
        private BigNumberContextHandle ctx = BigNumberContextHandle.Null;

        [SetUp]
        public void SetUp()
        {
            groupHandle = ECGroupHandle.CreateByCurveNID((int)EllipticCurveID.Prime256v1);
            rawPointHandle = ECGroupHandle.GetGenerator(groupHandle);
            ctx = BigNumberContextHandle.Create();
        }

        [TearDown]
        public void TearDown()
        {
            ctx.Close();
            rawPointHandle.Close();
            groupHandle.Close();
        }

        [Test]
        public void TestCopyConstructor()
        {
            var point = new ECPoint(groupHandle, rawPointHandle);

            Assert.That(!point.Handle.Equals(rawPointHandle));
            Assert.That(ECPointHandle.Compare(groupHandle, rawPointHandle, point.Handle, ctx));
        }

        [Test]
        public void TestToBytesAndBack()
        {
            var point = new ECPoint(groupHandle, rawPointHandle);

            byte[] buffer = point.ToBytes();
            var newPoint = ECPoint.CreateFromBytes(groupHandle, buffer);

            Assert.That(newPoint.Equals(point));
        }

        [Test]
        public void TestEqualsAndHash()
        {
            var point = new ECPoint(groupHandle, rawPointHandle);

            var equalPoint = new ECPoint(groupHandle, rawPointHandle);
            Assert.That(!point.Handle.Equals(equalPoint.Handle), "Distinct points share same handle!");
            Assert.That(point.Equals(equalPoint), "Equal points are not seen as equal!");
            Assert.That(point.GetHashCode().Equals(equalPoint.GetHashCode()), "Equals points do not share same hash code!");

            var unequalPoint = new ECPoint(groupHandle);
            ECPointHandle.Add(groupHandle, unequalPoint.Handle, rawPointHandle, rawPointHandle, ctx);
            Assert.That(!point.Equals(unequalPoint), "Unequal points are seen as equal!");

            Assert.That(!point.Equals(new { }), "Point equal to anonymous object!");
        }

        [Test]
        public void TestGetCoordinates()
        {
            var point = new ECPoint(groupHandle, rawPointHandle);

            var expectedXNum = new BigNumber(NISTP256Reference.generatorX);
            var expectedYNum = new BigNumber(NISTP256Reference.generatorY);

            (var x, var y) = point.GetCoordinates();

            Assert.That(x.Equals(expectedXNum));
            Assert.That(y.Equals(expectedYNum));
        }

        [Test]
        public void TestDispose()
        {
            var point = new ECPoint(groupHandle, rawPointHandle);
            Assert.That(!point.Handle.IsClosed);

            point.Dispose();
            Assert.That(point.Handle.IsClosed);

            Assert.DoesNotThrow(point.Dispose);
        }

        [Test]
        public void TestGetCoordinatesWithPointAtInfinity()
        {
            var point = new ECPoint(groupHandle);
            ECPointHandle.SetToInfinity(groupHandle, point.Handle);

            Assert.Throws<InvalidOperationException>(() => point.GetCoordinates());
        }

        [Test]
        public void TestIsAtInfinity()
        {
            var validPoint = new ECPoint(groupHandle, rawPointHandle);
            Assert.That(!validPoint.IsAtInfinity);

            var pointAtInf = new ECPoint(groupHandle);
            ECPointHandle.SetToInfinity(groupHandle, pointAtInf.Handle);
            Assert.That(pointAtInf.IsAtInfinity);
        }

    }
}
