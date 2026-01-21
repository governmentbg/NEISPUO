using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MON.ASPDataAccess;
using MON.DataAccess;
using MON.Models;
using MON.Models.Absence;
using MON.Models.ASP;
using MON.Models.Configuration;
using MON.Models.Enums;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Enums.AspIntegration;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using MON.Shared.Extensions;
using MON.Models.Grid;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Threading;

namespace MON.Services.Implementations
{
    public class AbsenceCampaignService : BaseService<AbsenceCampaignService>, IAbsenceCampaignService
    {
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly IInstitutionService _institutionService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly MONASPContext _aspContext;

        public AbsenceCampaignService(DbServiceDependencies<AbsenceCampaignService> dependencies,
            ISignalRNotificationService signalRNotificationService,
            IInstitutionService institutionService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            MONASPContext aspContext)
            : base(dependencies)
        {
            _signalRNotificationService = signalRNotificationService;
            _institutionService = institutionService;
            _blobServiceConfig = blobServiceConfig.Value;
            _aspContext = aspContext;
        }

        public async Task<IPagedList<AbsenceCampaignViewModel>> List(PagedListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DateTime now = Now;

            IQueryable<AbsenceCampaignViewModel> query = _context.AbsenceCampaigns
                .AsNoTracking()
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Name.Contains(input.Filter)
                   || predicate.Description.Contains(input.Filter)
                   || predicate.SchoolYearNavigation.Name.Contains(input.Filter))
                .Select(x => new AbsenceCampaignViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    SchoolYear = x.SchoolYear,
                    Month = x.Month,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsManuallyActivated = x.IsManuallyActivated,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    CreateDate = x.CreateDate,
                    Creator = x.CreatedBySysUser.Username,
                    ModifyDate = x.ModifyDate,
                    Updater = x.ModifiedBySysUser.Username,
                    IsActive = x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1))
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "IsActive desc, Id desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            List<AbsenceCampaignViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();


            HashSet<DateTime> datesHash = new HashSet<DateTime>(items.Select(x => Common.GetYearFromSchoolYear(x.SchoolYear, x.Month).Date));
            string sessionInfoType = SessionInfoTypeEnum.AspAsking.GetEnumDescriptionAttrValue();

            List<DateTime> aspAskingInfos = null;
            try
            {
                aspAskingInfos = await _aspContext.Asp2monSessionInfos
                    .Where(x => datesHash.Contains(x.TargetMonth) && x.InfoType == sessionInfoType)
                    .Select(x => x.TargetMonth)
                    .ToListAsync(cancellationToken);
            }
            catch
            {
                // Ignore
            }

