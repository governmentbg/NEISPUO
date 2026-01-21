using MON.Models.StudentModels;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IStudentScholarshipService
    {
        Task<ScholarshipViewModel> GetById(int scholarshipId);
        Task<StudentScholarshipsViewModel> GetByPersonId(int personId, int? schoolYear);
        Task Create(StudentScholarshipModel model);
        Task Update(StudentScholarshipModel model);
        Task Delete(int id);
    }
}
