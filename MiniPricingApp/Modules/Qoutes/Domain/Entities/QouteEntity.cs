using CsvHelper;
using Microsoft.AspNetCore.Authentication;
using MiniPricingApp.Shares.Exceptions;
using System.Globalization;

namespace MiniPricingApp.Modules.qoutes.Domain.Entities
{
    public class QouteEntity
    {
        public Guid  Id { get; set; }
        public float Weight { get; set; } 
        public string AreaCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public void validate() {
            if (Weight < 0)
            {
                throw new Exception("Weight cannot be negative");
            }
            
            if (String.IsNullOrEmpty(AreaCode))
            {
                throw new Exception("AreaCode  is required");
            }
        }
    }
}
