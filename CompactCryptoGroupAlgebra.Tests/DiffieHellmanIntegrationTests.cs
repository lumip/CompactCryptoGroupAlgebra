using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Globalization;
using System.Text.RegularExpressions;

using NUnit.Framework;
using CompactCryptoGroupAlgebra.TestUtils;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class DiffieHellmanIntegrationTests
    {
        public void DoDiffieHellman<T>(CryptoGroup<T> group) where T : notnull
        {
            RandomNumberGenerator randomNumberGenerator = new SeededRandomNumberGenerator();

            // Generating DH secret and public key for Alice
            (BigInteger dhSecretAlice, CryptoGroupElement<T> dhPublicAlice) = 
                group.GenerateRandom(randomNumberGenerator);

            // Generating DH secret and public key for Bob
            (BigInteger dhSecretBob, CryptoGroupElement<T> dhPublicBob) =
                group.GenerateRandom(randomNumberGenerator);

            // Computing shared secret for Alice and Bob
            CryptoGroupElement<T> sharedSecretBob = dhPublicAlice * dhSecretBob;
            CryptoGroupElement<T> sharedSecretAlice = dhPublicBob * dhSecretAlice;

            Assert.AreEqual(sharedSecretAlice, sharedSecretBob);
        }

        [Test]
        public void TestDiffieHellmanWithMultiplicativeGroup()
        {
            string primeHex = @"0
                FFFFFFFF FFFFFFFF C90FDAA2 2168C234 C4C6628B 80DC1CD1
                29024E08 8A67CC74 020BBEA6 3B139B22 514A0879 8E3404DD
                EF9519B3 CD3A431B 302B0A6D F25F1437 4FE1356D 6D51C245
                E485B576 625E7EC6 F44C42E9 A63A3620 FFFFFFFF FFFFFFFF";
                
            BigPrime prime = BigPrime.CreateWithoutChecks(
                BigInteger.Parse(Regex.Replace(primeHex, @"\s+", ""), NumberStyles.AllowHexSpecifier)
            );
            BigPrime order = BigPrime.CreateWithoutChecks((prime - 1) / 2);
            BigInteger generator = 4;

            var group = Multiplicative.MultiplicativeGroupAlgebra.CreateCryptoGroup(
                prime, order, generator
            );
            
            DoDiffieHellman(group);
        }

        [Test]
        public void TestDiffieHellmanWithNISTP256Curve()
        {
            var group = EllipticCurves.CurveGroupAlgebra.CreateCryptoGroup(
                EllipticCurves.CurveParameters.Curve25519
            );

            DoDiffieHellman(group);
        }

        [Test]
        public void TestDiffieHellmanWithCurve25519Curve()
        {
            var group = EllipticCurves.CurveGroupAlgebra.CreateCryptoGroup(
                EllipticCurves.CurveParameters.Curve25519
            );

            DoDiffieHellman(group);
        }

        [Test]
        public void TestDiffieHellmanWithXOnlyCurve25519Curve()
        {
            var group = EllipticCurves.XOnlyMontgomeryCurveAlgebra.CreateCryptoGroup(
                EllipticCurves.CurveParameters.Curve25519
            );
            
            DoDiffieHellman(group);
        }
    }
}