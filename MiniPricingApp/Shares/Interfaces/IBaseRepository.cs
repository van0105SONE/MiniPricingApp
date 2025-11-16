using MiniPricingApp.Modules.qoutes.Domain.Entities;

namespace MiniPricingApp.Shares.Interfaces
{
    public interface IBaseRepository<T>
    {
        public T GetById(Guid Id);
        public List<T> GetMany();

        public Task<Guid> Save(T entity);

        public Task<bool> SaveMany(List<T> entity);

        public Task<Guid> Update(T entity);

        public Task<bool> Detele(Guid Id);
        public Task<bool> UpdateMany(List<T> entity);
    }
}
