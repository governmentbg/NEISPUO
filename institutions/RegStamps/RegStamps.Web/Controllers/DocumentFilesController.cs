namespace RegStamps.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Identity;

    using Attributes;
    using Infrastructure.Extensions;
    using Settings;

    using Services.Neispuo;
    using Services.DocumentFiles;

    using Models.Stores;
    using Models.Constants;
    using Models.Shared.Database;

    [ValidateAuthenticationUser]
    public class DocumentFilesController : BaseController
    {
        private readonly IDocumentFileService documentFileService;

        private readonly UserManager<ApplicationUser> userManager;

        public DocumentFilesController(
            INeispuoService neispuoService,
            IDocumentFileService documentFileService,
            IOptions<AppSettings> options,
            UserManager<ApplicationUser> userManager) : base(neispuoService)
        {
            this.documentFileService = documentFileService;

            this.userManager = userManager;
        }

        public int SchoolId => Convert.ToInt32(this.userManager.GetUserId(this.User));

        [HttpGet]
        public async Task<ActionResult> ShowStampFile(string param)
        {
            if (param is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            DocumentDataDatabaseModel document = await this.documentFileService.GetStampFileAsync(this.SchoolId, stampId);

            if (document is null)
            {
                base.AddWarningMessage("Документa не е намерен. Нямате права за достъп.");
                return RedirectToAction(ActionNameType.StampDetails, ControllerNameType.Stamp, new { param = stampId.ToBase64Convert() });
            }

            byte[] doc = document.FileData.ToDecrypt().ToByteArray();

            if (doc is not null)
            {
                string contentTypeFile = document.FileName.ToContentType();

                return File(doc, contentTypeFile);
            }
            else
            {
                base.AddWarningMessage("Документa не може да бъде показан.");
                return RedirectToAction(ActionNameType.StampDetails, ControllerNameType.Stamp, new { param = stampId.ToBase64Convert() });
            }
        }

        [HttpGet]
        public async Task<ActionResult> ShowRequestFile(string paramFile, string paramStamp)
        {
            if (paramFile is null || paramStamp is null)
            {
                return base.RedirectToActionError();
            }

            int fileId = paramFile.FromBase64ConvertToInt();
            int stampId = paramStamp.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            DocumentDataDatabaseModel document = await this.documentFileService.GetRequestFileAsync(this.SchoolId, fileId);

            if (document is null)
            {
                base.AddWarningMessage("Документa не е намерен. Нямате права за достъп.");
                return RedirectToAction(ActionNameType.StampDetails, ControllerNameType.Stamp, new { param = stampId.ToBase64Convert() });
            }

            byte[] doc = document.FileData.ToDecrypt().ToByteArray();

            if (doc is not null)
            {
                return File(doc, document.FileName.ToContentType());
            }
            else
            {
                base.AddWarningMessage("Документa не може да бъде показан.");
                return RedirectToAction(ActionNameType.StampDetails, ControllerNameType.Stamp, new { param = stampId.ToBase64Convert() });
            }
        }
    }
}
