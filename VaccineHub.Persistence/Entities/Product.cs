using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaccineHub.Persistence.Types;

namespace VaccineHub.Persistence.Entities {
    
    [Table("product")]
    public class Product {

        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("cost")]
        public decimal? Cost { get; set; }

        [Column("currency")]
        public Currency? Currency { get; set; }

        [Column("doses")]
        public int? Doses { get; set; }

        [Column("maxIntervalInDays")]
        public int? MaxIntervalInDays { get; set; }

        [Column("minIntervalInDays")]
        public int? MinIntervalInDays { get; set; }
    }
}