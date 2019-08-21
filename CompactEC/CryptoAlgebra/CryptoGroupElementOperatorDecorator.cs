//using System;
//using System.Collections.Generic;
//using System.Numerics;
//using System.Text;

//namespace CompactEC.CryptoAlgebra
//{
//    public class CryptoGroupElementOperatorDecorator : ICryptoGroupElement
//    {
//        private ICryptoGroupElement _implementation;

//        public CryptoGroupElementOperatorDecorator(ICryptoGroupElement implementation)
//        {
//            _implementation = implementation;
//        }

//        public void Add(ICryptoGroupElement other)
//        {
//            _implementation.Add(other);
//        }

//        public void MultiplyScalar(BigInteger k)
//        {
//            _implementation.MultiplyScalar(k);
//        }

//        public void Negate()
//        {
//            _implementation.Negate();
//        }

//        public byte[] ToBytes()
//        {
//            return _implementation.ToBytes();
//        }

//        public ICryptoGroupElement Clone()
//        {
//            return CloneInternal();
//        }

//        private CryptoGroupElementOperatorDecorator CloneInternal()
//        {
//            return new CryptoGroupElementOperatorDecorator(_implementation.Clone());
//        }

//        public static CryptoGroupElementOperatorDecorator operator +(CryptoGroupElementOperatorDecorator left, ICryptoGroupElement right)
//        {
//            var result = left.CloneInternal();
//            result.Add(right);
//            return result;
//        }

//        public static CryptoGroupElementOperatorDecorator operator+(ICryptoGroupElement left, CryptoGroupElementOperatorDecorator right)
//        {
//            return right + left;
//        }

//        public static CryptoGroupElementOperatorDecorator operator -(CryptoGroupElementOperatorDecorator e)
//        {
//            var result = e.CloneInternal();
//            result.Negate();
//            return result;
//        }

//        public static CryptoGroupElementOperatorDecorator operator -(ICryptoGroupElement left, CryptoGroupElementOperatorDecorator right)
//        {
//            var result = right.CloneInternal();
//            result.Negate();
//            result.Add(left);
//            return result;
//        }

//        public static CryptoGroupElementOperatorDecorator operator -(CryptoGroupElementOperatorDecorator left, ICryptoGroupElement right)
//        {
//            return right - left;
//        }

//        public static CryptoGroupElementOperatorDecorator operator *(CryptoGroupElementOperatorDecorator e, BigInteger k)
//        {
//            var result = e.CloneInternal();
//            result.MultiplyScalar(k);
//            return result;
//        }

//        public static CryptoGroupElementOperatorDecorator operator *(BigInteger k, CryptoGroupElementOperatorDecorator e)
//        {
//            return e * k;
//        }
//    }
//}
