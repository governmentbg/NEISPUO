namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IStudentPublicationsQueryRepository;

internal class StudentPublicationsQueryRepository : Repository, IStudentPublicationsQueryRepository
{
    private readonly BlobPublicUrlCreator blobPublicUrlCreator;

    public StudentPublicationsQueryRepository(
        UnitOfWork unitOfWork,
        BlobPublicUrlCreator blobPublicUrlCreator)
        : base(unitOfWork)
    {
        this.blobPublicUrlCreator = blobPublicUrlCreator;
    }

    public async Task<TableResultVO<GetStudentPublicationsVO>> GetStudentPublicationsAsync(
        int schoolYear,
        int instId,
        bool archived,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Publication>()
            .And(p => p.SchoolYear == schoolYear)
            .And(p => p.InstId == instId)
            .And(p => p.Date <= DateTime.Now)
            .AndEquals(p => p.Type, PublicationType.Public);

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
                    HasAttachedFiles = p.Files.Any()
                })
            .OrderByDescending(p => p.Date)
            .ThenByDescending(p => p.PublicationId)
            .ToTableResultAsync(
                p => new GetStudentPublicationsVO(
                    p.PublicationId,
                    p.Date,
                    p.Title,
                    p.Content.TruncateMultilinedTextWithEllipsis(500, 4, true),
                    p.HasAttachedFiles),
                offset,
                limit,
                ct);
    }

    public async Task<GetStudentPublicationsMetadataVO> GetStudentPublicationsMetadataAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Publication>()
            .And(p => p.SchoolYear == schoolYear)
            .And(p => p.Date <= DateTime.Now)
            .AndEquals(p => p.Type, PublicationType.Public);

        var result = from sc in this.DbContext.Set<Institution>()
                     join p in this.DbContext.Set<Publication>().Where(predicate) on sc.InstitutionId equals p.InstId into j1
                     from p in j1.DefaultIfEmpty()
                     where sc.InstitutionId == instId
                     select new
                     {
                         p.Status,
                         InstName = sc.Name
                     };

        return await result
            .GroupBy(i => i.InstName)
            .Select(g => new GetStudentPublicationsMetadataVO(
                g.Key,
                g.Sum(p => p.Status == PublicationStatus.Published ? 1 : 0),
                g.Sum(p => p.Status == PublicationStatus.Archived ? 1 : 0)))
            .SingleOrDefaultAsync(ct)
                ?? new GetStudentPublicationsMetadataVO(string.Empty, 0, 0);
    }

    public async Task<GetStudentPublicationVO> GetStudentPublicationAsync(
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
            .Select(p => new GetStudentPublicationVO(
                p.PublicationId,
                p.Date,
                p.Title,
                p.Content,
                this.DbContext.Set<PublicationFile>()
                    .Where(f =>
                        f.SchoolYear == p.SchoolYear &&
                        f.PublicationId == p.PublicationId)
                    .Select(f => new GetStudentPublicationVOFile(
                        f.FileName,
                        this.blobPublicUrlCreator.Create(f.BlobId),
                        Path.GetExtension(f.FileName)))
                    .ToArray()))
            .SingleAsync(ct);
    }
}
