namespace SB.ExtApi;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

[AdditionalAccess(AuthorizationConstants.ScheduleProviderAdditionalAccess)]
public class SchedulesController : ClassBookSectionController
{
    /// <summary>{{Schedules.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Schedules.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<ScheduleDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.SchedulesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Schedules.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Schedules.Create.Returns}}</returns>
    [HttpPost]
    public async Task<ScheduleResultDO> CreateScheduleAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]ScheduleDO schedule,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IScopedAggregateRepository<Schedule> scheduleAggregateRepository,
        CancellationToken ct)
    {
        int scheduleId = await mediator.Send(
            new CreateScheduleCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                IsIndividualSchedule = schedule.IsIndividualCurriculum,
                PersonId = schedule.PersonId,
                Term = schedule.Term,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                IncludesWeekend = schedule.IncludesWeekend ?? false,
                HasAdhocShift = schedule.HasAdhocShift ?? false,
                AdhocShiftIsMultiday = (schedule.HasAdhocShift ?? false)
                    ? schedule.AdhocShiftIsMultiday
                    : null,
                AdhocShiftDays = (schedule.HasAdhocShift ?? false)
                    ? schedule.AdhocShiftHours
                        ?.GroupBy(sh => sh.Day)
                        ?.Select(g => new CreateScheduleCommandShiftDay()
                        {
                            Day = g.Key,
                            Hours = g
                                .Select(sh => new CreateScheduleCommandShiftHour
                                {
                                    HourNumber = sh.HourNumber,
                                    StartTime = sh.StartTime,
                                    EndTime = sh.EndTime,
                                })
                                .ToArray()
                        })
                        ?.ToArray()
                    : null,
                ShiftId = (schedule.HasAdhocShift ?? false)
                    ? null
                    : schedule.ShiftId,
                Weeks = schedule.Weeks
                    .Select(sw =>
                        new ScheduleCommandWeek
                        {
                            Year = sw.Year,
                            WeekNumber = sw.WeekNumber
                        })
                    .ToArray(),
                Days = schedule.Days
                    .Select(sd =>
                        new CreateScheduleCommandDay
                        {
                            Day = sd.Day,
                            Hours = sd.Hours
                                .Select(h =>
                                    new CreateScheduleCommandDayHour
                                    {
                                        HourNumber = h.HourNumber,
                                        Groups = this.CreateScheduleHourGroups(h)
                                    })
                                .ToArray()
                        })
                    .ToArray()
            },
            ct);

        var createdSchedule = await scheduleAggregateRepository.FindAsync(schoolYear, scheduleId, ct);
        return this.CreateScheduleResultDO(createdSchedule);
    }

    /// <summary>{{Schedules.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="scheduleId">{{Common.ScheduleId}}</param>
    /// <returns>{{Schedules.Update.Returns}}</returns>
    [HttpPut("{scheduleId:int}")]
    public async Task<ScheduleResultDO> UpdateScheduleAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int scheduleId,
        [FromBody]ScheduleDO schedule,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IScopedAggregateRepository<Schedule> scheduleAggregateRepository,
        CancellationToken ct)
    {
        _ = await mediator.Send(
            new UpdateScheduleCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleId = scheduleId,

                IsIndividualSchedule = schedule.IsIndividualCurriculum,
                PersonId = schedule.PersonId,
                Term = schedule.Term,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                IncludesWeekend = schedule.IncludesWeekend ?? false,
                HasAdhocShift = schedule.HasAdhocShift ?? false,
                AdhocShiftIsMultiday = (schedule.HasAdhocShift ?? false)
                    ? schedule.AdhocShiftIsMultiday
                    : null,
                AdhocShiftDays = (schedule.HasAdhocShift ?? false)
                    ? schedule.AdhocShiftHours
                        ?.GroupBy(sh => sh.Day)
                        ?.Select(g => new CreateScheduleCommandShiftDay()
                        {
                            Day = g.Key,
                            Hours = g
                                .Select(sh => new CreateScheduleCommandShiftHour
                                {
                                    HourNumber = sh.HourNumber,
                                    StartTime = sh.StartTime,
                                    EndTime = sh.EndTime,
                                })
                                .ToArray()
                        })
                        ?.ToArray()
                    : null,
                ShiftId = (schedule.HasAdhocShift ?? false)
                    ? null
                    : schedule.ShiftId,
                Weeks = schedule.Weeks
                    .Select(sw =>
                        new ScheduleCommandWeek
                        {
                            Year = sw.Year,
                            WeekNumber = sw.WeekNumber
                        })
                    .ToArray(),
                Days = schedule.Days
                    .Select(sd =>
                        new CreateScheduleCommandDay
                        {
                            Day = sd.Day,
                            Hours = sd.Hours
                                .Select(h =>
                                    new CreateScheduleCommandDayHour
                                    {
                                        HourNumber = h.HourNumber,
                                        Groups = this.CreateScheduleHourGroups(h)
                                    })
                                .ToArray()
                        })
                    .ToArray()
            },
            ct);

        var updatedSchedule = await scheduleAggregateRepository.FindAsync(schoolYear, scheduleId, ct);
        return this.CreateScheduleResultDO(updatedSchedule);
    }

    /// <summary>{{Schedules.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="scheduleId">{{Common.ScheduleId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{scheduleId:int}")]
    public async Task RemoveScheduleAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int scheduleId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveScheduleCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleId = scheduleId,
            },
            ct);

    /// <summary>{{Schedules.Split.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="scheduleId">{{Common.ScheduleId}}</param>
    /// <remarks>{{Schedules.Split.Remarks}}</remarks>
    /// <returns>{{Schedules.Split.Returns}}</returns>
    [HttpPost("{scheduleId:int}/splitSchedule")]
    public async Task<int> SplitScheduleAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int scheduleId,
        [FromBody]ScheduleWeekDO[] weeks,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new SplitScheduleCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleId = scheduleId,

                Weeks = weeks
                    .Select(sw =>
                        new ScheduleCommandWeek
                        {
                            Year = sw.Year,
                            WeekNumber = sw.WeekNumber
                        })
                    .ToArray()
            },
            ct);

    private ScheduleResultDO CreateScheduleResultDO(Schedule schedule)
        => new()
        {
            ScheduleId = schedule.ScheduleId,
            Lessons = schedule.Lessons
                .Select(l =>
                    new ScheduleLessonDO
                    {
                        ScheduleLessonId = l.ScheduleLessonId,
                        Date = l.Date,
                        Day = l.Day,
                        HourNumber = l.HourNumber,
                        CurriculumId = l.CurriculumId
                    })
                .ToArray()
        };

    private CreateScheduleCommandDayHourGroup[] CreateScheduleHourGroups(ScheduleHourDO hour)
    {
        var hourGroups = Array.Empty<CreateScheduleCommandDayHourGroup>();

        #pragma warning disable CS0618 // member is obsolete
        if (hour.GroupCurriculumIds != null && hour.GroupCurriculumIds.Any())
        {
            hourGroups = hour.GroupCurriculumIds
                .Select(g => new CreateScheduleCommandDayHourGroup
                {
                    CurriculumId = g
                })
                .ToArray();
        }
        #pragma warning restore CS0618 // member is obsolete
        else if (hour.Groups != null && hour.Groups.Any())
        {
            hourGroups = hour.Groups
                .Select(g => new CreateScheduleCommandDayHourGroup
                {
                    CurriculumId = g.CurriculumId,
                    Location = g.Location
                })
                .ToArray();
        }

        return hourGroups;
    }
}
