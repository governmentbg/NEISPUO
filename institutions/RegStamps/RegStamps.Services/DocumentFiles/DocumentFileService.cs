namespace RegStamps.Services.DocumentFiles
{
    using Microsoft.EntityFrameworkCore;
    using Models.Shared.Database;
    using RegStamps.Data.Entities;

    public class DocumentFileService : IDocumentFileService
    {
        private readonly DataStampsContext context;

        public DocumentFileService(DataStampsContext context)
            => this.context = context;


        public async Task<DocumentDataDatabaseModel> GetStampFileAsync(int schoolId, int stampId)
            => await this.context
                        .TbStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId)
                        .Select(x => new DocumentDataDatabaseModel
                        {
                            FileData = x.Image,
                            FileName = x.ImageName
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

        public async Task<DocumentDataDatabaseModel> GetRequestFileAsync(int schoolId, int fileId)
            => await this.context
                        .TbRequestFiles
                        .Where(x => x.SchoolId == schoolId
                                    && x.FileId == fileId)
                        .Select(x => new DocumentDataDatabaseModel
                        {
                            FileData = x.FileData,
                            FileName = x.FileName
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
    }
}
