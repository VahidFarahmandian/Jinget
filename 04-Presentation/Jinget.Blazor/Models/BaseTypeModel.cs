namespace Jinget.Blazor.Models;

public class BaseTypeModel<TId>
{
    public virtual TId Id { get; set; }

    public virtual string? Title { get; set; }
}
public class BaseTypeModel : BaseTypeModel<byte> { }

public class BaseTypeTreeModel<TId> : BaseTypeModel<TId>
{
    public virtual TId? ParentId { get; set; }
}
public class BaseTypeTreeModel : BaseTypeTreeModel<int> { }