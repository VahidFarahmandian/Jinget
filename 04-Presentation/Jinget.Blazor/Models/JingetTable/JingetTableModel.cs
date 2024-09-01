namespace Jinget.Blazor.Models.JingetTable;

public class JingetTableModel<T>(ICollection<T> items, int totalItems)
{
    public ICollection<T> Items { get; set; } = items;
    public int TotalItems { get; set; } = totalItems;
}
