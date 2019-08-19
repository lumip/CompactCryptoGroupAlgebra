using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Diagnostics;
using CompactEC.CryptoAlgebra;

namespace CompactEC
{
    /// <summary>
    /// An eliptic curve y² = x³ + Ax + B over the finite field defined by P
    /// </summary>
    public struct ECParameters
    {
        public BigInteger P;
        public BigInteger A;
        public BigInteger B;
        public BigInteger Order;
        public RawPoint Generator;
        public int OrderSize;
        public int GroupElementSize;
    }

    public class BigIntegerRing
    {
        public BigInteger Modulo;

        public BigIntegerRing(BigInteger primeModulo)
        {
            Modulo = primeModulo;
        }

        public BigInteger Pow(BigInteger x, BigInteger e)
        {
            Debug.Assert(e >= BigInteger.Zero);
            return BigInteger.ModPow(Mod(x), e, Modulo);
        }

        public BigInteger Square(BigInteger x)
        {
            return Pow(x, 2);
        }

        public BigInteger InvertMult(BigInteger x)
        {
            // note(lummip): sidechannels would leak p-2, but p is public anyways
            return Pow(x, Modulo - 2);
        }

        public BigInteger Mod(BigInteger x)
        {
            return (x % Modulo + Modulo) % Modulo; // to prevent negative results
        }

    }

    public class ECGroupAlgebra : CryptoGroupAlgebra<RawPoint>
    {
        private ECParameters _parameters;
        private BigIntegerRing _ring;
        
        public override RawPoint IdentityElement { get { return _parameters.Generator; } }

        public ECGroupAlgebra(ECParameters parameters)
            : base(parameters.Generator, parameters.Order, parameters.GroupElementSize, parameters.OrderSize)
        {
            _parameters = parameters;
            _ring = new BigIntegerRing(_parameters.P);
        }

        public void AssertPoint(RawPoint point)
        {
            Debug.Assert(point.IsAtInfinity || BigInteger.Zero <= point.X && point.X < _parameters.P);
            Debug.Assert(point.IsAtInfinity || BigInteger.Zero <= point.Y && point.Y < _parameters.P);
        }

        public bool IsValidPoint(RawPoint point)
        {
            AssertPoint(point);
            if (point.IsAtInfinity)
                return true;

            BigInteger r = _ring.Mod(_ring.Pow(point.X, 3) + _parameters.A * point.X + _parameters.B);
            BigInteger ySquared = _ring.Square(point.Y);
            return r == ySquared;
        }

        public bool ArePointsAdditiveInverse(RawPoint left, RawPoint right)
        {
            AssertPoint(left); AssertPoint(right);
            var inv = Negate(right);
            return left.Equals(inv);
        }

        public override RawPoint Add(RawPoint left, RawPoint right)
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

            if (left.IsAtInfinity && right.IsAtInfinity)
                return RawPoint.PointAtInfinity;
            else if (left.IsAtInfinity && !right.IsAtInfinity)
                return right.Clone();
            else if (right.IsAtInfinity && !left.IsAtInfinity)
                return left.Clone();
            else if (ArePointsAdditiveInverse(left, right))
                return RawPoint.PointAtInfinity;
            else
                return new RawPoint(x3, y3);
        }

        public override RawPoint Negate(RawPoint p)
        {
            AssertPoint(p);
            return new RawPoint(p.X, p.Y.IsZero ? p.Y : _parameters.P - p.Y);
        }


        private BigInteger Multiplex(BigInteger selection, BigInteger left, BigInteger right)
        {
            Debug.Assert(selection.IsOne || selection.IsZero);
            return right + selection * (left - right);
        }

        private BigInteger Multiplex(bool selection, BigInteger left, BigInteger right)
        {
            var sel = new BigInteger(Convert.ToByte(selection));
            return Multiplex(sel, left, right);
        }

        private bool Multiplex(bool selection, bool left, bool right)
        {
            return right ^ (selection & (left ^ right));
        }

        protected override RawPoint Multiplex(BigInteger selection, RawPoint left, RawPoint right)
        {
            Debug.Assert(selection.IsOne || selection.IsZero);
            var sel = !selection.IsZero;
            return new RawPoint(
                Multiplex(selection, left.X, right.X),
                Multiplex(selection, left.Y, right.Y),
                Multiplex(sel, left.IsAtInfinity, right.IsAtInfinity)
            );
        }
    }
}
