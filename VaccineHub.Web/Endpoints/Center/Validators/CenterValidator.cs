using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using VaccineHub.Persistence;

namespace VaccineHub.Web.Endpoints.Center.Validators
{
    [UsedImplicitly]
    public class CenterValidator : AbstractValidator<Models.Center>
    {
        public CenterValidator(IVaccineHubDbContext dbContext,
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

            RuleFor(x => x.EirCode)
                .NotNull()
                .NotEmpty()
                .WithMessage("Name cannot be null or empty")
                .Length(7)
                .Matches(@"\A\S+\z");

            //validate for adding products
            When(_ =>
                httpContextAccessor.HttpContext != null &&
                httpContextAccessor.HttpContext.Request.Method.Equals(HttpMethods.Post), () =>
            {
                RuleFor(x => x.Id)
                    .MustAsync(dbContext.IsValidCenterId)
                    .WithMessage("ProductId already present");
            });

            //validate for updating products
            When(_ =>
                httpContextAccessor.HttpContext != null &&
                httpContextAccessor.HttpContext.Request.Method.Equals(HttpMethods.Put), () =>
            {
                RuleFor(x => x.Id)
                    .MustAsync(dbContext.IsCenterIdPresent)
                    .WithMessage(
                        "One of the following actions failed : Specified product does not exist or Id change not allowed");
            });
        }
    }
}