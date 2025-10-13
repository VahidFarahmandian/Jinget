namespace Jinget.Core.Tests.Utilities.Expressions;

[TestClass]
public class FilterScoringGeneratorUtilityTests
{
    private class TestEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string City { get; set; }
        public bool IsActive { get; set; }
        public decimal Salary { get; set; }
    }

    [TestMethod]
    public void GenerateScoreExpression_WithSimpleEqualCondition_ReturnsCorrectScore()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age == 25;
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityMatch = new TestEntity { Age = 25 };
        var entityNoMatch = new TestEntity { Age = 30 };

        Assert.AreEqual(1, scoreFunc(entityMatch)); // Condition true = 1 point
        Assert.AreEqual(0, scoreFunc(entityNoMatch)); // Condition false = 0 points
    }

    [TestMethod]
    public void GenerateScoreExpression_WithOrCondition_ReturnsSumOfScores()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age == 25 || x.City == "Tehran";
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityFirstMatch = new TestEntity { Age = 25, City = "Shiraz" }; // 1 point
        var entitySecondMatch = new TestEntity { Age = 30, City = "Tehran" }; // 1 point  
        var entityBothMatch = new TestEntity { Age = 25, City = "Tehran" }; // 2 points
        var entityNoMatch = new TestEntity { Age = 30, City = "Shiraz" }; // 0 points

        Assert.AreEqual(1, scoreFunc(entityFirstMatch));
        Assert.AreEqual(1, scoreFunc(entitySecondMatch));
        Assert.AreEqual(2, scoreFunc(entityBothMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithAndCondition_ReturnsSumOfScores()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age > 20 && x.City == "Tehran";
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityBothMatch = new TestEntity { Age = 25, City = "Tehran" }; // 2 points
        var entityFirstMatch = new TestEntity { Age = 25, City = "Shiraz" }; // 1 point
        var entitySecondMatch = new TestEntity { Age = 18, City = "Tehran" }; // 1 point
        var entityNoMatch = new TestEntity { Age = 18, City = "Shiraz" }; // 0 points

        Assert.AreEqual(2, scoreFunc(entityBothMatch));
        Assert.AreEqual(1, scoreFunc(entityFirstMatch));
        Assert.AreEqual(1, scoreFunc(entitySecondMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithMethodCall_ReturnsCorrectScore()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Name.StartsWith("John");
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityMatch = new TestEntity { Name = "John Doe" };
        var entityNoMatch = new TestEntity { Name = "Jane Doe" };

        Assert.AreEqual(1, scoreFunc(entityMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithComplexFilter_ReturnsCorrectScore()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x =>
            x.Age > 18 && x.City == "Tehran" ||
            x.Salary > 1000 && x.IsActive;

        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityAllMatch = new TestEntity { Age = 25, City = "Tehran", Salary = 2000, IsActive = true }; // 4 points
        var entityFirstGroupMatch = new TestEntity { Age = 25, City = "Tehran", Salary = 500, IsActive = false }; // 2 points
        var entitySecondGroupMatch = new TestEntity { Age = 17, City = "Shiraz", Salary = 2000, IsActive = true }; // 2 points
        var entityPartialMatch = new TestEntity { Age = 25, City = "Shiraz", Salary = 2000, IsActive = false }; // 1 point
        var entityNoMatch = new TestEntity { Age = 17, City = "Shiraz", Salary = 500, IsActive = false }; // 0 points

        Assert.AreEqual(4, scoreFunc(entityAllMatch));
        Assert.AreEqual(2, scoreFunc(entityFirstGroupMatch));
        Assert.AreEqual(2, scoreFunc(entitySecondGroupMatch));
        Assert.AreEqual(2, scoreFunc(entityPartialMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithCustomConditionWeight_ReturnsWeightedScore()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age == 25 || x.City == "Tehran";
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 5 }; // Each condition worth 5 points

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityBothMatch = new TestEntity { Age = 25, City = "Tehran" }; // 10 points (5 + 5)
        var entitySingleMatch = new TestEntity { Age = 25, City = "Shiraz" }; // 5 points

        Assert.AreEqual(10, scoreFunc(entityBothMatch));
        Assert.AreEqual(5, scoreFunc(entitySingleMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithCustomAndWeight_AppliesAndWeightToAndConditions()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age > 18 && x.City == "Tehran";
        var rules = new FilterScoringGeneratorUtility.ScoringRules
        {
            AndWeight = 3, // AND conditions get multiplied by 3
            ConditionWeight = 1
        };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, rules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        // Expected: (1 + 1) * 3 = 6 points (each condition gets 1 point, AND multiplies by 3)
        var entityBothMatch = new TestEntity { Age = 25, City = "Tehran" };
        // Expected: (1 + 0) * 3 = 3 points
        var entityFirstMatch = new TestEntity { Age = 25, City = "Shiraz" };
        // Expected: (0 + 1) * 3 = 3 points
        var entitySecondMatch = new TestEntity { Age = 17, City = "Tehran" };
        // Expected: (0 + 0) * 3 = 0 points
        var entityNoMatch = new TestEntity { Age = 17, City = "Shiraz" };

        Assert.AreEqual(6, scoreFunc(entityBothMatch));
        Assert.AreEqual(3, scoreFunc(entityFirstMatch));
        Assert.AreEqual(3, scoreFunc(entitySecondMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithCustomOrWeight_AppliesOrWeightToOrConditions()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age == 25 || x.City == "Tehran";
        var rules = new FilterScoringGeneratorUtility.ScoringRules
        {
            OrWeight = 2, // OR conditions get multiplied by 2
            ConditionWeight = 1
        };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, rules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        // Expected: (1 + 1) * 2 = 4 points (each condition gets 1 point, OR multiplies by 2)
        var entityBothMatch = new TestEntity { Age = 25, City = "Tehran" };
        // Expected: (1 + 0) * 2 = 2 points
        var entityFirstMatch = new TestEntity { Age = 25, City = "Shiraz" };
        // Expected: (0 + 1) * 2 = 2 points
        var entitySecondMatch = new TestEntity { Age = 30, City = "Tehran" };
        // Expected: (0 + 0) * 2 = 0 points
        var entityNoMatch = new TestEntity { Age = 30, City = "Shiraz" };

        Assert.AreEqual(4, scoreFunc(entityBothMatch));
        Assert.AreEqual(2, scoreFunc(entityFirstMatch));
        Assert.AreEqual(2, scoreFunc(entitySecondMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithCustomAndAndOrWeights_AppliesBothWeightsInComplexExpression()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x =>
            (x.Age > 18 && x.City == "Tehran") ||
            (x.Salary > 1000 && x.IsActive);

        var rules = new FilterScoringGeneratorUtility.ScoringRules
        {
            AndWeight = 2,    // AND conditions multiplied by 2
            OrWeight = 3,     // OR conditions multiplied by 3  
            ConditionWeight = 1
        };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, rules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        // First group: (Age>18(1) + City==Tehran(1)) * AndWeight(2) = (2) * 2 = 4
        // Second group: (Salary>1000(0) + IsActive(0)) * AndWeight(2) = (0) * 2 = 0  
        // Total: (4 + 0) * OrWeight(3) = 4 * 3 = 12 points
        var entityFirstGroupMatch = new TestEntity { Age = 25, City = "Tehran", Salary = 500, IsActive = false };

        // First group: (0 + 0) * 2 = 0
        // Second group: (1 + 1) * 2 = 4
        // Total: (0 + 4) * 3 = 12 points
        var entitySecondGroupMatch = new TestEntity { Age = 17, City = "Shiraz", Salary = 2000, IsActive = true };

        // First group: (1 + 1) * 2 = 4
        // Second group: (1 + 1) * 2 = 4  
        // Total: (4 + 4) * 3 = 24 points
        var entityBothGroupsMatch = new TestEntity { Age = 25, City = "Tehran", Salary = 2000, IsActive = true };

        // First group: (1 + 0) * 2 = 2
        // Second group: (0 + 0) * 2 = 0
        // Total: (2 + 0) * 3 = 6 points
        var entityPartialFirstGroup = new TestEntity { Age = 25, City = "Shiraz", Salary = 500, IsActive = false };

        Assert.AreEqual(12, scoreFunc(entityFirstGroupMatch));
        Assert.AreEqual(12, scoreFunc(entitySecondGroupMatch));
        Assert.AreEqual(24, scoreFunc(entityBothGroupsMatch));
        Assert.AreEqual(6, scoreFunc(entityPartialFirstGroup));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithNotEqualCondition_ReturnsCorrectScore()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age != 25;
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityMatch = new TestEntity { Age = 30 };
        var entityNoMatch = new TestEntity { Age = 25 };

        Assert.AreEqual(1, scoreFunc(entityMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithComparisonOperators_ReturnsCorrectScore()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age > 18 && x.Salary <= 5000;
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityMatch = new TestEntity { Age = 25, Salary = 4000 }; // 2 points
        var entityPartialMatch = new TestEntity { Age = 25, Salary = 6000 }; // 1 point
        var entityNoMatch = new TestEntity { Age = 17, Salary = 5500 }; // 0 points

        Assert.AreEqual(2, scoreFunc(entityMatch));
        Assert.AreEqual(1, scoreFunc(entityPartialMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithBooleanProperty_ReturnsCorrectScore()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.IsActive;
        var scoringRules = new FilterScoringGeneratorUtility.ScoringRules { ConditionWeight = 1 };

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter, scoringRules);
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entityMatch = new TestEntity { IsActive = true };
        var entityNoMatch = new TestEntity { IsActive = false };

        Assert.AreEqual(1, scoreFunc(entityMatch));
        Assert.AreEqual(0, scoreFunc(entityNoMatch));
    }

    [TestMethod]
    public void GenerateScoreExpression_WithDefaultRules_WhenRulesNotProvided()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = x => x.Age == 25;

        // Act
        var scoreExpression = FilterScoringGeneratorUtility.GenerateScoreExpression(filter); // No rules provided
        var scoreFunc = scoreExpression.Compile();

        // Assert
        var entity = new TestEntity { Age = 25 };
        Assert.AreEqual(1, scoreFunc(entity)); // Should use default weight of 1
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GenerateScoreExpression_WithNullFilter_ThrowsException()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> filter = null;

        // Act & Assert
        FilterScoringGeneratorUtility.GenerateScoreExpression(filter);
    }
}