using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;

namespace MiniPricingApp.Modules.Rules.Domains.Validator
{
    /// <summary>
    /// Validator for <see cref="WeightTierEntity"/> rules.
    /// Ensures that any new WeightTier added to the system is valid and does not conflict
    /// with existing tiers.
    /// </summary>
    public class WeightTierValidator : IPricingRuleValidator
    {
            private readonly IPricingRuleRepository _repository;

            public WeightTierValidator(IPricingRuleRepository repository)
            {
                _repository = repository;
            }

        /// <summary>
        /// Validates a <see cref="WeightTierEntity"/> to ensure:
        /// 1. The new tier does not overlap with the last existing tier.
        /// 2. The minimum weight is strictly less than the maximum weight.
        /// 
        /// Logic:
        /// - If <paramref name="rule"/> is not a WeightTierEntity, the method returns without action.
        /// - Retrieves the last WeightTier from the repository to check for overlaps.
        /// - Throws <see cref="InvalidOperationException"/> if the new tier overlaps or min >= max.
        /// 
        /// This prevents gaps or duplicate coverage in weight ranges, ensuring all weights
        /// are properly accounted for during price calculation.
        /// </summary>
        /// <param name="rule">The pricing rule to validate.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the WeightTier overlaps an existing tier or has invalid min/max weights.
        /// </exception>
        public void Validate(BasePricingRule rule)
            {
                if (rule is not WeightTierEntity weightTier)
                    return;

                // retrieve last tier
                var lastTier = _repository.findLastMaxWeightTier();

                // validate min > last max
                if (lastTier != null && weightTier.MinWeight <= lastTier.MaxWeight)
                {
                    throw new InvalidOperationException(
                        $"WeightTier {weightTier.MinWeight}-{weightTier.MaxWeight} overlaps with existing tier {lastTier.MinWeight}-{lastTier.MaxWeight}"
                    );
                }


            // Ensure minWeight < maxWeight to avoid skipped weights or conflicts
            if (weightTier.MinWeight >= weightTier.MaxWeight)
                {
                    throw new InvalidOperationException("MinWeight must be less than MaxWeight.");
                }
            }
    }
}
