using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace VaccineHub.Persistence.Entities
{
    [Table("inventory")]
    public class Inventory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [UsedImplicitly]
        public string Id { get; set; }

        [Required]
        public virtual Product Product { get; set; }
 
        [Required]
        public virtual Center Center { get; set; }

        [Column("stock")]
        public int Stock { get; set; }
    }
}