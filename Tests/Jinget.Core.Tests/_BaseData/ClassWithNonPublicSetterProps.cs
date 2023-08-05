namespace Jinget.Core.Tests._BaseData
{
    public class ClassWithNonPublicSetterProps
    {
        public ClassWithNonPublicSetterProps(string name)
        {
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; private set; }
    }
}
