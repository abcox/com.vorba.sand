using AutoFixture;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

[assembly: FunctionsStartup(typeof(com.vorba.sand.method.Startup))]
namespace com.vorba.sand.method
{
    public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = GetOpenApiDocVersion(),
            Title = $"{GetOpenApiDocTitle()} Demo by Adam Cox (Vorba Corp)",
            Description = GetOpenApiDocDescription(),
            TermsOfService = new Uri("https://github.com/abcox"),
            Contact = new OpenApiContact()
            {
                Name = "Vorba Support",
                Email = "adam.cox@vorba.com",
                Url = new Uri("https://github.com/abcox"),
            },
            License = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        };

        public override OpenApiVersionType OpenApiVersion { get; set; } = GetOpenApiVersion();

        private static string GetOpenApiDocDescription() => "";
    }

    public class OpenApiHttpTriggerAuthorization : DefaultOpenApiHttpTriggerAuthorization
    {
        public override async Task<OpenApiAuthorizationResult> AuthorizeAsync(IHttpRequestDataObject req)
        {
            var result = default(OpenApiAuthorizationResult);

            /* // ⬇️⬇️⬇️ This is a sample custom authorisation logic ⬇️⬇️⬇️
            var authtoken = (string)req.Headers["Authorization"];
            if (authtoken.IsNullOrWhiteSpace())
            {
                result = new OpenApiAuthorizationResult()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ContentType = "text/plain",
                    Payload = "Unauthorized",
                };
                return await Task.FromResult(result).ConfigureAwait(false);
            }
            if (authtoken.StartsWith("Bearer", ignoreCase: true, CultureInfo.InvariantCulture) == false)
            {
                result = new OpenApiAuthorizationResult()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ContentType = "text/plain",
                    Payload = "Invalid auth format",
                };
                return await Task.FromResult(result).ConfigureAwait(false);
            }
            var token = authtoken.Split(' ').Last();
            if (token != "secret")
            {
                result = new OpenApiAuthorizationResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    ContentType = "text/plain",
                    Payload = "Invalid auth token",
                };
                return await Task.FromResult(result).ConfigureAwait(false);
            }
            // ⬆️⬆️⬆️ This is a sample custom authorisation logic ⬆️⬆️⬆️ */

            return await Task.FromResult(result).ConfigureAwait(false);
        }
    }

    public class OpenApiCustomUIOptions : DefaultOpenApiCustomUIOptions
    {
        public OpenApiCustomUIOptions(Assembly assembly)
            : base(assembly)
        {
        }

        // Declare if you want to change the custom CSS file name.
        public override string CustomStylesheetPath { get; } = "dist.swagger-ui-dark.css";

        // Declare if you want to change the custom JavaScript file name.
        public override string CustomJavaScriptPath { get; } = "dist.my-custom.js";

        // Declare if you want to change the behaviours of handling the custom CSS file.
        //public override async Task<string> GetStylesheetAsync()
        //{
        //    // DO SOMETHING BEFORE CALLING THE BASE METHOD
        //
        //    base.GetStylesheetAsync();
        //
        //    // DO SOMETHING AFTER CALLING THE BASE METHOD
        //}

        public override Task<string> GetStylesheetAsync()
        {
            return base.GetStylesheetAsync();
        }

        // Declare if you want to change the behaviours of handling the custom JavaScript file.
        //public override async Task<string> GetJavaScriptAsync()
        //{
        //    // DO SOMETHING BEFORE CALLING THE BASE METHOD
        //
        //    base.GetJavaScriptAsync();
        //
        //    // DO SOMETHING AFTER CALLING THE BASE METHOD
        //}

        public override Task<string> GetJavaScriptAsync()
        {
            return base.GetJavaScriptAsync();
        }

        //<!-- Uncomment if you want to use the embedded file. -->
        //public override string CustomStylesheetPath { get; set; } = "dist.my-custom.css";
        //public override string CustomJavaScriptPath { get; set; } = "dist.my-custom.js";
        //<!-- Uncomment if you want to use the embedded file. -->

        //<!-- Uncomment if you want to use the external URL. -->
        //public override string CustomStylesheetPath { get; set; } = "https://raw.githubusercontent.com/Azure/azure-functions-openapi-extension/main/samples/Microsoft.Azure.WebJobs.Extensions.OpenApi.FunctionApp.InProc/dist/my-custom.css";
        //public override string CustomJavaScriptPath { get; set; } = "https://raw.githubusercontent.com/Azure/azure-functions-openapi-extension/main/samples/Microsoft.Azure.WebJobs.Extensions.OpenApi.FunctionApp.InProc/dist/my-custom.js";
        //<!-- Uncomment if you want to use the external URL. -->
    }

    public class Startup : FunctionsStartup
    {
        private IConfiguration configuration;

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

            builder.Services//.AddSingleton(new Fixture())
                            .AddSingleton<IOpenApiConfigurationOptions>(_ =>
                            {
                                //var options = new OpenApiConfigurationOptions()
                                //{
                                //    Info = new OpenApiInfo()
                                //    {
                                //        Version = DefaultOpenApiConfigurationOptions.GetOpenApiDocVersion(),
                                //        Title = $"{DefaultOpenApiConfigurationOptions.GetOpenApiDocTitle()} (Injected)",
                                //        Description = DefaultOpenApiConfigurationOptions.GetOpenApiDocDescription(),
                                //        TermsOfService = new Uri("https://github.com/Azure/azure-functions-openapi-extension"),
                                //        Contact = new OpenApiContact()
                                //        {
                                //            Name = "Enquiry",
                                //            Email = "azfunc-openapi@microsoft.com",
                                //            Url = new Uri("https://github.com/Azure/azure-functions-openapi-extension/issues"),
                                //        },
                                //        License = new OpenApiLicense()
                                //        {
                                //            Name = "MIT",
                                //            Url = new Uri("http://opensource.org/licenses/MIT"),
                                //        }
                                //    },
                                //    Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                                //    OpenApiVersion = DefaultOpenApiConfigurationOptions.GetOpenApiVersion(),
                                //    IncludeRequestingHostName = DefaultOpenApiConfigurationOptions.IsFunctionsRuntimeEnvironmentDevelopment(),
                                //    ForceHttps = DefaultOpenApiConfigurationOptions.IsHttpsForced(),
                                //    ForceHttp = DefaultOpenApiConfigurationOptions.IsHttpForced(),
                                //};

                                var options = new OpenApiConfigurationOptions();

                                return options;
                            })
                            //.AddSingleton<IOpenApiHttpTriggerAuthorization>(_ =>
                            //{
                            //    var auth = new OpenApiHttpTriggerAuthorization(req =>
                            //    {
                            //        var result = default(OpenApiAuthorizationResult);
                            //
                            //        // ⬇️⬇️⬇️ Add your custom authorisation logic ⬇️⬇️⬇️
                            //        //
                            //        // CUSTOM AUTHORISATION LOGIC
                            //        //
                            //        // ⬆️⬆️⬆️ Add your custom authorisation logic ⬆️⬆️⬆️
                            //
                            //        return Task.FromResult(result);
                            //    });
                            //
                            //    return auth;
                            //})
                            .AddSingleton<IOpenApiCustomUIOptions>(_ =>
                            {
                                var assembly = Assembly.GetExecutingAssembly();
                                //var options = new OpenApiCustomUIOptions(assembly)
                                //{
                                //    GetStylesheet = () =>
                                //    {
                                //        var result = string.Empty;
                                //
                                //        // ⬇️⬇️⬇️ Add your logic to get your custom stylesheet ⬇️⬇️⬇️
                                //        //
                                //        // CUSTOM LOGIC TO GET STYLESHEET
                                //        //
                                //        // ⬆️⬆️⬆️ Add your logic to get your custom stylesheet ⬆️⬆️⬆️
                                //
                                //        return Task.FromResult(result);
                                //    },
                                //    GetJavaScript = () =>
                                //    {
                                //        var result = string.Empty;
                                //
                                //        // ⬇️⬇️⬇️ Add your logic to get your custom JavaScript ⬇️⬇️⬇️
                                //        //
                                //        // CUSTOM LOGIC TO GET JAVASCRIPT
                                //        //
                                //        // ⬆️⬆️⬆️ Add your logic to get your custom JavaScript ⬆️⬆️⬆️
                                //
                                //        return Task.FromResult(result);
                                //    }
                                //};
                                var options = new OpenApiCustomUIOptions(assembly);

                                return options;
                            });
            //builder.Services.AddDbContext<BloggingContext>(options =>
            //{
            //    options.UseSqlite(configuration.GetConnectionString(""));
            //});
        }
    }
}
