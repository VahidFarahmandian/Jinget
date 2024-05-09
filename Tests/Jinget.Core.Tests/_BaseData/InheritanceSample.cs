using Jinget.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Jinget.Core.Tests._BaseData;

[Summary("Non Generic Parent")]
public class NonGenericParent
{
    [Summary("Sample Method 1")]
    public void SampleMethod1() { }

    [Authorize]
    [Summary("Sample Method 2")]
    public void SampleMethod2() { }

    [Authorize]
    public void SampleMethod3() { }
}
public class GenericParent<T> { }
public class MultiGenericParent<T, U, R> { }

public class GenericChildNonGenericParent<T> : NonGenericParent { }
public class GenericChildGenericParent<T> : GenericParent<T> { }
public class GenericChildMultiGenericParent<T, U, R> : MultiGenericParent<T, U, R> { }


public class NonGenericChildNonGenericParent : NonGenericParent { }
public class NonGenericChildGenericParent : GenericParent<NonGenericParent> { }
public class NonGenericChildMultiGenericParent : MultiGenericParent<Type, Type, Type> { }
