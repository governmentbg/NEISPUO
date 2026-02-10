namespace SB.Domain;

using FluentValidation;
using FluentValidation.Results;
using System;

public static class ValidationExtensions
{
    public const string UserErrorCode = "USER_ERROR";
    public static void AddUserFailure<T>(this ValidationContext<T> context, string errorMessage)
        => context.AddFailure(
            new ValidationFailure(context.PropertyName, errorMessage)
            {
                ErrorCode = UserErrorCode,
            });

    public const string UnexpectedErrorCode = "UNEXPECTED_ERROR";
    public static void AddUnexpectedFailure<T>(this ValidationContext<T> context, string errorMessage)
        => context.AddFailure(
            new ValidationFailure(context.PropertyName, errorMessage)
            {
                ErrorCode = UnexpectedErrorCode,
            });

    public static IRuleBuilderOptions<T, TProperty> WithUserMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage)
        => rule.WithErrorCode(UserErrorCode).WithMessage(errorMessage);

    private const string ServiceProviderKey = "_SB_ServiceProviderKey";

    public static IServiceProvider GetServiceProvider<T>(this ValidationContext<T> context)
        => (context.RootContextData[ServiceProviderKey] as IServiceProvider)
            ?? throw new InvalidOperationException("ServiceProvider is not set");

    public static void SetServiceProvider<T>(this ValidationContext<T> context, IServiceProvider serviceProvider)
        => context.RootContextData[ServiceProviderKey] = serviceProvider;
}
