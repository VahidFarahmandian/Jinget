using Jinget.SourceGenerator.Common.Extensions;
using Jinget.SourceGenerator.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jinget.SourceGenerator.Tests.Generators
{
    [TestClass]
    public class WebAPIGeneratorTests
    {
        private Compilation? _compilation;

        [TestInitialize]
        public void Setup()
        {
            //dummy
            //just are here to tell to compiler not to ignore their referenced assemblies
            var dummy = Jinget.Core.Utilities.StringUtility.GetEnglishDigits();

            // Arrange
            _compilation = CreateCompilation();

        }

        [TestMethod]
        public void should_generate_non_jinget_webapis()
        {
            //Arrange
            var classes = _compilation?.GetTypeByMetadataName("Jinget.SourceGenerator.Tests._BaseTypes.Models.StudentModel");

            // Act
            var result = WebAPIGenerator.GenerateWebAPISource(_compilation!, [classes!]);

            // Assert
            Assert.AreEqual(1, result.Length);

            var generatedCode = result[0].Code;

            var tree = CSharpSyntaxTree.ParseText(generatedCode);
            var root = tree.GetRoot() as CompilationUnitSyntax;

            if (root == null)
            {
                Assert.Fail();
            }

            Assert.AreEqual(1, tree.GetClassNames().Count());
            Assert.AreEqual($"StudentApiExtensions", tree.GetClassNames().FirstOrDefault());

            Assert.AreEqual(1, tree.GetMethodNames().Count());
            Assert.AreEqual("MapStudentApis", tree.GetMethodNames().FirstOrDefault());

            Assert.AreEqual(7, root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Body?.Statements.Count);

            Assert.IsTrue(root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Body?.Statements[0].ToString().StartsWith("group.MapPost(\"/\","));
            Assert.IsTrue(root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Body?.Statements[1].ToString().StartsWith("group.MapPut(\"/\","));
            Assert.IsTrue(root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Body?.Statements[2].ToString().StartsWith("group.MapDelete(\"/{id}\","));
            Assert.IsTrue(root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Body?.Statements[3].ToString().StartsWith("group.MapGet(\"/{id}\","));
            Assert.IsTrue(root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Body?.Statements[4].ToString().StartsWith("group.MapGet(\"/\","));
            Assert.IsTrue(root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault()?.Body?.Statements[5].ToString().StartsWith("group.MapPost(\"/delete/list\","));
        }

        private CSharpCompilation CreateCompilation()
        {
            // Get the source root from MSBuild (works in .NET Core 3.1+)
            var sourceRoot = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..")); // Goes up from bin/Debug/netX.Y to project root

            var modelsPath = Path.Combine(sourceRoot, "_BaseTypes/Models/StudentModel.cs");

            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(modelsPath));
            var references = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .Select(a => MetadataReference.CreateFromFile(a.Location))
                            .Cast<MetadataReference>();

            return CSharpCompilation.Create("TestAssembly", [syntaxTree], references);
        }
    }
}