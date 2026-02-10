namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateClassBookStudentGradelessCurriculumsCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IStudentClassQueryRepository StudentClassQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateClassBookStudentGradelessCurriculumsCommand>
{
    public async Task Handle(UpdateClassBookStudentGradelessCurriculumsCommand command, CancellationToken ct)
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
            (command.GradelessCurriculums ?? Array.Empty<UpdateClassBookStudentGradelessCurriculumsCommandGradeless>())
            .Where(gc => gc.WithoutFirstTermGrade || gc.WithoutSecondTermGrade || gc.WithoutFinalGrade)
            .Select(gc => gc.CurriculumId)
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

        classBook.UpdateStudentGradeless(
            command.PersonId!.Value,
            (command.GradelessCurriculums ?? Array.Empty<UpdateClassBookStudentGradelessCurriculumsCommandGradeless>())
                    .Where(gc => gc.WithoutFirstTermGrade || gc.WithoutSecondTermGrade || gc.WithoutFinalGrade)
                    .Select(gc => (
                        curriculumId: gc.CurriculumId,
                        withoutFirstTermGrade: gc.WithoutFirstTermGrade,
                        withoutSecondTermGrade: gc.WithoutSecondTermGrade,
                        withoutFinalGrade: gc.WithoutFinalGrade))
                    .ToArray(),
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
