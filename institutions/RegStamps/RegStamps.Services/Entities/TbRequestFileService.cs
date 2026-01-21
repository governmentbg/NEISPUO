namespace RegStamps.Services.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using Data.Entities;

    using Infrastructure.Extensions;

    using Models.Shared.Database;

    using Models.AutoSave.Database;
    using Models.AutoSave.Response;

    public class TbRequestFileService : ITbRequestFileService
    {
        private readonly DataStampsContext context;

        public TbRequestFileService(DataStampsContext context)
            => this.context = context;


        public async Task<IEnumerable<RequestFileDataDatabaseModel>> GetRequestFilesAsync(int schoolId, int requestId)
            => await this.context
                        .TbRequestFiles
                        .Where(x => x.SchoolId == schoolId
                                    && x.RequestId == requestId)
                        .Select(x => new RequestFileDataDatabaseModel
                        {
                            FileId = x.FileId,
                            RequestId = x.RequestId,
                            SchoolId = x.SchoolId,
                            FileName = x.FileName,
                            //FileData = x.FileData,
                            FileType = x.FileTypeNavigation.FileTypeName,
                            FileIdByte = x.FileId.ToBase64Convert()
                        })
                        .AsNoTracking()
                        .OrderByDescending(x => x.FileId)
                        .ToListAsync();

        public async Task<IEnumerable<RequestFileResponseModel>> GetRequestUploadFilesAsync(int schoolId, int requestId)
            => (await this.context
                        .TbRequestFiles
                        .Where(x => x.SchoolId == schoolId
                                    && x.RequestId == requestId)
                        .Select(x => new RequestFileDatabaseModel
                        {
                            FileId = x.FileId,
                            FileName = x.FileName,
                            FileTypeName = x.FileTypeNavigation.FileTypeName,
                        })
                        .OrderByDescending(x => x.FileId)
                        .AsNoTracking()
                        .ToListAsync())
                        .Select(x => new RequestFileResponseModel
                        {
                            FileId = x.FileId,
                            FileIdByte = x.FileId.ToBase64Convert(),
                            FileName = x.FileName,
                            FileTypeName = x.FileTypeName,
                        })
                        .ToList();

        public async Task<bool> IsExistAsync(int schoolId, int requestId, int fileId)
            => await this.context
                        .TbRequestFiles
                        .AsNoTracking()
                        .AnyAsync(x => x.SchoolId == schoolId
                                    && x.RequestId == requestId
                                    && x.FileId == fileId);

        public async Task<int> InsertRequestFileAsync(int schoolId, int requestId, int fileTypeId, IFormFile file)
        {
            this.context.TbRequestFiles.Add(new TbRequestFile
            {
                RequestId = requestId,
                SchoolId = schoolId,
                FileName = file.FileName,
                FileData = (await file.ToByteArray()).ToBase64String().ToEncrypt(),
                FileType = fileTypeId,
                TimeStamp = DateTime.Now
            });

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> DeleteRequestFileAsync(int schoolId, int requestId, int fileId)
        {
            TbRequestFile tbRequstFile = await this.GetTbRequestFileAsync(schoolId, requestId, fileId);

            this.context.TbRequestFiles.Remove(tbRequstFile);

            return await this.context.SaveChangesAsync();
        }

        private async Task<TbRequestFile> GetTbRequestFileAsync(int schoolId, int requestId, int fileId)
             => await this.context
                        .TbRequestFiles
                        .Where(x => x.SchoolId == schoolId
                                    && x.RequestId == requestId
                                    && x.FileId == fileId)
                        .FirstOrDefaultAsync();

    }
}
