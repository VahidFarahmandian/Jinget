using Jinget.SourceGenerator.Common.Extensions;
using Jinget.SourceGenerator.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jinget.SourceGenerator.Tests.Generators
{
    [TestClass]
    public class ReadModelMappingConfigurationGeneratorTests
    {
        private Compilation? _compilation;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _compilation = CreateCompilation();
        }

        [TestMethod]
        public void should_generate_non_jinget_readonly_mapping_configurations()
        {
            //Arrange
            var classes = _compilation?.GetTypeByMetadataName("Jinget.SourceGenerator.Tests._BaseTypes.MappingConfigurations.StudentMappingConfiguration");

            // Act
            var result = ReadModelMappingConfigurationGenerator.GenerateReadModelMappingCode(_compilation!, [classes]);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);

            var generatedCode = result[0].Code;

            var tree = CSharpSyntaxTree.ParseText(generatedCode);
            var root = tree.GetRoot() as CompilationUnitSyntax;

            if (root == null)
            {
                Assert.Fail();
            }

            Assert.AreEqual(1, tree.GetClassNames().Count());
            Assert.AreEqual($"ReadOnlyStudentMappingConfiguration", tree.GetClassNames().FirstOrDefault());

            var classDeclaration = tree.GetClasses().First();
            if (classDeclaration.BaseList != null && classDeclaration.BaseList.Types.Any())
            {
                Assert.AreEqual("IEntityTypeConfiguration<Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyStudentModel>", classDeclaration.BaseList.Types.First().Type.ToString());
            }
            else
                Assert.Fail();

            Assert.AreEqual(1, tree.GetMethodNames().Count());
            Assert.AreEqual("Configure", tree.GetMethodNames().FirstOrDefault());
        }
        private CSharpCompilation CreateCompilation()
        {
            // Get the source root from MSBuild (works in .NET Core 3.1+)
            var sourceRoot = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..")); // Goes up from bin/Debug/netX.Y to project root

            var modelsPath = Path.Combine(sourceRoot, "_BaseTypes\\MappingConfigurations\\StudentMappingConfiguration.cs");

            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(modelsPath));
            var references = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .Select(a => MetadataReference.CreateFromFile(a.Location))
                            .Cast<MetadataReference>();

            return CSharpCompilation.Create("TestAssembly", [syntaxTree], references);
        }
    }
}