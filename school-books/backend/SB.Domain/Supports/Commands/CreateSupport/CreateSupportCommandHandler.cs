namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateSupportCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Support> SupportAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateSupportCommand, int>
{
    public async Task<int> Handle(CreateSupportCommand command, CancellationToken ct)
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

        foreach (var studentPersonId in command.StudentIds!)
        {
            if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                    schoolYear,
                    classBookId,
                    studentPersonId,
                    ct))
            {
                throw new DomainValidationException($"This person ({studentPersonId}) is not in the class book students list");
            }
        }

        var support = new Support(
            schoolYear,
            classBookId,
            command.EndDate!.Value,
            command.Description!,
            command.ExpectedResult!,
            command.SysUserId!.Value,
            command.StudentIds!,
            command.TeacherIds!, // TODO check teachers belong to institution
            command.SupportDifficultyTypeIds!);

        await this.SupportAggregateRepository.AddAsync(support, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return support.SupportId;
    }
}
