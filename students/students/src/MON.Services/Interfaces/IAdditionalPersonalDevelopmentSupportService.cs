namespace MON.Services.Interfaces
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Grid;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.PersonalDevelopmentSupport;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAdditionalPersonalDevelopmentSupportService
    {
        Task<AdditionalPersonalDevelopmentSupportViewModel> GetById(int id, CancellationToken cancellationToken);
        Task<IPagedList<AdditionalPersonalDevelopmentSupportViewModel>> List(StudentListInput input, CancellationToken cancellationToken);
        Task<IPagedList<VStudentEplrHoursTaken>> ListSchoolBookData(OresListInput input, CancellationToken cancellationToken);
        Task Create(AdditionalPersonalDevelopmentSupportModel model);
        Task Update(AdditionalPersonalDevelopmentSupportModel model);
        Task Delete(int id);
        Task SuspendAdditionalPersonalDevelopmentSupport(AdditionalPersonalDevelopmentSupportISuspendtemModel model, CancellationToken cancellationToken);
        Task<IEnumerable<SopDetailsViewModel>> GetSopForPerson(int personId, int schoolYear, CancellationToken cancellationToken);

    }
}
