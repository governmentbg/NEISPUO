namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IStudentMedicalNoticesQueryRepository;

[Authorize(Policy = Policies.StudentMedicalNoticesAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{personId:int}/[action]")]
public class StudentMedicalNoticeController
{
    public async Task<ActionResult<string>> GetStudentNameAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromRoute] int personId,
        [FromServices] IStudentMedicalNoticesQueryRepository studentMedicalNoticesQueryRepository,
        CancellationToken ct)
        => await studentMedicalNoticesQueryRepository.GetStudentNameAsync(personId, ct);

    public async Task<ActionResult<TableResultVO<GetStudentMedicalNoticesVO>>> GetStudentMedicalNoticesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int personId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IStudentMedicalNoticesQueryRepository studentMedicalNoticesQueryRepository,
        CancellationToken ct)
        => await studentMedicalNoticesQueryRepository.GetStudentMedicalNoticesAsync(
                schoolYear,
                personId,
                offset,
                limit,
                ct);
}
