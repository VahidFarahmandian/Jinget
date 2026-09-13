namespace Jinget.Core.Tests.CodeDom;

[TestClass]
public class JingetDynamicCodeTests
{
    [TestMethod]
    public void Should_compile_and_execute_dynamic_code_at_runtime_void_parameterless()
    {
        string expectedSource = @"
using System;
namespace JingetDynamic {
    internal sealed class DynamicInvoker {
        public void DynamicInvoke() {
            int x = 2*2;
        }
    }
}
";
        string source = @"int x = 2*2";

        var result = JingetDynamicCode.Execute(source, out List<string> errors, out string compiledSourceCode);

        Assert.IsNull(result);
        Assert.IsEmpty(errors);
        Assert.AreEqual(expectedSource.Trim(), compiledSourceCode);
    }

    [TestMethod]
    public void Should_compile_and_execute_dynamic_code_at_runtime_int_parameterless()
    {
        int expectedResult = 4;
        string expectedSource = @"
using System;
namespace JingetDynamic {
    internal sealed class DynamicInvoker {
        public int DynamicInvoke() {
            return 2*2;
        }
    }
}
";
        string source = @"return 2*2";

        var result = JingetDynamicCode.Execute(source, out List<string> errors, out string compiledSourceCode,
            new JingetDynamicCode.MethodOptions { ReturnType = typeof(int) });

        Assert.IsEmpty(errors);
        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(expectedSource.Trim(), compiledSourceCode);
    }

    [TestMethod]
    public void Should_compile_and_execute_dynamic_code_at_runtime_void_parametric()
    {
        string expectedSource = @"
using System;
using System.Linq;
namespace JingetDynamic {
    internal sealed class DynamicInvoker {
        public void DynamicInvoke(int a, double b) {
            double c = a*b;
        }
    }
}
";
        string source = @"using System.Linq; double c = a*b;";

        var result = JingetDynamicCode.Execute(source, out List<string> errors, out string compiledSourceCode,
            new JingetDynamicCode.MethodOptions
            {
                Parameters =
            [
                new() {Name = "a",Type = typeof(int)},
                new() {Name = "b",Type = typeof(double)}
            ]
            });

        Assert.IsEmpty(errors);
        Assert.IsNull(result);
        Assert.AreEqual(expectedSource.Trim(), compiledSourceCode);
    }

    [TestMethod]
    public void Should_compile_and_return_error()
    {
        string expectedSource = @"
using System;
namespace JingetDynamic {
    internal sealed class DynamicInvoker {
        public void DynamicInvoke(int a, double b) {
            c = a*b;
        }
    }
}
";

        string source = @"c = a*b;";

        var result = JingetDynamicCode.Execute(source, out List<string> errors, out string compiledSourceCode,
            new JingetDynamicCode.MethodOptions
            {
                Parameters =
            [
                new() {Name = "a",Type = typeof(int)},
                new() {Name = "b",Type = typeof(double)}
            ]
            });

        Assert.IsNotEmpty(errors);
        Assert.IsNull(result);
        Assert.AreEqual(expectedSource.Trim(), compiledSourceCode);
    }

    [TestMethod]
    public void Should_compile_and_execute_multiline_dynamic_code_at_runtime()
    {
        string expectedResult = "1399/07/21";

        string code = @"
                          using Jinget.Core;
                          return ConvertDate(dt);
                          string ConvertDate(DateTime dt)
                          {
                                return Jinget.Core.Utilities.DateTimeUtility.ToSolarDate(dt, false);
                          }";
        var result = JingetDynamicCode.Execute(
            code,
            out List<string> errors, out string compiledSourceCode,
            options: new JingetDynamicCode.MethodOptions
            {
                ReturnType = typeof(string),
                Parameters =
                [
                    new() {
                        Name="dt",
                        Value=new DateTime(2020, 10, 12),
                        Type = typeof(DateTime)
                    }
                ]
            },
            references: [typeof(DateTimeUtility).Assembly.Location]);

        Assert.IsEmpty(errors);
        Assert.IsFalse(string.IsNullOrEmpty(compiledSourceCode));
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void Should_compile_and_execute_single_dynamic_code_at_runtime()
    {
        string expectedResult = "1399/07/21";

        string code = @"using Jinget.Core;return ConvertDate(dt);string ConvertDate(DateTime dt){return Jinget.Core.Utilities.DateTimeUtility.ToSolarDate(dt, false);}";

        var result = JingetDynamicCode.Execute(
            code,
            out List<string> errors,
            out string compiledSourceCode,
            options: new JingetDynamicCode.MethodOptions
            {
                ReturnType = typeof(string),
                Parameters =
                [
                    new() {
                        Name="dt",
                        Value=new DateTime(2020, 10, 12),
                        Type = typeof(DateTime)
                    }
                ]
            },
            references: [typeof(DateTimeUtility).Assembly.Location]);

        Assert.IsEmpty(errors);
        Assert.IsFalse(string.IsNullOrEmpty(compiledSourceCode));
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void Should_compile_and_execute_single_dynamic_code_at_runtime_with_pass_parameter_value()
    {
        int expectedResult = 6;

        string code = @"return GetValue(a, b);int GetValue(int a, int b){return a*b;}";

        var result = JingetDynamicCode.Execute(code, out List<string> errors, out string compiledSourceCode,
            new JingetDynamicCode.MethodOptions()
            {
                ReturnType = typeof(int),
                Parameters =
                [
                    new() {
                        Name = "a",
                        Value = 2,
                        Type = typeof(int)
                    },
                    new() {
                        Name = "b",
                        Value = 3,
                        Type = typeof(int)
                    }
                ]
            });

        Assert.IsEmpty(errors);
        Assert.IsFalse(string.IsNullOrEmpty(compiledSourceCode));
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void Should_throw_exception()
    {
        string source = "";
        for (int i = 0; i < 10001; i++)
            source += "c = a*b;";

        Assert.Throws<ArgumentException>(() =>
        {
            JingetDynamicCode.Execute(
                source,
                out List<string> _,
                out string _);
        });
    }
}