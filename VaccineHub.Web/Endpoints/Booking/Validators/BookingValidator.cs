using System;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using VaccineHub.Persistence;
using VaccineHub.Web.Endpoints.VaccineHubDbContextExtensions;

namespace VaccineHub.Web.Endpoints.Booking.Validators
{
    [UsedImplicitly]
    public class BookingValidator : AbstractValidator<Models.Booking>
    {
        public BookingValidator(IVaccineHubDbContext dbContext,
            [NotNull] IHttpContextAccessor httpContextAccessor)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.BookingType)
                .NotNull()
                .NotEmpty()
                .WithMessage("BookingType cannot be null or empty");

            RuleFor(x => x.DosageType)
                .NotNull()
                .NotEmpty()
                .WithMessage("DosageType cannot be null or empty");

            RuleFor(x => x.AppointmentDate)
                .LessThanOrEqualTo(DateTime.Today.AddDays(7))
                .WithMessage("AppointmentDate must be within next 7 days")
                .GreaterThan(DateTime.Today)
                .WithMessage("AppointmentDate must be greater than today");

            RuleFor(x => x.CenterId)
                .MustAsync(dbContext.IsCenterIdPresent)
                .WithMessage("Specified center does not exist");

            RuleFor(x => x.ProductId)
                .MustAsync(dbContext.IsProductIdPresent)
                .WithMessage("Specified product does not exist");

            // Validate Payment Information for making or cancel booking
            RuleFor(x => x.PaymentInformation)
                .NotNull()
                .SetValidator(new PaymentInformationValidator());
        }
    }
}