namespace SB.Data;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Common;
using SB.Domain;
using static SB.Domain.IClassBooksQueryRepository;

internal class ClassBooksQueryRepository : Repository, IClassBooksQueryRepository
{
    private readonly string blobServiceHmacKey;
    private readonly string blobServicePublicWebUrl;

    public ClassBooksQueryRepository(
        UnitOfWork unitOfWork,
        IOptions<DomainOptions> domainOptions)
        : base(unitOfWork)
    {
        var opts = domainOptions.Value;

        if (string.IsNullOrEmpty(opts.BlobServiceHMACKey))
        {
            throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServiceHMACKey)} should have a configured value");
        }

        if (string.IsNullOrEmpty(opts.BlobServicePublicUrl))
        {
            throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServicePublicUrl)} should have a configured value");
        }

        this.blobServiceHmacKey = opts.BlobServiceHMACKey;
        this.blobServicePublicWebUrl = opts.BlobServicePublicUrl;
    }

    public async Task<GetAllVO[]> GetAllAsync(int schoolYear, int instId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join bc in this.DbContext.Set<BasicClass>() on cb.BasicClassId equals bc.BasicClassId
            into j1 from bc in j1.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            orderby bc.SortOrd, cb.BookName

            select new GetAllVO(
                cb.ClassBookId,
                cb.BookType,
                cb.BookName,
                cb.FullBookName,
                cb.BasicClassId,
                bc.Name,
                bc.SortOrd,
                cb.IsValid)
        ).ToArrayAsync(ct);
    }

    public async Task<GetAllVO[]> GetAllForTeacherAsync(int schoolYear, int instId, int personId, CancellationToken ct)
    {
        var teacherCurriculumClassBooks =
            from tcb in this.DbContext.Set<TeacherCurriculumClassBooksView>()

            where
                tcb.PersonId == personId &&
                tcb.SchoolYear == schoolYear &&
                tcb.InstId == instId

            select new
            {
                tcb.SchoolYear,
                tcb.ClassBookId
            };

        var teacherSupportClassBooks =
            from st in this.DbContext.Set<SupportTeacher>()

            join s in this.DbContext.Set<Support>()
            on new { st.SchoolYear, st.SupportId } equals new { s.SchoolYear, s.SupportId }

            join cb in this.DbContext.Set<ClassBook>()
            on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where
                st.PersonId == personId &&
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            select new
            {
                cb.SchoolYear,
                cb.ClassBookId
            };

        var teacherAbsenceHourClassBooks =
            from tah in this.DbContext.Set<TeacherAbsenceHour>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { tah.SchoolYear, tah.ScheduleLessonId } equals new { sl.SchoolYear, sl.ScheduleLessonId }

            join s in this.DbContext.Set<Schedule>()
            on new { sl.SchoolYear, sl.ScheduleId } equals new { s.SchoolYear, s.ScheduleId }

            join cb in this.DbContext.Set<ClassBook>()
            on new { s.SchoolYear, s.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where
                tah.SchoolYear == schoolYear &&
                tah.ReplTeacherPersonId != null &&
                tah.ReplTeacherPersonId == personId &&
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            select new
            {
                cb.SchoolYear,
                cb.ClassBookId
            };

        var teacherPersonnelSchoolBookAccessClassBooks =
            from psba in this.DbContext.Set<PersonnelSchoolBookAccess>()

            join cb in this.DbContext.Set<ClassBook>()
            on new { psba.SchoolYear, psba.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            where
                psba.PersonId == personId &&
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId

            select new
            {
                cb.SchoolYear,
                cb.ClassBookId
            };

        var teacherClassBooks =
            teacherCurriculumClassBooks
            .Concat(teacherSupportClassBooks)
            .Concat(teacherAbsenceHourClassBooks)
            .Concat(teacherPersonnelSchoolBookAccessClassBooks)
            .Distinct();

        return await (
            from tcb in teacherClassBooks

            join cb in this.DbContext.Set<ClassBook>() on new { tcb.SchoolYear, tcb.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }

            join bc in this.DbContext.Set<BasicClass>() on cb.BasicClassId equals bc.BasicClassId
            into j1 from bc in j1.DefaultIfEmpty()

            orderby bc.SortOrd, cb.BookName

            select new GetAllVO(
                cb.ClassBookId,
                cb.BookType,
                cb.BookName,
                cb.FullBookName,
                cb.BasicClassId,
                bc.Name,
                bc.SortOrd,
                cb.IsValid)
        ).ToArrayAsync(ct);
    }

    public async Task<GetAllForFinalizationVO[]> GetAllForFinalizationAsync(int schoolYear, int instId, int[]? classBookIds, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<ClassBook>();

        predicate = predicate.And(cb =>
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.IsValid);

        if (classBookIds != null && classBookIds.Length > 0)
        {
            predicate = predicate.And(
                cb => this.DbContext
                    .MakeIdsQuery(classBookIds)
                    .Any(id => cb.ClassBookId == id.Id));
        }

        return await (
            from cb in this.DbContext.Set<ClassBook>().Where(predicate)

            join bc in this.DbContext.Set<BasicClass>() on cb.BasicClassId equals bc.BasicClassId
            into j1 from bc in j1.DefaultIfEmpty()

            join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId

            join cbp in this.DbContext.Set<ClassBookPrint>().Where(cbp => cbp.IsFinal)
            on new { cb.SchoolYear, cb.ClassBookId } equals new { cbp.SchoolYear, cbp.ClassBookId }
            into j2 from cbp in j2.DefaultIfEmpty()

            join cbsc in this.DbContext.Set<ClassBookStatusChange>().Where(cbp => cbp.IsLast)
            on new { cb.SchoolYear, cb.ClassBookId } equals new { cbsc.SchoolYear, cbsc.ClassBookId }
            into j3 from cbsc in j3.DefaultIfEmpty()

            join csu in this.DbContext.Set<SysUser>() on cbsc.ChangedBySysUserId equals csu.SysUserId
            into j4 from csu in j4.DefaultIfEmpty()

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId
            into j5 from cp in j5.DefaultIfEmpty()

            orderby bc.SortOrd, cb.BookName

            select new GetAllForFinalizationVO(
                cb.ClassBookId,
                cg.ClassName,
                bc.Name,
                cb.FullBookName,
                cbp.ClassBookPrintId,
                cbp != null ? cbp.Status : null,
                cbp != null ? cbp.IsSigned : null,
                this.blobServiceHmacKey,
                this.blobServicePublicWebUrl,
                cbp.BlobId,
                cbsc.Type,
                cbsc.ChangeDate,
                cp.FirstName,
                cp.LastName,
                cbsc.Signees)
        ).ToArrayAsync(ct);
    }

    public async Task<GetVO> GetAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.BasicClassId,
                cb.ClassId,
                cb.ClassIsLvl2,
                cb.BookType
            }
        ).SingleAsync(ct);

        var leadTeacherNames = await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId
            join cut in this.DbContext.Set<CurriculumTeacher>() on c.CurriculumId equals cut.CurriculumId
            join sp in this.DbContext.Set<StaffPosition>() on cut.StaffPositionId equals sp.StaffPositionId
            join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId
            where cut.IsValid &&
                ((c.SubjectId == Subject.SchoolLeadTeacherHour && classBook.BasicClassId > 0) || (c.SubjectId == 80 && classBook.BasicClassId < 0))
            select StringUtils.JoinNames(p.FirstName, p.LastName)
        ).ToArrayAsync(ct);

        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join cbsys in this.DbContext.Set<ClassBookSchoolYearSettings>()
            on new { cb.SchoolYear, cb.ClassBookId }
            equals new { cbsys.SchoolYear, cbsys.ClassBookId }

            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId

            select new GetVO(
                cb.ClassBookId,
                cb.ClassId,
                cb.BookType,
                cb.FullBookName,
                string.Join(", ", leadTeacherNames),
                cb.BasicClassId,
                cb.IsFinalized,
                cb.IsValid,
                cb.ModifyDate,
                cbsys.SchoolYearStartDateLimit,
                cbsys.SchoolYearStartDate,
                cbsys.FirstTermEndDate,
                cbsys.SecondTermStartDate,
                cbsys.SchoolYearEndDate,
                cbsys.SchoolYearEndDateLimit,
                cbsys.HasFutureEntryLock,
                cbsys.PastMonthLockDay)
        ).SingleAsync(ct);
    }

    public async Task<GetStudentsVO[]> GetStudentsAsync(int schoolYear, int classBookId, CancellationToken ct)
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

        var specialNeeds = await (
            from cbssn in this.DbContext.Set<ClassBookStudentFirstGradeResultSpecialNeeds>()
            where cbssn.SchoolYear == schoolYear &&
                cbssn.ClassBookId == classBookId
            select cbssn.PersonId
        ).ToArrayAsync(ct);

        var students = await (
            from sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            orderby (sc.ClassNumber == null ? 99999 : sc.ClassNumber), p.FirstName, p.MiddleName, p.LastName

            select new GetStudentsVO(
                sc.PersonId,
                sc.ClassNumber,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                sc.IsTransferred,
                specialNeeds.Any(s => s == sc.PersonId)
                )
        ).ToArrayAsync(ct);

        return students
            .GroupBy(s => s.PersonId)
            .Select(g => g.OrderBy(s => !s.IsTransferred ? 1 : 2).First())
            .ToArray();
    }

    public async Task<int[]> GetCurriculumIdsForClassBookAsync(int schoolYear, int classId, bool classIsLvl2, CancellationToken ct)
    {
        return await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)

            // Filter by linked students
            join cs in this.DbContext.Set<CurriculumStudent>() on cc.CurriculumId equals cs.CurriculumId

            where cc.IsValid && cs.IsValid

            select cc.CurriculumId
        )
        .Distinct()
        .ToArrayAsync(ct);
    }

    public async Task<GetRemovedStudentsVO[]> GetRemovedStudentsAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int[] personIds,
        CancellationToken ct)
    {
        return await this.DbContext.Set<StudentsWithClassBookDataView>()
            .Where(p =>
                p.SchoolYear == schoolYear &&
                p.InstId == instId &&
                p.ClassBookId == classBookId &&
                this.DbContext
                    .MakeIdsQuery(personIds)
                    .Any(id => p.PersonId == id.Id))
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.MiddleName)
            .ThenBy(p => p.LastName)
            .Select(p => new GetRemovedStudentsVO(
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName))
            .ToArrayAsync(ct);
    }

    public async Task<GetDefaultCurriculumVO> GetDefaultCurriculumAsync(
        int? personId,
        int schoolYear,
        int classBookId,
        bool excludeGradeless,
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

        Expression<Func<ClassBookCurriculumGradeless, bool>> gradelessPredicate;
        if (excludeGradeless)
        {
            gradelessPredicate = cg => cg.SchoolYear == schoolYear && cg.ClassBookId == classBookId;
        }
        else
        {
            gradelessPredicate = cg => false;
        }

        int? curriculumId;
        if (personId.HasValue)
        {
            var defaultCurriculum = await (
                from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

                join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

                join cg in this.DbContext.Set<ClassBookCurriculumGradeless>().Where(gradelessPredicate)
                on c.CurriculumId equals cg.CurriculumId
                into j1 from cg in j1.DefaultIfEmpty()

                join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

                join sp in this.DbContext.Set<StaffPosition>()
                    .Where(sp => sp.PersonId == personId.Value)
                on t.StaffPositionId equals sp.StaffPositionId
                into j2 from sp in j2.DefaultIfEmpty()

                where cg == null &&
                    cc.IsValid &&
                    c.IsValid &&
                    t.IsValid

                // - if the person is a teacher in one or more cirriculums sp.PersonId will
                //   be the same non-null value for all rows where he/she is a teacher and
                //   they will come first (as null is the lowest value) and the rows will
                //   then be sorted according to curriculum order
                // - if the person is NOT a teacher in any curriculum sp.PersonId will be null
                //   for all records and the rows will be sorted according to curriculum order
                orderby sp.PersonId descending, c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

                select new {
                    CurriculumId = (int?)cc.CurriculumId,
                    PersonId = (int?)sp.PersonId
                }
            ).FirstOrDefaultAsync(ct);

            // if PersonId is null then the person is NOT a teacher in any curriculum
            // so maybe he is a replacement teacher for some curriculum
            if (defaultCurriculum != null && defaultCurriculum.PersonId == null)
            {
                curriculumId = await (
                    from sch in this.DbContext.Set<Schedule>()

                    join l in this.DbContext.Set<ScheduleLesson>()
                    on new { sch.SchoolYear, sch.ScheduleId }
                    equals new { l.SchoolYear, l.ScheduleId }

                    join tah in this.DbContext.Set<TeacherAbsenceHour>()
                    on new { l.SchoolYear, l.ScheduleLessonId }
                    equals new { tah.SchoolYear, tah.ScheduleLessonId }

                    where sch.SchoolYear == schoolYear
                        && sch.ClassBookId == classBookId
                        && tah.ReplTeacherPersonId == personId.Value
                        && tah.ReplTeacherIsNonSpecialist == false

                    select (int?)l.CurriculumId
                )
                .FirstOrDefaultAsync(ct) ?? defaultCurriculum.CurriculumId;
            } else {
                curriculumId = defaultCurriculum?.CurriculumId;
            }
        }
        else
        {
            curriculumId = await (
                from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

                join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

                join cg in this.DbContext.Set<ClassBookCurriculumGradeless>().Where(gradelessPredicate)
                on c.CurriculumId equals cg.CurriculumId
                into j1 from cg in j1.DefaultIfEmpty()

                where cg == null &&
                    cc.IsValid &&
                    c.IsValid

                orderby c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

                select (int?)cc.CurriculumId
            ).FirstOrDefaultAsync(ct);
        }

        return new GetDefaultCurriculumVO(curriculumId);
    }

    public async Task<GetCurriculumsVO[]> GetCurriculumsAsync(
        int schoolYear,
        int classBookId,
        bool excludeGradeless,
        bool includeInvalidWithGrades,
        bool includeInvalidWithTopicPlans,
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

        Expression<Func<ClassBookCurriculumGradeless, bool>> gradelessPredicate = excludeGradeless
            ? cg => cg.SchoolYear == schoolYear && cg.ClassBookId == classBookId
            : cg => false;

        Expression<Func<Grade, bool>> gradePredicate = includeInvalidWithGrades
            ? g => g.SchoolYear == schoolYear && g.ClassBookId == classBookId
            : g => false;

        Expression<Func<ClassBookTopicPlanItem, bool>> topicPlanPredicate = includeInvalidWithTopicPlans
            ? tpi => tpi.SchoolYear == schoolYear && tpi.ClassBookId == classBookId
            : tpi => false;

        var curriculumReplTeacherPersonIds = await (
            from sch in this.DbContext.Set<Schedule>()

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

            select new { l.CurriculumId, tah.ReplTeacherPersonId }
        )
        .Distinct()
        .ToArrayAsync(ct);

        var replTeacherPersonIdByCurriculum =
            curriculumReplTeacherPersonIds
            .ToLookup(tah => tah.CurriculumId, tah => tah.ReplTeacherPersonId!.Value);

        var curriculums = await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join cg in this.DbContext.Set<ClassBookCurriculumGradeless>().Where(gradelessPredicate)
            on c.CurriculumId equals cg.CurriculumId
            into j1 from cg in j1.DefaultIfEmpty()

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join t in this.DbContext.Set<CurriculumTeacher>().Where(t => t.IsValid)
            on c.CurriculumId equals t.CurriculumId
            into j2 from t in j2.DefaultIfEmpty()

            join sp in this.DbContext.Set<StaffPosition>()
            on t.StaffPositionId equals sp.StaffPositionId
            into j3 from sp in j3.DefaultIfEmpty()

            where (
                    cg == null &&
                    cc.IsValid &&
                    c.IsValid
                ) ||
                this.DbContext.Set<Grade>()
                    .Where(gradePredicate)
                    .Where(g => g.CurriculumId == c.CurriculumId)
                    .Any() ||
                this.DbContext.Set<ClassBookTopicPlanItem>()
                    .Where(topicPlanPredicate)
                    .Where(g => g.CurriculumId == c.CurriculumId)
                    .Any() ||
                SubjectType.ProfilingSubjectIds.Contains(c.SubjectTypeId ?? SubjectType.DefaultSubjectTypeId)

            select new
            {
                c.CurriculumId,
                ParentCurriculumId = c.ParentCurriculumID,
                s.SubjectName,
                s.SubjectNameShort,
                CurriculumGroupNum = (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                SubjectTypeId = st.SubjectTypeId,
                SubjectTypeName = st.Name,
                WithoutGrade = cg != null,
                IsValid = cc.IsValid && c.IsValid,
                c.CurriculumPartID,
                c.IsIndividualLesson,
                c.IsIndividualCurriculum,
                c.TotalTermHours,
                WriteAccessCurriculumTeacherPersonId = (int?)sp.PersonId
            }
        ).ToArrayAsync(ct);

        return curriculums
            .GroupBy(c =>
                new
                {
                    c.CurriculumId,
                    c.ParentCurriculumId,
                    c.SubjectName,
                    c.SubjectNameShort,
                    c.CurriculumGroupNum,
                    c.SubjectTypeId,
                    c.SubjectTypeName,
                    c.WithoutGrade,
                    c.CurriculumPartID,
                    c.IsIndividualLesson,
                    c.IsIndividualCurriculum,
                    c.TotalTermHours,
                })
            .OrderBy(g => g.Key.CurriculumPartID)
            .ThenBy(g => g.Key.IsIndividualLesson)
            .ThenByDescending(g => g.Key.TotalTermHours)
            .Select(g =>
                new GetCurriculumsVO(
                    g.Key.CurriculumId,
                    // curriculums are bugged and sometimes the parent has itself as parentCurriculumId
                    g.Key.CurriculumId != g.Key.ParentCurriculumId ? g.Key.ParentCurriculumId : null,
                    g.Key.SubjectName,
                    g.Key.SubjectNameShort,
                    g.Key.CurriculumGroupNum,
                    (g.Key.IsIndividualLesson ?? 0) != 0,
                    g.Key.IsIndividualCurriculum ?? false,
                    g.Key.SubjectTypeId,
                    g.Key.SubjectTypeName,
                    g.Key.WithoutGrade,
                    g.Any(gi => gi.IsValid),
                    g.Select(gi => gi.WriteAccessCurriculumTeacherPersonId)
                        .Where(pId => pId.HasValue)
                        .Select(pId => pId!.Value)
                        .Distinct()
                        .ToArray(),
                    replTeacherPersonIdByCurriculum[g.Key.CurriculumId].ToArray()))
            .ToArray();
    }

    public async Task<DateTime[]> GetReplTeacherDatesAsync(int schoolYear, int classBookId, int personId, CancellationToken ct)
    {
        return await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join tah in this.DbContext.Set<TeacherAbsenceHour>()
            on new { sl.SchoolYear, sl.ScheduleLessonId } equals new { tah.SchoolYear, tah.ScheduleLessonId }

            where sch.SchoolYear == schoolYear &&
                sch.ClassBookId == classBookId &&
                !sch.IsIndividualSchedule &&
                tah.ReplTeacherPersonId == personId
            select
                sl.Date
        ).ToArrayAsync(ct);
    }

    public async Task<string?> GetClassBookPrintContentHashAsync(
        int schoolYear,
        int classBookId,
        int classBookPrintId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<ClassBookPrint>()
            .Where(cbp =>
                cbp.SchoolYear == schoolYear &&
                cbp.ClassBookId == classBookId &&
                cbp.ClassBookPrintId == classBookPrintId)
            .Select(cbp => cbp.ContentHash)
            .SingleAsync(ct);
    }

    public async Task<string[]> GetClassBookNamesByIdsAsync(int schoolYear, int instId, int[] classsBookIds, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            where cb.SchoolYear == schoolYear &&
            cb.InstId == instId &&
                this.DbContext.MakeIdsQuery(classsBookIds)
                    .Any(id => cb.ClassBookId == id.Id)

            orderby cb.BasicClassId, cb.FullBookName

            select cb.FullBookName
        ).ToArrayAsync(ct);
    }
}
