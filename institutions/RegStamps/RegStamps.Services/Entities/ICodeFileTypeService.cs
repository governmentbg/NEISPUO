namespace RegStamps.Services.Entities
{
    using Models.Shared.Database;

    public interface ICodeFileTypeService
    {
        Task<IEnumerable<FileTypeDatabaseModel>> GetFileTypesAsync();
    }
}
