using System;
using System.Numerics;

using NUnit.Framework;

using CompactCryptoGroupAlgebra;

namespace CompactCryptoGroupAlgebra.Tests
{
    [TestFixture]
    public class ECCryptoGroupTests
    {
        private readonly ECParameters ecParams;

        public ECCryptoGroupTests()
        {
            ecParams = new ECParameters()
            {
                P = 23,
                A = -2,
                B = 2,
                Generator = new ECPoint(1, 1),
                Order = 16
            };
        }

        /// <summary>
        /// This indirectly tests <see cref="ECCryptoGroup.CreateGroupElement(byte[])"/>.
        /// </summary>
        [Test]
        public void TestFromBytes()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            var group = new ECCryptoGroup(groupAlgebra);

            var expectedRaw = new ECPoint(5, 5);
            var expected = new CryptoGroupElement<ECPoint>(expectedRaw, groupAlgebra);
            var bytes = expected.ToBytes();
            var result = group.FromBytes(bytes);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Indirectly also tests <see cref="ECCryptoGroup.CreateGroupElement(ECPoint)"/>.
        /// </summary>
        [Test]
        public void TestConstructorAlgebra()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            var group = new ECCryptoGroup(groupAlgebra);

            var expectedNeutralElement = new CryptoGroupElement<ECPoint>(ECPoint.PointAtInfinity, groupAlgebra);
            var expectedGenerator = new CryptoGroupElement<ECPoint>(ecParams.Generator, groupAlgebra);

            var resultNeutralElement = group.NeutralElement;
            var resultGenerator = group.Generator;

            Assert.AreEqual(expectedNeutralElement, resultNeutralElement, "verifying neutral element");
            Assert.AreEqual(expectedGenerator, resultGenerator, "verifying generator");
            Assert.AreEqual(ecParams.Order, group.Order, "verifying order");
        }

        /// <summary>
        /// Indirectly also tests <see cref="ECCryptoGroup.CreateGroupElement(ECPoint)"/>.
        /// </summary>
        [Test]
        public void TestConstructorParameters()
        {
            var groupAlgebra = new ECGroupAlgebra(ecParams);
            var group = new ECCryptoGroup(ecParams);

            var expectedNeutralElement = new CryptoGroupElement<ECPoint>(ECPoint.PointAtInfinity, groupAlgebra);
            var expectedGenerator = new CryptoGroupElement<ECPoint>(ecParams.Generator, groupAlgebra);

            var resultNeutralElement = group.NeutralElement;
            var resultGenerator = group.Generator;

            Assert.AreEqual(expectedNeutralElement, resultNeutralElement, "verifying neutral element");
            Assert.AreEqual(expectedGenerator, resultGenerator, "verifying generator");
            Assert.AreEqual(ecParams.Order, group.Order, "verifying order");
        }
    }
}
