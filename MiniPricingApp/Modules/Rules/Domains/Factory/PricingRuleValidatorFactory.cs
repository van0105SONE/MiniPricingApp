using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using MiniPricingApp.Modules.Rules.Domains.Validator;

namespace MiniPricingApp.Modules.Rules.Domains.Factory
{
    /// <summary>
    /// Factory responsible for resolving the appropriate validator instance
    /// based on the provided <see cref="BasePricingRule"/>.
    /// <br/><br/>
    /// This allows each pricing rule type (e.g., WeightTier, TimeWindowPromotion)
    /// to have its own validation logic encapsulated in an <see cref="IPricingRuleValidator"/>.
    /// </summary>
    public class PricingRuleValidatorFactory : IPricingRuleValidatorFactory
    {
        private readonly WeightTierValidator _weightTierValidator;

        public PricingRuleValidatorFactory(WeightTierValidator weightTierValidator)
        {
            _weightTierValidator = weightTierValidator;
        }

        /// <summary>
        /// Retrieves the appropriate validator for a given pricing rule.
        /// </summary>
        /// <param name="rule">The pricing rule that requires validation.</param>
        /// <returns>
        /// A matching <see cref="IPricingRuleValidator"/> if supported,
        /// or <c>null</c> if the rule type has no associated validator.
        /// </returns>
        public IPricingRuleValidator? GetValidator(BasePricingRule rule)
        {
            return rule switch
            {
                WeightTierEntity => _weightTierValidator,
                _ => null
            };
        }
    }
}
