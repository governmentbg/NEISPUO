
namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IPersonMedicalNoticeQueryRepository;

internal class PersonMedicalNoticeQueryRepository : Repository, IPersonMedicalNoticeQueryRepository
{
    public PersonMedicalNoticeQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllByAbsencesVO[]> GetAllByAbsencesAsync(
        int schoolYear,
        (int personId, DateTime absenceDate)[] absences,
        CancellationToken ct)
    {
        return await (
            from pmn in this.DbContext.Set<PersonMedicalNotice>()
            join hmn in this.DbContext.Set<HisMedicalNotice>()
                on pmn.HisMedicalNoticeId
                equals hmn.HisMedicalNoticeId
            where pmn.SchoolYear == schoolYear &&
                this.DbContext.MakeIdsQuery(absences)
                    .Any(id => pmn.PersonId == id.Id1 && hmn.FromDate <= id.Id2 && hmn.ToDate >= id.Id2)
            select new GetAllByAbsencesVO(
                pmn.PersonId,
                hmn.HisMedicalNoticeId,
                hmn.NrnMedicalNotice,
                hmn.Pmi,
                hmn.AuthoredOn,
                hmn.FromDate,
                hmn.ToDate)
            ).Distinct()
            .ToArrayAsync(ct);
    }

    public async Task<TableResultVO<GetAllByClassBookVO>> GetAllByClassBookAsync(
        int schoolYear,
        int classBookId,
        int? studentPersonId,
        DateTime? fromDate,
        DateTime? toDate,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var personPredicate = PredicateBuilder.True<Person>();
        if (studentPersonId.HasValue)
        {
            personPredicate = personPredicate.And(p => p.PersonId == studentPersonId.Value);
        }

        var hisMedicalNoticePredicate = PredicateBuilder.True<HisMedicalNotice>();
        if (fromDate.HasValue)
        {
            hisMedicalNoticePredicate = hisMedicalNoticePredicate.And(hmn => hmn.FromDate >= fromDate.Value);
        }
        if (toDate.HasValue)
        {
            hisMedicalNoticePredicate = hisMedicalNoticePredicate.And(hmn => hmn.ToDate <= toDate.Value);
        }

        return await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            join p in this.DbContext.Set<Person>().Where(personPredicate)
                on s.PersonId equals p.PersonId
            join pmn in this.DbContext.Set<PersonMedicalNotice>()
                on s.PersonId equals pmn.PersonId
            join hmn in this.DbContext.Set<HisMedicalNotice>().Where(hisMedicalNoticePredicate)
                on pmn.HisMedicalNoticeId equals hmn.HisMedicalNoticeId

            where pmn.SchoolYear == schoolYear

            orderby hmn.AuthoredOn descending

            select new GetAllByClassBookVO(
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                s.IsTransferred ? "ОТПИСАН" : null,
                hmn.NrnMedicalNotice,
                hmn.Pmi,
                hmn.AuthoredOn,
                hmn.FromDate,
                hmn.ToDate)
            )
            .ToTableResultAsync(offset, limit, ct);
    }
}
