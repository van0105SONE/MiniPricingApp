using MiniPricingApp.Modules.Rules.Application.Dtos;
using MiniPricingApp.Modules.Rules.Application.Services;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Factory;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;
using MiniPricingApp.Tests.Unit.Mock;
using Moq;
using Xunit;

namespace MiniPricingApp.Tests.Unit.Rules.Services
{
    public class PricingRuleServiceTests
    {
        private readonly Mock<IPricingRuleRepository> _mockRuleRepo;
        private readonly Mock<IPricingRuleValidatorFactory> _validatorFactory;
        private readonly Mock<IWeightTierDomainService> _weighTierService;
        private readonly PricingRuleFactory _factory;

        private readonly RuleService _service;

        public PricingRuleServiceTests()
        {
            _mockRuleRepo = new Mock<IPricingRuleRepository>();
            _validatorFactory = new Mock<IPricingRuleValidatorFactory>();
            _weighTierService = new Mock<IWeightTierDomainService>();
            _factory = new PricingRuleFactory(_weighTierService.Object);

            _service = new RuleService(
                _mockRuleRepo.Object,
                _validatorFactory.Object,
                _factory
            );
        }

        [Fact]
        public async Task CreatePricingRule_Success()
        {
            // Arrange
            var request = new PricingRuleRequestDto()
            {
                RuleType = "WeightTier",
                EffectiveFrom = DateTime.MinValue,
                EffectiveTo = DateTime.MaxValue,
                DistcountPercent = 10,
                MaxWeight = 12, // Here is the invalid data
                RemoteAreas = ["A1", "A2", "A3"],
                PricePerTier = 10,
                Surcharge = 10
            };

            var entityMock = new MockWeightTier(21, request.MaxWeight, request.PricePerTier);

            var rules = new List<BasePricingRule>
                {
                    new MockWeightTier(0, 10, addPrice: 10),
                    new MockWeightTier(11, 20, addPrice: 20),
                    new MockTimeWindowPromotion(DateTime.MinValue, DateTime.MaxValue, false, discount: 5),
                    new MockRemoteSurcharge(new List<string> { "A1" }, surcharge: 20)
                };

            Guid newId = Guid.NewGuid();
            _mockRuleRepo.Setup(r => r.GetMany()).Returns(rules);
            _validatorFactory.Setup(r => r.GetValidator(It.IsAny<BasePricingRule>()));
            _mockRuleRepo.Setup(r => r.Save(It.IsAny<BasePricingRule>())).ReturnsAsync(newId);
            // Act
            var result = await _service.CreateRule(request);

            // Assert
            Assert.Equal(newId, result.data.Id);
        }

    }
}
