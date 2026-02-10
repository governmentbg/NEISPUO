namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using System.Threading.Tasks;

    public interface IEarlyAssessmentService
    {
        Task<StudentEarlyAssessmentModel> GetByPerson(int personId);

        Task CreateOrUpdate(StudentEarlyAssessmentModel model);
    }
}
