using System;
using System.Numerics;
using System.Security.Cryptography;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

namespace CompactCryptoGroupAlgebra.OpenSsl.Multiplicative
{

    /// <summary>
    /// OpenSSL-based cryptographic group algebra based on multiplication in a prime field.
    ///
    /// <see cref="MultiplicativeGroupAlgebra" /> uses OpenSSL based <see cref="BigNumber" />s
    /// as raw group elements and <see cref="SecureBigNumber" />s as scalar factors and private keys.
    /// </summary>
    public sealed class MultiplicativeGroupAlgebra : ICryptoGroupAlgebra<SecureBigNumber, BigNumber>, IDisposable
    {

        BigNumber _modulo;
        BigNumber _order;

        /// <inheritdoc />
        public BigPrime Order { get; private set; }

        /// <inheritdoc />
        public BigNumber Generator { get; private set; }

        /// <inheritdoc />
        public BigNumber NeutralElement => BigNumber.One;

        /// <inheritdoc />
        public BigInteger Cofactor { get; private set; }

        /// <inheritdoc />
        public int ElementBitLength => _modulo.Length.InBits;

        /// <inheritdoc />
        public int OrderBitLength => _order.Length.InBits;

        /// <inheritdoc />
        public int SecurityLevel { get; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultiplicativeGroupAlgebra"/>
        /// class given the group's parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public MultiplicativeGroupAlgebra(BigPrime prime, BigPrime order, BigNumber generator)
        {
            _modulo = new BigNumber(prime);
            Order = order;
            _order = new BigNumber(order);
            // copy generator to ensure that disposing the original doesn't hurt us
            Generator = BigNumber.FromRawHandle(generator.Handle);
            Cofactor = (prime - 1) / order;

            if (!IsSafeElement(Generator))
                throw new ArgumentException("The generator must be an element of the group.", nameof(generator));

            SecurityLevel = 
                CompactCryptoGroupAlgebra.Multiplicative.MultiplicativeGroupAlgebra.ComputeSecurityLevel(prime, order);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultiplicativeGroupAlgebra"/>
        /// class given the group's parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public MultiplicativeGroupAlgebra(BigPrime prime, BigPrime order, BigInteger generator)
            : this(prime, order, new BigNumber(generator))
        {
        }

        /// <inheritdoc />
        public BigNumber Add(BigNumber left, BigNumber right)
        {
            return left.ModMul(right, _modulo);
        }

        /// <inheritdoc />
        public BigNumber FromBytes(byte[] buffer)
        {
            return new BigNumber(buffer);
        }

        /// <inheritdoc />
        public BigNumber GenerateElement(SecureBigNumber index)
        {
            return MultiplyScalar(Generator, index);
        }

        /// <inheritdoc />
        public (SecureBigNumber, BigNumber) GenerateRandomElement(RandomNumberGenerator randomNumberGenerator)
        {
            var index = SecureBigNumber.Random(_order);
            var element = GenerateElement(index);
            return (index, element);
        }

        /// <inheritdoc />
        public bool IsPotentialElement(BigNumber element)
        {
            // implementation-specific checks
            if (element.Equals(BigNumber.Zero) ||
                BigNumberHandle.Compare(element.Handle, _modulo.Handle) >= 0)
            {
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        public bool IsSafeElement(BigNumber element)
        {
            if (!IsPotentialElement(element)) return false;

            // verifying that the point is not from a small subgroup of the whole curve (and thus outside
            // of the safe subgroup over which operations are considered)
            using (var cofactorBignum = new BigNumber(Cofactor))
            {
                var check = element.ModExp(cofactorBignum, _modulo);
                if (check.Equals(NeutralElement))
                    return false;
            }
            return true;
        }

        /// <inheritdoc />
        public BigNumber MultiplyScalar(BigNumber e, SecureBigNumber k)
        {
            return e.ModExp(k, _modulo);
        }

        /// <inheritdoc />
        public BigNumber Negate(BigNumber element)
        {
            return element.ModReciprocal(_modulo);
        }

        /// <inheritdoc />
        public byte[] ToBytes(BigNumber element)
        {
            return element.ToBytes();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _modulo.Dispose();
                _order.Dispose();
                Generator.Dispose();
            }
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            MultiplicativeGroupAlgebra? other = obj as MultiplicativeGroupAlgebra;
            if (other == null) return false;

            return _modulo.Equals(other._modulo) && Generator.Equals(other.Generator);
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = 19973;
            hashCode = hashCode * 151 + _modulo.GetHashCode();
            hashCode = hashCode * 151 + Generator.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Creates a <see cref="CryptoGroup{SecureBigNumber, BigNumber}" /> instance using a <see cref="MultiplicativeGroupAlgebra" />
        /// instance with the given parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public static CryptoGroup<SecureBigNumber, BigNumber> CreateCryptoGroup(
            BigPrime prime, BigPrime order, BigInteger generator
        )
        {
            return new CryptoGroup<SecureBigNumber, BigNumber>(new MultiplicativeGroupAlgebra(
                prime, order, generator
            ));
        }

    }

}