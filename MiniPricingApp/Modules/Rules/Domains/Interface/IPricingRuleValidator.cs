using MiniPricingApp.Modules.Rules.Domains.Entities;

namespace MiniPricingApp.Modules.Rules.Domains.Interface
{
    public interface IPricingRuleValidator
    {
        void Validate(BasePricingRule rule);
    }
}
