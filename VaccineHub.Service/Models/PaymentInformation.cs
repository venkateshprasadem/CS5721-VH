using System;

namespace VaccineHub.Service.Models
{
    public sealed class PaymentInformation
    {
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

        private bool Equals(PaymentInformation other)
        {
            return CreditCardType == other.CreditCardType &&
                   CardHolderFirstName == other.CardHolderFirstName &&
                   CardHolderLastName == other.CardHolderLastName &&
                   CardNumber == other.CardNumber &&
                   ExpiryMonth == other.ExpiryMonth &&
                   ExpiryYear == other.ExpiryYear &&
                   Cvv == other.Cvv &&
                   AddressLine1 == other.AddressLine1 &&
                   AddressLine2 == other.AddressLine2 &&
                   City == other.City &&
                   ProvinceState == other.ProvinceState &&
                   CountryCode == other.CountryCode &&
                   PostalCode == other.PostalCode;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this,
                       obj) ||
                   obj is PaymentInformation other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add((int) CreditCardType);
            hashCode.Add(CardHolderFirstName);
            hashCode.Add(CardHolderLastName);
            hashCode.Add(CardNumber);
            hashCode.Add(ExpiryMonth);
            hashCode.Add(ExpiryYear);
            hashCode.Add(Cvv);
            hashCode.Add(AddressLine1);
            hashCode.Add(AddressLine2);
            hashCode.Add(City);
            hashCode.Add(ProvinceState);
            hashCode.Add(CountryCode);
            hashCode.Add(PostalCode);
            return hashCode.ToHashCode();
        }
    }
}