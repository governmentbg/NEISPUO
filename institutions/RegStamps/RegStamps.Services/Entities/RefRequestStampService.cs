namespace RegStamps.Services.Entities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Data.Entities;

    using Infrastructure.Extensions;

    using Models.Shared.Database;
    using Models.Stamp.ListStamp.Database;
    using Models.Stamp.RequestsForStamp.Database;
    using Models.Stamp.EditStamp.Database;
    using Models.Stamp.EditStamp.Response;
    using RegStamps.Models.Constants;

    public class RefRequestStampService : IRefRequestStampService
    {
        private readonly DataStampsContext context;

        public RefRequestStampService(DataStampsContext context)   
            => this.context = context;
        

        public async Task<IEnumerable<RequestBackDatabaseModel>> GetReturnBackRequestStamps(int schoolId)
            => await this.context
                        .RefRequestStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.TbRequest.RequestStatus == 2)
                        .Select(x => new RequestBackDatabaseModel
                        {
                            RequestId = x.RequestId,
                            RequestIdByte = x.RequestId.ToBase64Convert(),
                            StampId = x.StampId,
                            StampIdByte = x.StampId.ToBase64Convert(),
                            KeeperId = x.KeeperId,
                            PlaceId = x.KeepPlaceId,
                            RequestStatusId = x.TbRequest.RequestStatus,
                            RequestTypeId = x.TbRequest.RequestType,
                            MonNotes = x.MonNotes,
                            RequestBackDate = x.TimeStamp
                        })
                        .AsNoTracking()
                        .OrderBy(x => x.RequestId)
                        .ToListAsync();

        public async Task<IEnumerable<RequestStampDatabaseModel>> GetRequestsForStamp(int schoolId, int stampId)
            => await this.context
                        .RefRequestStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId)
                        .Select (x => new RequestStampDatabaseModel
                        {
                            RequestId = x.RequestId,
                            RequestIdByte = x.RequestId.ToBase64Convert(),
                            StampId = x.StampId,
                            StampIdByte= x.StampId.ToBase64Convert(),
                            SchoolId = x.SchoolId,
                            KeeperIdNumber = x.Keeper.IdNumber,
                            KeeperFullName = $"{x.Keeper.Name1} {x.Keeper.Name2} {x.Keeper.Name3}",
                            TimeStamp = x.TimeStamp,
                            RequestStatusId = x.TbRequest.RequestStatus,
                            RequestStatusName = x.TbRequest.RequestStatusNavigation.RequestStatusName,
                            RequestTypeId = x.TbRequest.RequestType,
                            RequestTypeName = x.TbRequest.RequestTypeNavigation.RequestTypeName,
                            StampStatusId = x.TbStamp.StampStatus,
                        })
                        .AsNoTracking()
                        .OrderByDescending(x => x.TimeStamp)
                        .ThenByDescending(x => x.RequestId)
                        .ToListAsync();

        public async Task<RequestStampDetailsDatabaseModel> GetRequestStampDetailsAsync(int schoolId, int stampId)
        {
            int count = (await this.context
                        .RefRequestStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId)
                        .AsNoTracking()
                        .ToListAsync())
                        .Count;

            if (count == 1)
            {
                return await this.context
                        .RefRequestStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId)
                        .Select(x => new RequestStampDetailsDatabaseModel 
                        { 
                            StampId = x.StampId,
                            RequestId = x.RequestId,
                            SchoolId = x.SchoolId,
                            KeeperId = x.KeeperId,
                            KeepPlaceId = x.KeepPlaceId,
                            RequestTypeId = x.TbRequest.RequestType,
                            RequestTypeName = x.TbRequest.RequestTypeNavigation.RequestTypeName,
                            RequestStatusId = x.TbRequest.RequestStatus,
                            KeepPlaceName = x.KeepPlace.KeepPlaceName,
                            OrderNumber = x.OrderNumber,
                            OrderDate = x.OrderDate,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
            }
            else 
            {
                return await this.context
                        .RefRequestStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId
                                    && x.IsActive)
                        .Select(x => new RequestStampDetailsDatabaseModel
                        {
                            StampId = x.StampId,
                            RequestId = x.RequestId,
                            SchoolId = x.SchoolId,
                            KeeperId = x.KeeperId,
                            KeepPlaceId = x.KeepPlaceId,
                            RequestTypeId = x.TbRequest.RequestType,
                            RequestTypeName = x.TbRequest.RequestTypeNavigation.RequestTypeName,
                            RequestStatusId = x.TbRequest.RequestStatus,
                            KeepPlaceName = x.KeepPlace.KeepPlaceName,
                            OrderNumber = x.OrderNumber,
                            OrderDate = x.OrderDate,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
            }
        }

        public async Task<RequestStampDetailsDatabaseModel> GetRequestDetailsAsync(int schoolId, int stampId, int requestId)
            => await this.context
                        .RefRequestStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId
                                    && x.RequestId == requestId)
                        .Select(x => new RequestStampDetailsDatabaseModel
                        {
                            StampId = x.StampId,
                            RequestId = x.RequestId,
                            SchoolId = x.SchoolId,
                            KeeperId = x.KeeperId,
                            KeepPlaceId = x.KeepPlaceId,
                            RequestTypeId = x.TbRequest.RequestType,
                            RequestTypeName = x.TbRequest.RequestTypeNavigation.RequestTypeName,
                            RequestStatusId = x.TbRequest.RequestStatus,
                            KeepPlaceName = x.KeepPlace.KeepPlaceName,
                            OrderNumber = x.OrderNumber,
                            OrderDate = x.OrderDate,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

        public async Task<EditRequestResponseModel> GetRequestDataAsync(int schoolId, int stampId, int requestId)
        {
            EditRequestResponseModel model 
                = (await this.context.RefRequestStamps
                            .Where(x => x.SchoolId == schoolId
                                        && x.StampId == stampId
                                        && x.RequestId == requestId
                                        && x.IsActive == false)
                            .Select(x => new EditRequestDatabaseModel
                            { 
                                StampId = x.StampId,
                                RequestId = x.RequestId,
                                SchoolId = x.SchoolId,
                                KeeperId = x.KeeperId,
                                KeepPlaceId = x.KeepPlaceId,
                                KeepPlaceName = x.KeepPlace.KeepPlaceName,
                                OrderDate = x.OrderDate,
                                OrderNumber = x.OrderNumber,
                                StartDate = x.StartDate,
                                EndDate = x.EndDate
                            })
                            .AsNoTracking()
                            .ToListAsync())
                            .Select(x => new EditRequestResponseModel 
                            {
                                StampId = x.StampId,
                                RequestId = x.RequestId,
                                RequestIdByte = x.RequestId.ToBase64Convert(),
                                SchoolId = x.SchoolId,
                                KeeperId = x.KeeperId,
                                KeepPlaceId = x.KeepPlaceId,
                                KeepPlaceName = x.KeepPlaceName,
                                OrderDate = x.OrderDate.HasValue ? x.OrderDate.Value.ToShortDateString() : null,
                                OrderNumber = x.OrderNumber,
                                StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToShortDateString() : null,
                                EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToShortDateString() : null
                            })
                            .FirstOrDefault();

            return model;

        }

        public async Task<bool> IsExistAsync(int schoolId, int requestId)
            => await this.context
                        .RefRequestStamps
                        .AsNoTracking()
                        .AnyAsync(x => x.SchoolId == schoolId
                                    && x.RequestId == requestId);

        public async Task<bool> IsExistAsync(int schoolId, int stampId, bool isActive)
            => await this.context
                        .RefRequestStamps
                        .AsNoTracking()
                        .AnyAsync(x => x.SchoolId == schoolId
                                    && x.StampId == stampId
                                    && x.IsActive == isActive);

        public async Task<bool> IsExistActiveRequestAsync(int schoolId, int stampId)
            => await this.context
                        .RefRequestStamps
                        .AsNoTracking()
                        .AnyAsync(x => x.SchoolId == schoolId
                                    && x.StampId == stampId
                                    && !x.IsActive
                                    && x.TbRequest.RequestType > 0
                                    && x.TbRequest.RequestStatus < 3);


        public async Task<int> UpdateOrderNumberAsync(int requestId, string orderNumber)
        {
            RefRequestStamp refRequestStamp = await this.GetRefRequestStampAsync(requestId);

            if (refRequestStamp.OrderNumber == orderNumber)
            {
                return JsonStatus.NoChange;
            }

            refRequestStamp.OrderNumber = orderNumber;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateOrderDateAsync(int requestId, DateTime orderDate)
        {
            RefRequestStamp refRequestStamp = await this.GetRefRequestStampAsync(requestId);

            if (refRequestStamp.OrderDate == orderDate)
            {
                return JsonStatus.NoChange;
            }

            refRequestStamp.OrderDate = orderDate;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateStartDateAsync(int requestId, DateTime startDate)
        {
            RefRequestStamp refRequestStamp = await this.GetRefRequestStampAsync(requestId);

            if (refRequestStamp.StartDate == startDate)
            {
                return JsonStatus.NoChange;
            }

            refRequestStamp.StartDate = startDate;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateEndDateAsync(int requestId, DateTime endDate)
        {
            RefRequestStamp refRequestStamp = await this.GetRefRequestStampAsync(requestId);

            if (refRequestStamp.EndDate == endDate)
            {
                return JsonStatus.NoChange;
            }

            refRequestStamp.EndDate = endDate;

            return await this.context.SaveChangesAsync();
        }

        private async Task<RefRequestStamp> GetRefRequestStampAsync(int requestId)
             => await this.context
                        .RefRequestStamps
                        .Where(x => x.RequestId == requestId)
                        .FirstOrDefaultAsync();

    }
}
