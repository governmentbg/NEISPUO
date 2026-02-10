namespace SB.ExtApi;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class OffDaysController : SchoolBookSectionController
{
    /// <summary>{{OffDays.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{OffDays.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<OffDayDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.OffDaysGetAllAsync(schoolYear, institutionId, ct);

    /// <summary>{{OffDays.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateOffDayAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromBody]OffDayDO offDay,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateOffDayCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                From = offDay.From,
                To = offDay.To,
                Description = offDay.Description,
                IsForAllClasses = offDay.IsForAllClasses,
                BasicClassIds = offDay.IsForAllClasses
                    ? Array.Empty<int>()
                    : offDay.BasicClassIds,
                ClassBookIds = offDay.IsForAllClasses
                    ? Array.Empty<int>()
                    : offDay.ClassBookIds,
                IsPgOffProgramDay = offDay.IsPgOffProgramDay ?? true,
            },
            ct);

    /// <summary>{{OffDays.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="offDayId">{{Common.OffDayId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{offDayId:int}")]
    public async Task UpdateOffDayAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int offDayId,
        [FromBody]OffDayDO offDay,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateOffDayCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                OffDayId = offDayId,

                From = offDay.From,
                To = offDay.To,
                Description = offDay.Description,
                IsForAllClasses = offDay.IsForAllClasses,
                BasicClassIds = offDay.IsForAllClasses
                    ? Array.Empty<int>()
                    : offDay.BasicClassIds,
                ClassBookIds = offDay.IsForAllClasses
                    ? Array.Empty<int>()
                    : offDay.ClassBookIds,
                IsPgOffProgramDay = offDay.IsPgOffProgramDay ?? true,
            },
            ct);

    /// <summary>{{OffDays.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="offDayId">{{Common.OffDayId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{offDayId:int}")]
    public async Task RemoveOffDayAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int offDayId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveOffDayCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                OffDayId = offDayId,
            },
            ct);
}
