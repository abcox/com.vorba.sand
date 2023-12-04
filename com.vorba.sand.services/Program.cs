using com.vorba.sand.services.CosmosDb.Demo;
using com.vorba.sand.services.Interfaces;
using com.vorba.sand.services.ServiceBus;
using com.vorba.sand.services.SignalR;

namespace com.vorba.sand.services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var services = builder.Services;
            
            //var settings = builder.Configuration
            //.AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
            //.AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            //    .AddEnvironmentVariables()
            //        .Build();


            builder.Services.AddOptions<ServiceBusOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(ServiceBusOptions)).Bind(settings);
                    configuration.GetSection(nameof(CosmosDbDemoServiceOptions)).Bind(settings);
                });

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options => {
                options.AddPolicy("CorsForDev", builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                );
            });

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddTransient<IMessageHub, MessageHub>();
            services.AddTransient<IQueueServices, QueueServices>();
            //services.AddTransient<IObjectDataService, CosmosDbDemoService>();
            services.AddHostedService<QueueConsumerService>();

            var app = builder.Build();
                        
            app.UseRouting();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseCors("CorsForDev");
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MessageHub>("/messages");
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}