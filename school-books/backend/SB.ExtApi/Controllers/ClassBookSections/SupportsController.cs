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

public class SupportsController : ClassBookSectionController
{
    /// <summary>{{Supports.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Supports.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<SupportDO[]> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.SupportsGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Supports.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateSupportAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromBody] SupportDO support,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateSupportExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                TeacherIds = support.TeacherIds,
                IsForAllStudents = support.IsForAllStudents,
                StudentIds = support.IsForAllStudents
                    ? Array.Empty<int>()
                    #pragma warning disable CS0618 // member is obsolete
                    : support.StudentIds ?? support.Students?.Select(s => s.PersonId).ToArray(),
                    #pragma warning restore CS0618 // member is obsolete
                SupportDifficultyTypeIds = support.SupportDifficultyTypeIds,
                ExpectedResult = support.ExpectedResult,
                Description = support.Description,
                EndDate = support.EndDate,
                Activities = support.Activities
                    .Select(a =>
                        new CreateSupportExtApiCommandActivity
                        {
                            SupportActivityTypeId = a.SupportActivityTypeId,
                            Target = a.Target,
                            Result = a.Result,
                            Date = a.Date
                        })
                    .ToArray(),
            },
            ct);

    /// <summary>{{Supports.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="supportId">{{Common.SupportId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{supportId:int}")]
    public async Task UpdateSupportAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int supportId,
        [FromBody] SupportDO support,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateSupportExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SupportId = supportId,

                TeacherIds = support.TeacherIds,
                IsForAllStudents = support.IsForAllStudents,
                StudentIds = support.IsForAllStudents
                    ? Array.Empty<int>()
                    #pragma warning disable CS0618 // member is obsolete
                    : support.StudentIds ?? support.Students?.Select(s => s.PersonId).ToArray(),
                    #pragma warning restore CS0618 // member is obsolete
                SupportDifficultyTypeIds = support.SupportDifficultyTypeIds,
                ExpectedResult = support.ExpectedResult,
                Description = support.Description,
                EndDate = support.EndDate,
                Activities = support.Activities
                    .Select(a =>
                        new CreateSupportExtApiCommandActivity
                        {
                            SupportActivityTypeId = a.SupportActivityTypeId,
                            Target = a.Target,
                            Result = a.Result,
                            Date = a.Date
                        })
                    .ToArray(),
            },
            ct);

    /// <summary>{{Supports.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="supportId">{{Common.SupportId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{supportId:int}")]
    public async Task RemoveSupportAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromRoute] int supportId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveSupportCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                SupportId = supportId,
            },
            ct);
}
