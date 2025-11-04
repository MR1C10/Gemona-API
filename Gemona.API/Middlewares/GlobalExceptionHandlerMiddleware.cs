using System.Net;
using System.Text.Json;
using Gemona.Application.Exceptions;

namespace Gemona.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next, 
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse
            {
                Success = false,
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case NotFoundException notFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = notFoundException.Message;
                    response.Errors = new[] { notFoundException.Message };
                    break;

                case UnauthorizedException unauthorizedException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = unauthorizedException.Message;
                    response.Errors = new[] { unauthorizedException.Message };
                    break;

                case BusinessException businessException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = businessException.Message;
                    response.Errors = new[] { businessException.Message };
                    break;

                case ArgumentException argumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Dados inválidos fornecidos.";
                    response.Errors = new[] { argumentException.Message };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Ocorreu um erro interno no servidor.";
                    
                    // Em desenvolvimento, mostra detalhes. Em produção, oculta.
                    if (_env.IsDevelopment())
                    {
                        response.Errors = new[] { exception.Message, exception.StackTrace ?? "" };
                        response.Details = new
                        {
                            Type = exception.GetType().Name,
                            exception.StackTrace,
                            InnerException = exception.InnerException?.Message
                        };
                    }
                    else
                    {
                        response.Errors = new[] { "Um erro inesperado ocorreu. Por favor, tente novamente mais tarde." };
                    }
                    break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment()
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private class ErrorResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public string[]? Errors { get; set; }
            public string? TraceId { get; set; }
            public object? Details { get; set; }
        }
    }
}
