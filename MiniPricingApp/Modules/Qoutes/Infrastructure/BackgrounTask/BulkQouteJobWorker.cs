
using CsvHelper;
using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.qoutes.Domain.Enums;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.Implemtation;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.intefaces;
using MiniPricingApp.Modules.Qoutes.Application.Dtos;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Channels;
using System.Xml;

namespace MiniPricingApp.Modules.qoutes.Infrastructure.BackgrounTask
{
    public class BulkQouteJobWorker : BackgroundService
    {
        private readonly ILogger<BulkQouteJobWorker> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IServiceScopeFactory _scopeFactory;

        public BulkQouteJobWorker(
            ILogger<BulkQouteJobWorker> logger,
            IWebHostEnvironment env,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _env = env;
            _scopeFactory = scopeFactory;

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {



                    using var scope = _scopeFactory.CreateScope();
                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                    var qouteRepository = scope.ServiceProvider.GetRequiredService<IQouteRepository>();

                    var jobs = jobRepository.GetJobUnComleteJob();
                    foreach (var job in jobs)
                    {
                        _logger.LogInformation($"START WORKING WITH JOB ==> {job.Id}");
                        job.Status = JobStatus.INPROGRESS;
                        await jobRepository.Update(job);
                        using var reader = new StreamReader(job.FilePath);
                        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                        List<QouteDto> qoutes = csv.GetRecords<QouteDto>().ToList();

                        foreach (QouteDto qoute in qoutes)
                        {
                            await qouteRepository.Save(new QouteEntity() { Id = Guid.NewGuid(), Weight = qoute.Weight, AreaCode = qoute.AreaCode });
                        }

                        job.Status = JobStatus.COMPLETE;
                        _logger.LogInformation($"COMPLETE WORKING WITH JOB ==> {job.Id}");
                        await jobRepository.Update(job);

                    }





                }
                catch (Exception ex)
                {
                    _logger.LogError($"Bulk Backgroud Error {ex.Message}");
                }

                // Wait 1 minute before next iteration
                await Task.Delay(60_000, stoppingToken);
            }
        }
    }
}
