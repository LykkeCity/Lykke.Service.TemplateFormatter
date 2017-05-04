using System.ComponentModel.DataAnnotations;
using Lykke.Service.EmailFormatter.Core.Validation;

namespace Lykke.Service.EmailFormatter.Core.Settings
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