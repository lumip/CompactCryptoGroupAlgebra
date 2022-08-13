// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography

// SPDX-FileCopyrightText: 2022 Lukas Prediger <lumip@lumip.de>
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

using System.Numerics;
using System.Security.Cryptography;

using NUnit.Framework;
using Moq;

using CompactCryptoGroupAlgebra.TestUtils;

namespace CompactCryptoGroupAlgebra.EllipticCurves
{
    [TestFixture]
    public class CurveParametersTests
    {

        private readonly Mock<CurveEquation> equationMock;

        public CurveParametersTests()
        {
            equationMock = new Mock<CurveEquation>(
                BigPrime.CreateWithoutChecks(11),
                new BigInteger(2),
                new BigInteger(-3)
            );
        }

        [SetUp]
        public void Setup()
        {
            equationMock.Reset();
            equationMock.Setup(eq => eq.Equals(It.IsAny<CurveEquation>())).Returns(true);
        }

        [Test]
        public void TestEqualsIsTrueForEqualObjects()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreEqual(parameters, otherParameters);
        }

        [Test]
        public void TestEqualsIsFalseForDifferentObjects()
        {
            var equation = equationMock.Object;
            equationMock.Reset();
            equationMock.Setup(eq => eq.Equals(It.IsAny<CurveEquation>())).Returns(false);

            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equation, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                equation, generator, order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            equationMock.Reset();
            equationMock.Setup(eq => eq.Equals(It.IsAny<CurveEquation>())).Returns(true);

            otherParameters = new CurveParameters(
                equation, new CurvePoint(1, 1), order, cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                equation, generator, BigPrime.CreateWithoutChecks(3), cofactor
            );
            Assert.AreNotEqual(parameters, otherParameters);

            otherParameters = new CurveParameters(
                equation, generator, order, BigInteger.One
            );
            Assert.AreNotEqual(parameters, otherParameters);
        }

