namespace MiniPricingApp.Modules.Rules.Application.Dtos
{
    public class PricingRuleResponseDto
    {
        public Guid? Id { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public bool IsActive { get; set; }

        // Optional rule-specific fields
        public decimal? DiscountPercent { get; set; }
        public List<string> RemoteAreas { get; set; }
        public decimal? Surcharge { get; set; }
        public float? MinWeight { get; set; }
        public float? MaxWeight { get; set; }
        public decimal? PricePerKg { get; set; }
    }
}
