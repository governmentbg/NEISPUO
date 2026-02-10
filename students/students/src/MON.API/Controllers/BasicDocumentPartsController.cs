using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models.Diploma;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class BasicDocumentPartsController : BaseApiController
    {
        private readonly IBasicDocumentPartsService _service;

        public BasicDocumentPartsController(IBasicDocumentPartsService service,
            ILogger<BasicDocumentPartsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForBasicDocumentEdit)]

        public Task<DiplomaTypeSchemaModel> GetSchemaById([FromQuery] int id)
        {
            return _service.GetSchemaById(id);
        }
    }
}
