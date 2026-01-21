namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record CreatePublicationCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Publication> PublicationAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreatePublicationCommand, int>
{
    public async Task<int> Handle(CreatePublicationCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var publication = new Publication(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Type!.Value,
            command.Title!,
            command.Content!,
            command.Date!.Value,
            command.Files?.Select(f => (f.BlobId!.Value, f.FileName!))?.ToArray() ?? Array.Empty<(int, string)>(),
            command.SysUserId!.Value);

        await this.PublicationAggregateRepository.AddAsync(publication, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return publication.PublicationId;
    }
}
