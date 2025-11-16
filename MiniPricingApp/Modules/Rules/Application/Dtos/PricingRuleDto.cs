namespace MiniPricingApp.Modules.Rules.Application.Dtos
{
    public class PricingRuleDto
    {
        public Guid? Id { get; set; }
        public string RuleType { get; set; }        // e.g., "TimeWindowPromotion"
        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime EffectiveFrom { get; set; } = DateTime.MinValue;
        public DateTime EffectiveTo { get; set; } = DateTime.MaxValue;

        // Rule-specific parameters
        public decimal DiscountPercent { get; set; }      // for TimeWindowPromotion
        public decimal Surcharge { get; set; }           // for RemoteAreaSurcharge
        public string[] RemoteAreas { get; set; }        // for RemoteAreaSurcharge
        public float MinWeight { get; set; }           // for WeightTier
        public float MaxWeight { get; set; }           // for WeightTier
        public decimal PricePerTier { get; set; }     // for WeightTier
    }
}
