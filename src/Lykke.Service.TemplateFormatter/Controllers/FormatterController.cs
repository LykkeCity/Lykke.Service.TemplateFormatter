using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureStorage;
using Common.Log;
using Lykke.Service.EmailSender;
using Lykke.Service.TemplateFormatter.AzureRepositories;
using Lykke.Service.TemplateFormatter.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.TemplateFormatter.Controllers
{
    public class FormatterController : Controller
    {
        private static readonly Regex ParameterRegex = new Regex(@"@\[([^\]]+)\]");

        private readonly INoSQLTableStorage<PartnerTemplateSettings> _partnerTemplateSettings;
        private readonly ILog _log;

        public FormatterController(INoSQLTableStorage<PartnerTemplateSettings> partnerTemplateSettings, ILog log)
        {
            _partnerTemplateSettings = partnerTemplateSettings;
            _log = log;
        }

        [HttpPost]
        [Route("api/[controller]/{caseId}/{partnerId}/{language}")]
        public async Task<EmailFormatResponse> Format(string caseId, string partnerId, string language, [FromBody]Dictionary<string, string> parameters)
        {
            if(null == parameters)
                parameters = new Dictionary<string, string>();

            try
            {
                var template = _partnerTemplateSettings[partnerId, $"{caseId}_{language}"];
                if (null == template)
                {
                    partnerId = "Lykke";
                    template = _partnerTemplateSettings[partnerId, $"{caseId}_{language}"];
                }
                if (null == template)
                    throw new InvalidOperationException($"Unable to find email template {caseId} ({language}) for partner {partnerId}");

                Console.WriteLine($"template url: {template.HtmlTemplateUrl}");

                string MatchEvaluator(Match match)
                {
                    var key = match.Groups[1].Value;
                    if (null != parameters && parameters.ContainsKey(key))
                        return parameters[key];
                    throw new KeyNotFoundException($"Unable to find parameter {key} required by email template {caseId} ({language}) for partner {partnerId}");
                }

                return new EmailFormatResponse
                {
                    EmailMessage = new EmailMessage
                    {
                        Subject = string.IsNullOrWhiteSpace(template.SubjectTemplate)
                            ? "testSubject"
                            : ParameterRegex.Replace(template.SubjectTemplate, MatchEvaluator),
                        TextBody = string.IsNullOrWhiteSpace(template.TextTemplateUrl)
                            ? null
                            : ParameterRegex.Replace(await LoadTemplate(template.TextTemplateUrl), MatchEvaluator),
                        HtmlBody = string.IsNullOrWhiteSpace(template.HtmlTemplateUrl)
                            ? null
                            : ParameterRegex.Replace(await LoadTemplate(template.HtmlTemplateUrl), MatchEvaluator)
                    }
                };
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(TemplateFormatter), nameof(Startup), nameof(Format), ex, DateTime.UtcNow);
                throw;
            }
        }

        private async Task<string> LoadTemplate(string templateUrl)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(templateUrl);
                if(response.StatusCode != HttpStatusCode.OK || null == response.Content)
                    throw new InvalidOperationException("Template not found");
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
