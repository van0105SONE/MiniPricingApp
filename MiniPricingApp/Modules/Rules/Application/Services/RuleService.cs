using Microsoft.AspNetCore.Http.HttpResults;
using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.Rules.Application.Dtos;
using MiniPricingApp.Modules.Rules.Application.Mapper;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Factory;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;
using MiniPricingApp.Shares.Common;
using System.Data;
using System.Runtime.CompilerServices;

namespace MiniPricingApp.Modules.Rules.Application.Services
{

    /// <summary>
    /// Application service responsible for managing pricing rules in the system.
    /// <para/>
    /// Responsibilities include:
    /// 1. Creating, updating, retrieving, and deleting pricing rules.
    /// 2. Validating rules using the appropriate <see cref="IPricingRuleValidator"/> from the validator factory.
    /// 3. Delegating object creation to <see cref="PricingRuleFactory"/> to ensure consistent and valid entity construction.
    /// </summary>
    public class RuleService : IRuleService
    {
        private readonly IPricingRuleRepository _pricingRuleRepository;
        private readonly IPricingRuleValidatorFactory _validatorFactory;
        private readonly PricingRuleFactory _factory;
        public RuleService(IPricingRuleRepository pricingRuleRepository, IPricingRuleValidatorFactory pricingRuleValidatorFactory, PricingRuleFactory factory)
        {
            _pricingRuleRepository = pricingRuleRepository;
             _validatorFactory = pricingRuleValidatorFactory;
            _factory = factory;
        }

        /// <summary>
        /// Creates a new pricing rule.
        /// <para/>
        /// Steps:
        /// 1. Use the <see cref="PricingRuleFactory"/> to construct the correct rule entity.
        /// 2. Validate the rule using the corresponding validator from <see cref="IPricingRuleValidatorFactory"/>.
        /// 3. Save the rule to the repository.
        /// </summary>
        /// <param name="request">The DTO containing rule data.</param>
        /// <returns>A <see cref="BaseResponse{CreateAndUpdateResponse}"/> with the created rule Id.</returns>
        public async Task<BaseResponse<CreateAndUpdateResponse>> CreateRule(PricingRuleRequestDto request)
        {
            var rule = _factory.Create(request);
            var validator = _validatorFactory.GetValidator(rule);
            validator?.Validate(rule);

            Guid result = await   _pricingRuleRepository.Save(rule);
            return new BaseResponse<CreateAndUpdateResponse>() {
                isSuccess = true,
                message = "Successful Create Rule",
               data = new CreateAndUpdateResponse() { Id = result }
            };
            
        }

        /// <summary>
        /// Updates an existing pricing rule.
        /// <para/>
        /// Steps:
        /// 1. Create a new rule entity using the factory with the Updated values.
        /// 2. Set its Id to match the existing rule.
        /// 3. Save the Updated rule to the repository.
        /// </summary>
        /// <param name="Id">The Id of the rule to Update.</param>
        /// <param name="request">The Updated rule data.</param>
        /// <returns>A <see cref="BaseResponse{CreateAndUpdateResponse}"/> containing the Updated rule Id.</returns>
        public async Task<BaseResponse<CreateAndUpdateResponse>> UpdateRule(Guid Id, PricingRuleRequestDto request)
        {
            var rule = _factory.Create(request);
            rule.Id = Id;
            Guid result = await _pricingRuleRepository.Update(rule);

            return new BaseResponse<CreateAndUpdateResponse>()
            {
                isSuccess = true,
                message = "Successful Updated Rule",
                data = new CreateAndUpdateResponse() { Id = result }
            };
        }

        public BaseResponse<PricingRuleResponseDto> GetRuleById(Guid Id)
        {
            BasePricingRule rule   =  _pricingRuleRepository.GetById(Id);
            var response = BasePricingRuleMapper.MapToResponse(rule);
            return new BaseResponse<PricingRuleResponseDto>()
            {
                isSuccess = true,
                message = "Get rule by id",
                data = response
            };
        }

        public BaseResponse<List<PricingRuleResponseDto>> GetRules()
        {
            List<BasePricingRule> rules =  _pricingRuleRepository.GetMany();

            var response = rules.Select(x => BasePricingRuleMapper.MapToResponse(x)).ToList();
            return new BaseResponse<List<PricingRuleResponseDto>>()
            {
                isSuccess = true,
                message = "Get rules",
                data = response
            };
        }



        public async Task<BaseResponse<CreateAndUpdateResponse>> DeleteRule(Guid Id)
        {
            var result = await _pricingRuleRepository.Detele(Id);
            return new BaseResponse<CreateAndUpdateResponse>()
            {
                isSuccess = result,
                message = "Successful Delete Rule",
                data = new CreateAndUpdateResponse() { Id = Id }
            };
        }
    }
}
