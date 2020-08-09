using System;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra.TestUtils
{
    public class SeededRandomNumberGenerator : RandomNumberGenerator
    {
        private readonly Random _random;

        public SeededRandomNumberGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public SeededRandomNumberGenerator() : this(0) { }

        public override void GetBytes(byte[] data)
        {
            _random.NextBytes(data);
        }
    }
}
