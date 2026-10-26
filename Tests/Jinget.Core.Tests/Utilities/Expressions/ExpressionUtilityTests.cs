namespace Jinget.Core.Tests.Utilities.Expressions;

[TestClass]
public class ExpressionUtilityTests
{
    [TestMethod]
    public void should_create_a_member_init_expression()
    {
        string[] inputs = ["property1", "property2"];
        string parameterName = "x";
        Expression<Func<TestClass, TestClass>> expectedExpression = x => new TestClass { Property1 = x.Property1, Property2 = x.Property2 };

        Expression<Func<TestClass, TestClass>> result = ExpressionUtility.CreateMemberInitExpression<TestClass>(parameterName, inputs);

        Assert.AreEqual(expectedExpression.Type, result.Type);
    }


    [TestMethod]
    public void Should_create_member_init_expression_with_nested_properties()
    {
        // Arrange
        string[] inputs =
        [
            "name",
            "profile.department",
            "role.claim.title",
            "orders.serial"
        ];

        Expression<Func<Employee, Employee>> expression =
            ExpressionUtility.CreateMemberInitExpression<Employee>(
                "x",
                inputs);

        var source = new Employee
        {
            Name = "Vahid",
            Profile = new EmployeeProfile
            {
                Department = "Engineering",
                Email = "vahid@example.com"
            },
            Role = new EmployeeRole
            {
                Claim = new EmployeeClaim
                {
                    Title = "Administrator",
                    Code = "ADMIN"
                },
                Description = "System administrator"
            },
            Orders =
            [
                new EmployeeOrder
                {
                    Serial = "ORD-001",
                    Amount = 100
                },
                new EmployeeOrder
                {
                    Serial = "ORD-002",
                    Amount = 200
                }
            ]
        };

        // Act
        var result = expression.Compile()(source);

        // Assert
        Assert.AreEqual("Vahid", result.Name);

        Assert.AreEqual(
            "Engineering",
            result.Profile.Department);

        Assert.AreEqual(
            "Administrator",
            result.Role.Claim.Title);

        Assert.HasCount(2, result.Orders);

        Assert.AreEqual(
            "ORD-001",
            result.Orders[0].Serial);

        Assert.AreEqual(
            "ORD-002",
            result.Orders[1].Serial);

        // Unselected properties should not be copied.
        Assert.IsNull(result.Profile.Email);

        Assert.IsNull(result.Role.Claim.Code);

        Assert.IsNull(result.Role.Description);

        Assert.AreEqual(0, result.Orders[0].Amount);
        Assert.AreEqual(0, result.Orders[1].Amount);
    }

    [TestMethod]
    public void CreateMemberInitExpression_WithObjectReturnType_ShouldNotAddConvert()
    {
        // Act
        Expression<Func<Employee, object>> expression =
            ExpressionUtility.CreateMemberInitExpression<Employee, object>(
                "x",
                "name");

        // Assert
        Assert.AreEqual(typeof(object), expression.ReturnType);
        Assert.IsInstanceOfType<MemberInitExpression>(expression.Body);

        var memberInit = (MemberInitExpression)expression.Body;

        Assert.AreEqual(typeof(Employee), memberInit.Type);
        Assert.HasCount(1, memberInit.Bindings);
        Assert.AreEqual(
            nameof(Employee.Name),
            memberInit.Bindings[0].Member.Name);
    }

    [TestMethod]
    public void should_create_a_equal_condition_expression()
    {
        Expression<Func<TestClass, bool>> expectedResult = x => x.Property1 == 1;
        var result = ExpressionUtility.CreateEqualCondition<TestClass, int>("Property1", 1, "x");

        Assert.AreEqual(expectedResult.Type, result.Type);
    }

    #region null or empty filters

