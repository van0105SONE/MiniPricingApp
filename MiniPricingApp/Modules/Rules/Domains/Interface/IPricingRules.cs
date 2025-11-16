using MiniPricingApp.Modules.qoutes.Domain.Entities;

namespace MiniPricingApp.Modules.Rules.Domains.Interface
{
    public interface IPricingRules
    {
        decimal Apply(decimal basePrice, QouteEntity context);
        bool IsApplicable(QouteEntity context);
    }
}
