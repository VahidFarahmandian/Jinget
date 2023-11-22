namespace Jinget.Core.Tests._BaseData
{
    public class ClassWithNonPublicSetterProps(string name)
    {
        public int Id { get; set; }
        public string Name { get; private set; } = name;
    }
}
