using System;
using System.Text.RegularExpressions;
using FluentValidation;
using VaccineHub.Web.Models;

namespace VaccineHub.Web.Endpoints.Booking.Validators
{
    internal sealed class PaymentInformationValidator : AbstractValidator<PaymentInformation>
    {
        private static readonly Regex TwoAlphaRegex = new("^[A-Z]{2}$", RegexOptions.Compiled);
        private static readonly Regex TwoOrThreeAlphaRegex = new("^[A-Z]{2,3}$", RegexOptions.Compiled);

        public PaymentInformationValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(i => i.AddressLine1)
                .NotEmpty();

            RuleFor(i => i.CardHolderFirstName)
                .NotEmpty();

            RuleFor(i => i.CardHolderLastName)
                .NotEmpty();

            RuleFor(i => i.CardNumber)
                .NotEmpty();

            RuleFor(i => i.City)
                .NotEmpty();

            RuleFor(i => i.CountryCode)
                .NotNull()
                .Must(TwoAlphaRegex.IsMatch)
                .WithMessage("Must be in 'XY' format.");

            RuleFor(i => i.Cvv)
                .NotEmpty()
                .Length(3, 4)
                .WithMessage("Must be 3 or 4 digits long.")
                .Must(i => int.TryParse(i, out _));

            RuleFor(i => i.ExpiryMonth)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(12);

            RuleFor(i => i.PostalCode)
                .NotEmpty();

            RuleFor(i => i.ProvinceState)
                .Must(TwoOrThreeAlphaRegex.IsMatch)
                .When(i => i.ProvinceState != null)
                .WithMessage("Must be in 'XY' or 'XYZ' format.");

            RuleFor(i => i.ExpiryYear)
                .GreaterThanOrEqualTo(DateTime.Now.Year % 100)
                .LessThan(100);

            RuleFor(i => i.CreditCardType)
                .NotNull();
        }
    }
}