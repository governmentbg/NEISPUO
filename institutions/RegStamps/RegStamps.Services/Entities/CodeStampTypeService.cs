namespace RegStamps.Services.Entities
{
    using Microsoft.EntityFrameworkCore;

    using Data.Entities;

    using Models.Shared.Database;

    public class CodeStampTypeService : ICodeStampTypeService
    {
        private readonly DataStampsContext context;

        public CodeStampTypeService(DataStampsContext context)
            => this.context = context;

        public async Task<IEnumerable<StampTypeDatabaseModel>> GetStampTypesAsync()
            => await this.context
                        .CodeStampTypes
                        .Where(x => x.StampTypeId > 0)
                        .Select(x => new StampTypeDatabaseModel
                        { 
                            StampTypeId = x.StampTypeId,
                            StampTypeName = x.StampTypeName
                        })
                        .OrderBy(x => x.StampTypeId)
                        .AsNoTracking()
                        .ToListAsync(); 
            
    }
}
