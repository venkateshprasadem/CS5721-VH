using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using VaccineHub.Persistence;
using VaccineHub.Web.Endpoints.VaccineHubDbContextExtensions;

namespace VaccineHub.Web.Endpoints.Product.Validators
{
    [UsedImplicitly]
    public class ProductValidator : AbstractValidator<Models.Product>
    {
        public ProductValidator(IVaccineHubDbContext dbContext,
            [NotNull] IHttpContextAccessor httpContextAccessor)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty()
                .WithMessage("Id cannot be null or empty");

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("Name cannot be null or empty");

            RuleFor(i => i.Doses)
                .NotNull()
                .NotEmpty()
                .WithMessage("Doses cannot be null or empty")
                .GreaterThanOrEqualTo(1)
                .WithMessage("Doses must be equal to or greater than 1")
                .LessThanOrEqualTo(2)
                .WithMessage("Doses must be equal to or less than 2");

            When(i => i.Doses is > 1, () =>
            {
                RuleFor(i => i.MinIntervalInDays)
                    .NotNull()
                    .NotEmpty()
                    .GreaterThan(0)
                    .WithMessage("MinIntervalInDays cannot be null / empty / 0 for doses > 1");

                RuleFor(i => i.MaxIntervalInDays)
                    .NotNull()
                    .NotEmpty()
                    .GreaterThan(0)
                    .WithMessage("MaxIntervalInDays cannot be null / empty / 0 for doses > 1");

                RuleFor(i => i.MaxIntervalInDays)
                    .GreaterThanOrEqualTo(x => x.MinIntervalInDays)
                    .WithMessage("MaxIntervalInDays must be greater than or equal to MinIntervalInDays");
            });

            //validate for adding products
            When(_ =>
                httpContextAccessor.HttpContext != null &&
                httpContextAccessor.HttpContext.Request.Method.Equals(HttpMethods.Post), () =>
            {
                RuleFor(x => x.Id)
                    .MustAsync(dbContext.IsValidProductId)
                    .WithMessage("ProductId already present");
            });

            //validate for updating products
            When(_ =>
                httpContextAccessor.HttpContext != null &&
                httpContextAccessor.HttpContext.Request.Method.Equals(HttpMethods.Put), () =>
            {
                RuleFor(x => x.Id)
                    .MustAsync(dbContext.IsProductIdPresent)
                    .WithMessage(
                        "One of the following actions failed : Specified product does not exist or Id change not allowed");
            });
        }
    }
}