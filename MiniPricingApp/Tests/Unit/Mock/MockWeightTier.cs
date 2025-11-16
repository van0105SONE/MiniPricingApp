using MiniPricingApp.Modules.Rules.Domains.Entities;

namespace MiniPricingApp.Tests.Unit.Mock
{
    public class MockWeightTier : WeightTierEntity
    {
        public MockWeightTier(float min, float max, decimal addPrice = 10) : base(Guid.NewGuid(), min, max, addPrice, 0, DateTime.MinValue, DateTime.MaxValue, true)
        {

        }
    }
}
