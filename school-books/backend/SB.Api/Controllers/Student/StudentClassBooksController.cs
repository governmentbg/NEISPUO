namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IStudentClassBooksQueryRepository;

[Authorize(Policy = Policies.StudentAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/[action]")]
public class StudentClassBooksController
{
    [HttpGet]
    public async Task<GetAllClassBooksVO[]> GetAllClassBooksAsync(
        [FromRoute]int schoolYear,
        [FromServices]IStudentClassBooksQueryRepository studentClassBooksQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await studentClassBooksQueryRepository.GetAllClassBooksAsync(
                schoolYear,
                httpContextAccessor.GetStudentPersonIds(),
                ct);
}
