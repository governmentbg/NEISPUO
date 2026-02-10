namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IClassBookPrintRepository;

internal class ClassBookPrintRepository : Repository, IClassBookPrintRepository
{
    private const int HomeCountryId = 34;
    private const int NotApplicableSPPOOSpecialityId = -1;

    private class RelativeInfo
    {
        public int StudentPersonId { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public ClassBookPrintRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetCoverPageDataVO> GetCoverPageDataAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            join cg in this.DbContext.Set<ClassGroup>() on cb.ClassId equals cg.ClassId

            join s in this.DbContext.Set<SPPOOSpeciality>() on cg.ClassSpecialityId equals s.SPPOOSpecialityId
            into j0 from s in j0.DefaultIfEmpty()

            join sy in this.DbContext.Set<InstitutionSchoolYear>()
            on new { cb.InstId, cb.SchoolYear } equals new { InstId = sy.InstitutionId, sy.SchoolYear }

            join la in this.DbContext.Set<LocalArea>() on sy.LocalAreaId equals la.LocalAreaId
            into j1 from la in j1.DefaultIfEmpty()

            join t in this.DbContext.Set<Town>() on sy.TownId equals t.TownId
            into j2 from t in j2.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on t.MunicipalityId equals m.MunicipalityId
            into j3 from m in j3.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into j4 from r in j4.DefaultIfEmpty()

            where cb.SchoolYear == schoolYear
                && cb.ClassBookId == classBookId

            select new GetCoverPageDataVO(
                cg.BasicClassId,
                cb.BookType,
                sy.Abbreviation,
                t.Type == "1" ? "гр." :
                    t.Type == "2" ? "с." :
                    null,
                t.Name,
                m.Name,
                la.Name,
                r.Name,
                cg.ClassName,
                null,
                s.Name,
                cb.SchoolYearProgram)
        ).SingleAsync(ct);
    }

