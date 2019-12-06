using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Diagnostics;
using CompactEC;

namespace CompactEC
{
    /// <summary>
    /// An eliptic curve y² = x³ + Ax + B over the finite field defined by P
    /// </summary>
    public class ECGroupAlgebra : CryptoGroupAlgebra<ECPoint>
    {
        private ECParameters _parameters;
        private BigIntegerRing _ring;

        public override ECPoint NeutralElement { get { return ECPoint.PointAtInfinity; } }

        public override BigInteger Order { get { return _parameters.Order; } }

        public override ECPoint Generator { get { return _parameters.Generator; } }

        public override int ElementBitLength { get { return 2 * GetBitLength(_ring.Modulo); } }

        public ECGroupAlgebra(ECParameters parameters)
        {
            _parameters = parameters;
            _ring = new BigIntegerRing(_parameters.P);
            if (!IsValid(Generator))
                throw new ArgumentException("The point given as generator is not a valid point on the curve.", nameof(parameters));
        }

        private void AssertPoint(ECPoint point)
        {
            Debug.Assert(IsValid(point));
        }

        public bool AreNegations(ECPoint left, ECPoint right)
        {
            AssertPoint(left);
            AssertPoint(right);

            var inv = Negate(right);
            return left.Equals(inv);
        }

        public override ECPoint Add(ECPoint left, ECPoint right)
        {
            // note(lumip): constant time add implementation. all code paths have the same amount of operations
            //   to mitigate timing/power side channel attacks.
            //   requires InvertMult to deal with non-invertible values (result can be undefined) ideally with
            //   constant cost, as currently implemented.
            AssertPoint(left); AssertPoint(right);

            BigInteger x1 = left.X;
            BigInteger x2 = right.X;
            BigInteger y1 = left.Y;
            BigInteger y2 = right.Y;

            BigInteger lambdaSame = _ring.Mod((3 * _ring.Square(x1) + _parameters.A) * _ring.InvertMult(2 * y1));
            BigInteger lambdaDiff = _ring.Mod((y2 - y1) * _ring.InvertMult(x2 - x1));
            BigInteger lambda;
            if (left.Equals(right))
            {
                lambda = lambdaSame;
            }
            else
            {
                lambda = lambdaDiff;
            }
            BigInteger x3 = _ring.Mod(_ring.Square(lambda) - x1 - x2);
            BigInteger y3 = _ring.Mod(lambda * (x1 - x3) - y1);

            ECPoint result = ECPoint.PointAtInfinity;
            bool pointsAreNegations = AreNegations(left, right);
            if (left.IsAtInfinity && right.IsAtInfinity)
                result = ECPoint.PointAtInfinity;
            if (left.IsAtInfinity && !right.IsAtInfinity)
                result = right.Clone();
            if (right.IsAtInfinity && !left.IsAtInfinity)
                result = left.Clone();
            if (!left.IsAtInfinity && !right.IsAtInfinity && pointsAreNegations)
                result = ECPoint.PointAtInfinity;
            if (!left.IsAtInfinity && !right.IsAtInfinity && !pointsAreNegations)
                result = new ECPoint(x3, y3);
            return result;
        }

        public override ECPoint Negate(ECPoint p)
        {
            AssertPoint(p);
            return new ECPoint(p.X, _ring.Mod(-p.Y), p.IsAtInfinity);
        }

        protected BigInteger Multiplex(BigInteger selection, BigInteger left, BigInteger right)
        {
            Debug.Assert(selection == BigInteger.Zero || selection == BigInteger.One);
            return left + selection * (right - left);
        }

        private bool Multiplex(bool selection, bool left, bool right)
        {
            return left ^ (selection & (right ^ left));
        }

        protected override ECPoint Multiplex(BigInteger selection, ECPoint left, ECPoint right)
        {
            Debug.Assert(selection.IsOne || selection.IsZero);
            var sel = !selection.IsZero;
            return new ECPoint(
                Multiplex(selection, left.X, right.X),
                Multiplex(selection, left.Y, right.Y),
                Multiplex(sel, left.IsAtInfinity, right.IsAtInfinity)
            );
        }

        public override bool IsValid(ECPoint point)
        {
            if (point.IsAtInfinity)
                return true;

            if (!(point.IsAtInfinity || BigInteger.Zero <= point.X && point.X < _parameters.P))
                return false;
            if (!(point.IsAtInfinity || BigInteger.Zero <= point.Y && point.Y < _parameters.P))
                return false;

            BigInteger r = _ring.Mod(_ring.Pow(point.X, 3) + _parameters.A * point.X + _parameters.B);
            BigInteger ySquared = _ring.Square(point.Y);
            return r == ySquared;
        }

        public override ECPoint FromBytes(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length < 2 * _ring.ElementByteLength)
                throw new ArgumentException("The given buffer is too short to contain a valid element representation.", nameof(buffer));

            byte[] xBytes = new byte[_ring.ElementByteLength];
            byte[] yBytes = new byte[_ring.ElementByteLength];

            Buffer.BlockCopy(buffer, 0, xBytes, 0, _ring.ElementByteLength);
            Buffer.BlockCopy(buffer, _ring.ElementByteLength, yBytes, 0, _ring.ElementByteLength);

            BigInteger x = new BigInteger(xBytes);
            BigInteger y = new BigInteger(yBytes);
            return new ECPoint(x, y);
        }

        public override byte[] ToBytes(ECPoint element)
        {
            byte[] xBytes = element.X.ToByteArray();
            byte[] yBytes = element.X.ToByteArray();

            Debug.Assert(xBytes.Length <= _ring.ElementByteLength);
            Debug.Assert(yBytes.Length <= _ring.ElementByteLength);

            byte[] result = new byte[2 * _ring.ElementByteLength];
            Buffer.BlockCopy(xBytes, 0, result, 0, xBytes.Length);
            Buffer.BlockCopy(yBytes, 0, result, _ring.ElementByteLength, yBytes.Length);
            return result;
        }
    }
}
