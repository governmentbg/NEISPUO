namespace RegStamps.Web.Controllers
{
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Infrastructure.Extensions;

    using Services.Neispuo;

    using Models.Shared.Response;
    using Models.Constants;
    using Models.Stores;

    public class HomeController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public HomeController(
            SignInManager<ApplicationUser> signInManager,
            INeispuoService neispuoService) : base (neispuoService)
        {
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Main(string signature)
        {
            X509Certificate2 certificate = new X509Certificate2(signature.ToByteArray());

            if (certificate is not null)
            {
                IEnumerable<SchoolDataResponseModel> schools = await this.neispuoService.GetAllSchoolsAsync();

                bool found = schools.Where(x => !string.IsNullOrWhiteSpace(x.Bulstat)).Any(x => certificate.Subject.Contains(x.Bulstat));

                if (!found)
                {
                    return RedirectToAction("InvalidSignature", "Signature");
                }

                SchoolDataResponseModel school = schools.Where(x => !string.IsNullOrWhiteSpace(x.Bulstat) && certificate.Subject.Contains(x.Bulstat)).FirstOrDefault();

                if (school.BasicSchoolTypeID == 0)
                {
                    //ruo check emails
                    IEnumerable<string> ruoEmails = school.Email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    bool isEmailExist = ruoEmails.Any(email => certificate.Subject.Contains(email));

                    if (!isEmailExist)
                    {
                        return RedirectToAction("InvalidSignature", "Signature");
                    }
                }

                await this.signInManager.SignInAsync(new ApplicationUser
                {
                    SchoolId = school.SchoolId,
                    SchlMidName = school.SchlMidName
                }, false);

                return RedirectToAction(ActionNameType.ListStamp, ControllerNameType.Stamp);
            }
            else
            {
                return RedirectToAction("NoCertificate", "Signature");
                // return RedirectToAction(ActionNameType.ListStamp, ControllerNameType.Stamp);
            }
        }
    }
}
