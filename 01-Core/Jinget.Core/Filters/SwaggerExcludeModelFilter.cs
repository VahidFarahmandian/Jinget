namespace Jinget.Core.Filters;

public class SwaggerExcludeTypeFilter : IDocumentFilter
{
    /// <summary>
    /// When <seealso cref="SwaggerExcludePropertyFilter"/> applied on a model, 
    /// then that model will be ignored while working with swagger
    /// </summary>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var assembly in
            AppDomain.CurrentDomain.GetAssemblies()
            .Where(ass => ass.GetTypes()
                             .Any(type => type
                                            .GetCustomAttributes(typeof(SwaggerExcludeAttribute), true)
                                            .Length > 0)))
        {
            // Get all models that are decorated with SwaggerExcludeAttribute
            // This will only work for models that are under current Assembly
            var excludedTypes = GetTypesWithExcludeAttribute(assembly);
            // Loop through them
            foreach (var _type in excludedTypes)
            {
                // Check if that type exists in SchemaRepository
                if (context.SchemaRepository.TryLookupByType(_type, out _))
                {
                    // If the type exists in SchemaRepository, check if name exists in the dictionary
                    if (swaggerDoc.Components.Schemas.ContainsKey(_type.Name))
                    {
                        // Remove the schema
                        swaggerDoc.Components.Schemas.Remove(_type.Name);
                    }
                }
            }
        }
    }

    // Get all types in assembly that contains SwaggerExcludeAttribute
    public static IEnumerable<Type> GetTypesWithExcludeAttribute(Assembly assembly) => assembly.GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(SwaggerExcludeAttribute), true).Length > 0);
}
