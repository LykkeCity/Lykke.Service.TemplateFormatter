﻿using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.TemplateFormatter.Web.Settings
{
    public class PartnerTemplateSettings : TableEntity
    {
        public string SubjectTemplate { get; set; }
        public string TextTemplateUrl { get; set; }
        public string HtmlTemplateUrl { get; set; }
    }
}