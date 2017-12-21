using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.WebExtensions;

namespace Lykke.Service.TemplateFormatter.Web.Settings
{
    public class AppSettings : IAppSettings, ILogSettings
    {
        public TemplateFormatterSettings TemplateFormatterSettings { get; set; }

        public SlackNotificationSettings SlackNotifications { get; set; }

        ILogToSlackSettings ILogSettings.LogToSlack => SlackNotifications;
        ILogToAzureSettings ILogSettings.LogToAzure => TemplateFormatterSettings.Log;
        string ILogSettings.ServiceName => nameof(TemplateFormatter);
    }
}
