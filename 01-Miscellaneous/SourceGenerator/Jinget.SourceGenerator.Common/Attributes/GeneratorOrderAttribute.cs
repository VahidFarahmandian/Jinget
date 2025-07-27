using System;

namespace Jinget.SourceGenerator.Common.Attributes
{
    public class GeneratorOrderAttribute : Attribute
    {
        public int Order { get; set; }
    }
}
