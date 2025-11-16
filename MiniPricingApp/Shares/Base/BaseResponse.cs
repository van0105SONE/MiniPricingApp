namespace MiniPricingApp.Shares.Common
{
    public class BaseResponse<T>
    {
       public   bool isSuccess { get; set; }
       public   string code { get; set; }
       public  string message { get; set; }
        
       public  T  data { get; set; }
    }
}
