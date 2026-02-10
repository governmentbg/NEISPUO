namespace RegStamps.Services.DocumentFiles
{
    using RegStamps.Models.Shared.Database;

    public interface IDocumentFileService
    {
        Task<DocumentDataDatabaseModel> GetStampFileAsync(int schoolId, int stampId);

        Task<DocumentDataDatabaseModel> GetRequestFileAsync(int schoolId, int fileId);
    }
}
