namespace Jinget.Core.Tests.Utilities;

[TestClass()]
public class ResponseResultUtilityTests
{
    [TestMethod()]
    public void should_map_source_data_type_to_destination_data_type()
    {
        var source = new ResponseResult<SampleModel>(
            [new SampleModel { Id = 1, Name = "Vahid" }, new SampleModel { Id = 2, Name = "Ali" }], 2);
        var result = ResponseResultUtility.MapTo<SampleModel, SampleViewModel>(source);

        Assert.AreEqual(result.Data.GetType(), typeof(List<SampleViewModel>));
        Assert.AreEqual(result.Data.First().Name, "Vahid");
        Assert.AreEqual(result.Data.Last().Name, "Ali");
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
        Assert.AreEqual(result.EffectedRowsCount, 2);
    }
}