        [Test]
        public void TestEqualsIsFalseForNull()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreNotEqual(parameters, null);
        }

        [Test]
        public void TestEqualsIsFalseForUnrelatedObject()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreNotEqual(parameters, new object());
        }

        [Test]
        public void TestHashCodeIsSameForEqualObjects()
        {
            var order = BigPrime.CreateWithoutChecks(7);
            var cofactor = new BigInteger(2);
            var generator = CurvePoint.PointAtInfinity;
            CurveParameters parameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            CurveParameters otherParameters = new CurveParameters(
                equationMock.Object, generator, order, cofactor
            );

            Assert.AreEqual(parameters.GetHashCode(), otherParameters.GetHashCode());
        }

        [Test]
        public void TestP256CurveParameters()
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();

            var parameters = CurveParameters.NISTP256;

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Pow(2, 256) - BigInteger.Pow(2, 224) + BigInteger.Pow(2, 192) + BigInteger.Pow(2, 96) - 1
                ),
                parameters.Equation.Field.Modulo,
                "modulo wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Equation.Field.Modulo, randomNumberGenerator), "modulo not prime");
            
            Assert.AreEqual(
                new BigInteger(-3),
                parameters.Equation.A,
                "A wrong"
            );
            Assert.AreEqual(
                BigInteger.Parse("41058363725152142129326129780047268409114441015993725554835256314039467401291"),
                parameters.Equation.B,
                "B wrong"
            );
            
            Assert.AreEqual(
                new CurvePoint(
                    BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286"),
                    BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109")
                ),
                parameters.Generator,
                "generator wrong"
            );

            Assert.That(parameters.Equation.IsPointOnCurve(parameters.Generator), "generator not on curve");

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Parse("115792089210356248762697446949407573529996955224135760342422259061068512044369 ")
                ),
                parameters.Order,
                "order wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Order, randomNumberGenerator), "order not prime");

            Assert.AreEqual(BigInteger.One, parameters.Cofactor, "cofactor wrong");
        }

        [Test]
        public void TestP384CurveParameters()
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();

            var parameters = CurveParameters.NISTP384;

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Pow(2, 384) - BigInteger.Pow(2, 128) - BigInteger.Pow(2, 96) + BigInteger.Pow(2, 32) - 1
                ),
                parameters.Equation.Field.Modulo,
                "modulo wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Equation.Field.Modulo, randomNumberGenerator), "modulo not prime");
            
            Assert.AreEqual(
                new BigInteger(-3),
                parameters.Equation.A,
                "A wrong"
            );
            Assert.AreEqual(
                BigInteger.Parse("27580193559959705877849011840389048093056905856361568521428707301988689241309860865136260764883745107765439761230575"),
                parameters.Equation.B,
                "B wrong"
            );
            
            Assert.AreEqual(
                new CurvePoint(
                    BigInteger.Parse("26247035095799689268623156744566981891852923491109213387815615900925518854738050089022388053975719786650872476732087"),
                    BigInteger.Parse("8325710961489029985546751289520108179287853048861315594709205902480503199884419224438643760392947333078086511627871")
                ),
                parameters.Generator,
                "generator wrong"
            );

            Assert.That(parameters.Equation.IsPointOnCurve(parameters.Generator), "generator not on curve");

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Parse("39402006196394479212279040100143613805079739270465446667946905279627659399113263569398956308152294913554433653942643")
                ),
                parameters.Order,
                "order wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Order, randomNumberGenerator), "order not prime");

            Assert.AreEqual(BigInteger.One, parameters.Cofactor, "cofactor wrong");
        }

        [Test]
        public void TestP521CurveParameters()
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();

            var parameters = CurveParameters.NISTP521;

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Pow(2, 521) - 1
                ),
                parameters.Equation.Field.Modulo,
                "modulo wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Equation.Field.Modulo, randomNumberGenerator), "modulo not prime");
            
            Assert.AreEqual(
                new BigInteger(-3),
                parameters.Equation.A,
                "A wrong"
            );
            Assert.AreEqual(
                BigIntegerUtils.ParseHex("0051953eb9618e1c9a1f929a21a0b68540eea2da725b99b315f3b8b489918ef109e156193951ec7e937b1652c0bd3bb1bf073573df883d2c34f1ef451fd46b503f00"),
                parameters.Equation.B,
                "B wrong"
            );
            
            Assert.AreEqual(
                new CurvePoint(
                    BigIntegerUtils.ParseHex("00c6858e06b70404e9cd9e3ecb662395b4429c648139053fb521f828af606b4d3dbaa14b5e77efe75928fe1dc127a2ffa8de3348b3c1856a429bf97e7e31c2e5bd66"),
                    BigIntegerUtils.ParseHex("011839296a789a3bc0045c8a5fb42c7d1bd998f54449579b446817afbd17273e662c97ee72995ef42640c550b9013fad0761353c7086a272c24088be94769fd16650")
                ),
                parameters.Generator,
                "generator wrong"
            );

            Assert.That(parameters.Equation.IsPointOnCurve(parameters.Generator), "generator not on curve");

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigIntegerUtils.ParseHex("01fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffa51868783bf2f966b7fcc0148f709a5d03bb5c9b8899c47aebb6fb71e91386409")
                ),
                parameters.Order,
                "order wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Order, randomNumberGenerator), "order not prime");

            Assert.AreEqual(BigInteger.One, parameters.Cofactor, "cofactor wrong");
        }

        [Test]
        public void TestCurve25519CurveParameters()
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();

            var parameters = CurveParameters.Curve25519;

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Pow(2, 255) - 19
                ),
                parameters.Equation.Field.Modulo,
                "modulo wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Equation.Field.Modulo, randomNumberGenerator), "modulo not prime");
            
            Assert.AreEqual(
                BigInteger.Parse("486662"),
                parameters.Equation.A,
                "A wrong"
            );
            Assert.AreEqual(
                BigInteger.One,
                parameters.Equation.B,
                "B wrong"
            );
            
            Assert.AreEqual(
                new CurvePoint(
                    BigInteger.Parse("9"),
                    BigInteger.Parse("14781619447589544791020593568409986887264606134616475288964881837755586237401")
                ),
                parameters.Generator,
                "generator wrong"
            );

            Assert.That(parameters.Equation.IsPointOnCurve(parameters.Generator), "generator not on curve");

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Parse("7237005577332262213973186563042994240857116359379907606001950938285454250989")
                ),
                parameters.Order,
                "order wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Order, randomNumberGenerator), "order not prime");

            Assert.AreEqual(new BigInteger(8), parameters.Cofactor, "cofactor wrong");
        }

        [Test]
        public void TestM383CurveParameters()
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();

            var parameters = CurveParameters.M383;

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Pow(2, 383) - 187
                ),
                parameters.Equation.Field.Modulo,
                "module wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Equation.Field.Modulo, randomNumberGenerator), "module not prime");
            
            Assert.AreEqual(
                BigInteger.Parse("2065150"),
                parameters.Equation.A,
                "A wrong"
            );
            Assert.AreEqual(
                BigInteger.One,
                parameters.Equation.B,
                "B wrong"
            );
            
            Assert.AreEqual(
                new CurvePoint(
                    BigInteger.Parse("12"),
                    BigInteger.Parse("4737623401891753997660546300375902576839617167257703725630389791524463565757299203154901655432096558642117242906494")
                ),
                parameters.Generator,
                "generator wrong"
            );

            Assert.That(parameters.Equation.IsPointOnCurve(parameters.Generator), "generator not on curve");

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Parse("2462625387274654950767440006258975862817483704404090416746934574041288984234680883008327183083615266784870011007447")
                ),
                parameters.Order,
                "wrong prime"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Order, randomNumberGenerator), "order not prime");

            Assert.AreEqual(new BigInteger(8), parameters.Cofactor, "wrong cofactor");
        }

        [Test]
        public void TestM511CurveParameters()
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();

            var parameters = CurveParameters.M511;

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Pow(2, 511) - 187
                ),
                parameters.Equation.Field.Modulo,
                "modulo wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Equation.Field.Modulo, randomNumberGenerator), "modulo not prime");
            
            Assert.AreEqual(
                BigInteger.Parse("530438"),
                parameters.Equation.A,
                "A wrong"
            );
            Assert.AreEqual(
                BigInteger.One,
                parameters.Equation.B,
                "B wrong"
            );
            
            Assert.AreEqual(
                new CurvePoint(
                    BigInteger.Parse("5"),
                    BigInteger.Parse("2500410645565072423368981149139213252211568685173608590070979264248275228603899706950518127817176591878667784247582124505430745177116625808811349787373477")
                ),
                parameters.Generator,
                "generator wrong"
            );

            Assert.That(parameters.Equation.IsPointOnCurve(parameters.Generator), "generator not on curve");

            Assert.AreEqual(
                BigPrime.CreateWithoutChecks(
                    BigInteger.Parse("837987995621412318723376562387865382967460363787024586107722590232610251879607410804876779383055508762141059258497448934987052508775626162460930737942299 ")
                ),
                parameters.Order,
                "order wrong"
            );
            Assert.DoesNotThrow(() => BigPrime.Create(parameters.Order, randomNumberGenerator), "order not prime");

            Assert.AreEqual(new BigInteger(8), parameters.Cofactor, "cofactor wrong     ");
        }

    }
}
