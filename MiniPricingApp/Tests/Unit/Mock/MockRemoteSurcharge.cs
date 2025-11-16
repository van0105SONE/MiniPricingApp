using MiniPricingApp.Modules.Rules.Domains.Entities;

namespace MiniPricingApp.Tests.Unit.Mock
{
    public class MockRemoteSurcharge : RemoteAreaSurcharge
    {
            public MockRemoteSurcharge(
                List<string> areas = null,
                decimal surcharge = 20,
                int priority = 2
            ) : base(
                id: Guid.NewGuid(),
                remoteAreas: areas ?? new List<string> { "A1", "B2" },
                surcharge: surcharge,
                priority: priority,
                effectiveFrom: DateTime.MinValue,
                effectiveTo: DateTime.MaxValue,
                isActive: true
            )
            {
                // Optionally override or initialize anything else here
            }
        }
}
