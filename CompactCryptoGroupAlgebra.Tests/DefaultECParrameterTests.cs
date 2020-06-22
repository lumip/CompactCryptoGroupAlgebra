using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.Tests
{
    [TestFixture]
    public class DefaultECParrameterTests
    {
        [Test]
        public void TestNISTP256()
        {
            ECParameters parameters = ECParameters.CreateNISTP256();
            Assert.AreEqual(BigInteger.Parse("41058363725152142129326129780047268409114441015993725554835256314039467401291"), parameters.B);
            Assert.AreEqual(BigInteger.Parse("48439561293906451759052585252797914202762949526041747995844080717082404635286"), parameters.Generator.X);
            Assert.AreEqual(BigInteger.Parse("36134250956749795798585127919587881956611106672985015071877198253568414405109"), parameters.Generator.Y);
            var groupAlgebra = new ECGroupAlgebra(parameters);
            var elem = groupAlgebra.GenerateElement(parameters.Order);
            Assert.AreEqual(groupAlgebra.NeutralElement, elem);

            elem = groupAlgebra.GenerateElement(BigInteger.Parse("28367823582636726875877070967237095880621"));
            Assert.AreEqual(groupAlgebra.NeutralElement, groupAlgebra.Add(elem, groupAlgebra.Negate(elem)));
        }

        [Test]
        public void TestConstructorFailsForNonPrimeP()
        {
            Assert.Throws<ArgumentException>(
                () => new ECParameters(10, 0, 0, ECPoint.PointAtInfinity, 3, 1, new SeededRandomNumberGenerator())
            );
        }

        [Test]
        public void TestConstructorFailsForNonPrimeOrder()
        {
            Assert.Throws<ArgumentException>(
                () => new ECParameters(5, 0, 0, ECPoint.PointAtInfinity, 4, 1, new SeededRandomNumberGenerator())
            );
        }

    }
}
