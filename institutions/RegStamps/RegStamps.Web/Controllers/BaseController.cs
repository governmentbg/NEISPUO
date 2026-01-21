namespace RegStamps.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    using Services.Neispuo;

    using Models.Constants;
    using Models.Shared.Database;
    using Models.Shared.Response;

    public class BaseController : Controller
    {
        protected readonly INeispuoService neispuoService;

        protected BaseController(INeispuoService neispuoService)
        {
            this.neispuoService = neispuoService;
        }

        protected void AddErrorMessage(string message)
               => TempData[GlobalConstants.Error] = message;

        protected void AddSuccessMessage(string message)
               => TempData[GlobalConstants.Success] = message;

        protected void AddWarningMessage(string message)
               => TempData[GlobalConstants.Warning] = message;

        protected RedirectToActionResult RedirectToActionError()
               => RedirectToAction("Error", "Home");

        protected async Task<SchoolDataResponseModel> PrepareSchoolDataAsync(int schoolId)
        {
            SchoolDataResponseModel schoolData = null;

            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("schoolData")))
            {
                schoolData = await this.neispuoService.GetSchoolInfoAsync(schoolId);

                HttpContext.Session.SetString("schoolData", JsonConvert.SerializeObject(schoolData));
            }
            else
            {
                schoolData = JsonConvert.DeserializeObject<SchoolDataResponseModel>(HttpContext.Session.GetString("schoolData"));
            }

            return schoolData;
        }

        protected async Task<IEnumerable<OccupationDatabaseModel>> PrepareOccupationsAsync()
        {
            IEnumerable<OccupationDatabaseModel> occupations = new List<OccupationDatabaseModel>();

            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("occupations")))
            {
                occupations = await this.neispuoService.GetOccupationsAsync();

                HttpContext.Session.SetString("occupations", JsonConvert.SerializeObject(occupations));
            }
            else
            {
                occupations = JsonConvert.DeserializeObject<IEnumerable<OccupationDatabaseModel>>(HttpContext.Session.GetString("occupations"));

                if (!occupations.Any())
                {
                    HttpContext.Session.SetString("occupations", null);
                    return await this.PrepareOccupationsAsync();
                }
            }

            return occupations;
        }
    }
}
