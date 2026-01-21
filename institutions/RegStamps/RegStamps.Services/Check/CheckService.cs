namespace RegStamps.Services.Check
{
    using Microsoft.EntityFrameworkCore;
    
    using Data.Entities;

    using Models.Check.Response;

    using static Models.Constants.RequestFileType;
    using static Models.Constants.RequestType;

    public class CheckService : ICheckService
    {
        private readonly DataStampsContext context;

        public CheckService(DataStampsContext context)
            => this.context = context;
        
        public async Task<IEnumerable<CheckMessageResponseModel>> CheckRequestDataAsync(int schoolId, int stampId, int requestId, int keeperId, int placeId)
        {
            ICollection<CheckMessageResponseModel> emptyFieldList = new List<CheckMessageResponseModel>();

            //tbKeeper Data
            var tbKeeperData = await this.context
                                        .TbKeepers
                                        .Where(x => x.KeeperId == keeperId)
                                        .Select(x => new 
                                        {
                                            IdNumber = x.IdNumber,
                                            Name1 = x.Name1,
                                            Name3 = x.Name3,
                                            OccupationId = x.OccupationId,
                                        })
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();

            if (tbKeeperData is null)
            {
                emptyFieldList.Add(this.PrepareResponse("Единен граждански номер"));
                emptyFieldList.Add(this.PrepareResponse("Име на пазител"));
                emptyFieldList.Add(this.PrepareResponse("Фамилия на пазител"));
                emptyFieldList.Add(this.PrepareResponse("Длъжност на пазителя"));
            } 
            else
            {
                if (string.IsNullOrWhiteSpace(tbKeeperData.IdNumber))
                {
                    emptyFieldList.Add(this.PrepareResponse("Единен граждански номер"));
                }

                if (string.IsNullOrWhiteSpace(tbKeeperData.Name1))
                {
                    emptyFieldList.Add(this.PrepareResponse("Име на пазител"));
                }

                if (string.IsNullOrWhiteSpace(tbKeeperData.Name3))
                {
                    emptyFieldList.Add(this.PrepareResponse("Фамилия на пазител"));
                }

                if (tbKeeperData.OccupationId == 0)
                {
                    emptyFieldList.Add(this.PrepareResponse("Длъжност на пазителя"));
                }
            }


            // tbKeepPlace Data
            string keepPlaceName = await this.context
                                            .TbKeepPlaces
                                            .Where(x => x.PlaceId == placeId)
                                            .Select(x => x.KeepPlaceName)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(keepPlaceName))
            {
                emptyFieldList.Add(this.PrepareResponse("Място на съхранение"));
            }

            //tbStamp Data
            var tbStampData = await this.context.TbStamps
                                        .Where(x => x.SchoolId == schoolId
                                                    && x.StampId == stampId)
                                        .Select(x => new 
                                        { 
                                            StampType = x.StampType,
                                            LetterDate = x.LetterDate,
                                            LetterNumber = x.LetterNumber,
                                            FirstUseDate = x.FirstUseDate,
                                            FirstUsePerson = x.FirstUsePerson,
                                            Image = x.Image
                                        })
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();

            if (tbStampData is null)
            {
                emptyFieldList.Add(this.PrepareResponse("Тип на печата"));
                emptyFieldList.Add(this.PrepareResponse("Дата на писмо за предаване"));
                emptyFieldList.Add(this.PrepareResponse("Номер на писмо за предаване"));
                emptyFieldList.Add(this.PrepareResponse("Дата на създаване"));
                emptyFieldList.Add(this.PrepareResponse("Лице на първоначално предаване"));
                emptyFieldList.Add(this.PrepareResponse("Образец на печата"));
            }

            if (tbStampData.StampType == 0)
            {
                emptyFieldList.Add(this.PrepareResponse("Тип на печата"));
            }

            if (tbStampData.StampType == 1)
            {
                if (!tbStampData.LetterDate.HasValue)
                {
                    emptyFieldList.Add(this.PrepareResponse("Дата на писмо за предаване"));
                }

                if (string.IsNullOrWhiteSpace(tbStampData.LetterNumber))
                {
                    emptyFieldList.Add(this.PrepareResponse("Номер на писмо за предаване"));
                }
            }

            if (!tbStampData.FirstUseDate.HasValue)
            {
                emptyFieldList.Add(this.PrepareResponse("Дата на създаване"));
            }

            if (string.IsNullOrWhiteSpace(tbStampData.FirstUsePerson))
            {
                emptyFieldList.Add(this.PrepareResponse("Лице на първоначално предаване"));
            }

            if (tbStampData.Image is null)
            {
                emptyFieldList.Add(this.PrepareResponse("Образец на печата"));
            }

            //tbРequest Data
            var refRequestStampData = await this.context
                                        .RefRequestStamps
                                        .Where(x => x.SchoolId == schoolId
                                                    && x.RequestId == requestId
                                                    && x.StampId == stampId)                
                                        .Select(x => new
                                        {
                                            OrderNumber = x.OrderNumber,
                                            OrderDate = x.OrderDate,
                                            StartDate = x.StartDate,
                                            EndDate = x.EndDate,
                                            RequestType = x.TbRequest.RequestType
                                        })
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync();

            if (refRequestStampData is null)
            {
                emptyFieldList.Add(this.PrepareResponse("Заповед номер"));
                emptyFieldList.Add(this.PrepareResponse("Заповед дата"));
                emptyFieldList.Add(this.PrepareResponse("Начална дата на връчване"));
                emptyFieldList.Add(this.PrepareResponse("Крайна дата на съхранение"));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(refRequestStampData.OrderNumber))
                {
                    emptyFieldList.Add(this.PrepareResponse("Заповед номер"));
                }

                if (!refRequestStampData.OrderDate.HasValue)
                {
                    emptyFieldList.Add(this.PrepareResponse("Заповед дата"));
                }

                if (!refRequestStampData.StartDate.HasValue)
                {
                    emptyFieldList.Add(this.PrepareResponse("Начална дата на връчване"));
                }

                if (refRequestStampData.RequestType == DestroyStampRequest || refRequestStampData.RequestType == LostStampRequest)
                {
                    if (!refRequestStampData.EndDate.HasValue)
                    {
                        emptyFieldList.Add(this.PrepareResponse("Крайна дата на съхранение"));
                    }
                }

                //tbRequestFile
                IEnumerable<int> tbRequestFileList = await this.context
                                            .TbRequestFiles
                                            .Where(x => x.SchoolId == schoolId
                                                    && x.RequestId == requestId)
                                            .Select(x => x.FileType)
                                            .ToListAsync();

                if (!tbRequestFileList.Any())
                {
                    emptyFieldList.Add(this.PrepareResponse("Други документи"));
                }

                if (refRequestStampData.RequestType == NewStampRequest)
                {
                    if (!tbRequestFileList.Any(x => x == OrderForStampUse))
                    {
                        emptyFieldList.Add(this.PrepareResponse("Други документи - Заповед за използване на печатите в училището"));
                    }

                    if (tbStampData.StampType == 1)
                    {
                        if (!tbRequestFileList.Any(x => new int[] { ExpeditionNote, AcceptanceProtocolFromRuo }.Contains(x)))
                        {
                            emptyFieldList.Add(this.PrepareResponse("Други документи - Експедиционна бележка от Монетен двор или Приемателно-предавателен протокол за получаване от РУО"));
                        }
                    }
                }

                if (refRequestStampData.RequestType == ChangeStampRequest)
                {
                    if (!tbRequestFileList.Any(x => x == OrderForStampChangeGuardian))
                    {
                        emptyFieldList.Add(this.PrepareResponse("Други документи - Заповед за промяна на пазител"));
                    }

                    if (!tbRequestFileList.Any(x => x == ReportForChanges))
                    {
                        emptyFieldList.Add(this.PrepareResponse("Други документи - Доклад за извършените промени"));
                    }

                    if (tbStampData.StampType == 1)
                    {
                        if (!tbRequestFileList.Any(x => new int[] { ExpeditionNote, AcceptanceProtocolFromRuo }.Contains(x)))
                        {
                            emptyFieldList.Add(this.PrepareResponse("Други документи - Експедиционна бележка от Монетен двор или Приемателно-предавателен протокол за получаване от РУО"));
                        }
                    }
                }

                if (refRequestStampData.RequestType == DestroyStampRequest)
                {
                    if (!tbRequestFileList.Any(x => x == ProtocolForStampDestroy))
                    {
                        emptyFieldList.Add(this.PrepareResponse("Други документи - Протокол за унищожаване на печата"));
                    }
                }

                if (refRequestStampData.RequestType == LostStampRequest)
                {
                    if (!tbRequestFileList.Any(x => new int[] { ProtocolForPoliceDepartment, ProtocolForFireDepartment }.Contains(x)))
                    {
                        emptyFieldList.Add(this.PrepareResponse("Други документи - Протокол от полицията при загубен печат/откраднат или Протокол от пожарната при пожар"));
                    }
                }
            }

            return emptyFieldList;
        }

        private CheckMessageResponseModel PrepareResponse(string fieldName)
            => new CheckMessageResponseModel
            {
                FieldName = fieldName
            };
    }
}
