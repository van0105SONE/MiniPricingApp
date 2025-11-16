using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.intefaces;
using System.Text.Json;

namespace MiniPricingApp.Modules.qoutes.Infrastructure.Repository.Implemtation
{
    public class QouteRepository : IQouteRepository
    {
        private String FILE_NAME = "qoutes_data.json";
        private readonly IWebHostEnvironment _env;

        public QouteRepository(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task<bool> Detele(Guid Id)
        {
            throw new NotImplementedException();
        }

        public QouteEntity GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<QouteEntity> GetMany()
        {
            var path = Path.Combine(
                _env.ContentRootPath,
                "Modules",
                "Qoutes",
                "Infrastructure",
                 FILE_NAME
            );


            var json = File.ReadAllText(path);
            var qoutes = JsonSerializer.Deserialize<List<QouteEntity>>(json);

            return qoutes;
        }

        public async Task<Guid> Save(QouteEntity entity)
        {
            var path = Path.Combine(
                  _env.ContentRootPath,
                  "Modules",
                  "Qoutes",
                  "Infrastructure",
                   FILE_NAME
              );
            // Ensure file exists
            if (!File.Exists(path))
            {
                await File.WriteAllTextAsync(path, "[]"); // create empty JSON array
            }

            var json = File.ReadAllText(path);
            var qoutes = JsonSerializer.Deserialize<List<QouteEntity>>(json);

            qoutes.Add(entity);

            var UpdatedJson = JsonSerializer.Serialize(
                qoutes,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(path, UpdatedJson);

            return entity.Id; // or whatever ID you generate
        }

        public Task<bool> SaveMany(List<QouteEntity> entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> Update(QouteEntity entity)
        {
            var path = Path.Combine(
                _env.ContentRootPath,
                "Modules",
                "Qoutes",
                "Infrastructure",
                FILE_NAME
            );
            // Ensure file exists
            if (!File.Exists(path))
            {
                await File.WriteAllTextAsync(path, "[]"); // create empty JSON array
            }

            var json = File.ReadAllText(path);
            var qoutes = JsonSerializer.Deserialize<List<QouteEntity>>(json);

            var qoute = qoutes.FirstOrDefault(t => t.Id == entity.Id);
            qoute.Weight = entity.Weight;
            qoute.AreaCode = entity.AreaCode;

            var UpdatedJson = JsonSerializer.Serialize(
                qoutes,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(path, UpdatedJson);

            return qoute.Id;
        }

        public Task<bool> UpdateMany(List<QouteEntity> entity)
        {
            throw new NotImplementedException();
        }
    }
}
