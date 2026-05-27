using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var errors = validationResults.SelectMany(result => result.Errors)
            .Where(error => error != null)
            .Select(error => error.ErrorMessage)
            .Distinct()
            .ToList();

        if (errors.Count == 0)
        {
            return await next();
        }

        var responseType = typeof(TResponse);
        if (responseType == typeof(ApiResponse))
        {
            return (TResponse)(object)ApiResponse.ValidationFail(errors);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
        {
            var method = responseType.GetMethod("ValidationFail", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                return (TResponse)method.Invoke(null, new object[] { errors })!;
            }
        }

        throw new InvalidOperationException("ValidationBehavior can only be used with ApiResponse return types.");
    }
}
