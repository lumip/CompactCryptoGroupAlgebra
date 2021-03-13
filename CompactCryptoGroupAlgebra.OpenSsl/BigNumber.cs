using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Numerics;

using CompactCryptoGroupAlgebra.OpenSsl.Internal.Native;

[assembly:InternalsVisibleTo("CompactCryptoGroupAlgebra.OpenSsl.Tests")]
namespace CompactCryptoGroupAlgebra.OpenSsl
{

    /// <summary>
    /// An arbitrarily large integer number.
    /// </summary>
    /// <remarks>
    /// This is essentially a managed code wrapper class for OpenSSL's <c>BIGNUM</c> structure.
    /// </remarks>
    public sealed class BigNumber : IDisposable
    {

        internal BigNumberHandle Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// The length of the integer in bits.
        /// </summary>
        public NumberLength Length
        {
            get
            {
                return NumberLength.FromBitLength(BigNumberHandle.GetNumberOfBits(Handle));
            }
        }

        /// <summary>
        /// A <see cref="BigNumber" /> instance representing the value zero.
        /// </summary>
        public static BigNumber Zero = new BigNumber(0);

        /// <summary>
        /// A <see cref="BigNumber" /> instance representing the value one.
        /// </summary>
        public static BigNumber One = new BigNumber(1);

        /// <summary>
        /// Creates a <see cref="BigNumber" /> instance from a valid <see cref="BigNumberHandle" />
        /// to an OpenSSL <c>BIGNUM</c> structure. A copy of the pointed to <c>BIGNUM</c> structure
        /// is made for the created instance.
        /// </summary>
        /// <param name="bigNumberHandle">
        /// A handle to a raw OpenSSL <c>BIGNUM</c> structure with which to initialize the new <see cref="BigNumber" />.
        /// </param>
        /// <returns>
        /// A new <see cref="BigNumber" /> instance with the same value as
        /// referred to by <paramref name="bigNumberHandle"/>.
        /// </returns>
        internal static BigNumber FromRawHandle(BigNumberHandle bigNumberHandle)
        {
            if (bigNumberHandle.IsInvalid)
            {
                throw new ArgumentException("The provided handle is invalid.", nameof(bigNumberHandle));
            }
            if (BigNumberHandle.GetFlags(bigNumberHandle).HasFlag(BigNumberFlags.Secure))
            {
                throw new ArgumentException(
                    "The provided handle is that of a secure big number. Converting secure into regular big numbers is not supported.",
                    nameof(bigNumberHandle)
                );
            }
            var bn = new BigNumber();
            BigNumberHandle.Copy(bn.Handle, bigNumberHandle);
            return bn;
        }

        /// <summary>
        /// Internal constructor for a given valid <see cref="BigNumberHandle" />.
        /// </summary>
        private BigNumber(BigNumberHandle handle)
        {
            Debug.Assert(!handle.IsInvalid, "BigNumberBase received an invalid handle internally!");
            Handle = handle;
        }

        /// <summary>
        /// Creates an unintialized <see cref="BigNumber" /> instance.
        /// </summary>
        public BigNumber() : this(BigNumberHandle.Create()) { }

        /// <summary>
        /// Creates a <see cref="BigNumber" /> instance from a
        /// <see cref="BigInteger" />.
        /// </summary>
        public BigNumber(BigInteger x) : this(x.ToByteArray()) { }

        /// <summary>
        /// Creates a <see cref="BigNumber" /> instance from a 
        /// little endian byte encoding.
        /// </summary>
        public BigNumber(byte[] buffer) : this()
        {
            BigNumberHandle.FromBytes(buffer, Handle);
        }

        /// <summary>
        /// Creates a <see cref="BigNumber" /> instance from a
        /// <see cref="ulong" /> number.
        /// </summary>
        public BigNumber(ulong x) : this()
        {
            BigNumberHandle.SetWord(Handle, x);
        }

        /// <summary>
        /// Returns a byte encoding of the <see cref="BigNumber" /> in little endian encoding.
        ///
        /// Optionally adds padding with zeros at then end of the byte encoding,
        /// the length of which is specified by the <paramref name="backPadding"/> parameter.
        /// </summary>
        /// <param name="backPadding">How many bytes of zero to add at the end of the byte encoding.</param>
        /// <returns>Byte buffer containing the byte encoding of the number.</returns>
        public byte[] ToBytes(uint backPadding = 0)
        {
            byte[] buffer = new byte[Length.InBytes + backPadding];
            BigNumberHandle.ToBytes(Handle, buffer);
            return buffer;
        }

