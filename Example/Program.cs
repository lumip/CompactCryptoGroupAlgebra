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
            ICryptoGroup group = new MultiplicativeCryptoGroup(prime, order, generator);
            // Instantiating a strong random number generator
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            // Generating DH secret and public key for Alice
            (BigInteger dhSecretAlice, ICryptoGroupElement dhPublicAlice) = group.GenerateRandom(rng);

            // Generating DH secret and public key for Bob
            (BigInteger dhSecretBob, ICryptoGroupElement dhPublicBob) = group.GenerateRandom(rng);

            // Computing shared secret for Alice and Bob
            ICryptoGroupElement sharedSecretBob = group.MultiplyScalar(dhPublicAlice, dhSecretBob);
            ICryptoGroupElement sharedSecretAlice = group.MultiplyScalar(dhPublicBob, dhSecretAlice);

            // Confirm that it's the same
            Debug.Assert(sharedSecretAlice.Equals(sharedSecretBob));

            Console.WriteLine($"Alice - Secret: {dhSecretAlice}, Public: {dhPublicAlice}");
            Console.WriteLine($"Bob   - Secret: {dhSecretBob}, Public: {dhPublicBob}");

            Console.WriteLine($"Alice - Result: {sharedSecretAlice}");
            Console.WriteLine($"Bob   - Result: {sharedSecretBob}");
        }
    }
}
