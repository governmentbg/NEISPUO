namespace MON.Services.Implementations
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MON.ASPDataAccess;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.ASP;
    using MON.Models.Configuration;
    using MON.Services.Extensions;
    using MON.Services.Infrastructure.Validators;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.Enums.AspIntegration;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Extensions;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class AbsenceService : BaseService<AbsenceService>, IAbsenceService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly AbsencesImportValidator _validator;
        private readonly ICertificateService _certificateService;
        private readonly IAdministrationService _administrationService;
        private readonly IAbsenceCampaignService _absenceCampaignService;
        private const string EXPORT_HEADER = "intYear|intMonth|SchoolID|IDNumber|intIDType|BasicClass|NotExcused";

        private readonly MONASPContext _aspContext;
        private readonly IConfiguration _configuration;

        public AbsenceService(DbServiceDependencies<AbsenceService> dependencies,
            IBlobService blobService,
            AbsencesImportValidator validator,
            ICertificateService certificateService,
            IAdministrationService administrationService,
            IAbsenceCampaignService absenceCampaignService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            MONASPContext aspContext,
            IConfiguration configuration)
        : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _certificateService = certificateService;
            _administrationService = administrationService;
            _validator = validator;
            _aspContext = aspContext;
            _configuration = configuration;
            _absenceCampaignService = absenceCampaignService;
        }

        #region Private members
        private async Task<List<AbsenceImportModel>> ReadAbsencesFileAsync(IFormFile file)
        {
            // "Описание на структурата на експортен файл за модул „Отсъствия на децата и учениците“, част от Информационната система на образованието"
            // Файлът трябва да бъде в plain-text формат CSV с кодировка CP-1251(WINDOWS-1251).
            // Разделител на данните трябва да бъде Pipe (“|“). 

            var enc1251 = Encoding.GetEncoding("windows-1251");
            List<string> lines = await file.ReadAsListAsync(enc1251);
            List<AbsenceImportModel> studentsAbsences = ParseAbsenceImportModels(lines);

            return studentsAbsences;
        }
        private AbsenceImportModel ParseAbsenceImportModel(string line)
        {
            if (line.IsNullOrEmpty()) return null;

            string[] split = line.Split("|");
            AbsenceImportModel model = new AbsenceImportModel
            {
                Identification = split[4],
                FirstName = split[5],
                MiddleName = split[6],
                LastName = split[7],
                ClassCode = split[9],
                GroupCode = split[10],
                ClassName = split[11]
            };

            if (int.TryParse(split[0], out int institutionCode))
            {
                model.InstitutionCode = institutionCode;
            }

            if (int.TryParse(split[1], out int schooYear))
            {
                model.SchoolYear = schooYear;
            }

            if (int.TryParse(split[2], out int month))
            {
                model.Month = month;
            }

            if (int.TryParse(split[3], out int studentIdentificationType))
            {
                model.StudentIdentificationType = studentIdentificationType;
            }

            if (int.TryParse(split[8], out int basicClassId))
            {
                model.Class = basicClassId;
            }

            if (int.TryParse(split[12], out int attendsClassRegularly))
            {
                model.AttendsClassRegularly = attendsClassRegularly;
            }

            if (DateTime.TryParse(split[13], out DateTime unsubscriptionDate))
            {
                model.UnsubscriptionDate = unsubscriptionDate;
            }

            if (decimal.TryParse(split[14]?.Replace(',', '.') ?? "", NumberStyles.Any, CultureInfo.InvariantCulture, out decimal monthlyAbsencesForValidReason))
            {
                model.MonthlyAbsencesForValidReason = monthlyAbsencesForValidReason;
            }

            if (decimal.TryParse(split[15]?.Replace(',', '.') ?? "", NumberStyles.Any, CultureInfo.InvariantCulture, out decimal monthlyAbsencesForUnvalidReason))
            {
                model.MonthlyAbsencesForUnvalidReason = monthlyAbsencesForUnvalidReason;
            }

            if (!model.Identification.IsNullOrEmpty())
            {
                model.Identification = model.Identification.PadLeft(10, '0');
            }

            return model;
        }
        private List<AbsenceImportModel> ParseAbsenceImportModels(List<string> lines)
        {
            if (lines.IsNullOrEmpty()) return null;

            List<AbsenceImportModel> studentsAbsences = new List<AbsenceImportModel>();
            foreach (string line in lines)
            {
                try
                {
                    AbsenceImportModel model = ParseAbsenceImportModel(line);
                    if (model != null)
                    {
                        studentsAbsences.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    var error = new
                    {
                        ex.GetInnerMostException().Message,
                        Action = "ParseAbsenceImportModel",
                        Data = line
                    };

                    _logger.LogError(JsonSerializer.Serialize(error));

                    if (ex is IndexOutOfRangeException)
                    {
                        throw new Exception("Файлът и редовете в него следва да отговарят на изискванията за импорт на отсъствия от файл. Разделителят трябва да е |. На всеки ред трябва да има точно 15 разделителя.");
                    }

                    throw;
                }
            }

            return studentsAbsences;
        }
        private IQueryable<InstitutionSchoolYear> GetInstitutionsListQuery(short schoolYear)
        {
            IQueryable<InstitutionSchoolYear> query = _context.InstitutionSchoolYears.Where(x => x.SchoolYear == schoolYear);

            return _userInfo.SysRoleID switch
            {
                (int)UserRoleEnum.School => query.Where(i => i.InstitutionId == _userInfo.InstitutionID),
                (int)UserRoleEnum.Mon => query,
                (int)UserRoleEnum.Ruo => query.Where(i => i.Town.Municipality.RegionId == _userInfo.RegionID),
                _ => query.Where(i => i.InstitutionId == int.MinValue),
            };
        }
        private IQueryable<AbsenceImport> GetAbsenceImportsListQuery()
        {
            IQueryable<AbsenceImport> query = _context.AbsenceImports;

            return _userInfo.SysRoleID switch
            {
                (int)UserRoleEnum.School => query.Where(i => i.InstitutionId == _userInfo.InstitutionID),
                (int)UserRoleEnum.Mon => query,
                (int)UserRoleEnum.Ruo => query.Where(i => i.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID),
                _ => query.Where(i => i.InstitutionId == int.MinValue),
            };
        }
        public async Task<List<AbsenceImportModel>> GetManualImportSampleData(short schoolYear, short month)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            int institutionId = _userInfo?.InstitutionID ?? throw new InvalidOperationException(Messages.InvalidInstitutionCodeError);

            List<AbsenceImportModel> absences = await _context.Absences
                .Where(x => x.InstitutionId == institutionId
                    && x.SchoolYear == schoolYear && x.Month == month
                    && (x.AbsenceImportId == null || x.AbsenceImport.ImportType == (byte)AbsenceImportTypeEnum.Manual)
                    && (x.Excused > 0 || x.Unexcused > 0))
                    .Select(x => new AbsenceImportModel
                    {
                        StudentAbsenceId = x.Id,
                        InstitutionCode = x.InstitutionId,
                        SchoolYear = x.SchoolYear,
                        SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                        Month = x.Month,
                        StudentIdentificationType = x.Person.PersonalIdtype ?? 0,
                        Identification = x.Person.PersonalId,
                        FirstName = x.Person.FirstName,
                        MiddleName = x.Person.MiddleName,
                        LastName = x.Person.LastName,
                        Class = x.Class.BasicClassId ?? default,
                        //ClassCode = split[9],
                        //GroupCode = split[10],
                        ClassName = x.Class.ClassName,
                        AttendsClassRegularly = default,
                        UnsubscriptionDate = null,
                        MonthlyAbsencesForValidReason = x.Excused,
                        MonthlyAbsencesForUnvalidReason = x.Unexcused,
                        PersonId = x.PersonId
                    })
                    .ToListAsync();

            return absences;
        }
        private byte[] GetBytes(List<AbsenceImportModel> absences)
        {
            if (absences == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (AbsenceImportModel absence in absences)
            {
                sb.AppendLine(absence.ToFileLine());
            }

            Encoding utfEncoding = Encoding.Unicode;

            byte[] bytes = utfEncoding.GetBytes(sb.ToString());
            var enc1251 = Encoding.GetEncoding("windows-1251");
            return Encoding.Convert(utfEncoding, enc1251, bytes);
        }
        private byte[] GetBytes(List<AbsenceExportModel> absences)
        {
            if (absences == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            sb.AppendLine(EXPORT_HEADER);
            foreach (AbsenceExportModel absence in absences)
            {
                sb.AppendLine(absence.ToFileLine());
            }

            sb.AppendLine(((char)26).ToString());

            Encoding utfEncoding = Encoding.UTF8;

            byte[] bytes = utfEncoding.GetBytes(sb.ToString());
            return bytes;
        }
        private string GetFileName(int institutionId, int schoolYear, int month, AbsenceImportTypeEnum importType)
        {
            return $"{institutionId}_{schoolYear}_{month}_{importType}.txt";
        }

        private async Task CheckForActiveCampaign(StudentAbsenceInputModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(StudentAbsenceInputModel)));
            }

            List<AbsenceCampaignViewModel> absenceCampaigns = await _absenceCampaignService.GetActive(CancellationToken.None);
            if (absenceCampaigns.IsNullOrEmpty() || !absenceCampaigns.Any(x => x.SchoolYear == model.SchoolYear && x.Month == model.Month))
            {
                throw new ApiException(Messages.MissingActiveCampaignForSelectedSchoolYearAndMont, new InvalidOperationException());
            }
        }
        #endregion

        #region АСП интеграция

        /// <summary>
        /// Запитване от АСП за определен месец.
        /// </summary>
        /// <param name="targetMonth"></param>
        /// <returns></returns>
        private async Task<List<AspAskingModel>> GetAspAskingModels(DateTime targetMonth)
        {
            List<AspAskingModel> aspAskingModels = await _aspContext.Asp2monSessionInfos
                .Where(x => x.TargetMonth == targetMonth.Date && x.InfoType == SessionInfoTypeEnum.AspAsking.GetEnumDescription())
                .SelectMany(x => x.AspAskings.Select(a => new AspAskingModel
                {
                    IdNumber = a.IdNumber,
                    IdType = a.IdType,
                    FlagOsn = a.FlagOsn,
                    FlagKor = a.FlagKor
                }))
                .ToListAsync();

            return aspAskingModels;
        }

        /// <summary>
        /// Обработка и запис на отсъствията за дадено запитване на АСП.
        /// </summary>
        /// <param name="asp2MonSessionInfoId"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private async Task<int> ProcessAsp2MonAbsences(int asp2MonSessionInfoId)
        {
            // Използват се sp с цел да не се теглят данните в ram.
            var sessionNoParam = new SqlParameter("AspSessionInfoNo", SqlDbType.Int)
            {
                Value = asp2MonSessionInfoId
            };

            SqlParameter countParam = new SqlParameter
            {
                ParameterName = "Count",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output,
            };

            SqlParameter errorParam = new SqlParameter
            {
                ParameterName = "Error",
                DbType = DbType.String,
                Direction = ParameterDirection.Output,
                Size = -1
            };

            var sqlParameters = new[]
            {
                sessionNoParam,
                countParam,
                errorParam
            };

            _aspContext.Database.SetCommandTimeout(600);
            _ = await _aspContext.Database.ExecuteSqlRawAsync("EXEC [dbo].[sp_AspAbsProcessor] @AspSessionInfoNo, @Count OUTPUT, @Error OUTPUT", sqlParameters);

            string spErrors = errorParam.Value.ConvertFromDBVal<string>();
            if (!spErrors.IsNullOrWhiteSpace())
            {
                throw new ApiException(spErrors);
            }

            int savedRowsCount = countParam.Value.ConvertFromDBVal<int>();

            return savedRowsCount;
        }


        private async Task CopyAspAsking(DateTime targetMonth)
        {
            await _context.AspAskingTemps.Where(x => x.TargetMonth == targetMonth)
                .DeleteAsync();

            List<AspAskingModel> aspAskingModels = await GetAspAskingModels(targetMonth);

            if (aspAskingModels.IsNullOrEmpty())
            {
                return;
            }

            var sqlConnStringBuilder = new SqlConnectionStringBuilder(_configuration.GetConnectionString("DefaultConnection"));
            var dbPass = Environment.GetEnvironmentVariable("ST__Data__DbPass");
            if (!string.IsNullOrWhiteSpace(dbPass))
            {
                sqlConnStringBuilder.Password = dbPass;
            }

            using (SqlConnection con = new SqlConnection(sqlConnStringBuilder.ConnectionString))
            {
                con.Open();

                SqlBulkCopy bc = new SqlBulkCopy(con);

                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("TARGET_MONTH");
                dt.Columns.Add("ID_NUMBER");
                dt.Columns.Add("ID_TYPE");
                dt.Columns.Add("FLAG_OSN");
                dt.Columns.Add("FLAG_KOR");

                foreach (AspAskingModel ask in aspAskingModels.Where(x => x.IdNumber != null && x.IdNumber.Length <= 10))
                {
                    if (ask.FlagOsn == "Y" && ask.FlagKor == "Y")
                    {
                        // Записваме два реда YN и NY
                        dt.Rows.Add(null, targetMonth, ask.IdNumber, ask.IdType, "Y", "N");
                        dt.Rows.Add(null, targetMonth, ask.IdNumber, ask.IdType, "N", "Y");
                    }
                    else
                    {
                        dt.Rows.Add(null, targetMonth, ask.IdNumber, ask.IdType, ask.FlagOsn, ask.FlagKor);
                    }
                }

                bc.BulkCopyTimeout = 900;
                bc.BatchSize = 1000;
                bc.DestinationTableName = "[student].[AspAskingTemp]";
                bc.WriteToServer(dt);

                con.Close();
            }
        }

        /// <summary>
        /// Обработка и запис на Записни/Оптисани за дадено запитване на АСП.
        /// </summary>
        /// <param name="asp2MonSessionInfoId"></param>
        /// <param name="targetMonth"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private async Task<int> ProcessAsp2MonZps(int asp2MonSessionInfoId, DateTime targetMonth)
        {
            // Използването на linked server има особености.
            // Записът в linked server е много бахва опреация.
            // Опитваме се да я избегнем, като се ограничаваме само до четене.
            // С тази цел от linked server-а прочитаме децата, за които пита АСП за даден месец
            // и ги записваме в [student].[AspAskingTemp] в прод база.
            // След това извикваме sp в АСП базата, която от своя страна първо вика sp от прод базата за обработка на данните,
            // след което ги прочита и записва в базата на АСП.
            // Всички това е наоправено с цел намаляване на времето за обработка на запитване за стотици хиляди деца.


            _ = await _context.Database.ExecuteSqlRawAsync("EXEC [student].[sp_TruncateAspAskingTemp]");

            _ = await _context.Database.ExecuteSqlRawAsync("EXEC [student].[sp_TruncateAspZpTemp]");

            await CopyAspAsking(targetMonth);

            int take = 10000;
            int count = await _context.AspAskingTemps.CountAsync(x => x.TargetMonth == targetMonth);
            var steps = count / take;

            var targetMonthParam = new SqlParameter("TargetMonth", SqlDbType.DateTime2)
            {
                Value = targetMonth
            };

            var takeParam = new SqlParameter("Take", SqlDbType.Int)
            {
                Value = take
            };

            _context.Database.SetCommandTimeout(600);

            for (int i = 0; i < steps + 1; i++)
            {
                var skipParam = new SqlParameter("Skip", SqlDbType.Int)
                {
                    Value = i * take
                };

                var sqlParameters = new[]
                {
                    targetMonthParam,
                    skipParam,
                    takeParam
                };

                // Обработва прехвърлените в student.AspAskingTemp запитвания от базата на АСП
                _ = await _context.Database.ExecuteSqlRawAsync("EXEC [student].[sp_AspAskingZpProcessor_3] @TargetMonth, @Skip, @Take", sqlParameters);
            }

            var sessionNoParam = new SqlParameter("AspSessionInfoNo", SqlDbType.Int)
            {
                Value = asp2MonSessionInfoId
            };

            SqlParameter countParam = new SqlParameter
            {
                ParameterName = "Count",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output,
            };

            SqlParameter errorParam = new SqlParameter
            {
                ParameterName = "Error",
                DbType = DbType.String,
                Direction = ParameterDirection.Output,
                Size = -1
            };

            var sqlParameters1 = new[]
            {
                sessionNoParam,
                countParam,
                errorParam
            };

            _aspContext.Database.SetCommandTimeout(900);
            _ = await _aspContext.Database.ExecuteSqlRawAsync("EXEC [dbo].[sp_AspZpProcessor_Without_AspAskingTemp_Setup] @AspSessionInfoNo, @Count OUTPUT, @Error OUTPUT", sqlParameters1);

            string spErrors = errorParam.Value.ConvertFromDBVal<string>();
            if (!spErrors.IsNullOrWhiteSpace())
            {
                throw new ApiException(spErrors);
            }

            int savedRowsCount = countParam.Value.ConvertFromDBVal<int>();

            return savedRowsCount;
        }

        #endregion

        public async Task CopyAspAsking(short schoolYear, short month)
        {
            DateTime targetMonth = Common.GetYearFromSchoolYear(schoolYear, month);
            await CopyAspAsking(targetMonth);
        }

        public async Task ExportAbsencesToFileAsync(short schoolYear, short month)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAbsencesExportManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DateTime targetMonth = Common.GetYearFromSchoolYear(schoolYear, month);
            int asp2MonSessionInfoId = await _aspContext.Asp2monSessionInfos.Where(x => x.TargetMonth == targetMonth)
                .Select(x => x.SessionNo)
                .FirstOrDefaultAsync();

            int savedAbsCount = await ProcessAsp2MonAbsences(asp2MonSessionInfoId);
            int savedZpCount = await ProcessAsp2MonZps(asp2MonSessionInfoId, targetMonth);

            // След интеграцията с АСП не е нужно да пазим експортираните отсъствия във файл.
            // Пазят се в базата MON-ASPAbsence.

            AbsenceExport absenceExport = await _context.AbsenceExports
                .SingleOrDefaultAsync(x => x.SchoolYear == schoolYear && x.Month == month);
            if (absenceExport == null)
            {
                // Не съществува импорт за дадената институция, учебна година и месец.
                absenceExport = new AbsenceExport
                {
                    SchoolYear = schoolYear,
                    Month = month,
                };
                _context.AbsenceExports.Add(absenceExport);
            }

            absenceExport.RecordsCount = savedAbsCount;
            absenceExport.ZpCount = savedZpCount;
            absenceExport.BlobId = null;
            absenceExport.IsSigned = false;
            absenceExport.SignedDate = null;
            absenceExport.Signature = null;
            absenceExport.IsFinalized = false;
            await SaveAsync();

            var asp2monSessionInfo = await _aspContext.Asp2monSessionInfos
                .FirstOrDefaultAsync(x => x.TargetMonth == targetMonth);
            if (asp2monSessionInfo != null)
            {
                asp2monSessionInfo.MonProcessed = Now;
                await _aspContext.SaveChangesAsync();
            }
        }

        public async Task<ClassAbsenceModel> GetAbsencesForClassAsync(int classId, short schoolYear, short month)
        {
            if (!await _authorizationService.HasPermissionForClass(classId, DefaultPermissions.PermissionNameForClassAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            ClassAbsenceModel model = await _context.ClassGroups
                .AsNoTracking()
                .Where(x => x.ClassId == classId && x.SchoolYear == schoolYear)
                .Select(x => new ClassAbsenceModel
                {
                    ClassId = x.ClassId,
                    Name = x.ClassName,
                    Month = month,
                    SchoolYear = x.SchoolYear,
                    IsValid = x.IsValid ?? true// В базата има default(1)
                })
                .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException($"Invalid class with Id: {classId} and SchoolYear: {schoolYear}"));
            }

            FormattableString queryString = $"select StudentClassId, PersonId, FullName, ClassId, StudentClassIsCurrent, ClassName, ClassNumber, SchoolYear, InstitutionId, AbsenceId, [Month], Excused, Unexcused, LodIsFinalized from [student].[fn_StudentClassAbsences]({classId}, {month})";
            IQueryable<StudentClassAbsencesDto> query = _context.Set<StudentClassAbsencesDto>()
                .FromSqlInterpolated(queryString);

            model.StudentAbsences = await query
                .AsNoTracking()
                .Where(x => x.StudentClassIsCurrent)
                .OrderBy(x => x.ClassNumber).ThenBy(x => x.FullName)
                .Select(x => new StudentAbsenceModel
                {
                    Id = x.AbsenceId,
                    PersonId = x.PersonId,
                    Name = x.FullName,
                    Excused = x.Excused ?? 0,
                    Unexcused = x.Unexcused ?? 0,
                    ClassNumber = x.ClassNumber,
                    ClassId = x.ClassId,
                    SchoolYear = x.SchoolYear,
                    Month = x.Month ?? month,
                    StudentClassIsCurrent = x.StudentClassIsCurrent,
                    IsLodFinalized = x.LodIsFinalized
                })
                .ToListAsync();

            return model;
        }

        public async Task Create(StudentAbsenceInputModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(StudentAbsenceInputModel)));
            }

            if (!await _authorizationService.HasPermissionForClass(model.ClassId, DefaultPermissions.PermissionNameForClassAbsenceManage)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.Excused < 0 || model.Unexcused < 0)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Невалидни стойности (по-малки от нула).")); ;
            }

            if (model.Excused == 0 && model.Unexcused == 0)
            {
                // Мъчим се да запишем две нули. В повечете случай не е нарочно.
                // Връщаме успешен запис, но не създаваме ред в таблицата.
                return;
            }

            if (await _context.Absences.AnyAsync(x => x.PersonId == model.PersonId
                && x.SchoolYear == model.SchoolYear && x.Month == model.Month && x.InstitutionId == _userInfo.InstitutionID))
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("За дадения период съществува запис. Нов не може да бъде създаден.")); ;
            }

            await CheckForActiveCampaign(model);

            Absence entry = new Absence
            {
                PersonId = model.PersonId,
                InstitutionId = _userInfo.InstitutionID ?? default,
                SchoolYear = model.SchoolYear,
                Month = model.Month,
                Excused = model.Excused,
                Unexcused = model.Unexcused,
                ClassId = model.ClassId,
                BasicClassId = await _context.ClassGroups.Where(x => x.ClassId == model.ClassId).Select(x => x.BasicClassId).SingleOrDefaultAsync()
            };

            _context.Absences.Add(entry);
            await SaveAsync();
        }

        public async Task Update(StudentAbsenceInputModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(StudentAbsenceInputModel)));
            }

            Absence entity = await _context.Absences.Where(a => a.Id == model.Id).SingleOrDefaultAsync();

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(entity), nameof(Absence)));
            }

            if (!await _authorizationService.HasPermissionForClass(model.ClassId, DefaultPermissions.PermissionNameForClassAbsenceManage)
                && !await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue || entity.InstitutionId != _userInfo.InstitutionID)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.Excused < 0 || model.Unexcused < 0)
            {
                throw new ApiException(Messages.InvalidOperation, new InvalidOperationException("Невалидни стойности (по-малки от нула).")); ;
            }

            await CheckForActiveCampaign(model);

            if (model.Excused == 0 && model.Unexcused == 0 && entity.AbsenceImportId == null)
            {
                // При нулеви извинени и неизвинени отсъствия и липса на обвързан AbsenceImport записът се трие
                _context.Absences.Remove(entity);
                await SaveAsync();

                return;
            }

            if (model.Excused != entity.Excused || model.Unexcused != entity.Unexcused)
            {
                entity.AbsenceHistories.Add(new AbsenceHistory()
                {
                    Excused = entity.Excused,
                    Unexcused = entity.Unexcused
                });

                entity.Excused = model.Excused;
                entity.Unexcused = model.Unexcused;

                await SaveAsync();
            }
        }

        public async Task<AbsenceFileMonthAndYearModel> ReadAbsenceFile(IFormFile file)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead)
                && !!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<AbsenceImportModel> studentsAbsences = await ReadAbsencesFileAsync(file) ?? new List<AbsenceImportModel>();

            ApiValidationResult validationResult = await _validator.ValidateAbsencesImport(studentsAbsences, AbsenceImportTypeEnum.File);

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            AbsenceImportModel first = studentsAbsences.FirstOrDefault() ?? throw new InvalidOperationException(Messages.EmptyFileError);
            short schoolYear = (short)first.SchoolYear;
            int month = first.Month;
            string institutionName = await _context.InstitutionSchoolYears
                .AsNoTracking()
                .Where(x => x.InstitutionId == first.InstitutionCode && x.SchoolYear == schoolYear)
                .Select(x => x.Name)
                .SingleOrDefaultAsync();

            return new AbsenceFileMonthAndYearModel() { SchoolYear = schoolYear, Month = month, InstitutionName = institutionName };
        }

        /// <summary>
        /// Импорт на данни за отсъствия
        /// </summary>
        /// <param name="studentsAbsences"></param>
        /// <param name="institutionId">институция (учебно заведение)</param>
        /// <param name="schoolYear">Учебна година</param>
        /// <param name="month">Месец</param>
        /// <returns></returns>
        public async Task ImportAbsencesAsync(IFormFile file)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            List<AbsenceImportModel> studentsAbsences = await ReadAbsencesFileAsync(file);

            ApiValidationResult validationResult = await _validator.ValidateAbsencesImport(studentsAbsences, AbsenceImportTypeEnum.File);

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            byte[] fileContent = file.ToByteArray();
            var blobDO = await _blobService.UploadFileAsync(fileContent, file?.FileName, file?.ContentType);

            AbsenceImportModel first = studentsAbsences.FirstOrDefault() ?? throw new InvalidOperationException(Messages.EmptyFileError);
            short schoolYear = (short)first.SchoolYear;
            short month = (short)first.Month;
            int institutionId = _userInfo?.InstitutionID ?? throw new InvalidOperationException(Messages.InvalidInstitutionCodeError);

            if (!await _context.InstitutionSchoolYears.AnyAsync(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear))
            {
                throw new ApiException(Messages.InvalidSchoolYear);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                AbsenceImport absenceImport = await _context.AbsenceImports
                    .SingleOrDefaultAsync(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.Month == month
                        && x.ImportType != null && x.ImportType == (byte)AbsenceImportTypeEnum.File);

                if (absenceImport == null)
                {
                    // Не съществува импорт за дадената институция, учебна година и месец.
                    absenceImport = new AbsenceImport
                    {
                        InstitutionId = institutionId,
                        SchoolYear = schoolYear,
                        Month = month,
                        ImportType = (byte)AbsenceImportTypeEnum.File
                    };
                    _context.AbsenceImports.Add(absenceImport);
                }
                else
                {
                    // Съществува импорт за дадената институция, учебна година и месец.
                    // Ще изтрием всички отсъствия обвързани със стария импорт, след което ще добавим новите от файла.

                    await _context.Absences.Where(x => x.AbsenceImportId != null && x.AbsenceImportId == absenceImport.Id)
                        .DeleteAsync();
                }

                absenceImport.BlobId = blobDO?.Data?.BlobId;
                absenceImport.IsSigned = false;
                absenceImport.SignedDate = null;
                absenceImport.Signature = null;
                absenceImport.IsFinalized = false;

                HashSet<int> personIds = studentsAbsences.Where(x => x.PersonId.HasValue).Select(x => x.PersonId.Value).ToHashSet();
                // Трием всички отсъствия за дадени учебна година, месец, институтция и ученици, които нямат AbsenceImportId
                await _context.Absences.Where(x => x.SchoolYear == schoolYear && x.Month == month
                    && x.InstitutionId == institutionId && x.AbsenceImportId == null
                    && personIds.Contains(x.PersonId))
                    .DeleteAsync();

                int count = 0;
                foreach (var studentAbsence in studentsAbsences.Where(x => x.PersonId.HasValue))
                {
                    count++;
                    absenceImport.Absences.Add(new Absence
                    {
                        PersonId = studentAbsence.PersonId.Value,
                        SchoolYear = schoolYear,
                        Month = month,
                        Excused = studentAbsence.MonthlyAbsencesForValidReason,
                        Unexcused = studentAbsence.MonthlyAbsencesForUnvalidReason,
                        InstitutionId = institutionId,
                        BasicClassId = studentAbsence.Class
                    });
                }

                absenceImport.RecordsCount = count;

                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message);
            }
        }

        /// <summary>
        /// Импорт на отсъствия от дневниците
        /// </summary>
        /// <param name="institutionId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task ImportAbsencesFromSchoolBooksAsync(short schoolYear, short month)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int institutionId = _userInfo?.InstitutionID ?? throw new InvalidOperationException(Messages.InvalidInstitutionCodeError);

            if (!await _context.InstitutionSchoolYears.AnyAsync(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear))
            {
                throw new ApiException(Messages.InvalidSchoolYear);
            }

            List<AbsenceImportModel> studentsAbsences = await _context.VSchoolBooksAbsencesGroups
                .Where(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.Month == month)
                .Select(x => new AbsenceImportModel
                {
                    InstitutionCode = x.InstitutionId,
                    SchoolYear = x.SchoolYear,
                    Month = x.Month ?? default,
                    StudentIdentificationType = x.PersonalIdtype ?? default,
                    Identification = x.PersonalId,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    Class = x.BasicClassId ?? default,
                    MonthlyAbsencesForValidReason = x.Excused ?? default,
                    MonthlyAbsencesForUnvalidReason = x.Unexcused ?? default,
                    PersonId = x.PersonId

                })
                .ToListAsync();

            ApiValidationResult validationResult = await _validator.ValidateAbsencesImport(studentsAbsences, AbsenceImportTypeEnum.SchoolBook, schoolYear, month);

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            byte[] fileContent = GetBytes(studentsAbsences);
            var blobDO = await _blobService.UploadFileAsync(fileContent, GetFileName(institutionId, schoolYear, month, AbsenceImportTypeEnum.SchoolBook), "text/plain");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                AbsenceImport absenceImport = await _context.AbsenceImports
                    .SingleOrDefaultAsync(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.Month == month
                        && x.ImportType != null && x.ImportType == (byte)AbsenceImportTypeEnum.SchoolBook);

                if (absenceImport == null)
                {
                    // Не съществува импорт за дадената институция, учебна година и месец.
                    absenceImport = new AbsenceImport
                    {
                        InstitutionId = institutionId,
                        SchoolYear = schoolYear,
                        Month = month,
                        ImportType = (byte)AbsenceImportTypeEnum.SchoolBook
                    };
                    _context.AbsenceImports.Add(absenceImport);
                }
                else
                {
                    // Съществува импорт за дадената институция, учебна година и месец.
                    // Ще изтрием всички отсъствия обвързани със стария импорт, след което ще добавим новите от файла.

                    await _context.Absences.Where(x => x.AbsenceImportId != null && x.AbsenceImportId == absenceImport.Id)
                        .DeleteAsync();
                }

                absenceImport.BlobId = blobDO?.Data?.BlobId;
                absenceImport.IsSigned = false;
                absenceImport.SignedDate = null;
                absenceImport.Signature = null;
                absenceImport.IsFinalized = false;

                HashSet<int> personIds = studentsAbsences.Where(x => x.PersonId.HasValue).Select(x => x.PersonId.Value).ToHashSet();
                // Трием всички отсъствия за дадени учебна година, месец, институтция и ученици, които нямат AbsenceImportId
                await _context.Absences.Where(x => x.SchoolYear == schoolYear && x.Month == month
                    && x.InstitutionId == institutionId && x.AbsenceImportId == null
                    && personIds.Contains(x.PersonId))
                    .DeleteAsync();

                int count = 0;
                foreach (var studentAbsence in studentsAbsences.Where(x => x.PersonId.HasValue))
                {
                    count++;
                    absenceImport.Absences.Add(new Absence
                    {
                        PersonId = studentAbsence.PersonId.Value,
                        SchoolYear = schoolYear,
                        Month = month,
                        Excused = studentAbsence.MonthlyAbsencesForValidReason,
                        Unexcused = studentAbsence.MonthlyAbsencesForUnvalidReason,
                        InstitutionId = institutionId,
                        BasicClassId = studentAbsence.Class
                    });
                }

                absenceImport.RecordsCount = count;

                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message);
            }
        }

        public async Task ImportAbsencesFromManualEntryAsync(short schoolYear, short month)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int institutionId = _userInfo?.InstitutionID ?? throw new ApiException(Messages.InvalidInstitutionCodeError);

            if (!await _context.InstitutionSchoolYears.AnyAsync(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear))
            {
                throw new ApiException(Messages.InvalidSchoolYear);
            }

            List<AbsenceImportModel> studentsAbsences = await GetManualImportSampleData(schoolYear, month);

            ApiValidationResult validationResult = await _validator.ValidateAbsencesImport(studentsAbsences, AbsenceImportTypeEnum.Manual);

            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 400, validationResult.Errors);
            }

            byte[] fileContent = GetBytes(studentsAbsences);
            var blobDO = await _blobService.UploadFileAsync(fileContent, GetFileName(institutionId, schoolYear, month, AbsenceImportTypeEnum.Manual), "text/plain");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                AbsenceImport absenceImport = await _context.AbsenceImports
                    .SingleOrDefaultAsync(x => x.InstitutionId == institutionId && x.SchoolYear == schoolYear && x.Month == month
                        && x.ImportType != null && x.ImportType == (byte)AbsenceImportTypeEnum.Manual);

                if (absenceImport == null)
                {
                    // Не съществува импорт за дадената институция, учебна година и месец.
                    absenceImport = new AbsenceImport
                    {
                        InstitutionId = institutionId,
                        SchoolYear = schoolYear,
                        Month = month,
                        ImportType = (byte)AbsenceImportTypeEnum.Manual
                    };

                    _context.AbsenceImports.Add(absenceImport);
                }

                absenceImport.RecordsCount = studentsAbsences.Count;
                absenceImport.BlobId = blobDO?.Data?.BlobId;
                absenceImport.IsSigned = false;
                absenceImport.SignedDate = null;
                absenceImport.Signature = null;
                absenceImport.IsFinalized = false;

                await SaveAsync();

                HashSet<int> idsHash = studentsAbsences.Where(x => x.StudentAbsenceId != null)
                    .Select(x => x.StudentAbsenceId.Value)
                    .ToHashSet();

                await _context.Absences
                    .Where(x => idsHash.Contains(x.Id))
                    .UpdateAsync(x => new Absence { AbsenceImportId = absenceImport.Id });

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message);
            }
        }

        public async Task<int> CreateNoAbsencesImport(NoAbsencesImportModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!model.SchoolYear.HasValue)
            {
                throw new ApiException(Messages.InvalidOperation, new ArgumentNullException(nameof(model.SchoolYear)));
            }

            if (!model.Month.HasValue)
            {
                throw new ApiException(Messages.InvalidOperation, new ArgumentNullException(nameof(model.Month)));
            }

            int institutionId = _userInfo?.InstitutionID ?? throw new ApiException(Messages.InvalidInstitutionCodeError);
            if (!await _context.InstitutionSchoolYears.AnyAsync(x => x.InstitutionId == institutionId && x.SchoolYear == model.SchoolYear.Value))
            {
                throw new ApiException(Messages.InvalidSchoolYear);
            }

            string absenceImportTypeStr = await _administrationService.GetTenantAppSetting("AbsenceImportType");
            byte.TryParse(absenceImportTypeStr, out byte absenceImportType);
            if (absenceImportType == default)
            {
                absenceImportType = (byte)AbsenceImportTypeEnum.Manual;
            }

            AbsenceImport absenceImport = await _context.AbsenceImports
                    .SingleOrDefaultAsync(x => x.InstitutionId == institutionId && x.SchoolYear == model.SchoolYear && x.Month == model.Month
                        && x.ImportType != null && x.ImportType == absenceImportType);
            if (absenceImport == null)
            {
                // Не съществува импорт за дадената институция, учебна година и месец.
                absenceImport = new AbsenceImport
                {
                    InstitutionId = institutionId,
                    SchoolYear = model.SchoolYear.Value,
                    Month = model.Month.Value,
                    ImportType = absenceImportType
                };
                _context.AbsenceImports.Add(absenceImport);
            }

            absenceImport.IsWithoutAbsences = true;
            absenceImport.RecordsCount = 0;
            absenceImport.BlobId = null;
            absenceImport.IsSigned = false;
            absenceImport.SignedDate = null;
            absenceImport.Signature = null;
            absenceImport.IsFinalized = false;

            await SaveAsync();

            return absenceImport.Id;
        }

        public async Task<IPagedList<StudentAbsenceViewModel>> GetStudentAbsences(StudentListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(StudentListInput)));
            }

            if (!await _authorizationService.HasPermissionForStudent(input?.StudentId ?? default, DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<StudentAbsenceViewModel> query = _context.VStudentAbsencesLists
                .Where(x => x.PersonId == input.StudentId)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.SchoolYearName.Contains(input.Filter)
                   || predicate.MonthName.Contains(input.Filter))
                .Select(x => new StudentAbsenceViewModel()
                {
                    Id = x.Id,
                    SchoolYear = x.SchoolYear,
                    Month = x.Month,
                    MonthName = x.MonthName,
                    SchoolYearName = x.SchoolYearName,
                    Excused = x.Excused,
                    Unexcused = x.Unexcused,
                    IsLodFinalized = x.LodIsFinalized
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, Month desc" : input.SortBy);

            int totalCount = await query.CountAsync();
            List<StudentAbsenceViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<AbsenceImportFileModel>> ListImportedFiles(InstitutionListInput input)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(InstitutionListInput)));

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!input.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DateTime now = Now;
            var activeCampaigns = await _context.AbsenceCampaigns
                .Where(x => x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1)))
                .Select(x => new { x.SchoolYear, x.Month })
                .ToListAsync();

            IQueryable<AbsenceImportFileModel> query = _context.AbsenceImports
                .AsNoTracking()
                .Where(x => x.InstitutionId == input.InstitutionId)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.InstitutionSchoolYear.SchoolYearNavigation.Name.Contains(input.Filter)
                   || predicate.MonthNavigation.Name.Contains(input.Filter))
                .Select(x => new AbsenceImportFileModel
                {
                    Id = x.Id,
                    TimestampUtc = x.ModifyDate ?? x.CreateDate,
                    CreateDate = x.CreateDate,
                    Month = x.Month,
                    MonthName = x.MonthNavigation.Name,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    RecordsCount = x.RecordsCount,
                    BlobId = x.BlobId,
                    IsFinalized = x.IsFinalized,
                    IsSigned = x.IsSigned,
                    FinalizedDate = x.FinalizedDate,
                    SignedDate = x.SignedDate,
                    ImportTypeId = x.ImportType,
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await query.CountAsync();
            List<AbsenceImportFileModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();
            foreach (var item in items)
            {
                if (item.BlobId.HasValue)
                {
                    DocumentExtensions.CalcHmac(item, _blobServiceConfig);
                }

                item.HasActiveCampaign = activeCampaigns.Any(x => x.SchoolYear == item.SchoolYear && x.Month == item.Month);
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<AbsenceExportFileModel>> ListExportedFiles(PagedListInput input)
        {
            if (input == null)
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(PagedListInput)));

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAbsencesExportRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<AbsenceExportFileModel> query = _context.AbsenceExports
                .AsNoTracking()
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.SchoolYearNavigation.Name.Contains(input.Filter))
                .Select(x => new AbsenceExportFileModel
                {
                    Id = x.Id,
                    TimestampUtc = x.ModifyDate ?? x.CreateDate,
                    CreateDate = x.CreateDate,
                    Month = x.Month,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    RecordsCount = x.RecordsCount,
                    ZpCount = x.ZpCount,
                    OresCount = x.OresCount,
                    BlobId = x.BlobId,
                    IsFinalized = x.IsFinalized,
                    IsSigned = x.IsSigned,
                    FinalizedDate = x.FinalizedDate,
                    SignedDate = x.SignedDate
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await query.CountAsync();
            List<AbsenceExportFileModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();
            foreach (var item in items)
            {
                if (item.BlobId.HasValue)
                {
                    DocumentExtensions.CalcHmac(item, _blobServiceConfig);
                }
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<AbsenceImportDetailsModel> GetImportDetails(int absenceImportId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            AbsenceImportDetailsModel model = await _context.AbsenceImports
                .Where(x => x.Id == absenceImportId)
                .Select(x => new AbsenceImportDetailsModel
                {
                    Id = x.Id,
                    Month = x.Month,
                    MonthName = x.MonthNavigation.Name,
                    SchoolYear = x.SchoolYear,
                    TimestampUtc = x.ModifyDate ?? x.CreateDate,
                    CreateDate = x.CreateDate,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    RecordsCount = x.RecordsCount,
                    BlobId = x.BlobId,
                    IsFinalized = x.IsFinalized,
                    IsSigned = x.IsSigned,
                    FinalizedDate = x.FinalizedDate,
                    SignedDate = x.SignedDate,
                    ImportTypeId = x.ImportType,
                    CreatorUsername = x.CreatedBySysUser.Username,
                    SignerUsername = x.IsSigned == true ? x.ModifiedBySysUser.Username : ""
                })
                .SingleOrDefaultAsync();

            DocumentExtensions.CalcHmac(model, _blobServiceConfig);

            if (model == null) return model;

            if (model.BlobId.HasValue)
            {
                byte[] importedFile = await _blobService.DownloadByteArrayAsync(model);
                var enc1251 = Encoding.GetEncoding("windows-1251");
                List<AbsenceImportModel> studentsAbsences = ParseAbsenceImportModels(await importedFile.ReadAsListAsync(enc1251));
                model.Records = studentsAbsences;

                if (model.Records.Any())
                {
                    int schoolYear = model.Records[0].SchoolYear;
                    string schoolYearName = await _context.SchoolYears
                        .FromSqlRaw("select CurrentYearID, [Name] from inst_basic.CurrentYear")
                        .AsNoTracking()
                        .Where(x => x.CurrentYearID == schoolYear)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();

                    foreach (var item in model.Records)
                    {
                        item.SchoolYearName = schoolYearName;
                    }
                }

            }

            return model;
        }

        public async Task<IPagedList<InstitutionImportedAbsencesOutputModel>> GetInstitutionsWithImportedData(InstitutionsAbsencesListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (input == null) throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(InstitutionsAbsencesListInput)));

            IQueryable<AbsenceImport> query = GetAbsenceImportsListQuery().AsNoTracking();

            if (input.SchoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.SchoolYear);
            }

            if (input.Month.HasValue)
            {
                query = query.Where(x => x.Month == input.Month);
            }

            IQueryable<InstitutionImportedAbsencesOutputModel> resulQuery = query
                .Select(x => new InstitutionImportedAbsencesOutputModel
                {
                    InstitutionName = x.InstitutionSchoolYear.Name,
                    SchoolYear = x.SchoolYear,
                    Month = x.Month,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    AbsenceId = x.Id
                })
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.InstitutionName.Contains(input.Filter)
                    || predicate.SchoolYearName.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, InstitutionName asc, Month desc" : input.SortBy);

            int totalCount = await resulQuery.CountAsync();
            IList<InstitutionImportedAbsencesOutputModel> items = await resulQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<IPagedList<InstitutionImportedAbsencesOutputModel>> GetInstitutionsWithoutImportedData(InstitutionsAbsencesListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (input == null) throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(InstitutionsAbsencesListInput)));

            if (!input.SchoolYear.HasValue)
            {
                // Трябва ни стойност за да вземем записите от core.InstitutionSchoolYear за дадена учебна година
                return null;
            }

            IQueryable<InstitutionSchoolYear> query = GetInstitutionsListQuery(input.SchoolYear.Value).AsNoTracking();

            if (input.SchoolYear.HasValue && input.Month.HasValue)
            {
                query = query.Where(x => !x.AbsenceImports.Any(x => x.SchoolYear == input.SchoolYear && x.Month == input.Month));
            }
            else if (input.SchoolYear.HasValue && !input.Month.HasValue)
            {
                query = query.Where(x => !x.AbsenceImports.Any(x => x.SchoolYear == input.SchoolYear));
            }
            else if (input.Month.HasValue && !input.SchoolYear.HasValue)
            {
                query = query.Where(x => !x.AbsenceImports.Any(x => x.Month == input.Month));
            }
            else if (!input.SchoolYear.HasValue && !input.Month.HasValue)
            {
                query = query.Where(x => !x.AbsenceImports.Any());
            }

            IQueryable<InstitutionImportedAbsencesOutputModel> resulQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                        predicate => predicate.Name.Contains(input.Filter))
                .OrderBy("Name")
                .Select(x => new InstitutionImportedAbsencesOutputModel
                {
                    InstitutionName = x.Name
                });

            int totalCount = await resulQuery.CountAsync();
            IList<InstitutionImportedAbsencesOutputModel> items = await resulQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<List<StudentAbsenceHistoryOutputModel>> GetStudentAbsencesHistoryAsync(int absenceId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await _context.AbsenceHistories
                .Where(x => x.AbsenceId == absenceId)
                .Select(x => new StudentAbsenceHistoryOutputModel
                {
                    Excused = x.Excused,
                    Unexcused = x.Unexcused,
                    CreateDate = x.CreateDate,
                    Username = x.CreatedBySysUser.Username
                })
                .ToListAsync();
        }

        public async Task DeleteAbsenceImport(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            AbsenceImport absenceImport = await _context.AbsenceImports
                .SingleOrDefaultAsync(x => x.Id == id);

            if (!absenceImport.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (absenceImport.IsFinalized)
            {
                throw new ApiException(Messages.IsFinalizedError, 400);
            }

            DateTime now = Now;
            bool hasActiveCapaign = await _context.AbsenceCampaigns.AnyAsync(x => x.SchoolYear == absenceImport.SchoolYear && x.Month == absenceImport.Month
                && (x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1)))
            );
            if (!hasActiveCapaign)
            {
                throw new ApiException($"За учебна година: {absenceImport.SchoolYear} и месец: {absenceImport.Month} липсва активна кампания за подаване(импортиране) на отсъствия.", 400);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // При речен импорт не трябва да трием отсъствията.
                // Ще затрием въведените данни до този момент и трябва да се въвеждат отново.
                if (absenceImport.ImportType == (int)AbsenceImportTypeEnum.Manual)
                {
                    await _context.Absences.Where(x => x.AbsenceImportId != null && x.AbsenceImportId == absenceImport.Id)
                            .UpdateAsync(x => new Absence { AbsenceImportId = null });
                }
                else
                {
                    await _context.Absences.Where(x => x.AbsenceImportId != null && x.AbsenceImportId == absenceImport.Id)
                            .DeleteAsync();
                }

                _context.AbsenceImports.Remove(absenceImport);
                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message);
            }
        }

        public async Task<string> ConstructAbsenceImportAsXml(AbsenceImportDetailsModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            byte[] importedFile = model.BlobId.HasValue
                ? await _blobService.DownloadByteArrayAsync(model)
                : null;

            var importDetails = await _context.AbsenceImports
                .Where(x => x.Id == model.Id)
                .Select(x => new
                {
                    x.Id,
                    x.InstitutionId,
                    x.SchoolYear,
                    x.Month,
                    x.BlobId,
                    x.RecordsCount,
                    x.CreatedBySysUserId,
                    x.CreateDate,
                    x.ModifiedBySysUserId,
                    x.ModifyDate,
                    x.ImportType,
                })
                .SingleOrDefaultAsync();

            SignedAbsenceImport signedModel = new SignedAbsenceImport
            {
                AbsenceImportId = model.Id,
                Version = 1,
                Contents = JsonSerializer.Serialize(importDetails)
            };

            using var sha512 = new SHA512Managed();

            signedModel.BlobHash = importedFile != null
                ? Convert.ToBase64String(sha512.ComputeHash(importedFile))
                : "";

            return signedModel.ToXml();
        }

        public async Task<string> ConstructAbsenceExportAsXml(AbsenceExportFileModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            byte[] exportedFile = model.BlobId.HasValue
                ? await _blobService.DownloadByteArrayAsync(model)
                : null;

            var exportDetails = await _context.AbsenceExports
                .Where(x => x.Id == model.Id)
                .Select(x => new
                {
                    x.Id,
                    x.SchoolYear,
                    x.Month,
                    x.BlobId,
                    x.RecordsCount,
                    x.CreatedBySysUserId,
                    x.CreateDate,
                    x.ModifiedBySysUserId,
                    x.ModifyDate,
                })
                .SingleOrDefaultAsync();

            SignedAbsenceExport signedModel = new SignedAbsenceExport
            {
                AbsenceExportId = model.Id,
                Version = 1,
                Contents = JsonSerializer.Serialize(exportDetails)
            };

            using var sha512 = new SHA512Managed();

            signedModel.BlobHash = exportedFile != null
                ? Convert.ToBase64String(sha512.ComputeHash(exportedFile))
                : "";

            return signedModel.ToXml();
        }

        public async Task<string> ConstructNoAbsencesImportAsXml(int? absenceImportId)
        {
            var importDetails = await _context.AbsenceImports
                .Where(x => x.Id == absenceImportId)
                .Select(x => new
                {
                    x.Id,
                    x.InstitutionId,
                    x.SchoolYear,
                    x.Month,
                    IsWithoutAbsences = true,
                    x.BlobId,
                    x.RecordsCount,
                    x.CreatedBySysUserId,
                    x.CreateDate,
                    x.ModifiedBySysUserId,
                    x.ModifyDate,
                    x.ImportType,
                })
                .SingleOrDefaultAsync();

            SignedAbsenceImport signedModel = new SignedAbsenceImport
            {
                AbsenceImportId = null,
                Version = 1,
                Contents = JsonSerializer.Serialize(importDetails)
            };
            return signedModel.ToXml();
        }

        public async Task SetAbsenceImportSigningAtrributes(AbsenceImportSigningAtrributesSetModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentAbsenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            AbsenceImport absenceImport = await _context.AbsenceImports
                .SingleOrDefaultAsync(x => x.Id == model.AbsenceImportId);

            if (absenceImport == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(absenceImport), nameof(AbsenceImport)));
            }

            if (!absenceImport.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (absenceImport.IsSigned)
            {
                throw new ApiException(Messages.IsSignedError, 400);
            }

            if (absenceImport.IsFinalized)
            {
                throw new ApiException(Messages.IsFinalizedError, 400);
            }

            string signature = model.Signature;

            try
            {
                byte[] data = Convert.FromBase64String(model.Signature);
                signature = Encoding.UTF8.GetString(data);
            }
            catch
            {
                // Не е base64 Или не сме успели да декодираме. Записваме каквото е дошло
                signature = model.Signature;
            }
            absenceImport.IsSigned = true;
            absenceImport.SignedDate = DateTime.UtcNow;
            absenceImport.Signature = signature;

            try
            {
                var certificateResult = await _certificateService.VerifyXml(absenceImport.Signature);
                if (!certificateResult.IsValid)
                {
                    _logger.LogWarning($"Грешка при подписване: {JsonSerializer.Serialize(certificateResult)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Грешка при подписване");
            }

            await SaveAsync();
        }

        public async Task SetAbsenceExportSigningAtrributes(AbsenceExportSigningAtrributesSetModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAbsencesExportManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            AbsenceExport absenceExport = await _context.AbsenceExports
                .SingleOrDefaultAsync(x => x.Id == model.AbsenceExportId);

            if (absenceExport == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(absenceExport), nameof(AbsenceExport)));
            }

            if (absenceExport.IsSigned)
            {
                throw new ApiException(Messages.IsSignedError, 400);
            }

            if (absenceExport.IsFinalized)
            {
                throw new ApiException(Messages.IsFinalizedError, 400);
            }

            string signature = model.Signature;

            try
            {
                byte[] data = Convert.FromBase64String(model.Signature);
                signature = Encoding.UTF8.GetString(data);
            }
            catch
            {
                // Не е base64 Или не сме успели да декодираме. Записваме каквото е дошло
                signature = model.Signature;
            }
            absenceExport.IsSigned = true;
            absenceExport.SignedDate = DateTime.UtcNow;
            absenceExport.Signature = signature;

            await SaveAsync();
        }

    }
}
