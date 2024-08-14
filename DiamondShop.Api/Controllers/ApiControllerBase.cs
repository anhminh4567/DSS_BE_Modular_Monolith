using DiamondShop.Commons;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiamondShop.Api.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected ActionResult MatchError(List<IError> errors, ModelStateDictionary modelState, string message = "Error")
        {
            if (errors.Any(err => err is ValidationError))
            {
                foreach (var error in errors)
                {
                    var metaError = error.Metadata;
                    foreach (var errAtt in metaError)
                    {
                        modelState.AddModelError(errAtt.Key,(string)errAtt.Value);
                    }
                }
                return ValidationProblem(modelStateDictionary: modelState, detail: message);
            }
            return Problem(message);
        }
        //private ActionResult Problem(IError error)
        //{
        //    var statusCode = error.GetType() switch
        //    {
        //        typeof(ConflictError) => StatusCodes.Status409Conflict,
        //        typeof(ValidationError) => StatusCodes.Status400BadRequest,
        //        typeof(NotFoundError) => StatusCodes.Status404NotFound,
        //        _ => StatusCodes.Status500InternalServerError,
        //    };

        //    return Problem(statusCode: statusCode, title: error.Description);
        //}

        private IActionResult ValidationProblem(List<IError> errors)
        {
            var modelStateDictionary = new ModelStateDictionary();

            foreach (var error in errors)
            {
                var metaError = error.Metadata;
                foreach (var errAtt in metaError)
                {
                    modelStateDictionary.AddModelError(errAtt.Key, (string)errAtt.Value);
                }
            }
            return ValidationProblem(modelStateDictionary: modelStateDictionary);
        }
    }
}

