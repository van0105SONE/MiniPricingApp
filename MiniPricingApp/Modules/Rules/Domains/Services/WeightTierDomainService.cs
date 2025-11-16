using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;

namespace MiniPricingApp.Modules.Rules.Domains.Services
{
    /// <summary>
    /// Domain service responsible for weight-tier related business logic.
    /// This service isolates all WeightTier evaluation rules from the application,
    /// following the Domain Service pattern (DDD).
    /// </summary>
    public class WeightTierDomainService : IWeightTierDomainService
    {
        private readonly IPricingRuleRepository _repo;

        public WeightTierDomainService(IPricingRuleRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves the next minimum weight boundary used when creating a new WeightTier.
        /// 
        /// Logic:
        /// - Fetch the last tier based on MaxWeight (highest weight range).
        /// - If none exists, start the first tier at 0.
        /// - Otherwise, the next tier must begin at (last.MaxWeight + 1).
        /// </summary>
        public float GetNextMinWeight()
        {
            var lastTier = _repo.findLastMaxWeightTier();
            return lastTier == null ? 0 : lastTier.MaxWeight + 1;
        }


        /// <summary>
        /// Checks if there is ANY WeightTier rule that covers the given weight.
        ///
        /// Why this check is needed:
        /// - When calculating price for a quote, the weight must fall into an existing
        ///   WeightTier rule. If not, pricing rules cannot be applied correctly.
        /// - This prevents invalid or incomplete configurations.
        ///
        /// Steps:
        /// 1. Load all pricing rules.
        /// 2. Filter only WeightTierEntity (since the repository returns BasePricingRuleEntity).
        /// 3. Check if any WeightTier has a range [MinWeight, MaxWeight] that includes the input.
        /// </summary>
        public bool ExistsTierForWeight(float weight)
        {
            var rules =  _repo.GetMany();
            var weightTiers = rules.OfType<WeightTierEntity>().ToList();
            return weightTiers.Any(t => weight >= t.MinWeight && weight <= t.MaxWeight);
        }
    }
}
