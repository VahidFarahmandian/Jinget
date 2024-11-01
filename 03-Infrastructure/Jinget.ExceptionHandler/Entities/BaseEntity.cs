using System.ComponentModel.DataAnnotations;

namespace Jinget.ExceptionHandler.Entities;

public abstract class BaseEntity<TKeyType>
{
    protected BaseEntity() { }

    protected BaseEntity(TKeyType id) : this() => Id = id;

    private TKeyType _id;

    [Key]
    public virtual TKeyType Id
    {
        get => _id;
        protected set
        {
            if (value != null)
            {
                _id = value;
            }
        }
    }

    public virtual void SetId(TKeyType id) => Id = id;
}