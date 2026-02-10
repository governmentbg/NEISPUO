namespace MON.Services.Interfaces
{
    using MON.Models.Diploma.Import;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDiplomaImportValidationExclusionService
    {
        Task<List<DiplomaImportValidationExclusionModel>> List();
        Task AddOrUpdate(DiplomaImportValidationExclusionModel model);
        Task Delete(string id);
    }
}
