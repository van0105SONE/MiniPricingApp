namespace MiniPricingApp.Modules.Rules.Domains.Interface
{
    public interface IWeightTierDomainService
    {
        float GetNextMinWeight();
        bool ExistsTierForWeight(float weight);
    }
}
