namespace Jinget.Core.Utilities.Expressions.BindingHierarchyUtility;

public class BindingHierarchyApi
{
    public static RootDefinition<TSource> Define<TSource>(params BindingDefinition[] properties) => new(properties);

    public static PropertyBuilder<T> Property<T>(string propertyName) => new(propertyName);

    public class RootDefinition<TSource>
    {
        private readonly List<BindingHierarchy> _bindings = new();

        public RootDefinition(params BindingDefinition[] properties)
        {
            foreach (var prop in properties)
            {
                _bindings.AddRange(prop.GetBindings());
            }
        }

        public Expression<Func<TSource, TSource>> Compile() => BindingHierarchyUtility.CreateBindingExpression<TSource>(_bindings);
    }

    public class BindingDefinition
    {
        private readonly List<BindingHierarchy> _bindings = [];

        public BindingDefinition(params BindingDefinition[] properties)
        {
            foreach (var prop in properties)
            {
                _bindings.AddRange(prop.GetBindings());
            }
        }

        public BindingDefinition(BindingHierarchy binding) => _bindings.Add(binding);

        internal List<BindingHierarchy> GetBindings() => _bindings;
    }

    public class PropertyBuilder<T>(string propertyName)
    {
        private BindingDefinition _parent;

        public PropertyBuilder<T> WithParent(BindingDefinition parent)
        {
            _parent = parent;
            return this;
        }

        public BindingDefinition Build()
        {
            BindingHierarchy parentBinding = null;
            if (_parent != null)
            {
                var parentBindings = _parent.GetBindings();
                if (parentBindings.Count != 1)
                {
                    throw new InvalidOperationException("Parent definition must resolve to exactly one binding");
                }
                parentBinding = parentBindings[0];
            }

            var binding = new BindingHierarchy(propertyName, typeof(T), parentBinding);
            return new BindingDefinition(binding);
        }

        public static implicit operator BindingDefinition(PropertyBuilder<T> builder) => builder.Build();
    }
}
