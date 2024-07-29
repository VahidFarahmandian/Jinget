namespace Jinget.Blazor.Models;

public class BaseTypeModel<TCode>
{
    public virtual TCode? Code { get; set; }

    public virtual string? Title { get; set; }
}
public class BaseTypeModel : BaseTypeModel<byte> { }

public class BaseTypeTreeModel<TCode> : BaseTypeModel<TCode>
{
    public virtual TCode? ParentCode { get; set; }
}
public class BaseTypeTreeModel : BaseTypeTreeModel<int> { }