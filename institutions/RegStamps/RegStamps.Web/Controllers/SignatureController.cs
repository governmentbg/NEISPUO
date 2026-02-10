namespace RegStamps.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Neispuo;

    public class SignatureController : BaseController
    {
        public SignatureController(INeispuoService neispuoService) : base(neispuoService)
        {
        }

        public ActionResult InvalidSignature()
        {
            return View();
        }

        public ActionResult NoCertificate()
        {
            return View();
        }

        public ActionResult NoNeispuo()
        {
            return View();
        }
    }
}
