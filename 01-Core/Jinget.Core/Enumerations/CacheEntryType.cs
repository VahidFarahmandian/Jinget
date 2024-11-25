namespace Jinget.Core.Enumerations;

public enum CacheEntryType : byte
{
    [Description("specific")]
    SpecificItemWithId,

    [Description("firstorsingle")]
    FirstOrSingleItem,

    [Description("list")]
    ListItems
}
