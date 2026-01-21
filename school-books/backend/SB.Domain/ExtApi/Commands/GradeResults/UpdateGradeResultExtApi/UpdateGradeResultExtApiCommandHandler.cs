namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateGradeResultExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IGradeResultsAggregateRepository GradeResultAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateGradeResultExtApiCommand>
{
    public async Task Handle(UpdateGradeResultExtApiCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{schoolYear}, classBookId:{classBookId}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var gradeResult = await this.GradeResultAggregateRepository
            .FindAsync(schoolYear, command.GradeResultId!.Value, ct);

        if (command.ClassBookId!.Value != gradeResult.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        foreach (var retakeExam in command.RetakeExamCurriculums!)
        {
            if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                    schoolYear,
                    classBookId,
                    retakeExam.CurriculumId!.Value,
                    ct))
            {
                throw new DomainValidationException($"This curriculum ({retakeExam.CurriculumId}) is not in class book curriculum list");
            }
        }

        var retakeExamCurriculumIds = command.RetakeExamCurriculums!.Select(c => c.CurriculumId!.Value).ToArray();

        gradeResult.UpdateData(
            command.InitialResultType!.Value,
            retakeExamCurriculumIds,
            command.FinalResultType,
            true,
            command.SysUserId!.Value);

        foreach (var retakeExam in command.RetakeExamCurriculums!)
        {
            gradeResult.SetSubjectResult(
                retakeExam.CurriculumId!.Value,
                retakeExam.Session1Grade,
                retakeExam.Session1NoShow,
                retakeExam.Session2Grade,
                retakeExam.Session2NoShow,
                retakeExam.Session3Grade,
                retakeExam.Session3NoShow,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
