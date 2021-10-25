using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace VaccineHub.Web.Filters
{
    [UsedImplicitly]
    internal sealed class ValidationFilterAttribute : ActionFilterAttribute
    {
        private static readonly Regex GetPropertyNameRegex = new Regex("^Required property '(.*?)'");

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;

            if (modelState.IsValid)
            {
                return;
            }

            UpdateSerializationErrors(modelState);

            context.Result = new BadRequestObjectResult(modelState);
        }

        private static void UpdateSerializationErrors(ModelStateDictionary modelState)
        {
            if (!modelState.TryGetValue(string.Empty, out var value))
            {
                return;
            }

            var errors = value.Errors;

            for (var i = errors.Count - 1; i >= 0; i--)
            {
                var error = errors[i];

                if (error.Exception is not JsonSerializationException)
                {
                    continue;
                }

                var match = GetPropertyNameRegex.Match(error.Exception.Message);

                if (!match.Success)
                {
                    continue;
                }

                errors.RemoveAt(i);
                        
                var propertyName = ToTitleCase(match.Groups[1].Value);

                modelState.AddModelError(propertyName, $"'{propertyName}' must not be empty.");
            }

            if (!errors.Any())
            {
                modelState.Remove(string.Empty);
            }

            modelState.Remove(string.Empty);
        }

        private static string ToTitleCase(string value)
        {
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }
    }
}