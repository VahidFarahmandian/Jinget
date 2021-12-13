namespace Jinget.Core.ExpressionToSql
{
    public class Table
    {
        public virtual string Name { get; set; }

        public string Schema { get; set; } = "[dbo]";
    }
}