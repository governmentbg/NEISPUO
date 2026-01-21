namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Data.IInstTeacherNomsRepository;

internal class InstTeacherNomsRepository : Repository, IInstTeacherNomsRepository
{
    public InstTeacherNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<InstTeacherNomVO[]> GetNomsByIdAsync(int instId, int[] ids, CancellationToken ct)
{
    if (!ids.Any())
    {
        throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
    }

    var idSet = ids.ToHashSet(); // fast lookup & EF generates IN (...)

    return await this.DbContext.Set<StaffPosition>()
        .Where(sp =>
            sp.InstitutionId == instId &&
            idSet.Contains(sp.PersonId)
        )
        .GroupBy(sp => sp.PersonId)
        .Select(g => new
        {
            PersonId = g.Key,
            IsValid = g.Any(sp => sp.IsValid)
        })
        .Join(this.DbContext.Set<Person>(),
            sp => sp.PersonId,
            p => p.PersonId,
            (sp, p) => new
            {
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                sp.IsValid
            })
        .OrderBy(p => p.FirstName)
        .ThenBy(p => p.MiddleName)
        .ThenBy(p => p.LastName)
        .Select(p => new InstTeacherNomVO(
            p.PersonId,
            StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
            p.FirstName,
            p.LastName,
            !p.IsValid ? "НЕАКТИВЕН" : null
        ))
        .ToArrayAsync(ct);
}

    public async Task<InstTeacherNomVO[]> GetNomsByTermAsync(
        int instId,
        int schoolYear,
        string? term,
        bool? includeNonPedagogical,
        bool? includeNotActiveTeachers,
        bool? includeNoReplacementTeachers,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var staffPositionPredicate = PredicateBuilder.True<StaffPosition>();
        var personPredicate = PredicateBuilder.True<Person>();

        staffPositionPredicate = staffPositionPredicate.AndEquals(sp => sp.InstitutionId, instId);

        if (includeNonPedagogical == null || includeNonPedagogical == false)
        {
            staffPositionPredicate =
                staffPositionPredicate.AndEquals(sp => sp.StaffTypeId, StaffPosition.PedagogicalStaffTypeId);
        }

        if (includeNotActiveTeachers == true)
        {
            // Get teachers who are currently valid or have termination date and topics in selected year
            var startOfSchoolYear = new DateTime(schoolYear, 9, 1);
            staffPositionPredicate = staffPositionPredicate.And(sp =>
                sp.IsValid ||
                (sp.TerminationDate > startOfSchoolYear &&
                 this.DbContext.Set<TopicTeacher>()
                     .Any(tt => tt.PersonId == sp.PersonId &&
                                tt.SchoolYear == schoolYear)));
        }
        else if (includeNoReplacementTeachers == true)
        {
            staffPositionPredicate = staffPositionPredicate.And(sp =>
                sp.IsValid ||
                this.DbContext.Set<CurriculumTeacher>()
                    .Any(cTeacher => cTeacher.SchoolYear == schoolYear &&
                                     cTeacher.StaffPositionId == sp.StaffPositionId &&
                                     cTeacher.NoReplacement == true));
        }
        else
        {
            staffPositionPredicate = staffPositionPredicate.AndEquals(sp => sp.IsValid, true);
        }

        if (!string.IsNullOrWhiteSpace(term))
        {
            var words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                personPredicate =
                    personPredicate.AndAnyStringContains(p => p.FirstName, p => p.MiddleName, p => p.LastName, word);
            }
        }

        return await (
            from sp in this.DbContext.Set<StaffPosition>().Where(staffPositionPredicate)

            join p in this.DbContext.Set<Person>().Where(personPredicate) on sp.PersonId equals p.PersonId

            select  new
            {
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                sp.IsValid
            })
            .Distinct()
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.MiddleName)
            .ThenBy(p => p.LastName)
            .Select(p => new InstTeacherNomVO(
                p.PersonId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                p.FirstName,
                p.LastName,
                !p.IsValid ? "НЕАКТИВЕН" : null
            ))
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);
    }
}
