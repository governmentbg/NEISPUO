namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ClassBookNomsRepository : Repository, IClassBookNomsRepository
{
    public ClassBookNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int schoolYear, int instId, int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await (
            from cb in this.DbContext.Set<ClassBook>()

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                this.DbContext.MakeIdsQuery(ids)
                    .Any(id => cb.ClassBookId == id.Id)

            orderby cb.FullBookName

            select new NomVO(cb.ClassBookId, cb.FullBookName, cb.IsValid ? null : "Архивиран"))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        bool? showPG,
        bool? showCdo,
        bool? showDplr,
        bool? showCsop,
        bool? showInvalid,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<ClassBook>();
        predicate = predicate.And(cb => cb.SchoolYear == schoolYear && cb.InstId == instId);

        if (showPG == false)
        {
            predicate = predicate.And(sc => sc.BookType != ClassBookType.Book_PG);
        }

        if (showCdo == false)
        {
            predicate = predicate.And(sc => sc.BookType != ClassBookType.Book_CDO);
        }

        if (showDplr == false)
        {
            predicate = predicate.And(sc => sc.BookType != ClassBookType.Book_DPLR);
        }

        if (showCsop == false)
        {
            predicate = predicate.And(sc => sc.BookType != ClassBookType.Book_CSOP);
        }

        if (showInvalid is null or false)
        {
            predicate = predicate.And(sc => sc.IsValid == true);
        }

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(cb => cb.FullBookName, word);
            }
        }

        return await
            this.DbContext.Set<ClassBook>()
            .Where(predicate)
            .OrderBy(cb => cb.BasicClassId)
            .ThenBy(cb => cb.FullBookName)
            .WithOffsetAndLimit(offset, limit)
            .Select(cb => new NomVO(cb.ClassBookId, cb.FullBookName, cb.IsValid ? null : "Архивиран"))
            .ToArrayAsync(ct);
    }
}
