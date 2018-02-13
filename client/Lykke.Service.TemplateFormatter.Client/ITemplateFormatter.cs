using Lykke.Service.EmailSender;
using Lykke.Service.TemplateFormatter.Client.AutorestClient.Models;
using System.Threading.Tasks;

namespace Lykke.Service.TemplateFormatter.Client
{
    /// <summary>
    /// TemplateFormatter Client interface
    /// </summary>
    public interface ITemplateFormatter
    {
        /// <summary>
        /// Formats an email template according to parameters
        /// </summary>
        Task<EmailMessage> FormatAsync(string caseId, string partnerId, string language, object model = null);
    }
}
