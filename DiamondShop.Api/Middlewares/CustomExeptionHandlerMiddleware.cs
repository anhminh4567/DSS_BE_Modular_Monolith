using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DiamondShop.Api.Middlewares
{
    public class CustomExeptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<CustomExeptionHandlerMiddleware> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public CustomExeptionHandlerMiddleware(ILogger<CustomExeptionHandlerMiddleware> logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError("exception catched in middleware with messager: {message}", ex.Message);
                _logger.LogError(ex.StackTrace);
                var problemDetailResponse = _problemDetailsFactory.CreateProblemDetails(context, StatusCodes.Status500InternalServerError, detail: ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(problemDetailResponse);
            }
        }
    }
}
