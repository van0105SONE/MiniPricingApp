using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using System.Text.Json.Serialization;

namespace MiniPricingApp.Modules.Rules.Domains.Entities
{
    /*
     * BasePricingRuleEntity
     * ----------------------
     * Abstract base class for all pricing rule types (WeightTier, TimeWindowPromotion,
     * RemoteAreaSurcharge, etc.). Every pricing rule inherits from this class and
     * implements its own applicability logic and price calculation behavior.
     *
     * Key Responsibilities:
     * • Provide common rule metadata:
     *     - Priority: determines execution order
     *     - EffectiveFrom / EffectiveTo: controls when a rule is active
     *     - IsActive: allows enabling/disabling a rule without deleting it
     *
     * • Define the standard evaluation flow for all rules:
     *     1. IsApplicable(context)
     *         - Checks global rule constraints (active + effective date range)
     *         - Delegates domain-specific checks to CheckApplicability()
     *
     *     2. Apply(basePrice, context)
     *         - Calls ApplyRule() implemented by child classes to modify the price
     *
     * Why this design:
     * • Ensures consistent behavior across all rule types.
     * • Centralizes shared logic so each child rule only implements what is unique.
     * • Supports polymorphic JSON serialization via JsonDerivedType, allowing
     *   dynamic rule loading from the database or configuration.
     *
     * Child classes must implement:
     * • CheckApplicability() – determines whether the rule applies to a given quote
     * • ApplyRule()          – applies the pricing logic when the rule is applicable
 */
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "RuleType")]
    [JsonDerivedType(typeof(WeightTierEntity), "WeightTier")]
    [JsonDerivedType(typeof(TimeWindowPromotion), "TimeWindowPromotion")]
    [JsonDerivedType(typeof(RemoteAreaSurcharge), "RemoteAreaSurcharge")]
    public abstract class BasePricingRule : IPricingRules
    {
        public Guid? Id { get; set; }
        public int Priority { get; set; } = 0;
        public DateTime EffectiveFrom { get; set; } = DateTime.MinValue;
        public DateTime EffectiveTo { get; set; } = DateTime.MaxValue;
        public bool IsActive { get; set; } = true;

        public bool IsApplicable(QouteEntity context)
        {
            if (!IsActive) return false;
            if (context.CreatedAt < EffectiveFrom || context.CreatedAt > EffectiveTo)
                return false;

            return CheckApplicability(context);
        }

        public decimal Apply(decimal basePrice, QouteEntity context)
        {
            return ApplyRule(basePrice, context);
        }

        // Child classes implement these
        protected abstract bool CheckApplicability(QouteEntity context);
        protected abstract decimal ApplyRule(decimal basePrice, QouteEntity context);
    }
}
