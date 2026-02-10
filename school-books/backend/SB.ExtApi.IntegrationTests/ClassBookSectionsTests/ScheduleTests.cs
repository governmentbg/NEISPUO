namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SB.Common;

[Collection(nameof(ExtApiIntegrationTestsCollection))]
public class ScheduleTests : RestoreSnapshotFixture
{
    private readonly int schoolYear;
    private readonly int institutionId;
    private readonly int classIdWithNoClassBook;
    private readonly int classBookId;
    private readonly int shiftId;
    private readonly int curriculumIdUpdate;
    private readonly int[] classBookCurriculums;
    private readonly int classIdWithNoClassBookCurriculumIdUpdate;
    private readonly int[] classIdWithNoClassBookCurriculums;
    private readonly ExtApiWebApplicationFactory appFactory;

    public ScheduleTests(
        OrderedFixtures<
            Tuple<
                CreateSnapshotFixture,
                DataFixture_V_XII,
                DataFixture_V_XII_TA,
                DataFixture_PG,
                ExtApiWebApplicationFactory
            >> fixtures)
    {
        DataFixture_V_XII data = fixtures.Values.Item2;
        ExtApiWebApplicationFactory appFactory = fixtures.Values.Item5;

        this.appFactory = appFactory;
        this.schoolYear = data.SchoolYear;
        this.institutionId = data.InstitutionId;
        this.classIdWithNoClassBook = data.ClassIdWithNoClassBook;
        this.classBookId = data.ClassBookId;
        this.shiftId = data.ShiftId;
        this.curriculumIdUpdate = data.CurriculumIdUpdate;
        this.classBookCurriculums = data.ClassBookCurriculumIds;
        this.classIdWithNoClassBookCurriculumIdUpdate = data.ClassIdWithNoClassBookCurriculumIdUpdate;
        this.classIdWithNoClassBookCurriculums = data.ClassIdWithNoClassBookCurriculumIds;
    }

    private ScheduleDO CreateSchedule(
        SchoolTerm term,
        DateTime startDate,
        DateTime endDate,
        bool includesWeekend,
        bool hasAdhocShift,
        (int day, int hourNumber, int curriculumId)[] hours)
    {
        return new ScheduleDO
        {
            IsIndividualCurriculum = false,
            ClassId = null,
            PersonId = null,
            Term = term,
            StartDate = startDate,
            EndDate = endDate,
            IncludesWeekend = includesWeekend,
            HasAdhocShift = hasAdhocShift,
            ShiftId = hasAdhocShift ? null : this.shiftId,
            AdhocShiftHours = hasAdhocShift
                ? new ScheduleAdhocShiftHourDO[]
                {
                    new()
                    {
                        Day = 1,
                        HourNumber = 1,
                        StartTime = "07:30",
                        EndTime = "08:10"
                    },
                    new()
                    {
                        Day = 1,
                        HourNumber = 2,
                        StartTime = "08:20",
                        EndTime = "09:00"
                    },
                    new()
                    {
                        Day = 1,
                        HourNumber = 3,
                        StartTime = "09:20",
                        EndTime = "10:00"
                    },
                    new()
                    {
                        Day = 1,
                        HourNumber = 4,
                        StartTime = "10:10",
                        EndTime = "10:50"
                    },
                    new()
                    {
                        Day = 1,
                        HourNumber = 5,
                        StartTime = "11:00",
                        EndTime = "11:40"
                    },
                    new()
                    {
                        Day = 1,
                        HourNumber = 6,
                        StartTime = "11:50",
                        EndTime = "12:30"
                    },
                    new()
                    {
                        Day = 1,
                        HourNumber = 7,
                        StartTime = "12:35",
                        EndTime = "13:15"
                    },
                }
                : null,
            AdhocShiftIsMultiday = hasAdhocShift ? false : null,
            Weeks = DateExtensions.GetWeeksInRange(startDate, endDate).Select(w =>
                new ScheduleWeekDO
                {
                    Year = w.year,
                    WeekNumber = w.weekNumber
                })
                .ToArray(),
            Days = hours
                .GroupBy(h => h.day)
                .Select(d => new ScheduleDayDO
                {
                    Day = d.Key,
                    Hours = d
                        .GroupBy(h => h.hourNumber)
                        .Select(h => new ScheduleHourDO
                        {
                            HourNumber = h.Key,
                            GroupCurriculumIds = h.Select(h => h.curriculumId).ToArray()
                        })
                        .ToArray()
                })
                .ToArray(),
            Lessons = null
        };
    }

