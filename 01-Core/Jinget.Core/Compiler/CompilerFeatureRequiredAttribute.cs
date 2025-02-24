namespace Jinget.Core.Compiler;

/// <summary>
/// add support for 'required' keyword using latest lang version and .net standard 2.1
/// </summary>
#pragma warning disable CS9113 // Parameter is unread.
public class CompilerFeatureRequiredAttribute(string name) : Attribute
#pragma warning restore CS9113 // Parameter is unread.
{
}