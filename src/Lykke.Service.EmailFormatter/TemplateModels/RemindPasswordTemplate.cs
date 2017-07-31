namespace Lykke.Service.EmailFormatter.TemplateModels
{
    public class RemindPasswordTemplate
    {
        public RemindPasswordTemplate(string hint, int year)
        {
            Hint = hint;
            Year = year;
        }

        public string Hint { get; set; }
        public int Year { get; set; }
    }
}
