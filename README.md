# CompactCryptoGroupAlgebra

[![Build Status](https://travis-ci.com/lumip/CompactCryptoGroupAlgebra.svg?branch=master)](https://travis-ci.com/lumip/CompactCryptoGroupAlgebra) [![Coverage Status](https://coveralls.io/repos/github/lumip/CompactCryptoGroupAlgebra/badge.svg?branch=master)](https://coveralls.io/github/lumip/CompactCryptoGroupAlgebra?branch=master)

A minimalist API and implementation of group algebra commonly used in asymmetric cryptography.

## NOTE: documentation is work in progress and API and code are still subject to changes

These groups are mathematical structures which are characterized by a set of group elements, an addition operation on these elements and multiplication of a group element with a scalar as well as a generator, i.e., an element that allows to uniquely obtain all other group elements by means of scalar multiplication.

The aim of this project is to provide a basis for this kind of cryptographic algebra that is both, simple to use and easy to extend and customise.

## Features



## Usage

The public API presents the two generic interfaces `ICryptoGroup` and `ICryptoGroupElement` which are completely agnostic of the underlying instantiation and implementation of the group. 

In addition, we currently provide group instantiations based on the multiplicative group of a finite field as well as the NIST-P256 and the Curve25519 elliptic curves.

Performing a Diffie-Hellman Key Exchange on a multiplicative group looks like

```c#
using System;
using System.Numerics;
using System.Security.Cryptography;

using CompactCryptoGroupAlgebra;
using CompactCryptoGroupAlgebra.Multiplicative;

namespace Example
{
    public class Program
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
            var group = MultiplicativeGroupAlgebra.CreateCryptoGroup(prime, order, generator);
            DoDiffieHellman(group, randomNumberGenerator);
        }

        public static void DoDiffieHellman<T>(
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

```

Note that all operations specific to the DH key exchange only use the abstract interfaces. We can therefore choose any group implementation instead
of the multiplicative prime field group.

## Organization





