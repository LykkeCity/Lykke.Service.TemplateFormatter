using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lykke.Service.EmailFormatter.Models
{
    public class EmailFormatRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string PartnerId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CaseId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Language { get; set; }

        public string ParametersJson
        {
            get { return JsonConvert.SerializeObject(Parameters); }
            set { Parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(value); }
        }

        [JsonIgnore]
        internal Dictionary<string, string> Parameters { get; set; }
    }

    public class EmailFormatResponse
    {
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
    }
}
