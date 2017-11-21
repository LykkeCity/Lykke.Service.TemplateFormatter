using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Service.TemplateFormatter.Web.Settings;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Lykke.WebExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Lykke.Service.TemplateFormatter.Web
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = env;
        }

        public string Title => "Template Formatting Service";
        public string Version => typeof(Startup).GetTypeInfo().Assembly.GetName().Version.ToString();

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Setup basic logging
            ILog log = new LogToConsole();
            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterWebExtensions<AppSettings, IAppSettings>(services, Configuration, "SettingsUrl", log);

                builder.Register(ctx =>
                    {
                        var settings = ctx.Resolve<IReloadingManager<IAppSettings>>();

                        return AzureTableStorage<PartnerTemplateSettings>.Create(
                            settings.ConnectionString(x => x.TemplateFormatterSettings.Partners.ConnectionString),
                            settings.CurrentValue.TemplateFormatterSettings.Partners.TableName, ctx.Resolve<ILog>());
                    })
                    .As<INoSQLTableStorage<PartnerTemplateSettings>>()
                    .SingleInstance();

                // Add framework services.
                services.AddMvc().AddWebExtensions();

                // Add swagger generator
                services.AddSwaggerGen(x => { x.SwaggerDoc("v1", new Info { Title = Title, Version = Version }); });

                builder.Populate(services);
                ApplicationContainer = builder.Build();

                // Create the IServiceProvider based on the container.
                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                log.WriteFatalErrorAsync(nameof(Startup), nameof(TemplateFormatter), nameof(ConfigureServices), ex, DateTime.UtcNow);
                throw;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebExtensions();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"{Title} {Version}");
            });
            app.UseStaticFiles();

            appLifetime.ApplicationStarted.Register(StartApplication);
            appLifetime.ApplicationStopping.Register(StopApplication);
            appLifetime.ApplicationStopped.Register(CleanUp);
        }

        private void StartApplication()
        {
            try
            {
                //Console.WriteLine("Starting...");
                //Console.WriteLine("Started");
            }
            catch (Exception ex)
            {
            }
        }

        private void StopApplication()
        {
            try
            {
                //Console.WriteLine("Stopping...");
                //Console.WriteLine("Stopped");
            }
            catch (Exception ex)
            {
            }
        }

        private void CleanUp()
        {
            try
            {
                Console.WriteLine("Cleaning up...");

                ApplicationContainer.Dispose();

                Console.WriteLine("Cleaned up");
            }
            catch (Exception ex)
            {
            }
        }
    }
}
