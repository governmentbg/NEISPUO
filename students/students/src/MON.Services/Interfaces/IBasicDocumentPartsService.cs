namespace MON.Services.Interfaces
{
    using MON.Models.Diploma;
    using System.Threading.Tasks;

    public interface IBasicDocumentPartsService
    {
        Task<DiplomaTypeSchemaModel> GetSchemaById(int id);
    }
}
