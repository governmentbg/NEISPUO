namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.StudentModels.PersonalDevelopmentSupport;
    using MON.Shared.Interfaces;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICommonPersonalDevelopmentSupportService
    {
        Task<CommonPersonalDevelopmentSupportViewModel> GetById(int id, CancellationToken cancellationToken);
        Task<IPagedList<CommonPersonalDevelopmentSupportViewModel>> List(StudentListInput input, CancellationToken cancellationToken);
        Task Create(CommonPersonalDevelopmentSupportModel model);
        Task Update(CommonPersonalDevelopmentSupportModel model);
        Task Delete(int id);
    }
}
