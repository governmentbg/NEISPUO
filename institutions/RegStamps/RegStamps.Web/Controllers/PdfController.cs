namespace RegStamps.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;

    using Attributes;
    using Infrastructure.Extensions;

    using Services.Pdf;
    using Services.Neispuo;
    using Services.Entities;
    
    using Models.Stores;
    using Models.Pdf.Response;

    [ValidateAuthenticationUser]
    public class PdfController : BaseController
    {
        private readonly IPdfService pdfService;
        private readonly IRazorTemplateService razorTemplateService;
        private readonly ITbStampService tbStampService;

        private readonly UserManager<ApplicationUser> userManager;

        public PdfController(
            INeispuoService neispuoService,
            IPdfService pdfService,
            IRazorTemplateService razorTemplateService,
            ITbStampService tbStampService,
            UserManager<ApplicationUser> userManager) : base(neispuoService)
        {
            this.pdfService = pdfService;
            this.razorTemplateService = razorTemplateService;
            this.tbStampService = tbStampService;

            this.userManager = userManager;
        }

        public int SchoolId => Convert.ToInt32(this.userManager.GetUserId(this.User));

        [HttpGet]
        public async Task<ActionResult> CreatePdf(string param)
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

            StampFormPdfResponseModel model = new StampFormPdfResponseModel();
            model.SchoolData = await base.PrepareSchoolDataAsync(this.SchoolId);
            model.StampData = await this.tbStampService.GetStampDetailsAsync(this.SchoolId, stampId);

            string html = await this.razorTemplateService
                                    .GetTemplateHtmlAsStringAsync<StampFormPdfResponseModel>(
                                        $"Pdf/StampFormPdf",
                                        model);

            byte[] pdf = this.pdfService.CreatePdfFromHtml(html, false, null, $"© Документът е генериран от секция Печати в системата на предучилищното и училищното образование - {DateTime.Now.ToShortDateString()}");

            return File(pdf, "a.pdf".ToContentType(), $"StampForm-{stampId}-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.pdf");
        }

    }
}
