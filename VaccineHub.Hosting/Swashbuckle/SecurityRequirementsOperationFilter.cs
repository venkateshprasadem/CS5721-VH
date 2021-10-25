using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VaccineHub.Hosting.Swashbuckle
{
    [UsedImplicitly]
    public sealed class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!HasAuthorizeAttribute(context.MethodInfo))
            {
                return;
            }

            operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Basic"},
                            Scheme = "basic"
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }

        private static bool HasAuthorizeAttribute(MemberInfo memberInfo)
        {
            var attribute = memberInfo.GetCustomAttribute<AuthorizeAttribute>()
                            ?? memberInfo.DeclaringType?.GetCustomAttribute<AuthorizeAttribute>();

            return attribute != null;
        }
    }
}
