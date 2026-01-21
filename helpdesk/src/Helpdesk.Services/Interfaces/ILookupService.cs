namespace Helpdesk.Services.Interfaces
{
    using Helpdesk.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILookupService
    {
        Task<IEnumerable<DropdownViewModel>> GetStatuses();
        Task<IEnumerable<DropdownViewModel>> GetPriorities();
        Task<IEnumerable<DropdownViewModel>> GetCategories();
        Task<CategoryModel> GetCategory(int id);
        Task<IEnumerable<DropdownViewModel>> GetSubcategories(int? parentId);
        Task<IEnumerable<DropdownViewModel>> GetRUOUsers(int regionId);
        Task<IEnumerable<DropdownViewModel>> GetMONUsers();
        Task<IEnumerable<DropdownViewModel>> GetUsers();
    }
}
