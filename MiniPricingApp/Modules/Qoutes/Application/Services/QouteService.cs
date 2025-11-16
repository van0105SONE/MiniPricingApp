using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.qoutes.Domain.Enums;
using MiniPricingApp.Modules.qoutes.Infrastructure.Repository.intefaces;
using MiniPricingApp.Modules.Qoutes.Application.Dtos;
using MiniPricingApp.Modules.Qoutes.Domain.Interfaces;
using MiniPricingApp.Modules.Rules.Domains.Entities;
using MiniPricingApp.Modules.Rules.Domains.Interface;
using MiniPricingApp.Modules.Rules.Infrastructure.Interface;
using MiniPricingApp.Shares.Common;
using MiniPricingApp.Shares.Exceptions;
using System.Text.Json;
using System.Threading.Channels;

namespace MiniPricingApp.Modules.Qoutes.Application.Services
{
    /// <summary>
    /// Application service responsible for handling all operations related to quotes,
    /// including price calculation, job management, and CSV-based bulk import.
    /// <para/>
    /// This service acts as the primary application layer orchestrator:
    /// 1. Interacts with domain validators and repositories.
    /// 2. Ensures quotes are validated against business rules (WeightTiers, promotions, surcharges).
    /// 3. Manages CSV imports and persists jobs for asynchronous processing.
    /// </summary>
    public class QouteService : IQouteService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IPricingRuleRepository _ruleRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IQouteWeightTierValidator _weightTierValidator;
        private readonly IQouteCsvValidator _csvValidator;
        public QouteService(IPricingRuleRepository ruleRepository, IWebHostEnvironment env, IJobRepository jobRepository, IQouteWeightTierValidator weightTierValidator, IQouteCsvValidator csvValidator)
        {
            _ruleRepository = ruleRepository;
            _env = env;
            _jobRepository = jobRepository;
            _weightTierValidator = weightTierValidator;
            _csvValidator = csvValidator;
        }

        /// <summary>
        /// Calculates the final price for a quote based on weight, area, and pricing rules.
        /// </summary>
        /// <param name="request">The pricing calculation request DTO containing weight and area code.</param>
        /// <returns>A response containing the calculated price.</returns>
        public BaseResponse<PricingCalculationResponseDto> CalculatePrice(PricingCalculationRequestDto request)
        {

            // Step 1: Create a new Quote entity from the request
            // - The entity holds the core data for this quote
            // - Id is generated here to uniquely identify this quote
            QouteEntity entity = new QouteEntity()
            {
                Id = Guid.NewGuid(),
                Weight = request.weight,
                AreaCode = request.areaCode
            };

            // Step 2: Validate the quote's weight against defined WeightTier rules
            // - This ensures the quote can be priced based on available tiers
            // - Throws exception if no valid WeightTier exists
            _weightTierValidator.Validate(entity);

            // Step 3: Load all pricing rules from the repository
            // - Only load once to avoid multiple database hits
            // - Rules include things like discounts, surcharges, or promotional adjustments
            List<BasePricingRule> rules = _ruleRepository.GetMany();

            // Step 4: Initialize the price accumulator
            decimal price = 0;

            // Step 5: Apply all rules in order of priority
            // - Priority determines the sequence rules are applied
            // - Each rule can modify the current price
            foreach (var rule in rules.OrderBy(r => r.Priority))
            {
                if (rule.IsApplicable(entity))
                {
                    price = rule.Apply(price, entity);
                }
            }

            return new BaseResponse<PricingCalculationResponseDto>() { 
               isSuccess = true,
               code = "SUCCESS",
               message = "Your ship price",
               data = new PricingCalculationResponseDto() { finalPrice = price }
            };

        }






        /// <summary>
        /// Saves quotes from an uploaded CSV file. 
        /// Background Task will work in 1 minute after you completed uploaded, please wait for 1 minute and check again
        /// <para/>
        /// Steps:
        /// 1. Validate file existence and extension (.csv only).
        /// 2. Save file to a dedicated folder under the application root.
        /// 3. Validate CSV format and content using <see cref="IQouteCsvValidator"/>.
        /// 4. Persist a job entity for tracking processing.
        /// 5. If validation fails, the uploaded file is deleted to prevent orphaned files.
        /// </summary>
        /// <param name="file">The uploaded CSV file.</param>
        /// <returns>A <see cref="BaseResponse{Guid}"/> containing the new job Id.</returns>
        /// <exception cref="FileException">Thrown if the file is empty or not a CSV.</exception>
        /// <exception cref="CsvFormatException">Thrown if CSV validation fails.</exception>
        public async Task<BaseResponse<CreateAndUpdateResponse>> SaveFromFile(IFormFile file)
        {

            if (file == null || file.Length == 0)
                throw new FileException("File is empty.");

            var extension = Path.GetExtension(file.FileName);
            if (!string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
                throw new FileException("Only CSV files are allowed.");

            // Build folder path
            var folderPath = Path.Combine(_env.ContentRootPath, "Modules", "Qoutes", "Infrastructure", "UploadedCsv");

            // Ensure folder exists
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

           //
            Guid newId = Guid.NewGuid();


            // Build file path
            var filePath = Path.Combine(folderPath, $"{newId}_{file.FileName}");

            // Save file to folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            try
            {
                await _csvValidator.Validate(filePath);

                Guid result = await _jobRepository.Save(new JobEntity() { Id = newId, Status = JobStatus.PENDING, FilePath = filePath });
                return new BaseResponse<CreateAndUpdateResponse>()
                {
                    isSuccess = true,
                    code = "SUCCESS",
                    message = "Successful imported csv file",
                    data = new CreateAndUpdateResponse() { Id = result }

                };
            }catch(Exception ex)
            {
                // Remove file if validation fails
                // delete file that background  task unable to to work with,because it waste of server storage
                if (File.Exists(filePath))
                    File.Delete(filePath);
                
                throw new CsvFormatException(ex.Message);
            }

        }

        /// <summary>
        /// Retrieves a job by its unique identifier.
        /// </summary>
        /// <param name="id">The job's unique identifier.</param>
        /// <returns>A <see cref="BaseResponse{JobEntity}"/> containing the job and status.</returns>
        public async Task<BaseResponse<JobEntity>> GetJobById(Guid id)
        {
            JobEntity? job = _jobRepository.GetById(id);
            return new BaseResponse<JobEntity>()
            {
                isSuccess = true,
                message = "Current job status",
                data = job,
            };

        }


        public async Task<BaseResponse<List<JobEntity>>> GetManyJob()
        {
            var jobs = _jobRepository.GetMany();
            return new BaseResponse<List<JobEntity>>()
            {
                isSuccess = true,
                code = "SUCCESS",
                message = "Get all jobs",
                data = jobs

            };
        }


    }
}
