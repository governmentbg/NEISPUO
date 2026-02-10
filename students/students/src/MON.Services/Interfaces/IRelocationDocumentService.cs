using MON.Models;
using MON.Models.StudentModels.Lod;
using MON.Models.StudentModels.StoredProcedures;
using MON.Report.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IRelocationDocumentService
    {
        Task<RelocationDocumentModel> GetById(int id, CancellationToken cancellationToken = default);
        Task<List<RelocationDocumentViewModel>> GetByPersonId(int personId, CancellationToken cancellationToken = default);
        Task<IEnumerable<RelocationDocumentOptionsModel>> GetRelocationDocumentOptionsByPerson(int personId, CancellationToken cancellationToken = default);
        Task Create(RelocationDocumentModel model);
        Task Update(RelocationDocumentModel model);
        Task Delete(int documentId);
        Task Confirm(int id);
        Task<StudentTermGradeViewModel> GetStudentCurrentTermGradesAsync(int relocationDocumentId, bool? filterForCurrentInstitution, bool? filterForCurrentSchoolBook, CancellationToken cancellationToken = default);
        Task<RelocationDocumentAbsencePrintModel> GetAbsences(int relocationDocumentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<StudentLodAssessmentListModel>> GetLodAssessmentsList(int relocationDocumentId, CancellationToken cancellationToken = default);
    }
}
