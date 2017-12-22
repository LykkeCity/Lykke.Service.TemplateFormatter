using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.TemplateFormatter.Services
{
    public class TemplateConfiguration
    {
        [Required]
        public string Subject { get; set; }
    }
}