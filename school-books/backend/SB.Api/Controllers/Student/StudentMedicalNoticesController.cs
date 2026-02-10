namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IStudentMedicalNoticesQueryRepository;

[Authorize(Policy = Policies.StudentAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/[action]")]
public class StudentMedicalNoticesController
{
    [HttpGet]
    public async Task<GetAllStudentsVO[]> GetAllStudentsAsync(
        [FromRoute][SuppressMessage("", "IDE0060")] int schoolYear,
        [FromServices] IStudentMedicalNoticesQueryRepository studentMedicalNoticesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await studentMedicalNoticesQueryRepository.GetAllStudentsAsync(
                httpContextAccessor.GetStudentPersonIds(),
                ct);
}
