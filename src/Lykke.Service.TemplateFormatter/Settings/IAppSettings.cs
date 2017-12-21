namespace Lykke.Service.TemplateFormatter.Web.Settings
{
    public interface IAppSettings
    {
        TemplateFormatterSettings TemplateFormatterSettings { get; set; }
        SlackNotificationSettings SlackNotifications { get; set; }
    }
}