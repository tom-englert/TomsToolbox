namespace TomsToolbox.Core
{
    /// <summary>
    /// Binary operations supported by the <see cref="BinaryOperationProcessor"/>
    /// </summary>
    public enum BinaryOperation
    {
        /// <summary>
        /// The addition operation.
        /// </summary>
        Addition,
        /// <summary>
        /// The subtraction operation.
        /// </summary>
        Subtraction,
        /// <summary>
        /// The multiply operation.
        /// </summary>
        Multiply,
        /// <summary>
        /// The division operation.
        /// </summary>
        Division,
        /// <summary>
        /// The equality operation.
        /// </summary>
        Equality,
        /// <summary>
        /// The inequality operation.
        /// </summary>
        Inequality,
        /// <summary>
        /// The greater than operation.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// The less than operation.
        /// </summary>
        LessThan,
        /// <summary>
        /// The greater than or equal operation.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// The less than or equal operation.
        /// </summary>
        LessThanOrEqual
    }
}