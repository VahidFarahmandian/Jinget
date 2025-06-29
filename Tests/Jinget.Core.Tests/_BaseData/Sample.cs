namespace Jinget.Core.Tests._BaseData;

public class SampleInterfaceClass : ISampleInterface
{

}
public class Type1
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
}
public class Type2
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
}
public class TypeParent
{
}
public class TypeChild : TypeParent
{
}
public static class TestClassExtensions
{
    public static string[] Method1<T>(this TestClass testClass) => [typeof(T).Name];
    public static string[] Method1<T, U>(this TestClass testClass) => [typeof(T).Name, typeof(U).Name];
}
public class PrivateClass
{
    private PrivateClass()
    {
        
    }
    public int Property1 { get; set; }
}
public class TestClass
{
    public static int Method1(int a) => a;
    public static int Method1(int a, int b) => a + b;
    public string GetInfo<T>(string s1, int i1) => $"string is: {s1}, integer is: {i1}, generic type is: {typeof(T).Name}";

    public class InnerClass
    {
        public int InnerProperty1 { get; set; }
        public string InnerProperty2 { get; set; }

        public PublicParentType Parent_1 { get; set; }
        public ICollection<PublicParentType> Parents_1 { get; set; }

    }

    public int Property1 { get; set; }
    public string Property2 { get; set; }
    public string Property3 { get; set; }
    public bool Property4 { get; set; }

    public InnerClass InnerSingularProperty { get; set; }

    public ICollection<InnerClass> InnerProperty { get; set; }
    public List<InnerClass> InnerListProperty { get; set; }
}

public class PublicParentType
{
    public int Id { get; set; }
    public SubType Sub { get; set; }
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
public class XmlSample
{
    public int Id { get; set; }
    public string Name { get; set; }

    public InnerXmlSample InnerSample { get; set; }
    public List<InnerXmlSample> InnerSampleList { get; set; }

    public class InnerXmlSample
    {
        public string Data { get; set; }
    }
}

[Serializable]
public class SoapSample
{
    private int id;
    private string name;

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
}
