namespace MiniPricingApp.Shares.Common
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; } // optional for dev mode
    }
}
