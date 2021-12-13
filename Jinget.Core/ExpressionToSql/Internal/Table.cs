namespace Jinget.Core.ExpressionToSql.Internal
{
    public class Table
    {
        public virtual string Name { get; set; }

        public string Schema { get; set; } = "[dbo]";
    }
}