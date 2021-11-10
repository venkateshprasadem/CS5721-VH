using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using VaccineHub.Hosting;
using VaccineHub.Hosting.Swashbuckle;
using VaccineHub.Persistence.DependencyInjection;
using VaccineHub.Web.Authentication;
using VaccineHub.Web.DependencyInjection;
using VaccineHub.Web.Filters;
using VaccineHub.Web.SeedData;

namespace VaccineHub.Web
{
    public class Startup
    {
        private const string ProjectName = "VaccineHub";

        private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromMinutes(5);
        private static IConfigurationRoot Configuration { get; set; }
        private static IWebHostEnvironment HostingEnvironment { get; set; }

        private static readonly IReadOnlyDictionary<string, string> DefaultConfiguration =
            new Dictionary<string, string>
            {
                ["Port"] = "5001",
                ["Database:Server"] = "memory"
            };
 
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            using var serviceProvider = services.BuildServiceProvider();

            Configuration = CreateConfigurationRoot(args);

            // configure fluent validation
            ValidatorOptions.Global.DisplayNameResolver = ResolveValidatorName;
            ValidatorOptions.Global.PropertyNameResolver = ResolveValidatorName;

            var host = new WebHostBuilder()
                .UseConfiguration(Configuration)
                .UseUrls($"http://*:{Configuration["port"]}")
                .UseStartup<Startup>()
                .UseKestrel()
                .UseTransport(KestrelTransport.Sockets)
                .UseDefaultServiceProvider((_, options) => options.ValidateScopes = true)
                .UseShutdownTimeout(ShutdownTimeout)
                .Build()
                .SeedData();

            host.Run();
        }

        private static string ResolveValidatorName(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.GetCustomAttribute<FromQueryAttribute>()?.Name
                   ?? memberInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName
                   ?? ToCamelCase(memberInfo.Name);
        }

        private static string ToCamelCase(string name)
        {
            return char.ToLowerInvariant(name[0]) + name[1..];
        }

        private static IConfigurationRoot CreateConfigurationRoot(string[] args)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(DefaultConfiguration)
                .AddEnvironmentVariables("VaccineHub_")
                .AddCommandLine(args)
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("VaccineHub")
                .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("VaccineHub", _ => { });

            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            services
                .RegisterServices()
                .AddResponseCompression()
                .AddDatabase(Configuration)
                .AddSwaggerGen(SetupSwaggerGen)
                .AddSwaggerGenNewtonsoftSupport()
                .AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly())
                .AddMvc(mvcOptions =>
                {
                    mvcOptions.Filters.Add<ValidationFilterAttribute>();
                    mvcOptions.EnableEndpointRouting = false;
                })
                .AddNewtonsoftJson(ConfigureJson)
                .AddFluentValidation(SetupFluentValidation);
        }

        private static void SetupSwaggerGen(SwaggerGenOptions options)
        {
            options
                .DefaultVaccineHubSwaggerOptions(ProjectName, "v1", HostingEnvironment.ContentRootPath,
                    null /* Add proper descriptions in all modules */)
            .AddSortKeySelector(x =>
            {
                return ((ControllerActionDescriptor) x.ActionDescriptor).ControllerName switch
                {
                    "Product" => "01",
                    "Center" => "02",
                    "Inventory" => "03",
                    "ApiUser" => "04",
                    "Health" => "05",
                    _ => throw new NotSupportedException("API order is undefined")
                };
            });
        }

        private static void ConfigureJson(MvcNewtonsoftJsonOptions options)
        {
            options.SerializerSettings.MaxDepth = 64;
        }

        private static void SetupFluentValidation(FluentValidationMvcConfiguration configuration)
        {
            configuration.RegisterValidatorsFromAssemblyContaining<Startup>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            HostingEnvironment = env;

            app
                .UseResponseCompression()
                .UseAuthentication()
                .UseMvc()
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(SetupSwaggerGui);
        }

        private static void SetupSwaggerGui(SwaggerUIOptions options)
        {
            options.ConfigObject.AdditionalItems.Add("syntaxHighlight", false); //Turns off syntax highlight which causing performance issues...
            options.ConfigObject.AdditionalItems.Add("theme", "agate"); //Reverts Swagger UI 2.x  theme which is simpler not much performance benefit...
            options.DefaultVaccineHubUi(ProjectName);
        }
    }
}