
using DiamondShop.Commons;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.PipelineBehavior
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>>? _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>>? validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators is null || _validators.Any() is false)
            {
                return await next();
            }
            List<Task<ValidationResult>> validateTasks = new();
            foreach (var validator in _validators)
            {
                validateTasks.Add(validator.ValidateAsync(request, otp => { }, cancellationToken));
            }
            ValidationResult[] validationResults = await Task.WhenAll(validateTasks);
            var validationFailure = validationResults
                .Where(result => result.IsValid is false)
                .SelectMany(result => result.Errors)
                .ToList();
            
            Dictionary<string, object> validationErrors= new();

            if (validationFailure is not null && validationFailure.Count > 0)
            {

                validationFailure
                    .ForEach(input =>
                        validationErrors.Add( input.PropertyName,input.ErrorMessage )
                        ) ;

                ValidationError validationError = new ValidationError($"validation error ",validationErrors);
                return (dynamic)validationError;
            }
            else
            {
                return await next();
            }

        }
    }
}
