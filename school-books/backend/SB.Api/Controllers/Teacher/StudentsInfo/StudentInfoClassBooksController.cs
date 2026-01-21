namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IStudentInfoClassBooksQueryRepository;

[Authorize(Policy = Policies.StudentInfoClassBookAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/{classBookId:int}/{personId:int}/[action]")]
public class StudentInfoClassBooksController
{
    [HttpGet]
    public async Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute][SuppressMessage("", "IDE0060")] int classBookId,
        [FromRoute]int personId,
        [FromServices]IStudentInfoClassBooksQueryRepository studentInfoClassBooksQueryRepository,
        CancellationToken ct)
        => await studentInfoClassBooksQueryRepository.GetAllClassBooksAsync(
                schoolYear,
                instId,
                personId,
                ct);
}
