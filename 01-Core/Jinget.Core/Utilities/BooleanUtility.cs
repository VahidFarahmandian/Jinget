using System;
using System.Linq.Expressions;

namespace Jinget.Core.Utilities;

public static class BooleanUtility
{
    public static Expression<Func<T, bool>> TrueCondition<T>() => x => 1 == 1;

    public static Expression<Func<T, bool>> FalseCondition<T>() => x => 1 == 0;
}