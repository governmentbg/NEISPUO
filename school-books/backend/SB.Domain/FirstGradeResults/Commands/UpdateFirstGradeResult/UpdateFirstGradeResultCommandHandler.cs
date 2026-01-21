namespace SB.Domain;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateFirstGradeResultCommandHandler(
    IUnitOfWork UnitOfWork,
    IFirstGradeResultsAggregateRepository FirstGradeResultAggregateRepository,
    IFirstGradeResultsQueryRepository FirstGradeResultsQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateFirstGradeResultCommand>
{
    public async Task Handle(UpdateFirstGradeResultCommand command, CancellationToken ct)
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

        var firstGradeResults = await this.FirstGradeResultAggregateRepository.FindAllByClassBookAsync(command.SchoolYear!.Value, command.ClassBookId!.Value, ct);
        var firstGradeResultsDict = firstGradeResults.ToDictionary(fgr => fgr.PersonId);

        foreach (var student in command.Students!)
        {
            if (firstGradeResultsDict.TryGetValue(student.PersonId!.Value, out var firstGradeResult))
            {
                if (student.QualitativeGrade.HasValue || student.SpecialGrade.HasValue)
                {
                    firstGradeResult.UpdateData(student.QualitativeGrade, student.SpecialGrade, command.SysUserId!.Value);
                }
                else
                {
                    this.FirstGradeResultAggregateRepository.Remove(firstGradeResult);
                }
            }
            else
            {
                if (student.QualitativeGrade.HasValue || student.SpecialGrade.HasValue)
                {
                    if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                            command.SchoolYear!.Value,
                            command.ClassBookId!.Value,
                            student.PersonId.Value,
                            ct))
                    {
                        throw new DomainValidationException($"This person ({student.PersonId.Value}) is not in the class book students list");
                    }

                    var newFirstGradeResult = new FirstGradeResult(
                        command.SchoolYear!.Value,
                        student.PersonId.Value,
                        command.ClassBookId!.Value,
                        student.QualitativeGrade,
                        student.SpecialGrade,
                        command.SysUserId!.Value);
                    await this.FirstGradeResultAggregateRepository.AddAsync(newFirstGradeResult, ct);
                }
            }
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
