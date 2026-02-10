namespace Helpdesk.API.Controllers
{
    using Helpdesk.Models;
    using Helpdesk.Models.Grid;
    using Helpdesk.Models.Issue;
    using Helpdesk.Models.Question;
    using Helpdesk.Models.Statistics;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class StatisticsController : BaseApiController
    {
        private readonly IStatisticsService _service;
        private readonly IAuthenticationService _authenticationService;

        public StatisticsController(IStatisticsService issueService, IAuthenticationService authenticationService)
        {
            _service = issueService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IPagedList<CategoryStatModel>> CategoryList([FromQuery] CategoryStatPageListInput input)
        {
            if (!await _authenticationService.DemandPermissionsForStatisticsAsync(StatisticsEnum.Category))
            {
                throw new UnauthorizedAccessException();
            }
            return await _service.GetCategoryListAsync(input);
        }
    }
}
