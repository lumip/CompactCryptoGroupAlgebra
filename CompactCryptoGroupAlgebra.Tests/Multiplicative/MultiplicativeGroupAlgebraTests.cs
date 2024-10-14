// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2022-2024 Lukas Prediger <lumip@lumip.de>
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

using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using Moq;
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
        public void TestComputeSecurityLevel()
        {
            var mod = BigPrime.CreateWithoutChecks(BigInteger.One << 2047);
            Assert.AreEqual(115, MultiplicativeGroupAlgebra.ComputeSecurityLevel(mod, mod));

            var smallOrder = BigPrime.CreateWithoutChecks(new BigInteger(4));
            Assert.AreEqual(2 * NumberLength.GetLength(smallOrder).InBits, MultiplicativeGroupAlgebra.ComputeSecurityLevel(mod, smallOrder));
        }

        [Test]
        public void TestComputePrimeLengthForSecurityLevel()
        {
            // dominated by NFS
            var l = MultiplicativeGroupAlgebra.ComputePrimeLengthForSecurityLevel(100);
            var p = BigPrime.CreateWithoutChecks(BigInteger.One << (l.InBits - 1));
            var s = MultiplicativeGroupAlgebra.ComputeSecurityLevel(p, p);
            Assert.AreEqual(100, s);

            // dominated by Pollard Rho
            l = MultiplicativeGroupAlgebra.ComputePrimeLengthForSecurityLevel(1);
            Assert.AreEqual(2, l.InBits);
        }

        [Test]
        public void TestSecurityLevel()
        {
            var expected = MultiplicativeGroupAlgebra.ComputeSecurityLevel(groupAlgebra!.Prime, groupAlgebra!.Order);
            Assert.AreEqual(expected, groupAlgebra!.SecurityLevel);
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
        public void TestIsElementAcceptsValidElements(int elementInt)
        {
            var element = new BigInteger(elementInt);
            Assert.IsTrue(groupAlgebra!.IsPotentialElement(element));
            Assert.IsTrue(groupAlgebra!.IsSafeElement(element));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-3)]
        [TestCase(23)]
        [TestCase(136)]
        public void TestIsElementRejectsInvalidElementsOutOfBounds(int elementInt)
        {
            var element = new BigInteger(elementInt);
            Assert.IsFalse(groupAlgebra!.IsPotentialElement(element));
            Assert.IsFalse(groupAlgebra!.IsSafeElement(element));
        }

        [Test]
        [TestCase(1)]
        [TestCase(22)]
        public void TestIsSafeElementRejectsUnsafeElements(int elementInt)
        {
            var element = new BigInteger(elementInt);
            Assert.IsTrue(groupAlgebra!.IsPotentialElement(element));
            Assert.IsFalse(groupAlgebra!.IsSafeElement(element));
        }

        [Test]
        public void TestIsSafeElementForNeutralElementCofactorOne()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(BigPrime.CreateWithoutChecks(11), BigPrime.CreateWithoutChecks(10), 2);
            Assert.That(groupAlgebra.IsPotentialElement(groupAlgebra.NeutralElement));
            Assert.That(!groupAlgebra.IsSafeElement(groupAlgebra.NeutralElement));
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
                BigPrime.CreateWithoutChecks(2*53+1),
                BigPrime.CreateWithoutChecks(53),
                4
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

        [Test]
        [TestCase(1)]  // rng returns even, +1 mod 6 == 1
        [TestCase(4)]  // rng returns odd, mod 6 == 1
        [TestCase(6)]  // rng returns odd, mod 6 == 5
        [TestCase(8)]  // rng returns odd, mod 6 == 3
        public void TestCreateCryptoGroupForLevel(int subtrahend)
        {
            int securityLevel = 32;
            var expectedPrimeLength = MultiplicativeGroupAlgebra.ComputePrimeLengthForSecurityLevel(securityLevel);
            var expectedOrderLength = NumberLength.FromBitLength(expectedPrimeLength.InBits - 1);

            var primeBeforeOrder = BigInteger.Parse("166153499473114484112975882535050653");  // not Sophie Germain prime
            var order = BigPrime.CreateWithoutChecks(BigInteger.Parse("166153499473114484112975882535050719"));
            var prime = BigPrime.CreateWithoutChecks(BigInteger.Parse("332306998946228968225951765070101439"));

            Debug.Assert(expectedPrimeLength.InBits == NumberLength.GetLength(prime).InBits);
            Debug.Assert(expectedOrderLength.InBits == NumberLength.GetLength(order).InBits);

            var rngResponse = (primeBeforeOrder - subtrahend).ToByteArray();
            Random random = new Random(0);
            bool firstCall = true;

            var rngMock = new Mock<RandomNumberGenerator>(MockBehavior.Strict);
            rngMock.Setup(rng => rng.GetBytes(It.IsAny<byte[]>()))
                   .Callback<byte[]>(buffer =>
                        {
                            if (firstCall)
                                Buffer.BlockCopy(rngResponse, 0, buffer, 0, Math.Min(rngResponse.Length, buffer.Length));
                            else
                                random.NextBytes(buffer);
                            firstCall = false;
                        }
                   );

            var group = MultiplicativeGroupAlgebra.CreateCryptoGroup(securityLevel, rngMock.Object);
            Assert.IsInstanceOf<MultiplicativeGroupAlgebra>(group.Algebra);

            Assert.That(group.SecurityLevel >= securityLevel, "Created group does not meet security level!");
            Assert.AreEqual(order, group.Algebra.Order);
            Assert.AreEqual(prime, ((MultiplicativeGroupAlgebra)group.Algebra).Prime);
            Assert.That(group.Algebra.IsSafeElement(group.Algebra.Generator));
        }

    }
}
