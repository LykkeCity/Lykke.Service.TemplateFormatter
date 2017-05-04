using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.EmailFormatter.Core.Settings
{
    public class PartnerTemplateSettings : TableEntity
    {
        public string TextTemplate { get; set; }
        public string HtmlTemplate { get; set; }
    }
}