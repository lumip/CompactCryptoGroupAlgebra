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
