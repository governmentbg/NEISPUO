namespace RegStamps.Web.Controllers
{
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Attributes;
    using Infrastructure.Extensions;

    using Services.Neispuo;
    using Services.Entities;
    using Services.DocumentFiles;

    using Models.Stores;
    using Models.Stamp.ListStamp.Response;
    using Models.Stamp.RequestsForStamp.Response;
    using Models.Stamp.StampDetails.Response;
    using Models.Stamp.RequestDetails.Response;
    using Models.Stamp.EditStamp;
    using Models.Constants;

    [ValidateAuthenticationUser]
    public class StampController : BaseController
    {
        private readonly ITbStampService tbStampService;
        private readonly IRefRequestStampService refRequestStampService;
        private readonly ITbRequestFileService tbRequestFileService;
        private readonly ITbKeeperService tbKeeperService;
        private readonly ICodeStampTypeService codeStampTypeService;
        private readonly ICodeFileTypeService codeFileTypeService;
        private readonly IDocumentFileService documentFileService;

        private readonly UserManager<ApplicationUser> userManager;

        public StampController(
            ITbStampService tbStampService,
            IRefRequestStampService refRequestStampService,
            ITbRequestFileService tbRequestFileService,
            ITbKeeperService tbKeeperService,
            ICodeStampTypeService codeStampTypeService,
            ICodeFileTypeService codeFileTypeService,
            IDocumentFileService documentFileService,
            INeispuoService neispuoService,
            UserManager<ApplicationUser> userManager) : base(neispuoService)
        {
            this.tbStampService = tbStampService;
            this.refRequestStampService = refRequestStampService;
            this.tbRequestFileService = tbRequestFileService;
            this.tbKeeperService = tbKeeperService;
            this.codeStampTypeService = codeStampTypeService;
            this.codeFileTypeService = codeFileTypeService;
            this.documentFileService = documentFileService;

            this.userManager = userManager;
        }

        public int SchoolId => Convert.ToInt32(this.userManager.GetUserId(this.User));

        [HttpGet]
        public async Task<ActionResult> ListStamp()
            => View(new ListStampResponseModel
            {
                SchoolData = await base.PrepareSchoolDataAsync(this.SchoolId),
                StampDataList = await this.tbStampService.GetAllStampsAsync(this.SchoolId),
                RequestBackDataList = await this.refRequestStampService.GetReturnBackRequestStamps(this.SchoolId)
            });

        //requests for stamp
        [HttpGet]
        public async Task<ActionResult> RequestsForStamp(string param)
        {
            if (param is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            RequestsForStampResponseModel model = new RequestsForStampResponseModel();
            model.SchoolData = await base.PrepareSchoolDataAsync(this.SchoolId);
            model.RequestStampList = await this.refRequestStampService.GetRequestsForStamp(this.SchoolId, stampId);

            ViewBag.StampId = stampId;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> CreateStamp()
        {
            int stampId = await this.tbStampService.CreateStampIdAsync();

            int requestId = await this.tbStampService.CreateRequestNewStampAsync(this.SchoolId, stampId);

            return RedirectToAction(nameof(EditStamp), new { param = stampId.ToBase64Convert(), paramReq = requestId.ToBase64Convert() });
        }

        [HttpGet]
        public async Task<ActionResult> EditStamp(string param, string paramReq)
        {
            if (param is null || paramReq is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();
            int requestId = paramReq.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            EditStampViewModel model = new EditStampViewModel();
            model.StampData = await this.tbStampService.GetStampDataAsync(this.SchoolId, stampId);
            model.RequestStampData = await this.refRequestStampService.GetRequestDataAsync(this.SchoolId, stampId, requestId);
            model.KeeperData = await this.tbKeeperService.GetKeeperDataAsync(model.RequestStampData.KeeperId);
            model.KeeperData.OccupationDropDown = await this.neispuoService.GetOccupationsAsync();
            model.RequestFileData.FileTypeDropDown = await this.codeFileTypeService.GetFileTypesAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateActiveRequest]
        public async Task<ActionResult> ChangeStampKeeper(string param)
        {
            if (param is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            int requestId = await this.tbStampService.CreateRequestChangeStampKeeperAsync(this.SchoolId, stampId);

            base.AddSuccessMessage($"Заявка за промяна на пазител на печат с УИН {stampId} е създадена успешно, моля, попълнете необходимите данни.");
            return RedirectToAction(nameof(EditStampChangeKeeper), new { paramStamp = stampId.ToBase64Convert(), paramReq = requestId.ToBase64Convert() });
        }

        [HttpGet]
        public async Task<ActionResult> EditStampChangeKeeper(string paramStamp, string paramReq)
        {
            if (paramStamp is null || paramReq is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = paramStamp.FromBase64ConvertToInt();
            int requestId = paramReq.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            //bool isExistRegisteredStamp = await unitOfStamp.TbStampRepo.GetFirstOrNull(where: x => x.StampId == stampId && x.SchoolId == schoolId && x.StampStatus == 2);
            //if (stampData == null)
            //{
            //    message = string.Format($"Печат с УИН {stampId} трябва да бъде одобрен от МОН. Нямате права да го променяте.");
            //    this.AddNotification(message, NotificationType.WARNING);
            //    return RedirectToAction("ListStamp", "Stamp");
            //}

            EditStampViewModel model = new EditStampViewModel();
            model.StampData = await this.tbStampService.GetStampDataAsync(this.SchoolId, stampId);
            model.RequestStampData = await this.refRequestStampService.GetRequestDataAsync(this.SchoolId, stampId, requestId);
            model.KeeperData = await this.tbKeeperService.GetKeeperDataAsync(model.RequestStampData.KeeperId);
            model.KeeperData.OccupationDropDown = await this.neispuoService.GetOccupationsAsync();
            model.RequestFileData.FileTypeDropDown = await this.codeFileTypeService.GetFileTypesAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateActiveRequest]
        public async Task<ActionResult> DestroyStamp(string param)
        {
            if (param is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            int requestId = await this.tbStampService.CreateRequestDestroyStampAsync(this.SchoolId, stampId);

            base.AddSuccessMessage($"Заявка за унищожаване на печат УИН {stampId} е създадена успешно, моля, попълнете необходимите данни.");
            return RedirectToAction(nameof(EditDestroyStamp), new { paramStamp = stampId.ToBase64Convert(), paramReq = requestId.ToBase64Convert() });
        }

        [HttpGet]
        public async Task<ActionResult> EditDestroyStamp(string paramStamp, string paramReq)
        {
            if (paramStamp is null || paramReq is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = paramStamp.FromBase64ConvertToInt();
            int requestId = paramReq.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            ////table
            //tbStamp stampData = await unitOfStamp.TbStampRepo.GetFirstOrNull(where: x => x.StampId == stampId && x.SchoolId == schoolId && x.StampStatus == 2);
            //if (stampData == null)
            //{
            //    message = string.Format($"Заявка за печат {stampId} не е намерена. Нямате права за достъп.");
            //    this.AddNotification(message, NotificationType.WARNING);
            //    return RedirectToAction("ListStamp", "Stamp");
            //}

            EditStampViewModel model = new EditStampViewModel();
            model.StampData = await this.tbStampService.GetStampDataAsync(this.SchoolId, stampId);
            model.RequestStampData = await this.refRequestStampService.GetRequestDataAsync(this.SchoolId, stampId, requestId);
            model.KeeperData = await this.tbKeeperService.GetKeeperDataAsync(model.RequestStampData.KeeperId);
            model.KeeperData.OccupationDropDown = await this.neispuoService.GetOccupationsAsync();
            model.RequestFileData.FileTypeDropDown = await this.codeFileTypeService.GetFileTypesAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateActiveRequest]
        public async Task<ActionResult> LostStamp(string param)
        {
            if (param is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            int requestId = await this.tbStampService.CreateRequestLostStampAsync(this.SchoolId, stampId);

            this.AddSuccessMessage($"Заявка за обявяване на печат УИН {stampId} за загубен е създадена успешно, моля, попълнете необходимите данни.");
            return RedirectToAction(nameof(EditLostStamp), new { paramStamp = stampId.ToBase64Convert(), paramReq = requestId.ToBase64Convert() });

        }

        [HttpGet]
        public async Task<ActionResult> EditLostStamp(string paramStamp, string paramReq)
        {
            if (paramStamp is null || paramReq is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = paramStamp.FromBase64ConvertToInt();
            int requestId = paramReq.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            //table
            //tbStamp stampData = await unitOfStamp.TbStampRepo.GetFirstOrNull(where: x => x.StampId == stampId && x.SchoolId == schoolId && x.StampStatus == 2);
            //if (stampData == null)
            //{
            //    message = string.Format($"Заявка за печат {stampId} не е намерена. Нямате права за достъп.");
            //    this.AddNotification(message, NotificationType.WARNING);
            //    return RedirectToAction("ListStamp", "Stamp");
            //}

            EditStampViewModel model = new EditStampViewModel();
            model.StampData = await this.tbStampService.GetStampDataAsync(this.SchoolId, stampId);
            model.RequestStampData = await this.refRequestStampService.GetRequestDataAsync(this.SchoolId, stampId, requestId);
            model.KeeperData = await this.tbKeeperService.GetKeeperDataAsync(model.RequestStampData.KeeperId);
            model.KeeperData.OccupationDropDown = await this.neispuoService.GetOccupationsAsync();
            model.RequestFileData.FileTypeDropDown = await this.codeFileTypeService.GetFileTypesAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> StampDetails(string param)
        {
            if (param is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            StampDetailsResponseModel model = new StampDetailsResponseModel();
            model.SchoolData = await base.PrepareSchoolDataAsync(this.SchoolId);
            model.StampData = await this.tbStampService.GetStampDetailsAsync(this.SchoolId, stampId);
            model.RequestStampData = await this.refRequestStampService.GetRequestStampDetailsAsync(this.SchoolId, stampId);
            model.KeeperData = await this.tbKeeperService.GetKeeperDetailsAsync(model.RequestStampData.KeeperId);
            model.KeeperData.OccupationName = (await base.PrepareOccupationsAsync()).Where(x => x.OccupId == model.KeeperData.OccupationId).Select(x => x.OccupName).FirstOrDefault();
            model.RequestFileDataList = await this.tbRequestFileService.GetRequestFilesAsync(this.SchoolId, model.RequestStampData.RequestId);

            return View(model);
        }

        public async Task<ActionResult> RequestDetails(string paramStamp, string paramReq)
        {
            if (paramStamp is null || paramReq is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = paramStamp.FromBase64ConvertToInt();
            int requestId = paramReq.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            RequestDetailsResponseModel model = new RequestDetailsResponseModel();
            model.SchoolData = await base.PrepareSchoolDataAsync(this.SchoolId);
            model.StampData = await this.tbStampService.GetStampDetailsAsync(this.SchoolId, stampId);
            model.RequestStampData = await this.refRequestStampService.GetRequestDetailsAsync(this.SchoolId, stampId, requestId);
            model.KeeperData = await this.tbKeeperService.GetKeeperDetailsAsync(model.RequestStampData.KeeperId);
            model.KeeperData.OccupationName = (await base.PrepareOccupationsAsync()).Where(x => x.OccupId == model.KeeperData.OccupationId).Select(x => x.OccupName).FirstOrDefault();
            model.RequestFileDataList = await this.tbRequestFileService.GetRequestFilesAsync(this.SchoolId, requestId);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteStamp(string param)
        {
            if (param is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = param.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            await this.tbStampService.DeleteStampAsync(this.SchoolId, stampId);

            base.AddSuccessMessage($"Печат с УИН {stampId} е изтрит успешно.");

            return RedirectToAction(nameof(ListStamp));

        }

        [HttpPost]
        public async Task<ActionResult> DeleteRequest(string paramReq, string paramStamp)
        {
            if (paramReq is null || paramStamp is null)
            {
                return base.RedirectToActionError();
            }

            int requestId = paramReq.FromBase64ConvertToInt();
            int stampId = paramStamp.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            await this.tbStampService.DeleteStampRequestAsync(this.SchoolId, stampId, requestId);

            base.AddSuccessMessage($"Заявка {requestId} е изтрита успешно.");

            return RedirectToAction(nameof(RequestsForStamp), new { param = stampId.ToBase64Convert() });
        }


        [HttpPost]
        public async Task<ActionResult> SendStampRequest(string paramReq, string paramStamp, string signData)
        {
            if (paramReq is null || paramStamp is null)
            {
                return base.RedirectToActionError();
            }

            int stampId = paramStamp.FromBase64ConvertToInt();
            int requestId = paramReq.FromBase64ConvertToInt();

            if (stampId < 100000 || stampId > 999999)
            {
                return base.RedirectToActionError();
            }

            X509Certificate2 certificate = new X509Certificate2(signData.ToByteArray());

            if (certificate is null)
            {
                return RedirectToAction(ActionNameType.NoCertificate, ControllerNameType.Signature);
            }

            (int result, string message) = await this.tbStampService.SendStampRequestAsync(this.SchoolId, stampId, requestId, signData, certificate);

            base.AddSuccessMessage(message);

            return RedirectToAction(nameof(ListStamp));
        }
    }
}
