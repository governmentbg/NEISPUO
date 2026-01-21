namespace MON.Services.Interfaces
{
    using MON.Models.StudentModels;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEnvironmentCharacteristicsService
    {
        Task UpdateEnvironmentCharacteristicsAsync(StudentEnvironmentCharacteristicsModel model);
        Task<StudentEnvironmentCharacteristicsModel> GetStudentEnvironmentCharacteristics(int personId);
        Task DeleteRelativeAsync(int relativeId, int personId);
        Task<IEnumerable<StudentRelativeModel>> GetRelativesAsync(int personId);
        Task AddRelativeAsync(StudentRelativeModel model);
        Task UpdateRelativeAsync(StudentRelativeModel model);
        Task<StudentRelativeModel> GetRelativeAsync(int relativeId, int personId);
        Task<StudentRelativeModel> GetRelatedRelativeByPinAsync(string pin, int personId);
    }
}
