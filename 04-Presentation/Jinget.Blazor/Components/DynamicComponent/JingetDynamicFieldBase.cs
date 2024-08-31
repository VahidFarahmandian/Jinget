using Jinget.Blazor.Attributes.DropDownList;
using Jinget.Core.ExtensionMethods.Reflection;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Jinget.Blazor.Components.DynamicComponent;

public class JingetDynamicFieldBase : ComponentBase
{
    [Inject]
    protected IServiceProvider? ServiceProvider { get; set; }

    [JsonProperty]
    public required object RefObject { get; set; }


    [JsonProperty]
    [Parameter] public string? Id { get; set; }

    [JsonProperty]
    [Parameter] public JingetFormElement Attribute { get; set; }

    [JsonProperty]
    [Parameter] public object? Value { get; set; }

    [JsonProperty]
    [Parameter] public PropertyInfo? Binding { get; set; }

    /// <summary>
    /// This event raised whenever a field's value changed
    /// </summary>
    [JsonIgnore]
    [Parameter] public EventCallback<object> ValueChanged { get; set; }

    /// <summary>
    /// This event raised whenever a field rendered on page
    /// </summary>
    [JsonIgnore]
    [Parameter] public EventCallback<JingetDynamicFieldBase> DynamicFieldAdded { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await OnDynamicFieldAddedAsync();
    }

    protected async Task OnDynamicFieldAddedAsync() => await DynamicFieldAdded.InvokeAsync(this);

    public async void OnChange(ChangeEventArgs e)
    {
        await OnDynamicFieldAddedAsync();
        await ValueChanged.InvokeAsync(e.Value);
    }

    object? InvokeBindingFunction(string? functionName, params object[] parameters)
    {
        if (functionName == null)
            return null;
        var method = Binding?.DeclaringType?.GetMethod(functionName);
        if (method != null)
        {
            object? callerObject = null;
            if (Binding?.DeclaringType != null)
            {
                callerObject = Binding.DeclaringType.GetDefaultConstructor() != null
                    ? Activator.CreateInstance(Binding.DeclaringType, null)
                    : throw new Exception($"Properties with {nameof(JingetDropDownListElement)} or {nameof(JingetDropDownListTreeElement)} attribute should have a parameterless constructor.");

                if (Binding.DeclaringType.BaseType != null)
                {
                    var propServiceProperty = Binding.DeclaringType.BaseType.GetProperty("ServiceProvider", BindingFlags.NonPublic | BindingFlags.Instance);
                    propServiceProperty?.SetValue(callerObject, ServiceProvider);
                }
            }

            object? data = null;

            if (callerObject != null)
            {
                data = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null
                    ? method.InvokeAsync(callerObject, parameters)
                    : method.Invoke(callerObject, parameters);
            }

            if (data != null)
                return data;
        }

        return null;
    }

    object GetData<TResult>() where TResult : JingetDropDownItemModelBase
    {
        //extract pre binding function and execute it
        var preBindingFunction = (Attribute as JingetDropDownListElementBase)?.PreBindingFunction;
        object? preBindingResult = null;
        if (!string.IsNullOrWhiteSpace(preBindingFunction))
        {
            preBindingResult = InvokeBindingFunction(preBindingFunction);
        }

        //extract binding function and execute it
        var bindingFunction = (Attribute as JingetDropDownListElementBase)?.BindingFunction;
        object? data = InvokeBindingFunction(bindingFunction, preBindingResult);

        data = data ?? new List<TResult>();


        //extract post binding function and execute it
        var postBindingFunction = (Attribute as JingetDropDownListElementBase)?.PostBindingFunction;
        if (!string.IsNullOrWhiteSpace(postBindingFunction))
        {
            InvokeBindingFunction(postBindingFunction, preBindingResult, data);
        }

        return data;
    }

    protected internal async Task<List<JingetDropDownItemModel>> GetDropDownListDataAsync() => await Task.FromResult((List<JingetDropDownItemModel>)GetData<JingetDropDownItemModel>());

    protected internal async Task<List<JingetDropDownTreeItemModel>> GetDropDownTreeDataAsync() => await Task.FromResult((List<JingetDropDownTreeItemModel>)GetData<JingetDropDownTreeItemModel>());
}
