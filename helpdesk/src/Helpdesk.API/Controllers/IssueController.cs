namespace Helpdesk.API.Controllers
{
    using Helpdesk.Models;
    using Helpdesk.Models.Grid;
    using Helpdesk.Models.Issue;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class IssueController : BaseApiController
    {
        private readonly IIssueService _service;
        private readonly IAuthenticationService _authenticationService;

        public IssueController(IIssueService issueService, IAuthenticationService authenticationService)
        {
            _service = issueService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public Task<IPagedList<IssueViewModel>> List([FromQuery] IssuePageListInput input)
        {
            return _service.GetListAsync(input);
        }

        [HttpGet]
        public async Task<IssueViewModel> GetById(int id)
        {
            if (!await _authenticationService.DemandPermissionsForIssueAsync(id, PermissionEnum.Read))
            {
                throw new UnauthorizedAccessException();
            }
            return await _service.GetById(id);
        }

        [HttpPost]
        public async Task Create(IssueModel model)
        {
            if (!await _authenticationService.DemandPermissionsForIssueAsync(null, PermissionEnum.Create))
            {
                throw new UnauthorizedAccessException();
            }

            await _service.Create(model);
        }

        [HttpPut]
        public async Task Reopen(IssueReopenModel model)
        {
            if (model?.IssueId == null || !await _authenticationService.DemandPermissionsForIssueAsync(model.IssueId, PermissionEnum.Reopen))
            {
                throw new UnauthorizedAccessException();
            }

            await _service.Reopen(model);
        }

        [HttpPut]
        public async Task Update(IssueModel model)
        {
            if (model.Id == null || !await _authenticationService.DemandPermissionsForIssueAsync(model.Id.Value, PermissionEnum.Update))
            {
                throw new UnauthorizedAccessException();
            }
            await _service.Update(model);
        }

        [HttpPut]
        public async Task Resolve(IssueCommentModel model)
        {
            if (!await _authenticationService.DemandPermissionsForIssueAsync(model.IssueId, PermissionEnum.Update))
            {
                throw new UnauthorizedAccessException();
            }
            await _service.Resolve(model);
        }

        [HttpPut]
        public Task AssignToMyself(IssueAssignmentModel model)
        {
            return _service.AssignToMyself(model);
        }

        [HttpPut]
        public Task AssignTo(IssueAssignmentModel model)
        {
            return _service.AssignTo(model);
        }

        [HttpPut]
        public async Task Comment(IssueCommentModel model)
        {
            if (!await _authenticationService.DemandPermissionsForIssueAsync(model.IssueId, PermissionEnum.Update))
            {
                throw new UnauthorizedAccessException();
            }
            await _service.Comment(model);
        }

        [HttpPost]
        public Task LogReadActivity([FromQuery]int? id)
        {
            return _service.LogReadActivity(id);
        }
    }
}
