using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.Multiplicative
{
    [TestFixture]
    public class MultiplicativeGroupAlgebraTests
    {
        private MultiplicativeGroupAlgebra? groupAlgebra;

        [SetUp]
        public void SetUpAlgebra()
        {
            groupAlgebra = new MultiplicativeGroupAlgebra(
                prime: BigPrime.CreateWithoutChecks(23),
                order: BigPrime.CreateWithoutChecks(11),
                generator: 2
            );
            // group elements: 1,  2,  3,  4,  6,  8,  9, 12, 13, 16, 18
        }

        [Test]
        public void TestAdd()
        {
            var result = groupAlgebra!.Add(4, 18);
            var expected = new BigInteger(3);

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(1, 3)]
        [TestCase(2, 9)]
        [TestCase(3, 4)]
        [TestCase(4, 12)]
        [TestCase(6, 16)]
        [TestCase(13, 9)]
        [TestCase(5, 13)] // this is not part of the actual (sub)group but the embedding group, we confirm that it works anyways
        public void TestMultiplyScalar(int scalarInt, int expectedInt)
        {
            var k = new BigInteger(scalarInt);
            var expected = new BigInteger(expectedInt);

            var x = new BigInteger(3);
            var result = groupAlgebra!.MultiplyScalar(x, k);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestGeneratorIsAsSet()
        {
            var generator = new BigInteger(2);
            Assert.AreEqual(generator, groupAlgebra!.Generator);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-3)]
        [TestCase(23)]
        [TestCase(136)]
        public void TestInvalidElementRejectedAsGenerator(int generatorInt)
        {
            var generator = new BigInteger(generatorInt);
            Assert.Throws<ArgumentException>(
                () => new MultiplicativeGroupAlgebra(groupAlgebra!.Prime, groupAlgebra!.Order, generator)
            );
        }

        [Test]
        [TestCase(2)]
        [TestCase(9)]
        [TestCase(13)]
        [TestCase(5)] // this is not part of the actual (sub)group but the embedding group, we confirm that it works anyways
        public void TestIsElementAcceptsValidElements(int elementInt)
        {
            var element = new BigInteger(elementInt);
            Assert.IsTrue(groupAlgebra!.IsElement(element));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-3)]
        [TestCase(23)]
        [TestCase(136)]
        public void TestIsElementRejectsInvalidElementsOutOfBounds(int elementInt)
        {
            var element = new BigInteger(elementInt);
            Assert.IsFalse(groupAlgebra!.IsElement(element));
        }

        [Test]
        [TestCase(1)]
        [TestCase(22)]
        public void TestIsElementRejectsUnsafeElements(int elementInt)
        {
            var element = new BigInteger(elementInt);
            Assert.IsFalse(groupAlgebra!.IsElement(element));
        }

        [Test]
        public void TestGroupElementBitLength()
        {
            Assert.AreEqual(5, groupAlgebra!.ElementBitLength);
        }

        [Test]
        public void TestOrderBitLength()
        {
            Assert.AreEqual(4, groupAlgebra!.OrderBitLength);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(9)]
        [TestCase(18)]
        public void TestNegate(int elementInt)
        {
            var x = new BigInteger(elementInt);
            var negated = groupAlgebra!.Negate(x);

            Assert.AreEqual(groupAlgebra!.Add(negated, x), BigInteger.One, "adding negated value does not result in neutral element");
            Assert.AreEqual(groupAlgebra!.MultiplyScalar(x, groupAlgebra!.Order - 1), negated, "negated value not as expected");
        }

        public void TestNegateForNonSubgroup()
        {
            var x = new BigInteger(5);
            var negated = groupAlgebra!.Negate(x);

            Assert.AreEqual(groupAlgebra!.Add(negated, x), BigInteger.One,
                "adding negated value for embedding group element does not result in neutral element"
            );
        }

        [Test]
        public void TestNeutralElement()
        {
            Assert.AreEqual(BigInteger.One, groupAlgebra!.NeutralElement);
        }

        [Test]
        public void TestFromBytes()
        {
            var expected = new BigInteger(7);
            var buffer = expected.ToByteArray();

            var result = groupAlgebra!.FromBytes(buffer);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestToBytes()
        {
            var element = new BigInteger(7);
            var expected = element.ToByteArray();

            var result = groupAlgebra!.ToBytes(element);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void TestProperties()
        {
            var generator = new BigInteger(2);
            var neutralElement = BigInteger.One;
            var modulo = BigPrime.CreateWithoutChecks(23);
            var order = BigPrime.CreateWithoutChecks(11);
            var cofactor = new BigInteger(2);

            Assert.AreEqual(neutralElement, groupAlgebra!.NeutralElement, "verifying neutral element");
            Assert.AreEqual(generator, groupAlgebra!.Generator, "verifying generator");
            Assert.AreEqual(order, groupAlgebra!.Order, "verifying order");
            Assert.AreEqual(modulo, groupAlgebra!.Prime, "verifying modulo");
            Assert.AreEqual(cofactor, groupAlgebra!.Cofactor, "verifying cofactor");
        }

        [Test]
        public void TestEqualsTrue()
        {
            var otherAlgebra = new MultiplicativeGroupAlgebra(
                groupAlgebra!.Prime, groupAlgebra!.Order, groupAlgebra!.Generator
            );

            bool result = groupAlgebra!.Equals(otherAlgebra);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            bool result = groupAlgebra!.Equals(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var otherAlgebra = new object();
            bool result = groupAlgebra!.Equals(otherAlgebra);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var otherAlgebra = new MultiplicativeGroupAlgebra(
                BigPrime.CreateWithoutChecks(59),
                BigPrime.CreateWithoutChecks(29),
                5
            );
            Assert.IsFalse(groupAlgebra!.Equals(otherAlgebra));

            otherAlgebra = new MultiplicativeGroupAlgebra(
                BigPrime.CreateWithoutChecks(23),
                BigPrime.CreateWithoutChecks(11),
                8
            );
            Assert.IsFalse(groupAlgebra!.Equals(otherAlgebra));

            otherAlgebra = new MultiplicativeGroupAlgebra(
                BigPrime.CreateWithoutChecks(23),
                BigPrime.CreateWithoutChecks(23),
                5
            );
            Assert.IsFalse(groupAlgebra!.Equals(otherAlgebra));
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var otherAlgebra = new MultiplicativeGroupAlgebra(
                groupAlgebra!.Prime, groupAlgebra!.Order, groupAlgebra!.Generator
            );

            Assert.AreEqual(groupAlgebra!.GetHashCode(), otherAlgebra.GetHashCode());
        }

        [Test]
        public void TestCreateCryptoGroup()
        {
            var group = MultiplicativeGroupAlgebra.CreateCryptoGroup(
                groupAlgebra!.Prime, groupAlgebra!.Order, groupAlgebra!.Generator
            );
            Assert.AreEqual(groupAlgebra, group.Algebra);
        }

    }
}
