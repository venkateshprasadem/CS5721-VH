using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using VaccineHub.Persistence;
using VaccineHub.Web.Services.Users.Models;

namespace VaccineHub.Web.Endpoints.Configuration.Validators
{
    [UsedImplicitly]
    public class ApiUserValidator : AbstractValidator<ApiUser>
    {
        public ApiUserValidator(IVaccineHubDbContext dbContext,
            [NotNull] IHttpContextAccessor httpContextAccessor)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.EmailId)
                .NotNull()
                .NotEmpty()
                .WithMessage("EmailId cannot be null or empty");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Password cannot be null or empty");

            //validate for adding users
            When(_ =>
                httpContextAccessor.HttpContext != null &&
                httpContextAccessor.HttpContext.Request.Method.Equals(HttpMethods.Post), () =>
            {
                RuleFor(x => x.EmailId)
                    .MustAsync(dbContext.IsValidEmailId)
                    .WithMessage("EmailId already present");
            });

            //validate for updating users
            When(_ =>
                httpContextAccessor.HttpContext != null &&
                httpContextAccessor.HttpContext.Request.Method.Equals(HttpMethods.Put), () =>
            {
                RuleFor(x => x.EmailId)
                    .MustAsync(dbContext.IsEmailIdPresent)
                    .WithMessage(
                        "One of the following actions failed : Specified user does not exist or EmailId change not allowed");
            });
        }
    }
}