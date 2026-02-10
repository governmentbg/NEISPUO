namespace SB.Domain.Tests.Mediatr.Behaviours;

using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Xunit;
using Moq;
using FluentValidation.Results;

public class ValidationBehaviourTests
{
    public class TestCommand : IRequest<int>
    {
    }

    [Fact]
    public async Task Should_throw_DomainValidationException_with_all_validation_errors()
    {
        // Setup
        var validator1 = Mock.Of<IValidator<IRequest<int>>>(
            m => m.ValidateAsync(It.IsAny<ValidationContext<IRequest<int>>>(), default)
                == Task.FromResult(
                    new ValidationResult(new [] { new ValidationFailure("prop1", "error1") { ErrorCode = ValidationExtensions.UserErrorCode } })));
        var validator2 = Mock.Of<IValidator<IRequest<int>>>(
            m => m.ValidateAsync(It.IsAny<ValidationContext<IRequest<int>>>(), default)
                == Task.FromResult(
                    new ValidationResult(new [] { new ValidationFailure("prop2", "error2") })));

        var behaviour = new ValidationBehaviour<IRequest<int>, int>(
            new IValidator<IRequest<int>>[]
            {
                validator1,
                validator2,
            },
            Mock.Of<IServiceProvider>());

        // Act
        var ex = await Record.ExceptionAsync(async () =>
        {
            await behaviour.Handle(new TestCommand(), Mock.Of<RequestHandlerDelegate<int>>(), default);
        });

        // Verify
        Assert.NotNull(ex);
        Assert.IsType<DomainValidationException>(ex);
        Assert.Equal(new [] { "error2" }, ((DomainValidationException)ex).Errors);
        Assert.Equal(new [] { "error1" }, ((DomainValidationException)ex).ErrorMessages);
    }

    [Fact]
    public async Task When_no_errors_should_call_next_delegate()
    {
        // Setup
        var behaviour = new ValidationBehaviour<IRequest<int>, int>(
            Array.Empty<IValidator<IRequest<int>>>(),
            Mock.Of<IServiceProvider>());

        var next = Mock.Of<RequestHandlerDelegate<int>>(
            m => m() == Task.FromResult(0));

        // Act
        await behaviour.Handle(new TestCommand(), next, default);

        // Verify
        Mock.Get(next).Verify(next => next(), Times.Once);
    }
}
