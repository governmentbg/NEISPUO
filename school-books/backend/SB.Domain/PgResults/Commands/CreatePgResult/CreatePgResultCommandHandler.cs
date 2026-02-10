namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreatePgResultCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<PgResult> PgResultAggregateRepository,
    IPgResultsQueryRepository PgResultsQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreatePgResultCommand, int>
{
    public async Task<int> Handle(CreatePgResultCommand command, CancellationToken ct)
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
        int? subjectId = command.SubjectId
            ?? (command.CurriculumId.HasValue
                ? await this.PgResultsQueryRepository.GetSubjectIdForCurriculumAsync(
                    command.CurriculumId!.Value,
                    ct)
                : null);

        var pgResultExists = await this.PgResultsQueryRepository.ExistsForPersonAndSubjectAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            subjectId,
            ct);

        if(pgResultExists)
        {
            if (subjectId.HasValue)
            {
                throw new DomainValidationException(new [] { "pg_result_exists" }, new [] { "Вече има въведени резултати от предучилищната подготовка за това дете и образователно направление." });
            }
            else
            {
                throw new DomainValidationException(new [] { "pg_result_exists" }, new [] { "Вече има въведени резултати от предучилищната подготовка без посочено образователно направление за това дете." });
            }
        }

        if (subjectId.HasValue &&
            !await this.ClassBookCachedQueryStore.ExistsSubjectForClassBookAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                subjectId.Value,
                ct))
        {
            throw new DomainValidationException($"This subject is not in any of the class book's curriculum");
        }

        var pgResult = new PgResult(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PersonId!.Value,
            subjectId,
            command.CurriculumId,
            command.StartSchoolYearResult!,
            command.EndSchoolYearResult!,
            command.SysUserId!.Value);

        await this.PgResultAggregateRepository.AddAsync(pgResult, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return pgResult.PgResultId;
    }
}
