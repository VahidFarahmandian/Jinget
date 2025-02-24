namespace Jinget.Core.CodeDom;

public sealed class JingetDynamicCode
{
    private class Compiler
    {
        /// <param name="errors">Compilation might be ended with some errors. The compile time errors are stored in this output parameter</param>
        /// <param name="jingetSource">During the dynamic code execution process, given sourceCode might change. Changed version of the sourceCode are stored in this output parameter</param>
        /// <param name="isTopLevelStatement">C# 9.0 enables you to write top level statements.</param>
        /// <param name="references">In order to compile the given sourceCode, some external references might needed to be added. Required references are being passed using this parameter</param>
        internal static byte[]? Compile(string sourceCode, MethodOptions? args, out List<string> errors, out string jingetSource,
            bool isTopLevelStatement = true, List<string>? references = null)
        {
            references ??= [];
            jingetSource = string.Empty;
            errors = [];

            sourceCode = sourceCode.Trim();
            sourceCode = sourceCode.StartsWith(Environment.NewLine)
                ? sourceCode.TrimStart(Environment.NewLine.ToCharArray())
                : sourceCode;
            sourceCode = sourceCode.Trim();

            if (isTopLevelStatement)
                sourceCode = GenerateSourceCode(sourceCode, args);
            jingetSource = sourceCode;

            using var peStream = new MemoryStream();
            var result = GenerateAssembly(sourceCode, references).Emit(peStream);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                errors.AddRange(failures.Select(diagnostic => $"{diagnostic.Id} {diagnostic.GetMessage()}"));

                return null;
            }

            peStream.Seek(0, SeekOrigin.Begin);

            return peStream.ToArray();
        }

        /// <summary>
        /// Generaetes new dynamic dll, based on the given source code. this dll created on the fly
        /// </summary>
        static CSharpCompilation GenerateAssembly(string sourceCode, List<string> externalReferences)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            externalReferences ??= [];

            var defaultReferences = new[] { "System.Private.CoreLib", "netstandard", "System.Runtime" };

            externalReferences.AddRange(defaultReferences.Select(x => Assembly.Load(x).Location));

            var references = new List<MetadataReference>();
            references.AddRange(externalReferences.Distinct().Select(x => MetadataReference.CreateFromFile(x)));

            return CSharpCompilation.Create($"{Guid.NewGuid()}.dll",
                [parsedSyntaxTree],
                references: references,
                options: new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }

        /// <summary>
        /// Generate source code using given expression. This method tries to put the given expression inside a method in a class
        /// so that it could be invoked
        /// </summary>
        static string GenerateSourceCode(string expression, MethodOptions? methodOptions)
        {
            CodeNamespace globalCodeNamespace = new();
            //add default using: using System;
            globalCodeNamespace.Imports.Add(new CodeNamespaceImport(nameof(System)));

            if (expression.StartsWith("using "))
            {
                string[] statements = expression.Split(";", StringSplitOptions.RemoveEmptyEntries);
                foreach (var @using in statements.Where(x => x.StartsWith("using ")))
                {
                    string[] usingSections = @using.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    //if len>2 then using statement does not terminated with semicolon
                    if (@usingSections.Length <= 2)
                    {
                        globalCodeNamespace.Imports.Add(new CodeNamespaceImport(usingSections[1]));
                        expression = expression.Replace(@using, "");
                    }
                }
            }

            CodeNamespace myNamespace = new("JingetDynamic");
            CodeTypeDeclaration classDeclaration = new()
            {
                IsClass = true,
                Name = "DynamicInvoker",
                TypeAttributes = TypeAttributes.NotPublic | TypeAttributes.Sealed

            };

            CodeMemberMethod myMethod = new()
            {
                Name = "DynamicInvoke",
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = methodOptions?.ReturnType is null ? new CodeTypeReference(typeof(void)) : new CodeTypeReference(methodOptions.ReturnType)
            };
            if (methodOptions?.Parameters != null)
            {
                myMethod.Parameters.AddRange(new CodeParameterDeclarationExpressionCollection(methodOptions
                    .Parameters.Select(x => new CodeParameterDeclarationExpression(x.Type, x.Name)).ToArray()));
            }

            expression = expression.Trim(';').Trim();

            myMethod.Statements.Add(new CodeSnippetExpression(expression));
            classDeclaration.Members.Add(myMethod);

            myNamespace.Types.Add(classDeclaration);

            CodeCompileUnit codeCompileUnit = new();
            codeCompileUnit.Namespaces.Add(globalCodeNamespace);
            codeCompileUnit.Namespaces.Add(myNamespace);

            var source = new StringBuilder();
            using (var sw = new StringWriter(source))
            {
                ICodeGenerator generator = new CSharpCodeProvider().CreateGenerator(sw);
                CodeGeneratorOptions codeOpts = new();
                generator.GenerateCodeFromCompileUnit(codeCompileUnit, sw, codeOpts);
                sw.Flush();
                sw.Close();
            }

            //remove auto generated comments
            var marker = "//------------------------------------------------------------------------------";
            var allTheCode = source.ToString();
            var justTheRealCode = allTheCode.Substring(allTheCode.IndexOf(marker) + marker.Length, allTheCode.LastIndexOf(marker) + marker.Length);

            string removedComments = allTheCode[justTheRealCode.Length..].Trim();

            var removedNewLine = removedComments.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x));
            source = new StringBuilder(string.Join(Environment.NewLine, removedNewLine));
            return source.ToString();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static object? Execute(string sourceCode, out List<string> errors, out string compiledSourceCode, MethodOptions? options = null, bool isTopLevelStatement = true, bool compileOnly = false, List<string>? references = null)
    {
        if (sourceCode.Length > 10000)
        {
            throw new ArgumentException("Jinget says: sourceCode is too long. sourceCode max length is 10000 characters");
        }

        var compiledCode = Compiler.Compile(sourceCode, options, out errors, out compiledSourceCode,
            isTopLevelStatement, references);
        if (compiledCode is null || compileOnly)
        {
            return null;
        }

        var @class = Assembly.Load(compiledCode).GetTypes().FirstOrDefault(x => x.Name == "DynamicInvoker");
        var method = @class?.GetMethod("DynamicInvoke");

        var instance = Activator.CreateInstance(@class ?? throw new InvalidOperationException("Jinget Says: Invalid Operation"));

        return method?.Invoke(instance, options?.Parameters.Select(x => x.Value).ToArray());
    }
    public class MethodOptions
    {
        /// <summary>
        /// What should be return type of the dynamically injected Invoke method?
        /// If not set, void will be used
        /// </summary>
        /// <remarks>
        /// Default value: typeof(void)
        /// </remarks>
        public Type ReturnType { get; set; } = typeof(void);

        /// <summary>
        /// What are the input parameters for dynamically injected Invoke method?
        /// </summary>
        public List<ParameterOptions> Parameters { get; set; } = [];

        public class ParameterOptions
        {
            public Type? Type { get; set; }
            public string? Name { get; set; }
            public object? Value { get; set; }
        }
    }
}