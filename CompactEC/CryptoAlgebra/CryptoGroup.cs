using System;
using System.Collections.Generic;
using System.Text;

namespace CompactEC.CryptoAlgebra
{
    public abstract class CryptoGroup<E, S>
    {
        private CryptoGroupAlgebra<E, S> _groupAlgebra;

        public CryptoGroup(CryptoGroupAlgebra<E, S> groupAlgebra)
        {
            if (groupAlgebra == null)
                throw new ArgumentNullException(nameof(groupAlgebra));
            _groupAlgebra = groupAlgebra;
        }

        public S Order { get { return _groupAlgebra.Order; } }

        public int GroupElementSize { get { return _groupAlgebra.GroupElementSize; } }
        public int OrderSize { get { return _groupAlgebra.OrderSize; } }

        public CryptoGroupElement<E, S> Generator { get { return CreateGroupElement(_groupAlgebra.Generator, _groupAlgebra); } }

        public CryptoGroupElement<E, S> GenerateElement(S index)
        {
            return CreateGroupElement(_groupAlgebra.GenerateElement(index), _groupAlgebra);
        }

        public CryptoGroupElement<E, S> CreateElementFromBytes(byte[] buffer)
        {
            return CreateGroupElement(buffer, _groupAlgebra);
        }

        protected abstract CryptoGroupElement<E, S> CreateGroupElement(E e, CryptoGroupAlgebra<E, S> groupAlgebra);
        protected abstract CryptoGroupElement<E, S> CreateGroupElement(byte[] buffer, CryptoGroupAlgebra<E, S> groupAlgebra);
    }
}
