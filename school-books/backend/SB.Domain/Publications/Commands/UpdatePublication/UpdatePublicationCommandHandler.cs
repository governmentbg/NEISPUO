namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record UpdatePublicationCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Publication> PublicationAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdatePublicationCommand, int>
{
    public async Task<int> Handle(UpdatePublicationCommand command, CancellationToken ct)
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

        publication.Update(
            command.Type!.Value,
            command.Title!,
            command.Content!,
            command.Date!.Value,
            command.Files?.Select(f => (f.BlobId!.Value, f.FileName!))?.ToArray() ?? Array.Empty<(int, string)>(),
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        return publication.PublicationId;
    }
}
