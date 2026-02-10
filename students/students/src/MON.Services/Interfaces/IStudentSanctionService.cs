namespace MON.Services.Interfaces
{
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Shared.Interfaces;
    using System.Threading.Tasks;

    public interface IStudentSanctionService
    {
        Task<IPagedList<StudentSanctionViewModel>> List(SanctionsListInput input);
        Task<StudentSanctionModel> GetById(int id);
        Task Create(StudentSanctionModel model);
        Task Update(StudentSanctionModel model);
        Task Delete(int id);
        Task Import(int personId);
    }
}
