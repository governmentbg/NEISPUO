namespace MON.Services.Interfaces
{
    using MON.Models;
    using MON.Models.Dropdown;
    using MON.Models.Institution.PrintTemplate;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPrintTemplateService
    {
        Task<List<PrintTemplateViewModel>> List(CancellationToken cancellationToken);
        Task<PrintTemplateViewModel> GetById(int id, CancellationToken cancellationToken);
        Task Update(PrintTemplateModel model);
        Task Create(PrintTemplateModel model);
        Task Delete(int id, CancellationToken cancellationToken);
        Task<List<PrintFormDropdownViewModel>> GetDropdownOptions(string searchStr, int? basicDocumentId, CancellationToken cancellationToken);
    }
}
