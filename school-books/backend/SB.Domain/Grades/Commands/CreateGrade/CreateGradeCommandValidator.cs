namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CreateGradeCommandValidator : AbstractValidator<CreateGradeCommand>
{
    public CreateGradeCommandValidator(IValidator<CreateGradeCommandStudent> studentValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.CurriculumId).NotNull().When(c => c.ScheduleLessonId == null);
        this.RuleFor(c => c.Type).NotNull();
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.Students).NotNull();
        this.RuleForEach(c => c.Students).SetValidator(studentValidator);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                if (c.Type != null &&
                    Grade.GradeTypeRequiresScheduleLesson(c.Type.Value) &&
                    c.ScheduleLessonId == null)
                {
                    context.AddUnexpectedFailure($"Schedule lesson should not be null when the type is {c.Type}");
                }

                if (c.ScheduleLessonId != null)
                {
                    var gradesQueryRepository = context.GetServiceProvider().GetRequiredService<IGradesQueryRepository>();

                    var existsVerifiedScheduleLesson = await gradesQueryRepository.ExistsVerifiedScheduleLessonAsync(
                        c.SchoolYear!.Value,
                        c.ScheduleLessonId.Value,
                        ct);

                    if (existsVerifiedScheduleLesson)
                    {
                        context.AddUserFailure($"Не може да се въведе оценка за час, който е проверен от директора.");
                    }
                }
                else
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
                }
            });
    }
}
