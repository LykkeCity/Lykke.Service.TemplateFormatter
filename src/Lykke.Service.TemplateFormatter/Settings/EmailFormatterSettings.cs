using System.ComponentModel.DataAnnotations;
using Lykke.WebExtensions;

namespace Lykke.Service.TemplateFormatter.Web.Settings
{
    public class TemplateFormatterSettings
    {
        [ValidateObject]
        public AzureTableSettings Log { get; set; }

        [Required, ValidateObject]
        public AzureTableSettings Partners { get; set; }
    }

    public class AzureTableSettings : ILogToAzureSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string ConnectionString { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string TableName { get; set; }
    }
}