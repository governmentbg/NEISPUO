namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IStudentMedicalNoticesQueryRepository;

internal class StudentMedicalNoticesQueryRepository : Repository, IStudentMedicalNoticesQueryRepository
{
    public StudentMedicalNoticesQueryRepository(
        UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllStudentsVO[]> GetAllStudentsAsync(int[] studentPersonIds, CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<Person>()
            where
                this.DbContext
                    .MakeIdsQuery(studentPersonIds)
                    .Any(id => p.PersonId == id.Id)

            orderby p.FirstName, p.MiddleName, p.LastName

            select new GetAllStudentsVO(
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName)
        ).ToArrayAsync(ct);
    }

    public async Task<string> GetStudentNameAsync(int studentPersonId, CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<Person>()
            where p.PersonId == studentPersonId

            select StringUtils.JoinNames (p.FirstName, p.MiddleName, p.LastName)
        ).FirstAsync(ct);
    }

    public async Task<TableResultVO<GetStudentMedicalNoticesVO>> GetStudentMedicalNoticesAsync(
        int schoolYear,
        int personId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var result = (
            from pmn in this.DbContext.Set<PersonMedicalNotice>()
            join hmn in this.DbContext.Set<HisMedicalNotice>() on pmn.HisMedicalNoticeId equals hmn.HisMedicalNoticeId
            where pmn.SchoolYear == schoolYear && pmn.PersonId == personId
            select new
            {
                hmn.HisMedicalNoticeId,
                hmn.NrnMedicalNotice,
                hmn.Pmi,
                hmn.AuthoredOn,
                hmn.FromDate,
                hmn.ToDate
            }).Distinct();

            return await result
            .Select(hmn => new GetStudentMedicalNoticesVO(
                hmn.HisMedicalNoticeId,
                hmn.NrnMedicalNotice,
                hmn.Pmi,
                hmn.AuthoredOn,
                hmn.FromDate,
                hmn.ToDate)
            ).ToTableResultAsync(offset, limit, ct);
    }
}
