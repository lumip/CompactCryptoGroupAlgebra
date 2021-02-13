using NUnit.Framework;

using System.Numerics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CompactCryptoGroupAlgebra.OpenSsl
{
    public class DiffieHellmanIntegrationTests
    {

        [Test]
        public void TestDiffieHellmanWithNISTPrime256v1Curve()
        {
            var curveAlgebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var group = new CryptoGroup<SecureBigNumber, ECPoint>(curveAlgebra);
            CompactCryptoGroupAlgebra.DiffieHellmanIntegrationTests.DoDiffieHellman(group);
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
            BigNumber generator = new BigNumber(4);

            var algebra = new MultiplicativeGroupAlgebra(prime, order, generator);
            var group = new CryptoGroup<SecureBigNumber, BigNumber>(algebra);
            
            CompactCryptoGroupAlgebra.DiffieHellmanIntegrationTests.DoDiffieHellman(group);
        }

    }
}