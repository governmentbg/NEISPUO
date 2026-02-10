namespace SB.ExtApi;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class SchoolYearDateInfosController : SchoolBookSectionController
{
    /// <summary>{{SchoolYearDateInfos.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{SchoolYearDateInfos.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<SchoolYearDateInfoDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.SchoolYearSettingsGetAllAsync(schoolYear, institutionId, ct);

    /// <summary>{{SchoolYearDateInfos.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateSchoolYearDateInfoAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromBody]SchoolYearDateInfoDO schoolYearDateInfo,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateSchoolYearSettingsCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                SchoolYearStartDate = schoolYearDateInfo.SchoolYearStartDate,
                FirstTermEndDate = schoolYearDateInfo.FirstTermEndDate,
                SecondTermStartDate = schoolYearDateInfo.SecondTermStartDate,
                SchoolYearEndDate = schoolYearDateInfo.SchoolYearEndDate,
                Description = schoolYearDateInfo.Description,
                HasFutureEntryLock = false,
                PastMonthLockDay = null,
                IsForAllClasses = schoolYearDateInfo.IsForAllClasses,
                BasicClassIds = schoolYearDateInfo.IsForAllClasses
                    ? Array.Empty<int>()
                    : schoolYearDateInfo.BasicClassIds,
                ClassBookIds = schoolYearDateInfo.IsForAllClasses
                    ? Array.Empty<int>()
                    : schoolYearDateInfo.ClassBookIds,
            },
            ct);

    /// <summary>{{SchoolYearDateInfos.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="schoolYearDateInfoId">{{Common.SchoolYearDateInfoId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{schoolYearDateInfoId:int}")]
    public async Task UpdateSchoolYearDateInfoAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int schoolYearDateInfoId,
        [FromBody]SchoolYearDateInfoDO schoolYearDateInfo,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateSchoolYearSettingsCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SchoolYearSettingsId = schoolYearDateInfoId,

                SchoolYearStartDate = schoolYearDateInfo.SchoolYearStartDate,
                FirstTermEndDate = schoolYearDateInfo.FirstTermEndDate,
                SecondTermStartDate = schoolYearDateInfo.SecondTermStartDate,
                SchoolYearEndDate = schoolYearDateInfo.SchoolYearEndDate,
                Description = schoolYearDateInfo.Description,
                HasFutureEntryLock = false,
                PastMonthLockDay = null,
                IsForAllClasses = schoolYearDateInfo.IsForAllClasses,
                BasicClassIds = schoolYearDateInfo.IsForAllClasses
                    ? Array.Empty<int>()
                    : schoolYearDateInfo.BasicClassIds,
                ClassBookIds = schoolYearDateInfo.IsForAllClasses
                    ? Array.Empty<int>()
                    : schoolYearDateInfo.ClassBookIds,
            },
            ct);

    /// <summary>{{SchoolYearDateInfos.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="schoolYearDateInfoId">{{Common.SchoolYearDateInfoId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{schoolYearDateInfoId:int}")]
    public async Task RemoveSchoolYearDateInfoAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int schoolYearDateInfoId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSchoolYearSettingsCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SchoolYearSettingsId = schoolYearDateInfoId,
            },
            ct);
}
