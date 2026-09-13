namespace Jinget.Core.Tests.Types;

[TestClass]
public class ResponseResultTests
{
    [TestMethod]
    public void Should_set_isSuccess_as_true_and_isFailure_as_false_for_ienumerable_data()
    {
        var obj = new ResponseResult<TestClass>([]);
        Assert.IsTrue(obj.IsSuccess);
        Assert.IsFalse(obj.IsFailure);
    }

    [TestMethod]
    public void Should_set_isSuccess_as_false_and_isFailure_as_true_for_ienumerable_data()
    {
        var obj = new ResponseResult<ProblemDetails>([]);
        Assert.IsFalse(obj.IsSuccess);
        Assert.IsTrue(obj.IsFailure);
    }

    [TestMethod]
    public void Should_set_isSuccess_as_true_and_isFailure_as_false_for_t_data()
    {
        var obj = new ResponseResult<TestClass>(new TestClass());
        Assert.IsTrue(obj.IsSuccess);
        Assert.IsFalse(obj.IsFailure);
    }

    [TestMethod]
    public void Should_set_isSuccess_as_false_and_isFailure_as_true_for_t_data()
    {
        var obj = new ResponseResult<ProblemDetails>(new ProblemDetails());
        Assert.IsFalse(obj.IsSuccess);
        Assert.IsTrue(obj.IsFailure);
    }

    [TestMethod]
    public void Should_throw_exception_for_null_input_mapto()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ResponseResult<SampleModel>.MapTo<SampleViewModel>(null));
    }

    [TestMethod]
    public void Should_throw_exception_for_incompatibale_types_mapto()
    {
        var source = new ResponseResult<SampleModel>(
            [new SampleModel { Id = 1, Name = "Vahid" }],
            1000);

        Assert.Throws<InvalidOperationException>(() =>
            ResponseResult<SampleModel>.MapTo<SampleInterfaceClass>(source));
    }

    [TestMethod()]
    public void Should_map_source_data_type_to_destination_data_type()
    {
        var source = new ResponseResult<SampleModel>(
            [new SampleModel { Id = 1, Name = "Vahid" }, new SampleModel { Id = 2, Name = "Ali" }], 1000);
        var result = ResponseResult<SampleModel>.MapTo<SampleViewModel>(source);

        Assert.AreEqual(typeof(List<SampleViewModel>), result.Data.GetType());
        Assert.AreEqual("Vahid", result.Data.First().Name);
        Assert.AreEqual("Ali", result.Data.Last().Name);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
        Assert.AreEqual(1000, result.EffectedRowsCount);
    }

    [TestMethod()]
    public void Should_create_new_responseResultObject_using_T_auto_effectedCount()
    {
        SampleModel obj = new() { Id = 1, Name = "Vahid" };

        var result = new ResponseResult<SampleModel>(obj);

        Assert.AreEqual(typeof(List<SampleModel>), result.Data.GetType());
        Assert.AreEqual(1, result.EffectedRowsCount);
        Assert.AreEqual("Vahid", result.Data.First().Name);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void Should_create_new_responseResultObject_using_T()
    {
        SampleModel obj = new() { Id = 1, Name = "Vahid" };

        var result = new ResponseResult<SampleModel>(obj, 1);

        Assert.AreEqual(typeof(List<SampleModel>), result.Data.GetType());
        Assert.AreEqual(1, result.EffectedRowsCount);
        Assert.AreEqual("Vahid", result.Data.First().Name);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void Should_create_new_responseResultObject_using_IEnumerableT()
    {
        List<SampleModel> obj = [new SampleModel { Id = 1, Name = "Vahid" }, new SampleModel { Id = 2, Name = "John" }];

        var result = new ResponseResult<SampleModel>(obj, 2);

        Assert.AreEqual(typeof(List<SampleModel>), result.Data.GetType());
        Assert.AreEqual(2, result.EffectedRowsCount);
        Assert.AreEqual("Vahid", result.Data.First().Name);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void Should_create_new_responseResultObject_using_IEnumerableT_auto_effectedCount()
    {
        List<SampleModel> obj = [new SampleModel { Id = 1, Name = "Vahid" }, new SampleModel { Id = 2, Name = "John" }];

        var result = new ResponseResult<SampleModel>(obj);

        Assert.AreEqual(typeof(List<SampleModel>), result.Data.GetType());
        Assert.AreEqual(2, result.EffectedRowsCount);
        Assert.AreEqual("Vahid", result.Data.First().Name);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void Should_create_new_responseResultObject_using_null()
    {
        var result = new ResponseResult<SampleModel>((IEnumerable<SampleModel>)null);

        Assert.AreEqual(typeof(List<SampleModel>), result.Data.GetType());
        Assert.AreEqual(0, result.EffectedRowsCount);
        Assert.IsNull(result.Data.FirstOrDefault());
        Assert.IsEmpty(result.Data);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }

    [TestMethod()]
    public void Should_handle_empty_collection()
    {
        List<SampleModel> emptyList = [];

        var result = new ResponseResult<SampleModel>(emptyList);

        Assert.AreEqual(typeof(List<SampleModel>), result.Data.GetType());
        Assert.AreEqual(0, result.EffectedRowsCount);
        Assert.IsEmpty(result.Data);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
    }
}