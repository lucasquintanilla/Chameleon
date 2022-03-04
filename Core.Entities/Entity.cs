using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    }
}
