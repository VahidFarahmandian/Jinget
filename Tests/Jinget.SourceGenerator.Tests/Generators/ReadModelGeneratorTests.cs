using Jinget.SourceGenerator.Common.Extensions;
using Jinget.SourceGenerator.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jinget.SourceGenerator.Tests.Generators
{
    [TestClass]
    public class ReadModelGeneratorTests
    {
        private Compilation? _compilation;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _compilation = CreateCompilation();
            ICollection<string> s = [];
        }

        [TestMethod]
        public void should_generate_non_jinget_readonly_models()
        {
            var classes = _compilation?.GetTypeByMetadataName("Jinget.SourceGenerator.Tests._BaseTypes.Models.StudentModel");

            var result = ReadModelGenerator.GenerateReadModelSources(_compilation!, [classes!]);

            Assert.AreEqual(1, result.Length);

            var generatedCode = result[0].Code;

            var tree = CSharpSyntaxTree.ParseText(generatedCode);

            if (tree.GetRoot() is not CompilationUnitSyntax root)
            {
                Assert.Fail();
            }

            Assert.AreEqual(1, tree.GetClassNames().Count());
            Assert.AreEqual($"ReadOnlyStudentModel", tree.GetClassNames().FirstOrDefault());

            Assert.IsNotNull(tree.GetCompilationUnitRoot().Usings.FirstOrDefault(x => x.ToString() == "using Jinget.Core.Contracts;"));

            var classDeclaration = tree.GetClasses().First();
            if (classDeclaration.BaseList != null && classDeclaration.BaseList.Types.Any())
            {
                Assert.IsTrue(classDeclaration.BaseList.Types.Where(x => x.ToString() == "Jinget.SourceGenerator.Tests._BaseTypes.Models.BaseEntity<int>").Any());
            }
            else
                Assert.Fail();

            Assert.IsNotNull(classDeclaration.BaseList.Types.FirstOrDefault(x => x.ToString() == "Jinget.Core.Contracts.IAggregateRoot"));

            Assert.IsNotNull(classDeclaration.BaseList.Types.Where(x => x.ToString() == "Jinget.Core.Contracts.BaseEntity").Any());

            Assert.AreEqual("private", ((PropertyDeclarationSyntax)classDeclaration.Members.First()).AccessorList?.Accessors.Where(x => x.Keyword.ToString() == "set").First().Modifiers.First().Value);

            Assert.AreEqual(10, tree.GetPropertyNames().Count());
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