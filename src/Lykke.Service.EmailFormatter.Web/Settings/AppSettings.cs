using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.WebExtensions;

namespace Lykke.Service.EmailFormatter.Web.Settings
{
    public class AppSettings
    {
        public static async Task<AppSettings> LoadAsync(string settingsUrl)
        {
            if (string.IsNullOrWhiteSpace(settingsUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(settingsUrl));

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(settingsUrl);
            var settingsString = await response.Content.ReadAsStringAsync();
            var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(settingsString);
            if (null == settings)
                throw new ArgumentException($"Unable to load settings from {settingsUrl}");

            var validationContext = new ValidationContext(settings);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(settings, validationContext, validationResults, true))
                throw new AggregateException("Invalid configuration", validationResults.Select(error => new Exception($"{error.MemberNames}: {error.ErrorMessage}")));

            return settings;
        }

        [Required, ValidateObject]
        public EmailFormatterSettings EmailFormatterSettings { get; set; }

        [Required, ValidateObject]
        public SlackNotificationSettings SlackNotifications { get; set; }
    }
}
