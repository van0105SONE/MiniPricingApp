using FluentAssertions;
using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.intefaces;
using MiniPricingApp.Modules.Qoutes.Application.Dtos;
using MiniPricingApp.Modules.Qoutes.Application.Services;
using MiniPricingApp.Modules.Qoutes.Domain.Interfaces;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;
using MiniPricingApp.Tests.Unit.Mock;
using Moq;
using Xunit;

namespace MiniPricingApp.Tests.Unit.Qoutes.Service
{
    public class QouteServiceTests
    {
        private readonly Mock<IPricingRuleRepository> _mockRuleRepo;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<IJobRepository> _mockJobRepo;
        private readonly Mock<IQouteWeightTierValidator> _mockQoutedValidator;
        private readonly Mock<IQouteCsvValidator> _csvValidator;

        private readonly QouteService _service;

        public QouteServiceTests()
        {
            _mockRuleRepo = new Mock<IPricingRuleRepository>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockJobRepo = new Mock<IJobRepository>();
            _mockQoutedValidator = new Mock<IQouteWeightTierValidator>();
            _csvValidator = new Mock<IQouteCsvValidator>();
            _service = new QouteService(
                _mockRuleRepo.Object,
                _mockEnv.Object,
                _mockJobRepo.Object,
                _mockQoutedValidator.Object,
                _csvValidator.Object
            );
        }

        [Fact]
        public void CalculatePrice_AppliesRulesCorrectly()
        {
            // Arrange
            var request = new PricingCalculationRequestDto()
            {
                weight = 12, // Here is the invalid data
                areaCode = "A1"
            };

            var rules = new List<BasePricingRule>
        {
            new MockWeightTier(0, 10, addPrice: 10),
            new MockWeightTier(11, 20, addPrice: 20),
            new MockTimeWindowPromotion(DateTime.MinValue, DateTime.MaxValue, false, discount: 5),
            new MockRemoteSurcharge(new List<string> { "A1" }, surcharge: 20)
        };

            _mockRuleRepo.Setup(r => r.GetMany()).Returns(rules);
        
            // Act
            var result = _service.CalculatePrice(request);

            // Assert
            Assert.Equal(40, result.data.finalPrice);
        }



        [Fact]
        public void CalculatePrice_WithWeightTierAndTimeWindowPromotion()
        {
            // Arrange
            var request = new PricingCalculationRequestDto()
            {
                weight = 9, // Here is the invalid data
                areaCode = "A13"
            };

            DateTime now = DateTime.Now;
            DateTime nextHour = now.AddHours(1); // Adds 1 hour

        var rules = new List<BasePricingRule>
        {
            new MockWeightTier(0, 10, addPrice: 10),
            new MockWeightTier(11, 20, addPrice: 20),
            new MockTimeWindowPromotion(DateTime.Now, nextHour, true, discount: 5),
            new MockRemoteSurcharge(new List<string> { "A1" }, surcharge: 20)
        };

            _mockRuleRepo.Setup(r => r.GetMany()).Returns(rules);

            // Act
            var result = _service.CalculatePrice(request);

            // Assert
            Assert.Equal(9.5, result.data.finalPrice);
        }


        [Fact]
        public void CalculatePrice_WithWeightTier_TimeWindowPromotion_RemoteSurCharge()
        {
            // Arrange
            var request = new PricingCalculationRequestDto()
            {
                weight = 9, // Here is the invalid data
                areaCode = "A1"
            };

            DateTime now = DateTime.Now;
            DateTime nextHour = now.AddHours(1); // Adds 1 hour

            var rules = new List<BasePricingRule>
        {
            new MockWeightTier(0, 10, addPrice: 10),
            new MockWeightTier(11, 20, addPrice: 20),
            new MockTimeWindowPromotion(DateTime.Now, nextHour, true, discount: 5),
            new MockRemoteSurcharge(new List<string> { "A1" }, surcharge: 20)
        };

            _mockRuleRepo.Setup(r => r.GetMany()).Returns(rules);

            // Act
            var result = _service.CalculatePrice(request);

            // Assert
            Assert.Equal(29.5, result.data.finalPrice);
        }


        [Fact]
        public void CalculatePrice_AppliesRulesInvalid()
        {
            // Arrange
            var request = new PricingCalculationRequestDto()
            {
                weight = -1, // Here is the invalid data
                areaCode = "A1"
            };

            
            _mockQoutedValidator.Setup(v => v.Validate(It.IsAny<QouteEntity>()))
            .Throws(new Exception("Weight cannot be negative"));

            var ex = Assert.Throws<Exception>(() =>
                    _service.CalculatePrice(request)
                );

            // You can also check the exception message to be more specific
            Assert.Contains("Weight cannot be negative", ex.Message);
        }


        [Fact]
        public void CalculatePrice_AppliesRulesInvalidAreaCode()
        {
            // Arrange
            var request = new PricingCalculationRequestDto()
            {
                weight = 0, // Here is the invalid data
                areaCode = ""
            };

            _mockQoutedValidator.Setup(v => v.Validate(It.IsAny<QouteEntity>())).Throws(new Exception("AreaCode  is required"));

            var ex = Assert.Throws<Exception>(() =>
                    _service.CalculatePrice(request)
                );

            // You can also check the exception message to be more specific
            Assert.Contains("AreaCode  is required", ex.Message);
        }
    }
}
