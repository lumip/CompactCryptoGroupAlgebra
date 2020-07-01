using System;
using System.Numerics;

using NUnit.Framework;

namespace CompactCryptoGroupAlgebra.EllipticCurves.Tests
{
    [TestFixture]
    public class CurveCryptoGroupTests
    {
        private readonly CurveParameters ecParams;

        public CurveCryptoGroupTests()
        {
            ecParams = new CurveParameters(
                p: BigPrime.CreateWithoutChecks(23),
                a: -2,
                b: 9,
                generator: new CurvePoint(5, 3),
                order: BigPrime.CreateWithoutChecks(11),
                cofactor: 2
            );
        }

        /// <summary>
        /// This indirectly tests <see cref="CurveCryptoGroup.CreateGroupElement(byte[])"/>.
        /// </summary>
        [Test]
        public void TestFromBytes()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);
            var group = new CurveCryptoGroup(groupAlgebra);

            var expectedRaw = new CurvePoint(5, 3);
            var expected = new CryptoGroupElement<CurvePoint>(expectedRaw, groupAlgebra);
            var bytes = expected.ToBytes();
            var result = group.FromBytes(bytes);
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Indirectly also tests <see cref="CurveCryptoGroup.CreateGroupElement(CurvePoint)"/>.
        /// </summary>
        [Test]
        public void TestConstructorAlgebra()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);
            var group = new CurveCryptoGroup(groupAlgebra);

            var expectedGenerator = new CryptoGroupElement<CurvePoint>(ecParams.Generator, groupAlgebra);

            var resultGenerator = group.Generator;

            Assert.AreEqual(expectedGenerator, resultGenerator, "verifying generator");
            Assert.AreEqual(ecParams.Order, group.Order, "verifying order");
        }

        /// <summary>
        /// Indirectly also tests <see cref="CurveCryptoGroup.CreateGroupElement(CurvePoint)"/>.
        /// </summary>
        [Test]
        public void TestConstructorParameters()
        {
            var groupAlgebra = new CurveGroupAlgebra(ecParams);
            var group = new CurveCryptoGroup(ecParams);

            var expectedGenerator = new CryptoGroupElement<CurvePoint>(ecParams.Generator, groupAlgebra);

            var resultGenerator = group.Generator;

            Assert.AreEqual(expectedGenerator, resultGenerator, "verifying generator");
            Assert.AreEqual(ecParams.Order, group.Order, "verifying order");
        }
    }
}
