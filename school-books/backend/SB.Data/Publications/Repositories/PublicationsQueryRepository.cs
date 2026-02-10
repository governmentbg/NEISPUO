namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IPublicationsQueryRepository;

internal class PublicationsQueryRepository : Repository, IPublicationsQueryRepository
{
    private readonly BlobPublicUrlCreator blobPublicUrlCreator;

    public PublicationsQueryRepository(
        UnitOfWork unitOfWork,
        BlobPublicUrlCreator blobPublicUrlCreator)
        : base(unitOfWork)
    {
        this.blobPublicUrlCreator = blobPublicUrlCreator;
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await this.DbContext.Set<Publication>()
            .Where(p =>
                p.SchoolYear == schoolYear &&
                p.InstId == instId)
            .OrderByDescending(p => p.Date)
            .ThenByDescending(p => p.PublicationId)
            .ToTableResultAsync(
                p => new GetAllVO(
                    p.PublicationId,
                    p.Type.GetEnumDescription(),
                    p.Status.GetEnumDescription(),
                    p.Date,
                    p.Title,
                    p.Content),
                offset,
                limit,
                ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int publicationId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<Publication>()
            .Where(p =>
                p.SchoolYear == schoolYear &&
                p.InstId == instId &&
                p.PublicationId == publicationId)
            .Select(p => new GetVO(
                p.PublicationId,
                p.Type,
                p.Status,
                p.Date,
                p.Title,
                p.Content,
                this.DbContext.Set<PublicationFile>()
                    .Where(f =>
                        f.SchoolYear == p.SchoolYear &&
                        f.PublicationId == p.PublicationId)
                    .Select(f => new GetVOFile(
                        f.BlobId,
                        f.FileName,
                        this.blobPublicUrlCreator.Create(f.BlobId)))
                    .ToArray()))
            .SingleAsync(ct);
    }

    public async Task<TableResultVO<GetAllPublishedVO>> GetAllPublishedAsync(
        int schoolYear,
        int instId,
        bool archived,
        PublicationType? type,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Publication>()
            .And(p => p.SchoolYear == schoolYear)
            .And(p => p.InstId == instId)
            .And(p => p.Date <= DateTime.Now)
            .AndEquals(p => p.Type, type);

        if (archived)
        {
            predicate = predicate.And(iq => iq.Status == PublicationStatus.Archived);
        }
        else
        {
            predicate = predicate.And(iq => iq.Status == PublicationStatus.Published);
        }

        return await this.DbContext.Set<Publication>()
            .Where(predicate)
            .Select(p =>
                new
                {
                    p.PublicationId,
                    p.Date,
                    p.Title,
                    p.Content,
                    IsInternal = p.Type == PublicationType.Internal,
                    HasAttachedFiles = p.Files.Any()
                })
            .OrderByDescending(p => p.Date)
            .ThenByDescending(p => p.PublicationId)
            .ToTableResultAsync(
                p => new GetAllPublishedVO(
                    p.PublicationId,
                    p.Date,
                    p.Title,
                    p.Content.TruncateMultilinedTextWithEllipsis(500, 4, true),
                    p.IsInternal,
                    p.HasAttachedFiles),
                offset,
                limit,
                ct);
    }

    public async Task<GetMetadataVO> GetMetadataAsync(
        int schoolYear,
        int instId,
        PublicationType? type,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Publication>()
            .And(p => p.SchoolYear == schoolYear)
            .And(p => p.InstId == instId)
            .And(p => p.Date <= DateTime.Now)
            .AndEquals(p => p.Type, type);

        return await this.DbContext.Set<Publication>()
            .Where(predicate)
            .GroupBy(i => 1)
            .Select(g => new GetMetadataVO(
                g.Sum(p => p.Status == PublicationStatus.Published ? 1 : 0),
                g.Sum(p => p.Status == PublicationStatus.Archived ? 1 : 0)))
            .SingleOrDefaultAsync(ct)
                ?? new GetMetadataVO(0, 0);
    }

    public async Task<GetPublishedVO> GetPublishedAsync(
        int schoolYear,
        int instId,
        int publicationId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<Publication>()
            .Where(p =>
                p.SchoolYear == schoolYear &&
                p.InstId == instId &&
                p.Status != PublicationStatus.Draft &&
                p.PublicationId == publicationId)
            .Select(p => new GetPublishedVO(
                p.PublicationId,
                p.Date,
                p.Title,
                p.Content,
                this.DbContext.Set<PublicationFile>()
                    .Where(f =>
                        f.SchoolYear == p.SchoolYear &&
                        f.PublicationId == p.PublicationId)
                    .Select(f => new GetPublishedVOFile(
                        f.FileName,
                        this.blobPublicUrlCreator.Create(f.BlobId),
                        Path.GetExtension(f.FileName)))
                    .ToArray()))
            .SingleAsync(ct);
    }
}
