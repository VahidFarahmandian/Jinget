using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using Jinget.Core.Tests._BaseData;
using System.Collections.Generic;
using System.Linq;
using Jinget.Core.Utilities.Expressions;
using Jinget.Core.Types;

namespace Jinget.Core.ExtensionMethods.Expressions.Tests
{
    [TestClass()]
    public class ExpressionUtilityTests
    {
        [TestMethod()]
        public void should_create_a_member_init_expression()
        {
            string[] inputs = new string[] { "Property1", "Property2" };
            string parameterName = "x";
            Expression<Func<TestClass, TestClass>> expectedExpression = x => new TestClass { Property1 = x.Property1, Property2 = x.Property2 };

            Expression<Func<TestClass, TestClass>> result = ExpressionUtility.CreateMemberInitExpression<TestClass>(parameterName, inputs);

            Assert.AreEqual(expectedExpression.Type, result.Type);
        }

        [TestMethod()]
        public void should_create_a_member_access_expression()
        {
            string[] inputs = new string[] { "Property1", "Property2" };
            string parameterName = "x";
            Expression<Func<TestClass, TestClass>> expectedExpression = x => new TestClass { Property1 = x.Property1, Property2 = x.Property2 };

            Expression<Func<TestClass, TestClass>> result = ExpressionUtility.CreateMemberInitExpression<TestClass>(parameterName, inputs);

            Assert.AreEqual(expectedExpression.Type, result.Type);
        }

        #region null or empty filters

        [TestMethod]
        public void should_return_all_data_using_empty_string_filter()
        {
            string json = "";
            string expectedFilter = "x => True";
            var data = new List<TestClass>
            {
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            }.AsQueryable();

            var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json);
            Assert.AreEqual(expectedFilter, filter.ToString());

            var result = data.Where(filter).ToList();
            Assert.IsTrue(result.Count == 5);
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
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            }.AsQueryable();

            var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json, false);
            Assert.AreEqual(expectedFilter, filter.ToString());

            var result = data.Where(filter).ToList();
            Assert.IsTrue(result.Count == 0);
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
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            }.AsQueryable();

            var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json);
            Assert.AreEqual(expectedFilter, filter.ToString());

            var result = data.Where(filter).ToList();

            Assert.IsTrue(result.Count == 5);
            Assert.IsTrue(result.First().Property1 == 1);
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
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            }.AsQueryable();

            var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json, false);
            Assert.AreEqual(expectedFilter, filter.ToString());

            var result = data.Where(filter).ToList();
            Assert.IsTrue(result.Count == 0);
        }

        #endregion

        #region json string filter

        [TestMethod]
        public void should_return_filtered_data_using_json_filter()
        {
            string json = @"{
                                'Property3':'urmia',
                                'Property4':true
                            }";
            string expectedFilter = "x => ((x.Property3 == \"urmia\") AndAlso (x.Property4 == True))";

            var data = new List<TestClass>
            {
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            }.AsQueryable();

            var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(json);
            Assert.AreEqual(expectedFilter, filter.ToString());

            var result = data.Where(filter).ToList();

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.First().Property1 == 4);
        }

        #endregion

        #region filter criteria

        [TestMethod]
        public void should_return_filtered_data_using_single_filter_criteria()
        {
            List<FilterCriteria> filters = new()
            {
                new()
                {
                    Operand = "Property2",
                    Operator = Enumerations.Operator.Contains,
                    Value = "ah"
                }
            };
            string expectedFilter = "x => x.Property2.Contains(\"ah\")";

            var data = new List<TestClass>
            {
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            }.AsQueryable();

            var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(filters);
            Assert.AreEqual(expectedFilter, filter.ToString());

            var result = data.Where(filter).ToList();

            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.First().Property1 == 2);
        }

        [TestMethod]
        public void should_return_filtered_data_using_multiple_filter_criterias()
        {
            List<FilterCriteria> filters = new()
            {
                new()
                {
                    Operand = "Property2",
                    Operator = Enumerations.Operator.Contains,
                    Value = "ah",
                    NextConditionCombination = Enumerations.ConditionCombiningType.AndAlso
                },
                new()
                {
                    Operand = "Property3",
                    Operator = Enumerations.Operator.Equal,
                    Value = "urmia",
                    NextConditionCombination = Enumerations.ConditionCombiningType.OrElse
                },
                new()
                {
                    Operand = "Property4",
                    Operator = Enumerations.Operator.Equal,
                    Value = true
                },
            };
            string expectedFilter = "x => ((x.Property2.Contains(\"ah\") AndAlso (x.Property3 == \"urmia\")) OrElse (x.Property4 == True))";

            var data = new List<TestClass>
            {
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            }.AsQueryable();

            var filter = ExpressionUtility.ConstructBinaryExpression<TestClass>(filters);
            Assert.AreEqual(expectedFilter, filter.ToString());

            var result = data.Where(filter).ToList();

            Assert.IsTrue(result.Count == 4);
            Assert.IsTrue(result.First().Property1 == 1);
        }

        #endregion
    }
}