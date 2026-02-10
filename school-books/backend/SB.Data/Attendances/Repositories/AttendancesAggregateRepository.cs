namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SB.Common;
using SB.Domain;

internal class AttendancesAggregateRepository : ScopedAggregateRepository<Attendance>, IAttendancesAggregateRepository
{
    public AttendancesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<Attendance[]> FindAllUnexcusedForPeriodForInstWithoutExtProviderAsync(
        int[] schoolYears,
        int personId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken ct)
    {
        if (schoolYears.Length == 0)
        {
            return Array.Empty<Attendance>();
        }

        var predicate = PredicateBuilder.True<Attendance>();
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
                a.Type == AttendanceType.UnexcusedAbsence &&
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
}
