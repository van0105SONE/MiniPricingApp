using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.qoutes.Domain.Enums;
using MiniPricingApp.Shares.Interfaces;

namespace MiniPricingApp.Modules.qoutes.Infrastructure.Repository.intefaces
{
    public interface IJobRepository : IBaseRepository<JobEntity>
    {
        public List<JobEntity> GetJobByStatus(JobStatus status);
        public List<JobEntity> GetJobUnComleteJob();
    }
}
