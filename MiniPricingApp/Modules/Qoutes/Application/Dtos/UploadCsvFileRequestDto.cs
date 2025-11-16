using Microsoft.AspNetCore.Mvc;

namespace MiniPricingApp.Modules.Qoutes.Application.Dtos
{
    public class UploadCsvFileRequestDto
    {
        public IFormFile? file { get; set; }
    }
}
