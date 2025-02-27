namespace Jinget.Core.Utilities;

/// <summary>
/// Provides utility methods for creating boolean expressions.
/// </summary>
public static class BooleanUtility
{
    /// <summary>
    /// Creates an expression that always evaluates to true.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter.</typeparam>
    /// <returns>An expression that always returns true.</returns>
    public static Expression<Func<T, bool>> TrueCondition<T>() => x => 1 == 1;

    /// <summary>
    /// Creates an expression that always evaluates to false.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter.</typeparam>
    /// <returns>An expression that always returns false.</returns>
    public static Expression<Func<T, bool>> FalseCondition<T>() => x => 1 == 0;
}