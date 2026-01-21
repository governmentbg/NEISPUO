using MON.Models;
using MON.Models.Dropdown;
using MON.Models.Grid;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{

    public interface IBasicDocumentService
    {
        Task<IPagedList<BasicDocumentModel>> List(DiplomaTypesListInput input);
        Task<BasicDocumentModel> GetByIdAsync(int id);
        Task<BasicDocumentTemplateUpdateModel> GetBasicDocumentTemplate(int? id);
        Task SaveBasicDocumentTemplate(BasicDocumentTemplateUpdateModel model);
        Task<string> GetSchema(int id);
        Task<string> GetSchemaByTemplateId(int templateId);
        Task IncludeInRegister(int id);
        Task ExcludeFromRegister(int id);
        Task<BasicDocumentSequenceViewModel> GetNextBasicDocumentSequence(int basicDocumentId, DateTime? registrationDate = null);
        Task<IPagedList<BasicDocumentSequenceViewModel>> GetBasicDocumentSequencesAsync(BasicDocumentSequenceListInput input, CancellationToken cancellationToke);
        Task DeleteBasicDocumentSequenceAsync(int id);
        Task<List<DropdownViewModel>> GetPrintFormDropdownOptions(string searchStr, int? basicDocumentId);
    }
}
