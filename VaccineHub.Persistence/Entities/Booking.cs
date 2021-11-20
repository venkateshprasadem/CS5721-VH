using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using VaccineHub.Persistence.Types;

namespace VaccineHub.Persistence.Entities
{
    [Table("booking")]
    public class Booking : IAuditEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [UsedImplicitly]
        public Guid Id { get; set; }

        [Required]
        [Column("booking_type")]
        public BookingType? BookingType { get; set; }

        [Required]
        [Column("dosage_type")]
        public DosageType? DosageType { get; set; }

        [Required]
        [Column("appointment_date")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public virtual ApiUser ApiUser { get; set; }

        [Required]
        public virtual Product Product { get; set; }

        [Required]
        public virtual Center Center { get; set; }

        [Required]
        public virtual PaymentInformation PaymentInformation { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}