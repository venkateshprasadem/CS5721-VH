using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using VaccineHub.Persistence;
using VaccineHub.Web.Endpoints.VaccineHubDbContextExtensions;

namespace VaccineHub.Web.Endpoints.Inventory.Validators
{
    [UsedImplicitly]
    public class InventoryValidator : AbstractValidator<Models.Inventory>
    {
        public InventoryValidator(IVaccineHubDbContext dbContext,
            [NotNull] IHttpContextAccessor httpContextAccessor)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.CenterId)
                .NotNull()
                .NotEmpty()
                .WithMessage("CenterId cannot be null or empty")
                .MustAsync(dbContext.IsCenterIdPresent)
                .WithMessage(
                    "One of the following actions failed : Specified center does not exist or Id change not allowed");

            RuleFor(x => x.ProductId)
                .NotNull()
                .NotEmpty()
                .WithMessage("ProductId cannot be null or empty")
                .MustAsync(dbContext.IsProductIdPresent)
                .WithMessage(
                    "One of the following actions failed : Specified product does not exist or Id change not allowed");

            RuleFor(x => x.Stock)
                .GreaterThan(0)
                .WithMessage("Stock must be greater than 0");
        }
    }
}