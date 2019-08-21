using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CompactEC.CryptoAlgebra;

namespace CompactEC.UnitTests.CryptoAlgebra
{
    [TestClass]
    public class MultiplicativeGroupAlgebraTest
    {
        [TestMethod]
        public void TestMultiplyScalarSmall()
        {
            MultiplicativeGroupAlgebra groupAlgebra = new MultiplicativeGroupAlgebra(11, 5, 3, 1, 1);

            var x = new BigInteger(3);

            var expectedRs = new BigInteger[] { 9, 5, 4, 1 };
            for (int i = 0; i < expectedRs.Length; ++i)
            {
                var actualR = groupAlgebra.MultiplyScalar(x, 2 + i);
                Assert.AreEqual(expectedRs[i], actualR);
            }

            groupAlgebra = new MultiplicativeGroupAlgebra(11, 10, 2, 1, 1);

            x = new BigInteger(2);

            expectedRs = new BigInteger[] { 2, 4, 8, 5, 10, 9, 7, 3, 6, 1 };
            for (int i = 0; i < expectedRs.Length; ++i)
            {
                var actualR = groupAlgebra.MultiplyScalar(x, 1 + i);
                Assert.AreEqual(expectedRs[i], actualR);
            }
        }

        //[TestMethod]
        //public void TestMultiplyScalarLarge()
        //{
        //    using (var cryptoContext = CryptoContext.CreateDefault())
        //    {
        //        MultiplicativeGroupAlgebra groupAlgebra = new MultiplicativeGroupAlgebra(SecurityParameters.CreateDefault768Bit());

        //        var x = groupAlgebra.GenerateElement(cryptoContext.RandomNumberGenerator.GetBigInteger(groupAlgebra.FactorSize));
        //        var k = cryptoContext.RandomNumberGenerator.GetBigInteger(groupAlgebra.FactorSize);

        //        var expectedR = groupAlgebra.MultiplyScalarUnsafe(x, k);
        //        var actualR = groupAlgebra.MultiplyScalar(x, k);
        //        Assert.AreEqual(expectedR, actualR);
        //    }
        //}
    }
}
