using MiniPricingApp.Shares.Common;
using MiniPricingApp.Shares.Exceptions;

namespace MiniPricingApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // call next middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    FileException =>  StatusCodes.Status400BadRequest,
                    CsvFormatException => StatusCodes.Status415UnsupportedMediaType,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                var response = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    ErrorCode = GetErrorCode(ex),
                    Message = GetReadableMessage(ex),
                    Details = _env.IsDevelopment() ? ex.StackTrace : null
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }

        private string GetErrorCode(Exception ex) =>
    ex switch
    {
        FileException => ErrorCodes.FILE_FORMAT_NOT_SUPPORTED,
        CsvFormatException => ErrorCodes.CSV_INVALID_FORMAT,
        KeyNotFoundException => ErrorCodes.NOT_FOUND,
        UnauthorizedAccessException => ErrorCodes.UNAUTHORIZED,
        ArgumentException => ErrorCodes.VALIDATION_ERROR,
        _ => ErrorCodes.INTERNAL_ERROR
    };

        private string GetReadableMessage(Exception ex) =>
            ex switch
            {
                FileException fe => fe.Message,                   // Example: "Only CSV files are allowed."
                CsvFormatException cfe => cfe.Message,            // Example: "Row 5: Invalid number format"
                KeyNotFoundException => "The requested resource was not found.",
                UnauthorizedAccessException => "Unauthorized. Please check your credentials.",
                ArgumentException ae => ae.Message,               // Example: "Weight must be greater than 0"
                Exception ae => ae.Message,
                _ => "An unexpected error occurred."
            };
    }
}