        /// <summary>
        /// Converts this <see cref="BigNumber" /> into a <see cref="BigInteger" />.
        /// </summary>
        /// <returns>
        /// A <see cref="BigInteger" /> instance representing the number.
        /// </returns>
        public BigInteger ToBigInteger()
        {
            byte[] buffer = ToBytes(backPadding: 1);
            var integer = new BigInteger(buffer);
            return integer;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {        
            var other = obj as BigNumber;
            if (other == null)
            {
                return false;
            }

            return (BigNumberHandle.Compare(this.Handle, other.Handle) == 0);
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ToBigInteger().GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToBigInteger().ToString();
        }

        /// <summary>
        /// Multiplies this <see cref="BigNumber" /> with <paramref name="other"/>
        /// modulo <paramref name="modulo"/> and returns the result.
        ///
        /// Precisely, the returned value is <c>z = x * y % m</c>, where
        /// <c>x</c> is the value of this <see cref="BigNumber" /> instance,
        /// <c>y</c> the value of <paramref name="other"/> and <c>m</c> the
        /// value of <paramref name="modulo"/>.
        /// </summary>
        /// <param name="other">The number with which to multiply.</param>
        /// <param name="modulo">The modulo for the multiplication.</param>
        /// <returns>
        /// A <see cref="BigNumber" /> instance with value<c>z</c>.
        /// </returns>
        public BigNumber ModMul(BigNumber other, BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.Create())
            {
                var result = new BigNumber();
                BigNumberHandle.ModMul(result.Handle, Handle, other.Handle, modulo.Handle, ctx);
                return result;
            }
        }

        /// <summary>
        /// Securely computes this <see cref="BigNumber" /> to the power
        /// of <paramref name="exponent"/> modulo <paramref name="modulo"/>
        /// and returns the result.
        ///
        /// Precisely, the returned value is <c>z = x^y % m</c>, where
        /// <c>x</c> is the value of this <see cref="BigNumber" /> instance,
        /// <c>y</c> the value of <paramref name="exponent"/> and <c>m</c> the
        /// value of <paramref name="modulo"/>.
        ///
        /// The computation is constant time for different instances of <c>this</c>
        /// but fixed <paramref name="exponent"/> and <paramref name="modulo"/>
        /// and uses OpenSSL's secure heap.
        /// </summary>
        /// <param name="exponent">The exponent which to raise this <see cref="BigNumber" /> to.</param>
        /// <param name="modulo">The modulo for the exponentiation.</param>
        /// <returns>
        /// A <see cref="BigNumber" /> instance with value<c>z</c>.
        /// </returns>
        public BigNumber ModExp(SecureBigNumber exponent, BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var result = new BigNumber();
                BigNumberHandle.SecureModExp(result.Handle, Handle, exponent.Handle, modulo.Handle, ctx);
                return result;
            }
        }

        /// <summary>
        /// Computes this <see cref="BigNumber" /> to the power
        /// of <paramref name="exponent"/> modulo <paramref name="modulo"/>
        /// and returns the result.
        ///
        /// Precisely, the returned value is <c>z = x^y % m</c>, where
        /// <c>x</c> is the value of this <see cref="BigNumber" /> instance,
        /// <c>y</c> the value of <paramref name="exponent"/> and <c>m</c> the
        /// value of <paramref name="modulo"/>.
        ///
        /// The computation is not secure. For a secure variant see
        /// <see cref="BigNumber.ModExp(SecureBigNumber, BigNumber)" />.
        /// </summary>
        /// <param name="exponent">The exponent which to raise this <see cref="BigNumber" /> to.</param>
        /// <param name="modulo">The modulo for the exponentiation.</param>
        /// <returns>
        /// A <see cref="BigNumber" /> instance with value<c>z</c>.
        /// </returns>
        public BigNumber ModExp(BigNumber exponent, BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var result = new BigNumber();
                BigNumberHandle.ModExp(result.Handle, Handle, exponent.Handle, modulo.Handle, ctx);
                return result;
            }
        }

        /// <summary>
        /// Computes the multiplicative inverse of this <see cref="BigNumber" />
        /// modulo <paramref name="modulo"/> and returns the result.
        ///
        /// Precisely, computes <c>z</c> such that <c>x * z % m = 1</c>, where
        /// <c>x</c> is the value of this <see cref="BigNumber" /> instance
        /// and <c>m</c> the value of <paramref name="modulo"/>.
        /// </summary>
        /// <param name="modulo">The modulo for computing the multiplicative inverse.</param>
        /// <returns>
        /// A <see cref="BigNumber" /> instance with value<c>z</c>.
        /// </returns>
        public BigNumber ModReciprocal(BigNumber modulo)
        {
            using (var ctx = BigNumberContextHandle.CreateSecure())
            {
                var result = new BigNumber();
                BigNumberHandle.ModInverse(result.Handle, Handle, modulo.Handle, ctx);
                return result;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handle.Dispose();
            }
        }
    }
}