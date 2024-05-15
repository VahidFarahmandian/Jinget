namespace Jinget.Blazor.Models;

public class BaseTypeModel<TCode>
{
    public virtual required TCode Code { get; set; }

    public virtual required string Title { get; set; }
}
public class BaseTypeModel : BaseTypeModel<byte>
{

}