
# CompactCryptoGroupAlgebra.OpenSsl

`CompactCryptoGroupAlgebra.OpenSsl` is a separate library implementing the interfaces declared in `CompactCryptoGroupAlgebra` using native calls to the [OpenSSL library](https://www.openssl.org). This is intended to provide an actually secure implementation of the interfaces.

## Licensing Acknowledgements
This library relies on software developed by the OpenSSL Project for use in the OpenSSL Toolkit. [https://www.openssl.org](https://www.openssl.org) and cryptographic software written by Eric Young (eay@cryptsoft.com).

`CompactCryptoGroupAlgebra.OpenSsl` is not endorsed by, nor in any way associated with, the OpenSSL Project or Eric Young.

<!-- ## Installing

`CompactCryptoGroupAlgebra.OpenSsl` can be installed from [nuget](https://www.nuget.org/packages/CompactCryptoGroupAlgebra/1.0.0).
Follow the link for instructions on how to install using your preferred method (package manager, .net cli, etc). -->

## Usage

The public API provides classes `Multiplicative.MultiplicativeGroupAlgebra` and `EllipticCurves.EllipticCurveAlgebra` that implement the `CompactCryptoGroupAlgebra.ICryptoGroupAlbebra` using OpenSSL routines. They can therefore be used directly as the algebra implementations of `CompactCryptoGroupAlgebra.CryptoGroup` for full compatibility.

Instantiating a multiplicative group may look like:

```c#
using CompactCryptoGroupAlgebra.OpenSsl.Multiplicative;

// Instantiating a strong random number generator
RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

// Choosing parameters for multiplicative group
// order 11 subgroup with generator 4 of characteristic 23 multiplicative group
BigPrime prime = BigPrime.Create(23, randomNumberGenerator);
BigPrime order = BigPrime.Create(11, randomNumberGenerator);
BigInteger generator = 4;

// Creating the multiplicative group instance
var multiplicativeGroup = MultiplicativeGroupAlgebra.CreateCryptoGroup(prime, order, generator);
```

Groups over elliptic curves can be instantiated for common curves using a curve id:

```c#
using CompactCryptoGroupAlgebra.OpenSsl.EllipticCurves;

var ecGroup = EllipticCurveAlgebra.CreateCryptoGroup(EllipticCurveID.Prime256v1);
```


## License

`CompactCryptoGroupAlgebra.OpenSsl` is licensed under the [GPLv3 license](/LICENSE.txt) for general use.

## Versioning

`CompactCryptoGroupAlgebra.OpenSsl` version numbers adhere to [Semantic Versioning](https://semver.org) and are independent of `CompactCryptoGroupAlgebra` and OpenSSL version numbers.
