﻿using System.ComponentModel.DataAnnotations;

namespace Jinget.ExceptionHandler.Entities;

public abstract class BaseEntity<TKeyType>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected BaseEntity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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