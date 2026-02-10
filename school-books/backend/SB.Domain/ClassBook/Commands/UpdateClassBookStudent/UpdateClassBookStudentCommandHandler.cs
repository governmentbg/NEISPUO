namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SB.Common;

internal record UpdateClassBookStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IStudentClassQueryRepository StudentClassQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateClassBookStudentCommand>
{
    public async Task Handle(UpdateClassBookStudentCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        var curriculumIds =
            command.GradelessCurriculums!
            .Where(gc => gc.WithoutFirstTermGrade!.Value || gc.WithoutSecondTermGrade!.Value || gc.WithoutFinalGrade!.Value)
            .Select(gc => gc.CurriculumId!.Value)
            .ToArray();
        foreach (var curriculumId in curriculumIds)
        {
            if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                curriculumId,
                ct))
            {
                throw new DomainValidationException($"This curriculum ({curriculumId}) is not in the class book curriculum list");
            }
        }

        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct);

        classBook.UpdateStudent(
            command.PersonId!.Value,
            command.GradelessCurriculums!
                .Where(gc => gc.WithoutFirstTermGrade!.Value || gc.WithoutSecondTermGrade!.Value || gc.WithoutFinalGrade!.Value)
                .Select(gc => (
                    curriculumId: gc.CurriculumId!.Value,
                    withoutFirstTermGrade: gc.WithoutFirstTermGrade!.Value,
                    withoutSecondTermGrade: gc.WithoutSecondTermGrade!.Value,
                    withoutFinalGrade: gc.WithoutFinalGrade!.Value))
                .ToArray(),
            command.SpecialNeedCurriculumIds!,
            command.HasSpecialNeedFirstGradeResult ?? false,
            command.Activities,
            (
                command.CarriedAbsences?.FirstTermExcusedCount ?? 0,
                command.CarriedAbsences?.FirstTermUnexcusedCount ?? 0,
                command.CarriedAbsences?.FirstTermLateCount ?? 0,
                command.CarriedAbsences?.SecondTermExcusedCount ?? 0,
                command.CarriedAbsences?.SecondTermUnexcusedCount ?? 0,
                command.CarriedAbsences?.SecondTermLateCount ?? 0
            ),
            command.SysUserId!.Value);

        var studentClasses = await this.StudentClassQueryRepository.FindAllByClassBookAndPersonIdAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            ct);

        foreach (var sc in studentClasses)
        {
            sc.SetClassNumber(command.ClassNumber);
        }

        await this.UnitOfWork.SaveWithRetryingStrategyAsync(
            new []
            {
                // see https://dba.stackexchange.com/questions/211467/error-updating-temporal-tables
                SqlServerErrorCodes.DataModificationFailedOnSystemVersionedTable
            },
            ct);
    }
}
