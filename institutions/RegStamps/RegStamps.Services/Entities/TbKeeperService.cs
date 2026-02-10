namespace RegStamps.Services.Entities
{
    using Microsoft.EntityFrameworkCore;

    using Data.Entities;

    using Models.Shared.Database;
    using Models.Stamp.EditStamp.Database;
    using Models.Stamp.EditStamp.Response;
    using Models.Constants;

    public class TbKeeperService : ITbKeeperService
    {
        private readonly DataStampsContext context;

        public TbKeeperService(DataStampsContext context)
            => this.context = context;

        public async Task<KeeperDataDatabaseModel> GetKeeperDetailsAsync(int keeperId)
            => await this.context
                .TbKeepers
                .Where(x => x.KeeperId == keeperId)
                .Select(x => new KeeperDataDatabaseModel
                {
                    KeeperId = x.KeeperId,
                    IdNumber = x.IdNumber,
                    FirstName = x.Name1,
                    SecondName = x.Name2,
                    FamilyName = x.Name3,
                    OccupationId = x.OccupationId,
                    IdType = x.Idtype
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();


        public async Task<EditKeeperResponseModel> GetKeeperDataAsync(int keeperId)
            => (await this.context
                .TbKeepers
                .Where(x => x.KeeperId == keeperId)
                .Select(x => new EditKeeperDatabaseModel
                {
                    KeeperId = x.KeeperId,
                    IdNumber = x.IdNumber,
                    FirstName = x.Name1,
                    SecondName = x.Name2,
                    FamilyName = x.Name3,
                    OccupationId = x.OccupationId,
                    IdType = x.Idtype
                })
                .AsNoTracking()
                .ToListAsync())
                .Select(x => new EditKeeperResponseModel
                {
                    KeeperId = x.KeeperId,
                    IdNumber = x.IdNumber,
                    FirstName = x.FirstName,
                    SecondName = x.SecondName,
                    FamilyName = x.FamilyName,
                    OccupationId = x.OccupationId,
                    IdType = x.IdType,
                    IdTypeDropDown = new List<IdTypeDatabaseModel> {
                        new IdTypeDatabaseModel
                        {
                            IdType = 0,
                            IdTypeName = "ЕГН"
                        },
                        new IdTypeDatabaseModel
                        {
                            IdType = 1,
                            IdTypeName = "ЛНЧ"
                        },
                        new IdTypeDatabaseModel
                        {
                            IdType = 2,
                            IdTypeName = "ИДН"
                        }
                    }
                })
                .FirstOrDefault();

        public async Task<bool> IsExistAsync(int keeperId)
            => await this.context
                        .TbKeepers
                        .AsNoTracking()
                        .AnyAsync(x => x.KeeperId == keeperId);

        public async Task<int> UpdateKeeperIdNumberAsync(int keeperId, string idNumber, int idType)
        {
            TbKeeper tbKeeper = await this.GetTbKeeperAsync(keeperId);

            if (tbKeeper.IdNumber == idNumber)
            {
                return JsonStatus.NoChange;
            }

            tbKeeper.Idtype = idType;
            tbKeeper.IdNumber = idNumber;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateKeeperFirstNameAsync(int keeperId, string firstName)
        {
            TbKeeper tbKeeper = await this.GetTbKeeperAsync(keeperId);

            if (tbKeeper.Name1 == firstName)
            {
                return JsonStatus.NoChange;
            }

            tbKeeper.Name1 = firstName;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateKeeperSecondNameAsync(int keeperId, string secondName)
        {
            TbKeeper tbKeeper = await this.GetTbKeeperAsync(keeperId);

            if (tbKeeper.Name2 == secondName)
            {
                return JsonStatus.NoChange;
            }

            tbKeeper.Name2 = string.IsNullOrWhiteSpace(secondName) ? null : secondName;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateKeeperFamilyNameAsync(int keeperId, string familyName)
        {
            TbKeeper tbKeeper = await this.GetTbKeeperAsync(keeperId);

            if (tbKeeper.Name3 == familyName)
            {
                return JsonStatus.NoChange;
            }

            tbKeeper.Name3 = familyName;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateKeeperOccupationAsync(int keeperId, int occupationId)
        {
            TbKeeper tbKeeper = await this.GetTbKeeperAsync(keeperId);

            if (tbKeeper.OccupationId == occupationId)
            {
                return JsonStatus.NoChange;
            }

            tbKeeper.OccupationId = occupationId;

            return await this.context.SaveChangesAsync();
        }

        private async Task<TbKeeper> GetTbKeeperAsync(int keeperId)
             => await this.context
                        .TbKeepers
                        .Where(x => x.KeeperId == keeperId)
                        .FirstOrDefaultAsync();
    }
}
