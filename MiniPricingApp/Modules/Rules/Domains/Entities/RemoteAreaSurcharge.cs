using MiniPricingApp.Modules.qoutes.Domain.Entities;

namespace MiniPricingApp.Modules.Rules.Domains.Entities
{
    public class RemoteAreaSurcharge : BasePricingRule
    {
        public List<string> RemoteAreas { get; }
        public decimal Surcharge { get; }

        public RemoteAreaSurcharge(Guid? id, IEnumerable<string> remoteAreas, decimal surcharge,
            int priority = 0, DateTime? effectiveFrom = null, DateTime? effectiveTo = null, bool isActive = true)
        {
            Id = id;
            RemoteAreas = remoteAreas.ToList();
            Surcharge = surcharge;
            Priority = priority;
            EffectiveFrom = effectiveFrom ?? DateTime.MinValue;
            EffectiveTo = effectiveTo ?? DateTime.MaxValue;
            IsActive = isActive;
        }

        protected override bool CheckApplicability(QouteEntity context)
        {
            return RemoteAreas.Contains(context.AreaCode);
        }

        protected override decimal ApplyRule(decimal basePrice, QouteEntity context)
        {
            return basePrice + Surcharge;
        }


    }
}