    [Fact]
    public async Task GetAll_returns_all_classbook_schedules_successfully()
    {
        _ = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, this.classBookId);
        // TODO assert something about the result
    }

    [Theory,
        InlineData(false, false),
        InlineData(false, true),
        InlineData(true, false),
        InlineData(true, true)
    ]
    public async Task Should_create_update_remove_classbook_schedule(bool includesWeekend, bool hasAdhocShift)
    {
        Random random = new Random();
        int classIdWithNoClassBookCurriculumsLength = this.classIdWithNoClassBookCurriculums.Length;

        var classBookId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPostAsync(this.schoolYear, this.institutionId, this.classIdWithNoClassBook);

        // Create

        var schedule = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesPostAsync(
            this.schoolYear,
            this.institutionId,
            classBookId,
            this.CreateSchedule(
                SchoolTerm.TermTwo,
                // we want two full weeks after the start of the term
                GetSecondMondayInMonth(2023, 2),
                GetSecondMondayInMonth(2023, 2).AddDays(13),
                includesWeekend,
                hasAdhocShift,
                new (int day, int hourNumber, int curriculumId)[]
                {
                    (1, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                }.Concat(
                    includesWeekend
                        ? new (int day, int hourNumber, int curriculumId)[]
                            {
                                (6, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (6, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (6, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (6, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (6, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (6, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (6, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (7, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (7, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (7, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (7, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (7, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (7, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                                (7, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            }
                        : Array.Empty<(int day, int hourNumber, int curriculumId)>()
                ).ToArray()
                ));

        int scheduleLessonCount = 2 * (includesWeekend ? 7 : 5)/* weeks */ * 7/* hours */;

        Assert.InRange(schedule.ScheduleId, 1, int.MaxValue);
        Assert.Equal(scheduleLessonCount, schedule.Lessons.Count);
        Assert.All(schedule.Lessons, l => Assert.InRange(l.ScheduleLessonId, 1, int.MaxValue));

        var scheduleCreated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, classBookId))
           .Where(e => e.ScheduleId == schedule.ScheduleId)
           .SingleOrDefault();

        Assert.NotNull(scheduleCreated);
        Assert.Equal(2, scheduleCreated.Weeks.Count);
        Assert.Equal(scheduleLessonCount, scheduleCreated.Lessons.Count);

        // Update

        var scheduleUpdateData = this.CreateSchedule(
            SchoolTerm.TermTwo,
            GetSecondMondayInMonth(2023, 3),
            GetSecondMondayInMonth(2023, 3).AddDays(6),
            includesWeekend,
            hasAdhocShift,
            new (int day, int hourNumber, int curriculumId)[]
            {
                (1, 1, this.classIdWithNoClassBookCurriculumIdUpdate),
                (1, 2, this.classIdWithNoClassBookCurriculumIdUpdate),
                (1, 3, this.classIdWithNoClassBookCurriculumIdUpdate),
                (1, 4, this.classIdWithNoClassBookCurriculumIdUpdate),
                (1, 5, this.classIdWithNoClassBookCurriculumIdUpdate),
                (1, 6, this.classIdWithNoClassBookCurriculumIdUpdate),
                (1, 7, this.classIdWithNoClassBookCurriculumIdUpdate),
                (2, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (2, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (2, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (2, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (2, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (2, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (2, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (3, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (3, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (3, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (3, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (3, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (3, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (3, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (4, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (4, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (4, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (4, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (4, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (4, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (4, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (5, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (5, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (5, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (5, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (5, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (5, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                (5, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
            }.Concat(
                includesWeekend
                    ? new (int day, int hourNumber, int curriculumId)[]
                        {
                            (6, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (6, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (6, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (6, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (6, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (6, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (6, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (7, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (7, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (7, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (7, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (7, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (7, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                            (7, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                        }
                    : Array.Empty<(int day, int hourNumber, int curriculumId)>()
            ).ToArray());

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesPutAsync(this.schoolYear, this.institutionId, classBookId, schedule.ScheduleId, scheduleUpdateData);

        var scheduleUpdated = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, classBookId))
           .Where(e => e.ScheduleId == schedule.ScheduleId)
           .SingleOrDefault();

        int updatedScheduleLessonCount = 1 * (includesWeekend ? 7 : 5)/* weeks */ * 7/* hours */;

        Assert.NotNull(scheduleUpdated);
        Assert.Equal(GetSecondMondayInMonth(2023, 3), scheduleUpdated.StartDate);
        Assert.Equal(GetSecondMondayInMonth(2023, 3).AddDays(6), scheduleUpdated.EndDate);
        Assert.Equal(1, scheduleUpdated.Weeks.Count);
        Assert.Equal(updatedScheduleLessonCount, scheduleUpdated.Lessons.Count);
        Assert.Equal(7, scheduleUpdated.Days
            .Where(sd => sd.Day == 1)
            .SelectMany(sd => sd.Hours)
            .Count());
        Assert.Equal(this.classIdWithNoClassBookCurriculumIdUpdate, scheduleUpdated.Days
            .Where(sd => sd.Day == 1)
            .SelectMany(sd => sd.Hours)
            .SelectMany(sh => sh.GroupCurriculumIds)
            .First());

        // Remove
        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesDeleteAsync(this.schoolYear, this.institutionId, classBookId, schedule.ScheduleId);

        var scheduleDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesGetAsync(this.schoolYear, this.institutionId, classBookId))
            .Where(a => a.ScheduleId == schedule.ScheduleId)
            .SingleOrDefault();

        Assert.Null(scheduleDeleted);

        await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksDeleteAsync(this.schoolYear, this.institutionId, classBookId);

        var classBookDeleted = (await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksGetAsync(this.schoolYear, this.institutionId))
            .Where(s => s.ClassBookId == classBookId)
            .SingleOrDefault();

        Assert.Null(classBookDeleted);
    }

    [Theory,
        InlineData(false, false),
        InlineData(false, true),
        InlineData(true, false),
        InlineData(true, true)
    ]
    public async Task Should_split_classbook_schedule(bool includesWeekend, bool hasAdhocShift)
    {
        Random random = new Random();
        int classIdWithNoClassBookCurriculumsLength = this.classIdWithNoClassBookCurriculums.Length;

        var classBookId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPostAsync(this.schoolYear, this.institutionId, this.classIdWithNoClassBook);

        var schedule = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesPostAsync(
            this.schoolYear,
            this.institutionId,
            classBookId,
            this.CreateSchedule(
                SchoolTerm.TermTwo,
                new DateTime(2023, 2, 4),
                new DateTime(2023, 2, 26),
                includesWeekend,
                hasAdhocShift,
                new (int day, int hourNumber, int curriculumId)[]
                {
                    (1, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (1, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (2, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (3, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (4, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 2, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 3, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 4, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 5, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 6, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                    (5, 7, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)]),
                }));

        Assert.InRange(schedule.ScheduleId, 1, int.MaxValue);

        var splitScheduleId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksSchedulesSplitScheduleAsync(
            this.schoolYear,
            this.institutionId,
            classBookId,
            schedule.ScheduleId,
            DateExtensions.GetWeeksInRange(
                    new DateTime(2023, 2, 4),
                    new DateTime(2023, 2, 26))
                .Take(2)
                .Select(w => new ScheduleWeekDO
                {
                    Year = w.year,
                    WeekNumber = w.weekNumber
                })
                .ToArray());

        Assert.InRange(splitScheduleId, 1, int.MaxValue);
        Assert.NotEqual(schedule.ScheduleId, splitScheduleId);
    }

    [Fact]
    public async Task Should_throw_exception_on_invalid_AdhocShiftDays_Hours()
    {
        Random random = new Random();
        int classIdWithNoClassBookCurriculumsLength = this.classIdWithNoClassBookCurriculums.Length;

        var classBookId = await this.appFactory.CreateExtApiClient(TestExtSystem.SchoolBooksProvider).ClassBooksPostAsync(this.schoolYear, this.institutionId, this.classIdWithNoClassBook);

        var invalidShiftHours = new ScheduleAdhocShiftHourDO[]
        {
            new()
            {
                Day = 1,
                HourNumber = 1,
                StartTime = "07:30",
                EndTime = "08:10"
            },
            new()
            {
                Day = 1,
                HourNumber = 2,
                StartTime = "",
                EndTime = ""
            }
        };

        var schedule = this.CreateSchedule(
            SchoolTerm.TermTwo,
            new DateTime(2023, 2, 4),
            new DateTime(2023, 2, 26),
            false,
            true,
            new (int day, int hourNumber, int curriculumId)[]
            {
                (1, 1, this.classIdWithNoClassBookCurriculums[random.Next(0, classIdWithNoClassBookCurriculumsLength)])
            });
        schedule.AdhocShiftHours = invalidShiftHours;

        await Assert.ThrowsAsync<ApiException<ValidationErrorResponse>>(async () => await this.appFactory
            .CreateExtApiClient(TestExtSystem.SchoolBooksProvider)
            .ClassBooksSchedulesPostAsync(
                this.schoolYear,
                this.institutionId,
                classBookId,
                schedule));
    }

    private static DateTime GetSecondMondayInMonth(int year, int month)
    {
        DateTime firstDayInMonth = new(year, month, 1);
        int isoDayOfWeek = firstDayInMonth.GetIsoDayOfWeek();

        return firstDayInMonth.AddDays(8 - isoDayOfWeek).AddDays(7);
    }
}
