namespace Jinget.Core.Utilities.Expressions;

/// <summary>
/// Utility class for generating scoring expressions from filter expressions.
/// Converts boolean filter expressions into integer scoring expressions where
/// higher scores indicate better matches.
/// </summary>
public static class FilterScoringGeneratorUtility
{
    /// <summary>
    /// Generates a scoring expression from a filter expression.
    /// The scoring expression returns an integer representing how well an item matches the filter.
    /// </summary>
    /// <typeparam name="T">The type of entity being filtered</typeparam>
    /// <param name="filterExpression">The original filter expression to convert</param>
    /// <param name="rules">Scoring rules to customize how points are assigned</param>
    /// <returns>A scoring expression that returns match score as integer</returns>
    /// <example>
    /// <code>
    /// // Filter: x => x.Age > 18 || x.Name.Contains("John")
    /// // Score: Returns 1 if Age > 18, plus 1 if Name contains "John"
    /// var scoreExpr = GenerateScoreExpression(filter);
    /// </code>
    /// </example>
    public static Expression<Func<T, int>> GenerateScoreExpression<T>(
        Expression<Func<T, bool>> filterExpression,
        ScoringRules? rules = null)
    {
        ArgumentNullException.ThrowIfNull(filterExpression, nameof(filterExpression));

        rules ??= ScoringRules.Default;
        var parameter = filterExpression.Parameters[0];
        var visitor = new FilterScoringGeneratorVisitor(rules);
        var scoreBody = visitor.Visit(filterExpression.Body);

        return Expression.Lambda<Func<T, int>>(scoreBody, parameter);
    }

    /// <summary>
    /// Defines rules for how scores are calculated from filter expressions
    /// </summary>
    public class ScoringRules
    {
        /// <summary>
        /// Gets the default scoring rules (all weights set to 1)
        /// </summary>
        public static ScoringRules Default => new ScoringRules();

        /// <summary>
        /// Weight applied to OR conditions (||)
        /// </summary>
        public int OrWeight { get; set; } = 1;

        /// <summary>
        /// Weight applied to AND conditions (&&)
        /// </summary>
        public int AndWeight { get; set; } = 1;

        /// <summary>
        /// Points awarded for each individual condition that evaluates to true
        /// </summary>
        public int ConditionWeight { get; set; } = 1;
    }

    /// <summary>
    /// Expression visitor that converts boolean expressions to integer scoring expressions
    /// </summary>
    private class FilterScoringGeneratorVisitor : ExpressionVisitor
    {
        private readonly ScoringRules _rules;

        public FilterScoringGeneratorVisitor(ScoringRules rules)
        {
            _rules = rules;
        }

        /// <summary>
        /// Visits binary expressions and converts them to scoring logic
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            return node.NodeType switch
            {
                // OR conditions: sum the scores of both sides and apply OR weight
                ExpressionType.OrElse => Expression.Multiply(
                    Expression.Add(ConvertToInt(left), ConvertToInt(right)),
                    Expression.Constant(_rules.OrWeight)),

                // AND conditions: sum the scores of both sides and apply AND weight  
                ExpressionType.AndAlso => Expression.Multiply(
                    Expression.Add(ConvertToInt(left), ConvertToInt(right)),
                    Expression.Constant(_rules.AndWeight)),

                // Comparison operations: convert to 1 or 0 and apply condition weight
                ExpressionType.Equal or ExpressionType.NotEqual or
                ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or
                ExpressionType.LessThan or ExpressionType.LessThanOrEqual =>
                    Expression.Multiply(
                        Expression.Condition(node, Expression.Constant(1), Expression.Constant(0)),
                        Expression.Constant(_rules.ConditionWeight)),

                // For other binary operations, use default visitor behavior
                _ => base.VisitBinary(node)
            };
        }
        /// <summary>
        /// Visits method call expressions and converts boolean-returning methods to scores
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Type == typeof(bool))
            {
                // Convert boolean method calls to 1 or 0 and apply condition weight
                return Expression.Multiply(
                    Expression.Condition(node, Expression.Constant(1), Expression.Constant(0)),
                    Expression.Constant(_rules.ConditionWeight));
            }

            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Visits member expressions and converts boolean members to scores
        /// </summary>
        protected override Expression VisitMember(MemberExpression node)
        {
            // Convert boolean member access to 1 or 0 with condition weight
            if (node.Type == typeof(bool))
            {
                return Expression.Multiply(
                    Expression.Condition(
                        node,
                        Expression.Constant(1),
                        Expression.Constant(0)),
                    Expression.Constant(_rules.ConditionWeight));
            }

            return base.VisitMember(node);
        }

        /// <summary>
        /// Converts an expression to integer type for scoring
        /// </summary>
        /// <param name="expression">Expression to convert</param>
        /// <returns>Integer expression representing the score</returns>
        private Expression ConvertToInt(Expression expression)
        {
            // If already integer, return as-is
            if (expression.Type == typeof(int))
                return expression;

            // If boolean, convert to 1 (true) or 0 (false)
            if (expression.Type == typeof(bool))
                return Expression.Condition(
                    expression,
                    Expression.Constant(1),
                    Expression.Constant(0));

            // For other types, return unchanged (may cause runtime issues)
            return expression;
        }
    }
}