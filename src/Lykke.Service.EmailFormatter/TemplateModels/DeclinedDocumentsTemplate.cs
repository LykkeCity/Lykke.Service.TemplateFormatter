using System;
using System.Text;

namespace Lykke.Service.EmailFormatter.TemplateModels
{
    public class DeclinedDocumentsTemplate
    {
        public string FullName { get; set; }
        public int Year { get; set; }
        public string DocumentsAsHtml { get; set; }
    }
}
