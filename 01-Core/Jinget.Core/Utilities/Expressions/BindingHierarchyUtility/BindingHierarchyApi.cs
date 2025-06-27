namespace Jinget.Core.Utilities.Expressions.BindingHierarchyUtility;

public static class BindingHierarchyApi
{
    public static RootDefinition<TSource> Define<TSource>(params BindingDefinition[] properties)
        => new(properties);

    public static RootDefinition<object> Define(Type sourceType, params BindingDefinition[] properties)
    {
        if (sourceType == null)
            throw new ArgumentNullException(nameof(sourceType));

        var method = typeof(BindingHierarchyApi)
            .GetMethod(nameof(Define), 1, new[] { typeof(BindingDefinition[]) })?
            .MakeGenericMethod(sourceType);

        return (RootDefinition<object>)method.Invoke(null, new object[] { properties });
    }

    public static PropertyBuilder<T> Property<T>(string propertyName) => new(propertyName);
    public static PropertyBuilder Property(string propertyName, Type type) => new(propertyName, type);

    public class RootDefinition<TSource>
    {
        private readonly List<BindingHierarchy> _bindings = [];

        public RootDefinition(params BindingDefinition[] properties)
            => _bindings.AddRange(properties.SelectMany(p => p.GetBindings()));

        public Expression<Func<TSource, TSource>> Compile()
            => BindingHierarchyUtility.CreateBindingExpression<TSource>(_bindings);
    }

    public class BindingDefinition
    {
        private readonly List<BindingHierarchy> _bindings = [];

        public BindingDefinition(params BindingDefinition[] properties)
            => _bindings.AddRange(properties.SelectMany(p => p.GetBindings()));

        public BindingDefinition(BindingHierarchy binding)
            => _bindings.Add(binding);

        internal List<BindingHierarchy> GetBindings() => _bindings;
    }

    public abstract class PropertyBuilderBase
    {
        protected BindingDefinition _parent;
        protected readonly string _propertyName;
        protected readonly Type _propertyType;

        protected PropertyBuilderBase(string propertyName, Type propertyType)
        {
            _propertyName = propertyName;
            _propertyType = propertyType;
        }

        public BindingDefinition Build()
        {
            var binding = new BindingHierarchy(_propertyName, _propertyType, GetParentBinding());
            return new BindingDefinition(binding);
        }

        protected BindingHierarchy GetParentBinding()
        {
            if (_parent == null) return null;

            var parentBindings = _parent.GetBindings();
            if (parentBindings.Count != 1)
                throw new InvalidOperationException("Parent definition must resolve to exactly one binding");

            return parentBindings[0];
        }
    }

    public class PropertyBuilder : PropertyBuilderBase
    {
        public PropertyBuilder(string propertyName, Type type) : base(propertyName, type) { }

        public PropertyBuilder WithParent(BindingDefinition parent)
        {
            _parent = parent;
            return this;
        }

        public static implicit operator BindingDefinition(PropertyBuilder builder)
            => builder.Build();
    }

    public class PropertyBuilder<T> : PropertyBuilderBase
    {
        public PropertyBuilder(string propertyName) : base(propertyName, typeof(T)) { }

        public PropertyBuilder<T> WithParent(BindingDefinition parent)
        {
            _parent = parent;
            return this;
        }

        public static implicit operator BindingDefinition(PropertyBuilder<T> builder)
            => builder.Build();
    }
}