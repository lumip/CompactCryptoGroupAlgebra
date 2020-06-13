using System;
using System.Numerics;

using NUnit.Framework;

using CompactCryptoGroupAlgebra;

namespace CompactCryptoGroupAlgebra.Tests
{
    [TestFixture]
    public class MultiplicativeCryptoGroupTests
    {
        /// <summary>
        /// This indirectly tests <see cref="MultiplicativeCryptoGroup.CreateGroupElement(byte[])"/>.
        /// </summary>
        [Test]
        public void TestFromBytes()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var group = new MultiplicativeCryptoGroup(groupAlgebra);

            var expectedRaw = new BigInteger(7);
            var expected = new CryptoGroupElement<BigInteger>(expectedRaw, groupAlgebra);
            var bytes = expectedRaw.ToByteArray();
            var result = group.FromBytes(bytes);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Indirectly also tests <see cref="MultiplicativeCryptoGroup.CreateGroupElement(BigInteger)"/>.
        /// </summary>
        [Test]
        public void TestConstructorAlgebra()
        {
            var generatorRaw = new BigInteger(2);
            var neutralElementRaw = BigInteger.One;
            var moduloRaw = new BigInteger(11);
            var orderRaw = new BigInteger(10);

            var groupAlgebra = new MultiplicativeGroupAlgebra(moduloRaw, orderRaw, generatorRaw);
            var group = new MultiplicativeCryptoGroup(groupAlgebra);

            var expectedNeutralElement = new CryptoGroupElement<BigInteger>(groupAlgebra.NeutralElement, groupAlgebra);
            var expectedGenerator = new CryptoGroupElement<BigInteger>(groupAlgebra.Generator, groupAlgebra);

            var resultNeutralElement = group.NeutralElement;
            var resultGenerator = group.Generator;

            Assert.AreEqual(expectedNeutralElement, resultNeutralElement, "verifying neutral element");
            Assert.AreEqual(expectedGenerator, resultGenerator, "verifying generator");
            Assert.AreEqual(orderRaw, group.Order, "verifying order");
        }

        /// <summary>
        /// Indirectly also tests <see cref="MultiplicativeCryptoGroup.CreateGroupElement(BigInteger)"/>.
        /// </summary>
        [Test]
        public void TestConstructorParameters()
        {
            var generatorRaw = new BigInteger(2);
            var neutralElementRaw = BigInteger.One;
            var moduloRaw = new BigInteger(11);
            var orderRaw = new BigInteger(10);

            var group = new MultiplicativeCryptoGroup(moduloRaw, orderRaw, generatorRaw);

            var groupAlgebra = new MultiplicativeGroupAlgebra(moduloRaw, orderRaw, generatorRaw);
            var expectedNeutralElement = new CryptoGroupElement<BigInteger>(neutralElementRaw, groupAlgebra);
            var expectedGenerator = new CryptoGroupElement<BigInteger>(generatorRaw, groupAlgebra);

            var resultNeutralElement = group.NeutralElement;
            var resultGenerator = group.Generator;

            Assert.AreEqual(neutralElementRaw, ((CryptoGroupElement<BigInteger>)resultNeutralElement).Value, "verifying neutral element");
            Assert.AreEqual(generatorRaw, ((CryptoGroupElement<BigInteger>)resultGenerator).Value, "verifying generator");
            //Assert.AreEqual(expectedNeutralElement, resultNeutralElement, "verifying neutral element");
            //Assert.AreEqual(expectedGenerator, resultGenerator, "verifying generator");
            Assert.AreEqual(orderRaw, group.Order, "verifying order");
        }
    }
}
