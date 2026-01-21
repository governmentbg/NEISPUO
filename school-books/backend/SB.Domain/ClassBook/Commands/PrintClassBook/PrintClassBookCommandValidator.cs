namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class PrintClassBookCommandValidator : AbstractValidator<PrintClassBookCommand>
{
    public PrintClassBookCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(s => s.SchoolYear).NotNull();
        this.RuleFor(s => s.InstId).NotNull();
        this.RuleFor(s => s.ClassBookId).NotNull();
        this.RuleFor(s => s.SysUserId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var classBooksAdminQueryRepository = context.GetServiceProvider().GetRequiredService<IClassBooksAdminQueryRepository>();

                if (await classBooksAdminQueryRepository.HasPendingPrintAsync(
                    c.SchoolYear!.Value,
                    c.ClassBookId!.Value,
                    ct))
                {
                    context.AddUserFailure("Дневника е в процес на принтиране. Моля, изчакайте да приключи, за да свалите pdf файла.");
                }
            });
    }
}
