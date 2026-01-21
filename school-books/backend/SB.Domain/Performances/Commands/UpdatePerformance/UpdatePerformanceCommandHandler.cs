namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdatePerformanceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Performance> SanctionAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdatePerformanceCommand>
{
    public async Task Handle(UpdatePerformanceCommand command, CancellationToken ct)
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

        var performance = await this.SanctionAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.PerformanceId!.Value,
            ct);

        if (command.ClassBookId!.Value != performance.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        performance.UpdateData(
            command.PerformanceTypeId!.Value,
            command.Name!,
            command.Description!,
            command.StartDate!.Value,
            command.EndDate!.Value,
            command.Location!,
            command.StudentAwards,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
