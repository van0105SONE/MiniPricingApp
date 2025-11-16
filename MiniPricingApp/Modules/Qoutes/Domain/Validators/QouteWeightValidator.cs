using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.Qoutes.Domain.Interfaces;
using MiniPricingApp.Modules.Rules.Domains.Interface;

namespace MiniPricingApp.Modules.Qoutes.Domain.Validators
{
    /*
     * QouteWeightValidator
     * --------------------
     * Validates all weight-related rules for a QouteEntity before pricing starts.
     *
     * Why this validator exists:
     * • QouteEntity should validate only its own basic fields.  
     *   It must not check database-driven rules like WeightTiers.
     *
     * • Weight-tier validation requires business rules stored in the DB, 
     *   so it belongs in a domain service + validator, not inside the entity.
     *
     * What it does:
     * 1. Runs entity.validate() to check local entity invariants.
     * 2. Uses IWeightTierDomainService to verify the weight is covered by a WeightTier.
     * 3. Throws an exception if no tier matches, preventing invalid quotes from being priced.
     *
     * This keeps the entity clean, separates responsibilities, and makes validation testable.
     */

    public class QouteWeightValidator : IQouteWeightTierValidator
    {
        private readonly IWeightTierDomainService _tierService;

        public QouteWeightValidator(IWeightTierDomainService tierService)
        {
            _tierService = tierService;
        }

        public void Validate(QouteEntity entity)
        {
            entity.validate(); // internal validation

            // Check if the provided weight falls within any configured WeightTier rule.
            // Each WeightTier defines a valid weight range (Min → Max) that the system
            // uses to compute pricing. If no tier is found, it means the business has
            // not defined how to price this weight.
            //
            // This validation prevents the system from calculating a price for a weight
            // that the business has not configured. Allowing such a case would produce
            // incorrect pricing or silent failures later in the pricing pipeline.
            if (!_tierService.ExistsTierForWeight(entity.Weight))
            {
                throw new Exception($"Weight {entity.Weight} does not match any WeightTier rule.");
            }
        }
    }
}
