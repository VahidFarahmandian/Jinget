using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Jinget.SourceGenerator.Tests")]
namespace Jinget.SourceGenerator.Generators;

[GeneratorOrder(Order = 3)]
public class WebAPIGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Define data source (find all entity classes)
        var entityDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (c, _) => GetSemanticTargetForGeneration(c))
            .Where(static data => data != null)
            .Collect();

        // 2. Transform data (generate code for each entity)
        var compilationProvider = context.CompilationProvider.Combine(entityDeclarations);
        var transformedData = compilationProvider.Select((data, _) => GenerateWebAPISource(data.Left, data.Right));

        // 3. Add generated code to compilation
        context.RegisterSourceOutput(transformedData,
            static (spc, source) =>
            {
                foreach (var (HintName, Code) in source)
                {
                    spc.AddSource($"{HintName}.g.cs", SourceText.From(Code, Encoding.UTF8));
                }
            });
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration && classDeclaration.HasAttribute("GenerateWebAPI");
    }

    private static INamedTypeSymbol GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var model = context.SemanticModel;
        var symbol = model.GetDeclaredSymbol(classDeclaration);
        return symbol!;
    }

    internal static ImmutableArray<(string HintName, string Code)> GenerateWebAPISource(
        Compilation compilation,
        ImmutableArray<INamedTypeSymbol> classes)
    {
        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        return [.. classes
            .Select(type =>
            {
                if(!type.HasAttribute("GenerateWebAPI"))
                    throw new InvalidOperationException($"Type {type.Name} does not have GenerateWebAPI attribute.");

                string webApiGroupName = type.Name.EndsWith("Model")
                ? type.Name.Substring(0, type.Name.Length - "Model".Length)
                : type.Name;

                string outputClassName = $"{webApiGroupName}ApiExtensions";

                var keyProperty = type.GetKeyProperty()
                ?? throw new InvalidOperationException($"Could not find the key property for {type.Name}. Key peroperty should have [Key] annotation");

                var (Handler, Model) = ConstructCommandHandler(compilation,type);
                var queryHandler = ConstructQueryHandler(compilation,type);
                var claims = ConstructClaims(compilation,type,webApiGroupName);
                var tenantAware = ConstructTenantAwareness(compilation,type);

                var classContent =
$@"using Microsoft.AspNetCore.Routing;
using Mapster;
using System.Linq;
using System.Threading;
using Jinget.Core.Types;
using Jinget.Core.Utilities.Expressions;
using Jinget.Core.Types;
namespace Jinget.SourceGenerator.WebApis;

public static class {outputClassName} 
{{
    public static RouteGroupBuilder Map{webApiGroupName}Apis(this RouteGroupBuilder group)
    {{
        group.MapPost(""/"", {claims.PostClaim}{tenantAware} async ({Handler} command, {queryHandler.Model} vm, CancellationToken cancellationToken) 
            => await command.SaveAsync(vm.Adapt<{Model}>(), cancellationToken: cancellationToken));

        group.MapPut(""/"", {claims.PutClaim}{tenantAware} async ({Handler} command, {queryHandler.Model} vm, CancellationToken cancellationToken) 
            => await command.SaveAsync(vm.Adapt<{Model}>(), cancellationToken: cancellationToken));

        group.MapDelete(""/{{id}}"", {claims.DeleteClaim}{tenantAware} async ({Handler} command, {keyProperty.Type} id, CancellationToken cancellationToken) 
            => await command.RemoveAsync(id, cancellationToken: cancellationToken));

        group.MapGet(""/{{id}}"", {claims.GetClaim}{tenantAware} async ({queryHandler.Handler} query, {keyProperty.Type} id, CancellationToken cancellationToken) 
            => await query.FetchAsync<{queryHandler.Model}>(id, cancellationToken: cancellationToken));

        group.MapGet(""/"", {claims.GetAllClaim}{tenantAware} async ({queryHandler.Handler} query, CancellationToken cancellationToken, int page=1, int pageSize=10, string sortColumn=""id"", string sortDirection=""asc"", string searchCriteria="""") 
            => await query.FetchAllAsync<{queryHandler.Model}>(
                new QueryOption<{queryHandler.Model}>
                {{
                    Filter = ExpressionUtility.CreateSearchAllColumnsExpression<{queryHandler.Model}>(searchCriteria),
                    PaginationOption = new QueryOption<{queryHandler.Model}>.PaginationOptionModel
                    {{
                        PageIndex = page,
                        PageSize = pageSize
                    }},
                    SortOption = new QueryOption<{queryHandler.Model}>.SortOptionModel
                    {{
                        SortColumn = sortColumn,
                        SortDirection = sortDirection
                    }}
                }}, cancellationToken: cancellationToken));

        group.MapPost(""/delete/list"", {claims.GetClaim}{tenantAware} async ({Handler} command, List<{keyProperty.Type}> ids, CancellationToken cancellationToken) 
            => new ResponseResult<List<{keyProperty.Type}>>(ids, (await command.RemoveRangeImmediateAsync(x => ids.Contains(x.Id), cancellationToken: cancellationToken)).EffectedRowsCount));

        return group;
    }}
}}";
                return (HintName: outputClassName, Code: classContent);
            })];
    }

    private static ClaimParts ConstructClaims(
        Compilation compilation, INamedTypeSymbol type, string webApiGroupName)
    {
        var hasClaim = type.GetAttributeNamedArgument<bool>(compilation, "GenerateWebAPI", "HasClaim", false);
        if (!hasClaim)
            return ("", "", "", "", "");
        var claimPrefix = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "ClaimPrefix", "") ?? "";
        var claimTitle = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "ClaimTitle", "") ?? "";
        var claimType = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "ClaimType", "") ?? "";

        claimType = claimType.EndsWith("Attribute") ? claimType.Substring(0, claimType.Length - "Attribute".Length) : claimType;

        var claimTypeMetadata = compilation.FindTypeInReferencedAssemblies($"{claimType}Attribute");

        claimTitle = string.IsNullOrWhiteSpace(claimTitle) ? webApiGroupName.ToLowerInvariant() : claimTitle.ToLowerInvariant();

        var postClaim = hasClaim ? $@"[{claimTypeMetadata}(Title = ""{claimPrefix}-{claimTitle}-create"")]" : string.Empty;
        var putClaim = hasClaim ? $@"[{claimTypeMetadata}(Title = ""{claimPrefix}-{claimTitle}-modify"")]" : string.Empty;
        var deleteClaim = hasClaim ? $@"[{claimTypeMetadata}(Title = ""{claimPrefix}-{claimTitle}-delete"")]" : string.Empty;
        var getClaim = hasClaim ? $@"[{claimTypeMetadata}(Title = ""{claimPrefix}-{claimTitle}-read"")]" : string.Empty;
        var getAllClaim = hasClaim ? $@"[{claimTypeMetadata}(Title = ""{claimPrefix}-{claimTitle}-read"")]" : string.Empty;

        return new ClaimParts(postClaim, putClaim, deleteClaim, getClaim, getAllClaim);
    }

    private static string ConstructTenantAwareness(Compilation compilation, INamedTypeSymbol type)
    {
        var isTenantAware = type.GetAttributeNamedArgument<bool>(compilation, "GenerateWebAPI", "TenantAware", false);
        if (!isTenantAware)
            return "";

        return "[Jinget.Domain.Core.Attributes.TenantAware]";
    }


    private static (string Handler, string Model) ConstructCommandHandler(Compilation compilation, INamedTypeSymbol type)
    {
        var handlerType = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "CommandHandler", "");
        if (string.IsNullOrWhiteSpace(handlerType))
            throw new InvalidOperationException($"CommandHandler not specified for {type.Name}.");

        var parts = GetSymbols(compilation, type, handlerType);

        string commandHandler;

        var model = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "Model", "");
        string modelFullName = string.IsNullOrWhiteSpace(model) ? $"{parts.Model}" : model;

        //custom interface without any generic type
        if (parts.Handler.IsGenericType == false)
        {
            commandHandler = parts.Handler.Name;
        }
        else
        {
            commandHandler = parts.Trace == null ?
                    $"{parts.Handler.ContainingNamespace}.{parts.Handler.Name}<{modelFullName}, {parts.Key.Type}>" :
                    $"{parts.Handler.ContainingNamespace}.{parts.Handler.Name}<{parts.Trace}, {parts.UserContext}, {modelFullName}, {parts.Key.Type}>";
        }

        return (commandHandler, modelFullName);
    }

    private static (string Handler, string Model) ConstructQueryHandler(Compilation compilation, INamedTypeSymbol type)
    {
        var handlerType = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "QueryHandler", "");
        if (string.IsNullOrWhiteSpace(handlerType))
            throw new InvalidOperationException($"QueryHandler not specified for {type.Name}.");

        var parts = GetSymbols(compilation, type, handlerType);

        var readOnlyModel = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "ReadOnlyModel", "");

        string queryHandler;

        string readOnlyModelFullName = string.IsNullOrWhiteSpace(readOnlyModel) ? $"{parts.Namespacename}.{parts.ReadOnlyModelName}" : readOnlyModel;

        //custom interface without any generic type
        if (parts.Handler.IsGenericType == false)
        {
            queryHandler = $"{parts.Handler.ContainingNamespace}.{parts.Handler.Name}";
        }
        else
        {
            queryHandler = parts.Trace == null ?
                $"{parts.Handler.ContainingNamespace}.{parts.Handler.Name}<{readOnlyModelFullName}, {parts.Key.Type}>" :
                $"{parts.Handler.ContainingNamespace}.{parts.Handler.Name}<{parts.Trace}, {parts.UserContext}, {readOnlyModelFullName}, {parts.Key.Type}>";
        }
        return (queryHandler, readOnlyModelFullName);
    }

    static HandlerParts GetSymbols(Compilation compilation, INamedTypeSymbol type, string handlerType)
    {
        var handlerSymbol = compilation.FindTypeInReferencedAssemblies(handlerType);

        var traceDataType = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "TraceDataType", "");
        var traceSymbol = compilation.FindTypeInReferencedAssemblies(traceDataType);

        var userContextType = type.GetAttributeNamedArgument<string>(compilation, "GenerateWebAPI", "UserContextType", "");
        var userContextSymbol = compilation.FindTypeInReferencedAssemblies(userContextType);

        var namespaceName = $"{type.ContainingNamespace}";
        var modelSymbol = compilation.GetTypeByMetadataName($"{namespaceName}.{type.Name}");
        var readOnlySymbolName = $"ReadOnly{type.Name}";

        var keySymbol = type.GetKeyProperty()
        ?? throw new InvalidOperationException($"Could not find the key property for {type.Name}.");

        return new HandlerParts(handlerSymbol!, traceSymbol!, userContextSymbol!, namespaceName, modelSymbol!, readOnlySymbolName, keySymbol);
    }
}

internal record struct HandlerParts(INamedTypeSymbol Handler, INamedTypeSymbol Trace, INamedTypeSymbol UserContext, string Namespacename, INamedTypeSymbol Model, string ReadOnlyModelName, IPropertySymbol Key)
{
    public static implicit operator (INamedTypeSymbol Handler, INamedTypeSymbol Trace, INamedTypeSymbol UserContext, string Namespacename, INamedTypeSymbol Model, string ReadOnlyModelName, IPropertySymbol Key)(HandlerParts value)
    {
        return (value.Handler, value.Trace, value.UserContext, value.Namespacename, value.Model, value.ReadOnlyModelName, value.Key);
    }

    public static implicit operator HandlerParts((INamedTypeSymbol Handler, INamedTypeSymbol Trace, INamedTypeSymbol UserContext, string Namespacename, INamedTypeSymbol Model, string ReadOnlyModelName, IPropertySymbol Key) value)
    {
        return new HandlerParts(value.Handler, value.Trace, value.UserContext, value.Namespacename, value.Model, value.ReadOnlyModelName, value.Key);
    }
}

internal record struct ClaimParts(string PostClaim, string PutClaim, string DeleteClaim, string GetClaim, string GetAllClaim)
{
    public static implicit operator (string PostClaim, string PutClaim, string DeleteClaim, string GetClaim, string GetAllClaim)(ClaimParts value)
    {
        return (value.PostClaim, value.PutClaim, value.DeleteClaim, value.GetClaim, value.GetAllClaim);
    }

    public static implicit operator ClaimParts((string PostClaim, string PutClaim, string DeleteClaim, string GetClaim, string GetAllClaim) value)
    {
        return new ClaimParts(value.PostClaim, value.PutClaim, value.DeleteClaim, value.GetClaim, value.GetAllClaim);
    }
}