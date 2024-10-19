# CompactCryptoGroupAlgebra

![Build Status](https://github.com/lumip/CompactCryptoGroupAlgebra/actions/workflows/main.yml/badge.svg?branch=master) [![Coverage Status](https://coveralls.io/repos/github/lumip/CompactCryptoGroupAlgebra/badge.svg?branch=master)](https://coveralls.io/github/lumip/CompactCryptoGroupAlgebra?branch=master)

A compact API and implementation of abelian group algebra commonly used in asymmetric cryptography, fully written in C#.

These groups are mathematical structures which are characterized by a set of group elements, an commutative addition operation on these elements and multiplication of a group element with a scalar. Additionally there exists a generator, i.e., an element that allows to obtain all other group elements by means of scalar multiplication with a unique factor for each element.

The aim of this project is to provide a basis for this kind of cryptographic algebra that is both, simple to use and easy to extend and customise. It also serves as a simple showcase on how concrete algebraic structures, such as elliptic curves, may be implemented in principle, without obfuscating the fundamentals for purposes of performance and security.

__!Security Advisory!__ Note that due to its focus on simplicity `CompactCryptoGroupAlgebra` is _neither_ a _fully secure_ implementation _nor_ the _most performant_. It is intended for experimental and educational purposes. If you require strict security, please use established cryptography libraries. A secure implementation of `CompactCryptoGroupAlgebra` interfaces using native calls to OpenSSL's libcrypto library is made available by the `CompactCryptoGroupAlgebra.LibCrypto` library that is included in this repository but published separately.

## Features

- Generic classes `CryptoGroup` and `CryptoGroupElement` that user code interfaces with for group operations
- Available implementations of `CryptoGroup`:
  - Multiplicative groups of a field with prime characteristic
  - Elliptic curves in Weierstrass form, e.g., the NIST-P256 curve
  - Elliptic curves in Montgomery form, e.g., Curve25519
  - The x-coordinate-only variant of Montgomery curves

## Installing

`CompactCryptoGroupAlgebra` can be installed from [nuget](https://www.nuget.org/packages/CompactCryptoGroupAlgebra/).
Follow the link for instructions on how to install using your preferred method (package manager, .net cli, etc).

## Usage

The public API presents the two generic base classes `CryptoGroup` and `CryptoGroupElement` which are agnostic of the underlying instantiation and implementation of the group.

In addition, `CompactCryptoGroupAlgebra` currently provide group instantiations based on the multiplicative group of a finite field as well as the NIST-P256 and the Curve25519 elliptic curves.

Performing a Diffie-Hellman Key Exchange on a multiplicative group may look like

```c#

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
            var group = MultiplicativeGroupAlgebra.CreateCryptoGroup(prime, order, generator);
            DoDiffieHellman(group, randomNumberGenerator);
        }

        private static void DoDiffieHellman<TScalar, TElement>(
            CryptoGroup<TScalar, TElement> group, RandomNumberGenerator randomNumberGenerator
        ) where TScalar : notnull where TElement : notnull
        {
            // Generating DH secret and public key for Alice
            (TScalar dhSecretAlice, CryptoGroupElement<TScalar, TElement> dhPublicAlice) =
                group.GenerateRandom(randomNumberGenerator);

            // Generating DH secret and public key for Bob
            (TScalar dhSecretBob, CryptoGroupElement<TScalar, TElement> dhPublicBob) =
                group.GenerateRandom(randomNumberGenerator);

            // Computing shared secret for Alice and Bob
            CryptoGroupElement<TScalar, TElement> sharedSecretBob = dhPublicAlice * dhSecretBob;
            CryptoGroupElement<TScalar, TElement> sharedSecretAlice = dhPublicBob * dhSecretAlice;

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

## API Overview

Functionality of `CompactCryptoGroupAlgebra` is split over a range of classes, each with a single specific purpose, the most important of which are highlighted below.

- `CryptoGroupElement<T>` represents an element of a cryptographic group and implements operators for ease of use, abstracting from a specific underlying implementation type via its template type argument.
- `CryptoGroup<T>` is a wrapper around `ICryptoGroupAlgebra<T>` that ensures that all returned values are returned as `CryptoGroupElement<T>` instances.
- `ICryptoGroupAlgebra<T>` is the common interface for implementations of a specific mathematical group structure using the underlying type `T`
- `CryptoGroupAlgebra<T>` is an abstract base class for implementations of `ICryptoGroupAlgebra<T>`, providing common implementations derived from fundamental group operations (such as generating and inverting group elements).
- `Multiplicative.MultiplicativeGroupAlgebra` is an implementation of `CryptoGroupAlgebra` for multiplicative groups in fields of prime characteristic.
- `EllipticCurves.CurveGroupAlgebra` is an implementation of `CryptoGroupAlgebra` for elliptic curves that in turn relies on a specific `CurveEquation` instance for fundamental operations.
- Subclasses of `EllipticCurves.CurveEquation` provide the implementations of specific forms of elliptic curves (currently, `EllipticCurves.WeierstrassCurveEquation` and `EllipticCurves.MontgomeryCurveEquation` are provided).
- `EllipticCurves.XOnlyMontgomeryCurveAlgebra` implements the x-coordinate-only implementation of Montgomery curves
- `EllipticCurves.CurveParameters` encapsulates numeric parameters of a specific curve (as opposed to `EllipticCurves.CurveEquation`, which implements a specific curve _form_ but does not provide values for the curve's parameters).

### Instantiating `CryptoGroup`

To obtain a usable instance of `CryptoGroup` with any of the provided implementations, use the method

```c#
Multiplicative.MultiplicativeGroupAlgebra.CreateCryptoGroup(prime, order, generator);
```

for an adequate choice of `prime`, `order` and `generator` or

```c#
EllipticCurves.CurveGroupAlgebra.CreateCryptoGroup(curveParameters);
```

with a `CurveParameters` instance.

`CurveParameters` provides preconfigured instances for the following standardized elliptic curves

```c#
CurveParameters.NISTP256
CurveParameters.NISTP384
CurveParameters.NISTP521
CurveParameters.Curve25519
CurveParameters.M383
CurveParameters.M511
```

but you also have the option of instantiating an instance for your own curve.

## License

`CompactCryptoGroupAlgebra` is licensed under the [GPLv3 license](/LICENSES/GPL-3.0-or-later.txt) (or any later version)
for general use. If you would like to use `CompactCryptoGroupAlgebra` under different terms, contact the authors.

`CompactCryptoGroupAlgebra` aims to be [REUSE Software](https://reuse.software/) compliant to facilitate easy reuse.

## Versioning

`CompactCryptoGroupAlgebra` version numbers adhere to [Semantic Versioning](https://semver.org).
