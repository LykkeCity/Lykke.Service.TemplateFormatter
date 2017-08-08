using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.EmailFormatter.Web.Settings;
using Newtonsoft.Json;

namespace Lykke.Service.EmailFormatter.Console
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(IReadOnlyList<string> args)
        {
            if (args.Count < 2)
            {
                ShowHelp();
                return;
            }

            var settingsUrl = GetEnvironmentVariable("SettingsUrl", args[0]);
            var settings = await AppSettings.LoadAsync(settingsUrl);

            await Run(settings, args.Skip(1).ToArray());
        }

        private static async Task Run(AppSettings settings, IReadOnlyList<string> args)
        {
            switch (args[0])
            {
                case "upload":
                    await RunUplaod(settings, args.Skip(1).ToArray());
                    break;
                default:
                    System.Console.WriteLine($"Unknown command {args[0]}");
                    ShowHelp();
                    break;
            }
        }

        private static async Task RunUplaod(AppSettings settings, IReadOnlyList<string> args)
        {
            var templates = new AzureTableStorage<PartnerTemplateSettings>(settings.EmailFormatterSettings.Partners.ConnectionString, settings.EmailFormatterSettings.Partners.TableName, new LogToConsole());

            var root = new DirectoryInfo(".");
            foreach (var partner in root.GetDirectories())
                foreach (var language in partner.GetDirectories())
                {
                    var subjectsPath = Path.Combine(language.FullName, "subjects.json");
                    if (!File.Exists(subjectsPath))
                        continue;

                    var subjects = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                            File.ReadAllText(subjectsPath, Encoding.UTF8));

                    System.Console.WriteLine($"Uploading templates for {partner.Name}");
                    foreach (var caseName in subjects.Keys)
                    {
                        string baseUrl;
                        switch (partner.Name)
                        {
                            case "AlpineVault":
                                baseUrl = "https://lkealpinevaultemails.blob.core.windows.net/alpinevault/EN/";
                                break;
                            default:
                                baseUrl = "https://lkefiles.blob.core.windows.net/mails/LykkeWallet/";
                                break;
                        }
                        var subjectTemplate = subjects[caseName];

                        await templates.ModifyOrCreateAsync(partner.Name, $"{caseName}_{language.Name}", () => new PartnerTemplateSettings
                        {
                            PartitionKey = partner.Name,
                            RowKey = $"{caseName}_{language.Name}",
                            SubjectTemplate = subjectTemplate,
                            HtmlTemplateUrl = baseUrl + caseName + ".html"
                        }, existing =>
                        {
                            existing.SubjectTemplate = subjectTemplate;
                            existing.HtmlTemplateUrl = baseUrl + caseName + ".html";
                        });
                    }
                }
        }

        private static string GetTemplate(DirectoryInfo folder, string caseName, string type)
        {
            var filePath = Path.Combine(folder.FullName, $"{caseName}.{type}");
            return File.Exists(filePath) ? File.ReadAllText(filePath, Encoding.UTF8) : null;
        }

        private static string GetEnvironmentVariable(string name, string environment)
        {
            var settingsVariable = $"{name}-{environment}";
            var result = Environment.GetEnvironmentVariable(settingsVariable);
            if (string.IsNullOrWhiteSpace(result))
                throw new ArgumentException($"Error acquiring {name} setting for {environment} environment: {name}-{environment} variable is not defined in .env");
            return result;
        }

        private static void ShowHelp()
        {
            System.Console.WriteLine($"{Assembly.GetEntryAssembly().GetName().Name} [env] [command]");
            System.Console.WriteLine("  env - dev/test/prod - current environment");
            System.Console.WriteLine("  command - upload - upload templates");
        }
    }
}