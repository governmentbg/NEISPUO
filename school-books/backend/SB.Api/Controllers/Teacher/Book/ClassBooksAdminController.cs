namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IClassBooksAdminQueryRepository;

[Authorize(Policy = Policies.ClassBookAdminAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/{classBookId:int}/[action]")]
public class ClassBooksAdminController
{
    [HttpGet]
    public async Task<GetVO> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromServices]IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<GetInfoVO> GetInfoAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromServices] IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] ICommonCachedQueryStore commonCachedQueryStore,
        CancellationToken ct)
    {
        var result = await classBooksAdminQueryRepository.GetInfoAsync(schoolYear, classBookId, ct);

        result.HasCBExtProvider =
            await commonCachedQueryStore.GetInstHasCBExtProviderAsync(
                schoolYear,
                instId,
                ct);

        result.SchoolYearIsFinalized =
            await commonCachedQueryStore.GetSchoolYearIsFinalizedAsync(
                schoolYear,
                instId,
                ct);

        bool hasClassBookAdminWriteAccess =
            await httpContextAccessor.HasClassBookAdminWriteAccessAsync(schoolYear, instId, classBookId, ct);

        result.HasEditCurriculumAccess =
            result.HasCreateScheduleAccess =
            result.HasEditScheduleAccess =
            result.HasEditSchoolYearProgramAccess =
            result.HasEditStudentAccess =
            result.HasRemoveAccess =
            result.HasFinalizeAccess =
            result.HasUnfinalizeAccess =
            hasClassBookAdminWriteAccess;

        // everyone viewing the class book admin page
        // should be able to print
        result.HasCreatePrintAccess = true;

        return result;
    }

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]UpdateClassBookMainDataCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ClassBookId = classBookId,
            },
            ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpDelete]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    => await mediator.Send(
        new RemoveClassBookCommand
        {
            SchoolYear = schoolYear,
            InstId = instId,
            SysUserId = httpContextAccessor.GetSysUserId(),
            ClassBookId = classBookId,
        },
        ct);

    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetCurriculumVO>>> GetCurriculumAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetCurriculumAsync(schoolYear, classBookId, offset, limit, ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task UpdateCurriculumItemAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]UpdateClassBookCurriculumCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetStudentsVO>>> GetStudentsAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetStudentsAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpGet]
    public async Task<GetStudentVO> GetStudentAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int personId,
        [FromServices]IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetStudentAsync(schoolYear, classBookId, personId, ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task UpdateStudentAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]UpdateClassBookStudentCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpGet]
    public async Task<GetStudentNumbersVO[]> GetStudentNumbersAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetStudentNumbersAsync(schoolYear, classBookId, ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task UpdateStudentNumbersAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] UpdateClassBookStudentNumbersCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpGet]
    public async Task<GetSchoolYearProgramVO> GetSchoolYearProgramAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetSchoolYearProgramAsync(schoolYear, classBookId, ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task UpdateClassBookSchoolYearProgramAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] UpdateClassBookSchoolYearProgramCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ClassBookId = classBookId,
            },
            ct);

    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetPrintsVO>>> GetClassBookPrintsAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetClassBookPrintsAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetStudentPrintsVO>>> GetClassBookStudentPrintsAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IClassBooksAdminQueryRepository classBooksAdminQueryRepository,
        CancellationToken ct)
        => await classBooksAdminQueryRepository.GetClassBookStudentPrintsAsync(schoolYear, classBookId, offset, limit, ct);

    [OverrideAccessType(AccessType.Read)]
    [HttpPost]
    public async Task PrintClassBookAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new PrintClassBookCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [OverrideAccessType(AccessType.Read)]
    [HttpPost]
    public async Task PrintClassBookStudentAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int personId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new PrintClassBookStudentCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                PersonId = personId
            },
            ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task SortStudentClassNumbersAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new SortStudentClassNumbersCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ClassBookId = classBookId,
            },
            ct);

    [DisallowWhenInstHasCBExtProvider]
    [HttpPost]
    public async Task FinalizeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new FinalizeClassBookCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

    [HttpPost]
    public async Task UnfinalizeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UnfinalizeClassBookCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);
}
