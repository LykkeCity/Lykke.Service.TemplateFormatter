
namespace Lykke.Service.TemplateFormatter.Core.Settings
{
    public class TemplateFormatterSettings
    {
        public AzureTableSettings Log { get; set; }

        public AzureTableSettings Partners { get; set; }
    }

    public class AzureTableSettings
    {
        public string ConnectionString { get; set; }

        public string TableName { get; set; }
    }
}
