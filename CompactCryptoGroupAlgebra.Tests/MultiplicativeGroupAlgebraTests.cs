using System;
using System.Numerics;

using NUnit.Framework;

using CompactCryptoGroupAlgebra;

// Best Practices: https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
namespace CompactCryptoGroupAlgebra.Tests
{
    [TestFixture]
    public class MultiplicativeGroupAlgebraTests
    {
        [Test]
        [TestCase(0, 1)]
        [TestCase(1, 3)]
        [TestCase(2, 9)]
        [TestCase(3, 5)]
        [TestCase(4, 4)]
        [TestCase(5, 1)]
        [TestCase(6, 3)]
        [TestCase(12, 9)]
        public void TestMultiplyScalar(int scalarInt, int expectedInt)
        {
            var k = new BigInteger(scalarInt);
            var expected = new BigInteger(expectedInt);

            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            var x = new BigInteger(3);
            var result = groupAlgebra.MultiplyScalar(x, k);
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(7)]
        public void TestGeneratorIsAsSet(int generatorInt)
        {
            var generator = new BigInteger(generatorInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, generator);
            Assert.AreEqual(generator, groupAlgebra.Generator);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-3)]
        [TestCase(11)]
        [TestCase(136)]
        public void TestInvalidElementRejectedAsGenerator(int generatorInt)
        {
            var generator = new BigInteger(generatorInt);
            Assert.Throws<ArgumentException>(
                () => new MultiplicativeGroupAlgebra(11, 10, generator)
            );
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void TestIsValidAcceptsValidElements(int elementInt)
        {
            var element = new BigInteger(elementInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.IsTrue(groupAlgebra.IsValid(element));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-3)]
        [TestCase(11)]
        [TestCase(136)]
        public void TestIsValidRejectsInvalidElementsOutOfBounds(int elementInt)
        {
            var element = new BigInteger(elementInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.IsFalse(groupAlgebra.IsValid(element));
        }

        [Test]
        [TestCase(22)]
        public void TestIsValidRejectsInvalidElementsNotGenerated(int elementInt)
        {
            var element = new BigInteger(elementInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(23, 11, 4);
            Assert.IsFalse(groupAlgebra.IsValid(element));
        }

        [Test]
        public void TestGroupElementBitLength()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.AreEqual(4, groupAlgebra.ElementBitLength);
        }

        [Test]
        public void TestOrderBitLength()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 5, 3);
            Assert.AreEqual(3, groupAlgebra.OrderBitLength);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public void TestNegate(int elementInt)
        {
            var x = new BigInteger(elementInt);
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.AreEqual(groupAlgebra.MultiplyScalar(x, groupAlgebra.Order - 1), groupAlgebra.Negate(x));
        }

        [Test]
        public void TestNeutralElement()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.AreEqual(BigInteger.One, groupAlgebra.NeutralElement);
        }

        [Test]
        public void TestFromBytes()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var expected = new BigInteger(7);
            var buffer = expected.ToByteArray();

            var result = groupAlgebra.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestFromBytesRejectsNullArgument()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            Assert.Throws<ArgumentNullException>(
                () => groupAlgebra.FromBytes(null)
            );
        }

        [Test]
        public void TestToBytes()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var element = new BigInteger(7);
            var expected = element.ToByteArray();

            var result = groupAlgebra.ToBytes(element);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestProperties()
        {
            var generatorRaw = new BigInteger(2);
            var neutralElementRaw = BigInteger.One;
            var moduloRaw = new BigInteger(11);
            var orderRaw = new BigInteger(10);
            var cofactorRaw = new BigInteger(1);

            var groupAlgebra = new MultiplicativeGroupAlgebra(moduloRaw, orderRaw, generatorRaw);

            Assert.AreEqual(neutralElementRaw, groupAlgebra.NeutralElement, "verifying neutral element");
            Assert.AreEqual(generatorRaw, groupAlgebra.Generator, "verifying generator");
            Assert.AreEqual(orderRaw, groupAlgebra.Order, "verifying order");
            Assert.AreEqual(moduloRaw, groupAlgebra.Prime, "verifying modulo");
            Assert.AreEqual(cofactorRaw, groupAlgebra.Cofactor, "verifying cofactor");
        }

        [Test]
        public void TestPropertiesSubgroup()
        {
            var generatorRaw = new BigInteger(4);
            var neutralElementRaw = BigInteger.One;
            var moduloRaw = new BigInteger(23);
            var orderRaw = new BigInteger(11);
            var cofactorRaw = new BigInteger(2);

            var groupAlgebra = new MultiplicativeGroupAlgebra(moduloRaw, orderRaw, generatorRaw);

            Assert.AreEqual(neutralElementRaw, groupAlgebra.NeutralElement, "verifying neutral element");
            Assert.AreEqual(generatorRaw, groupAlgebra.Generator, "verifying generator");
            Assert.AreEqual(orderRaw, groupAlgebra.Order, "verifying order");
            Assert.AreEqual(moduloRaw, groupAlgebra.Prime, "verifying modulo");
            Assert.AreEqual(cofactorRaw, groupAlgebra.Cofactor, "verifying cofactor");
        }

        [Test]
        public void TestEqualsTrue()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var otherAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            MultiplicativeGroupAlgebra otherAlgebra = null;

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var otherAlgebra = new object { };

            bool result = groupAlgebra.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            var otherAlgebra = new MultiplicativeGroupAlgebra(13, 10, 2);
            Assert.IsFalse(groupAlgebra.Equals(otherAlgebra));

            otherAlgebra = new MultiplicativeGroupAlgebra(11, 7, 2);
            Assert.IsFalse(groupAlgebra.Equals(otherAlgebra));

            otherAlgebra = new MultiplicativeGroupAlgebra(11, 10, 4);
            Assert.IsFalse(groupAlgebra.Equals(otherAlgebra));
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);
            var otherAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2);

            Assert.AreEqual(groupAlgebra.GetHashCode(), otherAlgebra.GetHashCode());
        }

    }
}
