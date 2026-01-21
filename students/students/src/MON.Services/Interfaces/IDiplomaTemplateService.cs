namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDiplomaTemplateService
    {
        Task<IPagedList<DiplomaTemplateListModel>> List(PagedListInput input, CancellationToken cancellationToken);
        Task<BasicDocumentTemplateModel> GetById(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Използва се основно при създаване на шаблон. Товага знаем само за кой basicDocumentId е.
        /// Ще налее BasicDocumentPart-овете и техните BasicDocumentSubject-и.
        /// </summary>
        /// <param name="basicDocumentId"></param>
        /// <returns></returns>
        Task<BasicDocumentTemplateModel> GetForBasicDocument(int basicDocumentId, CancellationToken cancellationToken);
        Task<BasicDocumentTemplateModel> GetForDiploma(int diplomaId);
        Task Create(BasicDocumentTemplateModel model);
        Task Update(BasicDocumentTemplateModel model);
        Task Delete(int id, CancellationToken cancellationToken);
        Task<List<DiplomaTemplateDropdownViewModel>> GetDropdownOptions(string searchStr, int? basicDocumentId, CancellationToken cancellationToken);
    }
}
