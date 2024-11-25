namespace Jinget.Core.Tests.Utilities;

[TestClass]
public class CachingUtilityTests
{
    [TestMethod]
    public void should_return_cache_key_for_specific_cache_type()
    {
        var expectedKey = "my-key_specific_1";
        var result = CachingUtility.GetKeyName(typeof(CacheSample), CacheEntryType.SpecificItemWithId, 1);
        Assert.AreEqual(expectedKey, result);
    }

    [TestMethod]
    public void should_return_cache_key_for_first_or_single_cache_type()
    {
        var expectedKey = "my-key_firstorsingle";
        var result = CachingUtility.GetKeyName(typeof(CacheSample), CacheEntryType.FirstOrSingleItem);
        Assert.AreEqual(expectedKey, result);
    }

    [TestMethod]
    public void should_return_cache_key_for_list_cache_type()
    {
        var expectedKey = "my-key_list";
        var result = CachingUtility.GetKeyName(typeof(CacheSample), CacheEntryType.ListItems);
        Assert.AreEqual(expectedKey, result);
    }
}