using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaccineHub.Persistence.Types;

namespace VaccineHub.Persistence.Entities
{
    [Table("api_user")]
    public class ApiUser : IAuditEntity
    {
        [Key]
        [Column("email_id")]
        public string EmailId { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Required]
        [Column("user_type")]
        public UserType UserType { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}