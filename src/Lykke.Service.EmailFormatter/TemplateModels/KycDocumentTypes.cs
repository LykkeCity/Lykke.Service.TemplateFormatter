namespace Lykke.Service.EmailFormatter.TemplateModels
{
    public static class KycDocumentTypes
    {
        public static string GetDocumentTypeName(KycDocumentTypeApi type)
        {
            switch (type)
            {
                case KycDocumentTypeApi.IdCard:
                    return "Passport or ID";
                case KycDocumentTypeApi.ProofOfAddress:
                    return "Proof of address";
                case KycDocumentTypeApi.Selfie:
                    return "Selfie";
                default:
                    return "Unknown";
            }
        }
    }
}