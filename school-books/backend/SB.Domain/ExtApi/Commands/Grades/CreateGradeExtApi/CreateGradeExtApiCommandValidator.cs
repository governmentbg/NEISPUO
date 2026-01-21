namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

public class CreateGradeExtApiCommandValidator : AbstractValidator<CreateGradeExtApiCommand>
{
    public CreateGradeExtApiCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.PersonId).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull().When(c => c.ScheduleLessonId == null);
        this.RuleFor(c => c.Date).NotEmpty();
        this.RuleFor(c => c.Type).NotEmpty().IsInEnum();
        this.RuleFor(c => c.Category).NotEmpty().IsInEnum();
        this.RuleFor(c => c.ScheduleLessonId).NotNull()
            .When(c => c.Type != null && Grade.GradeTypeRequiresScheduleLesson(c.Type.Value));
        this.RuleFor(c => c.Comment).MaximumLength(1000);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                if (c.ScheduleLessonId == null)
                {
                    var classBooksAdminQueryRepository = context.GetServiceProvider().GetRequiredService<IClassBooksAdminQueryRepository>();

                    var classBookInfo = await classBooksAdminQueryRepository.GetInfoAsync(
                        c.SchoolYear!.Value,
                        c.ClassBookId!.Value,
                        ct);

                    if (classBookInfo.BookType == ClassBookType.Book_I_III)
                    {
                        if (c.Type == GradeType.Term || c.Type == GradeType.OtherClassTerm || c.Type == GradeType.OtherSchoolTerm)
                        {
                            context.AddUserFailure($"Не може да се въведе срочна оценка за ученик, който е I - III клас.");
                        }
                        else if (classBookInfo.BasicClassId == 1 && c.Type == GradeType.Final)
                        {
                            context.AddUserFailure($"Не може да се въведе годишна оценка за ученик, който е I клас.");
                        }
                    }

                    if (c.Type == GradeType.Final)
                    {
                        var gradesQueryRepository = context.GetServiceProvider().GetRequiredService<IGradesQueryRepository>();

                        var existsFinalGrade = await gradesQueryRepository.ExistsFinalGradeForStudentAsync(
                            c.SchoolYear!.Value,
                            c.ClassBookId!.Value,
                            c.PersonId!.Value,
                            c.CurriculumId!.Value,
                            ct);

                        if (existsFinalGrade)
                        {
                            context.AddUserFailure($"Ученикът вече има въведена годишна оценка.");
                        }
                    }

                    if (c.Type == GradeType.Term || c.Type == GradeType.OtherClassTerm || c.Type == GradeType.OtherSchoolTerm)
                    {
                        var gradesQueryRepository = context.GetServiceProvider().GetRequiredService<IGradesQueryRepository>();
                        var classBookCachedQueryStore = context.GetServiceProvider().GetRequiredService<IClassBookCachedQueryStore>();

                        var term = c.Term ??
                            await classBookCachedQueryStore.GetTermForDateAsync(
                                c.SchoolYear!.Value,
                                c.ClassBookId!.Value,
                                c.Date!.Value, ct);

                        var existsTermGrade = await gradesQueryRepository.ExistsTermGradeForStudentAsync(
                            c.SchoolYear!.Value,
                            c.ClassBookId!.Value,
                            c.PersonId!.Value,
                            c.CurriculumId!.Value,
                            term,
                            ct);

                        if (existsTermGrade)
                        {
                            context.AddUserFailure($"Ученикът вече има въведена срочна оценка оценка за {EnumUtils.GetEnumDescription(term).ToLower()}.");
                        }
                    }
                }
            });
    }
}
