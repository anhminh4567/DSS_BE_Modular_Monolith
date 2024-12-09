using DiamondShop.Commons;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace DiamondShop.Api.Controllers
{
    //[ProducesResponseType(typeof(ValidationProblemDetailsDocumentation), 400)]
    //[ProducesResponseType(typeof(ProblemDetailDocumentation), 500)]

    public class ApiControllerBase : ControllerBase
    {
        protected ActionResult MatchError(List<IError> errors, ModelStateDictionary modelState)
        {
            if (errors.Any(err => err is ValidationError))
            {
                string message = "Validation error";
                foreach (var error in errors)
                {
                    var metaError = error.Metadata;
                    foreach (var errAtt in metaError)
                    {
                        var messageList = errAtt.Value as List<object>;
                        foreach(var errMess in messageList)
                            modelState.AddModelError(errAtt.Key, (string)errMess);
                        //modelState.AddModelError(errAtt.Key,(string)errAtt.Value);
                    }
                    var firstError = metaError.FirstOrDefault();
                    if (firstError.Equals(default(KeyValuePair<string, object>)) == false)
                    {
                        // Handle the case where firstError is not the default value
                        var messageList = firstError.Value as List<object>;
                        var firstMessage = messageList.FirstOrDefault();
                        if (firstMessage != null)
                            message = (string)firstMessage;

                    }
                }
                return ValidationProblem(modelStateDictionary: modelState, detail: message );// errors.First(x => x is ValidationError).Message
            }
            return Problem(errors.First());
        }
        private ActionResult Problem(IError error)
        {
            (int statusCode, string message) = error switch
            {
                ConflictError => (StatusCodes.Status409Conflict,error.Message),
                ValidationError => (StatusCodes.Status400BadRequest,error.Message),
                NotFoundError => (StatusCodes.Status404NotFound,error.Message),
                _ => (StatusCodes.Status400BadRequest, error.Message),
            };

            return Problem(statusCode: statusCode, detail: message);
        }

        //private IActionResult ValidationProblem(List<IError> errors)
        //{
        //    var modelStateDictionary = new ModelStateDictionary();

        //    foreach (var error in errors)
        //    {
        //        var metaError = error.Metadata;
        //        foreach (var errAtt in metaError)
        //        {
        //            modelStateDictionary.AddModelError(errAtt.Key, (string)errAtt.Value);
        //        }
        //    }
        //    return ValidationProblem(modelStateDictionary: modelStateDictionary);
        //}
        private class ProblemDetailDocumentation
        {
            public string? type { get; set; }
            public string? title { get; set; }
            public int? status { get; set; }
            public string? detail { get; set; }
            public string? instance { get; set; }
        }
        private class ValidationProblemDetailsDocumentation : ProblemDetailDocumentation
        {
            [JsonPropertyName("errors")]
            public IDictionary<string, string[]> Errors { get; set; }
        }
    }
}

