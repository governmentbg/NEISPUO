namespace NeispuoExtension.Api.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Services.Core.ItAsset;
    using Services.Core.Models.ItAsset.Request;
    using Services.Core.Models.ItAsset.Response;

    [Authorize]
    public class ITAssetController : BaseController
    {
        private readonly IItAssetService itAssetService;

        public ITAssetController(IItAssetService itAssetService)
        {
            this.itAssetService = itAssetService;
        }

        [HttpPost("Excel")]
        [ProducesResponseType(typeof(InsertItAssetFromExcelResponseModel), 200)]
        public async Task<IActionResult> InsertFromExcelAsync([FromForm] InsertItAssetFromExcelRequestModel request)
            => Ok(await this.itAssetService.InsertFromExcelAsync(request));
    }
}
