﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Diagnostics;
using System.Security.Cryptography;

namespace CompactEC.CryptoAlgebra
{
    /// <summary>
    /// Base implementation of <see cref="ICryptoGroup"/>.
    /// 
    /// Relies on <see cref="ICryptoGroupAlgebra{E}"/> to provide basic algebraic group
    /// opertions on group elements represented the template type. The implementation of
    /// CryptoGroup mainly provides abstraction from this specifics for production code,
    /// wrapping the template type into <see cref="CryptoGroupElement{E}"/>
    /// instances, thus allowing production code to interface only with the
    /// <see cref="ICryptoGroup"/> and <see cref="ICryptoGroupElement"/> interfaces,
    /// irrespective of the specific group implementation.
    /// </summary>
    /// <typeparam name="E">The data type used for raw group elements the algebraic operations operate on.</typeparam>
    /// <remarks>
    /// Implementers of <see cref="ICryptoGroup"/> should provide an implementation of
    /// <see cref="ICryptoGroupAlgebra{E}"/> (preferrably by extending <see cref="CryptoGroupAlgebra{E}"/>)
    /// to realize basic algebraic operations and an extension of <see cref="CryptoGroupElement{E}"/> 
    /// which can then be employed in a specialization of <see cref="CryptoGroup{E}"/>.
    /// 
    /// All these classes are designed to have a minimum of fully virtual/abstract methods in need of implementation
    /// to provide full algebraic group functionality.
    /// </remarks>
    public abstract class CryptoGroup<E> : ICryptoGroup where E : struct
    {
        /// <summary>
        /// Implementation of group operations on raw group element type <see cref="E"/>.
        /// </summary>
        protected ICryptoGroupAlgebra<E> Algebra { get; }

        /// <summary>
        /// Create a CryptoGroup instance using a given <see cref="ICryptoGroupAlgebra{E}"/> instance
        /// for algebraic operations.
        /// </summary>
        /// <param name="algebra"></param>
        public CryptoGroup(ICryptoGroupAlgebra<E> algebra)
        {
            if (algebra == null)
                throw new ArgumentNullException(nameof(algebra));
            Algebra = algebra;
        }

        /// <summary>
        /// <see cref="ICryptoGroup.NeutralElement"/>
        /// </summary>
        public ICryptoGroupElement NeutralElement { get { return CreateGroupElement(Algebra.NeutralElement); } }

        /// <summary>
        /// <see cref="ICryptoGroup.Generator"/>
        /// </summary>
        public ICryptoGroupElement Generator { get { return CreateGroupElement(Algebra.Generator); } }

        /// <summary>
        /// <see cref="ICryptoGroup.OrderBitLength"/>
        /// </summary>
        public int OrderBitLength { get { return Algebra.OrderBitLength; } }

        /// <summary>
        /// <see cref="ICryptoGroup.OrderByteLength"/>
        /// </summary>
        public int OrderByteLength { get { return (int)Math.Ceiling((double)OrderBitLength / 8); } }

        /// <summary>
        /// <see cref="ICryptoGroup.Order"/>
        /// </summary>
        public BigInteger Order { get { return Algebra.Order; } }

        /// <summary>
        /// <see cref="ICryptoGroup.ElementBitLength"/>
        /// </summary>
        public int ElementBitLength { get { return Algebra.ElementBitLength; } }

        /// <summary>
        /// <see cref="ICryptoGroup.ElementByteLength"/>
        /// </summary>
        public int ElementByteLength { get { return (int)Math.Ceiling((double)ElementBitLength / 8); } }

        /// <summary>
        /// Creates a new <see cref="ICryptoGroupElement"/> instance for a given raw group element of type <see cref="E"/>.
        /// </summary>
        /// <param name="value">Value of the group element as raw type.</param>
        /// <returns>An <see cref="ICryptoGroupElement"/>instance representing the given value.</returns>
        /// <remarks>
        /// Used to convert outputs of <see cref="ICryptoGroupAlgebra{E}"/> operations to <see cref="ICryptoGroupElement"/>
        /// instances by methods within this class.
        /// 
        /// Needs to be implemented by specializations.
        /// </remarks>
        protected abstract CryptoGroupElement<E> CreateGroupElement(E value);

        /// <summary>
        /// Creates a new <see cref="ICryptoGroupElement"/> instance from a byte representation of the group element.
        /// </summary>
        /// <param name="buffer">Byte representation of the group elements.</param>
        /// <returns>An <see cref="ICryptoGroupElement"/>instance representing the given value.</returns>
        /// <remarks>
        /// Needs to be implemented by specializations.
        /// </remarks>
        protected abstract CryptoGroupElement<E> CreateGroupElement(byte[] buffer);

