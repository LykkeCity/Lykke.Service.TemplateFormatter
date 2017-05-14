﻿using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.EmailFormatter.Core.Settings
{
    public class PartnerTemplateSettings : TableEntity
    {
        public string SubjectTemplate { get; set; }
        public string TextTemplate { get; set; }
        public string HtmlTemplate { get; set; }
    }
}