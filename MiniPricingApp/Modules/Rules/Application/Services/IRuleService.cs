using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.Rules.Application.Dtos;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Shares.Common;

namespace MiniPricingApp.Modules.Rules.Application.Services
{
    public interface IRuleService
    {
        public Task<BaseResponse<CreateAndUpdateResponse>> CreateRule(PricingRuleRequestDto request);
        public Task<BaseResponse<CreateAndUpdateResponse>> UpdateRule(Guid Id, PricingRuleRequestDto request);

        public Task<BaseResponse<CreateAndUpdateResponse>> DeleteRule(Guid Id);
        public BaseResponse<PricingRuleResponseDto> GetRuleById(Guid Id);
        public BaseResponse<List<PricingRuleResponseDto>> GetRules();

    }
}
