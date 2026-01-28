using Microsoft.OpenApi;

namespace Jinget.Core.Filters;

public class SwaggerExcludePropertyFilter : ISchemaFilter
{
    /// <summary>
    /// When <seealso cref="SwaggerExcludePropertyFilter"/> applied on a proprty in model, 
    /// then that property will be ignored while working with swagger
    /// </summary>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties.Count == 0)
            return;

        var excludedList = context.Type.GetProperties()
            .Where(t => t.GetCustomAttribute<SwaggerExcludeAttribute>() != null)
            .Select(m => m.Name.ToCamelCase());

        foreach (var excludedName in excludedList)
        {
            if (schema.Properties.ContainsKey(excludedName))
                schema.Properties.Remove(excludedName);
        }
    }
}
