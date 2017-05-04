using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;

namespace Lykke.Service.EmailFormatter.Core.Logs
{
    public class LogToAll : ILog
    {
        private readonly ILog[] _logs;

        public LogToAll(params ILog[] logs)
        {
            if (logs == null) logs = Array.Empty<ILog>();
            _logs = logs;
        }

        public Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            if (!dateTime.HasValue) dateTime = DateTime.UtcNow;
            return Task.WhenAll(_logs.Select(x => x.WriteInfoAsync(component, process, context, info, dateTime)));
        }

        public Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            if (!dateTime.HasValue) dateTime = DateTime.UtcNow;
            return Task.WhenAll(_logs.Select(x => x.WriteWarningAsync(component, process, context, info, dateTime)));
        }

        public Task WriteErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            if (!dateTime.HasValue) dateTime = DateTime.UtcNow;
            return Task.WhenAll(_logs.Select(x => x.WriteErrorAsync(component, process, context, exception, dateTime)));
        }

        public Task WriteFatalErrorAsync(string component, string process, string context, Exception exception, DateTime? dateTime = null)
        {
            if (!dateTime.HasValue) dateTime = DateTime.UtcNow;
            return Task.WhenAll(_logs.Select(x => x.WriteFatalErrorAsync(component, process, context, exception, dateTime)));
        }
    }
}