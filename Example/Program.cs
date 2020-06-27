using System;

using CompactCryptoGroupAlgebra;
using System.Numerics;
using System.Security.Cryptography;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Choosing parameters for multiplicative group
            // order 11 subgroup with generator 4 of characteristic 23 multiplicative group 
            BigInteger prime = 23;
            BigInteger order = 11;
            BigInteger generator = 4;

            // Creating the group instance
            var group = new MultiplicativeCryptoGroup(prime, order, generator);
            DoDiffieHelman(group);
        }

        public static void DoDiffieHelman<T>(CryptoGroup<T> group) where T : notnull
        {
            // Instantiating a strong random number generator
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

            // Generating DH secret and public key for Alice
            (BigInteger dhSecretAlice, CryptoGroupElement<T> dhPublicAlice) = 
                group.GenerateRandom(randomNumberGenerator);

            // Generating DH secret and public key for Bob
            (BigInteger dhSecretBob, CryptoGroupElement<T> dhPublicBob) =
                group.GenerateRandom(randomNumberGenerator);

            // Computing shared secret for Alice and Bob
            CryptoGroupElement<T> sharedSecretBob = group.MultiplyScalar(dhPublicAlice, dhSecretBob);
            CryptoGroupElement<T> sharedSecretAlice = group.MultiplyScalar(dhPublicBob, dhSecretAlice);

            // Confirm that it's the same
            Debug.Assert(sharedSecretAlice.Equals(sharedSecretBob));

            Console.WriteLine($"Alice - Secret: {dhSecretAlice}, Public: {dhPublicAlice}");
            Console.WriteLine($"Bob   - Secret: {dhSecretBob}, Public: {dhPublicBob}");

            Console.WriteLine($"Alice - Result: {sharedSecretAlice}");
            Console.WriteLine($"Bob   - Result: {sharedSecretBob}");
        }
    }
}
