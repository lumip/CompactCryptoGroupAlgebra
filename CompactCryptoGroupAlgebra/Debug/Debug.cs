// CompactCryptoGroupAlgebra - C# implementation of abelian group algebra for experimental cryptography
// Copyright (C) 2020  Lukas <lumip> Prediger
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
namespace CompactCryptoGroupAlgebra // Change namespace to reflect directory structure?
{
    /// <summary>
    /// Indicates the violation of a <see cref="CompactCryptoGroupAlgebra.Debug.Assert(bool, string)"/>
    /// statement.
    /// </summary>
    public class AssertionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AssertionException"/>.
        /// </summary>
        public AssertionException(string? message = null) : base(message ?? "Assertion failure!") { }
    }

    /// <summary>
    /// Static class for Debug assertions that are not silently ignored by
    /// Mono.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Indicates whether debug assertions are active i.e., 
        /// whether CompactCryptoGroupAlgebra was compiled with the DEBUG symbol.
        /// </summary>
#if DEBUG
        public static readonly bool IsActive = true;
#else
        public static readonly bool IsActive = false;
#endif

        /// <summary>
        /// Asserts the specified expression result and throws an <see cref="AssertionException"/>
        /// if the assertion fails.
        /// </summary>
        /// <param name="expressionResult">Expression result to assert.</param>
        /// <param name="message">Optional message for the thrown <see cref="AssertionException"/>.</param>
        public static void Assert(bool expressionResult, string? message = null)
        {
#if DEBUG
            if (!expressionResult)
                throw new AssertionException(message);
#endif
        }
    }
}
