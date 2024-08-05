namespace Jinget.Blazor.Models;

public class BaseTypeModel : BaseTypeModel<byte> { }
public class BaseTypeModel<TId>
{
    public virtual TId Id { get; set; }
    public virtual string? Title { get; set; }
}

public class BaseTypeTreeModel : BaseTypeTreeModel<int> { }
public class BaseTypeTreeModel<TId> : BaseTypeModel<TId> where TId : struct
{
    public virtual TId? ParentId { get; set; }
}
public class BaseTypeStringTreeModel : BaseTypeModel<string>
{
    public virtual string? ParentId { get; set; }
}