using NUnit.Framework;

namespace CompactCryptoGroupAlgebra
{
    [TestFixture]
    public class DebugAssertTests
    {
        [Test]
        public void TestDebugAssertDoesNotThrowIfTrue()
        {
            Assert.DoesNotThrow(() => Debug.Assert(true));
        }

        [Test]
        public void TestDebugAssertDoesThrowIfFalse()
        {
            if (Debug.IsActive)
            {
                var msg = "Assertion error!";
                var ex = Assert.Throws<AssertionException>(() => Debug.Assert(false, msg));
                Assert.AreEqual(msg, ex.Message);

                ex = Assert.Throws<AssertionException>(() => Debug.Assert(false));
                Assert.AreEqual("Assertion failure!", ex.Message);
            }
            else
            {
                Assert.DoesNotThrow(() => Debug.Assert(true));
            }
        }
    }
}
