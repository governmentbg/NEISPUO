namespace RegStamps.Services.Entities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models.Shared.Database;
    using RegStamps.Data.Entities;

    public class CodeFileTypeService : ICodeFileTypeService
    {
        private readonly DataStampsContext context;

        public CodeFileTypeService(DataStampsContext context)
            => this.context = context;

        public async Task<IEnumerable<FileTypeDatabaseModel>> GetFileTypesAsync()
            => await this.context
                        .CodeFileTypes
                        .Select(x => new FileTypeDatabaseModel
                        {
                            FileTypeId = x.FileTypeId,
                            FileTypeName = x.FileTypeName
                        })
                        .OrderBy(x => x.FileTypeId)
                        .AsNoTracking()
                        .ToListAsync();
    }
}
