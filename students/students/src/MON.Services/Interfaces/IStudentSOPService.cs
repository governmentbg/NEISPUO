namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Update;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStudentSOPService
    {
        Task<StudentSopUpdateModel> GetById(int id, CancellationToken cancellationToken);
        Task<List<SopViewModel>> GetListForPerson(int personId, CancellationToken cancellationToken);
        Task Create(StudentSopUpdateModel model);
        Task Update(StudentSopUpdateModel model);
        Task Delete(int id, CancellationToken cancellationToken);
    }
}
