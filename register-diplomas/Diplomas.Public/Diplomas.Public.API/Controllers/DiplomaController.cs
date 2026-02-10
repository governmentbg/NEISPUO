
using Diplomas.Public.Models;
using Diplomas.Public.Models.Access;
using Diplomas.Public.Services;
using Diplomas.Public.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diplomas.Public.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class DiplomaController : BaseApiController
    {
        private readonly CaptchaVerificationService _captchaVerification;
        private readonly IAccessService _accessService;
        private readonly IDiplomaService _diplomaService;

        public DiplomaController(ILogger<DiplomaController> logger, CaptchaVerificationService captchaVerification, IDiplomaService diplomaService, IAccessService accessService)
            : base(logger)
        {
            _captchaVerification = captchaVerification;
            _diplomaService = diplomaService;
            _accessService = accessService;
        }

        [HttpPost]
        public async Task<ActionResult<List<DiplomaViewModel>>> Search(DiplomaSearchModel searchModel)
        {
            bool requestIsValid = await _captchaVerification.IsCaptchaValid(searchModel?.ReCaptchaToken, 0.5f, "diplomaSearch");

            if (requestIsValid)
            {
                var access = new ExtAccessModel()
                {
                    PersonalId = searchModel.IdNumber,
                    PersonalIdType = searchModel.IdType,
                    Params = JsonConvert.SerializeObject(searchModel),
                    IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                };
                var diplomas = await _diplomaService.SearchAsync(searchModel);
                if (diplomas != null && diplomas.Count >= 1)
                {
                    access.HasResult = true;
                    await _accessService.Add(access);
                    return diplomas;
                }
                else
                {
                    access.HasResult = false;
                    await _accessService.Add(access);
                    return StatusCode(204);
                }
            }
            else
            {
                return BadRequest("Captcha is not valid");
            }
        }
    }
}
