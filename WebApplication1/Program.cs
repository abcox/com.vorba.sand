using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Reflection;
using com.vorba.sand.services2.CosmosDb;
using com.vorba.sand.services2;

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
                .AddEnvironmentVariables()
                    .Build();

            builder.Services.AddOptions<CosmosDbDemoServiceOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(CosmosDbDemoServiceOptions)).Bind(settings);
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
