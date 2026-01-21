namespace MON.Services.Interfaces
{
    using Microsoft.AspNetCore.Http;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAbsenceService
    {
        Task<ClassAbsenceModel> GetAbsencesForClassAsync(int classId, short schoolYear, short month);
        Task Create(StudentAbsenceInputModel model);
        Task Update(StudentAbsenceInputModel model);
        Task ImportAbsencesAsync(IFormFile file);
        Task<IPagedList<StudentAbsenceViewModel>> GetStudentAbsences(StudentListInput input);
        Task<AbsenceImportDetailsModel> GetImportDetails(int absenceImportId);
        Task ImportAbsencesFromSchoolBooksAsync(short schoolYear, short month);
        Task<List<StudentAbsenceHistoryOutputModel>> GetStudentAbsencesHistoryAsync(int absenceId);
        Task<AbsenceFileMonthAndYearModel> ReadAbsenceFile(IFormFile file);
        Task<IPagedList<InstitutionImportedAbsencesOutputModel>> GetInstitutionsWithImportedData(InstitutionsAbsencesListInput input);
        Task<IPagedList<InstitutionImportedAbsencesOutputModel>> GetInstitutionsWithoutImportedData(InstitutionsAbsencesListInput input);
        Task ExportAbsencesToFileAsync(short schoolYear, short month);
        Task<IPagedList<AbsenceImportFileModel>> ListImportedFiles(InstitutionListInput input);
        Task<IPagedList<AbsenceExportFileModel>> ListExportedFiles(PagedListInput input);
        Task DeleteAbsenceImport(int id);
        Task<List<AbsenceImportModel>> GetManualImportSampleData(short schoolYear, short month);
        Task ImportAbsencesFromManualEntryAsync(short schoolYear, short month);
        Task<string> ConstructAbsenceImportAsXml(AbsenceImportDetailsModel model);
        Task SetAbsenceImportSigningAtrributes(AbsenceImportSigningAtrributesSetModel model);
        Task<string> ConstructAbsenceExportAsXml(AbsenceExportFileModel model);
        Task SetAbsenceExportSigningAtrributes(AbsenceExportSigningAtrributesSetModel model);
        Task<int> CreateNoAbsencesImport(NoAbsencesImportModel model);
        Task<string> ConstructNoAbsencesImportAsXml(int? absenceImportId);
        Task CopyAspAsking(short schoolYear, short month);
    }
}
