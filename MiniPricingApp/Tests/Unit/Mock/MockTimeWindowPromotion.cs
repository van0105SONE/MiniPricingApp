using MiniPricingApp.Modules.Rules.Domains.Entities;

namespace MiniPricingApp.Tests.Unit.Mock
{
    public class MockTimeWindowPromotion : TimeWindowPromotion
    {
        public MockTimeWindowPromotion(DateTime effectiveFrom, DateTime effectiveTo,bool isActive = false, decimal discount = 5) : base(Guid.NewGuid(), discount,1, effectiveFrom, effectiveTo, isActive)
        {

        }
    }
}
