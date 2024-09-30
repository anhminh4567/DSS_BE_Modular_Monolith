using DiamondShop.Commons;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiamondShop.Api.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected ActionResult MatchError(List<IError> errors, ModelStateDictionary modelState)
        {
            if (errors.Any(err => err is ValidationError))
            {
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
                }
                return ValidationProblem(modelStateDictionary: modelState, detail: "Validation Error");
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

            return Problem(statusCode: statusCode, title: message);
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
    }
}

