using MiniPricingApp.Modules.qoutes.Domain.Entities;

namespace MiniPricingApp.Modules.Rules.Domains.Entities
{
    public class TimeWindowPromotion : BasePricingRule
    {
        public decimal DiscountPercent { get; }

        public TimeWindowPromotion(Guid? id, decimal distcountPercent, int priority = 0,
            DateTime? effectiveFrom = null, DateTime? effectiveTo = null, bool isActive = true)
        {
            Id = id;
            DiscountPercent = distcountPercent;
            Priority = priority;
            EffectiveFrom = effectiveFrom ?? DateTime.MinValue;
            EffectiveTo = effectiveTo ?? DateTime.MaxValue;
            IsActive = isActive;
        }

        protected override bool CheckApplicability(QouteEntity context)
        {
            // Only apply if order time is within promotion window
            return context.CreatedAt >= EffectiveFrom && context.CreatedAt <= EffectiveTo;
        }

        protected override decimal ApplyRule(decimal basePrice, QouteEntity context)
        {
            return basePrice - (basePrice * DiscountPercent / 100); ;
        }
    }
}
