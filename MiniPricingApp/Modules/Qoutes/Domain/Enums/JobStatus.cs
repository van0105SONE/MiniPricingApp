using System.Text.Json.Serialization;

namespace MiniPricingApp.Modules.qoutes.Domain.Enums
{
    public enum JobStatus
    {
        [JsonPropertyName("PENDING")]
        PENDING,

        [JsonPropertyName("INPROGRESS")]
        INPROGRESS,

        [JsonPropertyName("COMPLETE")]
        COMPLETE
    }
}
