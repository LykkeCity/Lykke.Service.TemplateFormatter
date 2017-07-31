namespace Lykke.Service.EmailFormatter.TemplateModels
{
    public class RequestForDocumentTemplate
    {
        public string ClientId { get; set; }
        public string FullName { get; set; }
        public string Text { get; set; }
        public string Comment { get; set; }
        public string Amount { get; set; }
        public string AssetId { get; set; }
    }
}