            if (aspAskingInfos != null)
            {
                foreach (var item in items)
                {
                    item.HasAspAskingSession = aspAskingInfos.Contains(Common.GetYearFromSchoolYear(item.SchoolYear, item.Month).Date);
                }
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<AbsenceReportViewModel>> AbsenceReportsList(AbsenceReportListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError);

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<VAbsenceReport> query = _context.VAbsenceReports
                .AsNoTracking();

            if (input.OnlyActiveAbsenceCampaigns == true)
            {
                query = query.Where(x => x.AbsenceCampaignIsActive == true);
            }

            switch (input.ReportTypeFilter)
            {
                case 0: // Неподадени отъствия
                    query = query.Where(x => x.AbsenceImportSubmitted != true);
                    break;
                case 1: // Подадени отсъствия
                    query = query.Where(x => x.AbsenceImportSubmitted == true);
                    break;
                case 2: // Неподписани отъствия
                    query = query.Where(x => x.AbsenceImportSubmitted == true && x.AbsenceImportIsSigned != true);
                    break;
                case 3: // Подписани отсъствия
                    query = query.Where(x => x.AbsenceImportSubmitted == true && x.AbsenceImportIsSigned == true);
                    break;
                case 4: // Неподадени или неподписани
                    query = query.Where(x => x.AbsenceImportSubmitted != true || x.AbsenceImportIsSigned != true);
                    break;
                case 5: // Всики
                default:
                    break;
            }

            switch (input.CampaignStatusFilter)
            {
                case 0: // Неактивна кампания
                    query = query.Where(x => x.AbsenceCampaignIsActive != true);
                    break;
                case 1: // Активна кампания
                    query = query.Where(x => x.AbsenceCampaignIsActive == true);
                    break;
                case 2: // Всики
                default:
                    break;
            }

            if (_userInfo.IsSchoolDirector)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID);
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => x.RegionId != null && x.RegionId == _userInfo.RegionID);
            }
            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear.Value);
            }
            if (input.Month.HasValue)
            {
                query = query.Where(x => x.Month == input.Month.Value);
            }

            query = query.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.InstitutionName.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionAbbreviation.Contains(input.Filter)
                   || predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.CreatorUsername.Contains(input.Filter)
                   || predicate.UpdaterUsername.Contains(input.Filter)
                   || predicate.InstitutionTown.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, Month desc" : input.SortBy);

            int totalCount = await query.CountAsync(cancellationToken);
            List<AbsenceReportViewModel> items = await query
                .Select(x => new AbsenceReportViewModel
                {
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionName,
                    InstitutionAbbreviation = x.InstitutionAbbreviation,
                    RegionId = x.RegionId,
                    InstitutionTown = x.InstitutionTown,
                    InstitutionMunicipality = x.InstitutionMunicipality,
                    InstitutionRegion = x.InstitutionRegion,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearName,
                    Month = x.Month,
                    AbsenceCampaignName = x.AbsenceCampaignName,
                    AbsenceCampaignDescription = x.AbsenceCampaignDescription,
                    AbsenceCampaignFromDate = x.AbsenceCampaignFromDate,
                    AbsenceCampaignToDate = x.AbsenceCampaignToDate,
                    AbsenceCampaignIsManuallyActivated = x.AbsenceCampaignIsManuallyActivated,
                    AbsenceCampaignIsActive = x.AbsenceCampaignIsActive,
                    AbsenceImportId = x.AbsenceImportId,
                    AbsenceImportBlobId = x.AbsenceImportBlobId,
                    BlobId = x.AbsenceImportBlobId,
                    AbsenceImportRecordsCount = x.AbsenceImportRecordsCount,
                    AbsenceImportIsSigned = x.AbsenceImportIsSigned,
                    AbsenceImportIsFinalized = x.AbsenceImportIsFinalized,
                    AbsenceImportSignedDate = x.AbsenceImportSignedDate,
                    AbsenceImportFinalizedDate = x.AbsenceImportFinalizedDate,
                    AbsenceImportCreateDate = x.AbsenceImportCreateDate,
                    AbsenceImportSubmitted = x.AbsenceImportSubmitted,
                    CreatorUsername = x.CreatorUsername,
                    AbsenceImportCmodifyDate = x.AbsenceImportCmodifyDate,
                    UpdaterUsername = x.UpdaterUsername
                })
                .PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                if (item.BlobId.HasValue)
                {
                    DocumentExtensions.CalcHmac(item, _blobServiceConfig);
                }
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<AbsenceCampaignInputModel> GetById(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await _context.AbsenceCampaigns
                .Where(x => x.Id == id)
                .Select(x => new AbsenceCampaignInputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    SchoolYear = x.SchoolYear,
                    Month = x.Month,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsManuallyActivated = x.IsManuallyActivated
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<AbsenceCampaignViewModel> GetDetailsById(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DateTime now = Now;

            return await _context.AbsenceCampaigns
               .Where(x => x.Id == id)
               .Select(x => new AbsenceCampaignViewModel
               {
                   Id = x.Id,
                   Name = x.Name,
                   Description = x.Description,
                   SchoolYear = x.SchoolYear,
                   Month = x.Month,
                   FromDate = x.FromDate,
                   ToDate = x.ToDate,
                   IsManuallyActivated = x.IsManuallyActivated,
                   SchoolYearName = x.SchoolYearNavigation.Name,
                   CreateDate = x.CreateDate,
                   Creator = x.CreatedBySysUser.Username,
                   ModifyDate = x.ModifyDate,
                   Updater = x.ModifiedBySysUser.Username,
                   IsActive = x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1))
               })
               .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<AspSessionInfoViewModel> GetAspSession(short schoolYear, short month, string infoType, CancellationToken cancellationToken)
        {
            try
            {
                DateTime targetMonth = Common.GetYearFromSchoolYear(schoolYear, month);
                var sessionInfo = await _aspContext.Asp2monSessionInfos
                    .Where(x => x.TargetMonth == targetMonth.Date && x.InfoType == infoType)
                    .Select(x => new AspSessionInfoViewModel
                    {
                        SessionNo = x.SessionNo,
                        TargetMonth = x.TargetMonth,
                        InfoType = x.InfoType,
                        MonProcessed = x.MonProcessed,
                    }).FirstOrDefaultAsync(cancellationToken);

                if (sessionInfo != null && sessionInfo.InfoType == SessionInfoTypeEnum.AspAsking.GetEnumDescription())
                {
                    sessionInfo.AbsenceCount = await _aspContext.MonAbs.CountAsync(x => x.SessionNoNavigation.TargetMonth == sessionInfo.TargetMonth.Date);
                    sessionInfo.ZpCount = await _aspContext.MonZps
                        .CountAsync(x => (x.SessionNoNavigation.TargetMonth == sessionInfo.TargetMonth.Date && x.SessionNoNavigation.InfoType == SessionInfoTypeEnum.MonEnrolledMain.GetEnumDescription())
                        || (x.SessionNoNavigation.TargetMonth == sessionInfo.TargetMonth.AddMonths(-1).Date && x.SessionNoNavigation.InfoType == SessionInfoTypeEnum.MonEnrolledCorrection.GetEnumDescription()));
                }

                if (sessionInfo != null && sessionInfo.InfoType == SessionInfoTypeEnum.AspConfirmation.GetEnumDescription())
                {
                    short aspSchoolYear = Common.GetSchoolYearFromYearMonth((short)sessionInfo.Year, (short)sessionInfo.Month);

                    sessionInfo.ConfirmationRecordsCount = await _aspContext.AspConfs.CountAsync(x => x.SessionNo == sessionInfo.SessionNo);
                    sessionInfo.HasConfirmationCampaign = await _context.AspmonthlyBenefitsImports.AnyAsync(x => x.ImportCompleted && x.SchoolYear == aspSchoolYear && x.Month == (short)sessionInfo.Month);
                }

                return sessionInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.GetInnerMostException().Message, ex);
                throw new ApiException("ASP integration db error. Check logs.");
            }
        }

        public async Task Create(AbsenceCampaignInputModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (model.ToDate < model.FromDate)
            {
                throw new ApiException("\"От дата\" следва да е преди \"До дата\".");
            }

            short currentYear = await _institutionService.GetCurrentYear(null);
            if (model.SchoolYear < currentYear)
            {
                throw new ApiException(Messages.PastSchoolYearError);
            }

            if (await _context.AbsenceCampaigns.AnyAsync(x => x.Month == model.Month && x.SchoolYear == currentYear))
            {
                throw new ApiException(Messages.AbsenceCampaignAlreadyExits);
            }

            AbsenceCampaign entry = new AbsenceCampaign
            {
                Name = model.Name,
                Description = model.Description,
                SchoolYear = model.SchoolYear,
                Month = model.Month,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsManuallyActivated = model.IsManuallyActivated
            };

            _context.AbsenceCampaigns.Add(entry);
            await SaveAsync();
            await SendNotification(entry);
            var payload = new { campaignId = entry.Id };

            var sqlASPCampaignStartParameters = new[]
            {
                    new SqlParameter("@TaskTypeCode", "ASPCampaignStart"),          // string
                    new SqlParameter("@Name", entry.Name),    // string
                    new SqlParameter("@SchoolYear", entry.SchoolYear),               // int
                    new SqlParameter("@ScheduledTime", DateTime.Now), // datetime
                    new SqlParameter("@Payload", JsonConvert.SerializeObject(payload)) // string/json
                };

            _ = await _context.Database.ExecuteSqlRawAsync("EXEC task.InsertScheduledTask  @TaskTypeCode, @Name, @SchoolYear, @ScheduledTime, @Payload", sqlASPCampaignStartParameters);

            var sqlASPCampaignEntryParameters = new[]
            {
                    new SqlParameter("@TaskTypeCode", "ASPCampaignEntry"),          // string
                    new SqlParameter("@Name", entry.Name),    // string
                    new SqlParameter("@SchoolYear", entry.SchoolYear),               // int
                    new SqlParameter("@ScheduledTime", entry.ToDate), // datetime
                    new SqlParameter("@Payload", JsonConvert.SerializeObject(payload)) // string/json
                };

            _ = await _context.Database.ExecuteSqlRawAsync("EXEC task.InsertScheduledTask  @TaskTypeCode, @Name, @SchoolYear, @ScheduledTime, @Payload", sqlASPCampaignEntryParameters);
        }

        public async Task Update(AbsenceCampaignInputModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            AbsenceCampaign entity = await _context.AbsenceCampaigns
               .FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new ApiException(Messages.EmptyEntityError);

            if (model.ToDate < model.FromDate)
            {
                throw new ApiException("\"От дата\" следва да е преди \"До дата\".");
            }

            short currentYear = await _institutionService.GetCurrentYear(null);
            if (model.SchoolYear < currentYear)
            {
                throw new ApiException(Messages.PastSchoolYearError);
            }

            if (await _context.AbsenceCampaigns.AnyAsync(x => x.Id != model.Id && x.SchoolYear == model.SchoolYear && x.Month == model.Month))
            {
                throw new ApiException(Messages.DocManagementCampaignAlreadyExits);
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.SchoolYear = model.SchoolYear;
            entity.Month = model.Month;
            entity.FromDate = model.FromDate;
            entity.ToDate = model.ToDate;
            entity.IsManuallyActivated = model.IsManuallyActivated;

            await SaveAsync();
            await SendNotification(entity);
        }

        public async Task ToggleManuallyActivation(AbsenceCampaignInputModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            AbsenceCampaign entity = await _context.AbsenceCampaigns
                .FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new ApiException(Messages.EmptyEntityError);

            entity.IsManuallyActivated = !entity.IsManuallyActivated;

            await SaveAsync();
            await SendNotification(entity);
        }

        public async Task<List<AbsenceCampaignViewModel>> GetActive(CancellationToken cancellationToken)
        {
            DateTime now = Now;

            var campaigns = await _context.AbsenceCampaigns
                .AsNoTracking()
                .Where(x => x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1)))
                .OrderByDescending(x => x.SchoolYear)
                .ThenByDescending(x => x.Month)
                .Select(x => new AbsenceCampaignViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    SchoolYear = x.SchoolYear,
                    Month = x.Month,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsManuallyActivated = x.IsManuallyActivated,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    IsActive = true,
                    CampaignType = CampaignType.Absence,
                    ImportId = _userInfo.InstitutionID.HasValue ? x.AbsenceImports.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value).Select(x => x.Id).FirstOrDefault() : default,
                })
                .ToListAsync(cancellationToken);

            campaigns.ForEach(campaign =>
            {
                campaign.Labels = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(campaign.CampaignTypeName, "primary") };
                if (campaign.IsActive)
                {
                    campaign.Labels.Add(new KeyValuePair<string, string>("Активна", "success"));
                }

                if (campaign.IsUpcoming)
                {
                    campaign.Labels.Add(new KeyValuePair<string, string>("Предстояща", "light"));
                }
            });


            return campaigns;
        }

        public async Task SendNotification(AbsenceCampaign entity)
        {
            // Todo: Да се развие логиката за изпрашане на нотификации
            try
            {
                await _signalRNotificationService.AbsenceCampaignModified(entity.Id);
            }
            catch (Exception ex)
            {
                // Ignore
                _logger.LogError($"AbsenceCampaign notification failed:{entity.Id}", ex);
            }
        }

        public async Task Delete(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            AbsenceCampaign entity = await _context.AbsenceCampaigns
                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (entity.FromDate <= Now || entity.IsManuallyActivated)
            {
                throw new ApiException("Кампанията е ръчно активирана или началната дата е настъпила");
            }

            _context.AbsenceCampaigns.Remove(entity);
            await SaveAsync();
        }

        public async Task<List<KeyValuePair<string, int>>> GetStats(int id)
        {
            var campaign = await _context.AbsenceCampaigns
                .Where(x => x.Id == id)
                .Select(x => new {
                    x.SchoolYear,
                    x.Month
                })
                .SingleOrDefaultAsync();

            if (campaign == null) throw new ApiException(Messages.EmptyEntityError);

            IQueryable<VAbsenceReport> query = _context.VAbsenceReports
                .Where(x => x.SchoolYear == campaign.SchoolYear && x.Month == campaign.Month)
                .AsNoTracking();


            switch (_userInfo.UserRole)
            {
                case Shared.Enums.UserRoleEnum.SchoolDirector:
                case Shared.Enums.UserRoleEnum.School:
                case Shared.Enums.UserRoleEnum.KindergartenDirector:
                case Shared.Enums.UserRoleEnum.CPLRDirector:
                case Shared.Enums.UserRoleEnum.CSOPDirector:
                case Shared.Enums.UserRoleEnum.SOZDirector:
                case Shared.Enums.UserRoleEnum.InstitutionAssociate:
                    query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID);
                    break;
                case Shared.Enums.UserRoleEnum.Ruo:
                case Shared.Enums.UserRoleEnum.RuoExpert:
                    query = query.Where(x => x.RegionId != null && x.RegionId == _userInfo.RegionID);
                    break;
                case Shared.Enums.UserRoleEnum.Mon:
                case Shared.Enums.UserRoleEnum.MonExpert:
                case Shared.Enums.UserRoleEnum.CIOO:
                case Shared.Enums.UserRoleEnum.MonOBGUM:
                case Shared.Enums.UserRoleEnum.MonOBGUM_Finance:
                case Shared.Enums.UserRoleEnum.MonHR:
                case Shared.Enums.UserRoleEnum.Consortium:
                    // Виждат всичко
                    break;
                default:
                    // Останалите не виждата нищо
                    return null;
            }

            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("absenceReport.filter.submitted", await query.CountAsync(x => x.AbsenceImportSubmitted == true)),
                new KeyValuePair<string, int>("absenceReport.filter.unsubmitted", await query.CountAsync(x => x.AbsenceImportSubmitted != true)),
                new KeyValuePair<string, int>("absenceReport.filter.signed", await query.CountAsync(x => x.AbsenceImportSubmitted == true && x.AbsenceImportIsSigned == true)),
                new KeyValuePair<string, int>("absenceReport.filter.unsigned", await query.CountAsync(x => x.AbsenceImportSubmitted == true && x.AbsenceImportIsSigned != true)),
                new KeyValuePair<string, int>("absenceReport.filter.all", await query.CountAsync())
            };

            return result;
        }

        public async Task<IPagedList<AspStudentGridItemModel>> GetASPStudentsForCampaign(int campaignId, PagedListInput input, CancellationToken cancellationToken)
        {
            // Списък с питанията е видим само за потребители, които могат да управляват кампании за отсъствия
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var campaign = await _context.AbsenceCampaigns
                .Where(x => x.Id == campaignId)
                .Select(x => new
                {
                    x.SchoolYear,
                    x.Month
                })
            .FirstOrDefaultAsync(cancellationToken);

            if (campaign == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            DateTime targetMonth = Common.GetYearFromSchoolYear(campaign.SchoolYear, campaign.Month);
            var askingSessionInfoNumber = await _aspContext.Asp2monSessionInfos
                .Where(x => x.TargetMonth == targetMonth.Date && x.InfoType == SessionInfoTypeEnum.AspAsking.GetEnumDescription())
                .Select(x => x.SessionNo)
                .FirstOrDefaultAsync(cancellationToken);

            var aspRequriedIdentificationNumbersQuery = _aspContext.AspAskings
                .Where(x => x.SessionNo == askingSessionInfoNumber)
                .Where(x => string.IsNullOrEmpty(input.Filter) || x.IdNumber.Contains(input.Filter))
                .Select(x => x.IdNumber);

            var aspREquiredIdentificationNumers = await aspRequriedIdentificationNumbersQuery
                    .PagedBy(input.PageIndex, input.PageSize)
                    .ToListAsync(cancellationToken);

            var studentsList = await _context.People
                .Where(x => aspREquiredIdentificationNumers.Contains(x.PersonalId))
                .Select(x => new AspStudentGridItemModel
                {
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    IdNumber = x.PersonalId,
                    IdTypeName = x.PersonalIdtypeNavigation.Name,
                    PublicEduNumber = x.PublicEduNumber
                })
                .ToListAsync(cancellationToken);

            int totalCount = await aspRequriedIdentificationNumbersQuery.CountAsync(cancellationToken);
            return studentsList.ToPagedList(totalCount);
        }

        public async Task<IPagedList<AspMonAbsenceViewModel>> GetAspAbsencesForCampaign(int campaignId, PagedListInput input, CancellationToken cancellationToken)
        {
            // Списък с подадените към АСП отсъствия е видим само за потребители, които могат да управляват кампании за отсъствия
           if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var campaign = await _context.AbsenceCampaigns
                 .Where(x => x.Id == campaignId)
                 .Select(x => new
                 {
                     x.SchoolYear,
                     x.Month
                 })
             .FirstOrDefaultAsync(cancellationToken);

            if (campaign == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            DateTime targetMonth = Common.GetYearFromSchoolYear(campaign.SchoolYear, campaign.Month);
            var askingSessionInfoNumber = await _aspContext.Mon2aspSessionInfos
                .Where(x => x.TargetMonth == targetMonth.Date && x.InfoType == SessionInfoTypeEnum.MonAbsence.GetEnumDescription())
                .Select(x => x.SessionNo)
                .FirstOrDefaultAsync(cancellationToken);

            IQueryable<AspMonAbsenceViewModel> query = _aspContext.MonAbs
                .Where(x => x.SessionNo == askingSessionInfoNumber)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.SchoolId.Contains(input.Filter)
                   || predicate.IdNumber.Contains(input.Filter))
                .Select(x => new AspMonAbsenceViewModel
                {
                    Id = x.Id,
                    IntYear = x.IntYear,
                    IntMonth = x.IntMonth,
                    Pin = x.IdNumber,
                    PinType = x.IdTypeName,
                    NotExcused = x.NotExcused,
                    InstitutionCode = x.SchoolId,
                    BasicClass = x.BasicClass
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id asc" : input.SortBy); ;

            int totalCount = await query.CountAsync(cancellationToken);
            List<AspMonAbsenceViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<AspMonZpViewModel>> GetAspZpForCampaign(int campaignId, AspMonZpListInput input, CancellationToken cancellationToken)
        {
            // Списък с подадените към АСП даннни за Записани/Отписани е видим само за потребители, които могат да управляват кампании за отсъствия
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var campaign = await _context.AbsenceCampaigns
                 .Where(x => x.Id == campaignId)
                 .Select(x => new
                 {
                     x.SchoolYear,
                     x.Month
                 })
             .FirstOrDefaultAsync(cancellationToken);

            if (campaign == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            DateTime targetMonth = Common.GetYearFromSchoolYear(campaign.SchoolYear, campaign.Month);
            var askingSessionInfoNumber = await _aspContext.Mon2aspSessionInfos
                .Where(x => x.TargetMonth == targetMonth.Date && x.InfoType == SessionInfoTypeEnum.MonEnrolledMain.GetEnumDescription())
                .Select(x => x.SessionNo)
                .FirstOrDefaultAsync(cancellationToken);

            IQueryable<MonZp> query = _aspContext.MonZps
                .Where(x => x.SessionNo == askingSessionInfoNumber);

            if (input.StatusTypeFilter.HasValue && input.StatusTypeFilter.Value != 0 ) // 0 - Всички
            {
                query = query.Where(x => x.Sstatus == input.StatusTypeFilter);
            }

            IQueryable<AspMonZpViewModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.SchoolId.Contains(input.Filter)
                   || predicate.IdNumber.Contains(input.Filter)
                   || predicate.Name1.Contains(input.Filter)
                   || predicate.Name2.Contains(input.Filter)
                   || predicate.Name3.Contains(input.Filter)
                   || predicate.SstatusName.Contains(input.Filter))
                .Select(x => new AspMonZpViewModel
                {
                    Id = x.Id,
                    InstitutionCode = x.SchoolId,   
                    Pin = x.IdNumber,
                    PinType = x.IdTypeName,
                    FirstName = x.Name1,
                    MiddleName = x.Name2,
                    LastName = x.Name3,
                    BasicClass = x.BasicClass,
                    EduForm = x.EdForm,
                    Status = x.SstatusName
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<AspMonZpViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }
    }
}
