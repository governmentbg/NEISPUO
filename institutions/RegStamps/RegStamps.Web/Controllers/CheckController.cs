namespace RegStamps.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;

    using Web.Attributes;

    using Services.Neispuo;
    using Services.Check;

    using Models.Stores;
    using Models.Constants;
    using Models.Check.Request;
    using Models.Check.Response;

    [ValidateAuthenticationUser]
    public class CheckController : BaseController
    {
        private readonly ICheckService checkService;

        private readonly UserManager<ApplicationUser> userManager;

        public CheckController(
            ICheckService checkService,
            UserManager<ApplicationUser> userManager,
            INeispuoService neispuoService) : base (neispuoService)
        {
            this.checkService = checkService;

            this.userManager = userManager;
        }
        public int SchoolId => Convert.ToInt32(this.userManager.GetUserId(this.User));

        public async Task<JsonResult> CheckData(CheckDataRequestModel request)
        {
            IEnumerable<CheckMessageResponseModel> errors 
                = await this.checkService.CheckRequestDataAsync(this.SchoolId, request.StampId, request.RequestId, request.KeeperId, request.PlaceId);

            return Json(errors.Any() ? errors : JsonStatus.Success);
        }
    }
}
