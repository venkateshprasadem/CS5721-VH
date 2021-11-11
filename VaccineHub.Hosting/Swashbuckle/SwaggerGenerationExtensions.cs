using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace VaccineHub.Hosting.Swashbuckle
{
    public static class SwaggerGenerationExtensions
    {
        public static SwaggerGenOptions DefaultVaccineHubSwaggerOptions(
            this SwaggerGenOptions options,
            string projectName,
            string version,
            string contentRootPath,
            IEnumerable<string> projectXmlFileNames)
        {
            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = $"{projectName} | CS5721", 
                Version = version,
                Description = "Group Project by Team Taffeite (UL Student Ids - 21004528, 21017301, 21197091)"
            });

            options.ExampleFilters();

            if (projectXmlFileNames != null)
            {
                foreach (var projectXmlFileName in projectXmlFileNames)
                {
                    options.IncludeXmlComments(Path.Combine(contentRootPath, projectXmlFileName));
                }
            }

            options.OperationFilter<SecurityRequirementsOperationFilter>();

            options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Description = "HTTP authorization header using the basic scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            options.CustomSchemaIds(i => i.FullName);
            options.GeneratePolymorphicSchemas();
            return options;
        }

        public static void AddSortKeySelector(this SwaggerGenOptions options,
            Func<ApiDescription, string> sortKeySelector)
        {
            options.OrderActionsBy(sortKeySelector);
        }

        public static void DefaultVaccineHubUi(this SwaggerUIOptions options, string projectName)
        {
            options.DocumentTitle = projectName;

            options.SwaggerEndpoint("/swagger/v1/swagger.json", projectName);

            options.DocExpansion(DocExpansion.List);
            options.InjectStylesheet("./vaccineHub.css");
            options.InjectJavascript("./vaccineHub.js");
        }
    }
}
