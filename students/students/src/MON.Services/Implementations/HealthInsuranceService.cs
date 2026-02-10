using Microsoft.EntityFrameworkCore;
using MON.Models.Grid;
using MON.Models.HealthInsurance;
using MON.Services.Interfaces;
using MON.Shared;
using MON.Shared.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using MON.Services.Security.Permissions;
using MON.DataAccess;
using System.Text;
using MON.Models.Blob;
using MON.Models;
using MON.Models.Configuration;
using Microsoft.Extensions.Options;
using MON.Shared.ErrorHandling;
using Newtonsoft.Json;

namespace MON.Services.Implementations
{
    public class HealthInsuranceService : BaseService<HealthInsuranceService>, IHealthInsuranceService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IAppConfigurationService _configurationService;
        private HealthInsuranceConfig _appConfig = null;

        public HealthInsuranceService(DbServiceDependencies<HealthInsuranceService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IAppConfigurationService configurationService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _configurationService = configurationService;
        }

        #region Private members
        private HealthInsuranceConfig AppConfig
        {
            get
            {
                if (_appConfig == null)
                {
                    string healtInsuranceSettings = _configurationService.GetValueByKey("HealtInsurance").Result;
                    if (healtInsuranceSettings.IsNullOrWhiteSpace())
                    {
                        throw new ApiException("Mission HealtInsurance app setttings");
                    }

                    _appConfig = JsonConvert.DeserializeObject<HealthInsuranceConfig>(healtInsuranceSettings);
                }

                return _appConfig;
            }
        }

        private async Task CheckPermissions()
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForHealthInsuranceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        private async Task<byte[]> GetBytes(HealthInsuranceStudentsFileModel healthInsuranceStudentsFileModel, int currentUserInstitutionId)
        {
            if (healthInsuranceStudentsFileModel == null)
            {
                return null;
            }

            string institutionVatNumber = await _context.InstitutionAlls.Where(x => x.InstitutionId == currentUserInstitutionId).Select(x => x.Bulstat).FirstOrDefaultAsync();

            var sb = new StringBuilder();
            foreach (HealthInsuranceStudentsViewModel model in healthInsuranceStudentsFileModel.HealthInsuranceStudentsModel.Where(x => !x.IsExcludeFromList))
            {
                sb.Append(model.ToFileLine(healthInsuranceStudentsFileModel.Year, healthInsuranceStudentsFileModel.Month, AppConfig.VatNumber, institutionVatNumber, AppConfig.InsuranceType));
                // Добавяне на нов ред. StringBulder appendLine използва Environment.NewLine, което в Linux е "\n".
                sb.Append("\r\n");
            }
            sb.Append(((char)26).ToString());

            Encoding enc1251 = Encoding.GetEncoding(1251);
            byte[] bytes = enc1251.GetBytes(sb.ToString());

            return bytes;
        }
        #endregion

