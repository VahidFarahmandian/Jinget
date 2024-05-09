using System.Collections.Generic;

namespace Jinget.Core.Operators;

public static class GenericEqualityOperator<T>
{
    /// <summary>
    /// Check whether two generic type properties are equal or not. 
    /// </summary>
    public static bool AreEqual(T t1, T t2) => EqualityComparer<T>.Default.Equals(t1, t2);
}
