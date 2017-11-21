using System.ComponentModel.DataAnnotations;
using Lykke.AzureQueueIntegration;
using Lykke.WebExtensions;

namespace Lykke.Service.TemplateFormatter.Web.Settings
{
    public class SlackNotificationSettings : ILogToSlackSettings
    {
        [Required]
        public AzureQueueSettings AzureQueue { get; set; }
    }
}