        public async Task<IPagedList<HealthInsuranceStudentsViewModel>> List(HealthInsuranceDataListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            await CheckPermissions();

            if (!input.Year.HasValue || !input.Month.HasValue)
            {
                return null;
            }

            DateTime startDate = new DateTime(input.Year.Value, input.Month.Value, 1);
            short schoolYear = Common.GetSchoolYearFromYearMonth(input.Year.Value, input.Month.Value);

            var healthInsuranceIncomeRate = await _context.HealthInsuranceIncomeRates
                .Where(x => x.ValidFrom <= startDate && startDate < x.ValidTo)
                .Select(x => new
                {
                    x.MinimalInsuranceIncomeRate,
                    x.Currency,
                    x.AltCurrency,
                    x.AltMinimalInsuranceIncomeRate,
                    x.InsuranceContributionPercentage
                })
                .FirstOrDefaultAsync();

            // Търси се състоянието в даден период от едим месец.
            // Възможно се да има повече от един запис за даден ученик.
            // Примерно в рамките на едим месец има промяна на даден StudentClass.
            // Тогава се появяват редове за един StudentClass, но с различни ValidFrom и ValidTo.
            // RowNumber ги подрежда по ValidTo desc.

            FormattableString queryString = $"select * from student.fn_HealthInsurance_For_Institution({_userInfo.InstitutionID}, {startDate})";
            IQueryable<HealthInsuranceStudentsViewModel> query = _context.Set<HealthInsuranceDto>()
                .FromSqlInterpolated(queryString)
                .Where(x => x.RowNumber == 1)
                .Select(x => new HealthInsuranceStudentsViewModel
                {
                    PersonId = x.PersonId,
                    Pin = x.Pin,
                    PinType = x.PinType,
                    PinTypeId = x.PinTypeId,
                    BirthDate = x.BirthDate,
                    FullName = x.FullName,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    ClassName = x.ClassName,
                    PaymentStartByEnrollmentDate = x.PaymentStartByEnrollmentDate,
                    PaymentEndByEnrollmentDate = x.PaymentEndByEnrollmentDate,
                    PaymentStartByBirthDate = x.PaymentStartByBirthDate,
                    PaymentEndByBirthDate = x.PaymentEndByBirthDate,
                    MonthDays = x.MonthDays,
                    DaysToPayByBirtDate = x.DaysToPayByBirtDate,
                    DaysToPayByEnrollmentDate = x.DaysToPayByEnrollmentDate
                });

            // Филтрирането и сортирането ще го правим на извлечениете данни, които не са много(стотици реда), а не на сървъра.
            // Търси се в темпорални таблици и търсенето и сортирането бави много.
            List<HealthInsuranceStudentsViewModel> items = (await query.ToListAsync())
                .AsQueryable()
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Pin.Contains(input.Filter)
                   || predicate.FullName.Contains(input.Filter)
                   || predicate.ClassName.Contains(input.Filter))
                 .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "FullName asc" : input.SortBy)
                 .ToList();

            foreach (var item in items)
            {
                if (item.DaysToPayByBirtDate < item.DaysToPayByEnrollmentDate)
                {
                    item.StartDayNumber = item.PaymentStartByBirthDate.HasValue ? item.PaymentStartByBirthDate.Value.Day : 0;
                    item.EndDayNumber = item.PaymentEndByBirthDate.HasValue ? item.PaymentEndByBirthDate.Value.Day : 0;
                }
                else
                {
                    item.StartDayNumber = item.PaymentStartByEnrollmentDate.HasValue ? item.PaymentStartByEnrollmentDate.Value.Day : 0;
                    item.EndDayNumber = item.PaymentEndByEnrollmentDate.HasValue ? item.PaymentEndByEnrollmentDate.Value.Day : 0;
                }

                item.MinimalInsuranceIncomeRate = healthInsuranceIncomeRate?.MinimalInsuranceIncomeRate ?? 0;
                item.InsuranceContributionPercentage = healthInsuranceIncomeRate?.InsuranceContributionPercentage ?? 0;
                item.Currency = healthInsuranceIncomeRate?.Currency ?? "";
                item.AltCurrency = healthInsuranceIncomeRate?.AltCurrency ?? "";
                item.AltMinimalInsuranceIncomeRate = healthInsuranceIncomeRate?.AltMinimalInsuranceIncomeRate ?? 0;
            }

            return items.ToPagedList(items.Count());
        }

        public async Task<IPagedList<HealthInsuranceExportFileModel>> ExportsList(HealthInsuranceDataListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            await CheckPermissions();

            IQueryable<HealthInsuranceExport> query = _context.HealthInsuranceExports
                .AsNoTracking()
                .Where(x => x.InstitutionId == _userInfo.InstitutionID)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.MonthNavigation.Name.ToString().Contains(input.Filter)
                   || predicate.SchoolYear.ToString().Contains(input.Filter));

            IQueryable<HealthInsuranceExportFileModel> listQuery = query
                .Select(x => new HealthInsuranceExportFileModel
                {
                    Id = x.Id,
                    Year = x.Year,
                    Month = x.Month,
                    BlobId = x.BlobId,
                    RecordsCount = x.RecordsCount,
                    CreateDate = x.CreateDate,
                    MonthName = x.MonthNavigation.Name,
                    CreatorUsername = x.CreatedBySysUser.Username
                })
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy);

            IList<HealthInsuranceExportFileModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            foreach (var item in items)
            {
                if (item.BlobId.HasValue)
                {
                    DocumentExtensions.CalcHmac(item, _blobServiceConfig);
                }
            }

            return items.ToPagedList(listQuery.Count());
        }

        public async Task GenerateHealthInsuranceFile(HealthInsuranceStudentsFileModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            await CheckPermissions();

            byte[] fileBytes = await GetBytes(model, _userInfo.InstitutionID.Value);
            ResultModel<BlobDO> blob = await _blobService.UploadFileAsync(fileBytes, AppConfig.FileName, AppConfig.FileMimeType);

            _context.HealthInsuranceExports.Add(new HealthInsuranceExport
            {
                Year = model.Year,
                Month = model.Month,
                BlobId = blob?.Data?.BlobId,
                RecordsCount = model.HealthInsuranceStudentsModel.Count(x => !x.IsExcludeFromList),
                InstitutionId = _userInfo.InstitutionID.Value
            });

            await SaveAsync();
        }

        public async Task Delete(int id)
        {
            await CheckPermissions();

            HealthInsuranceExport entity = await _context.HealthInsuranceExports
                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (entity.InstitutionId != _userInfo.InstitutionID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            _context.HealthInsuranceExports.Remove(entity);

            await SaveAsync();
        }
    }
}
