using Microsoft.AspNetCore.Http;
using MON.DataAccess;
using MON.Models;
using MON.Models.Diploma;
using MON.Models.Grid;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface IDiplomaService
    {
        Task<IEnumerable<DiplomaDocumentModel>> GetDiplomaDocumentsAsync(int diplomadId);
        Task<ResultModel<int>> UploadDiplomaDocumentAsync(byte[] contents, int diplomaId, string description, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task RemoveDiplomaDocumentAsync(int id);
        Task<IPagedList<DiplomaViewModel>> List(DiplomaListInput input);
        Task UpdateDiplomaFinalizationStepsAsync(DiplomaFinalizationUpdateModel model);
        Task<DiplomaFinalizationViewModel> GetDiplomaFinalizationDetailsByIdAsync(int diplomaId);
        Task<IPagedList<VStudentDiploma>> ListUnimported(DiplomaListInput input);
        Task ReorderDiplomaDocumentsAsync(DiplomaOrderDocuments model);
        Task AnnulDiploma(DiplomaAnullationModel model);
        Task<SignedDiploma> ConstructDiplomaByIdAsync(int id);
        Task<string> ConstructDiplomaByIdAsXmlAsync(int id);
        Task<DiplomaSigningData> GetDiplomaSigningDataAsync(int id);
        Task Import(IFormFile file, CancellationToken cancellationToken);
        Task SetAsEditable(DiplomaSetAsEditableModel model);

        Task<DiplomaSectionsSubjectsModel> GetDiplomaSubjectsById(int diplomaId);

        Task<DiplomaCreateModel> GetCreateModel(int? personId, int? templateId, int? basicDocument, int? basicClassId, CancellationToken cancellationToken);
        Task<DiplomaUpdateModel> GetUpdateModel(int diplomaId);
        Task<IEnumerable<DiplomaAdditionalDocumentViewModel>> GetOriginalDocuments(int? personId, string? personalId, int? personalIdType, int[] mainBasicDocuments, CancellationToken cancellationToken);
        Task<ApiValidationResult> Create(DiplomaCreateModel model);
        Task<ApiValidationResult> Update(DiplomaUpdateModel model);
        Task Delete(int id);
        Task<DiplomaBasicDetailsModel> GetBasicDetails(int diplomaId);
        Task<byte[]> GenerateApplicationFileAsync(int diplomaId);
        //Task<ApiValidationResult> Validate(int diplomaId);
        Task<DiplomaViewModel> GetAdditionalDocumentDetails(int personId, int basicDocumentId);
        Task<IList<DropdownViewModel>> GetRegBookBasicDocuments(RegBookTypeEnum regBookType);
        Task<IPagedList<RegBookDetailsModel>> GetRegBookList(RegBookListInput input);
        Task<IQueryable<RegBookDetailsModel>> GetRegBookListAll(RegBookListInput input);
    }
}
