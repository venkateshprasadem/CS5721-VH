using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace VaccineHub.Persistence.Entities
{
    [Table("inventory")]
    public class Inventory : IAuditEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [UsedImplicitly]
        public Guid Id { get; set; }

        [Required]
        public virtual Product Product { get; set; }
 
        [Required]
        public virtual Center Center { get; set; }

        [Column("stock")]
        public int Stock { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}