using System;
using System.Numerics;
using System.Security.Cryptography;

using CompactCryptoGroupAlgebra;
using CompactCryptoGroupAlgebra.Multiplicative;

namespace Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Instantiating a strong random number generator
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

            // Choosing parameters for multiplicative group
            // order 11 subgroup with generator 4 of characteristic 23 multiplicative group 
            BigPrime prime = BigPrime.Create(23, randomNumberGenerator);
            BigPrime order = BigPrime.Create(11, randomNumberGenerator);
            BigInteger generator = 4;

            // Creating the group instance
            CryptoGroup<BigInteger> group = MultiplicativeGroupAlgebra.CreateCryptoGroup(prime, order, generator);
            DoDiffieHellman(group, randomNumberGenerator);
        }

        private static void DoDiffieHellman<T>(
            CryptoGroup<T> group, RandomNumberGenerator randomNumberGenerator
        ) where T : notnull
        {
            // Generating DH secret and public key for Alice
            (BigInteger dhSecretAlice, CryptoGroupElement<T> dhPublicAlice) = 
                group.GenerateRandom(randomNumberGenerator);

            // Generating DH secret and public key for Bob
            (BigInteger dhSecretBob, CryptoGroupElement<T> dhPublicBob) =
                group.GenerateRandom(randomNumberGenerator);

            // Computing shared secret for Alice and Bob
            CryptoGroupElement<T> sharedSecretBob = dhPublicAlice * dhSecretBob;
            CryptoGroupElement<T> sharedSecretAlice = dhPublicBob * dhSecretAlice;

            // Confirm that it's the same
            Debug.Assert(sharedSecretAlice.Equals(sharedSecretBob));

            Console.WriteLine($"Alice - Secret: {dhSecretAlice}, Public: {dhPublicAlice}");
            Console.WriteLine($"Bob   - Secret: {dhSecretBob}, Public: {dhPublicBob}");

            Console.WriteLine($"Alice - Result: {sharedSecretAlice}");
            Console.WriteLine($"Bob   - Result: {sharedSecretBob}");
        }
    }
}
