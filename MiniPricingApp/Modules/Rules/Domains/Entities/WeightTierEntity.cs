using MiniPricingApp.Modules.qoutes.Domain.Entities;

namespace MiniPricingApp.Modules.Rules.Domains.Entities
{
    public class WeightTierEntity : BasePricingRule
    {


        public float MinWeight { get; }
        public float MaxWeight { get; }
        public decimal PricePerTier { get; }

        public WeightTierEntity(Guid? id, float min, float max, decimal pricePerTier,
            int priority = 0, DateTime? effectiveFrom = null, DateTime? effectiveTo = null, bool isActive = true)
        {
            Id = id;
            MinWeight = min;
            MaxWeight = max;
            PricePerTier = pricePerTier;
            Priority = priority;
            EffectiveFrom = effectiveFrom ?? DateTime.MinValue;
            EffectiveTo = effectiveTo ?? DateTime.MaxValue;
            IsActive = isActive;
        }


        protected override bool CheckApplicability(QouteEntity context)
        {
            bool isApplicability = context.Weight >= MinWeight && context.Weight <= MaxWeight;
            return isApplicability;
        }

        protected override decimal ApplyRule(decimal basePrice, QouteEntity context)
        {
            return basePrice + PricePerTier;
        }
    }

}
