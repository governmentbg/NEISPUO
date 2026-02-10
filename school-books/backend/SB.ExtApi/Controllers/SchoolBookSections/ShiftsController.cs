namespace SB.ExtApi;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class ShiftsController : SchoolBookSectionController
{
    /// <summary>{{Shifts.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{Shifts.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<ShiftDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.ShiftsGetAllAsync(schoolYear, institutionId, ct);

    /// <summary>{{Shifts.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateShiftAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromBody]ShiftDO shift,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateShiftCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IsExternal = true,

                Name = shift.Name,
                IsMultiday = shift.IsMultiday ?? false,
                Days = shift.Hours
                    .GroupBy(sh => sh.Day ?? 1)
                    .Select(g => new CreateShiftCommandDay()
                    {
                        Day = g.Key,
                        Hours = g
                            .Select(sh => new CreateShiftCommandHour
                            {
                                HourNumber = sh.HourNumber,
                                StartTime = sh.StartTime,
                                EndTime = sh.EndTime,
                            })
                            .ToArray()
                    })
                    .ToArray()
            },
            ct);

    /// <summary>{{Shifts.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="shiftId">{{Common.ShiftId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{shiftId:int}")]
    public async Task UpdateShiftAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int shiftId,
        [FromBody]ShiftDO shift,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateShiftCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IsExternal = true,
                ShiftId = shiftId,

                Name = shift.Name,
                IsMultiday = shift.IsMultiday ?? false,
                Days = shift.Hours
                    .GroupBy(sh => sh.Day ?? 1)
                    .Select(g => new CreateShiftCommandDay()
                    {
                        Day = g.Key,
                        Hours = g
                            .Select(sh => new CreateShiftCommandHour
                            {
                                HourNumber = sh.HourNumber,
                                StartTime = sh.StartTime,
                                EndTime = sh.EndTime,
                            })
                            .ToArray()
                    })
                    .ToArray()
            },
            ct);

    /// <summary>{{Shifts.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="shiftId">{{Common.ShiftId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{shiftId:int}")]
    public async Task RemoveShiftAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int shiftId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveShiftCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ShiftId = shiftId,
            },
            ct);
}
