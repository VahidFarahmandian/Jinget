using System.ComponentModel.DataAnnotations;

namespace Jinget.SourceGenerator.Tests._BaseTypes.Models;

public abstract class BaseEntity<T>
{
    [Key]
    public T Id { get; protected set; }

    public void SetId(T id) => Id = id;
}