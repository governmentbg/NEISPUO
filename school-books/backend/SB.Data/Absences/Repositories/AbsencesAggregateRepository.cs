namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SB.Common;
using SB.Domain;

internal class AbsencesAggregateRepository : ScopedAggregateRepository<Absence>, IAbsencesAggregateRepository
{
    public AbsencesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<Absence[]> FindAllUnexcusedForPeriodForInstWithoutExtProviderAsync(
        int[] schoolYears,
        int personId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken ct)
    {
        if (schoolYears.Length == 0)
        {
            return Array.Empty<Absence>();
        }

        var predicate = PredicateBuilder.True<Absence>();
        if (schoolYears.Length == 1)
        {
            predicate = predicate.And(a => a.SchoolYear == schoolYears[0]);
        }
        else
        {
            predicate = predicate.And(
                a => this.DbContext.MakeIdsQuery(schoolYears).Any(id => a.SchoolYear == id.Id));
        }

        predicate = predicate.And(
            a => a.PersonId == personId &&
                a.Date >= fromDate &&
                a.Date <= toDate &&
                a.HisMedicalNoticeId == null &&
                a.Type == AbsenceType.Unexcused &&
                (from cb in this.DbContext.Set<ClassBook>()
                    join cbep in this.DbContext.Set<ClassBookExtProvider>()
                    on new { cb.SchoolYear, cb.InstId }
                    equals new { cbep.SchoolYear, cbep.InstId }
                    where cb.SchoolYear == a.SchoolYear &&
                        cb.ClassBookId == a.ClassBookId &&
                        cbep.ExtSystemId != null
                    select 1).Any() == false);

        return await this.FindEntitiesAsync(
            predicate,
            ct);
    }

    public async Task<Absence[]> FindAllByIdsAsync(
        int schoolYear,
        int[] absenceIds,
        CancellationToken ct)
    {
        return await this.FindEntitiesAsync(
            a => a.SchoolYear == schoolYear && this.DbContext.MakeIdsQuery(absenceIds).Any(id => a.AbsenceId == id.Id),
            ct);
    }
}
