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
        public void Should_generate_non_jinget_readonly_mapping_configurations()
        {
            //Arrange
            var classes = _compilation?.GetTypeByMetadataName("Jinget.SourceGenerator.Tests._BaseTypes.MappingConfigurations.StudentMappingConfiguration");

            // Act
            var result = ReadModelMappingConfigurationGenerator.GenerateReadModelMappingCode(_compilation!, [classes]);

            // Assert
            Assert.HasCount(1, result);

            var generatedCode = result[0].Code;

            var tree = CSharpSyntaxTree.ParseText(generatedCode, cancellationToken: TestContext.CancellationToken);
            var root = tree.GetRoot() as CompilationUnitSyntax;

            if (root == null)
            {
                Assert.Fail();
            }

            Assert.HasCount(1, tree.GetClassNames());
            Assert.AreEqual($"ReadOnlyStudentMappingConfiguration", tree.GetClassNames().FirstOrDefault());

            var classDeclaration = tree.GetClasses().First();
            if (classDeclaration.BaseList != null && classDeclaration.BaseList.Types.Any())
            {
                Assert.AreEqual("IEntityTypeConfiguration<Jinget.SourceGenerator.Tests._BaseTypes.Models.ReadOnlyStudentModel>", classDeclaration.BaseList.Types.First().Type.ToString());
            }
            else
                Assert.Fail();

            Assert.HasCount(1, tree.GetMethodNames());
            Assert.AreEqual("Configure", tree.GetMethodNames().FirstOrDefault());
        }
        private static CSharpCompilation CreateCompilation()
        {
            // Get the source root from MSBuild (works in .NET Core 3.1+)
            var sourceRoot = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..")); // Goes up from bin/Debug/netX.Y to project root

            var modelsPath = Path.Combine(sourceRoot, "_BaseTypes/MappingConfigurations/StudentMappingConfiguration.cs");

            var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(modelsPath));
            var references = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .Select(a => MetadataReference.CreateFromFile(a.Location))
                            .Cast<MetadataReference>();

            return CSharpCompilation.Create("TestAssembly", [syntaxTree], references);
        }

        public TestContext TestContext { get; set; }
    }
}