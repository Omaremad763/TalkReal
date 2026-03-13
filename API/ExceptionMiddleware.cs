namespace API
{
    using System.Net;
    using System.Text.Json;

    using GlobalApiResponse;

    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the system");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return Task.CompletedTask;
            }
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Failure([]);

            switch (exception)
            {
                case FluentValidation.ValidationException validationResult:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Errors = [.. validationResult.Errors.Select(e => e.ErrorMessage)];
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Errors = [exception.Message];
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Errors = [exception.Message];
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

    }
}
