using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EmailFormatter.AutorestClient.Models;
using Lykke.Service.EmailSender;

namespace Lykke.Service.EmailFormatter
{
    public interface IEmailFormatter
    {
        Task<EmailMessage> FormatAsync(string caseId, string partnerId, string language, object model = null);
    }
}