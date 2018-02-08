// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.TemplateFormatter.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class EmailMessage
    {
        /// <summary>
        /// Initializes a new instance of the EmailMessage class.
        /// </summary>
        public EmailMessage()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the EmailMessage class.
        /// </summary>
        public EmailMessage(string subject = default(string), string htmlBody = default(string), string textBody = default(string), IList<EmailAttachment> attachments = default(IList<EmailAttachment>))
        {
            Subject = subject;
            HtmlBody = htmlBody;
            TextBody = textBody;
            Attachments = attachments;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Subject")]
        public string Subject { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "HtmlBody")]
        public string HtmlBody { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "TextBody")]
        public string TextBody { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Attachments")]
        public IList<EmailAttachment> Attachments { get; set; }

    }
}
