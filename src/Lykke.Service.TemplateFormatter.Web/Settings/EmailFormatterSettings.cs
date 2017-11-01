using System.ComponentModel.DataAnnotations;
using Lykke.WebExtensions;

namespace Lykke.Service.TemplateFormatter.Web.Settings
{
    public class EmailFormatterSettings
    {
        [ValidateObject]
        public AzureTableSettings Log { get; set; }

        [Required, ValidateObject]
        public AzureTableSettings Partners { get; set; }
    }

    public class AzureTableSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string ConnectionString { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string TableName { get; set; }
    }
}