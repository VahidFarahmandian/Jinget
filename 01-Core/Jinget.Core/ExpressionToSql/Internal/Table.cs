namespace Jinget.Core.ExpressionToSql.Internal;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class Table
{
    public virtual string Name { get; set; }

    public string Schema { get; set; } = "[dbo]";
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
