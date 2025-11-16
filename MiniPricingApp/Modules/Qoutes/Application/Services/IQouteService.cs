using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniPricingApp.Modules.qoutes.Domain.Entities;
using MiniPricingApp.Modules.Qoutes.Application.Dtos;
using MiniPricingApp.Shares.Common;
using System.Threading.Channels;

namespace MiniPricingApp.Modules.Qoutes.Application.Services
{
    public interface IQouteService
    {
        public BaseResponse<PricingCalculationResponseDto> CalculatePrice(PricingCalculationRequestDto request);

        public Task<BaseResponse<CreateAndUpdateResponse>> SaveFromFile(IFormFile file);

        public Task<BaseResponse<JobEntity>> GetJobById(Guid id);

        public Task<BaseResponse<List<JobEntity>>> GetManyJob();
    }
}
