namespace MON.Services.Interfaces
{
    using MON.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEqualizationService
    {
        Task Create(EqualizationModel model);

        Task<List<EqualizationGridViewModel>> GetListForPerson(int personId);

        Task Update(EqualizationModel model);

        Task<EqualizationModel> GetById(int id);

        Task Delete(int id);

        Task<List<DropdownViewModel>> GetEqualizationReasonTypeClasses();
    }
}
