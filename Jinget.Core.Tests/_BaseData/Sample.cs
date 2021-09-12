using System.Collections.Generic;

namespace Jinget.Core.Tests._BaseData
{
    public class TestClass
    {
        public class InnerClass
        {
            public int InnerProperty1 { get; set; }
            public string InnerProperty2 { get; set; }

        }

        public int Property1 { get; set; }
        public string Property2 { get; set; }

        public InnerClass InnerSingularProperty { get; set; }

        public ICollection<InnerClass> InnerProperty { get; set; }
    }

    public class ParentType
    {
        private ParentType() { }

        public int Id { get; protected set; }
        public SubType Sub { get; protected set; }
    }

    public class SubType
    {
        private SubType() { }

        public int Id { get; protected set; }
        public ICollection<ColSubType> ColSubs { get; protected set; }
    }

    public class ColSubType
    {
        private ColSubType() { }

        public int Id { get; protected set; }
        public SubType SubType { get; set; }
    }
}
