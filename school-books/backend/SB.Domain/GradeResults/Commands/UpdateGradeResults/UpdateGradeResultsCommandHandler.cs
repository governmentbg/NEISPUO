namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateGradeResultsCommandHandler(
    IUnitOfWork UnitOfWork,
    IGradeResultsAggregateRepository GradeResultAggregateRepository,
    IGradeResultsQueryRepository GradeResultsQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateGradeResultsCommand>
{
    public async Task Handle(UpdateGradeResultsCommand command, CancellationToken ct)
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

        var gradeResults = (await this.GradeResultAggregateRepository.FindAllByClassBookIdAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
            .ToDictionary(gr => gr.PersonId, gr => gr);

        foreach (var s in command.Students!)
        {
            if (gradeResults.TryGetValue(s.PersonId!.Value, out var gradeResult))
            {
                if (s.InitialResultType.HasValue)
                {
                    var gradeResultCurriculumIds =
                        gradeResult.GradeResultSubjects
                        .Select(s => s.CurriculumId)
                        .ToArray();

                    foreach (var curriculumId in s.RetakeExamCurriculumIds.Except(gradeResultCurriculumIds))
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

                    gradeResult.UpdateData(
                        s.InitialResultType!.Value,
                        s.RetakeExamCurriculumIds,
                        s.FinalResultType,
                        false,
                        command.SysUserId!.Value);
                }
                else
                {
                    this.GradeResultAggregateRepository.Remove(gradeResult);
                }
            }
            else if (s.InitialResultType != null)
            {
                if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                        command.SchoolYear!.Value,
                        command.ClassBookId!.Value,
                        s.PersonId!.Value,
                        ct))
                {
                    throw new DomainValidationException($"This person ({s.PersonId!.Value}) is not in the class book students list");
                }

                foreach (var curriculumId in s.RetakeExamCurriculumIds)
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

                var newGradeResult = new GradeResult(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    s.PersonId!.Value,
                    s.InitialResultType.Value,
                    s.RetakeExamCurriculumIds,
                    s.FinalResultType,
                    command.SysUserId!.Value);

                await this.GradeResultAggregateRepository.AddAsync(newGradeResult, ct);
            }
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
