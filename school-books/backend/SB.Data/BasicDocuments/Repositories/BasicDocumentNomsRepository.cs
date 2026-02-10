namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class BasicDocumentNomsRepository : Repository, IBasicDocumentNomsRepository
{
    public BasicDocumentNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int[] ids, RegBookType type, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await (
            from bd in this.GetRegBookTypeBasicDocumentSet(type)
            where this.DbContext.MakeIdsQuery(ids).Any(id => bd.Id == id.Id)
            orderby bd.Id
            select new NomVO(bd.Id, bd.Name))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(string? term, RegBookType type, int? offset, int? limit, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<BasicDocumentVO>();

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(bd => bd.Name, word);
            }
        }

        return await (
            from bd in this.GetRegBookTypeBasicDocumentSet(type).Where(predicate)
            orderby bd.Id
            select new NomVO(bd.Id, bd.Name))
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);
    }

    public record BasicDocumentVO()
    {
        public int Id { get; init; }
        public required string Name { get; init; }
    }
    public IQueryable<BasicDocumentVO> GetRegBookTypeBasicDocumentSet(RegBookType type)
    {
        return type switch
        {
            RegBookType.RegBookQualification =>
                from bd in this.DbContext.Set<RegBookQualificationBasicDocument>()
                select new BasicDocumentVO
                {
                    Id = bd.Id,
                    Name = bd.Name
                },
            RegBookType.RegBookQualificationDuplicate =>
                from bd in this.DbContext.Set<RegBookQualificationDuplicateBasicDocument>()
                select new BasicDocumentVO
                {
                    Id = bd.Id,
                    Name = bd.Name
                },
            RegBookType.RegBookCertificate =>
                from bd in this.DbContext.Set<RegBookCertificateBasicDocument>()
                select new BasicDocumentVO
                {
                    Id = bd.Id,
                    Name = bd.Name
                },
            RegBookType.RegBookCertificateDuplicate =>
                from bd in this.DbContext.Set<RegBookCertificateDuplicateBasicDocument>()
                select new BasicDocumentVO
                {
                    Id = bd.Id,
                    Name = bd.Name
                },
            _ => throw new ArgumentException("This book type is not maintained by the nomenclature."),
        };
    }
}
