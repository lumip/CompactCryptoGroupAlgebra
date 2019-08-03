using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace CompactEC
{
    public class RawPoint : IEquatable<RawPoint>
    {
        public bool IsAtInfinity { get; }
        public BigInteger X { get; }
        public BigInteger Y { get; }


        public RawPoint() : this(BigInteger.Zero, BigInteger.Zero, true) { }
        
        public RawPoint(BigInteger x, BigInteger y, bool isAtInfinity = false)
        {
            X = x;
            Y = y;
            IsAtInfinity = isAtInfinity;
        }

        public static RawPoint PointAtInfinity { get { return new RawPoint(); } }

        public RawPoint Clone()
        {
            return new RawPoint(X, Y);
        }

        

        public override string ToString()
        {
            if (IsAtInfinity)
                return string.Format("(atInf)");
            return string.Format("({0}, {1})", X, Y);
        }

        public bool Equals(RawPoint other)
        {
            return (IsAtInfinity && other.IsAtInfinity) || ((X == other.X) && (Y == other.Y));
        }

        public override bool Equals(object obj)
        {
            RawPoint other = obj as RawPoint;
            if (other == null)
                return false;
            return Equals(other);
        }
    }

    public class CurveAlgebra
    {
        private BigInteger Modulo { get; }
        private BigInteger a;
        private BigInteger b;

        private int OrderSize { get; }

        public CurveAlgebra(BigInteger modulo, BigInteger paramA, BigInteger paramB, int orderSize)
        {
            Modulo = modulo;
            a = paramA;
            b = paramB;
            OrderSize = orderSize;
        }

        public bool IsValidPoint(RawPoint point)
        {
            AssertPoint(point);
            if (point.IsAtInfinity)
                return true;

            BigInteger r = Mod(BigInteger.ModPow(point.X, 3, Modulo) + a * point.X + b);
            BigInteger ySquared = Square(point.Y);
            return r == ySquared;
        }

        public void AssertPoint(RawPoint point)
        {
            Debug.Assert(point.IsAtInfinity || BigInteger.Zero <= point.X && point.X < Modulo);
            Debug.Assert(point.IsAtInfinity || BigInteger.Zero <= point.Y && point.Y < Modulo);
        }

        public bool ArePointsAdditiveInverse(RawPoint left, RawPoint right)
        {
            AssertPoint(left); AssertPoint(right);
            //return left.X == right.X && left.Y == (p - right.Y);
            var inv = Invert(right);
            return left.Equals(inv);
        }

        public RawPoint Add(RawPoint left, RawPoint right)
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

            BigInteger lambdaSame = Mod((3 * Square(x1) + a) * InvertMult(2 * y1));
            BigInteger lambdaDiff = Mod((y2 - y1) * InvertMult(x2 - x1));
            BigInteger lambda;
            if (left.Equals(right))
            {
                lambda = lambdaSame;
            }
            else
            {
                lambda = lambdaDiff;
            }
            BigInteger x3 = Mod(Square(lambda) - x1 - x2);
            BigInteger y3 = Mod(lambda * (x1 - x3) - y1);
            
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

        public BigInteger Mod(BigInteger x)
        {
            return (x % Modulo + Modulo) % Modulo; // to prevent negative results
        }

        public BigInteger Multiplex(BigInteger selection, BigInteger left, BigInteger right)
        {
            Debug.Assert(selection.IsOne || selection.IsZero);
            return right + selection * (left - right);
        }

        public BigInteger Multiplex(bool selection, BigInteger left, BigInteger right)
        {
            var sel = new BigInteger(Convert.ToByte(selection));
            return Multiplex(sel, left, right);
        }

        public bool Multiplex(bool selection, bool left, bool right)
        {
            return right ^ (selection & (left ^ right));
        }

        public RawPoint Multiplex(BigInteger selection, RawPoint left, RawPoint right)
        {
            Debug.Assert(selection.IsOne || selection.IsZero);
            var sel = !selection.IsZero;
            return new RawPoint(
                Multiplex(selection, left.X, right.X),
                Multiplex(selection, left.Y, right.Y),
                Multiplex(sel, left.IsAtInfinity, right.IsAtInfinity)
            );
        }

        public RawPoint Multiply(RawPoint x, BigInteger k)
        {
            // note(lumip): double-and-add implementation that issues
            //  the same amount of adds no matter the value of k and 
            //  has no conditional control flow. It is thus
            //  safe(r) against timing/power/cache/branch prediction(?)
            //  side channel attacks (provided Add is constant time for
            //  all EC add cases).
            RawPoint r0 = RawPoint.PointAtInfinity;

            int i = OrderSize - 1;
            for (BigInteger mask = BigInteger.One << (OrderSize - 1); !mask.IsZero; mask = mask >> 1, --i)
            {
                BigInteger bitI = (k & mask) >> i;
                r0 = Add(r0, r0);
                RawPoint r1 = Add(r0, x);
                
                r0 = Multiplex(bitI, r1, r0);
            }
            Debug.Assert(i == -1);
            return r0;
        }

        public BigInteger Square(BigInteger x)
        {
            return BigInteger.ModPow(x, 2, Modulo);
        }

        public BigInteger InvertMult(BigInteger x)
        {
            // note(lummip): sidechannels would leak p-2, but p is public anyways
            return BigInteger.ModPow(x, Modulo - 2, Modulo);
        }

        public RawPoint Invert(RawPoint p)
        {
            AssertPoint(p);
            return new RawPoint(p.X, p.Y.IsZero ? p.Y : Modulo - p.Y);
        }
    }

    //public interface IPoint
    //{
    //    bool IsAtInfinity { get; }
    //    IPoint Clone();
    //}

    //public class PointAtInfinity : IPoint
    //{
    //    bool IsAtInfinity { get { return true; } }
    //    IPoint Clone() { return new PointAtInfinity(); }
    //}

    //public struct CurveParameters
    //{
    //    public BigInteger p;
    //}

    //public class Point : IPoint
    //{
    //    public BigInteger X { get; private set; }
    //    public BigInteger Y { get; private set; }

    //    public Point(BigInteger x, BigInteger y)
    //    {
    //        X = x;
    //        Y = y;
    //    }

    //    public void Add(IPoint other)
    //    {
    //        if (other.IsAtInfinity)
    //        {
    //            return other.Clone();
    //        }
    //        if (other.X == X)
    //        {

    //        }
    //    }

    //    public void Multiply(IPoint other)
    //    {

    //    }

    //    public IPoint Clone()
    //    {
    //        return new Point(X, Y);
    //    }

    //    public static Point operator +(Point left, IPoint right)
    //    {
    //        Point res = (Point)left.Clone();
    //        res.Add(right);
    //        return res;
    //    }

    //    public static Point operator *(Point left, IPoint right)
    //    {
    //        Point res = (Point)left.Clone();
    //        res.Multiply(right);
    //        return res;
    //    }
    //}
}
