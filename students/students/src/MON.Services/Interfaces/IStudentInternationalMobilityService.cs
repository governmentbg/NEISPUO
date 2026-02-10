namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IStudentInternationalMobilityService
    {
        Task Delete(int internationalMobilityId);
        Task<List<InternationalMobilityViewModel>> GetByPersonId(int personId);
        Task Create(InternationalMobilityModel model);
        Task<InternationalMobilityModel> GetById(int internationalMobilityId);
        Task Update(InternationalMobilityModel model);
    }
}
