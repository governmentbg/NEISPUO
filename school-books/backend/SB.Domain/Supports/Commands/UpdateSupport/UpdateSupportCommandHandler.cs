namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateSupportCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Support> SupportAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateSupportCommand, int>
{
    public async Task<int> Handle(UpdateSupportCommand command, CancellationToken ct)
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

        var support = await this.SupportAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SupportId!.Value,
            ct);

        if (command.ClassBookId!.Value != support.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        var supportStudentIds = support.Students.Select(s => s.PersonId).ToArray();

        // the ones in the Support.Students have already been checked so we'll skip them
        foreach (var studentPersonId in command.StudentIds!.Except(supportStudentIds))
        {
            if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    studentPersonId,
                    ct))
            {
                throw new DomainValidationException($"This person ({studentPersonId}) is not in the class book students list");
            }
        }

        support.Update(
            command.EndDate!.Value,
            command.Description,
            command.ExpectedResult,
            command.StudentIds!,
            command.TeacherIds!, // TODO check teachers belong to institution
            command.SupportDifficultyTypeIds!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);

        return support.SupportId;
    }
}
