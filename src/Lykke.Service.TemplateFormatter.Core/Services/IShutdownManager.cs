using System.Threading.Tasks;

namespace Lykke.Service.TemplateFormatter.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}