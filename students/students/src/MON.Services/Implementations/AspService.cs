namespace MON.Services.Implementations
{
    using DocumentFormat.OpenXml.Bibliography;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MON.ASPDataAccess;
    using MON.BackgroundWorker;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.ASP;
    using MON.Models.Blob;
    using MON.Models.Configuration;
    using MON.Models.Enums;
    using MON.Models.Grid;
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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class AspService : BaseService<AspService>, IAspService
    {
        private readonly MONASPContext _aspContext;

        private readonly IBackgroundTaskQueue _queue;
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly AspValidator _aspValidator;
        private readonly IStudentService _studentService;
        private readonly ICertificateService _certificateService;
        private const string EXPORT_HEADER = "Year|Month|SchoolID|IDNumber|intIDType|Name 1|Name 2|Name 3|AbsenceCount|DaysCount|ASPStatus|MONStatus|AbsenceCorrection|DaysCorrection";

        private const int YEAR_INDEX = 0;
        private const int MONTH_INDEX = 1;
        private const int INSTITUTIONID_INDEX = 2;
        private const int IDTYPE_INDEX = 3;
        private const int ID_INDEX = 4;
        private const int FIRSTNAME_INDEX = 5;
        private const int MIDDLENAME_INDEX = 6;
        private const int LASTNAME_INDEX = 7;
        private const int ABSENCE_INDEX = 8;
        private const int ASPSTATUS_INDEX = 9;
        private const int DAYSCOUNT_INDEX = 10;

        private const string ENROLLED_STUDENTS_EXPORT_HEADER = "InstitutionCode|PersonalID|PersonalIdtype|FirstName|MiddleName|LastName|StudentEduFormId|BasicClassID|Status";

        public AspService(DbServiceDependencies<AspService> dependencies,
            MONASPContext aspContext,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IStudentService studentService,
            ISignalRNotificationService signalRNotificationService,
            AspValidator aspValidator, IBackgroundTaskQueue queue,
            ICertificateService certificateService,
            IServiceScopeFactory serviceScopeFactory)
            : base(dependencies)
        {
            _aspContext = aspContext;
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _aspValidator = aspValidator;
            _studentService = studentService;
            _queue = queue;
            _serviceScopeFactory = serviceScopeFactory;
            _certificateService = certificateService;
            _signalRNotificationService = signalRNotificationService;
        }


        #region Private members

        private void ValidateCampaign(AspmonthlyBenefitsImport campaign)
        {
            if (campaign == null)
            {
                throw new ApiException("Липсваща кампания за потвърждаване на данни от АСП.");
            }

            bool isActiveCampaign = campaign.FromDate.HasValue
                && (campaign.FromDate <= Now.Date) && ((Now.Date <= campaign.ToDate) || campaign.ToDate == null);
            if (!isActiveCampaign)
            {
                throw new ApiException("Кампанията за потвърждаване на данни от АСП не е активна.");
            }
        }

        private async Task ValidateBenefit(AspmonthlyBenefit entity)
        {
            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(AspmonthlyBenefit)));
            }

            if (entity.Monstatus == (int)NEISPUOStatus.Rejected)
            {
                if (entity.Aspstatus == (int)ASPStatusEnum.Absence)
                {
                    // При отказ бройката на коригираните отсъствия/ дни трябва да се различава от началната
                    decimal abs = entity.AbsenceCorrection ?? -1;
                    if (abs < 0 || abs == entity.AbsenceCount)
                    {
                        throw new ApiException("При отказ, коригирания брой трябва да е въведен и да се различава от оригиналния.");
                    }

                    // В случай, че е отказано потвърждение броят отсъствия трябва да е по-малко от 5 за ученик, 3 за дете в ДГ
                    int? basicClassId = (await _studentService.GetCurrentClass(entity.PersonId, entity.SchoolYear))?.BasicClassId;
                    if (basicClassId.HasValue && basicClassId.Value > 0 && (abs < 0 || abs >= 5))
                    {
                        throw new ApiException("Коригираният брой отсъствия не може да надвишава или да е равен на 5 учебни часа за ученик.");
                    }

                    if (basicClassId.HasValue && basicClassId.Value <= 0 && (abs < 0 || abs > 3))
                    {
                        throw new ApiException("Коригираният брой отсъствия не трябва да надвишава или да е равен на 3 дни за дете в ДГ/ПГ.");
                    }
                }

                if (entity.Aspstatus == (int)ASPStatusEnum.NonVisiting)
                {
                    // При отказ бройката на коригираните отсъствия/ дни трябва да се различава от началната
                    decimal days = entity.DaysCorrection ?? -1;
                    if (days < 0 || days == entity.DaysCount)
                    {
                        throw new ApiException("При отказ, коригирания брой трябва да е въведен и да се различава от оригиналния.");
                    }
                }
            }
        }

        private async Task<ASPMonthlyBenefitsImportResultModel> ReadBenefitsFileAsync(string[] fileLines, MONContext scopedContext, IStudentService scopedStudentService)
        {
            var monthlyBenefits = new List<ASPMonthlyBenefitsImportModel>();

            var sbErrors = new StringBuilder();

            for (var rowIndex = 0; rowIndex < fileLines.Length; rowIndex++)
            {
                var split = fileLines[rowIndex].Split("|");

                try
                {
                    var b = new ASPMonthlyBenefitsImportModel();

                    var studentPin = split[ID_INDEX].Trim();
                    var studentFirstName = split[FIRSTNAME_INDEX].Trim();
                    var studentMiddleName = split[MIDDLENAME_INDEX].Trim();
                    var studentLastName = split[LASTNAME_INDEX].Trim();
                    var month = split[MONTH_INDEX].Trim();
                    var idType = split[IDTYPE_INDEX].Trim();

                    var errorMessageLocation = $" на ред: {rowIndex} за ученик:{studentFirstName + " " + studentLastName} с идентификатор {studentPin}";

                    if (string.IsNullOrEmpty(studentPin))
                    {
                        sbErrors.AppendLine($"Невалиден идентификатор на ред: {rowIndex} за ученик:{studentFirstName + " " + studentLastName}");
                        continue;
                    }
                    else
                    {
                        b.Identification = studentPin;
                    }

                    var person = await scopedContext.People.FirstOrDefaultAsync(x => x.PersonalId == studentPin);

                    if (person == null)
                    {
                        sbErrors.AppendLine($"Не е намерен ученик с идентификатор:{studentPin} на ред: {rowIndex} за ученик:{studentFirstName + " " + studentLastName}");
                        continue;
                    }

                    if (string.IsNullOrEmpty(idType) || idType.Length != 1)
                    {
                        sbErrors.AppendLine("Невалиден тип идентификатор" + errorMessageLocation);
                    }
                    else
                    {
                        if (int.TryParse(idType, out int parsedIdType))
                        {
                            if (person.PersonalIdtype != parsedIdType)
                            {
                                sbErrors.AppendLine("Подадения тип идентификатор не съвпада с намерения в системата " + errorMessageLocation);
                            }
                            else
                            {
                                b.StudentIdentificationType = idType;
                                b.PersonId = person.PersonId;
                            }
                        }
                        else
                        {
                            sbErrors.AppendLine("Невалиден тип идентификатор" + errorMessageLocation);
                        }
                    }

                    if (!int.TryParse(split[YEAR_INDEX], out int parsedYear) || split[YEAR_INDEX].Length != 4)
                    {
                        sbErrors.Append($"Невалидна учебна година" + errorMessageLocation);
                    }
                    else
                    {
                        b.SchoolYear = parsedYear;
                    }

                    if (string.IsNullOrEmpty(month) || month.Length != 2)
                    {
                        sbErrors.AppendLine("Невалиден месец" + errorMessageLocation);
                    }
                    else
                    {
                        b.Month = month;
                        if (b.SchoolYear > 0)
                        {
                            b.SchoolYear = Common.GetYearFromSchoolYear(b.SchoolYear, int.Parse(b.Month)).Year;
                        }
                    }

                    if (string.IsNullOrEmpty(studentFirstName))
                    {
                        sbErrors.AppendLine($"Невалидно име на ред: {rowIndex} за идентификатор {studentPin}");
                    }
                    else
                    {
                        b.FirstName = studentFirstName;
                    }

                    b.MiddleName = studentMiddleName;

                    //if (string.IsNullOrEmpty(studentLastName))
                    //{
                    //    sbErrors.AppendLine($"Невалиднa фамилия на ред: {rowIndex} за идентификатор {studentPin}");
                    //}
                    //else
                    //{
                    b.LastName = studentLastName;
                    //}

                    if (!decimal.TryParse(split[ABSENCE_INDEX], NumberStyles.AllowDecimalPoint, new CultureInfo("en-us"), out decimal absences) || split[ABSENCE_INDEX].Length != 6)
                    {
                        sbErrors.AppendLine("Невалиден брой отсъствия" + errorMessageLocation);
                    }
                    else
                    {
                        b.AbsenceCount = absences;
                    }

                    if (!int.TryParse(split[ASPSTATUS_INDEX], out int aspStatus) || split[ASPSTATUS_INDEX].Length != 1)
                    {
                        sbErrors.AppendLine($"Невалиден АСП статус на ред: {rowIndex} за идентификатор {studentPin}");
                    }
                    else
                    {
                        b.ASPStatus = new DropdownViewModel { Value = aspStatus, Text = ((ASPStatusEnum)aspStatus).GetEnumDescriptionAttrValue() };
                    }

                    if (!short.TryParse(split[DAYSCOUNT_INDEX], out short daysCount) || split[DAYSCOUNT_INDEX].Length != 2)
                    {
                        sbErrors.AppendLine("Невалиден брой дни" + errorMessageLocation);
                    }
                    else
                    {
                        b.DaysCount = daysCount;
                    }

                    if (int.TryParse(split[INSTITUTIONID_INDEX], out int institutionId) && split[INSTITUTIONID_INDEX].Length == 7)
                    {
                        b.CurrentInstitutionId = await scopedStudentService.GetAttendedInstitution(person.PersonId, institutionId);
                        b.InstitutionId = institutionId;
                    }
                    else
                    {
                        sbErrors.AppendLine($"Невалидно въведена институция:{split[INSTITUTIONID_INDEX]}" + errorMessageLocation);
                    }

                    monthlyBenefits.Add(b);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("File data is corrupt.", ex);
                }
            }

            return new ASPMonthlyBenefitsImportResultModel { Benefits = monthlyBenefits, Errors = sbErrors.ToString() };
        }

        private async Task<bool> BenefitsImported(short schoolYear, short month)
        {
            return await _context.AspmonthlyBenefitsImports.AsNoTracking().AnyAsync(ai => ai.SchoolYear == schoolYear && ai.Month == month);
        }

        private async Task<List<ASPEnrolledStudentClassData>> GetEnrolledStudentsClassesData(int schoolYear, int fileType)
        {
            return await (from sc in _context.StudentClasses
                          join p in _context.People on sc.PersonId equals p.PersonId
                          join cg in _context.ClassGroups on sc.ClassId equals cg.ClassId
                          join v_ia in _context.InstitutionAlls on cg.InstitutionId equals v_ia.InstitutionId
                          join ct in _context.ClassTypes on sc.ClassTypeId equals ct.ClassTypeId
                          join ef in _context.EduForms on sc.StudentEduFormId equals ef.ClassEduFormId
                          where sc.SchoolYear == schoolYear
                          && (fileType == (int)AspEnrolledStudentsExportFileType.EnrolledStudents ? sc.Status == (int)StudentClassStatus.Enrolled
                               : (fileType != (int)AspEnrolledStudentsExportFileType.EnrolledStudentsCorrections || (sc.Status == (int)StudentClassStatus.Enrolled || sc.Status == (int)StudentClassStatus.Discharged)))
                          && sc.IsCurrent
                          && sc.ClassType.ClassKind == (int)ClassKindEnum.Basic
                          && p.PersonalIdtype < 2 //Във файла трябва да се само лица с валидни ЕГН или ЛНЧ
                          && ((sc.PositionId == (int)PositionType.Student && (v_ia.InstType == (int)InstitutionTypeEnum.School || v_ia.InstType == (int)InstitutionTypeEnum.KinderGarden))
                              || sc.PositionId == (int)PositionType.StudentOtherInstitution && v_ia.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport)
                          && ((sc.BasicClassId >= (int)BasicClassType.SecondGroup && sc.BasicClassId <= (int)BasicClassType.TwelfthGrade)
                              || sc.BasicClassId == (int)BasicClassType.ThirdGroup)
                          select new ASPEnrolledStudentClassData
                          {
                              PersonId = sc.PersonId,
                              InstitutionCode = cg.InstitutionId,
                              PersonalId = p.PersonalId,
                              PersonalIdType = p.PersonalIdtype,
                              FirstName = p.FirstName,
                              MiddleName = p.MiddleName,
                              LastName = p.LastName,
                              StudentEduFormId = sc.StudentEduFormId,
                              BasicClassId = sc.BasicClassId,
                              Status = sc.Status
                          }).ToListAsync();
        }

        private async Task<InstitutionCacheModel> GetStudentInstitutionByPin(string studentPin, int schoolYear, ILookup<int, InstitutionAll> institutionAll)
        {
            InstitutionCacheModel currentStudentInstitution = await (
                from sc in _context.StudentClasses
                where sc.Person.PersonalId == studentPin && sc.PositionId == (int)PositionType.Student && sc.IsCurrent && sc.ClassType.ClassKind == (int)ClassKindEnum.Basic && sc.SchoolYear == schoolYear
                select new InstitutionCacheModel
                {
                    Id = sc.InstitutionId,
                }).FirstOrDefaultAsync();

            if (currentStudentInstitution != null)
            {
                var institution = institutionAll[currentStudentInstitution.Id].FirstOrDefault(x => x.InstType == (int)InstitutionTypeEnum.School || x.InstType == (int)InstitutionTypeEnum.KinderGarden);
                if (institution != null)
                {
                    currentStudentInstitution.Name = institution.Name;
                }
            }
            else
            {
                if (currentStudentInstitution == null)
                {
                    currentStudentInstitution = await (
                        from sc in _context.StudentClasses
                        where sc.Person.PersonalId == studentPin && sc.PositionId == (int)PositionType.StudentOtherInstitution && sc.IsCurrent && sc.ClassType.ClassKind == (int)ClassKindEnum.Basic && sc.SchoolYear == schoolYear
                        select new InstitutionCacheModel
                        {
                            Id = sc.InstitutionId,
                        }).FirstOrDefaultAsync();

                    if (currentStudentInstitution != null)
                    {
                        var institution = institutionAll[currentStudentInstitution.Id].FirstOrDefault(x => x.InstType == (int)InstitutionTypeEnum.CenterForSpecialEducationalSupport);

                        if (institution != null)
                        {
                            currentStudentInstitution.Name = institution.Name;
                        }
                    }
                }
            }

            return currentStudentInstitution;
        }

        private async Task<ResultModel<BlobDO>> GenerateEnrolledStudentsExportFile(IEnumerable<ASPEnrolledStudentClassData> exportData, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ENROLLED_STUDENTS_EXPORT_HEADER);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            foreach (var item in exportData)
            {
                string line = string.Format(CultureInfo.InvariantCulture,
                $"{item.InstitutionCode,-7}|{item.PersonalId}|{item.PersonalIdType}|{item.FirstName,-25}|{item.MiddleName,-25}|{item.LastName,-25}|{item.StudentEduFormId,-2}|{item.BasicClassId}|{item.Status}");
                sb.AppendLine(line);
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var blob = await _blobService.UploadFileAsync(fileBytes, fileName, ".txt");
            return blob;
        }

        private async Task<List<ASPEnrolledStudentSubmittedDataModel>> ReadSubmittedDataFileAsync(IFormFile file)
        {
            // "Описание на структурата на експортен файл за модул „Отсъствия на децата и учениците“, част от Информационната система на образованието"
            // Файлът трябва да бъде в plain-text формат CSV с кодировка CP-1251(WINDOWS-1251).
            // Разделител на данните трябва да бъде Pipe (“|“). 

            var enc1251 = Encoding.GetEncoding("windows-1251");
            List<string> lines = await file.ReadAsListAsync(enc1251);

            // Без първия и последния ред
            return ParseASPEnrolledStudentSubmittedDataModels(lines.Skip(1).Reverse().Skip(1).ToList());
        }

        private List<ASPEnrolledStudentSubmittedDataModel> ParseASPEnrolledStudentSubmittedDataModels(List<string> lines)
        {
            if (lines.IsNullOrEmpty()) return null;

            List<ASPEnrolledStudentSubmittedDataModel> models = new List<ASPEnrolledStudentSubmittedDataModel>();
            foreach (string line in lines)
            {
                try
                {
                    ASPEnrolledStudentSubmittedDataModel model = ParseASPEnrolledStudentSubmittedDataModel(line);
                    if (model != null)
                    {
                        models.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    var error = new
                    {
                        ex.GetInnerMostException().Message,
                        Action = "ASPEnrolledStudentSubmittedDataModel",
                        Data = line
                    };


                    if (ex is IndexOutOfRangeException)
                    {
                        throw new Exception("Файлът и редовете в него следва да отговарят на изискванията за импорт на отсъствия от файл. Разделителят трябва да е |. На всеки ред трябва да има точно 8 разделителя.");
                    }

                    throw;
                }
            }

            return models;
        }

        private ASPEnrolledStudentSubmittedDataModel ParseASPEnrolledStudentSubmittedDataModel(string line)
        {
            if (line.IsNullOrEmpty()) return null;
            if (line.Equals("null", StringComparison.OrdinalIgnoreCase)) return null;

            try
            {
                string[] split = line.Split("|");
                ASPEnrolledStudentSubmittedDataModel model = new ASPEnrolledStudentSubmittedDataModel
                {
                    PersonalId = split[1].Trim(),
                    FirstName = split[3].Trim().Truncate(25),
                    MiddleName = split[4].Trim().Truncate(25),
                    LastName = split[5].Trim().Truncate(25),
                };

                if (int.TryParse(split[0], out int institutionCode))
                {
                    model.InstitutionCode = institutionCode;
                }

                if (int.TryParse(split[2], out int personalIdtype))
                {
                    model.PersonalIdType = personalIdtype;
                }

                if (int.TryParse(split[6], out int eduFormId))
                {
                    model.StudentEduFormId = eduFormId;
                }

                if (int.TryParse(split[7], out int basicClassId))
                {
                    model.BasicClassId = basicClassId;
                }

                if (int.TryParse(split[8], out int aspStatus))
                {
                    model.Status = aspStatus;
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ex: {ex.GetInnerMostException().Message} / Line: {line}");
            }
        }

        #endregion

        public async Task<IPagedList<ASPMonthlyBenefitsImportFileModel>> GetImportedBenefitsFilesAsync(ASPBenefitsInput input)
        {
            var query = from i in _context.AspmonthlyBenefitsImports
                            .AsNoTracking().OrderByDescending(x => x.SchoolYear).ThenByDescending(x => x.Month)
                        select
                        new
                        {
                            Import = i.ToImportFileModel(_blobServiceConfig),
                            Count = _userInfo.InstitutionID != null ? i.AspmonthlyBenefits.Count(a => a.CurrentInstitutionId == _userInfo.InstitutionID) : i.AspmonthlyBenefits.Count(),
                            ForReview = _userInfo.InstitutionID != null ? i.AspmonthlyBenefits.Count(a => a.CurrentInstitutionId == _userInfo.InstitutionID && a.Monstatus == (short)NEISPUOStatus.UnderReview) : i.AspmonthlyBenefits.Count(a => a.Monstatus == (short)NEISPUOStatus.UnderReview),
                            IsSigned = _userInfo.InstitutionID != null ? i.AspmonthlyBenefitInstitutions.FirstOrDefault(a => a.InstitutionId == _userInfo.InstitutionID).IsSigned : false,
                            SignedDate = _userInfo.InstitutionID != null ? i.AspmonthlyBenefitInstitutions.FirstOrDefault(a => a.InstitutionId == _userInfo.InstitutionID).SignedDate : (DateTime?)null,
                            IsActive = i.FromDate.HasValue && (i.FromDate <= Now.Date) && ((Now.Date <= i.ToDate) || i.ToDate == null),
                            i.FileStatusCheck,
                            i.AspSessionNo,
                            i.MonSessionNo
                        };

            if (_userInfo.IsSchoolDirector)
            {
                query = query.Where(i => i.FileStatusCheck == (int)ASPFileStatusCheckEnum.Success);
            }

            int totalCount = await query.CountAsync();
            IEnumerable<ASPMonthlyBenefitsImportFileModel> items = (await query.PagedBy(input.PageIndex, input.PageSize)
                .ToListAsync())
                .Select(x =>
                {
                    x.Import.RecordsCount = x.Count;
                    x.Import.ForReview = x.ForReview;
                    x.Import.IsSigned = x.IsSigned;
                    x.Import.SignedDate = x.SignedDate;
                    x.Import.IsActive = x.IsActive;
                    x.Import.AspSessionNo = x.AspSessionNo;
                    x.Import.MonSessionNo = x.MonSessionNo;

                    if (!_userInfo.IsSchoolDirector)
                    {
                        x.Import.AspConfirmSessionCount = _aspContext.AspConfs.Count(i => i.SessionNo == x.AspSessionNo);
                        x.Import.MonConfirmSessionCount = _aspContext.MonConfs.Count(i => i.SessionNo == x.MonSessionNo);
                    }

                    return x.Import;
                });

            return items.ToPagedList(await query.CountAsync());
        }

        public async Task<Result> ImportBenefitsAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);

            var blobDO = await _blobService.UploadFileAsync(ms.ToArray(), file.FileName, file.ContentType);

            using var stream = file.OpenReadStream();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1251 = Encoding.GetEncoding("windows-1251");

            string data;
            using (var reader = new StreamReader(file.OpenReadStream(), enc1251))
            {
                data = await reader.ReadToEndAsync();
            }

            string[] fileLines = data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var firstLine = fileLines.First().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            short schoolYear = Convert.ToInt16(firstLine[YEAR_INDEX]);
            short month = Convert.ToInt16(firstLine[MONTH_INDEX]);

            if (await BenefitsImported(schoolYear, month))
            {
                return new Result() { ImportStarted = false, Message = "Файлът вече е бил импортиран!" };
            }

            short aspSchoolYear = Common.GetSchoolYearFromYearMonth(schoolYear, month);

            AspmonthlyBenefitsImport benefitImport = new AspmonthlyBenefitsImport()
            {
                SchoolYear = aspSchoolYear,
                Month = month,
                ImportCompleted = false,
                FileStatusCheck = (int)ASPFileStatusCheckEnum.InProgress,
                ImportedBlobId = blobDO.Data.BlobId
            };

            _context.AspmonthlyBenefitsImports.Add(benefitImport);
            await SaveAsync();

            //_queue.QueueBackgroundWorkItem(async token =>
            {
                var token = new CancellationToken();
                using var scope = _serviceScopeFactory.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var scopedContext = scopedServices.GetRequiredService<MONContext>();
                var scopedStudentService = scopedServices.GetRequiredService<IStudentService>();
                scopedContext.Attach(benefitImport);

                var benefitsResult = await ReadBenefitsFileAsync(fileLines, scopedContext, scopedStudentService);

                if (!string.IsNullOrEmpty(benefitsResult.Errors))
                {
                    benefitImport.FileStatusCheck = (int)ASPFileStatusCheckEnum.Error;
                    benefitImport.ImportFileMessages = benefitsResult.Errors;
                    await scopedContext.SaveChangesAsync(token);
                    return new Result() { ImportStarted = true, Message = "Импортирането е стартирано, но има грешки." };
                }

                benefitImport.FileStatusCheck = (int)ASPFileStatusCheckEnum.Success;
                await scopedContext.SaveChangesAsync(token);

                using var transaction = scopedContext.Database.BeginTransaction();

                try
                {
                    var benefitsList = new List<AspmonthlyBenefit>();

                    foreach (var benefit in benefitsResult.Benefits)
                    {
                        var b = new AspmonthlyBenefit()
                        {
                            PersonId = benefit.PersonId,
                            SchoolYear = aspSchoolYear,
                            Month = month,
                            DaysCount = benefit.DaysCount,
                            AbsenceCount = benefit.AbsenceCount,
                            AspmonthlyBenefitsImportId = benefitImport.Id,
                            Monstatus = (short)NEISPUOStatus.UnderReview,
                            DaysCorrection = benefit.DaysCorrection,
                            AbsenceCorrection = benefit.AbsenceCorrection,
                            OnlineEnvironmentDays = benefit.OnlineEnvironmentDays,
                            Aspstatus = (short)benefit.ASPStatus.Value,
                            InstitutionId = benefit.InstitutionId,
                            CreatedBySysUserId = _userInfo.SysUserID,
                            CreateDate = DateTime.UtcNow,
                            CurrentInstitutionId = benefit.CurrentInstitutionId,
                        };

                        var institutionSchoolYear = await scopedContext.InstitutionSchoolYears.FirstOrDefaultAsync(x => x.SchoolYear == b.SchoolYear && x.InstitutionId == b.InstitutionId);

                        if (institutionSchoolYear == null)
                        {
                            throw new ArgumentException($"InstitutionSchoolYear not found for the provided parameters: Year {b.SchoolYear}, InstitutionId {b.InstitutionId}");
                        }

                        benefitsList.Add(b);
                    }

                    benefitImport.RecordsCount = benefitsList.Count();

                    await scopedContext.AspmonthlyBenefits.AddRangeAsync(benefitsList);
                    await scopedContext.SaveChangesAsync(token);

                    await transaction.CommitAsync();

                    benefitImport.ImportCompleted = true;
                    await scopedContext.SaveChangesAsync(token);
                    await SendNotification(benefitImport);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    benefitImport.FileStatusCheck = (int)ASPFileStatusCheckEnum.Error;
                    benefitImport.ImportCompleted = false;
                    benefitImport.ImportFileMessages = ex.GetInnerMostException().Message;
                    await scopedContext.SaveChangesAsync(token);
                }
            }
            //);

            return new Result() { ImportStarted = true, Message = "Импортирането е стартирано." };
        }

        public async Task LoadAspConfirmations(AspSessionInfoViewModel sessionInfoModel)
        {
            if (sessionInfoModel == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            short aspSchoolYear = Common.GetSchoolYearFromYearMonth((short)sessionInfoModel.Year, (short)sessionInfoModel.Month);

            AspmonthlyBenefitsImport aspmonthlyBenefitsImport = await _context.AspmonthlyBenefitsImports.FirstOrDefaultAsync(x => x.SchoolYear == aspSchoolYear && x.Month == (short)sessionInfoModel.Month);

            if (aspmonthlyBenefitsImport != null && aspmonthlyBenefitsImport.ImportCompleted)
            {
                throw new ApiException("Съществува кампания за потвърждаване на данни от АСП за избраната учебна година и месец");
            }

            await _aspContext.AspConfs
                .Where(x => x.SessionNo == sessionInfoModel.SessionNo)
                .UpdateAsync(x => new AspConf { ErrDetails = null });

            HashSet<int> schoolYearInstitutions = (await _context.InstitutionSchoolYears
                .Where(x => x.SchoolYear == aspSchoolYear)
                .Select(x => x.InstitutionId)
                .ToListAsync()).ToHashSet();

            var aspConfirmmations = await _aspContext.AspConfs.Where(x => x.SessionNo == sessionInfoModel.SessionNo)
                .Select(x => new
                {
                    x.Id,
                    x.IdNumber,
                    x.IdType,
                    x.SchoolId,
                    x.InfoType,
                    x.Days,
                    x.NotExcused
                })
                .ToListAsync();

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (aspmonthlyBenefitsImport == null)
                {
                    aspmonthlyBenefitsImport = new AspmonthlyBenefitsImport()
                    {
                        SchoolYear = aspSchoolYear,
                        Month = (short)sessionInfoModel.Month,
                        ImportCompleted = false,
                        FileStatusCheck = (int)ASPFileStatusCheckEnum.InProgress,
                        ImportedBlobId = null
                    };

                    _context.AspmonthlyBenefitsImports.Add(aspmonthlyBenefitsImport);
                    await SaveAsync();
                }

                await _context.AspmonthlyBenefits.Where(x => x.AspmonthlyBenefitsImportId == aspmonthlyBenefitsImport.Id).DeleteAsync();

                var benefitsList = new List<AspmonthlyBenefit>();
                foreach (var aspConf in aspConfirmmations)
                {
                    StringBuilder errorDetailsSb = new StringBuilder();

                    if (!int.TryParse(aspConf.IdType, out int personalIdtype))
                    {
                        // Не се каства типа на идентификатора
                        errorDetailsSb.AppendLine($"Невалиден идентификатор: {aspConf.IdType}.");
                    }

                    var persons = await _context.People
                            .Where(x => x.PersonalId == aspConf.IdNumber)
                            .Select(x => new
                            {
                                x.PersonId,
                                x.PersonalId,
                                x.PersonalIdtype
                            }).ToListAsync();

                    int? personId = null;
                    if (persons == null || persons.Count == 0)
                    {
                        // Не е намереан Person с този идентофикатор
                        errorDetailsSb.AppendLine($"Не е намерен ученик с идентификатор '{aspConf.IdNumber}'.");
                    }
                    else
                    {
                        var person = persons.FirstOrDefault(x => x.PersonalIdtype == personalIdtype);
                        if (person == null)
                        {
                            // Намерен е Person с този идентификатор, но с друг тип на идентификатора.
                            errorDetailsSb.AppendLine($"Намереният ученик с идентификатор '{aspConf.IdNumber}' е с различен тип на индентификатор '{persons.Select(x => x.PersonalIdtype).FirstOrDefault()}' от подадения '{aspConf.IdType}'.");
                        }
                        else
                        {
                            personId = person.PersonId;
                        }

                    }

                    if (!int.TryParse(aspConf.SchoolId, out int institutionId))
                    {
                        errorDetailsSb.AppendLine($"Невалиден код по НЕИСПУО '{aspConf.SchoolId}'.");
                    }

                    if (!schoolYearInstitutions.Contains(institutionId))
                    {
                        // Не намерен запис за институцията и учебната година в InstitutionSchoolYears
                        errorDetailsSb.AppendLine($"Невалиден код по НЕИСПУО '{institutionId}' за учебна година '{aspSchoolYear}'.");
                    }

                    if (!short.TryParse(aspConf.InfoType, out short aspStatus))
                    {
                        // Не се каства АСП статус
                        errorDetailsSb.AppendLine($"Невалиден АСП статус '{aspConf.InfoType}'.");
                    }

                    if (errorDetailsSb.Length > 0)
                    {
                        // Има грешки
                        await _aspContext.AspConfs
                            .Where(x => x.Id == aspConf.Id)
                            .UpdateAsync(x => new AspConf { ErrDetails = errorDetailsSb.ToString() });

                        continue;
                    }

                    int currentInstitutionId = await _studentService.GetAttendedInstitution(persons.FirstOrDefault().PersonId, institutionId);

                    benefitsList.Add(new AspmonthlyBenefit()
                    {
                        PersonId = personId.Value,
                        SchoolYear = aspSchoolYear,
                        Month = (short)sessionInfoModel.Month,
                        DaysCount = (short)aspConf.Days,
                        AbsenceCount = aspConf.NotExcused,
                        AspmonthlyBenefitsImportId = aspmonthlyBenefitsImport.Id,
                        Monstatus = (short)NEISPUOStatus.UnderReview,
                        Aspstatus = aspStatus,
                        InstitutionId = institutionId,
                        CreatedBySysUserId = _userInfo.SysUserID,
                        CreateDate = DateTime.UtcNow,
                        CurrentInstitutionId = currentInstitutionId,
                    });
                }

                if (benefitsList.Count > 0)
                {
                    _context.AspmonthlyBenefits.AddRange(benefitsList);
                }

                await SaveAsync();

                aspmonthlyBenefitsImport.ImportCompleted = true;
                aspmonthlyBenefitsImport.AspSessionNo = sessionInfoModel.SessionNo;
                aspmonthlyBenefitsImport.ImportCompleted = true;
                aspmonthlyBenefitsImport.FileStatusCheck = (int)ASPFileStatusCheckEnum.Success;

                await SaveAsync();

                await transaction.CommitAsync();

                await _aspContext.Asp2monSessionInfos
                    .Where(x => x.TargetMonth == new DateTime(sessionInfoModel.Year, sessionInfoModel.Month, 1))
                    .UpdateAsync(x => new Asp2monSessionInfo { MonProcessed = Now });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException()?.Message, ex);
            }
        }

        public async Task UpdateImportedBenefitsFileMetaDataAsync(ASPMonthlyBenefitsImportFileModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPImport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            AspmonthlyBenefitsImport entity = await _context.AspmonthlyBenefitsImports
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            entity.FromDate = model.FromDate;
            entity.ToDate = model.ToDate;

            await SaveAsync();
            await SendNotification(entity);
        }

        public async Task<ASPMonthlyBenefitViewModel> GetImportedBenefitsFileMetaDataAsync(int importedFileId)
        {
            ASPMonthlyBenefitViewModel importedFileDetails = await _context.AspmonthlyBenefitsImports.AsNoTracking()
                .Where(x => x.Id == importedFileId)
                .Select(x => new ASPMonthlyBenefitViewModel
                {
                    DateImported = x.CreateDate.Date.ToString(),
                    Month = x.Month.ToString(),
                    SchoolYear = x.SchoolYear,
                    RecordsCount = _userInfo.InstitutionID != null ? x.AspmonthlyBenefits.Count(a => a.CurrentInstitutionId == _userInfo.InstitutionID) : x.AspmonthlyBenefits.Count(),
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsActive = x.FromDate.HasValue && (x.FromDate <= Now.Date) && ((Now.Date <= x.ToDate) || x.ToDate == null),
                    IsSigned = _userInfo.InstitutionID != null && x.AspmonthlyBenefitInstitutions.FirstOrDefault(x => x.InstitutionId == _userInfo.InstitutionID).IsSigned,
                    SignedDate = _userInfo.InstitutionID != null ? x.AspmonthlyBenefitInstitutions.FirstOrDefault(x => x.InstitutionId == _userInfo.InstitutionID).SignedDate : null,
                    AspSessionNo = x.AspSessionNo,
                    MonSessionNo = x.MonSessionNo
                }).SingleOrDefaultAsync();

            if (importedFileDetails != null && importedFileDetails.AspSessionNo.HasValue)
            {
                importedFileDetails.AspConfirmSessionCount = await _aspContext.AspConfs.CountAsync(i => i.SessionNo == importedFileDetails.AspSessionNo);
                importedFileDetails.MonConfirmSessionCount = await _aspContext.MonConfs.CountAsync(i => i.SessionNo == importedFileDetails.MonSessionNo);
            }

            importedFileDetails.Month = CultureInfo.GetCultureInfo("bg-BG").DateTimeFormat.GetMonthName(int.Parse(importedFileDetails.Month));

            return importedFileDetails;
        }

        public async Task<IPagedList<ASPMonthlyBenefitModel>> GetImportedBenefitsDetailsAsync(ASPBenefitsInput input)
        {
            var importedFile = await (from mb in _context.AspmonthlyBenefitsImports.AsNoTracking().Where(x => x.Id == input.ImportedFileId) select new { mb.SchoolYear, mb.Month }).SingleOrDefaultAsync();

            if (importedFile == null)
            {
                throw new Exception($"No record found in database for the provided id:{input.ImportedFileId}");
            }

            var query = _context.AspmonthlyBenefits.AsNoTracking().Where(x => x.AspmonthlyBenefitsImportId == input.ImportedFileId)
                                                                   .Where(!input.Filter.IsNullOrWhiteSpace(),
                                                                         predicate => predicate.Person.FirstName.Contains(input.Filter)
                                                                                    || predicate.Person.MiddleName.Contains(input.Filter)
                                                                                    || predicate.Person.LastName.Contains(input.Filter)
                                                                                    || predicate.Person.PublicEduNumber.Contains(input.Filter)
                                                                                    || predicate.CurrentInstitutionId.ToString().Contains(input.Filter));
            if (input.StatusFilter != -1)
            {
                query = query.Where(x => x.Monstatus == input.StatusFilter);
            }

            if (!_userInfo.InstitutionID.HasValue && _userInfo.IsMon)
            {

            }

            if (_userInfo.InstitutionID.HasValue && _userInfo.IsSchoolDirector)
            {
                query = query.Where(x => x.CurrentInstitutionId == _userInfo.InstitutionID);
            }

            if (_userInfo.RegionID.HasValue && _userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID);
            }

            var fileDetails = (from q in query
                               select new ASPMonthlyBenefitModel
                               {
                                   Id = q.Id,
                                   FirstName = q.Person.FirstName,
                                   MiddleName = q.Person.MiddleName,
                                   LastName = q.Person.LastName,
                                   DaysCount = q.DaysCount,
                                   AbsenceCount = q.AbsenceCount,
                                   NeispuoStatusId = q.Monstatus,
                                   AspMonthlyBenefitsImportId = q.AspmonthlyBenefitsImportId,
                                   DaysCorrection = q.DaysCorrection,
                                   AbsenceCorrection = q.AbsenceCorrection,
                                   CreatedBy = q.CreatedBySysUser.Username,
                                   CreatedDate = q.CreateDate,
                                   PublicEduNumber = q.Person.PublicEduNumber,
                                   AspStatusId = q.Aspstatus,
                                   InstitutionId = q.InstitutionId,
                                   InstitutionName = q.InstitutionSchoolYear.Abbreviation,
                                   CurrentInstitutionId = q.CurrentInstitutionId,
                                   CurrentInstitutionName = q.InstitutionSchoolYearNavigation.Abbreviation,
                                   ModifiedBy = q.ModifiedBySysUser.Username,
                                   Reason = q.Reason,
                                   ModifyDate = q.ModifyDate,
                                   Month = q.Month.ToString(),
                                   OnlineEnvironmentDays = q.OnlineEnvironmentDays,
                                   PersonId = q.PersonId,
                                   SchoolYear = q.SchoolYear
                               }).OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy); ;

            var items = await fileDetails.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            items = items.Select(i =>
            {
                i.NeispuoStatus = new DropdownViewModel { Value = i.NeispuoStatusId, Text = ((NEISPUOStatus)i.NeispuoStatusId).GetEnumDescriptionAttrValue(), Name = ((NEISPUOStatus)i.NeispuoStatusId).GetEnumDescriptionAttrValue() };
                i.ASPStatus = new DropdownViewModel { Value = i.AspStatusId, Text = ((ASPStatusEnum)i.AspStatusId).GetEnumDescriptionAttrValue(), Name = ((ASPStatusEnum)i.AspStatusId).GetEnumDescriptionAttrValue() };
                i.CurrentClassName = _studentService.GetCurrentClassName(i.PersonId, i.SchoolYear).Result;
                i.BasicClassId = _studentService.GetCurrentClass(i.PersonId, i.SchoolYear).Result?.BasicClassId;
                i.BasicClassName = _studentService.GetCurrentClass(i.PersonId, i.SchoolYear).Result?.BasicClassName;
                i.Month = CultureInfo.GetCultureInfo("bg-BG").DateTimeFormat.GetMonthName(int.Parse(i.Month));
                return i;
            }).ToList();

            var toReturn = items.ToPagedList(await fileDetails.CountAsync());

            return toReturn;
        }

        public async Task<IPagedList<AspUnprocessedRequestViewModel>> UnprocessedAspConfirmsList(ApPUnprocessedRequestsInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            IQueryable<AspConf> query = _aspContext.AspConfs
                .Where(x => x.SessionNo == input.AspSessionNo && x.ErrDetails != null);

            if (!input.Filter.IsNullOrEmpty())
            {
                query = query.Where(x => x.IdNumber.Contains(input.Filter)
                 || x.SchoolId.Contains(input.Filter)
                 || x.Name1.Contains(input.Filter)
                 || x.Name2.Contains(input.Filter)
                 || x.Name3.Contains(input.Filter));
            }

            IQueryable<AspUnprocessedRequestViewModel> listQuery = query
                .Select(x => new AspUnprocessedRequestViewModel
                {
                    Id = x.Id,
                    SessionNo = x.SessionNo,
                    Year = x.IntYear,
                    Month = x.IntMonth,
                    InstitutionId = x.SchoolId,
                    Pin = x.IdNumber,
                    PinType = x.IdTypeName,
                    FirstName = x.Name1,
                    MiddleName = x.Name2,
                    LastName = x.Name3,
                    NotExcused = x.NotExcused,
                    Days = x.Days,
                    AspStatus = x.InfoType,
                    Error = x.ErrDetails
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id" : input.SortBy);

            int totalCount = await listQuery.CountAsync();

            return (await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync()).ToPagedList(totalCount);
        }

        public async Task DeleteImporedFileRecordAsync(int importId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPImport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            AspmonthlyBenefitsImport entity = await _context.AspmonthlyBenefitsImports
                .SingleOrDefaultAsync(x => x.Id == importId);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.AspmonthlyBenefits
                    .Where(x => x.AspmonthlyBenefitsImportId == importId)
                    .DeleteAsync();

                _context.AspmonthlyBenefitsImports.Remove(entity);

                await SaveAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw new ApiException(ex.GetInnerMostException().Message, ex);
            }
        }

        public async Task<IPagedList<ASPMonthlyBenefitReportDetailsDto>> GetInstitutionsStillReviewingBenefitsAsync(ASPBenefitsInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPBenefitDetailsRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            FormattableString queryString = $"select * from [student].[fn_ASPMonthlyBenefitReportDetails]({input.SchoolYear}, {input.Month})";
            IQueryable<ASPMonthlyBenefitReportDetailsDto> query = _context.Set<ASPMonthlyBenefitReportDetailsDto>()
                .FromSqlInterpolated(queryString)
                //.Where(x => x.SchoolYear == input.SchoolYear && x.Month == x.Month)
                .AsNoTracking();

            if ((_userInfo.IsRuo || _userInfo.IsRuoExpert) && _userInfo.RegionID.HasValue)
            {
                query = query.Where(i => i.CurrentInstitutionRegionId == _userInfo.RegionID);
            }

            query = input.StatusFilter switch
            {
                0 => query.Where(x => x.AllRecords > 0 && x.AllRecords == x.Pending), // Институции, които не са запoчнали обработка
                1 => query.Where(x => x.AllRecords > 0 && x.AllRecords != x.Pending && x.Pending > 0), // Започнали са правят нещо, но не завършили всички
                2 => query.Where(x => x.AllRecords > 0 && x.Pending == 0 && !x.IsSigned), // Институции, които за обработили всички записи, но не са подписали
                3 => query.Where(x => x.AllRecords > 0 && x.IsSigned), // Подписали
                _ => query // Всички
            };

            query = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.CurrentInstitutionCode.Contains(input.Filter)
                   || predicate.CurrentInstitutionName.Contains(input.Filter)
                   || predicate.CurrentInstitutionAbbreviation.Contains(input.Filter)
                   || predicate.CurrentInstitutionDetailedSchoolType.Contains(input.Filter)
                   || predicate.CurrentInstitutionTown.Contains(input.Filter)
                   || predicate.CurrentInstitutionEmail.Contains(input.Filter)
                   || predicate.CurrentInstitutionPhone.Contains(input.Filter))
                .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "CurrentInstitutionId desc" : input.SortBy);

            int totalCount = query.Count();
            IList<ASPMonthlyBenefitReportDetailsDto> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task ExportApprovedMonthlyBenefits(int campaignId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPImport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            AspmonthlyBenefitsImport aspMonthlyBenefitsImport = await _context.AspmonthlyBenefitsImports
                .SingleOrDefaultAsync(x => x.Id == campaignId);

            if (aspMonthlyBenefitsImport == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentException(nameof(AspmonthlyBenefitsImport)));
            }

            var asp2MonSessionInfo = await _aspContext.Asp2monSessionInfos
                .Where(x => x.SessionNo == aspMonthlyBenefitsImport.AspSessionNo)
                .Select(x => new
                {
                    x.SessionNo,
                    x.TargetMonth,
                })
                .SingleOrDefaultAsync();

            if (asp2MonSessionInfo == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentException(nameof(Asp2monSessionInfo)));
            }

            Mon2aspSessionInfo mon2AspSessionInfo = aspMonthlyBenefitsImport.MonSessionNo.HasValue
                ? await _aspContext.Mon2aspSessionInfos.SingleOrDefaultAsync(x => x.SessionNo == aspMonthlyBenefitsImport.MonSessionNo)
                : await _aspContext.Mon2aspSessionInfos.SingleOrDefaultAsync(x => x.InfoType == SessionInfoTypeEnum.MonConfirmation.GetEnumDescription() && x.TargetMonth == asp2MonSessionInfo.TargetMonth);

            if (mon2AspSessionInfo == null)
            {
                mon2AspSessionInfo = new Mon2aspSessionInfo
                {
                    InfoType = SessionInfoTypeEnum.MonConfirmation.GetEnumDescription(),
                    TargetMonth = asp2MonSessionInfo.TargetMonth,
                    StartDate = Now
                };

                _aspContext.Mon2aspSessionInfos.Add(mon2AspSessionInfo);
                await _aspContext.SaveChangesAsync();
            }
            else
            {
                mon2AspSessionInfo.StartDate = Now;
            }

            await _aspContext.MonConfs.Where(x => x.SessionNo == mon2AspSessionInfo.SessionNo).DeleteAsync();

            List<MonConf> monConfs = new List<MonConf>();
            var aspmonthlyBenefits = await _context.AspmonthlyBenefits
                .Where(x => x.AspmonthlyBenefitsImportId == aspMonthlyBenefitsImport.Id)
                .Select(x => new
                {
                    x.CurrentInstitutionId,
                    x.Person.PersonalId,
                    x.Person.PersonalIdtype,
                    x.Person.FirstName,
                    x.Person.MiddleName,
                    x.Person.LastName,
                    x.Monstatus,
                    x.AbsenceCorrection,
                    x.AbsenceCount,
                    x.DaysCorrection,
                    x.DaysCount,
                    x.Aspstatus
                })
                .ToListAsync();

            foreach (var item in aspmonthlyBenefits.Where(x => x.PersonalIdtype.HasValue && x.PersonalId != null))
            {
                monConfs.Add(new MonConf
                {
                    SessionNo = mon2AspSessionInfo.SessionNo,
                    IntYear = mon2AspSessionInfo.TargetMonth.Year,
                    IntMonth = $"{mon2AspSessionInfo.TargetMonth.Month:00}",
                    SchoolId = $"{item.CurrentInstitutionId:0000000}".Truncate(7),
                    IdNumber = (item.PersonalId ?? "").Truncate(10),
                    IdType = $"{item.PersonalIdtype}".Truncate(1),
                    Name1 = (item.FirstName ?? "").Truncate(25),
                    Name2 = item.MiddleName?.Truncate(25),
                    Name3 = item.LastName?.Truncate(25),
                    NotExcused = item.AbsenceCount,
                    Days = item.DaysCount,
                    InfoType = $"{item.Aspstatus}".Truncate(1),
                    FlagConf = $"{item.Monstatus}".Truncate(1),
                    PnotExcused = item.Monstatus == (short)NEISPUOStatus.Rejected && item.Aspstatus == (short)MonConfirmationInfoTypeEnum.Absences
                        ? item.AbsenceCorrection ?? 0m
                        : 0m,
                    Pdays = item.Monstatus == (short)NEISPUOStatus.Rejected && item.Aspstatus == (short)MonConfirmationInfoTypeEnum.StudyEmployment
                        ? item.DaysCorrection ?? 0
                        : 0,
                });
            }

            _aspContext.MonConfs.AddRange(monConfs);
            mon2AspSessionInfo.EndDate = Now;

            await _aspContext.SaveChangesAsync();

            aspMonthlyBenefitsImport.ExportedDate = mon2AspSessionInfo.EndDate;
            aspMonthlyBenefitsImport.ExportedBySysUserId = _userInfo.SysUserID;
            aspMonthlyBenefitsImport.MonSessionNo = mon2AspSessionInfo.SessionNo;
            aspMonthlyBenefitsImport.RecordsCount = monConfs.Count;
            await SaveAsync();

            await SendNotification(aspMonthlyBenefitsImport);
        }

        public async Task ExportEnrolledStudentsAsync(EnrolledStudentsExportModel model)
        {
            ApiValidationResult validationResult = _aspValidator.ValidateEnrolledStudentsExportBasicRules(model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            // Get the export data from db
            _context.Database.SetCommandTimeout(300);

            List<ASPEnrolledStudentClassData> currentExportClassesData = await GetEnrolledStudentsClassesData(model.SchoolYear, model.FileType);
            int recordsCount = currentExportClassesData.Count;
            if (recordsCount == 0)
            {
                throw new InvalidOperationException("There isn't any data to export!");
            }

            // Generate the export file
            ResultModel<BlobDO> blob = await GenerateEnrolledStudentsExportFile(currentExportClassesData, $"MONZP{Now.Month:00}{model.SchoolYear:00}.txt");

            //Insert the exported data in the db
            AspenrolledStudentsExport existingExport = await _context.AspenrolledStudentsExports
               .Where(x => x.SchoolYear == model.SchoolYear)
               .Select(x => new AspenrolledStudentsExport { Id = x.Id })
               .FirstOrDefaultAsync();
            if (existingExport != null)
            {
                _context.AspenrolledStudentsExports.Remove(existingExport);
            }

            AspenrolledStudentsExport aspenrolledStudentsExport = new AspenrolledStudentsExport
            {
                FileType = model.FileType,
                Month = model.Month,
                SchoolYear = model.SchoolYear,
                RecordsCount = recordsCount,
                BlobId = blob.Data.BlobId,
                Contents = JsonConvert.SerializeObject(currentExportClassesData)
            };
            _context.AspenrolledStudentsExports.Add(aspenrolledStudentsExport);

            await SaveAsync();
        }

        public async Task ExportEnrolledStudentsCorrectionsAsync(EnrolledStudentsExportModel model)
        {
            ApiValidationResult validationResult = await _aspValidator.ValidateEnrolledStudentsExportCorrections(model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            // Get the previous version of the exported file contents and compare it with the current export data
            _context.Database.SetCommandTimeout(300);

            string existingExportClassesDataContents = _context.AspenrolledStudentsExports
               .AsNoTracking()
               .Where(x => x.SchoolYear == model.SchoolYear && x.Month == model.Month - 1)
               .Select(x => x.Contents)
               .FirstOrDefault();

            if (string.IsNullOrEmpty(existingExportClassesDataContents))
            {
                existingExportClassesDataContents = _context.AspenrolledStudentsExports
                   .AsNoTracking()
                   .Where(x => x.SchoolYear == model.SchoolYear)
                   .Select(x => x.Contents)
                   .FirstOrDefault();
            }

            List<ASPEnrolledStudentClassData> currentExportClassesData = await GetEnrolledStudentsClassesData(model.SchoolYear, model.FileType);

            IEnumerable<ASPEnrolledStudentClassData> existingExportClassesData = JsonConvert.DeserializeObject<List<ASPEnrolledStudentClassData>>(existingExportClassesDataContents);
            IEnumerable<ASPEnrolledStudentClassData> diffExportData = currentExportClassesData.Except(existingExportClassesData);
            int recordsCount = diffExportData.Count();
            if (recordsCount == 0)
            {
                throw new InvalidOperationException("The current data to export is equal to the previous one!");
            }

            // Generate the export file
            ResultModel<BlobDO> blob = await GenerateEnrolledStudentsExportFile(diffExportData, $"MONZPC{model.Month}{model.SchoolYear:00}.txt");

            //Insert the exported data in the db
            AspenrolledStudentsExport existingExport = await _context.AspenrolledStudentsExports
               .Where(x => x.SchoolYear == model.SchoolYear && x.Month == model.Month)
               .Select(x => new AspenrolledStudentsExport { Id = x.Id })
               .FirstOrDefaultAsync();
            if (existingExport != null)
            {
                _context.AspenrolledStudentsExports.Remove(existingExport);
            }

            AspenrolledStudentsExport aspenrolledStudentsExport = new AspenrolledStudentsExport
            {
                FileType = model.FileType,
                Month = model.Month,
                SchoolYear = model.SchoolYear,
                RecordsCount = recordsCount,
                BlobId = blob.Data.BlobId,
                Contents = JsonConvert.SerializeObject(currentExportClassesData),
                ContentsDiff = JsonConvert.SerializeObject(diffExportData)
            };
            _context.AspenrolledStudentsExports.Add(aspenrolledStudentsExport);

            await SaveAsync();
        }

        public async Task<bool> CheckForExistingEnrolledStudentsExportAsync(EnrolledStudentsExportModel model)
        {
            ApiValidationResult validationResult = _aspValidator.ValidateEnrolledStudentsExportBasicRules(model);
            if (validationResult.HasErrors)
            {
                throw new ApiException(Messages.ValidationError, 500, validationResult.Errors);
            }

            AspenrolledStudentsExport aspEnrolledStudentsExport;
            bool isExisting = false;

            switch (model.FileType)
            {
                case (int)AspEnrolledStudentsExportFileType.EnrolledStudents:
                    aspEnrolledStudentsExport = await _context.AspenrolledStudentsExports
                        .AsNoTracking()
                        .Where(x => x.SchoolYear == model.SchoolYear)
                        .Select(x => new AspenrolledStudentsExport { Id = x.Id })
                        .FirstOrDefaultAsync();
                    if (aspEnrolledStudentsExport != null)
                    {
                        isExisting = true;
                    }
                    break;

                case (int)AspEnrolledStudentsExportFileType.EnrolledStudentsCorrections:
                    aspEnrolledStudentsExport = await _context.AspenrolledStudentsExports
                        .AsNoTracking()
                        .Where(x => x.SchoolYear == model.SchoolYear && x.Month == model.Month)
                        .Select(x => new AspenrolledStudentsExport { Month = x.Month })
                        .FirstOrDefaultAsync();
                    if (aspEnrolledStudentsExport != null && aspEnrolledStudentsExport.Month == model.Month)
                    {
                        isExisting = true;
                    }
                    break;
            }

            return isExisting;
        }

        public async Task<IEnumerable<ASPEnrolledStudentsExportFileModel>> GetEnrolledStudentsExportFilesAsync(short? schoolYear)
        {
            IQueryable<AspenrolledStudentsExport> query = _context.AspenrolledStudentsExports
                .AsNoTracking()
                .Select(x => new AspenrolledStudentsExport
                {
                    Id = x.Id,
                    CreateDate = x.CreateDate,
                    Month = x.Month,
                    SchoolYear = x.SchoolYear,
                    RecordsCount = x.RecordsCount,
                    BlobId = x.BlobId,
                });

            if (schoolYear.HasValue)
            {
                query = query.Where(x => x.SchoolYear == schoolYear.Value);
            }

            IEnumerable<ASPEnrolledStudentsExportFileModel> importedFiles = query.Select(i => i.ToImportFileModel(_blobServiceConfig));

            return await Task.FromResult(importedFiles);
        }

        public async Task UpdateBenefitAsync(ASPMonthlyBenefitModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPBenefitUpdate))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            AspmonthlyBenefit entity = await _context.AspmonthlyBenefits
                .Include(x => x.AspmonthlyBenefitsImport)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            ValidateCampaign(entity.AspmonthlyBenefitsImport);

            if (entity.CurrentInstitutionId != _userInfo.InstitutionID &&
                !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPGlobalAdmin))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.DaysCount = model.DaysCount;
            if (((NEISPUOStatus)model.NeispuoStatus.Value) == NEISPUOStatus.Confirmed)
            {
                entity.DaysCorrection = 0;
                entity.AbsenceCorrection = 0;
            }
            else
            {
                entity.DaysCorrection = model.DaysCorrection;
                entity.AbsenceCorrection = model.AbsenceCorrection;
            }


            entity.Monstatus = (short)model.NeispuoStatus.Value;
            entity.Reason = model.Reason;

            await ValidateBenefit(entity);

            await SaveAsync();
        }

        public async Task<string> ConstructAspBenefitsAsXml(AspBenefitsModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            var exportDetails = await _context.AspmonthlyBenefits
                .Where(x => x.AspmonthlyBenefitsImportId == model.AspMonthlyBenefitImportId && x.CurrentInstitutionId == _userInfo.InstitutionID)
                .Select(x => new
                {
                    x.Id,
                    x.SchoolYear,
                    x.AspmonthlyBenefitsImport.Month,
                    x.Person.PersonalId,
                    MonStatus = x.Monstatus,
                    x.AbsenceCorrection,
                    x.DaysCorrection,
                })
                .ToListAsync();

            if (exportDetails.Any(x => x.MonStatus == (int)NEISPUOStatus.UnderReview))
            {
                throw new ApiException("Има записи, които са в статус 'За преглед'!", 400);
            }

            SignedAspBenefits signedModel = new SignedAspBenefits
            {
                AspBenefitsImportId = model.AspMonthlyBenefitImportId,
                Version = 1,
                Contents = JsonConvert.SerializeObject(exportDetails)
            };

            return signedModel.ToXml();
        }

        public async Task RemoveAspBenefitsSigningAtrributes(AspBenefitsSigningAtrributesSetModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            AspmonthlyBenefitsImport aspMonthlyBenefitImport = await _context.AspmonthlyBenefitsImports.SingleOrDefaultAsync(x => x.Id == model.AspBenefitsImportId);

            ValidateCampaign(aspMonthlyBenefitImport);

            AspmonthlyBenefitInstitution aspBenefitForInstitution = await _context.AspmonthlyBenefitInstitutions
                .SingleOrDefaultAsync(x => x.InstitutionId == _userInfo.InstitutionID && x.AspmonthlyBenefitImportId == model.AspBenefitsImportId);
            if (aspBenefitForInstitution != null)
            {
                _context.Remove(aspBenefitForInstitution);
                await SaveAsync();
            }
        }

        public async Task SetAspBenefitsSigningAtrributes(AspBenefitsSigningAtrributesSetModel model)
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.UnauthorizedMessageError);
            }

            if (model == null) throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);

            AspmonthlyBenefitsImport aspMonthlyBenefitImport = await _context.AspmonthlyBenefitsImports
                .SingleOrDefaultAsync(x => x.Id == model.AspBenefitsImportId);

            ValidateCampaign(aspMonthlyBenefitImport);

            AspmonthlyBenefitInstitution aspBenefitForInstitution = await _context.AspmonthlyBenefitInstitutions
                .SingleOrDefaultAsync(x => x.InstitutionId == _userInfo.InstitutionID && x.AspmonthlyBenefitImportId == model.AspBenefitsImportId);

            if (aspBenefitForInstitution == null)
            {
                aspBenefitForInstitution = new AspmonthlyBenefitInstitution()
                {
                    InstitutionId = _userInfo.InstitutionID.Value,
                    AspmonthlyBenefitImportId = model.AspBenefitsImportId,
                    SchoolYear = aspMonthlyBenefitImport.SchoolYear
                };

                _context.AspmonthlyBenefitInstitutions.Add(aspBenefitForInstitution);
            }

            if (aspBenefitForInstitution.IsSigned)
            {
                throw new ApiException(Messages.IsSignedError, 400);
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
            aspBenefitForInstitution.IsSigned = true;
            aspBenefitForInstitution.SignedDate = DateTime.UtcNow;
            aspBenefitForInstitution.Signature = signature;

            try
            {
                var certificateResult = await _certificateService.VerifyXml(aspBenefitForInstitution.Signature);
                if (!certificateResult.IsValid)
                {
                    _logger.LogWarning($"Резултат от подписване на АСП потвърждения: {JsonConvert.SerializeObject(certificateResult)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Грешка при подписване на АСП потвърждения");
            }

            await SaveAsync();
        }

        public async Task<IPagedList<ASPEnrolledStudentSubmittedDataViewModel>> ListEnrolledStudentsSubmittedData(EnrolledStudentsSubmittedDataListInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);
            }

            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPEnrolledStudentsExport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<AspSubmittedDataHistory> query = _context.AspSubmittedDataHistories
                .Where(x => x.ExportTypeCode != "Old")
                .AsNoTracking();

            if (input.Year.HasValue)
            {
                query = query.Where(x => x.Year == input.Year.Value);
            }

            if (input.Month.HasValue)
            {
                query = query.Where(x => x.Month == input.Month.Value);
            }

            if (!input.ExportTypeCode.IsNullOrEmpty())
            {
                query = query.Where(x => x.ExportTypeCode == input.ExportTypeCode);
            }

            if (input.AspStatus.HasValue)
            {
                query = query.Where(x => x.Aspstatus == input.AspStatus.Value);
            }

            IQueryable<ASPEnrolledStudentSubmittedDataViewModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.PersonalId.Contains(input.Filter)
                   || predicate.FirstName.Contains(input.Filter)
                   || predicate.MiddleName.Contains(input.Filter)
                   || predicate.LastName.Contains(input.Filter)
                   || predicate.ExportTypeCode.Contains(input.Filter)
                   || predicate.MonthNavigation.Name.Contains(input.Filter))
                .Select(x => new ASPEnrolledStudentSubmittedDataViewModel
                {
                    Id = x.Id,
                    InstitutionCode = x.InstitutionId,
                    PersonalId = x.PersonalId,
                    PersonalIdType = x.PersonalIdType,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    StudentEduFormId = x.EduFormId ?? 0,
                    BasicClassId = x.BasicClassId ?? 0,
                    Status = x.Aspstatus,
                    Year = x.Year ?? 0,
                    Month = (byte)(x.Month),
                    MonthName = x.MonthNavigation.Name,
                    ExportTypeCode = x.ExportTypeCode

                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await listQuery.CountAsync();
            List<ASPEnrolledStudentSubmittedDataViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task UploadSubmittedData(IFormFile file)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPEnrolledStudentsExport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (file == null || file.Length <= 0)
            {
                throw new ArgumentNullException(nameof(file));
            }

            string exportTypeCode;
            short? year = null;
            byte? month = null;
            if ((file.FileName ?? "").Contains("MONZPC", StringComparison.OrdinalIgnoreCase))
            {
                exportTypeCode = "MONZPC";

                if (byte.TryParse(file.FileName.Substring(6, 2), out byte m))
                {
                    month = m;
                }

                if (short.TryParse(file.FileName.Substring(8, 2), out short y))
                {
                    year = (short)(2000 + y);
                }

            }
            else if ((file.FileName ?? "").Contains("MONZP", StringComparison.OrdinalIgnoreCase))
            {
                exportTypeCode = "MONZP";

                if (byte.TryParse(file.FileName.Substring(5, 2), out byte m))
                {
                    month = m;
                }

                if (int.TryParse(file.FileName.Substring(7, 2), out int y))
                {
                    year = (short)(2000 + y);
                }
            }
            else
            {
                throw new ApiException("Unknown file name. MONZP{mm}{yy} or MONZPC{mm}{yy} expected.");
            }

            if (!year.HasValue) throw new InvalidOperationException("Cannot parse year from file name.");

            if (!month.HasValue) throw new InvalidOperationException("Cannot parse month from file name.");

            List<ASPEnrolledStudentSubmittedDataModel> models = await ReadSubmittedDataFileAsync(file);

            if (models == null || !models.Any()) throw new InvalidOperationException("Empty models");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.AspSubmittedDataHistories.Where(x => x.Year == year.Value
                        && x.Month == month.Value
                        && x.ExportTypeCode == exportTypeCode)
                    .DeleteAsync();

                _context.AspSubmittedDataHistories.AddRange(models.Select(x => new AspSubmittedDataHistory
                {
                    InstitutionId = x.InstitutionCode,
                    PersonalId = x.PersonalId,
                    PersonalIdType = x.PersonalIdType ?? default,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    LastName = x.LastName,
                    EduFormId = (short)x.StudentEduFormId,
                    BasicClassId = (short)x.BasicClassId,
                    Aspstatus = (byte)x.Status,
                    Year = year,
                    Month = (short)month,
                    ExportTypeCode = exportTypeCode
                }));

                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message, ex);
            }
        }

        public async Task<List<AbsenceCampaignViewModel>> GetActiveCampaigns()
        {
            DateTime utcNow = DateTime.UtcNow;

            List<AbsenceCampaignViewModel> campaigns = await _context.AspmonthlyBenefitsImports
                .AsNoTracking()
                .Where(x => x.FromDate.HasValue && x.ToDate.HasValue
                    && x.FromDate <= utcNow && utcNow < x.ToDate.Value.AddDays(1))
                .Select(x => new AbsenceCampaignViewModel
                {
                    Id = x.Id,
                    Name = CampaignType.Asp.GetEnumDescription(),
                    SchoolYear = x.SchoolYear,
                    Month = x.Month,
                    FromDate = x.FromDate ?? default,
                    ToDate = x.ToDate ?? default,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    IsActive = true,
                    CampaignType = CampaignType.Asp,
                    IsRelatedInstitutionImportSigned = _userInfo.InstitutionID.HasValue
                        ? x.AspmonthlyBenefitInstitutions.Any(i => i.InstitutionId == _userInfo.InstitutionID.Value && i.IsSigned)
                        : false
                })
                .ToListAsync();

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

        public async Task<List<KeyValuePair<string, int>>> GetCampaignStats(int id)
        {
            var campaign = await _context.AspmonthlyBenefitsImports
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.SchoolYear,
                    x.Month
                })
                .SingleOrDefaultAsync();

            if (campaign == null) throw new ArgumentNullException(nameof(campaign));

            FormattableString queryString = $"select * from [student].[fn_ASPMonthlyBenefitReportDetails]({campaign.SchoolYear}, {campaign.Month})";
            IQueryable<ASPMonthlyBenefitReportDetailsDto> query = _context.Set<ASPMonthlyBenefitReportDetailsDto>().FromSqlInterpolated(queryString).AsNoTracking();

            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.SchoolDirector:
                case UserRoleEnum.School:
                case UserRoleEnum.KindergartenDirector:
                case UserRoleEnum.CPLRDirector:
                case UserRoleEnum.CSOPDirector:
                case UserRoleEnum.SOZDirector:
                case UserRoleEnum.InstitutionAssociate:
                    query = query.Where(x => x.CurrentInstitutionId == _userInfo.InstitutionID);
                    break;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.RuoExpert:
                    query = query.Where(x => x.CurrentInstitutionRegionId == _userInfo.RegionID);
                    break;
                case UserRoleEnum.Mon:
                case UserRoleEnum.MonExpert:
                case UserRoleEnum.ExternalExpert:
                case UserRoleEnum.CIOO:
                case UserRoleEnum.MonOBGUM:
                case UserRoleEnum.MonOBGUM_Finance:
                case UserRoleEnum.MonHR:
                case UserRoleEnum.Consortium:
                    // Виждат всичко
                    break;
                default:
                    // Останалите не виждата нищо
                    return null;
            }

            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>("asp.headers.comfirmed", await query.SumAsync(x => x.Confirmed)),
                new KeyValuePair<string, int>("asp.headers.rejected", await query.SumAsync(x => x.Rejected)),
                new KeyValuePair<string, int>("asp.headers.forReview", await query.SumAsync(x => x.Pending)),
                new KeyValuePair<string, int>("absenceReport.filter.all", await query.SumAsync(x => x.AllRecords))
            };

            return result;
        }

        public async Task SendNotification(AspmonthlyBenefitsImport entity)
        {
            try
            {
                await _signalRNotificationService.AspCampaignModified(entity.Id);
            }
            catch (Exception ex)
            {
                // Ignore
                _logger.LogError($"AspCampaign notification failed:{entity.Id}", ex);
            }
        }

        public async Task<MonSessionInfoViewModel> GetMonSession(short schoolYear, short month, string infoType)
        {
            DateTime targetMonth = Common.GetYearFromSchoolYear(schoolYear, month);
            var sessionInfo = await _aspContext.Mon2aspSessionInfos
                .Where(x => x.TargetMonth == targetMonth.Date && x.InfoType == infoType)
                .Select(x => new MonSessionInfoViewModel
                {
                    SessionNo = x.SessionNo,
                    TargetMonth = x.TargetMonth,
                    InfoType = x.InfoType,
                    AspProcessed = x.AspProcessed,
                    MonProcessed = x.EndDate

                }).FirstOrDefaultAsync();


            if (sessionInfo != null && sessionInfo.InfoType == SessionInfoTypeEnum.MonConfirmation.GetEnumDescription())
            {
                short aspSchoolYear = Common.GetSchoolYearFromYearMonth((short)sessionInfo.Year, (short)sessionInfo.Month);

                sessionInfo.ConfirmationRecordsCount = await _aspContext.MonConfs.CountAsync(x => x.SessionNo == sessionInfo.SessionNo);
            }

            return sessionInfo;
        }

        public async Task<IPagedList<AspMonConfirmViewModel>> GetMonConfirmsForCampaign(int sessionNo, PagedListInput input)
        {
            // Списък с подадените към АСП отсъствия е видим само за потребители, които могат да управляват кампании за отсъствия
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForASPImport))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<AspMonConfirmViewModel> query = _aspContext.MonConfs
                .Where(x => x.SessionNo == sessionNo)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.SchoolId.Contains(input.Filter)
                   || predicate.IdNumber.Contains(input.Filter)
                   || predicate.Name1.Contains(input.Filter)
                   || predicate.Name2.Contains(input.Filter)
                   || predicate.Name3.Contains(input.Filter)
                   || predicate.InfoTypeName.Contains(input.Filter)
                   || predicate.FlagConfName.Contains(input.Filter))
                .Select(x => new AspMonConfirmViewModel
                {
                    Id = x.Id,
                    IntYear = x.IntYear,
                    IntMonth = x.IntMonth,
                    InstitutionCode = x.SchoolId,
                    Pin = x.IdNumber,
                    PinType = x.IdTypeName,
                    FirstName = x.Name1,
                    MiddleName = x.Name2,
                    LastName = x.Name3,
                    NotExcused = x.NotExcused,
                    NotExcusedCorrection = x.PnotExcused,
                    Days = x.Days,
                    DaysCorrection = x.Pdays,
                    AspStatus = x.InfoTypeName,
                    MonStatus = x.FlagConfName
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id asc" : input.SortBy); ;

            int totalCount = await query.CountAsync();
            List<AspMonConfirmViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }
    }
}
