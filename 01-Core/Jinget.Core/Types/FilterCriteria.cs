namespace Jinget.Core.Types;

/// <summary>
/// represents a filter criteria. if no operator specified, then "=" operator will be used by default
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class FilterCriteria
{
    public string Operand { get; set; }
    public object Value { get; set; }
    public Operator Operator { get; set; } = Operator.Equal;

    /// <summary>
    /// Combine current criteria with next criteria
    /// </summary>
    public ConditionCombiningType NextConditionCombination { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

