namespace MiniPricingApp.Modules.Qoutes.Domain.Interfaces
{
    public interface IQouteCsvValidator
    {
        Task Validate(string FilePath);
    }
}
