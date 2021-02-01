using NUnit.Framework;
using System;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

namespace CompactCryptoGroupAlgebra.OpenSsl
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
            
            Assert.That(!point.Equals(new {}), "Point equal to anonymous object!");
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