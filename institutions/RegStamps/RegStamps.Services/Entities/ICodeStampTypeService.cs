namespace RegStamps.Services.Entities
{
    using Models.Shared.Database;

    public interface ICodeStampTypeService
    {
        Task<IEnumerable<StampTypeDatabaseModel>> GetStampTypesAsync();
    }
}
