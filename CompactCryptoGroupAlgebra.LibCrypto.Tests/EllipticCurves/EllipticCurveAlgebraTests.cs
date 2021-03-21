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
using System.Numerics;
using System.Security.Cryptography;

using CompactCryptoGroupAlgebra.LibCrypto.Internal.Native;

namespace CompactCryptoGroupAlgebra.LibCrypto.EllipticCurves
{
    class EllipticCurveAlgebraTests
    {

        [Test]
        public void TestOrder()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var expected = NISTP256Reference.Order;

            Assert.That(algebra.Order.Equals(expected));
        }

        [Test]
        public void TestGenerator()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var expectedX = NISTP256Reference.generatorX;
            var expectedY = NISTP256Reference.generatorY;

            var generator = algebra.Generator;
            (var x, var y) = generator.GetCoordinates();
            Assert.That(x.ToBigInteger().Equals(expectedX));
            Assert.That(y.ToBigInteger().Equals(expectedY));
        }

        [Test]
        public void TestNeutralElement()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var neutralElement = algebra.NeutralElement;
            
            var result = algebra.Add(neutralElement, neutralElement);

            Assert.That(result.Equals(neutralElement));
        }

        [Test]
        public void TestCofactor()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var expected = NISTP256Reference.Cofactor;

            var cofactor = algebra.Cofactor;

            Assert.That(cofactor.Equals(expected));
        }

        [Test]
        public void TestOrderLength()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var expected = NISTP256Reference.OrderBitLength;

            var length = algebra.OrderBitLength;

            Assert.That(length.Equals(expected));
        }

        [Test]
        public void TestElementBitLength()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var expected = NISTP256Reference.ElementBitLength + 8;

            var length = algebra.ElementBitLength;

            Assert.That(length.Equals(expected));
        }

        [Test]
        public void TestSecurityLevel()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var expected = algebra.OrderBitLength / 2;
            Assert.That(expected == algebra.SecurityLevel);
        }

        [Test]
        public void TestAdd()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var generator = algebra.Generator;

            var groupHandle = ECGroupHandle.CreateByCurveNID((int)EllipticCurveID.Prime256v1);
            var ctx = BigNumberContextHandle.Create();

            var kA = new BigInteger(27);
            var kB = new BigInteger(13);
            var pointA = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, pointA.Handle, new BigNumber(kA).Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
            var pointB = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, pointB.Handle, new BigNumber(kB).Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
            var expected = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, expected.Handle, new BigNumber(kA + kB).Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
            
            var result = algebra.Add(pointA, pointB);

            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestMultipy()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var generator = algebra.Generator;

            var groupHandle = ECGroupHandle.CreateByCurveNID((int)EllipticCurveID.Prime256v1);
            var ctx = BigNumberContextHandle.Create();

            var factor = SecureBigNumber.FromBigNumber(new BigNumber(13));

            var basePointFactor = new BigInteger(27);
            var point = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, point.Handle, new BigNumber(basePointFactor).Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);

            var expected = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, expected.Handle, BigNumberHandle.Null, point.Handle, factor.Handle, ctx);
            
            var result = algebra.MultiplyScalar(point, factor);

            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestGenerateElement()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var generator = algebra.Generator;

            var groupHandle = ECGroupHandle.CreateByCurveNID((int)EllipticCurveID.Prime256v1);

            var ctx = BigNumberContextHandle.Create();
            var index = SecureBigNumber.FromBigNumber(new BigNumber(
                BigInteger.Parse("97752369786356789875745735", System.Globalization.NumberStyles.Integer)
            ));

            var expected = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, expected.Handle, index.Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
            
            var result = algebra.GenerateElement(index);

            Assert.That(result.Equals(expected));
        }

        [Test]
        public void TestIsElement()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var groupHandle = algebra.Handle;

            var ctx = BigNumberContextHandle.Create();
            var index = new BigInteger(3);

            var point = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, point.Handle, new BigNumber(index).Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
            
            Assert.That(algebra.IsPotentialElement(point), "valid point not accepted by IsPotentialElement!");
            Assert.That(algebra.IsSafeElement(point), "valid point not accepted by IsSafeElement!");
        }

        [Test]
        public void TestIsElementForPointAtInfinity()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            Assert.That(algebra.IsPotentialElement(algebra.NeutralElement), "point at infinity not accepted by IsPotentialElement!");
            Assert.That(!algebra.IsSafeElement(algebra.NeutralElement), "point at infinity accepted by IsSafeElement!");
        }

        [Test]
        public void TestNegate()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var generator = algebra.Generator;

            var groupHandle = ECGroupHandle.CreateByCurveNID((int)EllipticCurveID.Prime256v1);

            var ctx = BigNumberContextHandle.Create();
            var index = new BigInteger(3);

            var point = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, point.Handle, new BigNumber(index).Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);

            var result = algebra.Negate(point);

            var pointPlusNegated = algebra.Add(point, result);
            Assert.That(pointPlusNegated.Equals(algebra.NeutralElement));
        }

        [Test]
        public void TestGenerateRandomElement()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var generator = algebra.Generator;

            (var index, var point) = algebra.GenerateRandomElement(RandomNumberGenerator.Create());

            var expected = algebra.GenerateElement(index);
            Assert.That(point.Equals(expected));
        }

        [Test]
        public void TestFromToBytes()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            var generator = algebra.Generator;

            var groupHandle = ECGroupHandle.CreateByCurveNID((int)EllipticCurveID.Prime256v1);

            var ctx = BigNumberContextHandle.Create();
            var index = new BigInteger(3);
            var point = new ECPoint(groupHandle);
            ECPointHandle.Multiply(groupHandle, point.Handle, new BigNumber(index).Handle, ECPointHandle.Null, BigNumberHandle.Null, ctx);
            
            byte[] buffer = algebra.ToBytes(point);
            var result = algebra.FromBytes(buffer);

            Assert.That(result.Equals(point));
        }

        [Test]
        public void TestDispose()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            Assert.That(!algebra.Handle.IsClosed);

            algebra.Dispose();
            Assert.That(algebra.Handle.IsClosed);

            Assert.DoesNotThrow(algebra.Dispose);
        }


        [Test]
        public void TestEqualsTrue()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var otherAlgebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            Assert.That(algebra.Equals(otherAlgebra));
        }

        [Test]
        public void TestEqualsFalseForNull()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            Assert.That(!algebra.Equals(null));
        }

        [Test]
        public void TestEqualsFalseForUnrelatedObject()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var otherAlgebra = new object();
            Assert.That(!algebra.Equals(otherAlgebra));
        }

        [Test]
        public void TestEqualsFalseForOtherAlgebra()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var otherAlgebra = new EllipticCurveAlgebra(EllipticCurveID.Prime239v3);
            
            Assert.That(!algebra.Equals(otherAlgebra));
        }

        [Test]
        public void TestGetHashCodeSameForEqual()
        {
            var algebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var otherAlgebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);

            Assert.That(algebra.GetHashCode() == otherAlgebra.GetHashCode());
        }

        [Test]
        public void TestCreateCryptoGroup()
        {
            var expectedGroupAlgebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var group = EllipticCurveAlgebra.CreateCryptoGroup(EllipticCurveID.Prime256v1);
            Assert.That(group.Algebra.Equals(expectedGroupAlgebra));
        }

    }

}
