namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateClassBookStudentSpecialNeedCurriculumsCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ClassBook> ClassBookAggregateRepository,
    IStudentClassQueryRepository StudentClassQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateClassBookStudentSpecialNeedCurriculumsCommand>
{
    public async Task Handle(UpdateClassBookStudentSpecialNeedCurriculumsCommand command, CancellationToken ct)
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

        var curriculumIds = command.SpecialNeedCurriculumIds ?? Array.Empty<int>();
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

        classBook.UpdateStudentSpecialNeeds(
            command.PersonId!.Value,
            curriculumIds,
            command.HasSpecialNeedFirstGradeResult ?? false,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
