namespace Helpdesk.API.Controllers
{
    using Helpdesk.Models;
    using Helpdesk.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [AllowAnonymous]
    public class LookupsController : BaseApiController
    {
        private readonly ILookupService _service;

        public LookupsController(ILookupService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<IEnumerable<DropdownViewModel>> GetStatuses()
        {
            return _service.GetStatuses();
        }

        [HttpGet]
        public Task<IEnumerable<DropdownViewModel>> GetPriorities()
        {
            return _service.GetPriorities();
        }

        [HttpGet]
        public Task<IEnumerable<DropdownViewModel>> GetCategories()
        {
            return _service.GetCategories();
        }

        [HttpGet]
        public Task<CategoryModel> GetCategory(int id)
        {
            return _service.GetCategory(id);
        }

        [HttpGet]
        public Task<IEnumerable<DropdownViewModel>> GetSubcategories(int? parentId)
        {
            return _service.GetSubcategories(parentId);
        }

        [HttpGet]
        public Task<IEnumerable<DropdownViewModel>> GetMONUsers()
        {
            return _service.GetMONUsers();
        }

        [HttpGet]
        public Task<IEnumerable<DropdownViewModel>> GetRUOUsers(int id)
        {
            return _service.GetRUOUsers(id);
        }

        [HttpGet]
        public Task<IEnumerable<DropdownViewModel>> GetUsers()
        {
            return _service.GetUsers();
        }
    }
}
