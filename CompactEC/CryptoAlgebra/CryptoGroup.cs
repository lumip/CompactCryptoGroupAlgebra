using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroup<E>
    {
        private CryptoGroupAlgebra<E> _groupAlgebra;

        public CryptoGroup(CryptoGroupAlgebra<E> groupAlgebra)
        {
            if (groupAlgebra == null)
                throw new ArgumentNullException(nameof(groupAlgebra));
            _groupAlgebra = groupAlgebra;
        }

        public BigInteger Order { get { return _groupAlgebra.Order; } }

        public int GroupElementSize { get { return _groupAlgebra.GroupElementSize; } }
        public int OrderSize { get { return _groupAlgebra.OrderSize; } }
        public int FactorSize { get { return _groupAlgebra.FactorSize; } }

        public CryptoGroupElement<E> Generator { get { return CreateGroupElement(_groupAlgebra.Generator, _groupAlgebra); } }
        public CryptoGroupElement<E> IdentityElement { get { return CreateGroupElement(_groupAlgebra.IdentityElement, _groupAlgebra); } }

        public CryptoGroupElement<E> GenerateElement(BigInteger index)
        {
            return CreateGroupElement(_groupAlgebra.GenerateElement(index), _groupAlgebra);
        }

        public CryptoGroupElement<E> CreateElementFromBytes(byte[] buffer)
        {
            return CreateGroupElement(buffer, _groupAlgebra);
        }

        protected abstract CryptoGroupElement<E> CreateGroupElement(E e, CryptoGroupAlgebra<E> groupAlgebra);
        protected abstract CryptoGroupElement<E> CreateGroupElement(byte[] buffer, CryptoGroupAlgebra<E> groupAlgebra);
    }
}
