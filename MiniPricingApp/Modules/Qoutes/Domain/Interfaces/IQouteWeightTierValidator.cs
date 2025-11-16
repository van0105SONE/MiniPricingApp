using MiniPricingApp.Modules.qoutes.Domain.Entities;

namespace MiniPricingApp.Modules.Qoutes.Domain.Interfaces
{
    public interface IQouteWeightTierValidator
    {
        void Validate(QouteEntity entity);
    }
}
