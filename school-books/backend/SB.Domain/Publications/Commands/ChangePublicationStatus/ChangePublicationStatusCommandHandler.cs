namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record ChangePublicationStatusCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Publication> PublicationAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<ChangePublicationStatusCommand>
{
    public async Task Handle(ChangePublicationStatusCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var publication = await this.PublicationAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.PublicationId!.Value,
            ct);

        publication.ChangeStatus(command.Status!.Value, command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
