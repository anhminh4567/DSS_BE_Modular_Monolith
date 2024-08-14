using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace DiamondShop.Api.Configurations.ProblemErrors
{
    public class DiamonShopProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions _options;

        public DiamonShopProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
        {
            statusCode ??= 500;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,
            };

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);
            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            statusCode ??= 400;

            var problemDetails = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = statusCode,
                Type = type,
                Detail = detail,
                Instance = instance,
            };
            if (title != null)
            {
                problemDetails.Title = title;
            }

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode.Value);
            //List<CustomError> errorList = new();
            //foreach (var modelState in modelStateDictionary)
            //{
            //    var key = modelState.Key;
            //    var errors = modelState.Value.Errors;
            //    foreach (var errorEntry in errors)
            //    {
            //        errorList.Add(new CustomError() { key = key,ErrorDescriptiopn = errorEntry.ErrorMessage});
            //    }
            //}
            //if (errorList != null && errorList.Count > 0)
            //{
            //    problemDetails.Extensions.Add("validationErrors", errorList);
            //}
            return problemDetails;
        }
        private void ApplyProblemDetailsDefaults(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {
            problemDetails.Status ??= statusCode;

            if (_options.ClientErrorMapping.TryGetValue(statusCode, out var clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }

            var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }
            //var errors = httpContext?.Items[HttpContextItemKeys.Errors] as List<Error>;
            //if (errors is not null)
            //{
            //    problemDetails.Extensions.Add("errorCodes", errors.Select(e => e.Code));
            //}
        }

    }
    internal class CustomError
    {
        public string key { get; set; }
        public string ErrorDescriptiopn { get; set; }
    }
}
