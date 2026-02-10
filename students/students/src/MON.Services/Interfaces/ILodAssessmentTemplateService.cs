namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILodAssessmentTemplateService
    {
        Task<IPagedList<LodAssessmentTemplateViewModel>> List(PagedListInput input, CancellationToken cancellationToken);
        Task<LodAssessmentTemplateViewModel> GetById(int id);
        Task Create(LodAssessmentTemplateModel model);
        Task Delete(int id);
        Task Update(LodAssessmentTemplateModel model);
        Task<IAsyncEnumerable<DropdownViewModel>> GetDropdownOptions(int? basicClassId);
    }
}
