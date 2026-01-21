namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemovePublicationCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Publication> PublicationAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemovePublicationCommand>
{
    public async Task Handle(RemovePublicationCommand command, CancellationToken ct)
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

        if (publication.Status != PublicationStatus.Draft)
        {
            throw new DomainException("You can remove only drafts");
        }

        this.PublicationAggregateRepository.Remove(publication);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
