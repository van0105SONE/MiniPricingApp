using MiniPricingApp.Modules.Rules.Application.Dtos;
using MiniPricingApp.Modules.Rules.Domains.Entities;

namespace MiniPricingApp.Modules.Rules.Application.Mapper
{
    public class BasePricingRuleMapper
    {


        public static BasePricingRule MapToDomain(PricingRuleDto dto)
        {
            BasePricingRule rule = dto.RuleType switch
            {
                "WeightTier" => new WeightTierEntity(
                    dto.Id == null ? Guid.NewGuid() : dto.Id,
                    dto.MinWeight,
                    dto.MaxWeight,
                    dto.PricePerTier,
                    dto.Priority,
                    dto.EffectiveFrom,
                    dto.EffectiveTo,
                    dto.IsActive
                ),
                "TimeWindowPromotion" => new TimeWindowPromotion(
                    dto.Id == null ? Guid.NewGuid() : dto.Id,
                    dto.DiscountPercent,
                    dto.Priority,
                    dto.EffectiveFrom,
                    dto.EffectiveTo,
                    dto.IsActive
               ),
                "RemoteAreaSurcharge" => new RemoteAreaSurcharge(dto.Id == null ? Guid.NewGuid() : dto.Id, dto.RemoteAreas, dto.Surcharge, dto.Priority, dto.EffectiveFrom, dto.EffectiveTo, dto.IsActive),
                _ => throw new ArgumentException($"Unsupported rule type: {dto.RuleType}")
            };

            // common properties
            rule.Priority = dto.Priority;
            rule.EffectiveFrom = dto.EffectiveFrom;
            rule.EffectiveTo = dto.EffectiveTo;
            rule.IsActive = dto.IsActive;

            return rule;
        }

        public static PricingRuleResponseDto MapToResponse(BasePricingRule rule)
        {
            var dto = new PricingRuleResponseDto
            {
                Id = rule.Id,
                Type = rule.GetType().Name,
                Priority = rule.Priority,
                EffectiveFrom = rule.EffectiveFrom,
                EffectiveTo = rule.EffectiveTo,
                IsActive = rule.IsActive
            };

            switch (rule)
            {
                case TimeWindowPromotion p:
                    dto.DiscountPercent = p.DiscountPercent;
                    break;

                case RemoteAreaSurcharge s:
                    dto.Surcharge = s.Surcharge;
                    dto.RemoteAreas = s.RemoteAreas;
                    break;

                case WeightTierEntity w:
                    dto.MinWeight = w.MinWeight;
                    dto.MaxWeight = w.MaxWeight;
                    dto.PricePerKg = w.PricePerTier;
                    break;
            }

            return dto;
        }
    }
}
