using LautusInformatica.DTOs;
using LautusInformatica.Exceptions;

namespace LautusInformatica.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string errorMessage = $"{exception.GetType().Name}: {exception.Message}";

            if (exception is AppException appEx)
            {
                statusCode = appEx.StatusCode;
                errorMessage = appEx.ErrorMessage;
            }

            context.Response.StatusCode = statusCode;

            var responseObj = new ApiResponse<string>
            {
                Success = false,
                Message = errorMessage,
                Data = null
            };
            var payload = System.Text.Json.JsonSerializer.Serialize(responseObj);

            return context.Response.WriteAsync(payload);
        }
    }
}
