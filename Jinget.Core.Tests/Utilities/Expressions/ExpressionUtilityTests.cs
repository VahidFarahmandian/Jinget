using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using Jinget.Core.Tests._BaseData;
using System.Collections.Generic;
using System.Linq;
using Jinget.Core.Utilities.Expressions;

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

            Expression<Func<TestClass, TestClass>> result = Utilities.Expressions.ExpressionUtility.CreateMemberInitExpression<TestClass>(parameterName, inputs);

            Assert.AreEqual(expectedExpression.Type, result.Type);
        }

        [TestMethod()]
        public void should_create_a_member_access_expression()
        {
            string[] inputs = new string[] { "Property1", "Property2" };
            string parameterName = "x";
            Expression<Func<TestClass, TestClass>> expectedExpression = x => new TestClass { Property1 = x.Property1, Property2 = x.Property2 };

            Expression<Func<TestClass, TestClass>> result = Utilities.Expressions.ExpressionUtility.CreateMemberInitExpression<TestClass>(parameterName, inputs);

            Assert.AreEqual(expectedExpression.Type, result.Type);
        }

        [TestMethod]
        public void should_return_all_data_using_empty_json_filter()
        {
            string json = "";
            var data = new List<TestClass>
            {
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            };

            var result = data.Where(ExpressionUtility.ConstructBinaryExpression<TestClass>(json)).ToList();

            Assert.IsTrue(result.Count == 5);
        }

        [TestMethod]
        public void should_return_filtered_data_using_json_filter()
        {
            string json = @"{
                                'Property3':'urmia',
                                'Property4':true
                            }";
            var data = new List<TestClass>
            {
                new TestClass{ Property1=1, Property2="ali", Property3="tehran"   ,Property4=true},
                new TestClass{ Property1=2, Property2="rahim", Property3="karaj"  ,Property4=true},
                new TestClass{ Property1=3, Property2="vahid", Property3="urmia"  ,Property4=false},
                new TestClass{ Property1=4, Property2="saeid", Property3="urmia"  ,Property4=true},
                new TestClass{ Property1=5, Property2="maryam", Property3="urmia" ,Property4=false }
            };

            var result = data.Where(ExpressionUtility.ConstructBinaryExpression<TestClass>(json)).ToList();

            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result.First().Property1 == 4);
        }
    }
}