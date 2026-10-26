namespace Jinget.Core.Enumerations;

/// <summary>
/// Define Operator types which can be used in usecases such as filter criteria
/// </summary>
public enum Operator
{
    // Equality
    Equal,
    NotEqual,

    // Comparison
    GreaterThan,
    GreaterThanOrEqual,
    LowerThan,
    LowerThanOrEqual,

    // String operations
    Contains,
    NotContains,
    StartsWith,
    DoesNotStartWith,
    EndsWith,
    DoesNotEndWith,

    // Null checks
    IsNull,
    IsNotNull,

    // Collection membership
    In,
    NotIn,

    // Range
    Between
}