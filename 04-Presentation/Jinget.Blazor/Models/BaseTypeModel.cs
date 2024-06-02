namespace Jinget.Blazor.Models;

public class BaseTypeModel<TCode>
{
    public virtual TCode? Code { get; set; }

    public virtual string? Title { get; set; }

    public virtual bool Selected { get; set; } = false;
}
public class BaseTypeModel : BaseTypeModel<byte>
{

}