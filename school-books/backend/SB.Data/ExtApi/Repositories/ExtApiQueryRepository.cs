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

internal partial class ExtApiQueryRepository : Repository, IExtApiQueryRepository
{
    private readonly ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository;
    public ExtApiQueryRepository(
        UnitOfWork unitOfWork,
        ISchoolYearSettingsQueryRepository schoolYearSettingsQueryRepository)
        : base(unitOfWork)
    {
        this.schoolYearSettingsQueryRepository = schoolYearSettingsQueryRepository;
    }

    public async Task<AbsenceDO[]> AbsencesGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Absence>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { a.SchoolYear, a.ScheduleLessonId }
            equals new { sl.SchoolYear, sl.ScheduleLessonId }

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId

            select new AbsenceDO
            {
                AbsenceId = a.AbsenceId,
                PersonId = a.PersonId,
#pragma warning disable CS0618 // member is obsolete
                CurriculumId = sl.CurriculumId,
#pragma warning restore CS0618 // member is obsolete
                Date = a.Date,
                Type = a.Type,
                ExcusedReason = a.ExcusedReasonId,
                ExcusedReasonComment = a.ExcusedReasonComment,
                ScheduleLessonId = a.ScheduleLessonId,
            }
        ).ToArrayAsync(ct);
    }

    public async Task<AttendanceDO[]> AttendancesGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from a in this.DbContext.Set<Attendance>()

            where a.SchoolYear == schoolYear &&
                a.ClassBookId == classBookId

            select new AttendanceDO
            {
                AttendanceId = a.AttendanceId,
                PersonId = a.PersonId,
                Date = a.Date,
                Type = a.Type,
                ExcusedReason = a.ExcusedReasonId,
                ExcusedReasonComment = a.ExcusedReasonComment,
            }
         ).ToArrayAsync(ct);
    }

    public async Task<ClassBookDO[]> ClassBooksGetAllAsync(int schoolYear, int institutionId, CancellationToken ct)
    {
        return await (
            from cb in this.DbContext.Set<ClassBook>()

            where cb.SchoolYear == schoolYear &&
                cb.InstId == institutionId

            select new ClassBookDO
            {
                ClassBookId = cb.ClassBookId,
                ClassId = cb.ClassId,
                BookType = cb.BookType,
                BasicClassId = cb.BasicClassId,
                BookName = cb.BookName,
                IsFinalized = cb.IsFinalized,
                IsValid = cb.IsValid,
                InvalidationDate = cb.IsValid ? null : cb.ModifyDate
            }
         ).ToArrayAsync(ct);
    }

    public async Task<ExamDO[]> ExamsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from e in this.DbContext.Set<Exam>()

            where e.SchoolYear == schoolYear &&
                e.ClassBookId == classBookId

            select new ExamDO
            {
                ExamId = e.ExamId,
                Type = e.Type,
                CurriculumId = e.CurriculumId,
                Date = e.Date,
                Description = e.Description,
            }
        ).ToArrayAsync(ct);
    }

    public async Task<FirstGradeResultDO[]> FirstGradeResultsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from fgr in this.DbContext.Set<FirstGradeResult>()

            where fgr.SchoolYear == schoolYear &&
                fgr.ClassBookId == classBookId

            select new FirstGradeResultDO
            {
                FirstGradeResultId = fgr.FirstGradeResultId,
                PersonId = fgr.PersonId,
                QualitativeGrade = fgr.QualitativeGrade,
                SpecialGrade = fgr.SpecialGrade
            }
        ).ToArrayAsync(ct);
    }

    public async Task<GradeResultDO[]> GradeResultsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var gradeResults = await (
            from gr in this.DbContext.Set<GradeResult>()

            where gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId

            select new
            {
                GradeResultId = gr.GradeResultId,
                PersonId = gr.PersonId,
                InitialResultType = gr.InitialResultType,
                FinalResultType = gr.FinalResultType,
            }
        ).ToArrayAsync(ct);

        var subjects = (await (
            from grs in this.DbContext.Set<GradeResultSubject>()

            where grs.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(gradeResults
                        .Select(gr => gr.GradeResultId)
                        .ToArray())
                    .Any(id => grs.GradeResultId == id.Id)

            select new
            {
                GradeResultId = grs.GradeResultId,
                CurriculumId = grs.CurriculumId,
                Session1Grade = grs.Session1Grade,
                Session1NoShow = grs.Session1NoShow,
                Session2Grade = grs.Session2Grade,
                Session2NoShow = grs.Session2NoShow,
                Session3Grade = grs.Session3Grade,
                Session3NoShow = grs.Session3NoShow,
            }
        ).ToArrayAsync(ct))
        .ToLookup(s => s.GradeResultId, s =>
            new GradeResultCurriculumDO
            {
                CurriculumId = s.CurriculumId,
                Session1Grade = s.Session1Grade,
                Session1NoShow = s.Session1NoShow,
                Session2Grade = s.Session2Grade,
                Session2NoShow = s.Session2NoShow,
                Session3Grade = s.Session3Grade,
                Session3NoShow = s.Session3NoShow
            });

        return gradeResults.Select(gr =>
            new GradeResultDO
            {
                GradeResultId = gr.GradeResultId,
                PersonId = gr.PersonId,
                InitialResultType = gr.InitialResultType,
                RetakeExamCurriculums = subjects[gr.GradeResultId].ToArray(),
                FinalResultType = gr.FinalResultType,
            })
        .ToArray();
    }

    public async Task<GradeDO[]> GradesGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        string sql = """
            SELECT
                g.GradeId,
                g.PersonId,
                g.CurriculumId,
                g.Date,
                g.Category,
                g.Type,
                g.DecimalGrade,
                g.QualitativeGrade,
                g.SpecialGrade,
                g.Comment,
                g.ScheduleLessonId,
                g.Term
            FROM
                [school_books].[Grade] g
            WHERE
                g.SchoolYear = @schoolYear AND
                g.ClassBookId = @classBookId
            """;
        return await this.DbContext.Set<GradeDO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId))
            .ToArrayAsync(ct);
    }

    public async Task<NoteDO[]> NotesGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from n in this.DbContext.Set<Note>()

            where n.SchoolYear == schoolYear &&
                n.ClassBookId == classBookId

            select new NoteDO
            {
                NoteId = n.NoteId,
                Description = n.Description,
                IsForAllStudents = n.IsForAllStudents,
                StudentIds = n.Students.Select(s => s.PersonId).ToArray(),
#pragma warning disable CS0618 // member is obsolete
                Students = n.Students.Select(s => new NoteStudentDO { PersonId = s.PersonId }).ToArray(),
#pragma warning restore CS0618 // member is obsolete
            }
        ).ToArrayAsync(ct);
    }

    public async Task<OffDayDO[]> OffDaysGetAllAsync(int schoolYear, int institutionId, CancellationToken ct)
    {
        return await (
            from od in this.DbContext.Set<OffDay>()

            where od.SchoolYear == schoolYear &&
                od.InstId == institutionId

            select new OffDayDO
            {
                OffDayId = od.OffDayId,
                From = od.From,
                To = od.To,
                Description = od.Description,
                IsForAllClasses = od.IsForAllClasses,
                IsPgOffProgramDay = od.IsPgOffProgramDay,
                BasicClassIds =
                    (from odc in this.DbContext.Set<OffDayClass>()
                     where odc.SchoolYear == schoolYear
                         && odc.OffDayId == od.OffDayId
                     select odc.BasicClassId)
                    .ToArray(),
                ClassBookIds =
                    (from odcb in this.DbContext.Set<OffDayClassBook>()
                     where odcb.SchoolYear == schoolYear
                         && odcb.OffDayId == od.OffDayId
                     select odcb.ClassBookId)
                    .ToArray(),
            }
        ).ToArrayAsync(ct);
    }

    public async Task<ParentMeetingDO[]> ParentMeetingsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from pm in this.DbContext.Set<ParentMeeting>()

            where pm.SchoolYear == schoolYear &&
                pm.ClassBookId == classBookId

            select new ParentMeetingDO
            {
                ParentMeetingId = pm.ParentMeetingId,
                Date = pm.Date,
                StartTime = pm.StartTime.ToString("hh\\:mm"),
                Location = pm.Location,
                Title = pm.Title,
                Description = pm.Description,
            }
        ).ToArrayAsync(ct);
    }

    public async Task<PgResultDO[]> PgResultsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<PgResult>()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId

            select new PgResultDO
            {
                PgResultId = r.PgResultId,
                PersonId = r.PersonId,
                SubjectId = r.SubjectId,
#pragma warning disable CS0618 // member is obsolete
                CurriculumId = r.CurriculumId,
#pragma warning restore CS0618 // member is obsolete
                StartSchoolYearResult = r.StartSchoolYearResult,
                EndSchoolYearResult = r.EndSchoolYearResult,
            }
        ).ToArrayAsync(ct);
    }

    public async Task<RemarkDO[]> RemarksGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<Remark>()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId

            select new RemarkDO
            {
                RemarkId = r.RemarkId,
                PersonId = r.PersonId,
                CurriculumId = r.CurriculumId,
                Date = r.Date,
                Type = r.Type,
                Description = r.Description,
            }
        ).ToArrayAsync(ct);
    }

    public async Task<SanctionDO[]> SanctionsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Sanction>()

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new SanctionDO
            {
                SanctionId = s.SanctionId,
                PersonId = s.PersonId,
                Type = s.SanctionTypeId,
                OrderNumber = s.OrderNumber,
                OrderDate = s.OrderDate,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Description = s.Description,
                CancelOrderNumber = s.CancelOrderNumber,
                CancelOrderDate = s.CancelOrderDate,
                CancelReason = s.CancelReason,
                RuoOrderNumber = s.RuoOrderNumber,
                RuoOrderDate = s.RuoOrderDate
            }
        ).ToArrayAsync(ct);
    }

    public async Task<ScheduleDO[]> SchedulesGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var weeks = (await (
            from s in this.DbContext.Set<Schedule>()
            join sd in this.DbContext.Set<ScheduleDate>()
                on new { s.SchoolYear, s.ScheduleId }
                equals new { sd.SchoolYear, sd.ScheduleId }

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new
            {
                s.ScheduleId,
                sd.Year,
                sd.WeekNumber
            })
            .ToArrayAsync(ct))
            .GroupBy(s => s.ScheduleId)
            .ToDictionary(g => g.Key, g => g.GroupBy(sd => new { sd.Year, sd.WeekNumber })
                .Select(gsd => new ScheduleWeekDO { Year = gsd.Key.Year, WeekNumber = gsd.Key.WeekNumber })
                .OrderBy(w => w.Year)
                .ThenBy(w => w.WeekNumber)
                .ToArray());

        var days = (await (
            from s in this.DbContext.Set<Schedule>()
            join sh in this.DbContext.Set<ScheduleHour>()
                on new { s.SchoolYear, s.ScheduleId }
                equals new { sh.SchoolYear, sh.ScheduleId }

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new
            {
                s.ScheduleId,
                sh.Day,
                sh.HourNumber,
                sh.CurriculumId,
                sh.Location
            })
            .ToArrayAsync(ct))
            .GroupBy(s => s.ScheduleId)
            .ToDictionary(g => g.Key, g => g.GroupBy(sh => sh.Day)
                .Select(g =>
                    new ScheduleDayDO
                    {
                        Day = g.Key,
                        Hours = g.GroupBy(d => d.HourNumber)
                            .Select(gd =>
                                new ScheduleHourDO
                                {
                                    HourNumber = gd.Key,
                                    #pragma warning disable CS0618 // member is obsolete
                                    GroupCurriculumIds = gd.Select(h => h.CurriculumId).ToArray(),
                                    #pragma warning disable CS0618 // member is obsolete
                                    Groups = gd.Select(h =>
                                        new ScheduleHourGroupDO
                                        {
                                            CurriculumId = h.CurriculumId,
                                            Location = h.Location
                                        })
                                        .ToArray()
                                })
                            .ToArray()
                    })
                .OrderBy(d => d.Day)
                .ThenBy(d => d.Hours.Select(h => h.HourNumber))
                .ToArray());

        var lessons = (await (
            from s in this.DbContext.Set<Schedule>()
            join sl in this.DbContext.Set<ScheduleLesson>()
                on new { s.SchoolYear, s.ScheduleId }
                equals new { sl.SchoolYear, sl.ScheduleId }

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby sl.Date, sl.HourNumber

            select new
            {
                s.ScheduleId,
                sl.ScheduleLessonId,
                sl.Date,
                sl.Day,
                sl.HourNumber,
                sl.CurriculumId
            })
            .ToArrayAsync(ct))
            .GroupBy(s => s.ScheduleId)
            .ToDictionary(g => g.Key, g => g.Select(sl =>
                new ScheduleLessonDO
                {
                    ScheduleLessonId = sl.ScheduleLessonId,
                    Date = sl.Date,
                    Day = sl.Day,
                    HourNumber = sl.HourNumber,
                    CurriculumId = sl.CurriculumId
                })
                .OrderBy(sl => sl.Date)
                .ThenBy(sl => sl.HourNumber)
                .ToArray());

        var adhocShifts = (await (
            from s in this.DbContext.Set<Schedule>()
            join shift in this.DbContext.Set<Shift>()
                on new { s.SchoolYear, s.ShiftId }
                equals new { shift.SchoolYear, shift.ShiftId }
            join sh in this.DbContext.Set<ShiftHour>()
                on new { shift.SchoolYear, shift.ShiftId }
                equals new { sh.SchoolYear, sh.ShiftId }

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                shift.IsAdhoc

            select new
            {
                s.ScheduleId,
                shift.IsMultiday,
                sh.Day,
                sh.HourNumber,
                sh.StartTime,
                sh.EndTime
            })
            .ToArrayAsync(ct))
            .GroupBy(s => s.ScheduleId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    g.First().IsMultiday,
                    Hours = g.Select(sh =>
                        new ScheduleAdhocShiftHourDO
                        {
                            Day = sh.Day,
                            HourNumber = sh.HourNumber,
                            StartTime = sh.StartTime.ToString("hh\\:mm"),
                            EndTime = sh.EndTime.ToString("hh\\:mm")
                        })
                        .OrderBy(sh => sh.Day)
                        .ThenBy(sh => sh.HourNumber)
                        .ToArray()
                });

        return (await (
            from s in this.DbContext.Set<Schedule>()

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new
            {
                s.ScheduleId,
                s.IsIndividualSchedule,
                s.PersonId,
                s.Term,
                s.StartDate,
                s.EndDate,
                s.IncludesWeekend,
                s.ShiftId
            })
            .ToArrayAsync(ct))
            .Select(s =>
            {
                var adhocShift = adhocShifts.GetValueOrDefault(s.ScheduleId);
                return new ScheduleDO
                {
                    ScheduleId = s.ScheduleId,
                    IsIndividualCurriculum = s.IsIndividualSchedule,
                    PersonId = s.PersonId,
                    Term = s.Term,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IncludesWeekend = s.IncludesWeekend,
                    HasAdhocShift = adhocShift != null,
                    AdhocShiftIsMultiday = adhocShift?.IsMultiday,
                    AdhocShiftHours = adhocShift?.Hours,
                    ShiftId = adhocShift == null ? s.ShiftId : null,
                    Weeks = weeks.GetValueOrDefault(s.ScheduleId) ?? Array.Empty<ScheduleWeekDO>(),
                    Days = days.GetValueOrDefault(s.ScheduleId) ?? Array.Empty<ScheduleDayDO>(),
                    Lessons = lessons.GetValueOrDefault(s.ScheduleId) ?? Array.Empty<ScheduleLessonDO>()
                };
            })
            .ToArray();
    }

    public async Task<ShiftDO[]> ShiftsGetAllAsync(int schoolYear, int institutionId, CancellationToken ct)
    {
        return (await (
            from s in this.DbContext.Set<Shift>()
            join sh in this.DbContext.Set<ShiftHour>()
            on new { s.SchoolYear, s.ShiftId } equals new { sh.SchoolYear, sh.ShiftId }

            where s.SchoolYear == schoolYear &&
                s.InstId == institutionId &&
                !s.IsAdhoc

            select new
            {
                s.ShiftId,
                s.Name,
                s.IsMultiday,
                sh.Day,
                sh.HourNumber,
                StartTime = sh.StartTime.ToString("hh\\:mm"),
                EndTime = sh.EndTime.ToString("hh\\:mm")
            }
        ).ToArrayAsync(ct))
        .GroupBy(s => new { s.ShiftId, s.Name, s.IsMultiday })
        .Select(g =>
            new ShiftDO
            {
                ShiftId = g.Key.ShiftId,
                Name = g.Key.Name,
                IsMultiday = g.Key.IsMultiday,
                Hours = g
                    .Where(sh => g.Key.IsMultiday || sh.Day == 1)
                    .Select(h => new ShiftHourDO
                    {
                        Day = h.Day,
                        HourNumber = h.HourNumber,
                        StartTime = h.StartTime,
                        EndTime = h.EndTime
                    })
                    .ToArray()
            })
        .ToArray();
    }

    public async Task<TopicDO[]> TopicsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        var topics = await (
            from t in this.DbContext.Set<Topic>()

            where t.SchoolYear == schoolYear &&
                t.ClassBookId == classBookId

            select new
            {
                t.TopicId,
                t.Date,
                Titles = t.Titles.Select(tt => tt.Title).ToArray(),
                t.ScheduleLessonId,
            }
        ).ToArrayAsync(ct);

        return topics.Select(t =>
            new TopicDO
            {
                TopicId = t.TopicId,
                Date = t.Date,
#pragma warning disable CS0618 // member is obsolete
                Title = t.Titles.First(),
#pragma warning restore CS0618 // member is obsolete
                Titles = t.Titles,
                ScheduleLessonId = t.ScheduleLessonId,
            }
        ).ToArray();
    }

    public async Task<SchoolYearDateInfoDO[]> SchoolYearSettingsGetAllAsync(
        int schoolYear,
        int institutionId,
        CancellationToken ct)
    {
        var settings = await (
            from sys in this.DbContext.Set<SchoolYearSettings>()

            where sys.SchoolYear == schoolYear &&
                sys.InstId == institutionId

            select new
            {
                sys.SchoolYearSettingsId,
                sys.SchoolYearStartDate,
                sys.FirstTermEndDate,
                sys.SecondTermStartDate,
                sys.SchoolYearEndDate,
                sys.Description,
                sys.IsForAllClasses,
                BasicClassIds =
                    (from dic in this.DbContext.Set<SchoolYearSettingsClass>()
                     where dic.SchoolYear == schoolYear
                         && dic.SchoolYearSettingsId == sys.SchoolYearSettingsId
                     select dic.BasicClassId)
                    .ToArray(),
                ClassBookIds =
                    (from dicb in this.DbContext.Set<SchoolYearSettingsClassBook>()
                     where dicb.SchoolYear == schoolYear
                         && dicb.SchoolYearSettingsId == sys.SchoolYearSettingsId
                     select dicb.ClassBookId)
                    .ToArray(),
            }
        ).ToArrayAsync(ct);

        if (settings.Any(s =>
            s.SchoolYearStartDate == null ||
            s.FirstTermEndDate == null ||
            s.SecondTermStartDate == null ||
            s.SchoolYearEndDate == null))
        {
            bool isSportSchool = await this.schoolYearSettingsQueryRepository.IsSportSchoolAsync(schoolYear, institutionId, ct);
            var defaults = await this.schoolYearSettingsQueryRepository.GetDefaultAsync(schoolYear, ct);

            DateTime defaultSchoolYearStartDate = isSportSchool ? defaults.SportSchoolYearStartDate : defaults.OtherSchoolYearStartDate;
            DateTime defaultFirstTermEndDate = isSportSchool ? defaults.SportFirstTermEndDate : defaults.OtherFirstTermEndDate;
            DateTime defaultSecondTermStartDate = isSportSchool ? defaults.SportSecondTermStartDate : defaults.OtherSecondTermStartDate;
            DateTime defaultSchoolYearEndDate = isSportSchool ? defaults.SportSchoolYearEndDate : defaults.OtherSchoolYearEndDate;

            return settings.Select(s =>
                new SchoolYearDateInfoDO
                {
                    SchoolYearDateInfoId = s.SchoolYearSettingsId,
                    SchoolYearStartDate = s.SchoolYearStartDate ?? defaultSchoolYearStartDate,
                    FirstTermEndDate = s.FirstTermEndDate ?? defaultFirstTermEndDate,
                    SecondTermStartDate = s.SecondTermStartDate ?? defaultSecondTermStartDate,
                    SchoolYearEndDate = s.SchoolYearEndDate ?? defaultSchoolYearEndDate,
                    Description = s.Description,
                    IsForAllClasses = s.IsForAllClasses,
                    BasicClassIds = s.BasicClassIds,
                    ClassBookIds = s.ClassBookIds,
                })
                .ToArray();
        }

        return settings.Select(s =>
            new SchoolYearDateInfoDO
            {
                SchoolYearDateInfoId = s.SchoolYearSettingsId,
                SchoolYearStartDate = s.SchoolYearStartDate!.Value,
                FirstTermEndDate = s.FirstTermEndDate!.Value,
                SecondTermStartDate = s.SecondTermStartDate!.Value,
                SchoolYearEndDate = s.SchoolYearEndDate!.Value,
                Description = s.Description,
                IsForAllClasses = s.IsForAllClasses,
                BasicClassIds = s.BasicClassIds,
                ClassBookIds = s.ClassBookIds,
            })
            .ToArray();
    }

    public async Task<SupportDO[]> SupportsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from s in this.DbContext.Set<Support>()

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            select new SupportDO
            {
                SupportId = s.SupportId,
                Description = s.Description,
                ExpectedResult = s.ExpectedResult,
                EndDate = s.EndDate,
                SupportDifficultyTypeIds = s.DifficultyTypes.Select(d => d.SupportDifficultyTypeId).ToArray(),
                TeacherIds = s.Teachers.Select(t => t.PersonId).ToArray(),
                IsForAllStudents = s.IsForAllStudents,
                StudentIds = s.Students.Select(s => s.PersonId).ToArray(),
                Activities = s.Activities.Select(s =>
                    new SupportActivityDO
                    {
                        SupportActivityTypeId = s.SupportActivityTypeId,
                        Target = s.Target,
                        Result = s.Result,
                        Date = s.Date
                    }
                ).ToArray(),
#pragma warning disable CS0618 // member is obsolete
                Students = s.Students.Select(s => new SupportStudentDO { PersonId = s.PersonId }).ToArray(),
#pragma warning restore CS0618 // member is obsolete
            }
        ).ToArrayAsync(ct);
    }

    public async Task<IndividualWorkDO[]> IndividualWorksGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from iw in this.DbContext.Set<IndividualWork>()

            where iw.SchoolYear == schoolYear &&
                iw.ClassBookId == classBookId

            select new IndividualWorkDO
            {
                IndividualWorkId = iw.IndividualWorkId,
                PersonId = iw.PersonId,
                Date = iw.Date,
                IndividualWorkActivity = iw.IndividualWorkActivity
            }
        ).ToArrayAsync(ct);
    }

    public async Task<PerformanceDO[]> PerformancesGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<Performance>()

            where p.SchoolYear == schoolYear &&
                p.ClassBookId == classBookId

            select new PerformanceDO
            {
                PerformanceId = p.PerformanceId,
                PerformanceTypeId = p.PerformanceTypeId,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Description = p.Description,
                Location = p.Location,
                StudentAwards = p.StudentAwards
            }
        ).ToArrayAsync(ct);
    }

    public async Task<ReplrParticipationDO[]> ReplrParticipationsGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from rp in this.DbContext.Set<ReplrParticipation>()

            where rp.SchoolYear == schoolYear &&
                rp.ClassBookId == classBookId

            select new ReplrParticipationDO
            {
                ReplrParticipationId = rp.ReplrParticipationId,
                ReplrParticipationTypeId = rp.ReplrParticipationTypeId,
                Date = rp.Date,
                Topic = rp.Topic,
                Attendees = rp.Attendees,
                InstId = rp.InstId
            }
        ).ToArrayAsync(ct);
    }

    public async Task<TeacherAbsenceDO[]> TeacherAbsencesGetAllAsync(int schoolYear, int institutionId, CancellationToken ct)
    {
        return await (
            from ta in this.DbContext.Set<TeacherAbsence>()

            where ta.SchoolYear == schoolYear && ta.InstId == institutionId

            select new TeacherAbsenceDO
            {
                TeacherAbsenceId = ta.TeacherAbsenceId,
                TeacherPersonId = ta.TeacherPersonId,
                StartDate = ta.StartDate,
                EndDate = ta.EndDate,
                Reason = ta.Reason,
                Hours = ta.Hours.Select(s =>
                    new TeacherAbsenceHourDO
                    {
                        ScheduleLessonId = s.ScheduleLessonId,
                        Type =
                            s.ReplTeacherPersonId == null && string.IsNullOrEmpty(s.ExtReplTeacherName) ? TeacherAbsenceHourType.EmptyHour :
                            s.ExtReplTeacherName != null ? TeacherAbsenceHourType.ExtTeacher :
                            s.ReplTeacherIsNonSpecialist == true ? TeacherAbsenceHourType.NonSpecialist :
                            TeacherAbsenceHourType.Specialist,
                        ReplTeacherPersonId = s.ReplTeacherPersonId
                    }
                ).ToArray()
            }
        ).ToArrayAsync(ct);
    }

    public async Task<AdditionalActivityDO[]> AdditionalActivitiesGetAllAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await (
            from aa in this.DbContext.Set<AdditionalActivity>()

            where aa.SchoolYear == schoolYear &&
                aa.ClassBookId == classBookId

            select new AdditionalActivityDO
            {
                AdditionalActivityId = aa.AdditionalActivityId,
                Year = aa.Year,
                WeekNumber = aa.WeekNumber,
                Activity = aa.Activity
            }
        ).ToArrayAsync(ct);
    }

    public async Task<StudentActivitiesDO[]> StudentActivitiesGetAllAsync(
        int schoolYear,
        int classBookId,
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

        return await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join sa in this.DbContext.Set<ClassBookStudentActivity>().Where(sa => sa.SchoolYear == schoolYear && sa.ClassBookId == classBookId)
            on s.PersonId equals sa.PersonId
            into j0
            from sa in j0.DefaultIfEmpty()

            orderby (s.ClassNumber == null ? 99999 : s.ClassNumber)

            select new StudentActivitiesDO
            {
                PersonId = s.PersonId,
                Activities = sa.Activities
            }
        ).ToArrayAsync(ct);
    }

    public async Task<StudentClassNumberDO[]> StudentClassNumbersGetAllAsync(
        int schoolYear,
        int classBookId,
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

        return await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            orderby (s.ClassNumber == null ? 99999 : s.ClassNumber)

            select new StudentClassNumberDO
            {
                PersonId = s.PersonId,
                ClassNumber = s.ClassNumber
            }
        ).ToArrayAsync(ct);
    }

    public async Task<StudentGradelessCurriculumsDO[]> StudentGradelessCurriculumsGetAllAsync(
        int schoolYear,
        int classBookId,
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

        return (await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join grdls in this.DbContext.Set<ClassBookStudentGradeless>().Where(sa => sa.SchoolYear == schoolYear && sa.ClassBookId == classBookId)
            on s.PersonId equals grdls.PersonId
            into j0
            from grdls in j0.DefaultIfEmpty()

            select new
            {
                s.PersonId,
                s.ClassNumber,
                CurriculumId = (int?)grdls.CurriculumId,
                WithoutFirstTermGrade = (bool?)grdls.WithoutFirstTermGrade,
                WithoutSecondTermGrade = (bool?)grdls.WithoutSecondTermGrade,
                WithoutFinalGrade = (bool?)grdls.WithoutFinalGrade
            }
        ).ToArrayAsync(ct))
        .GroupBy(s => new { s.PersonId, s.ClassNumber })
        .OrderBy(g => g.Key.ClassNumber == null ? 99999 : g.Key.ClassNumber)
        .Select(g =>
            new StudentGradelessCurriculumsDO
            {
                PersonId = g.Key.PersonId,
                GradelessCurriculums =
                    g.Where(grdls => grdls.CurriculumId != null)
                    .Select(grdls =>
                        new StudentGradelessCurriculumDO
                        {
                            CurriculumId = grdls.CurriculumId!.Value,
                            WithoutFirstTermGrade = grdls.WithoutFirstTermGrade!.Value,
                            WithoutSecondTermGrade = grdls.WithoutSecondTermGrade!.Value,
                            WithoutFinalGrade = grdls.WithoutFinalGrade!.Value,
                        })
                    .ToArray()
            })
        .ToArray();
    }

    public async Task<StudentSpecialNeedCurriculumsDO[]> StudentSpecialNeedCurriculumsGetAllAsync(
        int schoolYear,
        int classBookId,
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

        return (await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join sn in this.DbContext.Set<ClassBookStudentSpecialNeeds>().Where(sa => sa.SchoolYear == schoolYear && sa.ClassBookId == classBookId)
            on s.PersonId equals sn.PersonId
            into j0
            from sn in j0.DefaultIfEmpty()

            join fgrsn in this.DbContext.Set<ClassBookStudentFirstGradeResultSpecialNeeds>().Where(sa => sa.SchoolYear == schoolYear && sa.ClassBookId == classBookId)
            on s.PersonId equals fgrsn.PersonId
            into j1
            from fgrsn in j1.DefaultIfEmpty()

            select new
            {
                s.PersonId,
                s.ClassNumber,
                HasSpecialNeedFirstGradeResult = fgrsn != null,
                CurriculumId = (int?)sn.CurriculumId,
            }
        ).ToArrayAsync(ct))
        .GroupBy(s => new { s.PersonId, s.ClassNumber, s.HasSpecialNeedFirstGradeResult })
        .OrderBy(g => g.Key.ClassNumber == null ? 99999 : g.Key.ClassNumber)
        .Select(g =>
            new StudentSpecialNeedCurriculumsDO
            {
                PersonId = g.Key.PersonId,
                StudentSpecialNeedCurriculumIds =
                    g.Where(sn => sn.CurriculumId != null)
                    .Select(sn => sn.CurriculumId!.Value)
                    .ToArray(),
                HasSpecialNeedFirstGradeResult = g.Key.HasSpecialNeedFirstGradeResult
            })
        .ToArray();
    }

    public async Task<StudentCarriedAbsencesDO[]> StudentCarriedAbsencesGetAllAsync(
        int schoolYear,
        int classBookId,
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

        return await (
            from s in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join ca in this.DbContext.Set<ClassBookStudentCarriedAbsence>().Where(sa => sa.SchoolYear == schoolYear && sa.ClassBookId == classBookId)
            on s.PersonId equals ca.PersonId
            into j0
            from ca in j0.DefaultIfEmpty()

            orderby (s.ClassNumber == null ? 99999 : s.ClassNumber)

            select new StudentCarriedAbsencesDO
            {
                PersonId = s.PersonId,
                FirstTermExcusedCount = ca.FirstTermExcusedCount,
                FirstTermUnexcusedCount = ca.FirstTermUnexcusedCount,
                FirstTermLateCount = ca.FirstTermLateCount,
                SecondTermExcusedCount = ca.SecondTermExcusedCount,
                SecondTermUnexcusedCount = ca.SecondTermUnexcusedCount,
                SecondTermLateCount = ca.SecondTermLateCount,
            }
        ).ToArrayAsync(ct);
    }
}
