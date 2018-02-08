using Lykke.Service.LykkeService.Core.Settings.SlackNotifications;

namespace Lykke.Service.TemplateFormatter.Core.Settings
{
    public class AppSettings
    {
        public TemplateFormatterSettings TemplateFormatterSettings { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
