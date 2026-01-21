namespace SB.ExtApi;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class TeacherAbsencesController : SchoolBookSectionController
{
    /// <summary>{{TeacherAbsences.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{TeacherAbsences.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<TeacherAbsenceDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.TeacherAbsencesGetAllAsync(schoolYear, institutionId, ct);

    /// <summary>{{TeacherAbsences.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateTeacherAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromBody]TeacherAbsenceDO teacherAbsence,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (teacherAbsence.Hours.Any(sh => sh.Type == TeacherAbsenceHourType.EmptyHour && sh.ReplTeacherPersonId != null))
        {
            throw new DomainValidationException("If hour is empty, ReplTeacherPersonId should be null");
        }

        if (teacherAbsence.Hours.Any(sh => sh.Type != TeacherAbsenceHourType.EmptyHour && sh.ReplTeacherPersonId == null))
        {
            throw new DomainValidationException("If hour is not empty, ReplTeacherPersonId should have value");
        }

        return await mediator.Send(
            new CreateTeacherAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TeacherAbsenceId = teacherAbsence.TeacherAbsenceId,
                TeacherPersonId = teacherAbsence.TeacherPersonId,

                StartDate = teacherAbsence.StartDate,
                EndDate = teacherAbsence.EndDate,
                Reason = teacherAbsence.Reason,
                Hours = teacherAbsence.Hours
                    .Select(sh =>
                        new CreateTeacherAbsenceCommandHour
                        {
                            ScheduleLessonId = sh.ScheduleLessonId,
                            ReplTeacherPersonId = sh.ReplTeacherPersonId,
                            ReplTeacherIsNonSpecialist = sh.Type == TeacherAbsenceHourType.EmptyHour ? null : sh.Type == TeacherAbsenceHourType.Specialist ? false : true,
                        })
                    .ToArray(),
            },
            ct);
    }

    /// <summary>{{TeacherAbsences.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="teacherAbsenceId">{{Common.TeacherAbsenceId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{teacherAbsenceId:int}")]
    public async Task UpdateTeacherAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int teacherAbsenceId,
        [FromBody]TeacherAbsenceDO teacherAbsence,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (teacherAbsence.Hours.Any(sh => sh.Type == TeacherAbsenceHourType.EmptyHour && sh.ReplTeacherPersonId != null))
        {
            throw new DomainValidationException("If hour is empty, ReplTeacherPersonId should be null");
        }

        if (teacherAbsence.Hours.Any(sh => sh.Type != TeacherAbsenceHourType.EmptyHour && sh.ReplTeacherPersonId == null))
        {
            throw new DomainValidationException("If hour is not empty, ReplTeacherPersonId should have value");
        }

        await mediator.Send(
            new UpdateTeacherAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TeacherAbsenceId = teacherAbsenceId,
                TeacherPersonId = teacherAbsence.TeacherPersonId,

                StartDate = teacherAbsence.StartDate,
                EndDate = teacherAbsence.EndDate,
                Reason = teacherAbsence.Reason,
                Hours = teacherAbsence.Hours
                    .Select(sh =>
                        new CreateTeacherAbsenceCommandHour
                        {
                            ScheduleLessonId = sh.ScheduleLessonId,
                            ReplTeacherPersonId = sh.ReplTeacherPersonId,
                            ReplTeacherIsNonSpecialist = sh.Type == TeacherAbsenceHourType.EmptyHour ? null : sh.Type == TeacherAbsenceHourType.Specialist ? false : true,
                        })
                    .ToArray(),
            },
            ct);
    }

    /// <summary>{{TeacherAbsences.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="teacherAbsenceId">{{Common.TeacherAbsenceId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{teacherAbsenceId:int}")]
    public async Task RemoveTeacherAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int teacherAbsenceId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveTeacherAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TeacherAbsenceId = teacherAbsenceId,
            },
            ct);
}
