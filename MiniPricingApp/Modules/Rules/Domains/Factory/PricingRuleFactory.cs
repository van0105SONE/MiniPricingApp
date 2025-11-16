using MiniPricingApp.Modules.Rules.Application.Dtos;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Interface;

namespace MiniPricingApp.Modules.Rules.Domains.Factory
{
    /*
     * PricingRuleFactory
     * -------------------
     * Factory responsible for creating concrete pricing rule entities based on
     * the incoming PricingRuleRequestDto. This keeps object construction logic
     * centralized and prevents controllers/services from instantiating rule types
     * directly.
     *
     * Why this class exists:
     * • Encapsulates rule creation logic (Factory Pattern).
     * • Keeps the domain consistent by ensuring all rules are created in a valid state.
     * • Avoids spreading “if RuleType == …” logic across the application.
     *
     * Special logic for WeightTier:
     * • WeightTier rules define sequential weight ranges.
     * • To prevent gaps between tiers (which would cause missing price coverage),
     *   MinWeight is automatically assigned using the next available boundary
     *   provided by IWeightTierDomainService.
     *
     * Behavior:
     * • Reads RuleType from the DTO.
     * • Constructs the appropriate rule entity with validated/defaulted values.
     * • Throws an exception if the rule type is unsupported.
     */
    public class PricingRuleFactory
    {
        private readonly IWeightTierDomainService _weightTierService;

        public PricingRuleFactory(IWeightTierDomainService weightTierService)
        {
            _weightTierService = weightTierService;
        }

        public BasePricingRule Create(PricingRuleRequestDto dto)
        {
            if (dto.RuleType == "WeightTier")
            {
                // Automatically assigns a correct MinWeight to avoid gaps in tier ranges.
                float minWeight = _weightTierService.GetNextMinWeight();
                return new WeightTierEntity(
                    Guid.NewGuid(),
                    minWeight,
                    dto.MaxWeight,
                    dto.PricePerTier,
                    0,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    dto.IsActive
                );
            } else if (dto.RuleType == "TimeWindowPromotion") {
               return new TimeWindowPromotion(
                    Guid.NewGuid(),
                    dto.DistcountPercent,
                    1,
                    dto.EffectiveFrom,
                    dto.EffectiveTo,
                    dto.IsActive
                );
             } else if(dto.RuleType == "RemoteAreaSurcharge") {
                return new RemoteAreaSurcharge(Guid.NewGuid(), dto.RemoteAreas, dto.Surcharge, 2, dto.EffectiveFrom, dto.EffectiveTo, dto.IsActive);
            }
            else
            {
                throw new ArgumentException($"Unsupported rule type: {dto.RuleType}");
            }

        }
    }

}
