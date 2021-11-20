using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using VaccineHub.Persistence.Types;

namespace VaccineHub.Persistence.Entities
{
    public sealed class PaymentInformation : IAuditEntity
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [UsedImplicitly]
        public Guid Id { get; set; }

        [Column("credit_card_type")]
        public CreditCardType CreditCardType { get; set; }

        [Column("card_holder_first_name")]
        public string CardHolderFirstName { get; set; }

        [Column("card_holder_last_name")]
        public string CardHolderLastName { get; set; }

        [Column("card_number")]
        public string CardNumber { get; set; }

        [Column("expiry_month")]
        public int ExpiryMonth { get; set; }

        [Column("expiry_year")]
        public int ExpiryYear { get; set; }

        [Column("cvv")]
        public string Cvv { get; set; }

        [Column("address_line1")]
        public string AddressLine1 { get; set; }

        [Column("address_line2")]
        public string AddressLine2 { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("province_state")]
        public string ProvinceState { get; set; }

        [Column("country_code")]
        public string CountryCode { get; set; }

        [Column("postal_code")]
        public string PostalCode { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}