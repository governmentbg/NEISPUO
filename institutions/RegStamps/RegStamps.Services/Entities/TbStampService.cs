namespace RegStamps.Services.Entities
{
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using Data.Entities;

    using Infrastructure.Extensions;

    using Models.Stamp.ListStamp.Database;
    using Models.Stamp.EditStamp.Database;
    using Models.Stamp.EditStamp.Response;
    using Models.Shared.Database;
    using Models.Stamp.SendStampRequest.Request;
    using Models.Constants;
    using Services.Signature;

    public class TbStampService : ITbStampService
    {
        private readonly DataStampsContext context;
        private readonly ICertificateService certificateService;

        public TbStampService(
            DataStampsContext dataStampsContext,
            ICertificateService certificateService)
        {
            this.context = dataStampsContext;
            this.certificateService = certificateService;
        }

        public async Task<IEnumerable<StampDataDatabaseModel>> GetAllStampsAsync(int schoolId)
        {
            IEnumerable<StampDataDatabaseModel> stamps = await this.context
               .TbStamps
               .Where(x => x.SchoolId == schoolId)
                .Select(x => new StampDataDatabaseModel
                {
                    StampId = x.StampId,
                    SchoolId = x.SchoolId,
                    StampTypeId = x.StampType,
                    StampTypeName = x.StampTypeNavigation.StampTypeName,
                    StampStatusId = x.StampStatus,
                    StampStatusName = x.StampStatusNavigation.StampStatusName,
                    FirstUseDate = x.FirstUseDate,
                    CurrentRequestId = x.RefRequestStamps.Where(r => r.StampId == x.StampId && r.SchoolId == x.SchoolId).Select(x => x.RequestId).Max()
                })
               .OrderBy(x => x.StampStatusId)
               .AsNoTracking()
               .ToListAsync();

            foreach (var item in stamps)
            {
                if (item.CurrentRequestId == 0)
                {
                    continue;
                }

                var requestData = await this.context
                    .TbRequests
                    .Where(x => x.RequestId == item.CurrentRequestId)
                    .Select(x => new {
                        RequestStatus = x.RequestStatus,
                        RequestType = x.RequestType,
                    })
                    .FirstOrDefaultAsync();

                item.StampIdByte = item.StampId.ToBase64Convert();
                item.CurrentRequestIdByte = item.CurrentRequestId.ToBase64Convert();
                item.RequestStatusId = requestData.RequestStatus;
                item.RequestTypeId = requestData.RequestType;

                int dataActiveKeeperId = await this.context
                                                            .RefRequestStamps
                                                            .Where(x => x.StampId == item.StampId
                                                                    && x.SchoolId == item.SchoolId
                                                                    && x.IsActive == true)
                                                            .Select(x => x.KeeperId)
                                                            .FirstOrDefaultAsync();

                if (dataActiveKeeperId == 0)
                {
                    int dataCurrentKeeperId = await this.context
                        .RefRequestStamps
                        .Where(x => x.StampId == item.StampId
                                && x.SchoolId == item.SchoolId
                                && x.RequestId == item.CurrentRequestId)
                        .Select(x => x.KeeperId)
                        .FirstOrDefaultAsync();

                    item.KeeperFullName = await this.context
                            .TbKeepers
                            .Where(x => x.KeeperId == dataCurrentKeeperId)
                            .Select(x => $"{x.Name1} {x.Name2} {x.Name3}")
                            .FirstOrDefaultAsync();
                }
                else
                {
                    item.KeeperFullName = await this.context
                            .TbKeepers
                            .Where(x => x.KeeperId == dataActiveKeeperId)
                            .Select(x => $"{x.Name1} {x.Name2} {x.Name3}")
                            .FirstOrDefaultAsync();
                }
            }

            return stamps;
        }

        public async Task<StampDetailsDataDatabaseModel> GetStampDetailsAsync(int schoolId, int stampId)
            => await this.context
                        .TbStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId)
                        .Select(x => new StampDetailsDataDatabaseModel 
                        { 
                            StampId = x.StampId,
                            StampIdByte = x.StampId.ToBase64Convert(),
                            SchoolId = x.SchoolId,
                            StampTypeId = x.StampType,
                            StampTypeName = x.StampTypeNavigation.StampTypeName,
                            StampStatusId = x.StampStatus,
                            StampStatusName = x.StampStatusNavigation.StampStatusName,
                            FirstUseDate = x.FirstUseDate,
                            FirstUsePerson = x.FirstUsePerson,
                            LetterDate = x.LetterDate,
                            LetterNumber = x.LetterNumber,
                            Image = x.Image,
                            ImageName = x.ImageName
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

        public async Task<EditStampResponseModel> GetStampDataAsync(int schoolId, int stampId)
        {
            IEnumerable<EditStampDatabaseModel> data 
                = await this.context
                    .TbStamps
                    .Where(x => x.SchoolId == schoolId
                                && x.StampId == stampId)
                    .Select(x => new EditStampDatabaseModel
                    {
                        StampId = x.StampId,
                        SchoolId = x.SchoolId,
                        StampTypeId = x.StampType,
                        FirstUseDate = x.FirstUseDate,
                        FirstUsePerson = x.FirstUsePerson,
                        LetterDate = x.LetterDate,
                        LetterNumber = x.LetterNumber,
                        Image = x.Image
                    })
                    .AsNoTracking()
                    .ToListAsync();

            IEnumerable<StampTypeDatabaseModel> stampTypes 
                = await this.context
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

            return data
                    .Select(x => new EditStampResponseModel 
                    { 
                        StampId = x.StampId,
                        StampIdByte = x.StampId.ToBase64Convert(),
                        SchoolId = x.SchoolId,
                        StampTypeId = x.StampTypeId,
                        FirstUseDate = x.FirstUseDate.HasValue ? x.FirstUseDate.Value.ToShortDateString() : null,
                        FirstUsePerson = x.FirstUsePerson,
                        LetterDate = x.LetterDate.HasValue ? x.LetterDate.Value.ToShortDateString() : null,
                        LetterNumber = x.LetterNumber,
                        Image = x.Image,
                        StampTypeDropDown = stampTypes
                    })
                    .FirstOrDefault();
        }

        public async Task<bool> IsExistAsync(int stampId)
            => await this.context
                        .TbStamps
                        .AsNoTracking()
                        .AnyAsync(x => x.StampId == stampId);
        
        public async Task<bool> IsExistAsync(int schoolId, int stampId)
            => await this.context
                        .TbStamps
                        .AsNoTracking()
                        .AnyAsync(x => x.SchoolId == schoolId
                                    && x.StampId == stampId);

        public async Task<int> CreateStampIdAsync()
        {
            Random random = new Random();
            int stampId = 0;

            while (true)
            {
                stampId = random.Next(100000, 999999);
                bool isExist = await this.IsExistAsync(stampId);

                if (!isExist)
                {
                    break;
                }
            }

            return stampId;
        }

        public async Task<int> CreateRequestNewStampAsync(int schoolId, int stampId)
        {
            RefRequestStamp requestStamp = new RefRequestStamp
            {
                TimeStamp = DateTime.Now,
                TbStamp = new TbStamp
                {
                    SchoolId = schoolId,
                    StampId = stampId,
                    StampType = 0,
                    StampStatus = 0
                },
                TbRequest = new TbRequest
                {
                    SchoolId = schoolId,
                    RequestType = 0,
                    RequestStatus = 0,
                    RequestDate = DateTime.Now
                },
                Keeper = new TbKeeper
                {
                    Idtype = 0,
                    OccupationId = 0
                },
                KeepPlace = new TbKeepPlace()
            };

            this.context.RefRequestStamps.Add(requestStamp);

            await this.context.SaveChangesAsync();
            return requestStamp.RequestId;
        }

        public async Task<int> CreateRequestChangeStampKeeperAsync(int schoolId, int stampId)
        {
            RefRequestStamp requestStamp = new RefRequestStamp
            {
                StampId = stampId,
                TimeStamp = DateTime.Now,
                TbRequest = new TbRequest
                {
                    SchoolId = schoolId,
                    RequestType = 1,
                    RequestStatus = 0,
                    RequestDate = DateTime.Now
                },
                Keeper = new TbKeeper
                {
                    Idtype = 0,
                    OccupationId = 0
                },
                KeepPlace = new TbKeepPlace()
            };

            this.context.RefRequestStamps.Add(requestStamp);

            await this.context.SaveChangesAsync();
            return requestStamp.RequestId;
        }

        public async Task<int> CreateRequestDestroyStampAsync(int schoolId, int stampId)
        {
            var requestData = await this.context
                                .RefRequestStamps
                                .Where(x => x.SchoolId == schoolId
                                            && x.StampId == stampId
                                            && x.IsActive)
                                .Select(x => new 
                                { 
                                    KeeperId = x.KeeperId,
                                    PlaceId =  x.KeepPlaceId,
                                    OrderNumber = x.OrderNumber,
                                    OrderDate = x.OrderDate,
                                    StartDate = x.StartDate
                                })
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

            RefRequestStamp requestStamp = new RefRequestStamp
            {
                StampId = stampId,
                KeeperId = requestData.KeeperId,
                KeepPlaceId = requestData.PlaceId,
                OrderNumber = requestData.OrderNumber,
                OrderDate = requestData.OrderDate,
                StartDate = requestData.StartDate,
                TimeStamp = DateTime.Now,
                TbRequest = new TbRequest
                {
                    SchoolId = schoolId,
                    RequestType = 2,
                    RequestStatus = 0,
                    RequestDate = DateTime.Now
                }
            };

            this.context.RefRequestStamps.Add(requestStamp);

            await this.context.SaveChangesAsync();
            return requestStamp.RequestId;
        }

        public async Task<int> CreateRequestLostStampAsync(int schoolId, int stampId)
        {
            var requestData = await this.context
                    .RefRequestStamps
                    .Where(x => x.SchoolId == schoolId
                                && x.StampId == stampId
                                && x.IsActive)
                    .Select(x => new
                    {
                        KeeperId = x.KeeperId,
                        PlaceId = x.KeepPlaceId,
                        OrderNumber = x.OrderNumber,
                        OrderDate = x.OrderDate,
                        StartDate = x.StartDate
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            RefRequestStamp requestStamp = new RefRequestStamp
            {
                StampId = stampId,
                KeeperId = requestData.KeeperId,
                KeepPlaceId = requestData.PlaceId,
                OrderNumber = requestData.OrderNumber,
                OrderDate = requestData.OrderDate,
                StartDate = requestData.StartDate,
                TimeStamp = DateTime.Now,
                TbRequest = new TbRequest
                {
                    SchoolId = schoolId,
                    RequestType = 3,
                    RequestStatus = 0,
                    RequestDate = DateTime.Now
                }
            };

            this.context.RefRequestStamps.Add(requestStamp);

            await this.context.SaveChangesAsync();
            return requestStamp.RequestId;
        }

        public async Task<int> UpdateStampTypeAsync(int schoolId, int stampId, int stampTypeId)
        {
            TbStamp tbStamp = await this.GetTbStampAsync(schoolId, stampId);

            if (tbStamp.StampType == stampTypeId)
            {
                return JsonStatus.NoChange;
            }


            if (stampTypeId == 2)
            {
                if (tbStamp.LetterDate.HasValue)
                {
                    tbStamp.LetterDate = null;
                }

                if (!string.IsNullOrWhiteSpace(tbStamp.LetterNumber))
                {
                    tbStamp.LetterNumber = null;
                }
            }

            tbStamp.StampType = stampTypeId;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateFirstUseDateAsync(int schoolId, int stampId, DateTime firstUseDate)
        {
            TbStamp tbStamp = await this.GetTbStampAsync(schoolId, stampId);

            if (tbStamp.FirstUseDate == firstUseDate)
            {
                return JsonStatus.NoChange;
            }

            tbStamp.FirstUseDate = firstUseDate;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateLetterDateAsync(int schoolId, int stampId, DateTime letterDate)
        {
            TbStamp tbStamp = await this.GetTbStampAsync(schoolId, stampId);

            if (tbStamp.LetterDate == letterDate)
            {
                return JsonStatus.NoChange;
            }

            tbStamp.LetterDate = letterDate;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateFirstUsePersonAsync(int schoolId, int stampId, string firstUsePerson)
        {
            TbStamp tbStamp = await this.GetTbStampAsync(schoolId, stampId);

            if (tbStamp.FirstUsePerson == firstUsePerson)
            {
                return JsonStatus.NoChange;
            }

            tbStamp.FirstUsePerson = firstUsePerson;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateLetterNumberAsync(int schoolId, int stampId, string letterNumber)
        {
            TbStamp tbStamp = await this.GetTbStampAsync(schoolId, stampId);

            if (tbStamp.LetterNumber == letterNumber)
            {
                return JsonStatus.NoChange;
            }

            tbStamp.LetterNumber = letterNumber;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> UpdateStampFileAsync(int schoolId, int stampId, IFormFile file)
        {
            TbStamp tbStamp = await this.GetTbStampAsync(schoolId, stampId);

            tbStamp.ImageName = file.FileName;
            tbStamp.Image = (await file.ToByteArray()).ToBase64String().ToEncrypt();

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> DeleteStampFileAsync(int schoolId, int stampId)
        {
            TbStamp tbStamp = await this.GetTbStampAsync(schoolId, stampId);

            tbStamp.ImageName = null;
            tbStamp.Image = null;

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> DeleteStampAsync(int schoolId, int stampId)
        {
            TbStamp tbStampDel = await this.context
                                        .TbStamps
                                        .Where(x => x.SchoolId == schoolId
                                                    && x.StampId == stampId
                                                    && x.StampStatus < 2)
                                        .FirstOrDefaultAsync();

            if (tbStampDel is null)
            {
                throw new Exception($"Invalid stamp {stampId} to delete");
            }

            this.context.TbStamps.Remove(tbStampDel);

            ICollection<RefRequestStamp> refRequestStampListDel = await this.context
                                                                            .RefRequestStamps
                                                                            .Where(x => x.SchoolId == schoolId 
                                                                                        && x.StampId == stampId)
                                                                            .ToListAsync();

            if (refRequestStampListDel.Count != 1)
            {
                throw new Exception($"Invalid stamp {stampId} to delete");
            }

            RefRequestStamp refRequestStampDel = refRequestStampListDel.FirstOrDefault();

            this.context.RefRequestStamps.Remove(refRequestStampDel);

            TbKeeper tbKeeperDel = await context
                                        .TbKeepers
                                        .Where(x => x.KeeperId == refRequestStampDel.KeeperId)
                                        .FirstOrDefaultAsync();
            if (tbKeeperDel is null)
            {
                throw new Exception($"Invalid stamp {stampId} to delete keeper {refRequestStampDel.KeeperId}");
            }

            this.context.TbKeepers.Remove(tbKeeperDel);

            TbKeepPlace tbKeepPlaceDel = await context
                                                .TbKeepPlaces
                                                .Where(x => x.PlaceId == refRequestStampDel.KeepPlaceId)
                                                .FirstOrDefaultAsync();
            if (tbKeepPlaceDel is null)
            {
                throw new Exception($"Invalid stamp {stampId} to delete keep place {refRequestStampDel.KeepPlaceId}");
            }

            this.context.TbKeepPlaces.Remove(tbKeepPlaceDel);

            TbRequest tbRequestDel = await context
                                            .TbRequests
                                            .Where(x => x.SchoolId == schoolId
                                                        && x.RequestId == refRequestStampDel.RequestId)
                                            .FirstOrDefaultAsync();
            if (tbRequestDel is null)
            {
                throw new Exception($"Invalid stamp {stampId} to delete request {refRequestStampDel.RequestId}");
            }

            this.context.TbRequests.Remove(tbRequestDel);

            ICollection<TbRequestFile> tbRequestFileListDel = await context
                                                                    .TbRequestFiles
                                                                    .Where( x => x.SchoolId == schoolId
                                                                                && x.RequestId == refRequestStampDel.RequestId)
                                                                    .ToListAsync();

            if (tbRequestFileListDel.Count > 0)
            {
                this.context.TbRequestFiles.RemoveRange(tbRequestFileListDel);
            }

            return await this.context.SaveChangesAsync();
        }

        public async Task<int> DeleteStampRequestAsync(int schoolId, int stampId, int requestId)
        {

            TbStamp tbStampDel = await this.context
                                        .TbStamps
                                        .Where(x => x.SchoolId == schoolId
                                                    && x.StampId == stampId
                                                    && x.StampStatus == 2)
                                        .FirstOrDefaultAsync();

            if (tbStampDel is null)
            {
                throw new Exception($"Stamp {stampId} not found or is registered");
            }

            RefRequestStamp refRequestStampDel = await context
                                                        .RefRequestStamps
                                                        .Where(x => x.SchoolId == schoolId
                                                                    && x.StampId == stampId
                                                                    && x.RequestId == requestId)
                                                        .FirstOrDefaultAsync();

            if (refRequestStampDel is null)
            {
                throw new Exception($"Invalid request {requestId} to delete");
            }

            this.context.RefRequestStamps.RemoveRange(refRequestStampDel);


            TbRequest tbRequestDel = await context
                                            .TbRequests
                                            .Where(x => x.SchoolId == schoolId
                                                        && x.RequestId == refRequestStampDel.RequestId)
                                            .FirstOrDefaultAsync();
            if (tbRequestDel is null)
            {
                throw new Exception($"Invalid request {requestId} to delete");
            }

            this.context.TbRequests.Remove(tbRequestDel);

            if (tbRequestDel.RequestType == 1)
            {
                TbKeeper tbKeeperDel = await context
                                            .TbKeepers
                                            .Where(x => x.KeeperId == refRequestStampDel.KeeperId)
                                            .FirstOrDefaultAsync();
                if (tbKeeperDel is null)
                {
                    throw new Exception($"Invalid request {requestId} to delete keeper {refRequestStampDel.KeeperId}");
                }

                this.context.TbKeepers.Remove(tbKeeperDel);

                TbKeepPlace tbKeepPlaceDel = await context
                                                    .TbKeepPlaces
                                                    .Where(x => x.PlaceId == refRequestStampDel.KeepPlaceId)
                                                    .FirstOrDefaultAsync();
                if (tbKeepPlaceDel is null)
                {
                    throw new Exception($"Invalid request {requestId} to delete keep place {refRequestStampDel.KeepPlaceId}");
                }

                this.context.TbKeepPlaces.Remove(tbKeepPlaceDel);
            }

            ICollection<TbRequestFile> tbRequestFileListDel = await context
                                                                    .TbRequestFiles
                                                                    .Where(x => x.SchoolId == schoolId
                                                                                && x.RequestId == refRequestStampDel.RequestId)
                                                                    .ToListAsync();

            if (tbRequestFileListDel.Count > 0)
            {
                this.context.TbRequestFiles.RemoveRange(tbRequestFileListDel);
            }

            return await this.context.SaveChangesAsync();
        }

        public async Task<(int result, string message)> SendStampRequestAsync(int schoolId, int stampId, int requestId, string signData, X509Certificate2 certificate)
        {
            SignatureRequestModel signature = this.certificateService.PrepareSignatureData(certificate);

            TbStamp tbStampData = await this.GetTbStampAsync(schoolId, stampId);

            if (tbStampData is null)
            {
                throw new Exception($"Invalid stamp {stampId}");
            }

            int stampStatusId = tbStampData.StampStatus;

            TbRequest tbRequestData = await context
                                        .TbRequests
                                        .Where(x => x.SchoolId == schoolId
                                                    && x.RequestId == requestId)
                                        .FirstOrDefaultAsync();
            if (tbRequestData is null)
            {
                throw new Exception($"Invalid stamp {stampId}, request {requestId} not found");
            }

            int requestStatusId = tbRequestData.RequestStatus;
            int requestTypeId = tbRequestData.RequestType;

            RefRequestStamp refRequestStampData = await context
                                                    .RefRequestStamps
                                                    .Where(x => x.SchoolId == schoolId 
                                                                && x.StampId == stampId
                                                                && x.RequestId == requestId)
                                                    .FirstOrDefaultAsync();
            if (refRequestStampData is null)
            {
                throw new Exception($"Invalid stamp {stampId}, request {requestId}, refRequest not found");
            }

            tbRequestData.RequestStatus = 1;
            tbRequestData.RequestDate = DateTime.Now;
            tbRequestData.SignerName = signature.SignerName;
            tbRequestData.SignerEmail = signature.SignerEmail;
            tbRequestData.Organisation = signature.Organisation;
            tbRequestData.SignerBulstat = signature.SignerBulstat;
            tbRequestData.SignerIdent = signature.SignerIdent;
            tbRequestData.Signature = signData;
            tbRequestData.SignTimeStamp = DateTime.Now;

            refRequestStampData.TimeStamp = DateTime.Now;

            if (stampStatusId == 0 
                && requestTypeId == 0)
            {
                tbStampData.StampStatus = 1;

                await this.context.SaveChangesAsync();

                return (1 , $"Печат с УИН {stampId} е изпратен успешно за одобрение в МОН.");
            }
            else if (stampStatusId == 2)
            {
                if (requestTypeId == 1)
                {
                    await this.context.SaveChangesAsync();

                    return (1, $"Промяна на обстоятелство за печат с УИН {stampId} е изпратен успешно за одобрение в МОН.");
                }
                else if (requestTypeId == 2)
                {
                    await this.context.SaveChangesAsync();

                    return (1, $"Заявка за унищожаване на печат с УИН {stampId} е изпратен успешно за одобрение в МОН.");
                }
                else if (requestTypeId == 3)
                {
                    await this.context.SaveChangesAsync();

                    return (1, $"Заявка за обявяване за загубен печат с УИН {stampId} е изпратен успешно за одобрение в МОН.");
                }
                else
                {
                    throw new Exception($"Invalid requestType {requestTypeId}");
                }
            }
            else
            {
                throw new Exception($"Invalid stampStatus {stampStatusId}");
            }
        }

        private async Task<TbStamp> GetTbStampAsync(int schoolId, int stampId)
             => await this.context
                        .TbStamps
                        .Where(x => x.SchoolId == schoolId
                                    && x.StampId == stampId)
                        .FirstOrDefaultAsync();


    }
}
