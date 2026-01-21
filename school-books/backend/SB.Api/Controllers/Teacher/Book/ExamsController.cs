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
using static SB.Domain.IExamsQueryRepository;

public class ExamsController : BookController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IExamsQueryRepository examsQueryRepository,
        CancellationToken ct)
        => await examsQueryRepository.GetAllAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpGet("{examId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int examId,
        [FromServices] IExamsQueryRepository examsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();

        var exam = await examsQueryRepository.GetAsync(schoolYear, classBookId, examId, ct);
        exam.HasEditAccess = exam.HasRemoveAccess =
            await authService.HasCurriculumAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                exam.CurriculumId,
                ct);

        return exam;
    }

    [Authorize(Policy = Policies.CurriculumAccess)]
    [HttpPost("{curriculumId:int}")]
    public async Task<int> CreateExamAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int curriculumId,
        [FromBody]CreateExamCommand command,
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

                CurriculumId = curriculumId
            },
            ct);

    [Authorize(Policy = Policies.CurriculumAccess)]
    [HttpPost("{curriculumId:int}/{examId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int curriculumId,
        [FromRoute]int examId,
        [FromBody]UpdateExamCommand command,
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

                CurriculumId = curriculumId,
                ExamId = examId,
            },
            ct);

    [Authorize(Policy = Policies.CurriculumAccess)]
    [HttpDelete("{curriculumId:int}/{examId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int curriculumId,
        [FromRoute]int examId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveExamCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                IsExternal = false,
                CurriculumId = curriculumId,
                ExamId = examId,
            },
            ct);
}
