using System.Net;
using System.Text.Json;
using Domain.Exceptions;

namespace Library.API.Middleware
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
                ValidationException => HttpStatusCode.BadRequest, // 400
                BusinessException => HttpStatusCode.BadRequest,   // 400
                NotFoundException => HttpStatusCode.NotFound,     // 404
                _ => HttpStatusCode.InternalServerError           // 500
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new { message = exception.Message };
            
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}