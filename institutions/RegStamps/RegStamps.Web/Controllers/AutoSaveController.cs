namespace RegStamps.Web.Controllers
{
    using System.Globalization;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    
    using Attributes;
    using Infrastructure.Extensions;
    
    using Services.Entities;
    using Services.Neispuo;
    using Services.DocumentFiles;

    using Models.Stores;
    using Models.Constants;
    using Models.Shared.Database;
    using Models.AutoSave.Response;
    using Models.AutoSave.Request;

    [ValidateAuthenticationUser]
    public class AutoSaveController : BaseController
    {
        private readonly ITbStampService tbStampService;
        private readonly ITbKeeperService tbKeeperService;
        private readonly ITbKeepPlaceService tbKeepPlaceService;
        private readonly ITbRequestFileService tbRequestFileService;
        private readonly IRefRequestStampService refRequestStampService;
        private readonly IDocumentFileService documentFileService;
        private readonly ITbErrorLogService tbErrorLogService;

        private readonly UserManager<ApplicationUser> userManager;

        public AutoSaveController(
            ITbStampService tbStampService,
            ITbKeeperService tbKeeperService,
            ITbKeepPlaceService tbKeepPlaceService,
            IDocumentFileService documentFileService,
            ITbRequestFileService tbRequestFileService,
            IRefRequestStampService refRequestStampService,
            ITbErrorLogService tbErrorLogService,
            UserManager<ApplicationUser> userManager,
            INeispuoService neispuoService) : base(neispuoService)
        {
            this.tbStampService = tbStampService;
            this.tbKeeperService = tbKeeperService;
            this.tbKeepPlaceService = tbKeepPlaceService;
            this.documentFileService = documentFileService;
            this.tbRequestFileService = tbRequestFileService;
            this.refRequestStampService = refRequestStampService;
            this.tbErrorLogService = tbErrorLogService;
            this.userManager = userManager;
        }

        public int SchoolId => Convert.ToInt32(this.userManager.GetUserId(this.User));

        //tbStamp

        [ValidateStampRange]
        [ValidateState(nameof(stampTypeVal))]
        public async Task<JsonResult> SaveStampType(int stampId, string stampTypeVal)
        {
            if (!await this.IsStampExist(stampId))
            {
                return Json(JsonStatus.NotFound);
            }

            int stampTypeId = Convert.ToInt32(stampTypeVal);

            int result = await this.tbStampService.UpdateStampTypeAsync(this.SchoolId, stampId, stampTypeId);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateStampRange]
        [ValidateState(nameof(firstUseDateVal))]
        public async Task<JsonResult> SaveFirstUseDate(int stampId, string firstUseDateVal)
        {
            if (!await this.IsStampExist(stampId))
            {
                return Json(JsonStatus.NotFound);
            }

            DateTime firstUseDate = DateTime.ParseExact(firstUseDateVal, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            int result = await this.tbStampService.UpdateFirstUseDateAsync(this.SchoolId, stampId, firstUseDate);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }


        [ValidateStampRange]
        [ValidateState(nameof(firstLetterDateVal))]
        public async Task<JsonResult> SaveFirstLetterDate(int stampId, string firstLetterDateVal)
        {
            if (!await this.IsStampExist(stampId))
            {
                return Json(JsonStatus.NotFound);
            }

            DateTime firstLetterDate = DateTime.ParseExact(firstLetterDateVal, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            int result = await this.tbStampService.UpdateLetterDateAsync(this.SchoolId, stampId, firstLetterDate);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateStampRange]
        [ValidateState(nameof(firstUsePersonVal))]
        public async Task<JsonResult> SaveFirstUsePerson(int stampId, string firstUsePersonVal)
        {
            if (!await this.IsStampExist(stampId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbStampService.UpdateFirstUsePersonAsync(this.SchoolId, stampId, firstUsePersonVal.ToSanitize());

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateStampRange]
        [ValidateState(nameof(firstLetterNumberVal))]
        public async Task<JsonResult> SaveFirstLetterNumber(int stampId, string firstLetterNumberVal)
        {
            if (!await this.IsStampExist(stampId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbStampService.UpdateLetterNumberAsync(this.SchoolId, stampId, firstLetterNumberVal);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [HttpPost]
        public async Task<JsonResult> SaveUploadStampFile(StampFileRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return Json(JsonStatus.Fail);
            }

            if (!await this.IsStampExist(request.StampId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbStampService.UpdateStampFileAsync(this.SchoolId, request.StampId, request.UploadData);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateStampRange]
        public async Task<JsonResult> ShowStampImageData(int stampId)
        {
            if (!await this.IsStampExist(stampId))
            {
                return Json(JsonStatus.NotFound);
            }

            DocumentDataDatabaseModel document = await this.documentFileService.GetStampFileAsync(this.SchoolId, stampId);

            if (document.FileData is null)
            {
                return Json(JsonStatus.NoFiles);
            }

            return Json(document.FileName);
        }

        [ValidateStampRange]
        public async Task<JsonResult> DeleteStampFile(int stampId)
        {
            if (!await this.IsStampExist(stampId))
            {
                return Json(JsonStatus.NotFound);
            }
            int result = await this.tbStampService.DeleteStampFileAsync(this.SchoolId, stampId);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }


        //tbKeeper
        [ValidateState(nameof(keeperIdNumberVal))]
        [ValidateState(nameof(keeperIdTypeVal))]
        public async Task<JsonResult> SaveKeeperIdNumber(int keeperId, string keeperIdNumberVal, string keeperIdTypeVal)
        {
            int idType = Convert.ToInt32(keeperIdTypeVal);
            bool isIdNumberValid = false;
                
            if (idType == 0)
            {
                //egn
                isIdNumberValid = keeperIdNumberVal.IsValidPersonalIdNumber();
            }
            else if (idType == 1)
            {
                //LNCH
                isIdNumberValid = keeperIdNumberVal.IsValidForeignIdNumber();
            }
            else
            {
                isIdNumberValid = true;
            }

            if (isIdNumberValid)
            {
                if (!await this.IsKeeperExist(keeperId))
                {
                    return Json(JsonStatus.NotFound);
                }

                int result = await this.tbKeeperService.UpdateKeeperIdNumberAsync(keeperId, keeperIdNumberVal, idType);

                if (result == JsonStatus.NoChange)
                {
                    return Json(JsonStatus.NoChange);
                }

                return Json(JsonStatus.Success);
            }
            else
            {
                return Json(JsonStatus.Fail);
            }
        }

        [ValidateState(nameof(keeperFirstNameVal))]
        public async Task<JsonResult> SaveKeeperFirstName(int keeperId, string keeperFirstNameVal)
        {
            if (!await this.IsKeeperExist(keeperId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbKeeperService.UpdateKeeperFirstNameAsync(keeperId, keeperFirstNameVal);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        public async Task<JsonResult> SaveKeeperSecondName(int keeperId, string keeperSecondNameVal)
        {
            if (!await this.IsKeeperExist(keeperId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbKeeperService.UpdateKeeperSecondNameAsync(keeperId, keeperSecondNameVal);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateState(nameof(keeperFamilyNameVal))]
        public async Task<JsonResult> SaveKeeperFamilyName(int keeperId, string keeperFamilyNameVal)
        {
            if (!await this.IsKeeperExist(keeperId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbKeeperService.UpdateKeeperFamilyNameAsync(keeperId, keeperFamilyNameVal);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateState(nameof(occupNameVal))]
        public async Task<JsonResult> SaveOccupName(int keeperId, string occupNameVal)
        {
            if (!await this.IsKeeperExist(keeperId))
            {
                return Json(JsonStatus.NotFound);
            }

            int occupationId = Convert.ToInt32(occupNameVal);

            int result = await this.tbKeeperService.UpdateKeeperOccupationAsync(keeperId, occupationId);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        //tbKeepPlace
        [ValidateState(nameof(keepPlaceNameVal))]
        public async Task<JsonResult> SaveKeepPlaceName(int keepPlaceId, string keepPlaceNameVal)
        {
            if (!await this.IsKeepPlaceExist(keepPlaceId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbKeepPlaceService.UpdateKeepPlaceNameAsync(keepPlaceId, keepPlaceNameVal.ToSanitize());

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        //refRequestStamp
        [ValidateState(nameof(orderNumberVal))]
        public async Task<JsonResult> SaveOrderNumber(int requestId, string orderNumberVal)
        {
            if (!await this.IsRefRequestStampExist(requestId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.refRequestStampService.UpdateOrderNumberAsync(requestId, orderNumberVal);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateState(nameof(orderDateVal))]
        public async Task<JsonResult> SaveOrderDate(int stampId, int requestId, string orderDateVal)
        {
            if (!await this.IsRefRequestStampExist(requestId))
            {
                return Json(JsonStatus.NotFound);
            }

            DateTime orderDate = DateTime.ParseExact(orderDateVal, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            int result = await this.refRequestStampService.UpdateOrderDateAsync(requestId, orderDate);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateState(nameof(startDateVal))]
        public async Task<JsonResult> SaveStartDate(int stampId, int requestId, string startDateVal)
        {
            if (!await this.IsRefRequestStampExist(requestId))
            {
                return Json(JsonStatus.NotFound);
            }

            DateTime startDate = DateTime.ParseExact(startDateVal, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            int result = await this.refRequestStampService.UpdateStartDateAsync(requestId, startDate);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        [ValidateState(nameof(endDateVal))]
        public async Task<JsonResult> SaveEndDate(int stampId, int requestId, string endDateVal)
        {
            if (!await this.IsRefRequestStampExist(requestId))
            {
                return Json(JsonStatus.NotFound);
            }

            DateTime endDate = DateTime.ParseExact(endDateVal, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            int result = await this.refRequestStampService.UpdateEndDateAsync(requestId, endDate);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        //tbRequestFile
        [HttpPost]
        public async Task<JsonResult> SaveUploadRequestFile(RequestFileRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return Json(JsonStatus.Fail);
            }

            int result = await this.tbRequestFileService.InsertRequestFileAsync(this.SchoolId, request.RequestId.Value, request.FileTypeVal, request.UploadData);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        public async Task<JsonResult> ShowRequestFileList(int requestId)
        {
            IEnumerable<RequestFileResponseModel> fileList = await this.tbRequestFileService.GetRequestUploadFilesAsync(this.SchoolId, requestId);

            if (!fileList.Any())
            {
                return Json(JsonStatus.NoFiles);
            }

            return Json(fileList);
        }

        public async Task<JsonResult> DeleteRequestFile(int fileId, int requestId)
        {
            if (!await this.IsRequestFileExist(requestId, fileId))
            {
                return Json(JsonStatus.NotFound);
            }

            int result = await this.tbRequestFileService.DeleteRequestFileAsync(this.SchoolId, requestId, fileId);

            if (result == JsonStatus.NoChange)
            {
                return Json(JsonStatus.NoChange);
            }

            return Json(JsonStatus.Success);
        }

        private async Task<bool> IsStampExist(int stampId) 
             => await this.tbStampService.IsExistAsync(this.SchoolId, stampId);

        private async Task<bool> IsKeeperExist(int keeperId)
             => await this.tbKeeperService.IsExistAsync(keeperId);

        private async Task<bool> IsKeepPlaceExist(int keepPlaceId)
             => await this.tbKeepPlaceService.IsExistAsync(keepPlaceId);

        private async Task<bool> IsRequestFileExist(int requestId, int fileId)
            => await this.tbRequestFileService.IsExistAsync(this.SchoolId, requestId, fileId);

        private async Task<bool> IsRefRequestStampExist(int requestId)
            => await this.refRequestStampService.IsExistAsync(this.SchoolId, requestId);
    }
}
