namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateRemarkExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Remark> RemarkAggregateRepository,
    IRemarksQueryRepository RemarksQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateRemarkExtApiCommand>
{
    public async Task Handle(UpdateRemarkExtApiCommand command, CancellationToken ct)
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

        var remark = await this.RemarkAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.RemarkId!.Value,
            ct);

        if (command.ClassBookId!.Value != remark.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (command.IsExternal == false &&
            (command.CurriculumId == null ||
            command.CurriculumId.Value != remark.CurriculumId))
        {
            // the curriculum check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.CurriculumId)}.");
        }

        remark.UpdateData(
            command.Date!.Value,
            command.Description!,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
