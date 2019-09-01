using System.Numerics;
using System.Diagnostics;

namespace CompactEC
{
    class BigIntegerRing
    {
        public BigInteger Modulo { get; }
        public int ElementByteLength { get; }

        public BigIntegerRing(BigInteger primeModulo)
        {
            Modulo = primeModulo;
            ElementByteLength = primeModulo.ToByteArray().Length;
        }

        public BigInteger Pow(BigInteger x, BigInteger e)
        {
            Debug.Assert(e >= BigInteger.Zero);
            return BigInteger.ModPow(Mod(x), e, Modulo);
        }

        public BigInteger Square(BigInteger x)
        {
            return Pow(x, 2);
        }

        public BigInteger InvertMult(BigInteger x)
        {
            return Pow(x, Modulo - 2);
        }

        public BigInteger Mod(BigInteger x)
        {
            return (x % Modulo + Modulo) % Modulo; // to prevent negative results of BigInteger modulo
        }
    }
}
