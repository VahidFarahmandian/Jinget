namespace Jinget.Blazor.Models;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Model">model's current state</param>
/// <param name="PropertyName">Property which its value is changed</param>
/// <param name="PreviousValue">Property's previous value</param>
/// <param name="CurrentValue">Property's current value</param>
/// <param name="ConvertedValue">Property's current value after conversion</param>
public record ObjectChangedModel<T>(T? Model, string PropertyName, object? PreviousValue, object? CurrentValue, object? ConvertedValue);
