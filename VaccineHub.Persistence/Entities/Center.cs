using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaccineHub.Persistence.Entities 
{

    [Table("center")]
    public class Center : IAuditEntity
    {
        [Key]
        [Column("id")]
        // Hospital Code 
        public string Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("eir_code")]
        [MinLength(7)]
        [MaxLength(7)]
        public string EirCode { get; set; }
 
        [Required]
        [Column("telephone")]
        public string Telephone { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("created_at")]
        public DateTime UpdatedAt { get; set; }
    }
}