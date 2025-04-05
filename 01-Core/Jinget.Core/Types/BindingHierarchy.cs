namespace Jinget.Core.Types;

/// <summary>
/// 
/// </summary>
/// <param name="PropertyName"></param>
/// <param name="InitiatorPropertyType">by creating new object of this type we can access <paramref name="PropertyName"/> property</param>
/// <param name="ParentProperty"><paramref name="ParentProperty"/> exists inside this type. if <paramref name="ParentProperty"/> is in root of <paramref name="InitiatorPropertyType"/>, then this argument is optional</param>
public record BindingHierarchy(string PropertyName, Type InitiatorPropertyType, BindingHierarchy? ParentProperty = null);
