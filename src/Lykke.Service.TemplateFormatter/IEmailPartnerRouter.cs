using System.Threading.Tasks;
using Lykke.Service.EmailSender;

namespace Lykke.Service.TemplateFormatter
{
    public interface IEmailFormatter
    {
        Task<EmailMessage> FormatAsync(string caseId, string partnerId, string language, object model = null);
    }
}