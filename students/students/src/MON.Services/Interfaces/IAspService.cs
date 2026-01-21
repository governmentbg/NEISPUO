namespace MON.Services.Interfaces
{
    using Microsoft.AspNetCore.Http;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.ASP;
    using MON.Models.Grid;
    using MON.Services.Extensions;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAspService
    {
        Task<IPagedList<ASPMonthlyBenefitsImportFileModel>> GetImportedBenefitsFilesAsync(ASPBenefitsInput input);
        Task<Result> ImportBenefitsAsync(IFormFile file);
        Task LoadAspConfirmations(AspSessionInfoViewModel sessionInfoModel);
        Task<IPagedList<ASPMonthlyBenefitModel>> GetImportedBenefitsDetailsAsync(ASPBenefitsInput input);
        Task UpdateBenefitAsync(ASPMonthlyBenefitModel model);
        Task<IPagedList<ASPMonthlyBenefitReportDetailsDto>> GetInstitutionsStillReviewingBenefitsAsync(ASPBenefitsInput input);
        Task ExportApprovedMonthlyBenefits(int campaignId);
        Task ExportEnrolledStudentsAsync(EnrolledStudentsExportModel model);
        Task<bool> CheckForExistingEnrolledStudentsExportAsync(EnrolledStudentsExportModel model);
        Task<IEnumerable<ASPEnrolledStudentsExportFileModel>> GetEnrolledStudentsExportFilesAsync(short? schoolYear);
        Task ExportEnrolledStudentsCorrectionsAsync(EnrolledStudentsExportModel model);
        Task<string> ConstructAspBenefitsAsXml(AspBenefitsModel model);
        Task SetAspBenefitsSigningAtrributes(AspBenefitsSigningAtrributesSetModel model);
        Task RemoveAspBenefitsSigningAtrributes(AspBenefitsSigningAtrributesSetModel model);
        Task<ASPMonthlyBenefitViewModel> GetImportedBenefitsFileMetaDataAsync(int importedFileId);
        Task UpdateImportedBenefitsFileMetaDataAsync(ASPMonthlyBenefitsImportFileModel model);
        Task DeleteImporedFileRecordAsync(int importId);
        Task UploadSubmittedData(IFormFile file);
        Task<IPagedList<ASPEnrolledStudentSubmittedDataViewModel>> ListEnrolledStudentsSubmittedData(EnrolledStudentsSubmittedDataListInput input);
        Task<List<AbsenceCampaignViewModel>> GetActiveCampaigns();
        Task<List<KeyValuePair<string, int>>> GetCampaignStats(int id);
        Task<IPagedList<AspUnprocessedRequestViewModel>> UnprocessedAspConfirmsList(ApPUnprocessedRequestsInput input);
        Task<MonSessionInfoViewModel> GetMonSession(short schoolYear, short month, string infoType);
        Task<IPagedList<AspMonConfirmViewModel>> GetMonConfirmsForCampaign(int sessionNo, PagedListInput input);


    }
}
