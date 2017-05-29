using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Logs;
using Lykke.Service.EmailFormatter.Settings;
using Lykke.SlackNotification.AzureQueue;
using Lykke.WebExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace Lykke.Service.EmailFormatter.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IContainer ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; }

        public string ApiVersion => "1.0";
        public string ApiTitle => "Email Formatting Service";

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Setup basic logging
            ILog log = new LogToConsole();
            try
            {
                // Add framework services.
                services.AddMvc();

                // Add swagger generator
                services.AddSwaggerGen(x => { x.SwaggerDoc(ApiVersion, new Info { Title = ApiTitle, Version = ApiVersion }); });

                // Load settings
                var settings = AppSettings.Load(Configuration["SettingsUrl"]);

                // Setup advanced logging
                var slackService = services.UseSlackNotificationsSenderViaAzureQueue(settings.SlackNotifications.AzureQueue, log);

                if (null != settings.EmailFormatterSettings.Log)
                {
                    var logToTable = new LykkeLogToAzureStorage(nameof(EmailFormatter), new AzureTableStorage<LogEntity>(
                        settings.EmailFormatterSettings.Log.ConnectionString, settings.EmailFormatterSettings.Log.TableName, log), slackService);

                    log = new LogAggregate()
                        .AddLogger(log)
                        .AddLogger(logToTable)
                        .CreateLogger();
                }

                // Register dependencies, populate the services from
                // the collection, and build the container. If you want
                // to dispose of the container at the end of the app,
                // be sure to keep a reference to it as a property or field.
                var builder = new ContainerBuilder();
                builder.RegisterInstance(log).As<ILog>().SingleInstance();

                // Register business services
                builder.RegisterInstance(
                        new AzureTableStorage<PartnerTemplateSettings>(settings.EmailFormatterSettings.Partners.ConnectionString,
                            settings.EmailFormatterSettings.Partners.TableName, log))
                    .As<INoSQLTableStorage<PartnerTemplateSettings>>()
                    .SingleInstance();

                // Create the IServiceProvider based on the container.
                builder.Populate(services);
                ApplicationContainer = builder.Build();
                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                log.WriteFatalErrorAsync(nameof(Startup), nameof(EmailFormatter), nameof(ConfigureServices), ex, DateTime.UtcNow);
                throw;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", $"{ApiTitle} {ApiVersion}");
            });
        }
    }
}
