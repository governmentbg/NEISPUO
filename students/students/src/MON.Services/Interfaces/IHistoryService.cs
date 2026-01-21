using MON.DataAccess;
using MON.Models.StudentModels.History;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IHistoryService
    {
        Task<IEnumerable<StudentResourceSupportHistoryModel>> GetStudentResourceSupportHistoryRecords(int personId);

        Task<IEnumerable<StudentResourceSopDetails>> GetStudentResourceSopHistoryRecords(int personId);

        Task<IEnumerable<StudentPersonalDataHistoryModel>> GetStudentPersonalDataHistoryRecords(int personId);
    }
}
