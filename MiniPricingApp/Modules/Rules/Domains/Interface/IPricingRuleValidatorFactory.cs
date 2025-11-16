using MiniPricingApp.Modules.Rules.Domains.Entities;

namespace MiniPricingApp.Modules.Rules.Domains.Interface
{
    public interface IPricingRuleValidatorFactory
    {
        IPricingRuleValidator? GetValidator(BasePricingRule rule);
    }
}
