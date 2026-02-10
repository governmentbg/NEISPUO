namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IRemarksQueryRepository;

internal class RemarksQueryRepository : Repository, IRemarksQueryRepository
{
    public RemarksQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllForClassBookVO[]> GetAllForClassBookAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await
            (from r in this.DbContext.Set<Remark>()
             where r.SchoolYear == schoolYear && r.ClassBookId == classBookId
             select new
             {
                 r.PersonId,
                 r.Type
             })
             .GroupBy(g => g.PersonId)
             .Select(r => new GetAllForClassBookVO(
                 r.Key,
                 r.Count(x => x.Type == RemarkType.Bad),
                 r.Count(y => y.Type == RemarkType.Good)))
             .ToArrayAsync(ct);
    }

    public async Task<GetAllForStudentAndTypeVO[]> GetAllForStudentAndTypeAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        RemarkType type,
        CancellationToken ct)
    {
        var curriculumReplTeacherPersonIds = await
            (from sch in this.DbContext.Set<Schedule>()

                join l in this.DbContext.Set<ScheduleLesson>()
                on new { sch.SchoolYear, sch.ScheduleId }
                equals new { l.SchoolYear, l.ScheduleId }

                join tah in this.DbContext.Set<TeacherAbsenceHour>()
                on new { l.SchoolYear, l.ScheduleLessonId }
                equals new { tah.SchoolYear, tah.ScheduleLessonId }

                where sch.SchoolYear == schoolYear
                    && sch.ClassBookId == classBookId
                    && tah.ReplTeacherPersonId != null
                    && tah.ReplTeacherIsNonSpecialist == false

                select new { l.CurriculumId, tah.ReplTeacherPersonId })
            .Distinct()
            .ToArrayAsync(ct);

        var replTeacherPersonIdByCurriculum =
            curriculumReplTeacherPersonIds
            .ToLookup(tah => tah.CurriculumId, tah => tah.ReplTeacherPersonId!.Value);

        var remarks = await (
            from r in this.DbContext.Set<Remark>()

            join c in this.DbContext.Set<Curriculum>() on r.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join csu in this.DbContext.Set<SysUser>() on r.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            join msu in this.DbContext.Set<SysUser>() on r.ModifiedBySysUserId equals msu.SysUserId

            join mp in this.DbContext.Set<Person>() on msu.PersonId equals mp.PersonId

            join t in this.DbContext.Set<CurriculumTeacher>().Where(t => t.IsValid)
            on r.CurriculumId equals t.CurriculumId
            into j1 from t in j1.DefaultIfEmpty()

            join sp in this.DbContext.Set<StaffPosition>()
            on t.StaffPositionId equals sp.StaffPositionId
            into j2 from sp in j2.DefaultIfEmpty()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId &&
                r.PersonId == studentPersonId &&
                r.Type == type

            select new
            {
                r.RemarkId,
                r.CurriculumId,
                r.PersonId,
                s.SubjectName,
                s.SubjectNameShort,
                SubjectTypeName = st.Name,
                r.Date,
                r.Type,
                TypeName = EnumUtils.GetEnumDescription(r.Type),
                r.Description,
                r.IsReadFromParent,
                r.CreateDate,
                r.CreatedBySysUserId,
                CreatedBySysUserFirstName = cp.FirstName,
                CreatedBySysUserMiddleName = cp.MiddleName,
                CreatedBySysUserLastName = cp.LastName,
                r.ModifyDate,
                r.ModifiedBySysUserId,
                ModifiedBySysUserFirstName = mp.FirstName,
                ModifiedBySysUserMiddleName = mp.MiddleName,
                ModifiedBySysUserLastName = mp.LastName,
                WriteAccessCurriculumTeacherPersonId = (int?)sp.PersonId
            })
            .ToArrayAsync(ct);

        return remarks
            .GroupBy(r =>
                new
                {
                    r.RemarkId,
                    r.CurriculumId,
                    r.PersonId,
                    r.SubjectName,
                    r.SubjectNameShort,
                    r.SubjectTypeName,
                    r.Date,
                    r.Type,
                    r.TypeName,
                    r.Description,
                    r.IsReadFromParent,
                    r.CreateDate,
                    r.CreatedBySysUserId,
                    r.CreatedBySysUserFirstName,
                    r.CreatedBySysUserMiddleName,
                    r.CreatedBySysUserLastName,
                    r.ModifyDate,
                    r.ModifiedBySysUserId,
                    r.ModifiedBySysUserFirstName,
                    r.ModifiedBySysUserMiddleName,
                    r.ModifiedBySysUserLastName,
                })
            .OrderByDescending(g => g.Key.Date)
            .ThenByDescending(g => g.Key.CreateDate)
            .Select(g => new GetAllForStudentAndTypeVO(
                g.Key.RemarkId,
                g.Key.CurriculumId,
                g.Key.PersonId,
                g.Key.SubjectName,
                g.Key.SubjectNameShort,
                g.Key.SubjectTypeName,
                g.Key.Date,
                g.Key.Type,
                g.Key.TypeName,
                g.Key.Description,
                g.Key.IsReadFromParent,
                g.Key.CreateDate,
                g.Key.CreatedBySysUserId,
                g.Key.CreatedBySysUserFirstName,
                g.Key.CreatedBySysUserMiddleName,
                g.Key.CreatedBySysUserLastName,
                g.Key.ModifyDate,
                g.Key.ModifiedBySysUserId,
                g.Key.ModifiedBySysUserFirstName,
                g.Key.ModifiedBySysUserMiddleName,
                g.Key.ModifiedBySysUserLastName,
                g.Select(gi => gi.WriteAccessCurriculumTeacherPersonId)
                    .Where(pId => pId.HasValue)
                    .Select(pId => pId!.Value)
                    .ToArray(),
                replTeacherPersonIdByCurriculum[g.Key.CurriculumId].ToArray()))
        .ToArray();
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int remarkId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<Remark>()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId &&
                r.RemarkId == remarkId

            select new GetVO(
                r.RemarkId,
                r.CurriculumId,
                r.PersonId,
                r.Date,
                r.Description))
            .SingleAsync(ct);
    }
}
