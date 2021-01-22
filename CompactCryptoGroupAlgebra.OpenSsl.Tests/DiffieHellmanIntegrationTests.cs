using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.OpenSsl
{
    public class DiffieHellmanIntegrationTests
    {

        [Test]
        public void TestPrime256v1()
        {
            var curveAlgebra = new EllipticCurveAlgebra(EllipticCurveID.Prime256v1);
            var group = new CryptoGroup<ECPoint>(curveAlgebra);
            CompactCryptoGroupAlgebra.DiffieHellmanIntegrationTests.DoDiffieHellman(group);
        }

    }
}