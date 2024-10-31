namespace Jinget.Core.Tests.Types;

[TestClass]
public class ResponseResultTests
{
    [TestMethod]
    public void should_set_isSuccess_as_true_and_isFailure_as_false_for_ienumerable_data()
    {
        var obj = new ResponseResult<TestClass>([]);
        Assert.IsTrue(obj.IsSuccess);
        Assert.IsFalse(obj.IsFailure);
    }

    [TestMethod]
    public void should_set_isSuccess_as_false_and_isFailure_as_true_for_ienumerable_data()
    {
        var obj = new ResponseResult<ProblemDetails>([]);
        Assert.IsFalse(obj.IsSuccess);
        Assert.IsTrue(obj.IsFailure);
    }

    [TestMethod]
    public void should_set_isSuccess_as_true_and_isFailure_as_false_for_t_data()
    {
        var obj = new ResponseResult<TestClass>(new TestClass());
        Assert.IsTrue(obj.IsSuccess);
        Assert.IsFalse(obj.IsFailure);
    }

    [TestMethod]
    public void should_set_isSuccess_as_false_and_isFailure_as_true_for_t_data()
    {
        var obj = new ResponseResult<ProblemDetails>(new ProblemDetails());
        Assert.IsFalse(obj.IsSuccess);
        Assert.IsTrue(obj.IsFailure);
    }
}