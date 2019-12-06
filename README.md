# CompactCryptoGroupAlgebra

A minimalist API and implementation of group algebra commonly used in asymmetric cryptography.

These groups are mathematical structures which are characterized by a set of group elements, an addition operation on these elements and multiplication of a group element with a scalar as well as a generator, i.e., an element that allows to uniquely obtain all other group elements by means of scalar multiplication.

The aim of this project is to provide a basis for this kind of cryptographic algebra that is both, simple to use and easy to extend and customise.

## Features



## Usage

The public API presents the two generic interfaces `ICryptoGroup` and `ICryptoGroupElement` which are completely agnostic of the underlying instantiation and implementation of the group. 

In addition, we currently provide group instantiations based on the multiplicative group of a finite field as well as the NIST-P256 elliptic curve.

Performing a Diffie-Helman Key Exchange on the multiplicative group characterized by prime 11 (where 7 is a generator) might look like

```c#
using CompactEC;
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;

// Choosing parameters for multiplicative group
BigInteger prime = 11;
BigInteger order = 10;
BigInteger generator = 7;

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
```

## Organization





