using System;
using System.Numerics;
using System.Security.Cryptography;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

namespace CompactCryptoGroupAlgebra.OpenSsl
{

    public class MultiplicativeGroupAlgebra : ICryptoGroupAlgebra<SecureBigNumber, BigNumber>, IDisposable
    {

        BigNumber _modulo;
        BigNumber _order;

        public BigPrime Order { get; private set; }

        public BigNumber Generator { get; private set; }

        public BigNumber NeutralElement => BigNumber.One;

        public BigInteger Cofactor { get; private set; }

        public int ElementBitLength => _modulo.Length.InBits;

        public int OrderBitLength => _order.Length.InBits;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CompactCryptoGroup.OpenSsl.MultiplicativeGroupAlgebra"/>
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

            if (!IsElement(Generator))
                throw new ArgumentException("The generator must be an element of the group.", nameof(generator));
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CompactCryptoGroup.OpenSsl.MultiplicativeGroupAlgebra"/>
        /// class given the group's parameters.
        /// </summary>
        /// <param name="prime">The prime modulo of the group.</param>
        /// <param name="order">The order of the group</param>
        /// <param name="generator">The generator of the group.</param>
        public MultiplicativeGroupAlgebra(BigPrime prime, BigPrime order, BigInteger generator)
            : this(prime, order, new BigNumber(generator))
        {
        }

        public BigNumber Add(BigNumber left, BigNumber right)
        {
            return left.ModMul(right, _modulo);
        }

        public BigNumber FromBytes(byte[] buffer)
        {
            return new BigNumber(buffer);
        }

        public BigNumber GenerateElement(SecureBigNumber index)
        {
            return MultiplyScalar(Generator, index);
        }

        public (SecureBigNumber, BigNumber) GenerateRandomElement(RandomNumberGenerator randomNumberGenerator)
        {
            var index = SecureBigNumber.Random(_order);
            var element = GenerateElement(index);
            return (index, element);
        }

        public bool IsElement(BigNumber element)
        {
            // implementation-specific checks
            if (element.Equals(BigNumber.Zero) ||
                BigNumberHandle.Compare(element.Handle, _modulo.Handle) >= 0)
            {
                return false;
            }

            // verifying that the point is not from a small subgroup of the whole curve (and thus outside
            // of the safe subgroup over which operations are considered)
            if (Cofactor > 1)
            {
                using (var cofactorBignum = new BigNumber(Cofactor))
                {
                    var check = element.ModExp(cofactorBignum, _modulo);
                    if (check.Equals(NeutralElement))
                        return false;
                }
            }
            return true;
        }

        public BigNumber MultiplyScalar(BigNumber e, SecureBigNumber k)
        {
            return e.ModExp(k, _modulo);
        }

        public BigNumber Negate(BigNumber element)
        {
            return element.ModReciprocal(_modulo);
        }

        public byte[] ToBytes(BigNumber element)
        {
            return element.ToBytes();
        }

        /// <inheritdocs />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _modulo.Dispose();
                _order.Dispose();
                Generator.Dispose();
            }
        }

        /// <inheritdocs />
        public override bool Equals(object? obj)
        {
            MultiplicativeGroupAlgebra? other = obj as MultiplicativeGroupAlgebra;
            if (other == null) return false;

            return _modulo.Equals(other._modulo) && Generator.Equals(other.Generator);
        }
        
        /// <inheritdocs />
        public override int GetHashCode()
        {
            int hashCode = 19973;
            hashCode = hashCode * 151 + _modulo.GetHashCode();
            hashCode = hashCode * 151 + Generator.GetHashCode();
            return hashCode;
        }

    }

}