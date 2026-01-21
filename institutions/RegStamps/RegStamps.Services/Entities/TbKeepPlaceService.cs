namespace RegStamps.Services.Entities
{
    using Microsoft.EntityFrameworkCore;

    using Data.Entities;

    using Models.Constants;

    public class TbKeepPlaceService : ITbKeepPlaceService
    {
        private readonly DataStampsContext context;

        public TbKeepPlaceService(DataStampsContext context)
            => this.context = context;

        public async Task<bool> IsExistAsync(int keepPlaceId)
            => await this.context
                        .TbKeepPlaces
                        .AsNoTracking()
                        .AnyAsync(x => x.PlaceId == keepPlaceId);

        public async Task<int> UpdateKeepPlaceNameAsync(int keepPlaceId, string keepPlaceName)
        {
            TbKeepPlace tbKeepPlace = await this.GetTbKeepPlaceAsync(keepPlaceId);

            if (tbKeepPlace.KeepPlaceName == keepPlaceName)
            {
                return JsonStatus.NoChange;
            }

            tbKeepPlace.KeepPlaceName = keepPlaceName;

            return await this.context.SaveChangesAsync();
        }

        private async Task<TbKeepPlace> GetTbKeepPlaceAsync(int keepPlaceId)
             => await this.context
                        .TbKeepPlaces
                        .Where(x => x.PlaceId == keepPlaceId)
                        .FirstOrDefaultAsync();

    }
}
