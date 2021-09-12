using System;

namespace Jinget.Core.Tests._BaseData
{
    public class NonGenericParent { }
    public class GenericParent<T> { }
    public class MultiGenericParent<T, U, R> { }

    public class GenericChildNonGenericParent<T> : NonGenericParent { }
    public class GenericChildGenericParent<T> : GenericParent<T> { }
    public class GenericChildMultiGenericParent<T, U, R> : MultiGenericParent<T, U, R> { }


    public class NonGenericChildNonGenericParent : NonGenericParent { }
    public class NonGenericChildGenericParent : GenericParent<NonGenericParent> { }
    public class NonGenericChildMultiGenericParent : MultiGenericParent<Type, Type, Type> { }
}
