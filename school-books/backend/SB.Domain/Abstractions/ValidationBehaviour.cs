namespace SB.Domain;

using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> validators;
    private readonly IServiceProvider serviceProvider;

    public ValidationBehaviour(
        IEnumerable<IValidator<TRequest>> validators,
        IServiceProvider serviceProvider)
    {
        this.validators = validators;
        this.serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (this.validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            context.SetServiceProvider(this.serviceProvider);

            var validationResults = await Task.WhenAll(
                this.validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToArray();

            if (failures.Length != 0)
            {
                throw new DomainValidationException(
                    failures.Where(f => f.ErrorCode != ValidationExtensions.UserErrorCode).Select(f => f.ErrorMessage).ToArray(),
                    failures.Where(f => f.ErrorCode == ValidationExtensions.UserErrorCode).Select(f => f.ErrorMessage).ToArray());
            }
        }
        return await next();
    }
}
