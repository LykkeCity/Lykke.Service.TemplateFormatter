﻿using System;

namespace Lykke.Service.EmailFormatter.TemplateModels
{
    public class KycDocument
    {
        public string ClientId { get; set; }
        public string DocumentId { get; set; }
        public string Type { get; set; }
        public string Mime { get; set; }
        public string KycComment { get; set; }
        public string State { get; set; }
        public string FileName { get; set; }
        public DateTime DateTime { get; set; }
        public string DocumentName { get; set; }
    }
}