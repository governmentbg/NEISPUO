namespace SB.ExtApi;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class ClassBooksController : SchoolBookSectionController
{
    /// <summary>{{ClassBooks.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{ClassBooks.GetAll.Returns}}</returns>
    [AdditionalAccess(AuthorizationConstants.ScheduleProviderAdditionalAccess)]
    [HttpGet]
    public async Task<ClassBookDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.ClassBooksGetAllAsync(schoolYear, institutionId, ct);

    /// <summary>{{ClassBooks.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="classId">{{ClassBooks.Create.Param.ClassId}}</param>
    /// <returns>{{ClassBooks.Create.Returns}}</returns>
    [HttpPost]
    public async Task<int> CreateClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromBody] int classId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateClassBookExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ClassId = classId
            },
            ct);

    /// <summary>{{ClassBooks.CreateMultiple.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="classIds">{{ClassBooks.CreateMultiple.Param.ClassIds}}</param>
    /// <returns>{{ClassBooks.CreateMultiple.Returns}}</returns>
    [HttpPost("createClassBooks")]
    public async Task<int[]> CreateClassBooksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromBody] int[] classIds,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateClassBooksExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ClassIds = classIds
            },
            ct);

    /// <summary>{{ClassBooks.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="classBookId">{{Common.ClassBookId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [ServiceFilter(typeof(ClassBookBelongsToInstitutionFilter))]
    [HttpDelete("{classBookId:int}")]
    public async Task RemoveClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    => await mediator.Send(
        new RemoveClassBookCommand
        {
            SchoolYear = schoolYear,
            InstId = institutionId,
            SysUserId = httpContextAccessor.GetSysUserId(),
            ClassBookId = classBookId,
        },
        ct);

    public record FinalizeFormData(IFormFile? SignedClassBookPrintFile);

    /// <summary>{{ClassBooks.Finalize.Summary}}</summary>
    /// <remarks><see cref="FinalizeFormData.SignedClassBookPrintFile"/>{{ClassBooks.Finalize.Remarks}}</remarks>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="classBookId">{{Common.ClassBookId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [ServiceFilter(typeof(ClassBookBelongsToInstitutionFilter))]
    [HttpPost("{classBookId:int}/finalize")]
    public async Task FinalizeClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromForm] FinalizeFormData form,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        using Stream? signedClassBookPrintStream = form.SignedClassBookPrintFile?.OpenReadStream();

        _ = await mediator.Send(
            new FinalizeClassBookWithSignedPdfCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ClassBookId = classBookId,

                SignedClassBookPrintFile = signedClassBookPrintStream,
                SignedClassBookPrintFileName = form.SignedClassBookPrintFile?.FileName,
                ExtractClassBookIdFromMetadataOrFileName = false,
            },
            ct);
    }

    /// <summary>{{ClassBooks.Unfinalize.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <param name="classBookId">{{Common.ClassBookId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [ServiceFilter(typeof(ClassBookBelongsToInstitutionFilter))]
    [HttpPost("{classBookId:int}/unfinalize")]
    public async Task UnfinalizeClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromRoute] int classBookId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    => await mediator.Send(
        new UnfinalizeClassBookCommand
        {
            SchoolYear = schoolYear,
            InstId = institutionId,
            SysUserId = httpContextAccessor.GetSysUserId(),
            ClassBookId = classBookId,
        },
        ct);

    /// <summary>{{ClassBooks.Combine.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{ClassBooks.Combine.Returns}}</returns>
    [HttpPost("combineClassBooks")]
    public async Task<int> CombineClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromBody] ClassBookCombineDO classBookCombine,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CombineClassBooksExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ParentClassId = classBookCombine.ParentClassId,
                ChildClassIdForDataTransfer = classBookCombine.ChildClassIdForDataTransfer
            },
            ct);

    /// <summary>{{ClassBooks.Separate.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonSchoolBookSectionParams/*'/>
    /// <returns>{{ClassBooks.Separate.Returns}}</returns>
    [HttpPost("separateClassBooks")]
    public async Task<int[]> SeparateClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int institutionId,
        [FromBody] ClassBookSeparateDO classBookSeparate,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new SeparateClassBooksExtApiCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ParentClassId = classBookSeparate.ParentClassId,
                ChildClassBooks = classBookSeparate.ChildClassIds!.Select(ci =>
                    new SeparateClassBooksExtApiCommandClassBook
                    {
                        ClassId = ci
                    })
                .ToArray()
            },
            ct);
}
