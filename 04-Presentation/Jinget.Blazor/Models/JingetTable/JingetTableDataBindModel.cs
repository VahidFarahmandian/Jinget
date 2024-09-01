using Jinget.Blazor.Enums;
using Jinget.Core.Enumerations;

namespace Jinget.Blazor.Models.JingetTable;

public record JingetTableDataBindModel(
    string SearchTerm,
    int PageIndex,
    int PageSize,
    string SortColumn,
    OrderByDirection SortDirection = OrderByDirection.Ascending,
    JingetTableEventType EventType = JingetTableEventType.None);

