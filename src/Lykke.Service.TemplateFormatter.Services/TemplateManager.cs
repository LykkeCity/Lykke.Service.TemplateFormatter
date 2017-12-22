using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureStorage;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Service.TemplateFormatter.Services
{
    public class TemplateManager
    {
        public const string TemplatesContainerName = "templates";

        private const string PartnerIdGroupName = "partnerId";
        private const string LanguageGroupName = "language";
        private const string TemplateNameGroupName = "templateName";
        private const string TemplatePartTypeGroupName = "partType";
        public const string TemplatePartTypeHtml = "html";
        public const string TemplatePartTypeText = "txt";
        public const string TemplatePartTypeSubject = "json";

        private static readonly Regex TemplatePathRegex = new Regex($"^(?<{PartnerIdGroupName}>[^/]+)/(?<{LanguageGroupName}>[^/]+)/(?<{TemplateNameGroupName}>[^/]+).(?<{TemplatePartTypeGroupName}>{TemplatePartTypeSubject}|{TemplatePartTypeHtml}|{TemplatePartTypeText})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IBlobStorage _blob;

        public TemplateManager([NotNull] IBlobStorage blob)
        {
            _blob = blob ?? throw new ArgumentNullException(nameof(blob));
        }

        public async Task<IEnumerable<TemplateInfo>> GetTemplatesAsync([NotNull] string templateName)
        {
            if (String.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateName));

            var result = new Dictionary<string, TemplateInfo>();

            foreach (var templateFilePath in await _blob.GetListOfBlobKeysAsync(TemplatesContainerName))
            {
                var match = TemplatePathRegex.Match(templateFilePath);
                if (!match.Success || match.Groups[TemplateNameGroupName].Value != templateName)
                    continue;

                var partnerId = match.Groups[PartnerIdGroupName].Value;
                var language = match.Groups[LanguageGroupName].Value;
                var templateKey = GetTemplatePath(partnerId, language, templateName);
                var fileType = match.Groups[TemplatePartTypeGroupName].Value.ToLower();

                if (fileType == TemplatePartTypeSubject)
                {
                    var settingsJson = await _blob.GetAsTextAsync(TemplatesContainerName, templateFilePath);
                    var settings = JsonConvert.DeserializeObject<TemplateConfiguration>(settingsJson);
                    if (string.IsNullOrWhiteSpace(settings?.Subject))
                        continue;
                }

                TemplateInfo templateInfo;
                if (result.ContainsKey(templateKey))
                {
                    templateInfo = result[templateKey];
                }
                else
                {
                    templateInfo = new TemplateInfo(partnerId, language, templateName);
                    result.Add(templateKey, templateInfo);
                }

                switch (fileType)
                {
                    case TemplatePartTypeSubject:
                        templateInfo.HasSubject = true;
                        break;
                    case TemplatePartTypeHtml:
                        templateInfo.HasHtml = true;
                        break;
                    case TemplatePartTypeText:
                        templateInfo.HasText = true;
                        break;
                    default:
                        throw new Exception($"Unknown part type {fileType}");
                }
            }

            return result.Values;
        }

        public async Task<Template> GetTemplateAsync([NotNull] string partnerId, [NotNull] string language, [NotNull] string templateName)
        {
            if (String.IsNullOrWhiteSpace(partnerId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(partnerId));
            if (String.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(language));
            if (String.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateName));

            var template = new Template(partnerId, language, templateName);

            if (await _blob.HasBlobAsync(TemplatesContainerName, GetTemplatePath(partnerId, language, templateName, TemplatePartTypeSubject)))
            {
                var settingsJson = await _blob.GetAsTextAsync(TemplatesContainerName, GetTemplatePath(partnerId, language, templateName, TemplatePartTypeSubject));
                var settings = JsonConvert.DeserializeObject<TemplateConfiguration>(settingsJson);
                if (settings != null && !string.IsNullOrWhiteSpace(settings.Subject))
                    template.Subject = settings.Subject;
            }

            if (await _blob.HasBlobAsync(TemplatesContainerName, GetTemplatePath(partnerId, language, templateName, TemplatePartTypeHtml)))
                template.HtmlBody = await _blob.GetAsTextAsync(TemplatesContainerName, GetTemplatePath(partnerId, language, templateName, TemplatePartTypeHtml));

            if (await _blob.HasBlobAsync(TemplatesContainerName, GetTemplatePath(partnerId, language, templateName, TemplatePartTypeText)))
                template.TextBody = await _blob.GetAsTextAsync(TemplatesContainerName, GetTemplatePath(partnerId, language, templateName, TemplatePartTypeText));

            if (string.IsNullOrWhiteSpace(template.Subject) && string.IsNullOrWhiteSpace(template.HtmlBody) && string.IsNullOrWhiteSpace(template.TextBody))
                return null;

            return template;
        }

        public static string GetTemplatePath(string partnerId, string language, string templateName)
        {
            return $"{partnerId}/{language}/{templateName}";
        }

        public static string GetTemplatePath(string partnerId, string language, string templateName, string partType)
        {
            return $"{GetTemplatePath(partnerId, language, templateName)}.{partType}";
        }
    }
}
