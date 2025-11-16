using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.qoutes.Domain.Enums;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.intefaces;
using System.Text.Json;

namespace MiniPricingApp.Modules.qoutes.Infrastructure.Repository.Implemtation
{
    public class JobRepository : IJobRepository
    {
        private String FILE_NAME = "job_track_data.json";
        private readonly IWebHostEnvironment _env;

        public JobRepository(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task<bool> Detele(Guid Id)
        {
            throw new NotImplementedException();
        }

        public JobEntity GetById(Guid id)
        {
            var jobTrackDataPath = Path.Combine(
            _env.ContentRootPath,
            "Modules",
            "Qoutes",
            "Infrastructure",
            FILE_NAME
            );
            var json = File.ReadAllText(jobTrackDataPath);
            var jobs = JsonSerializer.Deserialize<List<JobEntity>>(json);

            return jobs.FirstOrDefault(t => t.Id == id);
        }


        public List<JobEntity> GetMany() {
                    var jobTrackDataPath = Path.Combine(
                  _env.ContentRootPath,
                  "Modules",
                  "Qoutes",
                  "Infrastructure",
                  FILE_NAME
              );
            var json = File.ReadAllText(jobTrackDataPath);
            var jobs = JsonSerializer.Deserialize<List<JobEntity>>(json);

            return jobs;
        }

        public async Task<Guid> Save(JobEntity entity)
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
            var jobs = JsonSerializer.Deserialize<List<JobEntity>>(json);

            jobs.Add(entity);

            var UpdatedJson = JsonSerializer.Serialize(
                jobs,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(path, UpdatedJson);

            return entity.Id; // or whatever ID you generate
        }

        public Task<bool> SaveMany(List<JobEntity> entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> Update(JobEntity entity)
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
            var jobs = JsonSerializer.Deserialize<List<JobEntity>>(json);

            JobEntity existing = jobs.FirstOrDefault(t => t.Id == entity.Id);
            if (existing == null)
                throw new Exception($"Qoute with Id {entity.Id} not found.");

            existing.Status = entity.Status;
            existing.FilePath = entity.FilePath;

            var UpdatedJson = JsonSerializer.Serialize(
                jobs,
                new JsonSerializerOptions { WriteIndented = true }
            );

            await File.WriteAllTextAsync(path, UpdatedJson);

            return entity.Id; // or whatever ID you generate
        }

        public Task<bool> UpdateMany(List<JobEntity> entity)
        {
            throw new NotImplementedException();
        }

        public List<JobEntity> GetJobByStatus(JobStatus status)
        {
            var jobTrackDataPath = Path.Combine(
              _env.ContentRootPath,
              "Modules",
              "Qoutes",
              "Infrastructure",
               FILE_NAME
              );
            var json = File.ReadAllText(jobTrackDataPath);
            var jobs = JsonSerializer.Deserialize<List<JobEntity>>(json);

            return jobs.Where(t => t.Status == status).ToList();
        }

        public List<JobEntity> GetJobUnComleteJob()
        {
            var jobTrackDataPath = Path.Combine(
                 _env.ContentRootPath,
                 "Modules",
                 "Qoutes",
                 "Infrastructure",
                  FILE_NAME
                 );
            var json = File.ReadAllText(jobTrackDataPath);
            var jobs = JsonSerializer.Deserialize<List<JobEntity>>(json);

            return jobs.Where(t => t.Status != JobStatus.COMPLETE).OrderBy(t => t.Status).ToList();
        }
    }
}
