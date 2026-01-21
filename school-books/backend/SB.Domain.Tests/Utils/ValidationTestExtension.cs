namespace SB.Domain.Tests;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.TestHelper;

public static class ValidationTestExtension
{
    public static void ShouldHaveChildValidator<T, TProperty>(this IValidator<T> validator, Expression<Func<T, IEnumerable<TProperty>>> expression)
        => validator.ShouldHaveChildValidator(expression, typeof(IValidator<TProperty>));
}