        /// <summary>
        /// <see cref="ICryptoGroup.Add(ICryptoGroupElement, ICryptoGroupElement)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="CryptoGroupElement{E}"/> wrapper for <see cref="ICryptoGroupAlgebra{E}.Add(E, E)"/>.
        /// </remarks>
        public ICryptoGroupElement Add(CryptoGroupElement<E> left, CryptoGroupElement<E> right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            return CreateGroupElement(Algebra.Add(left.Value, right.Value));
        }

        /// <summary>
        /// <see cref="ICryptoGroup.FromBytes(byte[])"/>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public ICryptoGroupElement FromBytes(byte[] buffer)
        {
            return CreateGroupElement(buffer);
        }

        /// <summary>
        /// <see cref="ICryptoGroup.Generate(BigInteger)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="CryptoGroupElement{E}"/> wrapper for <see cref="ICryptoGroupAlgebra{E}.GenerateElement(BigInteger)"/>
        /// </remarks>
        public ICryptoGroupElement Generate(BigInteger index)
        {
            return CreateGroupElement(Algebra.GenerateElement(index));
        }

        /// <summary>
        /// <see cref="ICryptoGroup.MultiplyScalar(ICryptoGroupElement, BigInteger)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="CryptoGroupElement{E}"/> wrapper for <see cref="ICryptoGroupAlgebra{E}.MultiplyScalar(E, BigInteger)"/>
        /// </remarks>
        public ICryptoGroupElement MultiplyScalar(CryptoGroupElement<E> element, BigInteger k)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return CreateGroupElement(Algebra.MultiplyScalar(element.Value, k));
        }

        /// <summary>
        /// <see cref="ICryptoGroup.Negate(ICryptoGroupElement)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="CryptoGroupElement{E}"/> wrapper for <see cref="ICryptoGroupAlgebra{E}.Negate(E)"/>
        /// </remarks>
        public ICryptoGroupElement Negate(CryptoGroupElement<E> element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return CreateGroupElement(Algebra.Negate(element.Value));
        }

        /// <summary>
        /// <see cref="ICryptoGroup.Add(ICryptoGroupElement, ICryptoGroupElement)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="ICryptoGroupElement"/> wrapper for <see cref="ICryptoGroupAlgebra{E}.Add(E, E)"/>.
        /// </remarks>
        public ICryptoGroupElement Add(ICryptoGroupElement left, ICryptoGroupElement right)
        {
            CryptoGroupElement<E> lhs = left as CryptoGroupElement<E>;
            CryptoGroupElement<E> rhs = right as CryptoGroupElement<E>;
            if (lhs == null)
                throw new ArgumentException("The left summand is not an element of the group.", nameof(left));
            if (rhs == null)
                throw new ArgumentException("The right summand is not an element of the group.", nameof(left));

            return Add(lhs, rhs);
        }

        /// <summary>
        /// <see cref="ICryptoGroup.MultiplyScalar(ICryptoGroupElement, BigInteger)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="ICryptoGroupElement"/> wrapper for <see cref="ICryptoGroupAlgebra{E}.MultiplyScalar(E, BigInteger)"/>.
        /// </remarks>
        public ICryptoGroupElement MultiplyScalar(ICryptoGroupElement element, BigInteger k)
        {
            CryptoGroupElement<E> e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return MultiplyScalar(e, k);
        }

        /// <summary>
        /// <see cref="ICryptoGroup.Negate(ICryptoGroupElement)"/>
        /// </summary>
        /// <remarks>
        /// <see cref="ICryptoGroupElement"/> wrapper for <see cref="ICryptoGroupAlgebra{E}.Negate(E)"/>.
        /// </remarks>
        public ICryptoGroupElement Negate(ICryptoGroupElement element)
        {
            CryptoGroupElement<E> e = element as CryptoGroupElement<E>;
            if (e == null)
                throw new ArgumentException("The provided value is not an element of the group.", nameof(element));

            return Negate(e);
        }

        /// <summary>
        /// <see cref="ICryptoGroup.GenerateRandom(RandomNumberGenerator)"/>
        /// </summary>
        public Tuple<BigInteger, ICryptoGroupElement> GenerateRandom(RandomNumberGenerator rng)
        {
            BigInteger index;
            do
            {
                byte[] buffer = new byte[OrderByteLength];
                rng.GetBytes(buffer);
                index = new BigInteger(buffer);
            }
            while (index <= 1 || index >= Order - 1);

            ICryptoGroupElement element = Generate(index);
            return new Tuple<BigInteger, ICryptoGroupElement>(index, element);
        }
    }
}
