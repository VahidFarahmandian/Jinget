using Jinget.Core.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

public abstract class BaseEntity<T> : IEntity
{
    [Key]
    public T Id { get; protected set; }

    public void SetId(T id) => Id = id;
}

public abstract class TraceBaseEntity<TModel, TKey> : BaseEntity<TKey>
{
    public TModel Trace { get; set; }
}