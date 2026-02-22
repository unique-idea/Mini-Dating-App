using System.Net;

namespace Mini_Dating_App_BE.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorException = new ErrorException() { TimeStamp = DateTime.UtcNow, Message = exception.Message };

            switch (exception)
            {
                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorException.StatusCode = (int)HttpStatusCode.NotFound;
                    _logger.LogInformation(exception.Message);
                    break;
                case BadHttpRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorException.StatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogInformation(exception.Message);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorException.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(exception.ToString());
                    break;
            }

            var result = errorException.ToString();
            await context.Response.WriteAsJsonAsync(errorException);
        }
    }
}
