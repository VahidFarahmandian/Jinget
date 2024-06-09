namespace Jinget.Blazor.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class JingetTableElement : JingetFormElement { }

public class JingetTableMember : JingetFormElement
{
    public bool Sortable { get; set; }
}