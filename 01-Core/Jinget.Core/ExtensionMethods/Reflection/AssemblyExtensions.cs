using System.Text.RegularExpressions;
using Jinget.Core.Attributes;
using Jinget.Core.Utilities;

namespace Jinget.Core.ExtensionMethods.Reflection;

public static class AssemblyExtensions
{
    public class AssemblyInfo
    {
        public string Summary { get; set; } = "";
        public string TypeName { get; set; } = "";
        public string AssemblyName { get; set; } = "";
        public string MethodName { get; set; } = "";
        public string Claim { get; set; } = "";
        public string ParentTitle { get; set; } = "";
    }

    /// <summary>
    /// Get all types which has <paramref name="resourceType"/> as their parent in the given assembly <paramref name="assembly"/>
    /// This Method is mainly used for gathering Controllers in a WebAPI or MVC project
    /// </summary>
    /// <param name="methodSummaryAttribute">If types have any <seealso cref="SummaryAttribute"/> then its description will also parsed</param>
    public static List<AssemblyInfo> GetTypes(this Assembly assembly, Type resourceType, Type? methodSummaryAttribute = null, string normalizingPattern = @"Controller$")
    {
        methodSummaryAttribute ??= typeof(SummaryAttribute);

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        return assembly.GetTypes()
            .Where(resourceType.IsAssignableFrom)
            .Select(x => new AssemblyInfo
            {
                Summary = ((SummaryAttribute)x.GetCustomAttributes().FirstOrDefault(a => a.GetType() == methodSummaryAttribute))?.Description.Trim(),
                TypeName = Regex.Replace(x.Name, normalizingPattern, string.Empty, RegexOptions.IgnoreCase), //x.Name.Replace("Controller", string.Empty).Trim(),
                AssemblyName = assembly.GetName().Name
            }).ToList();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    public static List<Type> GetTypes(this Assembly assembly, Expression<Func<Type, bool>>? filter = null) => assembly.GetTypes()
            .Where(c => c.IsClass && !c.IsGenericTypeDefinition && !c.IsAbstract && !c.IsNested)
            .Where(filter?.Compile() ?? BooleanUtility.TrueCondition<object>().Compile()).ToList();

    /// <summary>
    /// Get all methods of th egiven resource type inside an assembly
    /// </summary>
    /// <param name="onlyAuthorizedMethods">If true, then only methods with <seealso cref="AuthorizeAttribute"/> will be returned</param>
    public static List<AssemblyInfo> GetMethods(this Assembly executingAssembly, Type resourceType, Type? methodSummaryAttribute = null, bool onlyAuthorizedMethods = true)
    {
        methodSummaryAttribute ??= typeof(SummaryAttribute);

        var allMethods = executingAssembly.GetTypes()
            .Where(resourceType.IsAssignableFrom)
            .SelectMany(type =>
                type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            .Where(m => !m.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any());

#pragma warning disable CS8604 // Possible null reference argument.
        var authorizedMethods = allMethods.Where(m => !onlyAuthorizedMethods ||
            (//methods marked as Authorize
            m.GetCustomAttributes().Any(x => x.GetType() == typeof(AuthorizeAttribute)) ||
            //class marked as Authorize
            m.DeclaringType.GetCustomAttributes().Any(x => x.GetType() == typeof(AuthorizeAttribute))));
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        return

            [
                .. authorizedMethods.Select(x => new AssemblyInfo
                {
                    Summary = ((SummaryAttribute)x.GetCustomAttributes()
                            .FirstOrDefault(a => a.GetType() == methodSummaryAttribute))?.Description.Trim(),
                    TypeName = x.DeclaringType?.Name.Replace("Controller", string.Empty).Trim(),
                    AssemblyName = executingAssembly.GetName().Name,
                    MethodName = x.Name.Trim(),
                    Claim = ((ClaimAttribute)x.GetCustomAttributes()
                            .FirstOrDefault(a => a.GetType() == typeof(ClaimAttribute)))?.Title,
                    ParentTitle = ((SummaryAttribute)x.DeclaringType?.GetCustomAttributes()
                        .FirstOrDefault(a => a.GetType() == methodSummaryAttribute))?.Description
                })
                    .OrderBy(x => x.Summary).ThenBy(x => x.MethodName).ThenBy(x => x.Claim),
            ];
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8601 // Possible null reference assignment.
    }
}
