using System.Net;
using System.Text.Json;

namespace miskAssisment.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = exception switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                statusCode = context.Response.StatusCode,
                message = exception.Message,
                details = statusCode == HttpStatusCode.InternalServerError ? "An unexpected system fault occurred." : null
            };

            var jsonResult = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(jsonResult);
        }
    }
}