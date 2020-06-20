﻿using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CompactCryptoGroupAlgebra
{
    /// <summary>
    /// Extends <see cref="RandomNumberGenerator"/> by a convenient way to
    /// sample a random <see cref="BigInteger"/>.
    /// </summary>
    public static class RandomNumberGeneratorExtensions
    {
        /// <summary>
        /// Returns a random <see cref="BigInteger"/> between (and including)
        /// <paramref name="lower"/> and <paramref name="upper"/>.
        /// </summary>
        /// <returns>The <see cref="T:System.Numerics.BigInteger"/>.</returns>
        /// <param name="rng">Random number generator.</param>
        /// <param name="lower">Inclusive lower bound.</param>
        /// <param name="upper">Inclusive upper bound.</param>
        public static BigInteger RandomBetween(
            this RandomNumberGenerator rng, BigInteger lower, BigInteger upper
        )
        {
            NumberLength length = NumberLength.GetLength(upper - lower);
            BigInteger delta;
            do
            {
                byte[] buffer = new byte[length.InBytes];
                rng.GetBytes(buffer);
                delta = new BigInteger(buffer);
            }
            while (delta >= upper - lower);
            delta *= delta.Sign;
            Debug.Assert(delta >= BigInteger.Zero);
            return lower + delta;
        }
    }
}
