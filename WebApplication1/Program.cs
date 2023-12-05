using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Reflection;
using com.vorba.sand.services2.CosmosDb;
using com.vorba.sand.services2;
using Azure.Identity;
using com.vorba.sand.services2.ServiceBus;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var envName = builder.Environment.EnvironmentName;
            var webRootPath = builder.Environment.WebRootPath;
            var settings = builder.Configuration
                .AddJsonFile(Path.Combine(webRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(webRootPath, $"appsettings.{envName}.json"), optional: true, reloadOnChange: false)
                .AddAzureKeyVault(
                    //new Uri("https://vorba-sand-kv-2.vault.azure.net/"),
                    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
                    new DefaultAzureCredential())
                .AddEnvironmentVariables()
                .Build();

            //builder.Configuration.AddAzureAppConfiguration(options =>
            //{
            //    options.Connect(
            //        builder.Configuration["ConnectionStrings:AppConfig"])
            //            .ConfigureKeyVault(kv =>
            //            {
            //                kv.SetCredential(new DefaultAzureCredential());
            //            });
            //});

            //ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            //configBuilder.AddAzureKeyVault(new Uri(""), new DefaultAzureCredential());

            builder.Services
                .AddOptions<CosmosDbDemoServiceOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(CosmosDbDemoServiceOptions)).Bind(settings);
                });
            builder.Services
                .AddOptions<ServiceBusOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(ServiceBusOptions)).Bind(settings);
                });

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
            });

            builder.Services.AddTransient<ICosmosDbService, CosmosDbService>();
            builder.Services.AddTransient<IQueueServices, QueueServices>();

            var app = builder.Build();

            string swaggerUiRoutePrefix = string.Empty;

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                swaggerUiRoutePrefix = "swagger";
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = swaggerUiRoutePrefix; // use string.Empty to serve from root path
                options.InjectStylesheet("/assets/css/swagger-ui-dark.css");
            });
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
