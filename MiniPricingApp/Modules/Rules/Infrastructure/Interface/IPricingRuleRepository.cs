using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Shares.Interfaces;

namespace MiniPricingApp.Modules.Rules.Infrastructure.Interface
{
    public interface IPricingRuleRepository : IBaseRepository<BasePricingRule>
    {
        WeightTierEntity findLastMaxWeightTier();
    }
}
