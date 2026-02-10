namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreatePerformanceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Performance> PerformanceAggregateRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreatePerformanceCommand, int>
{
    public async Task<int> Handle(CreatePerformanceCommand command, CancellationToken ct)
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

        var performance = new Performance(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.PerformanceTypeId!.Value,
            command.Name!,
            command.Description!,
            command.StartDate!.Value,
            command.EndDate!.Value,
            command.Location!,
            command.StudentAwards,
            command.SysUserId!.Value);

        await this.PerformanceAggregateRepository.AddAsync(performance, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return performance.PerformanceId;
    }
}
