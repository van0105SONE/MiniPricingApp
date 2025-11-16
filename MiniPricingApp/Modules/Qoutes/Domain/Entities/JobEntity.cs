using CsvHelper;
using MiniPricingApp.Modules.qoutes.Domain.Enums;
using MiniPricingApp.Shares.Exceptions;
using System.Globalization;

namespace MiniPricingApp.Modules.qoutes.Domain.Entities
{
    public class JobEntity
    {
        public Guid Id { get; set; }
        public JobStatus Status { get; set; }
        public string FilePath { get; set; }


    }
}
