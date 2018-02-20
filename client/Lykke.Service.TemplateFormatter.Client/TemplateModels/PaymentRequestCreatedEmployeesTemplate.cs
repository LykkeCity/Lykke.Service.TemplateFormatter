namespace Lykke.Service.TemplateFormatter.TemplateModels
{
    public class PaymentRequestCreatedEmployeesTemplate
    {
        public string InvoiceNumber { get; set; }
        public string ClientFullName { get; set; }
        public decimal AmountToBePaid { get; set; }
        public string SettlementCurrency { get; set; }
        public string DueDate { get; set; }
        public string InvoiceDetailsLink { get; set; }
        public int Year { get; set; }
    }
}