    public async Task<GetTeacherSubjectsVO[]> GetTeacherInfoAndSubjectsAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        var curriculumTeachers = (await (
                from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)

                join t in this.DbContext.Set<CurriculumTeacher>() on cc.CurriculumId equals t.CurriculumId

                join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId

                join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId

                join pd in this.DbContext.Set<PersonDetail>() on p.PersonId equals pd.PersonId
                into j1 from pd in j1.DefaultIfEmpty()

                orderby t.StaffPositionStartDate descending, t.StaffPositionTerminationDate

                where cc.IsValid

                select new
                {
                    cc.CurriculumId,
                    Name = StringUtils.JoinNames(p.FirstName, p.LastName),
                    pd.PhoneNumber
                })
                .ToArrayAsync(ct))
                .ToLookup(t => t.CurriculumId, t => new { t.Name, t.PhoneNumber });

        var result = await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            orderby c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            where
                cc.IsValid &&
                c.IsValid

            select new
            {
                s.SubjectName,
                SubjectTypeName = st.Name,
                c.CurriculumId
            }
        )
        .ToArrayAsync(ct);

        return result
                .Select(t => new GetTeacherSubjectsVO(
                    t.SubjectName,
                    t.SubjectTypeName,
                    curriculumTeachers[t.CurriculumId]
                        .Take(2)
                        .Select(cTeacher => new TeacherInfo(cTeacher.Name, cTeacher.PhoneNumber))
                        .ToArray()))
                .ToArray();
    }

    public async Task<GetSchedulesDataVO[]> GetSchedulesDataAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Schedule>()

            join p in this.DbContext.Set<Person>()
            on new { PersonId = s.PersonId } equals new { PersonId = (int?)p.PersonId }
            into g1
            from p in g1.DefaultIfEmpty()

            where s.SchoolYear == schoolYear
                && s.ClassBookId == classBookId

            orderby s.EndDate

            select new GetSchedulesDataVO(
                s.ScheduleId,
                (int?)p.PersonId,
                (string?)StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                s.StartDate,
                s.EndDate)
        ).ToArrayAsync(ct);
    }

    public async Task<GetSchedulesLessonsVO[]> GetSchedulesLessonsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where sch.SchoolYear == schoolYear
                && sch.ClassBookId == classBookId

            select new GetSchedulesLessonsVO(
                sch.ScheduleId,
                sl.Day,
                sl.HourNumber,
                s.SubjectName,
                st.Name)
        ).ToArrayAsync(ct);
    }

    public async Task<GetParentMeetingsVO[]> GetParentMeetingsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from pm in this.DbContext.Set<ParentMeeting>()

            where pm.SchoolYear == schoolYear
                && pm.ClassBookId == classBookId

            orderby pm.Date, pm.StartTime

            select new GetParentMeetingsVO(
                pm.Date,
                pm.Title,
                pm.Description)
        ).ToArrayAsync(ct);
    }

    public async Task<GetExamsVO[]> GetExamsAsync(int schoolYear, int classBookId, BookExamType type, CancellationToken ct)
    {
        var schoolYearSettings = await (
            from sy in this.DbContext.Set<ClassBookSchoolYearSettings>()
            where sy.SchoolYear == schoolYear && sy.ClassBookId == classBookId
            select new
            {
                sy.SchoolYearStartDateLimit,
                sy.SchoolYearStartDate,
                sy.FirstTermEndDate,
                sy.SecondTermStartDate,
                sy.SchoolYearEndDate,
                sy.SchoolYearEndDateLimit
            }
        ).SingleAsync(ct);

        var exams = await (
            from e in this.DbContext.Set<Exam>()

            join c in this.DbContext.Set<Curriculum>() on e.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where e.SchoolYear == schoolYear &&
                e.ClassBookId == classBookId &&
                e.Type == type

            orderby e.Date descending

            select new
            {
                e.Type,
                s.SubjectName,
                SubjectTypeName = st.Name,
                e.Date
            }).ToArrayAsync(ct);

        return exams
            .GroupBy(e => new { e.SubjectName, e.SubjectTypeName })
            .OrderBy(g => g.Key.SubjectName)
            .ThenBy(g => g.Key.SubjectTypeName)
            .Select(g => new GetExamsVO(
                g.Key.SubjectName,
                g.Key.SubjectTypeName,
                g.Where(gi => schoolYearSettings.SchoolYearStartDateLimit <= gi.Date && gi.Date < schoolYearSettings.SecondTermStartDate)
                .OrderBy(gi => gi.Date)
                .Select(gi => gi.Date)
                .ToArray(),
                g.Where(gi => schoolYearSettings.SecondTermStartDate <= gi.Date && gi.Date <= schoolYearSettings.SchoolYearEndDateLimit)
                .OrderBy(gi => gi.Date)
                .Select(gi => gi.Date)
                .ToArray()
                ))
            .ToArray();
    }

    public async Task<GetStudentsDataVO[]> GetStudentsDataAsync(int schoolYear, int instId, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        var unregisteredRelatives = await (
                from sc in this.DbContext.StudentsForClassBook(schoolYear, classId, classIsLvl2)

                join r in this.DbContext.Set<Relative>() on sc.PersonId equals r.PersonId

                where r.FirstName != null && r.LastName != null

                select new RelativeInfo
                {
                    StudentPersonId = sc.PersonId,
                    FirstName = r.FirstName ?? string.Empty,
                    LastName = r.LastName ?? string.Empty,
                    Email = r.Email ?? string.Empty,
                    PhoneNumber = r.PhoneNumber ?? string.Empty
                })
            .ToListAsync(ct);

        var registeredRelatives = await (
                from sc in this.DbContext.StudentsForClassBook(schoolYear, classId, classIsLvl2)

                join pa in this.DbContext.Set<ParentChildSchoolBookAccess>() on sc.PersonId equals pa.ChildId
                join su in this.DbContext.Set<SysUser>().Where(s => s.DeletedOn == null) on pa.ParentId equals su.PersonId
                join p in this.DbContext.Set<Person>() on pa.ParentId equals p.PersonId

                where pa.HasAccess && p.FirstName != null && p.LastName != null

                select new RelativeInfo
                {
                    StudentPersonId = sc.PersonId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = su.Username,
                    PhoneNumber = string.Empty
                })
            .ToListAsync(ct);

        // We have duplicate records in unregistered and registered relatives,
        // and we need to get registered users as priority, but since there is no phone number in SysUser table
        // we need to transfer the phone number to registered record
        foreach (var registeredRelative in registeredRelatives)
        {
            var recordForDeletion =
                unregisteredRelatives
                    .FirstOrDefault(ur =>
                        ur.StudentPersonId == registeredRelative.StudentPersonId &&
                        (registeredRelative.Email.Trim().Equals(ur.Email.Trim(), StringComparison.OrdinalIgnoreCase) ||
                         (registeredRelative.FirstName.Trim().Equals(ur.FirstName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                          registeredRelative.LastName.Trim().Equals(ur.LastName.Trim(), StringComparison.OrdinalIgnoreCase))));

            if (recordForDeletion != null)
            {
                registeredRelative.PhoneNumber =
                    string.IsNullOrWhiteSpace(recordForDeletion.PhoneNumber) ?
                        registeredRelative.PhoneNumber :
                        recordForDeletion.PhoneNumber;
                unregisteredRelatives.Remove(recordForDeletion);
            }
        }

        var allRelatives = registeredRelatives
            .Union(unregisteredRelatives)
            .GroupBy(t => new
            {
                t.StudentPersonId,
                t.FirstName,
                t.LastName
            })
            .ToLookup(t => t.Key.StudentPersonId, t =>
                new GetStudentsDataVORelative(
                    t.Key.FirstName,
                    t.Key.LastName,
                    string.Join(", ", t.Select(x => x.PhoneNumber).Distinct())
                )
             );

        var studentPersonIds = allRelatives.Select(r => r.Key).ToArray();

        var studentRegularClasses = (await (
            from cb in this.DbContext.ClassBooksForStudents(schoolYear, studentPersonIds)
            where
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                cb.PersonStatus == StudentClassStatus.Enrolled &&
                (cb.BookType == ClassBookType.Book_I_III || 
                 cb.BookType == ClassBookType.Book_IV ||
                 cb.BookType == ClassBookType.Book_V_XII)
            select new
            {
                cb.PersonId,
                cb.FullBookName
            }
        ).ToArrayAsync(ct))
        .ToLookup(t => t.PersonId, t => t.FullBookName);

        var studentGradelessCurriculums = (await (
                from sg in this.DbContext.Set<ClassBookStudentGradeless>()
                where sg.SchoolYear == schoolYear && sg.ClassBookId == classBookId
                select new
                {
                    sg.PersonId,
                    sg.CurriculumId,
                    sg.WithoutFirstTermGrade,
                    sg.WithoutSecondTermGrade,
                    sg.WithoutFinalGrade
                })
                .ToListAsync(ct)
            )
            .ToLookup(
                sg => sg.PersonId,
                sg =>
                    new GradelessCurriculum(
                        sg.CurriculumId,
                        sg.WithoutFirstTermGrade,
                        sg.WithoutSecondTermGrade,
                        sg.WithoutFinalGrade)
            );

        return await (
            from sc in this.DbContext.StudentsForClassBook(schoolYear, classId, classIsLvl2)

            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId
            join s in this.DbContext.Set<Student>() on sc.PersonId equals s.PersonId

            join sp in this.DbContext.Set<SPPOOSpeciality>() on sc.StudentSpecialityId equals sp.SPPOOSpecialityId
            into j2 from sp in j2.DefaultIfEmpty()

            join bt in this.DbContext.Set<Town>() on p.BirthPlaceTownId equals bt.TownId
            into j3 from bt in j3.DefaultIfEmpty()

            join pt in this.DbContext.Set<Town>() on p.PermanentTownId equals pt.TownId
            into j4 from pt in j4.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on bt.MunicipalityId equals m.MunicipalityId
            into j5 from m in j5.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into j6 from r in j6.DefaultIfEmpty()

            join cbp in this.DbContext.Set<Country>() on p.BirthPlaceCountry equals cbp.CountryId
            into j7 from cbp in j7.DefaultIfEmpty()

            join ad in this.DbContext.Set<AdmissionDocument>() on sc.AdmissionDocumentId equals ad.Id
            into j8 from ad in j8.DefaultIfEmpty()

            join art in this.DbContext.Set<AdmissionReasonType>() on ad.AdmissionReasonTypeId equals art.Id
            into j9 from art in j9.DefaultIfEmpty()

            join rd in this.DbContext.Set<RelocationDocument>() on sc.RelocationDocumentId equals rd.Id
            into j10 from rd in j10.DefaultIfEmpty()

            orderby sc.ClassNumber ?? 99999

            select new GetStudentsDataVO(
                sc.ClassNumber,
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                p.PersonalId,
                p.BirthDate,
                cbp.CountryId != HomeCountryId,
                cbp.Name,
                bt.Type == "1" ? "гр." : bt.Type == "2" ? "с." : null,
                bt.Name,
                pt.Type == "1" ? "гр." : pt.Type == "2" ? "с." : null,
                pt.Name,
                p.PermanentAddress,
                s.HomePhone,
                s.MobilePhone,
                s.GPName,
                s.GPPhone,
                sc.EnrollmentDate,
                sc.DischargeDate,
                string.Join(", ", studentRegularClasses[sc.PersonId]),
                sp.SPPOOSpecialityId != NotApplicableSPPOOSpecialityId ? sp.Name : string.Empty,
                allRelatives[sc.PersonId].ToArray(),
                rd.NoteNumber,
                rd.NoteDate,
                ad.NoteNumber,
                ad.NoteDate,
                art.Name,
                sc.IsTransferred,
                studentGradelessCurriculums[sc.PersonId].ToArray()
            )
        ).ToArrayAsync(ct);
    }

    public async Task<GetStudentsDataVO[]> GetRemovedStudentsDataAsync(int schoolYear, int classBookId, int[] studentPersonIds, CancellationToken ct)
    {
        var studentClassBookDataViewQuery = this.DbContext.Set<StudentsWithClassBookDataView>()
                .Where(s =>
                    s.SchoolYear == schoolYear &&
                    s.ClassBookId == classBookId &&
                    !this.DbContext
                        .MakeIdsQuery(studentPersonIds)
                        .Any(id => s.PersonId == id.Id));

        var studentRelatives = (await (
            from scw in studentClassBookDataViewQuery

            join r in this.DbContext.Set<Relative>() on scw.PersonId equals r.PersonId

            select new
            {
                StudentPersonId = scw.PersonId,
                RelativeFirstName = r.FirstName,
                RelativeLastName = r.LastName,
                RelativePhoneNumber = r.PhoneNumber
            }
        ).ToArrayAsync(ct))
        .ToLookup(t => t.StudentPersonId, t => new GetStudentsDataVORelative(t.RelativeFirstName, t.RelativeLastName, t.RelativePhoneNumber));

        var studentRegularClasses = (await (
            from cb in this.DbContext.ClassBooksForStudents(schoolYear, studentPersonIds)
            where cb.SchoolYear == schoolYear && cb.PersonStatus == StudentClassStatus.Enrolled && cb.BookType != ClassBookType.Book_CDO
            select new
            {
                cb.PersonId,
                cb.FullBookName
            }
        ).ToArrayAsync(ct))
        .ToLookup(t => t.PersonId, t => t.FullBookName);

        var studentGradelessCurriculums = (await (
                from sg in this.DbContext.Set<ClassBookStudentGradeless>()
                where sg.SchoolYear == schoolYear && sg.ClassBookId == classBookId
                select new
                {
                    sg.PersonId,
                    sg.CurriculumId,
                    sg.WithoutFirstTermGrade,
                    sg.WithoutSecondTermGrade,
                    sg.WithoutFinalGrade
                })
                .ToListAsync(ct)
            )
            .ToLookup(
                sg => sg.PersonId,
                sg =>
                    new GradelessCurriculum(
                        sg.CurriculumId,
                        sg.WithoutFirstTermGrade,
                        sg.WithoutSecondTermGrade,
                        sg.WithoutFinalGrade)
            );

        return await (
            from scw in studentClassBookDataViewQuery

            join p in this.DbContext.Set<Person>() on scw.PersonId equals p.PersonId

            join s in this.DbContext.Set<Student>() on scw.PersonId equals s.PersonId
            into j2 from s in j2.DefaultIfEmpty()

            join bt in this.DbContext.Set<Town>() on p.BirthPlaceTownId equals bt.TownId
            into j3 from bt in j3.DefaultIfEmpty()

            join pt in this.DbContext.Set<Town>() on p.PermanentTownId equals pt.TownId
            into j4 from pt in j4.DefaultIfEmpty()

            join m in this.DbContext.Set<Municipality>() on bt.MunicipalityId equals m.MunicipalityId
            into j5 from m in j5.DefaultIfEmpty()

            join r in this.DbContext.Set<Region>() on m.RegionId equals r.RegionId
            into j6 from r in j6.DefaultIfEmpty()

            join cbp in this.DbContext.Set<Country>() on p.BirthPlaceCountry equals cbp.CountryId
            into j7 from cbp in j7.DefaultIfEmpty()

            select new GetStudentsDataVO(
                null,
                p.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                p.PersonalId,
                p.BirthDate,
                cbp.CountryId != HomeCountryId,
                cbp.Name,
                bt.Type == "1" ? "гр." : bt.Type == "2" ? "с." : null,
                bt.Name,
                pt.Type == "1" ? "гр." : pt.Type == "2" ? "с." : null,
                pt.Name,
                p.PermanentAddress,
                s.HomePhone,
                s.MobilePhone,
                s.GPName,
                s.GPPhone,
                null,
                null,
                string.Join(", ", studentRegularClasses[p.PersonId]),
                string.Empty,
                studentRelatives[scw.PersonId].ToArray(),
                null,
                null,
                null,
                null,
                null,
                true,
                studentGradelessCurriculums[scw.PersonId].ToArray()
            )
        ).ToArrayAsync(ct);
    }

    public async Task<GetGradesVO[]> GetGradesAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var gradesSql = """
            SELECT
                g.PersonId,
                g.CurriculumId,
                g.GradeId,
                g.Date as GradeDate,
                g.Type,
                g.Term,
                g.Category,
                g.DecimalGrade,
                g.QualitativeGrade,
                g.SpecialGrade,
                c.SubjectTypeID
            FROM
                [school_books].[Grade] g
            INNER JOIN [inst_year].[Curriculum] c ON g.CurriculumId = c.CurriculumID
            WHERE
                g.SchoolYear = @schoolYear AND
                g.ClassBookId = @classBookId
            ORDER BY
                g.Date,
                g.CreateDate
            """;
        return await this.DbContext.Set<GetGradesVO>()
            .FromSqlRaw(gradesSql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId))
            .ToArrayAsync(ct);
    }

    public async Task<GetRemarksVO[]> GetRemarksAsync(int schoolYear, int classBookId, CancellationToken ct)
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

        return await (
            from r in this.DbContext.Set<Remark>()

            join c in this.DbContext.Set<Curriculum>()
            on new { r.SchoolYear, r.CurriculumId } equals new { c.SchoolYear, c.CurriculumId }

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join p in this.DbContext.Set<Person>() on r.PersonId equals p.PersonId
            join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            on r.PersonId equals sc.PersonId
            into g1 from sc in g1.DefaultIfEmpty()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId

            orderby (sc.ClassNumber == null ? 99999 : sc.ClassNumber)

            select new GetRemarksVO(
                sc.ClassNumber,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                (bool?)sc.IsTransferred ?? true,
                r.Date,
                EnumUtils.GetEnumDescription(r.Type),
                s.SubjectName,
                st.Name,
                r.Description))
            .ToArrayAsync(ct);
    }

    public async Task<GetScheduleAndAbsencesByWeekVO[]> GetScheduleAndAbsencesByWeekAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var topicTeachers = (await (
            from t in this.DbContext.Set<Topic>()

            join tt in this.DbContext.Set<TopicTeacher>()
            on new { t.SchoolYear, t.TopicId } equals new { tt.SchoolYear, tt.TopicId }

            join p in this.DbContext.Set<Person>()
            on tt.PersonId equals p.PersonId

            where t.SchoolYear == schoolYear &&
                t.ClassBookId == classBookId

            select new
            {
                t.ScheduleLessonId,
                TeacherPersonId = p.PersonId,
                TeacherFirstName = p.FirstName,
                TeacherLastName = p.LastName,
                tt.IsReplTeacher
            }
        ).ToListAsync(ct))
        .ToLookup(
            tt => tt.ScheduleLessonId,
            tt => StringUtils.JoinNames(tt.TeacherFirstName, tt.TeacherLastName) + (tt.IsReplTeacher ? " (зам.)" : string.Empty));

        var scheduleHours = await (
            from sch in this.DbContext.Set<Schedule>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { sch.SchoolYear, sch.ScheduleId } equals new { sl.SchoolYear, sl.ScheduleId }

            join sh in this.DbContext.Set<ScheduleHour>()
            on new { sl.SchoolYear, sl.ScheduleId, sl.Day, sl.HourNumber, sl.CurriculumId } equals new { sh.SchoolYear, sh.ScheduleId, sh.Day, sh.HourNumber, sh.CurriculumId }

            join sd in this.DbContext.Set<ScheduleDate>()
            on new { sl.SchoolYear, sl.ScheduleId, sl.Date } equals new { sd.SchoolYear, sd.ScheduleId, sd.Date }

            join c in this.DbContext.Set<Curriculum>() on sl.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join stp in this.DbContext.Set<Person>()
            on new { sch.PersonId } equals new { PersonId = (int?)stp.PersonId }
            into g3 from stp in g3.DefaultIfEmpty()

            where sch.SchoolYear == schoolYear &&
                sch.ClassBookId == classBookId

            select new
            {
                sl.ScheduleLessonId,
                sl.Date,
                sl.Day,
                sl.HourNumber,
                sl.CurriculumId,
                sd.Year,
                sd.WeekNumber,

                StudentPersonId = (int?)stp.PersonId,
                StudentName = StringUtils.JoinNames(stp.FirstName, stp.MiddleName, stp.LastName),

                sch.ShiftId,

                s.SubjectName,
                SubjectTypeName = st.Name,
                sh.Location
            }
        )
        .AsSplitQuery()
        .ToArrayAsync(ct);

        var scheduleLessonIds = scheduleHours.Select(sh => sh.ScheduleLessonId).ToArray();
        var scheduleTeacherAbsences = (await (
                from tah in this.DbContext.Set<TeacherAbsenceHour>()

                join repl in this.DbContext.Set<Person>()
                    on new { PersonId = tah.ReplTeacherPersonId } equals new { PersonId = (int?)repl.PersonId }
                    into g1 from repl in g1.DefaultIfEmpty()

                where tah.SchoolYear == schoolYear &&
                      this.DbContext
                          .MakeIdsQuery(scheduleLessonIds)
                          .Any(id => tah.ScheduleLessonId == id.Id)

                select new
                {
                    tah.ScheduleLessonId,
                    tah.TeacherAbsenceId,
                    tah.ReplTeacherIsNonSpecialist,
                    ReplTeacherName = StringUtils.JoinNames(repl.FirstName, repl.LastName),
                    tah.ExtReplTeacherName,
                    IsEmptyHour = tah.ReplTeacherPersonId == null && string.IsNullOrEmpty(tah.ExtReplTeacherName)
                })
            .ToDictionaryAsync(
                sta => sta.ScheduleLessonId,
                sta => sta,
                ct));

        var scheduleTopics = (await (
                    from tt in this.DbContext.Set<TopicTitle>()
                    join t in this.DbContext.Set<Topic>()
                        on new { tt.SchoolYear, tt.TopicId } equals new { t.SchoolYear, t.TopicId }
                    where t.SchoolYear == schoolYear &&
                          t.ClassBookId == classBookId &&
                          this.DbContext
                              .MakeIdsQuery(scheduleLessonIds)
                              .Any(id => t.ScheduleLessonId == id.Id)
                    select new
                    {
                        t.ScheduleLessonId,
                        tt.Title
                    })
                .ToListAsync(ct))
            .ToLookup(
                sc => sc.ScheduleLessonId,
                sc => sc.Title);

        var scheduleCurriculumsTeachers = (await (
                    from ctr in this.DbContext.Set<CurriculumTeacher>()

                    join sp in this.DbContext.Set<StaffPosition>()
                        on ctr.StaffPositionId equals sp.StaffPositionId

                    join p in this.DbContext.Set<Person>()
                        on sp.PersonId equals p.PersonId

                    where this.DbContext
                              .MakeIdsQuery(scheduleHours.Select(sh => sh.CurriculumId).ToArray())
                              .Any(id => ctr.CurriculumId == id.Id) &&
                               ((ctr.SchoolYear <= 2022 && ctr.IsValid) || ctr.SchoolYear > 2022)

                    select new
                    {
                        ctr.CurriculumId,
                        TeacherNames = StringUtils.JoinNames(p.FirstName, p.LastName),
                        StartDate = ctr.StaffPositionStartDate ?? ctr.ValidFrom ?? new DateTime(schoolYear, 9, 1),
                        EndDate = ctr.StaffPositionTerminationDate
                    })
                .ToListAsync(ct))
            .ToLookup(
                sc => sc.CurriculumId,
                sc => new { sc.TeacherNames, sc.StartDate, sc.EndDate });

        var offDayDates = await (
            from od in this.DbContext.Set<ClassBookOffDayDate>()

            where
                od.SchoolYear == schoolYear &&
                od.ClassBookId == classBookId

            select
                od.Date
        ).ToArrayAsync(ct);

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

        var absences = (await (
                from a in this.DbContext.Set<Absence>()

                join p in this.DbContext.Set<Person>() on a.PersonId equals p.PersonId

                join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
                on a.PersonId equals sc.PersonId
                into g1 from sc in g1.DefaultIfEmpty()

                where a.SchoolYear == schoolYear &&
                     a.ClassBookId == classBookId

                select new
                {
                    a.ScheduleLessonId,
                    a.Type,
                    sc.ClassNumber,
                    StudentName = (string?)StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                }
            )
            .ToArrayAsync(ct))
            .ToLookup(a => a.ScheduleLessonId);

        var additionalActivities = (await (
                from t in this.DbContext.Set<AdditionalActivity>()

                where t.SchoolYear == schoolYear &&
                    t.ClassBookId == classBookId

                select new
                {
                    t.Year,
                    t.WeekNumber,
                    t.Activity
                }
            )
            .ToArrayAsync(ct))
            .ToLookup(a => new { a.Year, a.WeekNumber }, a => a.Activity);

        var shiftIds = scheduleHours.Select(h => h.ShiftId).Distinct().ToArray();

        var shiftHours = await (
            from s in this.DbContext.Set<Shift>()

            join sh in this.DbContext.Set<ShiftHour>()
            on new { s.SchoolYear, s.ShiftId } equals new { sh.SchoolYear, sh.ShiftId }

            where s.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(shiftIds)
                    .Any(id => s.ShiftId == id.Id)

            select new
            {
                s.ShiftId,
                sh.Day,
                sh.HourNumber,
                sh.StartTime,
                sh.EndTime
            }
        ).ToArrayAsync(ct);

        var scheduleIncludesWeekend = scheduleHours.Any(sh => sh.Day > 5);

        return scheduleHours
            .GroupBy(r => new { r.StudentPersonId, r.StudentName, r.Year, r.WeekNumber })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.WeekNumber)
            .Select(g => new GetScheduleAndAbsencesByWeekVO
            (
                StudentName: g.Key.StudentName,
                Year: g.Key.Year,
                WeekNumber: g.Key.WeekNumber,
                WeekName: $"{DateExtensions.GetDateFromIsoWeek(g.Key.Year, g.Key.WeekNumber, 1):dd.MM} " +
                    $"- {DateExtensions.GetDateFromIsoWeek(g.Key.Year, g.Key.WeekNumber, 7):dd.MM}",
                AdditionalActivities: additionalActivities[new { g.Key.Year, g.Key.WeekNumber }].ToArray(),
                Days: Enumerable.Range(1, scheduleIncludesWeekend ? 7 : 5).Select(day =>
                {
                    var date = DateExtensions.GetDateFromIsoWeek(g.Key.Year, g.Key.WeekNumber, day);
                    var shiftId = g.FirstOrDefault(h => h.Day == day)?.ShiftId;

                    if (shiftId == null)
                    {
                        return new GetScheduleAndAbsencesByWeekVODay(
                            day,
                            date,
                            date.ToString("dddd"),
                            offDayDates.Contains(date),
                            true,
                            Array.Empty<GetScheduleAndAbsencesByWeekVOHour>()
                        );
                    }

                    var shours = shiftHours.Where(sh => sh.ShiftId == shiftId.Value && sh.Day == day);

                    var hours = Enumerable.Range(shours.DefaultIfEmpty().Min(s => s?.HourNumber ?? 0), shours.Count()).SelectMany((hourNumber, index) =>
                    {
                        var hours = g.Where(h => h.Day == day && h.HourNumber == hourNumber).ToArray();
                        if (!hours.Any())
                        {
                            return new GetScheduleAndAbsencesByWeekVOHour[]
                            {
                                new GetScheduleAndAbsencesByWeekVOHour(
                                    hourNumber,
                                    null,
                                    null,
                                    Array.Empty<string>(),
                                    null,
                                    null,
                                    null,
                                    null,
                                    null,
                                    Array.Empty<string>(),
                                    Array.Empty<string>(),
                                    Array.Empty<string>(),
                                    Array.Empty<string>(),
                                    Array.Empty<string>(),
                                    Array.Empty<string>(),
                                    null
                                )
                            };
                        }

                        return hours.Select(h =>
                        {
                            scheduleTeacherAbsences.TryGetValue(h.ScheduleLessonId, out var teacherAbsence);

                            return new GetScheduleAndAbsencesByWeekVOHour(
                                hourNumber,
                                h.SubjectName,
                                h.SubjectTypeName,
                                topicTeachers[h.ScheduleLessonId].Any()
                                    ? topicTeachers[h.ScheduleLessonId].ToArray()
                                    : scheduleCurriculumsTeachers[h.CurriculumId]
                                        .Where(t => t.StartDate <= h.Date && (t.EndDate == null || t.EndDate >= h.Date))
                                        .Select(t => t.TeacherNames)
                                        .ToArray(),
                                teacherAbsence?.ScheduleLessonId,
                                teacherAbsence?.ReplTeacherName,
                                teacherAbsence?.ExtReplTeacherName,
                                teacherAbsence?.ReplTeacherIsNonSpecialist,
                                teacherAbsence?.IsEmptyHour,
                                absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.Excused)
                                    .Select(a => a.ClassNumber?.ToString() ?? a.StudentName).ToArray(),
                                absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.Unexcused)
                                    .Select(a => a.ClassNumber?.ToString() ?? a.StudentName).ToArray(),
                                absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.Late)
                                    .Select(a => a.ClassNumber?.ToString() ?? a.StudentName).ToArray(),
                                absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.DplrAbsence)
                                    .Select(a => a.ClassNumber?.ToString() ?? a.StudentName).ToArray(),
                                absences[h.ScheduleLessonId].Where(a => a.Type == AbsenceType.DplrAttendance)
                                    .Select(a => a.ClassNumber?.ToString() ?? a.StudentName).ToArray(),
                                scheduleTopics[h.ScheduleLessonId].ToArray(),
                                h.Location);
                        });
                    });

                    return new GetScheduleAndAbsencesByWeekVODay(
                            day,
                            date,
                            date.ToString("dddd"),
                            offDayDates.Contains(date),
                            true,
                            hours.ToArray()
                        );
                })
                .ToArray()
            ))
            .ToArray();
    }

    public async Task<GetAbsencesVO[]> GetAbsencesAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        string sql = $"""
            WITH Absences AS (
                SELECT
                    PersonId,
                    SUM(IIF(Type = {(int)AbsenceType.Late} AND Term = {(int)SchoolTerm.TermOne}, 1, 0)) AS FirstTermLateCount,
                    SUM(IIF(Type = {(int)AbsenceType.Unexcused} AND Term = {(int)SchoolTerm.TermOne}, 1, 0)) AS FirstTermUnexcusedCount,
                    SUM(IIF(Type = {(int)AbsenceType.Excused} AND Term = {(int)SchoolTerm.TermOne}, 1, 0)) AS FirstTermExcusedCount,
                    SUM(IIF(Type = {(int)AbsenceType.Late}, 1, 0)) AS WholeYearLateCount,
                    SUM(IIF(Type = {(int)AbsenceType.Unexcused}, 1, 0)) AS WholeYearUnexcusedCount,
                    SUM(IIF(Type = {(int)AbsenceType.Excused}, 1, 0)) AS WholeYearExcusedCount
                FROM
                    [school_books].[Absence]
                WHERE
                    SchoolYear = @schoolYear AND ClassBookId = @classBookId
                GROUP BY
                    PersonId
            ),
            CarriedAbsences AS (
                SELECT
                    PersonId,
                    FirstTermLateCount AS FirstTermCarriedLateCount,
                    FirstTermUnexcusedCount AS FirstTermCarriedUnexcusedCount,
                    FirstTermExcusedCount AS FirstTermCarriedExcusedCount,
                    (FirstTermLateCount + SecondTermLateCount) AS WholeYearCarriedLateCount,
                    (FirstTermUnexcusedCount + SecondTermUnexcusedCount) AS WholeYearCarriedUnexcusedCount,
                    (FirstTermExcusedCount + SecondTermExcusedCount) AS WholeYearCarriedExcusedCount
                FROM
                    [school_books].[ClassBookStudentCarriedAbsence] ca
                WHERE
                    SchoolYear = @schoolYear AND ClassBookId = @classBookId
            )
            SELECT
                COALESCE(a.PersonId, ca.PersonId) AS PersonId,
                COALESCE(a.FirstTermLateCount, 0) AS FirstTermLateCount,
                COALESCE(a.FirstTermUnexcusedCount, 0) AS FirstTermUnexcusedCount,
                COALESCE(a.FirstTermExcusedCount, 0) AS FirstTermExcusedCount,
                COALESCE(a.WholeYearLateCount, 0) AS WholeYearLateCount,
                COALESCE(a.WholeYearUnexcusedCount, 0) AS WholeYearUnexcusedCount,
                COALESCE(a.WholeYearExcusedCount, 0) AS WholeYearExcusedCount,
                COALESCE(ca.FirstTermCarriedLateCount, 0) AS FirstTermCarriedLateCount,
                COALESCE(ca.FirstTermCarriedUnexcusedCount, 0) AS FirstTermCarriedUnexcusedCount,
                COALESCE(ca.FirstTermCarriedExcusedCount, 0) AS FirstTermCarriedExcusedCount,
                COALESCE(ca.WholeYearCarriedLateCount, 0) AS WholeYearCarriedLateCount,
                COALESCE(ca.WholeYearCarriedUnexcusedCount, 0) AS WholeYearCarriedUnexcusedCount,
                COALESCE(ca.WholeYearCarriedExcusedCount, 0) AS WholeYearCarriedExcusedCount
            FROM
                Absences a
                FULL OUTER JOIN CarriedAbsences ca ON a.PersonId = ca.PersonId
            """;
        return await this.DbContext.Set<GetAbsencesVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId))
            .ToArrayAsync(ct);
    }

    public async Task<GetAbsencesByDateVO[]> GetAbsencesByDateAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        string sql = $"""
            SELECT
                a.PersonId,
                a.Date,
                CAST(SUM(IIF(a.Type = {(int)AbsenceType.Late}, 1, 0)) AS INT) AS LateCount,
                CAST(SUM(IIF(a.Type = {(int)AbsenceType.Unexcused}, 1, 0)) AS INT) AS UnexcusedCount,
                CAST(SUM(IIF(a.Type = {(int)AbsenceType.Excused}, 1, 0)) AS INT) AS ExcusedCount
            FROM
                [school_books].[Absence] a
            WHERE
                a.SchoolYear = @schoolYear AND
                a.ClassBookId = @classBookId
            GROUP BY
                a.PersonId,
                a.Date
            """;
        return await this.DbContext.Set<GetAbsencesByDateVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId))
            .ToArrayAsync(ct);
    }

    public async Task<GetGradeResultsVO[]> GetGradeResultsAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        var studentRetakeExams = (await (
            from gr in this.DbContext.Set<GradeResult>()

            join grs in this.DbContext.Set<GradeResultSubject>()
                on new { gr.SchoolYear, gr.GradeResultId } equals new { grs.SchoolYear, grs.GradeResultId }

            join c in this.DbContext.Set<Curriculum>() on grs.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId

            select new
            {
                gr.PersonId,
                RetakeExamSubject = string.Join(" ", s.SubjectName, st.Name)
            })
            .ToArrayAsync(ct))
            .ToLookup(grs => grs.PersonId, grs => grs.RetakeExamSubject);

        return await (
            from gr in this.DbContext.Set<GradeResult>()

            join p in this.DbContext.Set<Person>() on gr.PersonId equals p.PersonId

            join sc in this.DbContext.StudentsForClassBook(schoolYear, classId, classIsLvl2)
            on gr.PersonId equals sc.PersonId
            into j1 from sc in j1.DefaultIfEmpty()

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId

            orderby (sc.ClassNumber == null ? 99999 : sc.ClassNumber)

            select new GetGradeResultsVO(
                sc.ClassNumber,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                (bool?)sc.IsTransferred ?? true,
                gr.InitialResultType,
                studentRetakeExams[p.PersonId].ToArray()
            )
        ).ToArrayAsync(ct);
    }

    public async Task<GetFirstGradeResultsVO[]> GetFirstGradeResultsAsync(int schoolYear, int classBookId, CancellationToken ct)
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

        return await (
                from r in this.DbContext.Set<FirstGradeResult>()

                join p in this.DbContext.Set<Person>()
                on r.PersonId equals p.PersonId

                join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
                on r.PersonId equals sc.PersonId
                into g1 from sc in g1.DefaultIfEmpty()

                where r.SchoolYear == schoolYear &&
                    r.ClassBookId == classBookId

                orderby (sc.ClassNumber == null ? 99999 : sc.ClassNumber)

                select new GetFirstGradeResultsVO(
                    sc.ClassNumber,
                    StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                    (bool?)sc.IsTransferred ?? true,
                    r.QualitativeGrade,
                    r.SpecialGrade))
                .ToArrayAsync(ct);
    }

    public async Task<GetGradeResultSessionsVO[]> GetGradeResultSessionsAsync(int schoolYear, int classBookId, CancellationToken ct)
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

        return await (
                from gr in this.DbContext.Set<GradeResult>()

                join grs in this.DbContext.Set<GradeResultSubject>()
                on new { gr.SchoolYear, gr.GradeResultId } equals new { grs.SchoolYear, grs.GradeResultId }

                join c in this.DbContext.Set<Curriculum>() on grs.CurriculumId equals c.CurriculumId

                join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

                join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

                join p in this.DbContext.Set<Person>() on gr.PersonId equals p.PersonId

                join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
                on gr.PersonId equals sc.PersonId
                into g1 from sc in g1.DefaultIfEmpty()

                orderby sc.ClassNumber

                where gr.SchoolYear == schoolYear &&
                    gr.ClassBookId == classBookId

                select new GetGradeResultSessionsVO(
                    sc.ClassNumber,
                    StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                    (bool?)sc.IsTransferred ?? true,
                    string.Join(" / ", s.SubjectName, st.Name),
                    grs.Session1NoShow,
                    grs.Session1Grade,
                    grs.Session2NoShow,
                    grs.Session2Grade,
                    grs.Session3NoShow,
                    grs.Session3Grade,
                    gr.FinalResultType
                ))
                .ToArrayAsync(ct);
    }

    public async Task<GetSanctionsVO[]> GetSanctionsAsync(int schoolYear, int classBookId, CancellationToken ct)
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

        return await (
            from s in this.DbContext.Set<Sanction>()
            join st in this.DbContext.Set<SanctionType>() on s.SanctionTypeId equals st.Id

            join p in this.DbContext.Set<Person>() on s.PersonId equals p.PersonId
            join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            on s.PersonId equals sc.PersonId
            into g1 from sc in g1.DefaultIfEmpty()

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby (sc.ClassNumber == null ? 99999 : sc.ClassNumber)

            select new GetSanctionsVO(
                sc.ClassNumber,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                (bool?)sc.IsTransferred ?? true,
                st.Name,
                s.OrderNumber,
                s.OrderDate,
                s.CancelOrderNumber,
                s.CancelOrderDate))
            .ToArrayAsync(ct);
    }

    public async Task<GetSupportsVO[]> GetSupportsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var activities = (await (
            from s in this.DbContext.Set<Support>()

            join a in this.DbContext.Set<SupportActivity>()
                on new { s.SchoolYear, s.SupportId }
                equals new { a.SchoolYear, a.SupportId }

            join sat in this.DbContext.Set<SupportActivityType>() on a.SupportActivityTypeId equals sat.Id

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby a.SupportActivityId

            select new
            {
                s.SupportId,
                ActivityTypeName = sat.Name,
                a.Date,
                a.Target,
                a.Result

            })
            .ToArrayAsync(ct))
            .ToLookup(
                a => a.SupportId,
                a => new GetSupportsVOActivity(a.ActivityTypeName, a.Date, a.Target, a.Result));

        var difficulties = (await (
            from s in this.DbContext.Set<Support>()

            join d in this.DbContext.Set<SupportDifficulty>()
                on new { s.SchoolYear, s.SupportId }
                equals new { d.SchoolYear, d.SupportId }

            join sdt in this.DbContext.Set<SupportDifficultyType>() on d.SupportDifficultyTypeId equals sdt.Id

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new
            {
                s.SupportId,
                sdt.Name
            })
            .ToArrayAsync(ct))
            .ToLookup(
                d => d.SupportId,
                d => d.Name);

        var teachers = (await (
            from s in this.DbContext.Set<Support>()

            join st in this.DbContext.Set<SupportTeacher>()
                on new { s.SchoolYear, s.SupportId }
                equals new { st.SchoolYear, st.SupportId }

            join p in this.DbContext.Set<Person>() on st.PersonId equals p.PersonId
            where s.SchoolYear == schoolYear && s.ClassBookId == classBookId
            select new
            {
                s.SupportId,
                TeacherName = StringUtils.JoinNames(p.FirstName, p.LastName)
            }
            )
            .ToArrayAsync(ct))
            .ToLookup(
                d => d.SupportId,
                d => d.TeacherName);

        var students = (await (
            from s in this.DbContext.Set<Support>()

            join st in this.DbContext.Set<SupportStudent>()
                on new { s.SchoolYear, s.SupportId }
                equals new { st.SchoolYear, st.SupportId }
            join p in this.DbContext.Set<Person>()
                on st.PersonId
                equals p.PersonId

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName

            select new
            {
                s.SupportId,
                p.FirstName,
                p.MiddleName,
                p.LastName
            })
            .ToArrayAsync(ct))
            .ToLookup(
                ss => ss.SupportId,
                ss => StringUtils.JoinNames(ss.FirstName, ss.MiddleName, ss.LastName));

        return await (
           from s in this.DbContext.Set<Support>()

           where s.SchoolYear == schoolYear &&
               s.ClassBookId == classBookId

           orderby s.SupportId

           select new GetSupportsVO(
               students[s.SupportId].ToArray(),
               string.Join(", ", difficulties[s.SupportId]),
               s.Description,
               s.ExpectedResult,
               s.EndDate,
               teachers[s.SupportId].ToArray(),
               activities[s.SupportId].ToArray()))
           .ToArrayAsync(ct);
    }

    public async Task<GetNotesVO[]> GetNotesAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var students = (await (
            from s in this.DbContext.Set<Note>()

            join st in this.DbContext.Set<NoteStudent>()
                on new { s.SchoolYear, s.NoteId }
                equals new { st.SchoolYear, st.NoteId }
            join p in this.DbContext.Set<Person>()
                on st.PersonId
                equals p.PersonId

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName

            select new
            {
                s.NoteId,
                p.FirstName,
                p.MiddleName,
                p.LastName
            })
            .ToArrayAsync(ct))
            .ToLookup(
                ss => ss.NoteId,
                ss => StringUtils.JoinNames(ss.FirstName, ss.MiddleName, ss.LastName));

        return await (
            from n in this.DbContext.Set<Note>()

            where n.SchoolYear == schoolYear &&
                n.ClassBookId == classBookId

            orderby n.NoteId

            select new GetNotesVO(
                students[n.NoteId].ToArray(),
                n.Description,
                n.IsForAllStudents))
            .ToArrayAsync(ct);
    }

    public async Task<GetAttendancesVO[]> GetAttendancesAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
                from a in this.DbContext.Set<Attendance>()

                where a.SchoolYear == schoolYear &&
                    a.ClassBookId == classBookId

                select new GetAttendancesVO(
                    a.PersonId,
                    a.Type,
                    a.Date)
            ).ToArrayAsync(ct);
    }

    public async Task<GetPgResultsVO[]> GetPgResultsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from pgr in this.DbContext.Set<PgResult>()

            join p in this.DbContext.Set<Person>()
                on pgr.PersonId
                equals p.PersonId

            join s in this.DbContext.Set<Subject>() on pgr.SubjectId equals s.SubjectId
            into g2 from s in g2.DefaultIfEmpty()

            where pgr.SchoolYear == schoolYear &&
                pgr.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName

            select new GetPgResultsVO(
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                s.SubjectName,
                pgr.StartSchoolYearResult,
                pgr.EndSchoolYearResult)
            ).ToArrayAsync(ct);
    }

    public async Task<GetIndividualWorksVO[]> GetIndividualWorksAsync(int schoolYear, int classBookId, CancellationToken ct)
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

        return await (
            from iw in this.DbContext.Set<IndividualWork>()

            join p in this.DbContext.Set<Person>()
            on iw.PersonId equals p.PersonId

            join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
            on iw.PersonId equals sc.PersonId
            into g1 from sc in g1.DefaultIfEmpty()

            where iw.SchoolYear == schoolYear &&
                iw.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName, iw.Date

            select new GetIndividualWorksVO(
                sc.ClassNumber,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                (bool?)sc.IsTransferred ?? true,
                iw.Date,
                iw.IndividualWorkActivity))
            .ToArrayAsync(ct);
    }

    public async Task<GetClassBookInfoVO> GetClassBookInfoAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new GetClassBookInfoVO(
                cb.InstId,
                cb.ClassId,
                cb.ClassIsLvl2)
        ).SingleAsync(ct);
    }

    public async Task<GetCurriculumInfoVO[]> GetCurriculumAsync(int schoolYear, int classBookId, int classId, bool classIsLvl2, CancellationToken ct)
    {
        return await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classId, classIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join cg in this.DbContext.Set<ClassBookCurriculumGradeless>().Where(cg => cg.SchoolYear == schoolYear && cg.ClassBookId == classBookId)
            on c.CurriculumId equals cg.CurriculumId
            into j1 from cg in j1.DefaultIfEmpty()

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where (
                    cg == null &&
                    cc.IsValid &&
                    c.IsValid
                ) ||
                this.DbContext.Set<Grade>()
                    .Where(g =>
                        g.SchoolYear == schoolYear &&
                        g.ClassBookId == classBookId &&
                        g.CurriculumId == c.CurriculumId)
                    .Any()

            select new GetCurriculumInfoVO(
                c.CurriculumId,
                s.SubjectName,
                st.Name)
        ).ToArrayAsync(ct);
    }

    public async Task<GetReplrParticipationsVO[]> GetReplrParticipationsAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from rp in this.DbContext.Set<ReplrParticipation>()
            join rpt in this.DbContext.Set<ReplrParticipationType>() on rp.ReplrParticipationTypeId equals rpt.Id
            join i in this.DbContext.Set<InstitutionSchoolYear>() on new { rp.InstId, rp.SchoolYear } equals new { InstId = (int?)i.InstitutionId, i.SchoolYear }
            into j1 from i in j1.DefaultIfEmpty()

            where rp.SchoolYear == schoolYear &&
                rp.ClassBookId == classBookId
            select new GetReplrParticipationsVO(
                rpt.Name,
                rp.Date,
                rp.Topic,
                i.Name,
                rp.Attendees)
        ).ToArrayAsync(ct);
    }

    public async Task<GetPerformancesVO[]> GetPerformancesAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<Performance>()
            join pt in this.DbContext.Set<PerformanceType>() on p.PerformanceTypeId equals pt.Id
            where p.SchoolYear == schoolYear &&
                p.ClassBookId == classBookId
            select new GetPerformancesVO(
                pt.Name,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Location,
                p.StudentAwards)
        ).ToArrayAsync(ct);
    }
}
