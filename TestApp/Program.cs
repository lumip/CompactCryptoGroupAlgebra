using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompactEC;
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
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
            Tuple<BigInteger, ICryptoGroupElement> dhElementAlice = group.GenerateRandom(rng);
            BigInteger dhSecretAlice = dhElementAlice.Item1;
            ICryptoGroupElement dhPublicAlice = dhElementAlice.Item2;

            // Generating DH secret and public key for Bob
            Tuple<BigInteger, ICryptoGroupElement> dhElementBob = group.GenerateRandom(rng);
            BigInteger dhSecretBob = dhElementBob.Item1;
            ICryptoGroupElement dhPublicBob = dhElementBob.Item2;

            // Computing shared secret for Alice and Bob
            ICryptoGroupElement sharedSecretBob = group.MultiplyScalar(dhPublicAlice, dhSecretBob);
            ICryptoGroupElement sharedSecretAlice = group.MultiplyScalar(dhPublicBob, dhSecretAlice);

            // Confirm that it's the same
            Debug.Assert(sharedSecretAlice.Equals(sharedSecretBob));

            Console.WriteLine("Alice - Secret: {0}, Public: {1}", dhSecretAlice, dhPublicAlice);
            Console.WriteLine("Bob   - Secret: {0}, Public: {1}", dhSecretBob, dhPublicBob);

            Console.WriteLine("Alice - Result: {0}", sharedSecretAlice);
            Console.WriteLine("Bob   - Result: {0}", sharedSecretBob);
        }
    }
}
