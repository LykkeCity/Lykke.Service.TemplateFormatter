using System.ComponentModel.DataAnnotations;
using Lykke.AzureQueueIntegration;

namespace Lykke.Service.TemplateFormatter.Web.Settings
{
    public class SlackNotificationSettings
    {
        [Required]
        public AzureQueueSettings AzureQueue { get; set; }
    }
}