using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; init; }
    }

    public abstract class Entity<TKey> : IEntity<TKey> 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public TKey Id { get; init; }
    }

    public abstract class MutableEntity<TKey> : Entity<TKey>
    {
        public DateTimeOffset CreatedOn { get; init; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;
        //public virtual IUser CreatedBy { get; set; }
        //public virtual IUser ModifiedBy { get; set; }
    }
}
