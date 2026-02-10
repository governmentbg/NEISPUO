namespace SB.Domain;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateGradeResultExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IGradeResultsAggregateRepository GradeResultAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateGradeResultExtApiCommand, int>
{
    public async Task<int> Handle(CreateGradeResultExtApiCommand command, CancellationToken ct)
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

        var retakeExamCurriculumIds = command.RetakeExamCurriculums!.Select(c => c.CurriculumId!.Value).ToArray();

        if (new HashSet<int>(retakeExamCurriculumIds).Count != retakeExamCurriculumIds.Length)
        {
            throw new DomainValidationException("Duplicated curriculumIds in the retakeExamCurriculums are not allowed.");
        }

        foreach (var curriculumId in retakeExamCurriculumIds)
        {
            if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                    schoolYear,
                    classBookId,
                    curriculumId,
                    ct))
            {
                throw new DomainValidationException($"This curriculum ({curriculumId}) is not in the class book curriculum list");
            }
        }

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                schoolYear,
                classBookId,
                command.PersonId!.Value,
                ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        var gradeResult = new GradeResult(
            schoolYear,
            classBookId,
            command.PersonId!.Value,
            command.InitialResultType!.Value,
            retakeExamCurriculumIds,
            command.FinalResultType,
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

        await this.GradeResultAggregateRepository.AddAsync(gradeResult, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return gradeResult.GradeResultId;
    }
}
