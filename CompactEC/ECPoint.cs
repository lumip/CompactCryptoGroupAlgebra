using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace CompactEC
{
    public struct ECPoint : IEquatable<ECPoint>
    {
        public bool IsAtInfinity { get; }
        public BigInteger X { get; }
        public BigInteger Y { get; }
        
        public ECPoint(BigInteger x, BigInteger y, bool isAtInfinity = false)
        {
            X = x;
            Y = y;
            IsAtInfinity = isAtInfinity;
        }

        public static ECPoint PointAtInfinity { get { return new ECPoint(BigInteger.Zero, BigInteger.Zero, true); } }

        public ECPoint Clone()
        {
            return new ECPoint(X, Y, IsAtInfinity);
        }
        
        public override string ToString()
        {
            if (IsAtInfinity)
                return string.Format("(atInf)");
            return string.Format("({0}, {1})", X, Y);
        }

        public bool Equals(ECPoint other)
        {
            return (IsAtInfinity && other.IsAtInfinity) || ((X == other.X) && (Y == other.Y));
        }
    }
}
