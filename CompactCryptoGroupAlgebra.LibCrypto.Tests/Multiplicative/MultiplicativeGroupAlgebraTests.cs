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

using NUnit.Framework;

using System;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra.LibCrypto.Multiplicative
{
    public class MultiplicativeGroupAlgebraTests
    {

        MultiplicativeGroupAlgebra? groupAlgebra;

        int cofactor = 2;
        BigPrime prime = BigPrime.CreateWithoutChecks(23);
        BigPrime order = BigPrime.CreateWithoutChecks(11);
        int generator = 2;

        

        [SetUp]
        public void SetUp()
        {
            groupAlgebra = new MultiplicativeGroupAlgebra(prime, order, generator);
        }

        [TearDown]
        public void TearDown()
        {
            groupAlgebra!.Dispose();
            groupAlgebra = null;
        }

        [Test]
        public void TestOrderAndCofactor()
        {
            Assert.That(groupAlgebra!.Order.Equals(order));
            Assert.That(groupAlgebra!.Cofactor.Equals(cofactor));
        }

        [Test]
        public void TestSecurityLevel()
        {
            var expected = 
                CompactCryptoGroupAlgebra.Multiplicative.MultiplicativeGroupAlgebra.ComputeSecurityLevel(prime, order);
            Assert.That(groupAlgebra!.SecurityLevel == expected);
        }

        [Test]
        public void TestGenerator()
        {
            var expected = new BigNumber(generator);
            Assert.That(groupAlgebra!.Generator.Equals(expected));
        }

        [Test]
        public void TestNeutralElement()
        {
            Assert.That(groupAlgebra!.NeutralElement.Equals(BigNumber.One));
        }

        [Test]
        public void TestElementBitLength()
        {
            Assert.That(groupAlgebra!.ElementBitLength == NumberLength.GetLength(prime).InBits);
        }

        [Test]
        public void TestOrderBitLength()
        {
            Assert.That(groupAlgebra!.OrderBitLength == NumberLength.GetLength(order).InBits);
        }

        [Test]
        public void TestAdd()
        {
            var x = new BigNumber(4);
            var y = new BigNumber(18);
            var expected = new BigNumber(3);
            var result = groupAlgebra!.Add(x, y);

            Assert.That(result.Equals(expected));
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
            var k = SecureBigNumber.FromBigNumber(new BigNumber(scalarInt));
            var expected = new BigNumber(expectedInt);

            var x = new BigNumber(3);
            var result = groupAlgebra!.MultiplyScalar(x, k);
            Assert.That(result.Equals(expected));
        }


        [Test]
        [TestCase(0)]
        [TestCase(-3)]
        [TestCase(23)]
        [TestCase(136)]
        public void TestInvalidElementRejectedAsGenerator(int generatorInt)
        {
            var generator = new BigNumber(generatorInt);
            Assert.Throws<ArgumentException>(
                () => new MultiplicativeGroupAlgebra(prime, order, generator)
            );
        }

        [Test]
        [TestCase(2)]
        [TestCase(9)]
        [TestCase(13)]
        public void TestIsElementAcceptsValidElements(int elementInt)
        {
            var element = new BigNumber(elementInt);
            Assert.That(groupAlgebra!.IsPotentialElement(element));
            Assert.That(groupAlgebra!.IsSafeElement(element));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-3)]
        [TestCase(23)]
        [TestCase(136)]
        public void TestIsElementRejectsInvalidElementsOutOfBounds(int elementInt)
        {
            var element = new BigNumber(elementInt);
            Assert.That(!groupAlgebra!.IsPotentialElement(element));
            Assert.That(!groupAlgebra!.IsSafeElement(element));
        }

        [Test]
        [TestCase(1)]
        [TestCase(22)]
        public void TestIsElementForUnsafeElements(int elementInt)
        {
            var element = new BigNumber(elementInt);
            Assert.That(groupAlgebra!.IsPotentialElement(element));
            Assert.That(!groupAlgebra!.IsSafeElement(element));
        }

        [Test]
        public void TestIsElementForNeutralElementAndNoCofactor()
        {
            var groupAlgebra = new MultiplicativeGroupAlgebra(BigPrime.CreateWithoutChecks(11), BigPrime.CreateWithoutChecks(10), 2);
            Assert.That(groupAlgebra.IsPotentialElement(groupAlgebra.NeutralElement));
            Assert.That(!groupAlgebra.IsSafeElement(groupAlgebra.NeutralElement));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(9)]
        [TestCase(18)]
        public void TestNegate(int elementInt)
        {
            var exponent = SecureBigNumber.FromBigNumber(new BigNumber(order - 1));
            var x = new BigNumber(elementInt);

            var negated = groupAlgebra!.Negate(x);
            Assert.That(groupAlgebra!.Add(negated, x).Equals(BigNumber.One));
            Assert.That(groupAlgebra!.Negate(x).Equals(groupAlgebra!.MultiplyScalar(x, exponent)));
        }

        public void TestNegateForNonSubgroup()
        {
            var x = new BigNumber(5);
            var negated = groupAlgebra!.Negate(x);

            Assert.AreEqual(groupAlgebra!.Add(negated, x), BigNumber.One,
                "adding negated value for embedding group element does not result in neutral element"
            );
        }

        [Test]
        public void TestToBytesFromBytes()
        {
            var element = new BigNumber(13);
            var bytes = groupAlgebra!.ToBytes(element);
            var result = groupAlgebra!.FromBytes(bytes);
            Assert.That(result.Equals(element));
        }

        [Test]
        public void TestGenerateElement()
        {
            var k = SecureBigNumber.FromBigNumber(new BigNumber(5));

            var result = groupAlgebra!.GenerateElement(k);

            var groupGenerator = new BigNumber(generator);
            var groupModulo = new BigNumber(prime);
            var expected = groupGenerator.ModExp(k, groupModulo);

            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestGenerateRandomElement()
        {
            var generator = groupAlgebra!.Generator;

            (var index, var point) = groupAlgebra!.GenerateRandomElement(RandomNumberGenerator.Create());

            var expected = groupAlgebra!.GenerateElement(index);
            Assert.That(point.Equals(expected));
        }

        [Test]
        public void TestEqualsTrue()
        {
            var otherAlgebra = new MultiplicativeGroupAlgebra(
                prime, order, generator
            );

            Assert.That(groupAlgebra!.Equals(otherAlgebra));
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            Assert.That(!groupAlgebra!.Equals(null));
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var otherAlgebra = new object();
            Assert.That(!groupAlgebra!.Equals(otherAlgebra));
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
                prime, order, generator
            );

            Assert.That(groupAlgebra!.GetHashCode() == otherAlgebra.GetHashCode());
        }

        [Test]
        public void TestDispose()
        {
            var algebra = new MultiplicativeGroupAlgebra(
                prime, order, generator
            );
            Assert.That(!algebra.Generator.Handle.IsClosed);

            algebra.Dispose();
            Assert.That(algebra.Generator.Handle.IsClosed);

            Assert.DoesNotThrow(algebra.Dispose);
        }

        [Test]
        public void TestCreateCryptoGroup()
        {
            var group = MultiplicativeGroupAlgebra.CreateCryptoGroup(
                prime, order, generator
            );
            var expectedGroupAlgebra = new MultiplicativeGroupAlgebra(
                prime, order, generator
            );
            Assert.That(group.Algebra.Equals(expectedGroupAlgebra));
        }

    }
}