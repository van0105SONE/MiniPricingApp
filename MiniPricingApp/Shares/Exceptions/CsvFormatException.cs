namespace MiniPricingApp.Shares.Exceptions
{
    public class CsvFormatException  : Exception
    {
        public CsvFormatException(string message) : base(message) { }
    }
}
