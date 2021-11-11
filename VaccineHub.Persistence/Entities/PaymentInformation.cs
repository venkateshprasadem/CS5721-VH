using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;
using VaccineHub.Persistence.Types;

namespace VaccineHub.Persistence.Entities
{
    public sealed class PaymentInformation
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [UsedImplicitly]
        public Guid Id { get; set; }
        public CreditCardType CreditCardType { get; set; }

        public string CardHolderFirstName { get; set; }

        public string CardHolderLastName { get; set; }

        public string CardNumber { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public string Cvv { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string ProvinceState { get; set; }

        public string CountryCode { get; set; }

        public string PostalCode { get; set; }
    }
}