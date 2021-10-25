using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaccineHub.Persistence.Entities
{
    public interface IAuditEntity
    {
        [Column("created_at")]
        DateTime CreatedAt { get; set; }
    }
}