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

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void should_throw_exception_for_null_input_mapto()
    {
        ResponseResult<SampleModel>.MapTo<SampleViewModel>(null);
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidOperationException))]
    public void should_throw_exception_for_incompatibale_types_mapto()
    {
        var source = new ResponseResult<SampleModel>([new SampleModel { Id = 1, Name = "Vahid" }], 1000);
        ResponseResult<SampleModel>.MapTo<SampleInterfaceClass>(source);
    }

    [TestMethod()]
    public void should_map_source_data_type_to_destination_data_type()
    {
        var source = new ResponseResult<SampleModel>(
            [new SampleModel { Id = 1, Name = "Vahid" }, new SampleModel { Id = 2, Name = "Ali" }], 1000);
        var result = ResponseResult<SampleModel>.MapTo<SampleViewModel>(source);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleViewModel>));
        Assert.AreEqual(result.Data.First().Name, "Vahid");
        Assert.AreEqual(result.Data.Last().Name, "Ali");
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
        Assert.AreEqual(result.EffectedRowsCount, 1000);
    }

    [TestMethod()]
    public void should_create_new_responseResultObject_using_T_auto_effectedCount()
    {
        SampleModel obj = new() { Id = 1, Name = "Vahid" };

        var result = new ResponseResult<SampleModel>(obj);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleModel>));
        Assert.IsTrue(result.EffectedRowsCount == 1);
        Assert.IsTrue(result.Data.First().Name == "Vahid");
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void should_create_new_responseResultObject_using_T()
    {
        SampleModel obj = new() { Id = 1, Name = "Vahid" };

        var result = new ResponseResult<SampleModel>(obj, 1);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleModel>));
        Assert.IsTrue(result.EffectedRowsCount == 1);
        Assert.IsTrue(result.Data.First().Name == "Vahid");
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void should_create_new_responseResultObject_using_IEnumerableT()
    {
        List<SampleModel> obj = [new SampleModel { Id = 1, Name = "Vahid" }, new SampleModel { Id = 2, Name = "John" }];

        var result = new ResponseResult<SampleModel>(obj, 2);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleModel>));
        Assert.IsTrue(result.EffectedRowsCount == 2);
        Assert.IsTrue(result.Data.First().Name == "Vahid");
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void should_create_new_responseResultObject_using_IEnumerableT_auto_effectedCount()
    {
        List<SampleModel> obj = [new SampleModel { Id = 1, Name = "Vahid" }, new SampleModel { Id = 2, Name = "John" }];

        var result = new ResponseResult<SampleModel>(obj);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleModel>));
        Assert.IsTrue(result.EffectedRowsCount == 2);
        Assert.IsTrue(result.Data.First().Name == "Vahid");
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void should_create_new_responseResultObject_using_null()
    {
        var result = new ResponseResult<SampleModel>((IEnumerable<SampleModel>)null);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleModel>));
        Assert.IsTrue(result.EffectedRowsCount == 0);
        Assert.IsTrue(result.Data.FirstOrDefault() is null);
        Assert.IsTrue(result.Data.Count == 0);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void should_handle_empty_collection()
    {
        List<SampleModel> emptyList = [];

        var result = new ResponseResult<SampleModel>(emptyList);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleModel>));
        Assert.IsTrue(result.EffectedRowsCount == 0);
        Assert.IsTrue(result.Data.Count == 0);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }
}