    [TestMethod]
    public void should_return_all_data_using_empty_string_filter()
    {
        string json = "";
        string expectedFilter = "x => True";
        var data = new List<TestClass>
        {
            new() { Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
            new() { Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
            new() { Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
            new() { Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
            new() { Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
        }.AsQueryable();

        var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json);
        Assert.AreEqual(expectedFilter, filter.ToString());

        var result = data.Where(filter).ToList();
        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void should_return_no_data_using_empty_string_filter()
    {
#nullable enable
        object? json = "";
#nullable disable

        string expectedFilter = "x => False";

        var data = new List<TestClass>
        {
            new() { Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
            new() { Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
            new() { Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
            new() { Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
            new() { Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
        }.AsQueryable();

        var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json, false);
        Assert.AreEqual(expectedFilter, filter.ToString());

        var result = data.Where(filter).ToList();
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void should_return_all_data_using_null_object()
    {
#nullable enable
        object? json = null;
#nullable disable

        string expectedFilter = "x => True";

        var data = new List<TestClass>
        {
            new() { Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
            new() { Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
            new() { Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
            new() { Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
            new() { Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
        }.AsQueryable();

        var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json);
        Assert.AreEqual(expectedFilter, filter.ToString());

        var result = data.Where(filter).ToList();

        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(1, result.First().Property1);
    }

    [TestMethod]
    public void should_return_no_data_using_null_object()
    {
#nullable enable
        object? json = null;
#nullable disable

        string expectedFilter = "x => False";

        var data = new List<TestClass>
        {
            new() { Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
            new() { Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
            new() { Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
            new() { Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
            new() { Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
        }.AsQueryable();

        var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json, false);
        Assert.AreEqual(expectedFilter, filter.ToString());

        var result = data.Where(filter).ToList();
        Assert.IsEmpty(result);
    }

    #endregion

    #region json string filter

    [TestMethod]
    public void should_return_filtered_data_using_json_filter()
    {
        string json = @"{
                                ""Property3"":""urmia"",
                                ""Property4"":true
                            }";
        string expectedFilter = "x => ((x.Property3 == \"urmia\") AndAlso (x.Property4 == True))";

        var data = new List<TestClass>
        {
            new() { Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
            new() { Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
            new() { Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
            new() { Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
            new() { Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
        }.AsQueryable();

        var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json);
        Assert.AreEqual(expectedFilter, filter.ToString());

        var result = data.Where(filter).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(4, result.First().Property1);
    }

    #endregion

    #region filter criteria

    [TestMethod]
    public void should_return_filtered_data_using_single_filter_criteria()
    {
        List<FilterCriteria> filters =
        [
            new()
            {
                Operand = "Property2",
                Operator = Enumerations.Operator.Contains,
                Value = "ah"
            }
        ];

        var data = new List<TestClass>
        {
            new() { Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
            new() { Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
            new() { Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
            new() { Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
            new() { Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
        }.AsQueryable();

        var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(filters);

        var result = data.Where(filter).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result.First().Property1);
    }

    [TestMethod]
    public void should_return_filtered_data_using_multiple_filter_criterias()
    {
        List<FilterCriteria> filters =
        [
            new()
            {
                Operand = "Property2",
                Operator = Enumerations.Operator.Contains,
                Value = "ah",
                NextConditionCombination = Enumerations.ConditionJoinType.AndAlso
            },
            new()
            {
                Operand = "Property3",
                Operator = Enumerations.Operator.Equal,
                Value = "urmia",
                NextConditionCombination = Enumerations.ConditionJoinType.OrElse
            },
            new()
            {
                Operand = "Property4",
                Operator = Enumerations.Operator.Equal,
                Value = true
            }
        ];

        var data = new List<TestClass>
        {
            new() { Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
            new() { Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
            new() { Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
            new() { Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
            new() { Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
        }.AsQueryable();

        var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(filters);

        var result = data.Where(filter).ToList();

        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(1, result.First().Property1);
    }



    private static List<Employee> CreateEmployees() =>
    [
        new()
        {
            Name = "Vahid",
            Profile = new()
            {
                Department = "Engineering",
                Email = "vahid@example.com"
            },
            Role = new()
            {
                Claim = new() { Title = "Developer", Code = "DEV" },
                Description = "Senior developer"
            },
            Orders = [new() { Serial = "ORD-001", Amount = 1500m }]
        },
        new()
        {
            Name = "Rahim",
            Profile = new()
            {
                Department = "Finance",
                Email = "rahim@example.com"
            },
            Role = new()
            {
                Claim = new() { Title = "Manager", Code = "MGR" },
                Description = "Finance manager"
            },
            Orders = [new() { Serial = "ORD-002", Amount = 500m }]
        },
        new()
        {
            Name = "Ahmad",
            Profile = new()
            {
                Department = "Finance",
                Email = null
            },
            Role = new()
            {
                Claim = new() { Title = "Intern", Code = "INT" },
                Description = "Intern"
            },
            Orders = [new() { Serial = "ORD-003", Amount = 2500m }]
        },
        new()
        {
            Name = "TestUser",
            Profile = new()
            {
                Department = "Engineering",
                Email = "test@example.com"
            },
            Role = new()
            {
                Claim = new() { Title = "Developer", Code = "DEV" },
                Description = "Developer"
            },
            Orders = [new() { Serial = "ORD-004", Amount = 3000m }]
        },
        new()
        {
            Name = "Sarah",
            Profile = new()
            {
                Department = "Engineering",
                Email = "sarah@example.com"
            },
            Role = new()
            {
                Claim = new() { Title = "Developer", Code = "DEV" },
                Description = "Developer"
            },
            Orders = [new() { Serial = "ORD-005", Amount = 750m }]
        }
    ];

    private static List<Employee> FilterEmployees(
        string operand,
        Enumerations.Operator op,
        object? value)
    {
        var filters = new List<FilterCriteria>
        {
            new()
            {
                Operand = operand,
                Operator = op,
                Value = value
            }
        };

        var predicate =
            ExpressionUtility.ConstructBinaryExpression<Employee>(filters);

        return CreateEmployees().AsQueryable()
            .Where(predicate)
            .ToList();
    }

    private static List<EmployeeOrder> FilterOrders(
        Enumerations.Operator op,
        object? value)
    {
        var filters = new List<FilterCriteria>
        {
            new()
            {
                Operand = nameof(EmployeeOrder.Amount),
                Operator = op,
                Value = value
            }
        };

        var predicate =
            ExpressionUtility.ConstructBinaryExpression<EmployeeOrder>(filters);

        return CreateEmployees()
            .SelectMany(e => e.Orders)
            .AsQueryable()
            .Where(predicate)
            .ToList();
    }

    [TestMethod]
    public void Should_Filter_Using_Equal()
    {
        var result = FilterEmployees(
            "Profile.Department",
            Enumerations.Operator.Equal,
            "Engineering");

        CollectionAssert.AreEqual(
            new[] { "Vahid", "TestUser", "Sarah" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_NotEqual()
    {
        var result = FilterEmployees(
            "Role.Claim.Title",
            Enumerations.Operator.NotEqual,
            "Developer");

        CollectionAssert.AreEqual(
            new[] { "Rahim", "Ahmad" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_Contains()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.Contains,
            "ah");

        Assert.AreSequenceEqual(
            new[] { "Vahid", "Rahim", "Ahmad", "Sarah" }, result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_NotContains()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.NotContains,
            "test");

        CollectionAssert.AreEqual(
            new[] { "Vahid", "Rahim", "Ahmad", "Sarah" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_StartsWith()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.StartsWith,
            "Sa");

        CollectionAssert.AreEqual(
            new[] { "Sarah" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_DoesNotStartWith()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.DoesNotStartWith,
            "Sa");

        CollectionAssert.AreEqual(
            new[] { "Vahid", "Rahim", "Ahmad", "TestUser" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_EndsWith()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.EndsWith,
            "id");

        CollectionAssert.AreEqual(
            new[] { "Vahid" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_DoesNotEndWith()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.DoesNotEndWith,
            "id");

        CollectionAssert.AreEqual(
            new[] { "Rahim", "Ahmad", "TestUser", "Sarah" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_IsNull()
    {
        var result = FilterEmployees(
            "Profile.Email",
            Enumerations.Operator.IsNull,
            null);

        CollectionAssert.AreEqual(
            new[] { "Ahmad" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_IsNotNull()
    {
        var result = FilterEmployees(
            "Profile.Email",
            Enumerations.Operator.IsNotNull,
            null);

        CollectionAssert.AreEqual(
            new[] { "Vahid", "Rahim", "TestUser", "Sarah" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_GreaterThan()
    {
        var result = FilterOrders(
            Enumerations.Operator.GreaterThan,
            1000m);

        CollectionAssert.AreEqual(
            new[] { "ORD-001", "ORD-003", "ORD-004" },
            result.Select(o => o.Serial).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_GreaterThanOrEqual()
    {
        var result = FilterOrders(
            Enumerations.Operator.GreaterThanOrEqual,
            1500m);

        CollectionAssert.AreEqual(
            new[] { "ORD-001", "ORD-003", "ORD-004" },
            result.Select(o => o.Serial).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_LowerThan()
    {
        var result = FilterOrders(
            Enumerations.Operator.LowerThan,
            1000m);

        CollectionAssert.AreEqual(
            new[] { "ORD-002", "ORD-005" },
            result.Select(o => o.Serial).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_LowerThanOrEqual()
    {
        var result = FilterOrders(
            Enumerations.Operator.LowerThanOrEqual,
            750m);

        CollectionAssert.AreEqual(
            new[] { "ORD-002", "ORD-005" },
            result.Select(o => o.Serial).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_In()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.In,
            new[] { "Vahid", "Sarah" });

        CollectionAssert.AreEqual(
            new[] { "Vahid", "Sarah" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_NotIn()
    {
        var result = FilterEmployees(
            nameof(Employee.Name),
            Enumerations.Operator.NotIn,
            new[] { "Vahid", "Sarah" });

        CollectionAssert.AreEqual(
            new[] { "Rahim", "Ahmad", "TestUser" },
            result.Select(e => e.Name).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_Between()
    {
        var result = FilterOrders(
            Enumerations.Operator.Between,
            new[] { 1000m, 2500m });

        CollectionAssert.AreEqual(
            new[] { "ORD-001", "ORD-003" },
            result.Select(o => o.Serial).ToArray());
    }

    [TestMethod]
    public void Should_Filter_Using_Mixed_AndAlso_OrElse_Conditions()
    {
        var filters = new List<FilterCriteria>
        {
            new()
            {
                Operand = nameof(Employee.Name),
                Operator = Enumerations.Operator.Contains,
                Value = "ah",
                NextConditionCombination =
                    Enumerations.ConditionJoinType.AndAlso
            },
            new()
            {
                Operand = "Profile.Department",
                Operator = Enumerations.Operator.Equal,
                Value = "Engineering",
                NextConditionCombination =
                    Enumerations.ConditionJoinType.OrElse
            },
            new()
            {
                Operand = "Role.Claim.Title",
                Operator = Enumerations.Operator.Equal,
                Value = "Intern"
            }
        };

        var predicate =
            ExpressionUtility.ConstructBinaryExpression<Employee>(filters);

        var result = CreateEmployees()
            .AsQueryable()
            .Where(predicate)
            .ToList();

        // (Name contains "ah" AND Department == "Engineering")
        // OR Role.Claim.Title == "Intern"
        Assert.AreSequenceEqual(
            new[] { "Vahid", "Ahmad", "Sarah" }, result.Select(e => e.Name).ToArray());
    }


    #endregion

    [TestMethod()]
    public void should_return_searchallexpression_combined_with_orelse()
    {
        Expression<Func<TestClass, bool>> expectedResult =
            x => x.Property2.ToLower().Contains("test string") ||
            x.Property3.ToLower().Contains("test string");

        var result = ExpressionUtility.CreateSearchAllColumnsExpression<TestClass>("test string");

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod()]
    public void should_return_searchallexpression_combined_with_case_sensitivity()
    {
        Expression<Func<TestClass, bool>> expectedResult = x => x.Property2.Contains("test string") || x.Property3.Contains("test string");

        var result = ExpressionUtility.CreateSearchAllColumnsExpression<TestClass>("test string", preserveCase: true);

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod()]
    public void should_return_searchallexpression_combined_with_andalso()
    {
        Expression<Func<TestClass, bool>> expectedResult =
            x => x.Property2.ToLower().Contains("test string") &&
            x.Property3.ToLower().Contains("test string");

        var result = ExpressionUtility.CreateSearchAllColumnsExpression<TestClass>("test string", conditionJoinType: ConditionJoinType.AndAlso);

        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }
}