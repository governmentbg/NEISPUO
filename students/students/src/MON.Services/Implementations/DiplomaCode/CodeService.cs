namespace MON.Services.Implementations.DiplomaCode
{
    using Microsoft.EntityFrameworkCore;
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Models.Diploma.Import;
    using MON.Models.Interfaces;
    using MON.Services.Interfaces;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using System.Threading.Tasks;
    using System.Linq;
    using MON.Shared.Extensions;
    using System.Collections.Generic;
    using MON.Models.StudentModels;
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using MON.Shared.Enums;
    using MON.Models.Dynamic;
    using Newtonsoft.Json;
    using System.Dynamic;
    using Newtonsoft.Json.Converters;
    using MON.Models.Institution;
    using Newtonsoft.Json.Serialization;
    using MON.Services.Security.Permissions;
    using MON.Shared.Extensions.Utils;
    using MON.Models.Enums;
    using System.Text.RegularExpressions;
    using ZXing;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp;
    using MON.DataAccess;
    using System.Collections.Immutable;
    using MON.Report.Model;
    using System.Threading;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using System.Diagnostics;
    using MetadataExtractor;
    using System.IO;

    public class CodeService : BaseService<CodeService>, IScopedService, IDiplomaCode
    {
        private readonly ILookupService _lookupService;
        private readonly IAppConfigurationService _configurationService;
        private readonly IDiplomaImportValidationExclusionService _diplomaImportValidationExclusionService;
        private readonly IBarcodeService _barcodeService;
        private readonly IBarcodeYearService _barcodeYearService;
        private readonly IBasicDocumentService _basicDocumentService;
        private readonly IStudentService _studentService;
        private readonly IImageService _imageService;
        private readonly ILodAssessmentService _lodAssessmentService;
        private bool? _checkForNomNameMatch = null;
        private bool? _limitedSubjectCheck = null;
        private bool? _basicDocumentSequenceCheck = null;
        private bool? _validateDiplomaImages = null;
        private bool? _validateDiplomaBarcode = null;
        private HashSet<int> _basicDocumentSequenceValidationException = null;
        private string _basicDocumentSequenceCheckLocation = null;
        private const int MIN_IMAGE_DIMENSIONS = 1024;
        private List<int> PrimaryEducationBasicDocumentId = new List<int>() { 133, 142 };

        private List<ExternalEvaluationTypeModel> _externalEvaluationTypes;
        private List<SubjectTypeDropdownViewModel> _subjectTypes;
        private Dictionary<int, BasicDocumentDetailsModel> _basicDocumentsDetails = new Dictionary<int, BasicDocumentDetailsModel>();

        private Dictionary<int, List<BasicDocumentPartDetailsModel>> _basicDocumentParts = null;
        private Dictionary<int, List<DropdownViewModel>> _basicDocumentsOptions = null;
        private Dictionary<int, List<DropdownViewModel>> _ministriesOptions = null;
        private Dictionary<int, List<DropdownViewModel>> _eduFormsOptions = null;
        private Dictionary<int, List<DropdownViewModel>> _classTypesOptions = null;
        private Dictionary<int, List<DropdownViewModel>> _sppooProfessionOptions = null;
        private Dictionary<int, List<DropdownViewModel>> _sppooSpecialityOptions = null;
        private Dictionary<int, List<DropdownViewModel>> _eduTypesOptions = null;
        private Dictionary<int, List<DropdownViewModel>> _itLevelsOptions = null;
        private Dictionary<string, List<DropdownViewModel>> _flLevelsOptions = null;
        private Dictionary<string, List<DropdownViewModel>> _countriesOptions = null;



        protected readonly IServiceProvider _serviceProvider;

        private List<int> GradesCategories = new List<int>
        {
            3, // Годишна
            4, // Първа поправителна сесия
            5, // Втора поправителна сесия
            6, // Допълнителна поправителна сесия
        };

        protected List<int> BasicClassIds = new List<int>();

        /// <summary>
        /// Типове изпити за повишаване на оценка
        /// </summary>
        protected List<int> ReassessmentTypeIds = new List<int>();

        public CodeService(DbServiceDependencies<CodeService> dependencies,
            IServiceProvider serviceProvider)
        : base(dependencies)
        {
            _lookupService = serviceProvider.GetRequiredService<ILookupService>();
            _configurationService = serviceProvider.GetRequiredService<IAppConfigurationService>();
            _diplomaImportValidationExclusionService = serviceProvider.GetRequiredService<IDiplomaImportValidationExclusionService>();
            _barcodeService = serviceProvider.GetRequiredService<IBarcodeService>();
            _barcodeYearService = serviceProvider.GetRequiredService<IBarcodeYearService>();
            _studentService = serviceProvider.GetRequiredService<IStudentService>();
            _imageService = serviceProvider.GetRequiredService<IImageService>();
            _basicDocumentService = serviceProvider.GetRequiredService<IBasicDocumentService>();
            _lodAssessmentService = serviceProvider.GetRequiredService<ILodAssessmentService>();
        }

        #region Private members


        private bool CheckForNomNameMatch
        {
            get
            {
                if (_checkForNomNameMatch == null)
                {
                    string config = _configurationService.GetValueByKey("DiplomaImportNomNameCheck").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _checkForNomNameMatch = result;
                }

                return _checkForNomNameMatch.Value;
            }
        }

        private bool LimitedSubjectCheck
        {
            get
            {
                if (_limitedSubjectCheck == null)
                {
                    string config = _configurationService.GetValueByKey("DiplomaImportLimitedSubjectCheck").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _limitedSubjectCheck = result;
                }

                return _limitedSubjectCheck.Value;
            }
        }

        private bool BasicDocumentSequenceCheck
        {
            get
            {
                if (_basicDocumentSequenceCheck == null)
                {
                    string config = _configurationService.GetValueByKey("ValidateBasicDocumentSequence").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _basicDocumentSequenceCheck = result;
                }

                return _basicDocumentSequenceCheck.Value;
            }
        }

        private bool ValidateDiplomaImages
        {
            get
            {
                if (_validateDiplomaImages == null)
                {
                    string config = _configurationService.GetValueByKey("ValidateDiplomaImages").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _validateDiplomaImages = result;
                }

                return _validateDiplomaImages.Value;
            }
        }

        private bool ValidateDiplomaBarcode
        {
            get
            {
                if (_validateDiplomaBarcode == null)
                {
                    string config = _configurationService.GetValueByKey("ValidateDiplomaBarcode").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _validateDiplomaBarcode = result;
                }

                return _validateDiplomaBarcode.Value;
            }
        }

        private string BasicDocumentSequenceCheckLocation
        {
            get
            {
                if (_basicDocumentSequenceCheckLocation == null)
                {
                    _basicDocumentSequenceCheckLocation = _configurationService.GetValueByKey("BasicDocumentSequenceCheckLocation").Result;
                }

                return _basicDocumentSequenceCheckLocation;
            }
        }

        private HashSet<int> BasicDocumentSequenceValidationException
        {
            get
            {
                if (_basicDocumentSequenceValidationException == null)
                {
                    string config = _configurationService.GetValueByKey("BasicDocumentSequenceValidationException").Result;
                    if (config.IsNullOrWhiteSpace())
                    {
                        _basicDocumentSequenceValidationException = new HashSet<int>();
                    }
                    else
                    {
                        _basicDocumentSequenceValidationException = JsonConvert.DeserializeObject<HashSet<int>>(config ?? "");
                    }
                }

                return _basicDocumentSequenceValidationException;
            }
        }

        private async Task<ExternalEvaluationTypeModel> GetExternalEvaluationTypeDetails(int id)
        {
            if (_externalEvaluationTypes.IsNullOrEmpty())
            {
                _externalEvaluationTypes = await _lookupService.GetExternalEvaluationTypeOptions();
            }

            return _externalEvaluationTypes.SingleOrDefault(x => x.Id == id);
        }

        private async Task<SubjectTypeDropdownViewModel> GetSubjectTypeDetails(int? id)
        {
            if (_subjectTypes.IsNullOrEmpty())
            {
                _subjectTypes = await _lookupService.GetSubjectTypeOptions(null, null, true);
            }

            return _subjectTypes.SingleOrDefault(x => x.Value == id);
        }

        private async Task<BasicDocumentDetailsModel> GetBasicDocumentDetails(int? id)
        {
            if (!id.HasValue) return null;

            if (_basicDocumentsDetails == null) _basicDocumentsDetails = new Dictionary<int, BasicDocumentDetailsModel>();

            if (!_basicDocumentsDetails.ContainsKey(id.Value))
            {
                _basicDocumentsDetails[id.Value] = await _lookupService.GetBasicDocumentDetails(id.Value);
            }

            return _basicDocumentsDetails[id.Value];
        }

        private async Task<Dictionary<int, List<BasicDocumentPartDetailsModel>>> GetBasicDocumentParts()
        {
            _basicDocumentParts ??= (await _context.BasicDocumentParts
                    .Select(x => new BasicDocumentPartDetailsModel
                    {
                        Id = x.Id,
                        BasicDocumentId = x.BasicDocumentId,
                        SubjectTypesStr = x.SubjectTypesList,
                        ExternalEvaluationTypesStr = x.ExternalEvaluationTypesList,
                    })
                    .ToListAsync())
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _basicDocumentParts;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetBasicDocumentsOptions()
        {
            _basicDocumentsOptions ??= (await _lookupService.GetBasicDocumentOptions())
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _basicDocumentsOptions;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetMinistriesOptions()
        {
            _ministriesOptions ??= (await _lookupService.GetMinistryOptions())
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _ministriesOptions;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetEdutFormsOptions()
        {
            _eduFormsOptions ??= (await _lookupService.GetEducationFormDiplomaOptions(ValidEnum.All))
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _eduFormsOptions;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetClassTypesOptions()
        {
            _classTypesOptions ??= (await _lookupService.GetClassTypeOptions(ValidEnum.All))
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _classTypesOptions;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetSPPOOProfessionOptions()
        {
            _sppooProfessionOptions ??= (await _lookupService.GetSPPOOProfessionOptions(ValidEnum.All))
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _sppooProfessionOptions;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetSPPOOSpecialityOptions()
        {
            _sppooSpecialityOptions ??= (await _lookupService.GetSPPOOSpecialityOptions(ValidEnum.All))
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _sppooSpecialityOptions;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetEduTypesOptions()
        {
            _eduTypesOptions ??= (await _lookupService.GetDocumentEducationTypeOptions(ValidEnum.All))
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _eduTypesOptions;
        }

        private async Task<Dictionary<int, List<DropdownViewModel>>> GetItLevelsOptions()
        {
            _itLevelsOptions ??= (await _lookupService.GetITLevelOptions())
                    .GroupBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _itLevelsOptions;
        }

        private async Task<Dictionary<string, List<DropdownViewModel>>> GetFlLevelsOptions()
        {
            _flLevelsOptions ??= (await _lookupService.GetFLLevelOptions())
                    .GroupBy(x => x.Name.ToUpper())
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _flLevelsOptions;
        }

        private async Task<Dictionary<string, List<DropdownViewModel>>> GetCountriesOptions()
        {
            _countriesOptions ??= (await _lookupService.GetCountryOptions())
                    .GroupBy(x => x.Code.ToUpper())
                    .ToDictionary(x => x.Key, x => x.ToList());

            return _countriesOptions;
        }

        /// <summary>
        /// Извлича оценките за диплома и part
        /// </summary>
        /// <param name="diplomaId"></param>
        /// <param name="basicDocumentPartId"></param>
        /// <returns></returns>
        protected async Task<List<BasicDocumentTemplateSubjectModel>> GetSubjects(int diplomaId, int basicDocumentPartId)
        {
            var subjects = await (
                from ds in _context.DiplomaSubjects
                where ds.DiplomaId == diplomaId && ds.BasicDocumentPartId == basicDocumentPartId
                let subjectNom = _context.Subjects.FirstOrDefault(i => i.SubjectId == ds.SubjectId)
                select new BasicDocumentTemplateSubjectModel()
                {
                    SubjectId = ds.SubjectId,
                    SubjectName = ds.SubjectName,
                    SubjectTypeName = ds.SubjectType.Name,
                    ShowSubjectNamePreview = false == (ds.SubjectName ?? "").Equals((subjectNom.SubjectName ?? ""), StringComparison.OrdinalIgnoreCase),
                    SubjectCanChange = true,
                    Horarium = ds.Horarium,
                    Ects = ds.Ects,
                    FlHorarium = ds.FlHorarium,
                    FlLevel = ds.FlLevel,
                    FlSubjectId = ds.FlSubjectId,
                    FlSubjectName = ds.FlSubjectName,
                    ShowFlSubject = ds.FlSubjectId.HasValue,
                    Grade = ds.Grade,
                    GradeText = ds.GradeText,
                    NvoPoints = ds.Nvopoints,
                    Position = ds.Position,
                    SubjectTypeId = ds.SubjectTypeId,
                    GradeCategory = ds.GradeCategory,
                    OtherGrade = ds.OtherGrade,
                    QualitativeGrade = ds.QualitativeGrade,
                    SpecialNeedsGrade = ds.SpecialNeedsGrade,
                    BasicClassId = ds.BasicClassId
                }).ToListAsync();

            return subjects;
        }

        /// <summary>
        /// Добавя данните от диплома с тип basicDocumentId, Която е за този ученик, не е анулирана
        /// </summary>
        /// <param name="model"></param>
        /// <param name="basicDocumentId"></param>
        /// <returns></returns>
        protected async Task FillAdditionalDocuments(DiplomaCreateModel model, int basicDocumentId)
        {
            var targetDiploma =
                await (from d in _context.Diplomas.AsNoTracking()
                       where d.PersonId == model.PersonId && d.BasicDocumentId == basicDocumentId && !d.IsCancelled
                       select new
                       {
                           d.RegistrationDate,
                           d.RegistrationNumberTotal,
                           d.RegistrationNumberYear,
                           d.FactoryNumber,
                           d.Series,
                           d.InstitutionId,
                       }).FirstOrDefaultAsync();

            if (targetDiploma != null)
            {
                if (model.AdditionalDocuments == null) model.AdditionalDocuments = new List<DiplomaAdditionalDocumentModel>();
                var additionalDocument = new DiplomaAdditionalDocumentModel()
                {
                    BasicDocumentId = basicDocumentId,
                    RegistrationDate = targetDiploma.RegistrationDate,
                    RegistrationNumber = targetDiploma.RegistrationNumberTotal,
                    RegistrationNumberYear = targetDiploma.RegistrationNumberYear,
                    FactoryNumber = targetDiploma.FactoryNumber,
                    Series = targetDiploma.Series,
                    InstitutionId = targetDiploma.InstitutionId,
                };
                model.AdditionalDocuments.Add(additionalDocument);
            }
        }

        protected async Task<List<VStudentLodAsssessment>> GetLodAssessments(int personId)
        {
            List<VStudentLodAsssessment> lodAsssessments = await _context.VStudentLodAsssessments
                .Where(x => x.PersonId == personId
                    && x.CurriculumPartId.HasValue
                    && x.SubjectId.HasValue
                    && x.SubjectTypeId.HasValue
                    && x.GradeCategoryId.HasValue
                    && (x.ReassessmentTypeId.HasValue || (x.BasicClassId.HasValue && BasicClassIds.Contains(x.BasicClassId.Value))) // 
                    && GradesCategories.Contains(x.GradeCategoryId.Value))
                .ToListAsync();

            return lodAsssessments;
        }

        /// <summary>
        /// Пренареждане на модулите и добавяне, ако липсват, на подрубрики за съответните випуски.
        /// </summary>
        /// <returns></returns>
        private async Task<List<BasicDocumentTemplateSubjectModel>> ReorderModules(BasicDocumentTemplateSubjectModel basicDocumentSubject)
        {
            List<BasicDocumentTemplateSubjectModel> reorderedModules = new List<BasicDocumentTemplateSubjectModel>();

            if (basicDocumentSubject?.Modules?.IsNullOrEmpty() ?? true)
            {
                return reorderedModules;
            }

            int modulePosition = 1;
            foreach (var kvp in basicDocumentSubject.Modules.OrderBy(x => x.BasicClassId).GroupBy(x => x.BasicClassId))
            {
                BasicDocumentTemplateSubjectModel moduleSubheader = kvp.FirstOrDefault(x => x.SubjectTypeId == -1);

                if (modulePosition == 1 && moduleSubheader != null)
                {
                    // Сетване на първоначалната позиция.
                    // Ако има подрубрика взимамаме нейната позиция, инак започваме от едно(!!!задължително).
                    modulePosition = moduleSubheader.Position;
                }

                if (moduleSubheader != null)
                {
                    moduleSubheader.Position = modulePosition++;
                    reorderedModules.Add(moduleSubheader);
                }
                else
                {
                    string subHeaderTitile = kvp.Key.HasValue
                        ? $"Модули за {(await _context.BasicClasses.Where(x => x.BasicClassId == kvp.Key.Value).Select(x => x.RomeName).FirstOrDefaultAsync())} клас"
                        : null;

                    reorderedModules.Add(new BasicDocumentTemplateSubjectModel
                    {
                        TemplateId = basicDocumentSubject.TemplateId,
                        BasicDocumentPartId = basicDocumentSubject.BasicDocumentPartId,
                        SubjectName = subHeaderTitile,
                        SubjectNameShort = subHeaderTitile,
                        SubjectTypeName = "-",
                        BasicClassId = kvp.Key,
                        GradeCategory = -1,
                        Position = modulePosition++,
                        SubjectTypeId = -1,
                        Uid = new Guid().ToString(),
                        SubjectCanChange = true,
                        Modules = new List<BasicDocumentTemplateSubjectModel>()
                    });
                }

                foreach (BasicDocumentTemplateSubjectModel module in kvp.Where(x => x.SubjectTypeId.HasValue && x.SubjectTypeId != -1).OrderBy(x => x.Position))
                {
                    module.Position = modulePosition++;
                    reorderedModules.Add(module);
                }
            }


            return reorderedModules;
        }


        // Попълва оценки в секция за външно оценяване, само ако няма оценки
        public void FillExternalEvalGrades(DiplomaCreateModel model)
        {
            if (model.PersonId.HasValue)
            {
                foreach (BasicDocumentTemplatePartModel basicDocumentPart in model.BasicDocumentTemplate.Parts.Where(i => i.HasExternalEvaluationLimit))
                {
                    if (basicDocumentPart.Subjects.IsNullOrEmpty())
                    {
                        FillExternalEvalGrades(basicDocumentPart, model.PersonId.Value);
                    }
                }
            }
        }

        private void FillExternalEvalGrades(BasicDocumentTemplatePartModel basicDocumentPart, int personId)
        {

            var externalEvaluationGrades = _context.ExternalEvaluationItems.Where(i => basicDocumentPart.ExternalEvaluationTypes.Contains(i.ExternalEvaluation.ExternalEvaluationTypeId) && i.ExternalEvaluation.PersonId == personId).ToList();

            if (basicDocumentPart.Subjects == null)
            {
                basicDocumentPart.Subjects = new List<BasicDocumentTemplateSubjectModel>();
            }

            IEnumerable<BasicDocumentTemplateSubjectModel> basicDocumentSubjects = basicDocumentPart.Subjects
                .Where(x => x.SubjectId.HasValue && x.SubjectTypeId.HasValue);

            int position = basicDocumentSubjects.IsNullOrEmpty() ? 1 : basicDocumentSubjects.Max(x => x.Position) + 1;

            // Изключваме теория и практика на професията SubjectId = 93
            foreach (var externalEvaluationGrade in externalEvaluationGrades.Where(i => i.SubjectId != 93))
            {
                BasicDocumentTemplateSubjectModel basicDocumentTemplateSubjectModel = null;
                basicDocumentTemplateSubjectModel = basicDocumentSubjects.FirstOrDefault(i => i.SubjectId == externalEvaluationGrade.SubjectId);
                var ectsGrade = (basicDocumentPart.ShowEctsGrade && externalEvaluationGrade.Grade.HasValue) ? FunctionsExtension.GetEctsGrade(externalEvaluationGrade.Grade.Value) : null;
                if (basicDocumentTemplateSubjectModel != null)
                {
                    // Има го в шаблона, слагам оценка
                    basicDocumentTemplateSubjectModel.Grade = externalEvaluationGrade.Grade;
                    basicDocumentTemplateSubjectModel.NvoPoints = externalEvaluationGrade.Points;
                    basicDocumentTemplateSubjectModel.Ects = ectsGrade;
                    basicDocumentTemplateSubjectModel.FlLevel = externalEvaluationGrade.Fllevel;
                }
                else
                {
                    // Няма го в шаблона, но го има като оценка
                    BasicDocumentTemplateSubjectModel subject = new BasicDocumentTemplateSubjectModel
                    {
                        Uid = new Guid().ToString(),
                        BasicDocumentPartId = basicDocumentPart.Id.Value,
                        SubjectCanChange = true,
                        Position = position++,
                        SubjectId = externalEvaluationGrade.SubjectId,
                        SubjectName = externalEvaluationGrade.Subject,
                        SubjectTypeId = externalEvaluationGrade.SubjectTypeId,
                        Grade = externalEvaluationGrade.Grade,
                        NvoPoints = externalEvaluationGrade.Points,
                        FlLevel = externalEvaluationGrade.Fllevel,
                        Ects = ectsGrade
                        // BasicClassId = базов клас?
                    };

                    basicDocumentPart.Subjects.Add(subject);
                }
            }
        }

        private void FillBasicDocumentPartGrades(BasicDocumentTemplatePartModel basicDocumentPart, List<VStudentLodAsssessment> grades)
        {
            var gradesDict = grades.Where(x => x.SubjectTypeId.HasValue && x.SubjectId.HasValue
                   && basicDocumentPart.SubjectTypes.Contains(x.SubjectTypeId.Value)
                   && (basicDocumentPart.BasicClassId == null || basicDocumentPart.BasicClassId == x.BasicClassId))
                   .OrderBy(x => x.SortOrder)
                   .ThenBy(x => x.SubjectTypeId)
                   .ThenBy(x => x.SubjectId)
                   .GroupBy(x => new { SubjectId = x.SubjectId.Value, SubjectTypeId = x.SubjectTypeId.Value })
                   .ToDictionary(x => x.Key, x =>
                   {
                       var data = x.ToList();
                       // Ако е зададен конкретен клас, взимаме последната година, в която ученикът е бил в този клас
                       if (basicDocumentPart.BasicClassId.HasValue)
                       {
                           short? lastSchoolYear = data.Max(i => i.SchoolYear);
                           data = data.Where(i => i.SchoolYear == lastSchoolYear).ToList();
                       }
                       return data;
                   }
                   );

            if (gradesDict.Keys.Count == 0)
            {
                // Липсват оценки за дадения раздел
                return;
            }

            if (basicDocumentPart.Subjects == null)
            {
                basicDocumentPart.Subjects = new List<BasicDocumentTemplateSubjectModel>();
            }

            IEnumerable<BasicDocumentTemplateSubjectModel> basicDocumentSubjects = basicDocumentPart.Subjects
                .Where(x => x.SubjectId.HasValue && x.SubjectTypeId.HasValue);

            // Итерираме предметите, които идват от шаблона или базовия документ, ако няма шаблон.
            foreach (BasicDocumentTemplateSubjectModel basicDocumentSubject in basicDocumentSubjects)
            {
                // Оценките за дадения предмет. Сравняваме по SubjectId и SubjectTypeId.
                var assessmentsDictKey = new { SubjectId = basicDocumentSubject.SubjectId.Value, SubjectTypeId = basicDocumentSubject.SubjectTypeId.Value };
                if (gradesDict.ContainsKey(assessmentsDictKey))
                {
                    List<VStudentLodAsssessment> subjectLodGrades = gradesDict[assessmentsDictKey]; // Всички оценки по даден предмет и начин на изучаване
                    SetSubjectData(basicDocumentSubject, subjectLodGrades);

                    // Премахваме оценките от речника, тъх като сме ги обработили вече.
                    gradesDict.Remove(assessmentsDictKey);
                }

                // Обработваме модулите на предмета.
                if (!basicDocumentSubject.Modules.IsNullOrEmpty())
                {
                    List<BasicDocumentTemplateSubjectModel> basicDocumentSubjectModules = basicDocumentSubject.Modules
                        .Where(x => x.SubjectId.HasValue && x.SubjectTypeId.HasValue).ToList();

                    foreach (BasicDocumentTemplateSubjectModel basicDocumentSubjectModule in basicDocumentSubjectModules)
                    {
                        var moduleAssessmentsDictKey = new { SubjectId = basicDocumentSubjectModule.SubjectId.Value, SubjectTypeId = basicDocumentSubjectModule.SubjectTypeId.Value };
                        if (!gradesDict.ContainsKey(moduleAssessmentsDictKey))
                        {
                            // Липсват оценки за дадения модул и начин на изучаване
                            continue;
                        }

                        // При създаването на шаблон на документ определяме випускът за всеки един модул т.е. знаме basicClassId-то.
                        List<VStudentLodAsssessment> moduleLodGrades = gradesDict[moduleAssessmentsDictKey]
                            .Where(x => x.BasicClassId == basicDocumentSubjectModule.BasicClassId).ToList();

                        SetSubjectData(basicDocumentSubjectModule, moduleLodGrades);

                        // Премахваме използваните оценки.
                        foreach (var item in moduleLodGrades)
                        {
                            gradesDict[moduleAssessmentsDictKey].Remove(item);
                        }

                        // Ако за дадения модул няма вече оценки изтриваме ключа от речника.
                        if (gradesDict[moduleAssessmentsDictKey].Count == 0)
                        {
                            gradesDict.Remove(moduleAssessmentsDictKey);
                        }
                    }
                }
            }


            // Добавяме останалите оценки от ЛОД-а за предмети, който не са описани в шаблона на документа.
            if (gradesDict.Count > 0)
            {
                // Оценки на предмети, които не са модули
                var mainSubjectGrades = gradesDict.Where(x => !x.Value.Any(g => g.ParentCurriculumId.HasValue));
                var modulesSubjectGrades = gradesDict.Where(x => x.Value.Any(g => g.ParentCurriculumId.HasValue));

                int position = basicDocumentSubjects.IsNullOrEmpty() ? 1 : basicDocumentSubjects.Max(x => x.Position) + 1;

                // Оценки на предмети, които не са модули
                foreach (var kvp in mainSubjectGrades)
                {
                    //Добявяме предмета

                    // Взимаме профилиращите предмети
                    BasicDocumentTemplateSubjectModel subject = new BasicDocumentTemplateSubjectModel
                    {
                        Uid = new Guid().ToString(),
                        BasicDocumentPartId = basicDocumentPart.Id.Value,
                        SubjectCanChange = true,
                        Position = position++,
                        SubjectId = kvp.Key.SubjectId,
                        SubjectName = kvp.Value.FirstOrDefault()?.SubjectName,
                        SubjectTypeId = kvp.Key.SubjectTypeId,
                        BasicClassId = kvp.Value.FirstOrDefault()?.BasicClassId,
                        Modules = new List<BasicDocumentTemplateSubjectModel>()
                    };

                    SetSubjectData(subject, kvp.Value);

                    basicDocumentPart.Subjects.Add(subject);
                }

                // Оценки на модули
                foreach (var kvp in modulesSubjectGrades)
                {
                    List<VStudentLodAsssessment> moduleLodGrades = kvp.Value;
                    int moduleSubjectId = kvp.Key.SubjectId;
                    int moduleSubjectTypeId = kvp.Key.SubjectTypeId;
                    int? parentSubjectId = moduleLodGrades.FirstOrDefault(x => x.ParentSubjectId.HasValue)?.ParentSubjectId;
                    int? parentSubjectTypeId = moduleLodGrades.FirstOrDefault(x => x.ParentSubjectTypeId.HasValue)?.ParentSubjectTypeId;

                    BasicDocumentTemplateSubjectModel subject = basicDocumentPart.Subjects.FirstOrDefault(x => x.SubjectId.HasValue && x.SubjectTypeId.HasValue
                        && x.SubjectId.Value == parentSubjectId && x.SubjectTypeId.Value == parentSubjectTypeId);

                    if (subject == null)
                    {
                        // Не сме намерили проф. предмет. към който да закачим модула.
                        // Добавяме го като основен предмет.
                        // Това не е правилно, но ще ни посочи, че имаме грешки.
                        subject = new BasicDocumentTemplateSubjectModel
                        {
                            Uid = new Guid().ToString(),
                            BasicDocumentPartId = basicDocumentPart.Id.Value,
                            SubjectCanChange = true,
                            Position = position++,
                            SubjectId = moduleSubjectId,
                            SubjectName = moduleLodGrades.FirstOrDefault()?.SubjectName,
                            SubjectTypeId = moduleSubjectTypeId,
                            Modules = new List<BasicDocumentTemplateSubjectModel>()
                        };

                        SetSubjectData(subject, moduleLodGrades);
                        basicDocumentPart.Subjects.Add(subject);
                    }
                    else
                    {
                        // Намерили сме проф.предмет, към който да закачим модулите.
                        // Следва да ги групираме по basicClassid

                        foreach (var g in moduleLodGrades.Where(x => x.BasicClassId.HasValue).GroupBy(x => x.BasicClassId))
                        {
                            BasicDocumentTemplateSubjectModel module = new BasicDocumentTemplateSubjectModel
                            {
                                Uid = new Guid().ToString(),
                                BasicDocumentPartId = basicDocumentPart.Id.Value,
                                SubjectCanChange = true,
                                Position = 0,
                                SubjectId = moduleSubjectId,
                                SubjectName = g.FirstOrDefault()?.SubjectName,
                                SubjectTypeId = moduleSubjectTypeId,
                                BasicClassId = g.Key
                            };


                            subject.Modules.Add(module);
                            SetSubjectData(module, g.ToList());
                        }
                    }
                }
            }
        }

        #endregion

        public virtual async Task FillGrades(DiplomaCreateModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), nameof(DiplomaCreateModel));
            }

            if (!model.PersonId.HasValue || model.BasicDocumentTemplate == null || model.BasicDocumentTemplate.Parts.IsNullOrEmpty())
            {
                return;
            }

            List<VStudentLodAsssessment> grades = await GetLodAssessments(model.PersonId.Value);

            foreach (BasicDocumentTemplatePartModel basicDocumentPart in model.BasicDocumentTemplate.Parts)
            {
                if (basicDocumentPart.HasExternalEvaluationLimit)
                {
                    FillExternalEvalGrades(basicDocumentPart, model.PersonId.Value);
                }
                else
                {
                    await _lodAssessmentService.ModulesWithoutProfSubjectFixture(grades);
                    FillBasicDocumentPartGrades(basicDocumentPart, grades);

                    // Преподреждаме подрубриките и модулите в проф. предмети.
                    // Пресмятаме хорариумите и средните оценки на проф. предмети.
                    foreach (BasicDocumentTemplateSubjectModel subject in basicDocumentPart.Subjects)
                    {
                        if (!subject.Modules.IsNullOrEmpty())
                        {
                            subject.Modules = await ReorderModules(subject);

                            // Средно аритметично на средните оценки по години
                            var avgGrades = (
                                from m in subject.Modules
                                where m.Grade.HasValue
                                group m by m.BasicClassId into g
                                select new
                                {
                                    BasicClassId = g.Key,
                                    AvgGrade = g.Where(x => x.Grade != null && x.GradeCategory == (int)GradeCategoryEnum.Normal)
                                        .Average(x => x.Grade)
                                }).ToList();

                            decimal? avgGrade = avgGrades
                                .Where(i => i.AvgGrade != null)
                                .Select(i => Math.Round(i.AvgGrade.Value, 2, MidpointRounding.AwayFromZero))
                                .Average(x => (decimal?)x);

                            // Средно аритметично на всички оценки
                            // decimal ? avgGrade = subject.Modules.Where(x => x.Grade.HasValue).Average(x => x.Grade);
                            subject.Grade = avgGrade.HasValue
                                ? Math.Round(avgGrade.Value, 2, MidpointRounding.AwayFromZero)
                                : (decimal?)null;

                            subject.Horarium = subject.Modules.Sum(x => x.Horarium ?? 0);
                        }

                        // Няма дошла оценка от оценките и сме ФУЧ, слагам категория Друга оценка => Без оценка
                        if (subject.Grade == null && subject.OtherGrade == null && subject.SpecialNeedsGrade == null && subject.QualitativeGrade == null
                            && subject.GradeCategory != (int)GradeCategoryEnum.SubSection
                            && (
                                basicDocumentPart.Code == ((int)BasicDocumentPartCategoryEnum.ZIPNoProfPart).ToString()
                                || basicDocumentPart.Code == ((int)BasicDocumentPartCategoryEnum.OptionalPart).ToString()
                            ))
                        {
                            subject.GradeCategory = 3;
                            subject.OtherGrade = 34;
                        }
                    }
                }
            }
        }

        public virtual void CalcProfSubjectsHorarium(DiplomaCreateModel model)
        {
            if (model == null || model.BasicDocumentTemplate == null || model.BasicDocumentTemplate.Parts.IsNullOrEmpty())
            {
                return;
            }

            foreach (var subject in model.BasicDocumentTemplate.Parts.SelectMany(x => x.Subjects))
            {
                if (subject.Horarium.HasValue || subject.Modules.IsNullOrEmpty())
                {
                    continue;
                }

                subject.Horarium = subject.Modules.Where(m => m.Horarium.HasValue).Sum(m => m.Horarium);
            }
        }

        /// <summary>
        /// Изчислява годишната оценка.
        /// Добявя хорариумът и артикулите за изучаване на чужд език.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="lodGrades"></param>
        private void SetSubjectData(BasicDocumentTemplateSubjectModel subject, List<VStudentLodAsssessment> lodGrades)
        {
            if (subject == null || lodGrades.IsNullOrEmpty())
            {
                return;
            }

            // Взимаме оценките, които има BasicClassId
            var gradesByBasicClassDict = lodGrades.Where(x => x.BasicClassId.HasValue)
                .GroupBy(x => x.BasicClassId)
                .ToDictionary(x => x.Key, x => new
                {
                    Grades = x.ToList(),
                });

            List<(decimal? grade, int? specialNeedsGrade, int? otherGrade, int? qualitativeGrade, int gradeType, string gradeText, int? gradeId, int? basicClassId)> results =
                new List<(decimal? grade, int? specialNeedsGrade, int? otherGrade, int? qualitativeGrade, int gradeType, string gradeText, int? gradeId, int? basicClassId)>();

            int lodGradesHorarium = 0;
            foreach (var kvp in gradesByBasicClassDict)
            {
                (decimal? grade, int? specialNeedsGrade, int? otherGrade, int? qualitativeGrade, int gradeType, string gradeText, int? gradeId, int? basicClassId)? grade = CalcGrade(kvp.Value.Grades, kvp.Key);
                if (grade.HasValue)
                {
                    results.Add(grade.Value);
                }

                // Хорариуът за взима от нормалните оценки, а не от изпитите за промяна на годишна оценка
                lodGradesHorarium += kvp.Value.Grades.OrderBy(x => x.ReassessmentTypeId).First().AnnualHorarium ?? 0;
            }

            // Свидетелството позволява изпит за промяна на окончателна оценка за гимназиален етап
            if (ReassessmentTypeIds.Count > 0)
            {
                // Групираме по ReassessmentTypeId 
                var reassessmentsGroups = lodGrades.Where(x => !x.BasicClassId.HasValue && x.ReassessmentTypeId.HasValue && ReassessmentTypeIds.Contains(x.ReassessmentTypeId.Value))
                    .GroupBy(x => x.ReassessmentTypeId);
                foreach (var group in reassessmentsGroups)
                {
                    // За всяка група взимаме една оценка. Няма логика за два изпита за един и същ гимназиален етап.
                    (decimal? grade, int? specialNeedsGrade, int? otherGrade, int? qualitativeGrade, int gradeType, string gradeText, int? gradeId, int? basicClassId)? grade = CalcGrade(group.ToList(), null);
                    if (!grade.HasValue)
                    {
                        continue;
                    }

                    int[] basicClassIdsToRemove = group.Key switch
                    {
                        (int)ReassessmentTypeEnum.FirstHighSchoolStage => new[] { 8, 9, 10 },
                        (int)ReassessmentTypeEnum.SecondHighSchoolStage => new[] { 11, 12 },
                        _ => Array.Empty<int>(),
                    };

                    results = results.Where(x => !x.basicClassId.HasValue || !basicClassIdsToRemove.Contains(x.basicClassId.Value)).ToList();
                    results.Add(grade.Value);
                }
            }

            if (results.Count == 1)
            {
                subject.Grade = results[0].grade;
                subject.SpecialNeedsGrade = results[0].specialNeedsGrade;
                subject.OtherGrade = results[0].otherGrade;
                subject.QualitativeGrade = results[0].qualitativeGrade;
                subject.GradeCategory = results[0].gradeType; // GradeNom.GradeTypeId:  1 - Нормална оценка, 2 - СОП оценка, 3 - Друга оценка, 4 - Качествена 
                subject.GradeText = results[0].gradeText;
            }
            else if (results.All(x => x.gradeType == (int)GradeCategoryEnum.Normal) && results.Count() > 0)
            {
                decimal? avg = results.Where(x => x.grade.HasValue && x.gradeId >= 2 && x.gradeId <= 6).Average(x => x.grade);
                subject.Grade = avg.HasValue ? Math.Round(avg.Value, 2, MidpointRounding.AwayFromZero) : avg;
                subject.SpecialNeedsGrade = null;
                subject.OtherGrade = null;
                subject.GradeCategory = results.First().gradeType;
            }

            if (!subject.Horarium.HasValue)
            {
                // В шаблона не е описан хорариум
                subject.Horarium = lodGradesHorarium > 0 ? lodGradesHorarium : (int?)null;
            }

            if (!subject.FlSubjectId.HasValue)
            {
                // В шаблона не е описан FlSubjectId
                VStudentLodAsssessment lodSubjct = lodGrades.First();
                subject.FlSubjectId = lodSubjct.FlSubjectId;
                subject.FlSubjectName = lodSubjct.FlSubjectName;
                subject.ShowFlSubject = lodSubjct.FlSubjectId.HasValue;
            }

            if (!subject.FlHorarium.HasValue)
            {
                // В шаблона не е описан FlHorarium
                VStudentLodAsssessment lodSubjct = lodGrades.First();
                subject.FlHorarium = lodSubjct.FlHorarium;
            }
        }

        private (decimal? grade, int? specialNeedsGrade, int? otherGrade, int? qualitativeGrade, int gradeType, string gradeText, int? gradeId, int? basicClassId)? CalcGrade(List<VStudentLodAsssessment> grades, int? basicClassId)
        {
            // GradeId = 7 => Неявил се се филтрират

            // Търсим оценки от изпит за промяна на годишна оценка.
            // Ако има такива, взимаме първата, подредени по GradeCategoryId в низходящ ред
            VStudentLodAsssessment grade = grades.Where(x => x.GradeId != 7 &&
                (
                    (x.BasicClassId.HasValue && x.ReassessmentTypeId == (int)ReassessmentTypeEnum.AnualGrade)
                    ||
                    (!x.BasicClassId.HasValue && (x.ReassessmentTypeId == (int)ReassessmentTypeEnum.FirstHighSchoolStage || x.ReassessmentTypeId == (int)ReassessmentTypeEnum.SecondHighSchoolStage))
                ))
                .OrderByDescending(x => x.GradeCategoryId)
                .FirstOrDefault();

            // Не са намерени оценки от изпит за промяна на годишна оценка.
            // Търсим нормални годишни оценки.
            grade ??= grades.Where(x => x.GradeId != 7 && x.ReassessmentTypeId == null)
                .OrderByDescending(x => x.SchoolYear)
                .ThenByDescending(x => x.GradeCategoryId)
                .FirstOrDefault();

            if (grade == null)
            {
                return null;
            }

            // Освободен е нормална оценка в ЛОД-а, но за дипломите трябва да е Друга
            if (grade.GradeTypeId == (int)GradeCategoryEnum.Normal && grade.GradeId == 8)
            {
                return (grade: null, specialNeedsGrade: null, otherGrade: 32, qualitativeGrade: null, gradeType: (int)GradeCategoryEnum.Other, gradeText: grade.GradeText, gradeId: 32, basicClassId);
            }

            return grade.GradeTypeId switch
            {
                1 => (grade: (grade.DecimalGrade.HasValue ? grade.DecimalGrade.Value : (decimal?)grade.GradeId), specialNeedsGrade: null, otherGrade: null, qualitativeGrade: null, gradeType: grade.GradeTypeId ?? 1, gradeText: grade.GradeText, gradeId: grade.GradeId, basicClassId),
                2 => (grade: null, specialNeedsGrade: grade.GradeId, otherGrade: null, qualitativeGrade: null, gradeType: grade.GradeTypeId ?? 1, gradeText: grade.GradeText, gradeId: grade.GradeId, basicClassId),
                3 => (grade: null, specialNeedsGrade: null, otherGrade: grade.GradeId, qualitativeGrade: null, gradeType: grade.GradeTypeId ?? 1, gradeText: grade.GradeText, gradeId: grade.GradeId, basicClassId),
                4 => (grade: null, specialNeedsGrade: null, otherGrade: null, qualitativeGrade: grade.GradeId, gradeType: grade.GradeTypeId ?? 1, gradeText: grade.GradeText, gradeId: grade.GradeId, basicClassId),
                _ => null,
            };
        }

        public virtual Task AfterFinalization(int id)
        {
            return Task.CompletedTask;
        }

        public virtual Task FillAdditionalDocuments(DiplomaCreateModel model)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterFillGrades(DiplomaCreateModel model)
        {
            return Task.CompletedTask;
        }

        public virtual async Task<ApiValidationResult> CanCreate(int basicDocumentId, int personId)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            if (basicDocumentId <= 0 || personId <= 0)
            {
                validationResult.Errors.Add(new ValidationError("diplomas.incorrectInputParameters"));
            }

            if (!await _context.People.AnyAsync(x => x.PersonId == personId))
            {
                validationResult.Errors.Add(new ValidationError("errors.studentNotFound"));
            }

            return validationResult;
        }

        public virtual async Task<ApiValidationResult> ValidateImportModel(DiplomaImportModel importModel, CancellationToken cancellationToken = default)
        {
            ApiValidationResult validationResult = new ApiValidationResult();
            if (importModel == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Empty model: {nameof(DiplomaImportModel)}",
                    ID = importModel.FileName
                });
                return validationResult;
            }

            DiplomaImportParseModel model = importModel.ParseModel;
            if (model == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Empty model: {nameof(DiplomaImportParseModel)}",
                    ID = importModel.FileName
                });
                return validationResult;
            }

            DiplomaPerformaceStats stats = new DiplomaPerformaceStats
            {
                PersonalId = model.Person?.PersonalId
            };
            validationResult.DiplomaPerformaceStats.Add(stats);

            string errorId = $"{model.Person.FirstName} {model.Person.MiddleName} {model.Person.LastName}";
            string errorControlId = importModel.FileName;

            Stopwatch sw = new Stopwatch();

            sw.Start();
            validationResult.Merge(await ValidateStudent(model, errorControlId, errorId));
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "ValidateStudent",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            List<DiplomaImportValidationExclusionModel> exclusionList = await _diplomaImportValidationExclusionService.List();
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "GetExclusionList",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            if (exclusionList.Any(x => x.PersonalIdTypeId == model.Person.PersonalIdType && x.PersonalId == model.Person.PersonalId
                && x.Series == model.Document.Series && x.FactoryNumber == model.Document.FactoryNumber
                && (!x.InstitutonId.HasValue || x.InstitutonId.Value.ToString() == model.Institution.InstitutionCode)))
            {
                // Скипваме проверката поради въведено изключение
                return validationResult;
            }

            // Todo: Да се мисли за проверка за ученик записан в дадената институция, дали е завършил правилен basicClassId и дали има НВО/ДЗИ(за седно образование)
            validationResult.Merge(await ValidateInstitution(model, errorControlId, errorId, importModel.IsManualCreateOrUpdate, importModel.DiplomaId));
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "ValidateInstitution",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            validationResult.Merge(await ValidateDiplomaDocument(model, errorControlId, errorId, importModel.IsManualCreateOrUpdate, importModel.DiplomaId));
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "ValidateDiplomaDocument",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            validationResult.Merge(await ValidateNomenclatures(model, errorControlId, errorId));
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "ValidateNomenclatures",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            validationResult.Merge(await ValidateSubjects(model, errorControlId, errorId, importModel.IsManualCreateOrUpdate));
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "ValidateSubjects",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            validationResult.Merge(await ValidateEducation(model, errorControlId, errorId));
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "ValidateEducation",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            if (BasicDocumentSequenceCheck && !BasicDocumentSequenceValidationException.Any(x => x == model.Document.BasicDocumentType))
            {
                if (importModel.IsManualCreateOrUpdate)
                {
                    // При ръчно въвеждане/редактиране ще направим контрол на регистрационните номера,
                    // ако в конф. BasicDocumentSequenceCheckLocation в student.AppSettings таблицата липсва или е CreateOrUpdate
                    if (BasicDocumentSequenceCheckLocation.IsNullOrWhiteSpace()
                        || BasicDocumentSequenceCheckLocation.Equals("CreateOrUpdate", StringComparison.OrdinalIgnoreCase))
                    {
                        validationResult.Merge(await ValidateBasicDocumentSequence(model, errorControlId, errorId, importModel.DiplomaId, importModel.IsManualCreateOrUpdate));
                    }
                    else
                    {
                        validationResult.Merge(await ValidateBasicDocumentSequence(model, errorControlId, errorId, importModel.DiplomaId, importModel.IsManualCreateOrUpdate, true));
                    }
                }
                else
                {
                    // Контрол на регистрационните номера при импорт на диплома
                    validationResult.Merge(await ValidateBasicDocumentSequence(model, errorControlId, errorId, importModel.DiplomaId, importModel.IsManualCreateOrUpdate));
                }
            }
            ;
            sw.Stop();
            stats.PerformanceStats.Add(new PerformanceStats
            {
                Name = "BasicDocumentSequenceCheck",
                Duration = sw.ElapsedMilliseconds,
            });
            sw.Restart();

            //При ръчно въвеждане на диплома снимките се прикачват след създаването
            if (!importModel.IsManualCreateOrUpdate)
            {
                validationResult.Merge(await ValidateImages(model, errorControlId, errorId, cancellationToken));
                sw.Stop();
                stats.PerformanceStats.Add(new PerformanceStats
                {
                    Name = "ValidateImages",
                    Duration = sw.ElapsedMilliseconds,
                });
            }

            // Todo: Да се валидира секция AdditionalDocuments.

            return validationResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diploma"></param>
        /// <returns></returns>
        public async Task<ApiValidationResult> ValidateSignModel(DiplomaViewModel diploma)
        {
            ApiValidationResult validationResult = new ApiValidationResult();
            if (diploma == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Empty entity: {nameof(diploma)}",
                });

                return validationResult;
            }

            if (BasicDocumentSequenceCheck
                && (BasicDocumentSequenceCheckLocation ?? "").Equals("Sign", StringComparison.OrdinalIgnoreCase)
                && !BasicDocumentSequenceValidationException.Any(x => x == diploma.BasicDocumentId))
            {
                DiplomaImportParseModel diplomaImprtModel = new DiplomaImportParseModel
                {
                    Education = new DiplomaEducation { SchoolYear = diploma.SchoolYear },
                    Institution = new DiplomaInstitution { InstitutionCode = diploma.InstitutionId.ToString(), InstitutionName = diploma.InstitutionName },
                    Person = new DiplomaPerson { PersonalId = diploma.PersonalId, PersonalIdType = (short)diploma.PersonalIdType },
                    Document = new Models.Diploma.Import.DiplomaDocument
                    {
                        BasicDocumentType = diploma.BasicDocumentId,
                        RegNumber1 = diploma.RegistrationNumberTotal,
                        RegNumber2 = diploma.RegistrationNumberYear,
                        RegDate = diploma.RegistrationDate ?? default,
                        BasicDocumentIsRuoDoc = diploma.IsRuoDocBasicDocument,

                    },
                    AdditionalDocuments = diploma.AdditionalDocuments.Select(d => new MON.Models.Diploma.Import.DiplomaAdditionalDocument
                    {
                        BasicDocumentType = d.BasicDocumentId,
                        BasicDocumentTypeSpecified = d.BasicDocumentId.HasValue,
                        Series = d.Series,
                        FactoryNumber = d.FactoryNumber,
                        RegNumber1 = d.RegistrationNumber,
                        RegNumber2 = d.RegistrationNumberYear,
                        RegDate = d.RegistrationDate ?? default,
                    }).ToArray()
                };

                string errorId = $"{diploma.FirstName} {diploma.MiddleName} {diploma.LastName}";
                validationResult.Merge(await ValidateBasicDocumentSequence(diplomaImprtModel, "", errorId, diploma.Id, true));
                validationResult.Merge(await ValidateDiplomaAdditionalDocuments(diplomaImprtModel, "", errorId, null, diploma.Id, true, true));
            }

            return validationResult;
        }

        /// <summary>
        /// Валидация на секция Institution.
        /// 1. Проверка дали кодът по НЕИСПУО е число.
        /// 2. Проверка дали кодът по НЕИСПУО съвпада с този на логнатия потребител.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> ValidateInstitution(DiplomaImportParseModel model, string controlId, string id,
            bool isManualCreateOrUpdate = false, int? diplomaId = null)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            // Документът е за създаване от РУО. Няма институция. Използва се само при ръчно създаване/промяна на диплома.
            bool isRuoBasicDocument = isManualCreateOrUpdate && (model.Document?.BasicDocumentIsRuoDoc ?? false);

            if (!isRuoBasicDocument)
            {
                if (!int.TryParse(model?.Institution?.InstitutionCode ?? "", out int institutionId))
                {
                    validationResult.Errors.Add(new ValidationError()
                    {
                        Message = Messages.InvalidInstitutionCodeError,
                        ControlID = controlId,
                        ID = id,
                        Data = model.Institution.ToJson()
                    });

                    return validationResult;
                }

                if (_userInfo.InstitutionID.HasValue && _userInfo.InstitutionID.Value == institutionId)
                {
                    // Дипломата е за нашата институция
                    return validationResult;
                }
            }

            if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage)
                || await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForMonHrDiplomaManage))
            {
                // Пълни администраторски права
                return validationResult;
            }

            if (isRuoBasicDocument && await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRuoHrDiplomaManage))
            {
                // Документът е за създаване от РУО и имаме права
                return validationResult;
            }

            // Нищо от горното не е ОК. Връщаме грешка за невалидна институция.
            validationResult.Errors.Add(new ValidationError()
            {
                Message = Messages.UnauthorizedMessageError,
                ControlID = controlId,
                ID = id,
                Data = model.Institution?.ToJson()
            });

            return validationResult;
        }


        /// <summary>
        /// Валидация на секция Education.
        /// 1. Проверка дали SchoolYear е след датата на раждане на ученика.
        /// 2. Проверка дали YearGraduated е след датата на раждане на ученика
        /// </summary>
        /// <param name="model"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task<ApiValidationResult> ValidateEducation(DiplomaImportParseModel model, string controlId, string id)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), nameof(DiplomaImportParseModel));

            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null) return validationResult;

            if (model.Person != null)
            {
                var person = await _context.People.FirstOrDefaultAsync(x => x.PersonalIdtype == model.Person.PersonalIdType && x.PersonalId == model.Person.PersonalId);
                if (person != null)
                {
                    if (person.BirthDate.HasValue)
                    {
                        if (person.BirthDate.Value.Year > model.Education.SchoolYear)
                        {
                            validationResult.Errors.Add(new ValidationError()
                            {
                                Message = $"Учебната година (SchoolYear) {model.Education.SchoolYear} е недопустима спрямо рождената година на ученика {person.BirthDate.Value.Year}",
                                ControlID = controlId,
                                ID = id,
                                Data = ""
                            });
                        }

                        if (person.BirthDate.Value.Year > model.Education.YearGraduated)
                        {
                            validationResult.Errors.Add(new ValidationError()
                            {
                                Message = $"Годината на завършване (YearGraduated) {model.Education.YearGraduated} е недопустима спрямо рождената година на ученика {person.BirthDate.Value.Year}",
                                ControlID = controlId,
                                ID = id,
                                Data = ""
                            });
                        }
                    }
                }
                return validationResult;
            }

            return validationResult;
        }

        /// <summary>
        /// Валидация на секция Person.
        /// 1. Проверка дали съществува запис в core.Person с подадените PersonalId и PersonalIdType.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task<ApiValidationResult> ValidateStudent(DiplomaImportParseModel model, string controlId, string id)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), nameof(DiplomaImportParseModel));

            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null) return validationResult;

            if (model.Person == null)
            {
                validationResult.Errors.Add(new ValidationError()
                {
                    Message = $"Не е намерен ученик. Празен Person модел.",
                    ControlID = controlId,
                    ID = id,
                    Data = ""
                });

                return validationResult;
            }

            if (!await _context.People.AnyAsync(x => x.PersonalIdtype == model.Person.PersonalIdType && x.PersonalId == model.Person.PersonalId))
            {
                validationResult.Errors.Add(new ValidationError()
                {
                    Message = $"В НЕИСПУО не съществува лице с подадения идентификатор: {model.Person.PersonalId}! Моля да създадете нов ученик и след това да импортирате файла. Не е необходимо да записвате ученика в институцията!",
                    ControlID = controlId,
                    ID = id,
                    Data = model.Person.ToJson()
                });
            }

            return validationResult;
        }

        /// <summary>
        /// Контрола на регистрационен номер при запис/импорт на диплома
        /// </summary>
        /// <param name="model"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task<ApiValidationResult> ValidateBasicDocumentSequence(DiplomaImportParseModel model, string controlId,
            string id, int? diplomaId, bool isManualCreateOrUpdate, bool generateWarnings = false)
        {
            /*
            Във валидатора нa дипломи трябва да се проверява дали за BasicDocument-a, който се създава,
            комбинацията от RegNumberTotal+RegNumberYear+RegDate+InstitutionId за учебната година (SchoolYear, не YearGraduated) може да се намери в document.BasicDocumentSequence.
            Същото трябва да се прави при запис в OtherDocument таблицата. Там полетата RegNumberTotal, RegNumberYear и RegDate може да се казват по друг начин.
            Проверката НЕ трябва да се прилага за документи със SchoolYear < 2022 и такива, чиито BasicDocument е IsAppendix == true.
            Таблицата document.BasicDocumentSequence е променена/разширена. Освен за институция и учебна година съдържа и комбинации от Id на област и учебна година.
            Това е направено заради необходимостта РУО потребители да създават документ за приравняване.
            */

            ApiValidationResult validationResult = new ApiValidationResult();
            if (model == null) return validationResult;

            if (model.Education.SchoolYear < 2022)
            {
                return validationResult;
            }

            int basicDoumentId = model.Document?.BasicDocumentType ?? default;
            BasicDocumentDetailsModel basicDocument = await GetBasicDocumentDetails(basicDoumentId);
            // Проверката за RegNumber при запис/импорт на дипломи да се махне за IsAppendix = 1 #1157
            if (basicDocument?.IsAppendix == true)
            {
                return validationResult;
            }

            // Документът е за създаване от РУО. Няма институция. Използва се само при ръчно създаване/промяна на диплома.
            bool isRuoBasicDocument = isManualCreateOrUpdate && (model.Document?.BasicDocumentIsRuoDoc ?? false);

            bool validInstitutionId = int.TryParse(model.Institution?.InstitutionCode, out int institutionId);
            if (isRuoBasicDocument)
            {
                if (!_userInfo.RegionID.HasValue)
                {
                    ValidationError validationError = new ValidationError()
                    {
                        Message = Messages.InvalidRuoDistrictCodeError,
                        ControlID = controlId,
                        ID = id,
                        Data = model.Institution?.ToJson()
                    };

                    if (generateWarnings)
                    {
                        validationResult.Warnings.Add(validationError);
                    }
                    else
                    {
                        validationResult.Errors.Add(validationError);
                    }

                    return validationResult;
                }
            }
            else
            {
                if (!validInstitutionId)
                {
                    ValidationError validationError = new ValidationError()
                    {
                        Message = Messages.InvalidInstitutionCodeError,
                        ControlID = controlId,
                        ID = id,
                        Data = model.Institution?.ToJson()
                    };

                    if (generateWarnings)
                    {
                        validationResult.Warnings.Add(validationError);
                    }
                    else
                    {
                        validationResult.Errors.Add(validationError);
                    }

                    return validationResult;
                }
            }

            IQueryable<BasicDocumentSequence> sequencesQuery = _context.BasicDocumentSequences;

            if (isRuoBasicDocument)
            {
                sequencesQuery = sequencesQuery.Where(x => x.RegionId == _userInfo.RegionID && x.SchoolYear == model.Education.SchoolYear
                    && x.BasicDocumentId == model.Document.BasicDocumentType);
            }
            else
            {
                sequencesQuery = sequencesQuery.Where(x => x.InstitutionId == institutionId && x.SchoolYear == model.Education.SchoolYear
                    && x.BasicDocumentId == model.Document.BasicDocumentType);
            }

            var sequences = await sequencesQuery
                .Select(x => new
                {
                    x.RegNumberTotal,
                    x.RegNumberYear,
                    x.RegDate
                })
                .ToListAsync();

            int.TryParse(model.Document.RegNumber1, out int regNumberTotal);
            int.TryParse(model.Document.RegNumber2, out int regNumberYear);

            if (!sequences.Any(x => x.RegNumberTotal == regNumberTotal
                && x.RegNumberYear == regNumberYear
                && x.RegDate == model.Document.RegDate))
            {
                ValidationError validationError = new ValidationError()
                {
                    Message = $"Не е намерено съответствие между общия рег. номер ({regNumberTotal}), рег. номер за годината ({regNumberYear}) и датата на регистрация ({model.Document.RegDate:dd.MM.yyyy}) за учебната година на издаване на документа ({model.Education.SchoolYear}/{model.Education.SchoolYear + 1}) с такива, взети от НЕИСПУО за {model.Education.SchoolYear}/{model.Education.SchoolYear + 1} учебна година.",
                    ControlID = controlId,
                    ID = id,
                    Data = System.Text.Json.JsonSerializer.Serialize(model.Document)
                };
                if (generateWarnings)
                {
                    validationResult.Warnings.Add(validationError);
                }
                else
                {
                    validationResult.Errors.Add(validationError);
                }
            }

            if (!isRuoBasicDocument)
            {
                // Проверка за използвани вече номера в друг запис в OtherDocuments
                var existingOtherDocument = await _context.OtherDocuments
                    .Where(x => x.InstitutionId == institutionId && x.SchoolYear == model.Education.SchoolYear
                        && x.BasicDocumentId == model.Document.BasicDocumentType
                        && x.RegNumberTotal == model.Document.RegNumber1
                        && x.RegNumber == model.Document.RegNumber2
                        && x.IssueDate == model.Document.RegDate)
                    .Select(x => new
                    {
                        PersonName = x.Person.FirstName + " " + x.Person.MiddleName + " " + x.Person.LastName,
                        BasicDocumentName = x.BasicDocument.Name
                    })
                    .FirstOrDefaultAsync();

                if (existingOtherDocument != null)
                {
                    ValidationError validationError = new ValidationError()
                    {
                        Message = $"Рег.номер ({model.Document.RegNumber1} - {model.Document.RegNumber2} / {model.Document.RegDate:dd.MM.yyyy}) е използван за създаването на документ от тип '{existingOtherDocument.BasicDocumentName}' на '{existingOtherDocument.PersonName}'",
                        Data = System.Text.Json.JsonSerializer.Serialize(model.Document)
                    };

                    if (generateWarnings)
                    {
                        validationResult.Warnings.Add(validationError);
                    }
                    else
                    {
                        validationResult.Errors.Add(validationError);
                    }
                }
            }

            // Проверка за използвани вече номера в друг запис в Diplomas
            IQueryable<Diploma> diplomaQuery = _context.Diplomas;

            if (isRuoBasicDocument)
            {
                diplomaQuery = diplomaQuery.Where(x => x.RuoRegId == _userInfo.RegionID && x.SchoolYear == model.Education.SchoolYear
                    && x.BasicDocumentId == model.Document.BasicDocumentType);
            }
            else
            {
                diplomaQuery = diplomaQuery.Where(x => x.InstitutionId == institutionId && x.SchoolYear == model.Education.SchoolYear);
            }

            diplomaQuery = diplomaQuery
                .Where(x => x.BasicDocumentId == model.Document.BasicDocumentType
                    && x.RegistrationNumberTotal == model.Document.RegNumber1
                    && x.RegistrationNumberYear == model.Document.RegNumber2
                    && x.RegistrationDate == model.Document.RegDate);

            if (diplomaId.HasValue)
            {
                diplomaQuery = diplomaQuery.Where(x => x.Id != diplomaId.Value);
            }

            var existingDiploma = await diplomaQuery
                .Select(x => new
                {
                    PersonName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                    x.BasicDocumentName
                })
                .FirstOrDefaultAsync();

            if (existingDiploma != null)
            {
                ValidationError validationError = new ValidationError()
                {
                    Message = $"Рег.номер ({model.Document.RegNumber1} - {model.Document.RegNumber2} / {model.Document.RegDate:dd.MM.yyyy}) е използван за създаването на документ от тип '{existingDiploma.BasicDocumentName}' на '{existingDiploma.PersonName}'",
                    Data = System.Text.Json.JsonSerializer.Serialize(model.Document)
                };

                if (generateWarnings)
                {
                    validationResult.Warnings.Add(validationError);
                }
                else
                {
                    validationResult.Errors.Add(validationError);
                }
            }

            return validationResult;
        }

        private async Task<ApiValidationResult> ValidateSubjects(DiplomaImportParseModel model, string controlId, string id, bool isManualCreateOrUpdate = false)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), nameof(DiplomaImportParseModel));

            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null) return validationResult;

            string personalId = model.Person?.PersonalId;
            int personalIdType = model.Person?.PersonalIdType ?? default;
            var person = await _context.People.FirstOrDefaultAsync(x => x.PersonalIdtype == model.Person.PersonalIdType && x.PersonalId == model.Person.PersonalId);

            int basicDoumentId = model.Document?.BasicDocumentType ?? default;
            BasicDocumentDetailsModel basicDocument = await GetBasicDocumentDetails(basicDoumentId);

            if (model.Subjects.IsNullOrEmpty())
            {
                if (basicDocument?.HasSubjects ?? false)
                {
                    validationResult.Errors.Add($"Документ от тип: {basicDoumentId} {basicDocument?.Name} следва да има секция с оценки");
                }

                return validationResult;
            }

            // Todo: Да се мисли по писанията на Радосвета
            /*
                Кодовете и имената на предметите. Тези, които са SubjectID between 1 and 200, ще пристигат консистентно с еднакви имена.
                Обаче останалите вероянто ще идват с различни имена и с кодове, които няма да ги има в сегашното НЕИСПУО 
                (защото външните доставчици нямат синхронизирани номенклатури, а и да имат, те не са в десктоп-приложението).
                Възможно ли е, когато дойде предмет със  SubjectID>200, експорта да си вземе името от файла,
                и после да си потърси ID-то в номенклатурата на НЕИСПУО?
             */


            var basicSubjects = await _context.Subjects.Where(x => x.IsValid == true && x.SubjectId > 0 && x.SubjectId < 200)
                .Select(x => new
                {
                    x.SubjectId,
                    x.SubjectName,
                })
                .ToListAsync();

            Dictionary<int, List<BasicDocumentPartDetailsModel>> basicDocumentPartsDict = await GetBasicDocumentParts();

            foreach (var subject in model.Subjects)
            {
                List<string> errors = new List<string>();

                // Закоментирано по молба на Марги. МОН още не са решили какво искат!
                // 09.09.2024 rnikolov
                //if (!isManualCreateOrUpdate)
                //{
                //    // Проверка за задължително SubjectId, SubjectTypeId при импорт на диплома
                //    if (!subject.SubjectId.HasValue)
                //    {
                //        errors.Add($"Липсва стойност SubjectId за Subject (Предмет) с име:{subject.SubjectName}");
                //    }

                //    if (!subject.SubjectType1.HasValue)
                //    {
                //        errors.Add($"Липсва стойност SubjectId за Subject (Предмет) с ИД:{subject.SubjectId} и име:{subject.SubjectName}");
                //    }
                //}

                if (subject.SubjectName.IsNullOrWhiteSpace())
                {
                    errors.Add($"Липсва стойност SubjectName за Subject (Предмет) с ИД:{subject.SubjectId}");
                }

                if (subject.BasicDocumentPartSpecified && subject.BasicDocumentPart.HasValue)
                {
                    if (basicDocumentPartsDict.Count == 0 || !basicDocumentPartsDict.ContainsKey(subject.BasicDocumentPart.Value))
                    {
                        errors.Add($"Не е намерен BasicDocumentPart (секция) с ИД:{subject.BasicDocumentPart}");
                    }
                    else if (!basicDocumentPartsDict[subject.BasicDocumentPart.Value].Any(x => x.BasicDocumentId == basicDoumentId))
                    {
                        errors.Add($"Не е намерен BasicDocumentPart (секция) с ИД:{subject.BasicDocumentPart} за този тип документ {basicDoumentId}");
                    }
                }

                if (subject.SubjectIdSpecified && subject.SubjectId != 0)
                {
                    // При сетната конфигурационна настройка DiplomaImportLimitedSubjectCheck в student.AppSettings
                    // за ограничена по SubjectId проверка проверяваме кодовете 1 - 200.
                    if (false == LimitedSubjectCheck || (subject.SubjectId <= 200 && subject.SubjectId >= 1))
                    {
                        DropdownViewModel subjectDetails = await _lookupService.GetSubjectDetails(subject.SubjectId ?? default, ValidEnum.All);
                        if (subjectDetails == null)
                        {
                            errors.Add($"Не е намерен Subject (Предмет) с ИД:{subject.SubjectId}");
                        }
                        else
                        {
                            if (CheckForNomNameMatch && !subjectDetails.Name.Equals(subject.SubjectName, StringComparison.OrdinalIgnoreCase))
                            {
                                errors.Add($"Разлика в Subject (Предмет). Номенклатурна стойност: {subjectDetails.Name} / Подадена стойност: {subject.SubjectName}");
                            }
                        }
                    }
                }

                if (subject.GradeTypeSpecified && subject.GradeType.HasValue)
                {
                    // Валидиране на GradeType
                    var gradeTypeNom = await GetExternalEvaluationTypeDetails(subject.GradeType ?? default);
                    if (gradeTypeNom == null)
                    {
                        errors.Add($"Не е намерен GradeType (Вид оценка НВО/ДЗИ и т.н.) с ИД:{subject.GradeType}");
                    }

                    if (!basicDocument.DocumetPartsDetails.Any(x => x.ExternalEvaluationTypes != null
                            && x.ExternalEvaluationTypes.Any(e => e == subject.GradeType)))
                    {
                        errors.Add($"Не е намерена секция за оценка от тип с GradeType (Вид оценка НВО/ДЗИ и т.н.): {subject.GradeType}");
                    }

                    // Оценките от секция НВО/ ДЗИ трябва да съвпадат с подадените в системата, ако такива са подадени
                    if (person != null)
                    {
                        // Взимаме всички външни оценявания, които са постъпили при нас, защото може да има няколко за един тип
                        var externalEvaluations = await (
                            from eei in _context.ExternalEvaluationItems
                            where eei.ExternalEvaluation.PersonId == person.PersonId
                                && eei.ExternalEvaluation.ExternalEvaluationTypeId == subject.GradeType.Value
                                && eei.SubjectId == subject.SubjectId
                            select eei
                            ).ToListAsync();

                        // Проверка само, ако има данни в списъка за външните оценявания
                        if (externalEvaluations != null && externalEvaluations.Count() > 0 && subject.PointsSpecified && !externalEvaluations.Any(i => i.Points == subject.Points))
                        {
                            errors.Add($"Точките {subject.Points}, въведени за НВО/ДЗИ по предмет {subject.SubjectName}, не отговарят на служебно постъпилите в системата {String.Join(',', externalEvaluations.Select(i => i.Points).ToList())}");
                        }
                    }

                    // Ако е ДЗИ по чужд език, задължително да е въведено поле за FLLevel
                    if (subject.GradeTypeSpecified && subject.GradeType == (int)ExternalEvaluationTypeEnum.DZI)
                    {
                        // TODO - долното трябва да отиде в Diploma_3_44_Code тъй като е специфично за там
                        if (model.Document.BasicDocumentType == 253)
                        {
                            if (subject.SubjectIdSpecified)
                            {
                                bool isForeignLanguage = _context.Fls.Any(i => i.Flid == subject.SubjectId);
                                if (isForeignLanguage)
                                {
                                    if (String.IsNullOrWhiteSpace(subject.FlLevel))
                                    {
                                        errors.Add($"При ДЗИ по чужд език задължително трябва да е попълнено ниво на владеене");
                                    }
                                }
                            }

                            var allowedSubjectTypes = Enumerable.Union(GlobalConstants.SubjectTypesOfGeneralEduSubject, GlobalConstants.SubjectTypesOfProfileSubject);
                            if (!allowedSubjectTypes.Contains(subject.SubjectType1))
                            {
                                errors.Add($"При ДЗИ задължително трябва да e избран за вид на изпита някой от следните - ООП/ПП изб.модул/ ПП - 1/2/3/4 профилиращ предметза дипломи 3-44");
                            }
                        }

                        // TODO - в 3-34_Code
                        if (model.Document.BasicDocumentType == 35)
                        {
                            var allowedSubjectTypes = new List<int?>() { 1, 3, 101 };
                            if (!allowedSubjectTypes.Contains(subject.SubjectType1))
                            {
                                errors.Add($"При ДЗИ задължително трябва да e избран за вид на изпита някой от следните - ООП/ЗП/ЗПП за дипломи 3-34");
                            }
                        }
                    }
                }

                //Валидиране на FlLevel
                if (!String.IsNullOrWhiteSpace(subject.FlLevel))
                {
                    // Валидиране на FLLevel  
                    var flNom = (await _lookupService.GetFLLevelOptions())
                        .SingleOrDefault(x => x.Name.ToUpper() == subject.FlLevel.ToUpper());
                    if (flNom == null)
                    {
                        errors.Add($"Не е намерен FlLevel (Ниво на владеене на чужд език) с ИД:{subject.FlLevel}");
                    }
                }

                // Валидиране на SubjectType
                if (subject.SubjectType1 != null)
                {
                    var subjectTypeNom = await GetSubjectTypeDetails(subject.SubjectType1);
                    if (subjectTypeNom == null)
                    {
                        errors.Add($"Не е намерен SubjectType (Тип на предмета) с ИД:{subject.SubjectType1}");
                    }
                    else
                    {
                        if (subjectTypeNom.BasicSubjectTypeId == (int)BasicSubjectTypeEnum.CompulsoryCourses)
                        {
                            if (PrimaryEducationBasicDocumentId.Contains(model.Document.BasicDocumentType))
                            {
                                var existingBasicSubject = basicSubjects.FirstOrDefault(x => x.SubjectName.Equals(subject.SubjectName, StringComparison.OrdinalIgnoreCase));

                                if (!subject.SubjectId.HasValue && existingBasicSubject != null)
                                {
                                    errors.Add($"Предмет от ЗУЧ: '{subject.SubjectName}' трябва да има код");
                                }
                                else if ((subject.SubjectId >= 200 || subject.SubjectId <= 0) && existingBasicSubject != null)
                                {
                                    errors.Add($"В ЗУЧ е подаден предмет: '{subject.SubjectName}' с код: {subject.SubjectId} вместо с код {existingBasicSubject.SubjectId}");
                                }
                            }
                        }
                    }
                }

                // Валидация на SubjectType.
                if (subject.SubjectType1.HasValue)
                {
                    if (isManualCreateOrUpdate && subject.SubjectType1 == -1 && subject.Item is NoGradeType)
                    {
                        // Подрубриките при ръчно създаване/редактиране на диплома не трябва да се валидират.
                        // Те са със SubjectType1 == -1 и subject.Item от тип NoGradeType
                        // DiplomaCreateModel.cs line 486

                        // skip

                    }
                    else
                    {
                        if (!basicDocument.DocumetPartsDetails.Any(x => x.SubjectTypes != null
                                && x.SubjectTypes.Any(e => e == subject.SubjectType1)))
                        {
                            errors.Add($"Не е намерена секция за предмет с избрания начин на изучаване({subject.SubjectType1})");
                        }
                    }

                }

                if (errors.Any())
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = string.Join(" ; ", errors),
                        ControlID = controlId,
                        ID = id,
                        Data = subject.ToJson()
                    });
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Валидация на номенклатурите. Проверява дали съществуват номенклатури с подадените ID-та.
        /// При включена настройка (ключ DiplomaImportNomNameCheck в таблица student.AppSettings)
        /// проверява за съвпадение подадена текстова част на номенклатурата с тази в базата.
        /// Проверява елементи:
        /// 1. BasicDocumentType
        /// 2. BasicDocumentType
        /// 3. EducationForm
        /// 4. Profile
        /// 5. Profession
        /// 6. Speciality
        /// 7. Nationality
        /// </summary>
        /// <param name="model"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> ValidateNomenclatures(DiplomaImportParseModel model, string controlId, string id)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null) return validationResult;

            // Валидиране на BasicDocumentType
            Dictionary<int, List<DropdownViewModel>> basicDocumentsDict = await GetBasicDocumentsOptions();
            if (!basicDocumentsDict.ContainsKey(model.Document.BasicDocumentType))
            {
                validationResult.Errors.Add(new ValidationError($"Не е намерен BasicDocumentType с ИД:{model.Document.BasicDocumentType}", controlId, id));
            }
            else
            {
                var basicDocumentNomName = basicDocumentsDict[model.Document.BasicDocumentType].FirstOrDefault()?.Name ?? "";
                if (CheckForNomNameMatch && !basicDocumentNomName.Equals(model.Document.BasicDocumentTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    validationResult.Errors.Add(new ValidationError($"Разлика в BasicDocumentType. Номенклатурна стойност: {basicDocumentNomName} / Подадена стойност: {model.Document.BasicDocumentTypeName}", controlId, id));
                }
            }

            // Валидиране на Ministry
            Dictionary<int, List<DropdownViewModel>> ministriesDict = await GetMinistriesOptions();
            if (!ministriesDict.ContainsKey(model.Document.Ministry ?? 0))
            {
                validationResult.Errors.Add(new ValidationError($"Не е намерен Ministry с ИД:{model.Document.Ministry}", controlId, id));
            }
            else
            {
                var ministryNomName = ministriesDict[model.Document.Ministry ?? 0].FirstOrDefault()?.Name ?? "";
                if (CheckForNomNameMatch && !ministryNomName.Equals(model.Document.MinistryName, StringComparison.OrdinalIgnoreCase))
                {
                    validationResult.Errors.Add(new ValidationError($"Разлика в Ministry. Номенклатурна стойност: {ministryNomName} / Подадена стойност: {model.Document.MinistryName}", controlId, id));
                }
            }

            // Валидиране на EducationForm
            if (model.Education.EducationFormSpecified)
            {
                // TODO BasicDcoumentId = 265 e 3-19
                if (model.Education.EducationForm == -1 && model.Document.BasicDocumentType == 265)
                {
                    // Неприложимо
                }
                else
                {
                    Dictionary<int, List<DropdownViewModel>> eduFormsDict = await GetEdutFormsOptions();
                    if (!eduFormsDict.ContainsKey(model.Education.EducationForm ?? 0))
                    {
                        validationResult.Errors.Add(new ValidationError($"Не е намерен EducationForm с ИД:{model.Education.EducationForm}", controlId, id));
                    }
                    else
                    {
                        var eduFormNomName = eduFormsDict[model.Education.EducationForm ?? 0].FirstOrDefault()?.Name ?? "";
                        if (CheckForNomNameMatch && !eduFormNomName.Equals(model.Education.EducationFormName, StringComparison.OrdinalIgnoreCase))
                        {
                            validationResult.Errors.Add(new ValidationError($"Разлика в EducationForm. Номенклатурна стойност: {eduFormNomName} / Подадена стойност: {model.Education.EducationFormName}", controlId, id));
                        }
                    }
                }
            }

            // Валидиране на Profile (ClassType)
            if (model.Education.ProfileSpecified)
            {
                Dictionary<int, List<DropdownViewModel>> classTypesDict = await GetClassTypesOptions();
                if (!classTypesDict.ContainsKey(model.Education.Profile ?? 0))
                {
                    validationResult.Errors.Add(new ValidationError($"Не е намерен Profile с ИД:{model.Education.Profile}", controlId, id));
                }
                else
                {
                    string classTypeNomName = classTypesDict[model.Education.Profile ?? 0].FirstOrDefault()?.Name ?? "";
                    string classTypeNomText = classTypesDict[model.Education.Profile ?? 0].FirstOrDefault()?.Text ?? "";
                    if (CheckForNomNameMatch
                        && !classTypeNomName.Equals(model.Education.EducationFormName, StringComparison.OrdinalIgnoreCase)
                        && !!classTypeNomText.Equals(model.Education.EducationFormName, StringComparison.OrdinalIgnoreCase))
                    {
                        validationResult.Errors.Add(new ValidationError($"Разлика в Profile. Номенклатурна стойност: {classTypeNomName}, {classTypeNomText} / Подадена стойност: {model.Education.ProfileName}", controlId, id));
                    }
                }
            }

            if (model.Education.ProfessionSpecified)
            {
                // Валидиране на Profession 
                Dictionary<int, List<DropdownViewModel>> sppooProfessionDict = await GetSPPOOProfessionOptions();
                if (!sppooProfessionDict.ContainsKey(model.Education.Profession ?? 0))
                {
                    validationResult.Errors.Add(new ValidationError($"Не е намерен Profession с ИД:{model.Education.Profession}", controlId, id));
                }
                else
                {
                    string professionNomName = sppooProfessionDict[model.Education.Profession ?? 0].FirstOrDefault()?.Name ?? "";
                    if (CheckForNomNameMatch && !professionNomName.Equals(model.Education.ProfessionName, StringComparison.OrdinalIgnoreCase))
                    {
                        validationResult.Errors.Add(new ValidationError($"Разлика в Profession. Номенклатурна стойност: {professionNomName} / Подадена стойност: {model.Education.ProfessionName}", controlId, id));
                    }
                }
            }

            if (model.Education.SpecialitySpecified)
            {
                // Валидиране на Speciality
                Dictionary<int, List<DropdownViewModel>> sppooSpecialityDict = await GetSPPOOSpecialityOptions();
                if (!sppooSpecialityDict.ContainsKey(model.Education.Speciality ?? 0))
                {
                    validationResult.Errors.Add(new ValidationError($"Не е намерен Speciality с ИД:{model.Education.Speciality}", controlId, id));
                }
                else
                {
                    string specialityNomName = sppooSpecialityDict[model.Education.Speciality ?? 0].FirstOrDefault()?.Name ?? "";
                    if (CheckForNomNameMatch && !specialityNomName.Equals(model.Education.SpecialityName, StringComparison.OrdinalIgnoreCase))
                    {
                        validationResult.Errors.Add(new ValidationError($"Разлика в Speciality. Номенклатурна стойност: {specialityNomName} / Подадена стойност: {model.Education.SpecialityName}", controlId, id));
                    }
                }
            }

            if (model.Education.EducationTypeSpecified)
            {
                // Валидиране на EducationType  
                Dictionary<int, List<DropdownViewModel>> eduTypesDict = await GetEduTypesOptions();
                if (!eduTypesDict.ContainsKey(model.Education.EducationType ?? 0))
                {
                    validationResult.Errors.Add(new ValidationError($"Не е намерен EducationType с ИД:{model.Education.EducationType}", controlId, id));
                }
            }

            if (model.Education.ITLevelSpecified)
            {
                // Валидиране на ITLevel  
                Dictionary<int, List<DropdownViewModel>> itLevelsDict = await GetItLevelsOptions();
                if (!itLevelsDict.ContainsKey(model.Education.ITLevel ?? 0))
                {
                    validationResult.Errors.Add(new ValidationError($"Не е намерен ITLevel с ИД:{model.Education.ITLevel}. Възможните опции са: {string.Join(", ", itLevelsDict.Keys.ToArray())}", controlId, id));
                }
            }

            if (!String.IsNullOrWhiteSpace(model.Education.FLGELevel))
            {
                // Валидиране на FLGELevel
                Dictionary<string, List<DropdownViewModel>> flLevelsDict = await GetFlLevelsOptions();
                if (!flLevelsDict.ContainsKey((model.Education.FLGELevel ?? "").Trim().ToUpper()))
                {
                    validationResult.Errors.Add(new ValidationError($"Не е намерен FLGELevel с ИД:{model.Education.FLGELevel}. Възможните опции са: {string.Join(", ", flLevelsDict.Keys.ToArray())}", controlId, id));
                }
            }

            if (!String.IsNullOrEmpty(model.Person.Nationality))
            {
                // Валидиране на Nationality
                Dictionary<string, List<DropdownViewModel>> countriesDict = await GetCountriesOptions();
                string countryKey = (model.Person.Nationality ?? "").Trim().ToUpper();
                if (!countriesDict.ContainsKey(countryKey))
                {
                    validationResult.Errors.Add(new ValidationError($"Не е намерен Nationality с ИД:{model.Person.Nationality}", controlId, id));
                }
                else
                {
                    string countryNomName = countriesDict[countryKey].FirstOrDefault()?.Name ?? "";
                    if (CheckForNomNameMatch && !countryNomName.Equals(countryKey, StringComparison.OrdinalIgnoreCase))
                    {
                        validationResult.Errors.Add(new ValidationError($"Разлика в Nationality. Номенклатурна стойност: {countryNomName} / Подадена стойност: {countryKey}", controlId, id));
                    }
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Валидация на секция Document.
        /// 1. Проверка дали съществува документ с подадения тип (запис в diploma.BasicDocument с Id = Diploma.BasicDocumentType)
        /// 2. Проверка за уникалност на документ от даден тип за подадения ученик (ако diploma.BasicDocument е отбелязан, че е уникален, колона IsUniqueForStudent).
        /// 3. Проверява дали съществуват документи от даден тип, серия и фабричен номер за подадения ученик (ако diploma.BasicDocument е отбелязан, че е има фабричен номер, колона HasFactoryNumber).
        /// 4. Проверява, ако документът е приложение дали в AdditionalDocument е въведен документ с тази серия и сериен номер
        /// 5. Проверява, ако документът е дубликат дали в AdditionalDocument е въведен документ с тип, който не е дубликат
        /// </summary>
        /// <param name="model"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> ValidateDiplomaDocument(DiplomaImportParseModel model, string controlId, string id,
            bool isManualCreateOrUpdate = false, int? diplomaId = null)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null) return validationResult;

            if (!await CheckBasicDocumentType(model.Document, validationResult, controlId, id))
            {
                return validationResult;
            }

            string personalId = model.Person?.PersonalId;
            int personalIdType = model.Person?.PersonalIdType ?? default;

            int basicDoumentId = model.Document?.BasicDocumentType ?? default;
            BasicDocumentDetailsModel basicDocument = await GetBasicDocumentDetails(basicDoumentId);

            if (basicDocument == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"За ученик с идентификатор {personalId} не е намерен документ от вид: {basicDoumentId}.",
                    ControlID = controlId,
                    ID = id,
                });
            }

            var diplomas = await GetPersonDiplomas(personalId, personalIdType, isManualCreateOrUpdate, diplomaId);

            // BasicDocument-и за които е отбелязано, че за ученик може да има един документ от този тип и е подписан.
            if (basicDocument.IsUniqueForStudent == true && diplomas.Any(x => x.BasicDocumentId == model.Document?.BasicDocumentType))
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"За ученик с идентификатор {personalId} съществува документ от вид {model.Document?.BasicDocumentTypeName}. Позволен е само един документ от този вид.",
                    ControlID = controlId,
                    ID = id,
                });
            }

            // BasicDocument-и, за които е отбелязано, че имат серия е фабричен номер.
            // За даден ученик може да има един от този тип, серия и фабричен номер, който е подписан
            //2025-06-06 basicDocument.IsUniqueForStudent - workaround
            if (basicDocument.HasFactoryNumber == true && basicDocument.IsUniqueForStudent == true && diplomas.Any(x => x.BasicDocumentId == model.Document?.BasicDocumentType))
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"За ученик с идентификатор {personalId} съществува документ от вид {model.Document?.BasicDocumentTypeName}, " +
                    $"Серия: {model.Document?.Series} и Фабричен номер: {model.Document?.FactoryNumber}",
                    ControlID = controlId,
                    ID = id,
                });
            }

            // Проверка за формата на серията
            if (!string.IsNullOrWhiteSpace(basicDocument.SeriesFormat) && (basicDocument.IsDuplicate == false))
            {
                if (!string.IsNullOrWhiteSpace(model.Document.Series))
                {
                    string pattern = basicDocument.SeriesFormat.ToUpper().Replace("Y", @"\d");

                    if (!Regex.IsMatch(model.Document.Series.ToUpper().Trim(), pattern))
                    {
                        validationResult.Errors.Add(new ValidationError
                        {
                            Message = $"За документ {model.Document?.BasicDocumentTypeName ?? model.Document?.BasicDocumentType.ToString()} форматът на серията трябва да бъде {basicDocument?.SeriesFormat}. " +
                            $"Серията на текущия {model.Document.Series.ToUpper()} не отговаря на изискванията. Серията трябва да е изписана на кирилица.",
                            ControlID = controlId,
                            ID = id,
                        });
                    }
                }
                else
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"За документ {model.Document?.BasicDocumentTypeName ?? model.Document?.BasicDocumentType.ToString()} форматът на серията трябва да бъде {basicDocument?.SeriesFormat}, но за текущия документ липсва серия. Серията трябва да е изписана на кирилица.",
                        ControlID = controlId,
                        ID = id,
                    });
                }
            }

            // Проверка за серия и фабричен номер
            if (basicDocument.HasFactoryNumber == true)
            {
                if (string.IsNullOrWhiteSpace(model.Document.Series) || string.IsNullOrWhiteSpace(model.Document.FactoryNumber) || (model.Document.FactoryNumber != null && model.Document.FactoryNumber.Length != 6))
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"За документ {model.Document?.BasicDocumentTypeName ?? model.Document?.BasicDocumentType.ToString()} се изисква да бъдат попълнени серия и фабричен номер. В текущия документ един от двата атрибута липсва или фабричният номер не е 6 знака.",
                        ControlID = controlId,
                        ID = id,
                    });
                }
            }

            /*
             * https://github.com/Neispuo/students/issues/1377
             * ДФ 7 - връзка между вида на документа и вида на институцията
             * В BasicDocument да се добави поле DetailedSchoolTypes, което да носи информация кой документ може да се издава от институция с даден DetailedSchoolType. При създаване на нов документ в падащото меню за вид документ, да се показват само тези видове, които съответстват на вида на училището.
             * При импорт на документи от файл, да се сигнализира грешка, ако няма съответствие.
             */
            if (!isManualCreateOrUpdate && !basicDocument.DocumetPartsDetails.IsNullOrEmpty()
                && int.TryParse(model.Institution?.InstitutionCode, out int diplomaintstitutionId))
            {
                var diplomaInstitution = await GetInstitution(diplomaintstitutionId, null);
                var diplomaInstitutionDetailedSchoolTypeId = diplomaInstitution?.DetailedSchoolTypeId ?? 0;
                var diplomaInstitutionDetailedSchoolTypeName = diplomaInstitution?.DetailedSchoolTypeName;
                if (basicDocument.DetailedSchoolTypes != null && !basicDocument.DetailedSchoolTypes.Contains($"|{diplomaInstitutionDetailedSchoolTypeId}|"))
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"Ученик {personalId}: Институция с вид по чл.38 от ЗПУО \"{diplomaInstitutionDetailedSchoolTypeName}\" няма право да издава документ \"{basicDocument.Name}\".",
                        ControlID = controlId,
                        ID = id,
                    });
                }
            }

            validationResult.Merge(await ValidateDiplomaAdditionalDocuments(model, controlId, id, diplomas, diplomaId, isManualCreateOrUpdate, false));

            // BasicDocument-и, за които е отбелязано, че са Приложения, трябва да имат запис в AdditionalDocuments, който не е приложение/ приложение
            // Махаме проверката за IsAppendix
            //if (basicDocument?.IsAppendix == true)
            //{
            //    //if (!additionalBasicDocuments.Any(i => !i.IsAppendix)) // Todo: 
            //    if (!additionalBasicDocuments.Any())
            //    {
            //        validationResult.Errors.Add(new ValidationError
            //        {
            //            Message = $"За документ {model.Document?.BasicDocumentTypeName ?? model.Document?.BasicDocumentType.ToString()} трябва да се посочи основния документ",
            //            ControlID = controlId,
            //            ID = id,
            //        });
            //    }
            //    else
            //    {
            //        if (!model.AdditionalDocuments.Any(i => (i.Series ?? "").Equals(model.Document.Series ?? "", StringComparison.OrdinalIgnoreCase)
            //            && (i.FactoryNumber ?? "").Equals(model.Document.FactoryNumber ?? "", StringComparison.OrdinalIgnoreCase)))
            //        {
            //            validationResult.Errors.Add(new ValidationError
            //            {
            //                Message = $"Не е посочен правилният документ (серия и номер), към който е приложението",
            //                ControlID = controlId,
            //                ID = id,
            //            });
            //        }
            //    }
            //}

            return validationResult;
        }

        private async Task<ApiValidationResult> ValidateDiplomaAdditionalDocuments(DiplomaImportParseModel model, string controlId,
            string id, List<DiplomaViewModel> existiongDiplomas, int? diplomaId, bool isManualCreateOrUpdate = false, bool isSign = false)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null) return validationResult;

            string personalId = model.Person?.PersonalId;
            int personalIdType = model.Person?.PersonalIdType ?? default;
            int basicDoumentId = model.Document?.BasicDocumentType ?? default;
            BasicDocumentDetailsModel basicDocument = await GetBasicDocumentDetails(basicDoumentId);

            if (basicDocument == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"За ученик с идентификатор {personalId} не е намерен документ от вид: {basicDoumentId}.",
                    ControlID = controlId,
                    ID = id,
                });
            }

            existiongDiplomas ??= await GetPersonDiplomas(personalId, personalIdType, isManualCreateOrUpdate, diplomaId);

            // BasicDocument-и, за които е отбелязано, че имат оригинален документ, трябва да имат запис в AdditionalDocuments,
            // който не е дубликат/ приложение
            if (basicDocument.MainBasicDocumentsList.Count() > 0)
            {
                if (!(model.AdditionalDocuments ?? Array.Empty<Models.Diploma.Import.DiplomaAdditionalDocument>()).Any(i => i.BasicDocumentTypeSpecified && i.BasicDocumentType != null
                    && basicDocument.MainBasicDocumentsList.Contains(i.BasicDocumentType.Value)))
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"За документ {model.Document?.BasicDocumentTypeName ?? model.Document?.BasicDocumentType.ToString()} трябва да се посочи неговия оригинал",
                        ControlID = controlId,
                        ID = id,
                    });

                    return validationResult;
                }
            }

            /*
             * https://github.com/Neispuo/students/issues/1374
             * Връзка между дубликат и оригинален документ #1374
             *  При импорт на дипломи от файл – ако се импортира дубликат и той е IsIncludedInRegister = 1 
             *  и за лицето се намери оригинален документ в НЕИСПУО - подписан и неанулиран, 
             *  но реквизитите му (серия, фабричен номер, рег. номера и дата на регистрация) не съвпадат с подадените във файла, 
             *  да се сигнализира грешка и да се покажат като информация намерените, с които е направено сравнението.
             */

            // При подписване (за IsIncludedInRegister = 1) или запис (за IsIncludedInRegister = 0) или импорт (за IsIncludedInRegister = 1)
            if (((isSign && basicDocument.IsIncludedInRegister) || (isManualCreateOrUpdate && !basicDocument.IsIncludedInRegister) || (!isManualCreateOrUpdate && basicDocument.IsIncludedInRegister))
                && basicDocument.MainBasicDocumentsList.Count() > 0)
            {
                var mainDocuments = existiongDiplomas.Where(x => basicDocument.MainBasicDocumentsList.Contains(x.BasicDocumentId)).ToList();
                // ако няма намерени такива, не се прави нищо
                if (!model.AdditionalDocuments.Any(x => x.BasicDocumentTypeSpecified
                    && mainDocuments.Any(md => md.BasicDocumentId == x.BasicDocumentType.Value
                        && md.Series == x.Series && md.FactoryNumber == x.FactoryNumber
                        && md.RegistrationNumberTotal == x.RegNumber1 && md.RegistrationNumberYear == x.RegNumber2
                        && md.RegistrationDate.HasValue && md.RegistrationDate.Value.Date == x.RegDate.Date)))
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"За ученик с идентификатор {personalId} са намерени следните оригинални документи: {string.Join(";", mainDocuments.Select(x => $"{x.BasicDocumentName}, серия {x.Series}, фабричен номер {x.FactoryNumber}, рег.номер {x.RegistrationNumberTotal}-{x.RegistrationNumberYear}/{x.RegistrationDate?.ToString("dd.MM.yyyy")}"))}. " +
                        $"Въведените атрибути на свързанито документи не съвпадат с намерените.",
                        ControlID = controlId,
                        ID = id,
                    });
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Проверка дали съществува документ с подадения тип (запис в diploma.BasicDocument с Id = Diploma.BasicDocumentType).
        /// При конфигурационна настройка за проверка на съвпадение на имената сравнява подаденото Document.BasicDocumentTypeName
        /// с имено на BasicDocument намерен по подаденото ИД.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="validationResult"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private async Task<bool> CheckBasicDocumentType(Models.Diploma.Import.DiplomaDocument model, ApiValidationResult validationResult, string controlId, string id)
        {
            if (validationResult == null) throw new ArgumentNullException(nameof(validationResult));
            if (model == null) throw new ArgumentNullException(nameof(model), nameof(Models.Diploma.Import.DiplomaDocument));

            int basicDoumentId = model.BasicDocumentType;
            BasicDocumentDetailsModel basicDocument = await GetBasicDocumentDetails(basicDoumentId);
            if (basicDocument == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Непознат документ от тип: {model.BasicDocumentType}",
                    ControlID = controlId,
                    ID = id,
                });

                return false;
            }

            if (CheckForNomNameMatch && !model.BasicDocumentTypeName.IsNullOrWhiteSpace() && !model.BasicDocumentTypeName.Equals(basicDocument.Name, StringComparison.OrdinalIgnoreCase))
            {
                validationResult.Errors.Add($"Разлика в BasicDocumentTypeName. Номенклатурна стойност: {basicDocument.Name} / Подадена стойност: {model.BasicDocumentTypeName}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Валидация на секция Images.
        /// 1. Проверява броя на елементите(Image) дали удовлетворяват условитя описани в колоните AttachedImagesCountMin
        /// и AttachedImagesCountMin за дадения.
        /// 2. Проверка дали баркодовете съвпадат
        /// Особеното е, че при ръчно въвеждане на диплома снимките се прикачват след създаването.
        /// В този случай трябва да пропуснем тази валидация.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<ApiValidationResult> ValidateImages(DiplomaImportParseModel model, string controlId, string id, CancellationToken cancellationToken)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            if (model == null) return validationResult;

            int basicDocumentId = model.Document?.BasicDocumentType ?? default;
            BasicDocumentDetailsModel basicDocument = await GetBasicDocumentDetails(basicDocumentId);
            if (basicDocument == null)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Непознат документ от тип {basicDocumentId}",
                    ControlID = controlId,
                    ID = id,
                });
                return validationResult;
            }

            int imagesCount = model.Images != null ? model.Images.Where(x => !string.IsNullOrWhiteSpace(x.Contents)).Count() : 0;
            if (basicDocument.AttachedImagesCountMin.HasValue && imagesCount < basicDocument.AttachedImagesCountMin.Value)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Документ {basicDocumentId} {basicDocument?.Name} трябва да има минимум {basicDocument.AttachedImagesCountMin} сканирани страници.",
                    ControlID = controlId,
                    ID = id,
                });
            }

            if (basicDocument.AttachedImagesCountMax.HasValue && imagesCount > basicDocument.AttachedImagesCountMax.Value)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Документ {basicDocumentId} {basicDocument?.Name} може да има максимум {basicDocument.AttachedImagesCountMax} сканирани страници.",
                    ControlID = controlId,
                    ID = id,
                });
            }


            if (model.Images.IsNullOrEmpty())
            {
                return validationResult;
            }

            foreach (DiplomaImage image in model.Images.Where(x => !string.IsNullOrWhiteSpace(x.Contents)))
            {
                byte[] imageContents = null;
                try
                {
                    imageContents = Convert.FromBase64String(image.Contents);
                }
                catch (Exception ex)
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"Невалидно сканирано изображение на позиция: {image.Position}. {ex.GetInnerMostException().Message}",
                        ControlID = controlId,
                        ID = id,
                    });

                    continue;
                }

                Image attachedImage;
                try
                {
                    attachedImage = _imageService.LoadImage(imageContents, out IImageFormat imageFormat);

                    if (imageFormat.Name.Equals("JPEG", StringComparison.InvariantCultureIgnoreCase) || imageFormat.Name.Equals("PNG", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (attachedImage.Size().Width < MIN_IMAGE_DIMENSIONS && attachedImage.Size().Height < MIN_IMAGE_DIMENSIONS)
                        {
                            validationResult.Errors.Add(new ValidationError
                            {
                                Message = $"Прикаченият файл на позиция: {image.Position} е с твърде ниска резолюция. Необходимо е да е с размери поне {MIN_IMAGE_DIMENSIONS} пиксела по дългата страна!",
                                ControlID = controlId,
                                ID = id,
                            });
                            continue;
                        }
                    }
                    else
                    {
                        validationResult.Errors.Add(new ValidationError
                        {
                            Message = $"Позволено е прикачването на изображения (позиция: {image.Position}) само в JPEG или PNG формат!",
                            ControlID = controlId,
                            ID = id,
                        });
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"Прикаченото изображение (позиция: {image.Position}) е невалидно. Проверете отново файла! {ex.Message}",
                        ControlID = controlId,
                        ID = id,
                    });
                    continue;
                }

                if (ValidateDiplomaImages)
                {
                    validationResult.Merge(await ValidateImageDetails(imageContents, basicDocumentId, controlId, id, image.Position));
                }
                if (ValidateDiplomaBarcode)
                {
                    validationResult.Merge(await ValidateImageBarcodes(imageContents, model.Education.SchoolYear, basicDocumentId, basicDocument.HasBarcode, controlId, id, image.Position));
                }
            }

            return validationResult;
        }

        private async Task<decimal?> LoadDippkGrade(int? personId)
        {
            decimal? dippkGrade = await _context.ExternalEvaluations
                .Where(x => x.PersonId == personId && x.ExternalEvaluationTypeId == (int)ExternalEvaluationTypeEnum.Dippk)
                .SelectMany(x => x.ExternalEvaluationItems.Where(s => s.SubjectId == GlobalConstants.DippkSubjectId))
                .OrderByDescending(x => x.ExternalEvaluation.SchoolYear)
                .ThenByDescending(x => x.Id)
                .Select(x => x.Grade)
                .FirstOrDefaultAsync();

            // ДИППК оценката е възможно да е в секция ДЗИ
            decimal? dippkGradeFromDZI = await _context.ExternalEvaluations
                .Where(x => x.PersonId == personId && x.ExternalEvaluationTypeId == (int)ExternalEvaluationTypeEnum.DZI)
                .SelectMany(x => x.ExternalEvaluationItems.Where(s => s.SubjectId == GlobalConstants.DippkSubjectId))
                .Where(x => x.SubjectId == 93)
                .OrderByDescending(x => x.ExternalEvaluation.SchoolYear)
                .ThenByDescending(x => x.Id)
                .Select(x => x.Grade)
                .FirstOrDefaultAsync();

            return dippkGrade ?? dippkGradeFromDZI;

        }

        public async Task<ApiValidationResult> ValidateImageDetails(byte[] imageContents, int basicDocumentId, string controlId, string id, int? imagePosition)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            try
            {
                BasicDocumentDetailsModel basicDocument = await GetBasicDocumentDetails(basicDocumentId);

                var image = _imageService.LoadRGBImage(imageContents);
                var imageExif = _imageService.ExtractExifInformation(imageContents);
                var imageDetails = _imageService.AnalyseImage(image, imageExif);

                // Изтегляне на фабричния номер
                //if (basicDocument.HasFactoryNumber)
                //{
                //    string factoryNumber = _imageService.ExtractFactoryNumber(image);
                //}

                // Проверка дали изображението е в пълноцветна гама
                if (!imageDetails.HasColor)
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"Изображението на позиция {imagePosition} трябва да е сканирано пълноцветно",
                        ControlID = controlId,
                        ID = id,
                    });
                }

                if (basicDocument.PageOrientation != PageOrientationEnum.Unspecified)
                {
                    if (imageDetails.PageOrientation != basicDocument.PageOrientation)
                    {
                        validationResult.Errors.Add(new ValidationError
                        {
                            Message = $"Изображението на позиция {imagePosition} не е подадено в правилния портретен или пейзажен формат. Необходимо е да е във формат {basicDocument.PageOrientation.GetEnumDescription()}",
                            ControlID = controlId,
                            ID = id,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                validationResult.Errors.Add(new ValidationError
                {
                    Message = $"Възникна грешка при разчитането на изображението на позиция {imagePosition}. {ex.GetInnerMostException().Message}",
                    ControlID = controlId,
                    ID = id,
                });
            }

            return validationResult;
        }

        /// <summary>
        /// Проверка дали баркодът съвпада, ако за този документ се изисква
        /// </summary>
        /// <param name="imageContents"></param>
        /// <param name="schoolYear"></param>
        /// <param name="basicDocumentId"></param>
        /// <param name="hasBarcode"></param>
        /// <param name="controlId"></param>
        /// <param name="id"></param>
        /// <param name="imagePosition"></param>
        /// <returns></returns>
        public async Task<ApiValidationResult> ValidateImageBarcodes(byte[] imageContents, short schoolYear, int basicDocumentId,
            bool hasBarcode, string controlId, string id, int? imagePosition, CancellationToken cancellationToken = default)
        {
            ApiValidationResult validationResult = new ApiValidationResult();

            // В DiplomaBarcodeValidationInstitutionException на student.AppSettings може да добавим код на институция,
            // за която ще пропуснем проверката на баркодовете
            string barcodeValidationExceptionListStr = (await _configurationService.GetValueByKey("DiplomaBarcodeValidationInstitutionException")) ?? "";
            string[] splitStr = barcodeValidationExceptionListStr.Split("|", StringSplitOptions.RemoveEmptyEntries);
            HashSet<int> barcodeValidationInstitutionExceptionList = splitStr.ToHashSet<int>();

            if (hasBarcode
                && !barcodeValidationInstitutionExceptionList.Contains(_userInfo?.InstitutionID ?? int.MinValue))
            {
                // Използваме баркодове за тази и предишната година
                IEnumerable<BarcodeYearModel> currentYearBarcodes = await _barcodeYearService.GetBarcodesByYear(basicDocumentId, schoolYear);
                IEnumerable<BarcodeYearModel> lastYearBarcodes = await _barcodeYearService.GetBarcodesByYear(basicDocumentId, (short)(schoolYear - 1));
                var yearBarcodes = (currentYearBarcodes ?? new List<BarcodeYearModel>()).Concat(lastYearBarcodes ?? new List<BarcodeYearModel>());

                if (yearBarcodes.IsNullOrEmpty())
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"Не е описан и конфигуриран в НЕИСПУО баркод за {schoolYear} година.",
                        ControlID = controlId,
                        ID = id,
                    });

                    return validationResult;
                }

                try
                {
                    // Todo: _imageService.Resize да се забърза. Ако е възможно да приема Bitmap или Image, който сме създали още при първото извикване на Decode
                    Result result = await _barcodeService.DecodeTryHarderAsync(imageContents);

                    if (result == null || result.Text.IsNullOrWhiteSpace()
                        || !(yearBarcodes.Any(x => x.HeaderPage.PadLeft(result.Text.Length, '0').Equals(result.Text)) || yearBarcodes.Any(x => x.InternalPage.PadLeft(result.Text.Length, '0').Equals(result.Text))
                        ))
                    {
                        Result resultNew = await _barcodeService.DecodeTryHarderAsync(imageContents, new List<ZXing.BarcodeFormat> { BarcodeFormat.IMB });
                        result = resultNew;

                        if (result == null || !(yearBarcodes.Any(x => x.HeaderPage.PadLeft(result.Text.Length, '0').Equals(result.Text)) || yearBarcodes.Any(x => x.InternalPage.PadLeft(result.Text.Length, '0').Equals(result.Text))))
                        {
                            validationResult.Errors.Add(new ValidationError
                            {
                                Message = result == null
                                    ? $"Баркодът от изображение на позиция {imagePosition} не е разпознат!"
                                    : $"Баркодът от изображение на позиция {imagePosition} със стойност '{result.Text}' не съвпада с описаните в НЕИСПУО!",
                                ControlID = controlId,
                                ID = id,
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    validationResult.Errors.Add(new ValidationError
                    {
                        Message = $"Възникна грешка при разчитането на баркода. {ex.GetInnerMostException().Message}",
                        ControlID = controlId,
                        ID = id,
                    });
                }
            }

            return validationResult;
        }

        public virtual async Task<string> AutoFillDynamicContent(int basicDocumentId, int? personId, string contents)
        {
            string basicDocumentSchema = await _context.BasicDocuments
                .Where(x => x.Id == basicDocumentId)
                .Select(x => x.Contents)
                .SingleOrDefaultAsync();

            List<DynamicEntitySection> schemaModel = basicDocumentSchema.IsNullOrWhiteSpace()
                    ? new List<DynamicEntitySection>()
                    : JsonConvert.DeserializeObject<List<DynamicEntitySection>>(basicDocumentSchema);

            ExpandoObject model = contents.IsNullOrWhiteSpace()
                ? new ExpandoObject()
                : JsonConvert.DeserializeObject<ExpandoObject>(contents, new ExpandoObjectConverter());

            var person = await _context.People
                .Where(x => x.PersonId == personId)
                .Select(x => new
                {
                    x.PersonId,
                    PersonalIdType = x.PersonalIdtype.ToString(),
                    x.PersonalId,
                    x.BirthPlaceTownId,
                    x.FirstName,
                    x.MiddleName,
                    x.LastName,
                    BirthPlaceTown = x.BirthPlaceTown.Name,
                    BirthPlaceMunicipalityId = x.BirthPlaceTown.MunicipalityId,
                    BirthPlaceMunicipality = x.BirthPlaceTown.Municipality.Name,
                    BirthPlaceRegionId = x.BirthPlaceTown.Municipality.RegionId,
                    BirthPlaceRegion = x.BirthPlaceTown.Municipality.Region.Name,
                    x.NationalityId,
                    Nationality = x.Nationality.Name,
                    NationalityDropdown = x.NationalityId != null
                        ? new DropdownViewModel
                        {
                            Value = x.NationalityId ?? 0,
                            Text = x.Nationality.Name,
                            Name = x.Nationality.Name,
                            Code = x.Nationality.Code
                        }
                        : null,
                    Gender = x.Gender != null
                        ? new DropdownViewModel
                        {
                            Value = x.Gender ?? 0,
                            Text = x.GenderNavigation.Name,
                            Name = x.GenderNavigation.Name
                        }
                        : null
                })
                .SingleOrDefaultAsync();

            InstitutionDropdownViewModel institutiton = await GetInstitution(_userInfo.InstitutionID, person?.PersonId);
            StudentClassViewModel basicStudentClass = person != null ? await _studentService.GetCurrentClass(personId.Value) : null;
            decimal? dippkGrade = null;
            bool dippkLoaded = false;

            foreach (DynamicEntityItem field in schemaModel.SelectMany(x => x.Items))
            {
                switch (field.Id)
                {
                    case "firstName": //string
                        AddOrUpdate(model, field.Id, person?.FirstName);
                        break;
                    case "middleName": //string
                        AddOrUpdate(model, field.Id, person?.MiddleName);
                        break;
                    case "lastName": //string
                        AddOrUpdate(model, field.Id, person?.LastName);
                        break;
                    case "personalIdType": //string
                        AddOrUpdate(model, field.Id, person?.PersonalIdType);
                        break;
                    case "personalId": //string
                        AddOrUpdate(model, field.Id, person?.PersonalId);
                        AddOrUpdate(model, "personalIdType", person?.PersonalIdType);
                        if (person?.Gender != null)
                        {
                            AddOrUpdate(model, "gender", person.Gender);
                        }
                        break;
                    case "birthPlaceTown": //string town.name
                        AddOrUpdate(model, field.Id, person?.BirthPlaceTown);
                        break;
                    case "birthPlaceMunicipality": // string municipality.name
                        AddOrUpdate(model, field.Id, person?.BirthPlaceMunicipality);
                        break;
                    case "birthPlaceRegion": // string region.name
                        AddOrUpdate(model, field.Id, person?.BirthPlaceRegion);
                        break;
                    case "nationality": // DropdownViewModel
                        AddOrUpdate(model, field.Id, person?.NationalityDropdown);
                        break;
                    case "institution": // InstitutionDropdownViewModel
                        AddIfNotExists(model, field.Id, institutiton);
                        break;
                    case "institutionTown": // string region.name
                        AddIfNotExists(model, field.Id, institutiton?.Town);
                        break;
                    case "institutionLocalArea": // string localArea.Name
                        AddIfNotExists(model, field.Id, institutiton?.LocalArea);
                        break;
                    case "institutionMunicipality": // string region.name
                        AddIfNotExists(model, field.Id, institutiton?.Municipality);
                        break;
                    case "institutionRegion": // string region.name
                        AddIfNotExists(model, field.Id, institutiton?.Region);
                        break;
                    case "eduForm":
                        if (basicStudentClass != null)
                        {
                            var eduForm = (await _lookupService.GetEducationFormDiplomaOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == basicStudentClass.StudentEduFormId);
                            if (eduForm != null)
                            {
                                AddIfNotExists(model, field.Id, eduForm);
                            }
                        }
                        ;
                        break;
                    case "classType":
                        if (basicStudentClass != null)
                        {
                            var classType = (await _lookupService.GetClassTypeDiplomaOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == basicStudentClass.SelectedClassTypeId);
                            if (classType != null)
                            {
                                AddIfNotExists(model, field.Id, classType);
                            }
                        }
                        ;
                        break;
                    case "profession":
                    case "sppooProfession":
                        if (basicStudentClass != null)
                        {
                            var profession = (await _lookupService.GetSPPOOProfessionOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == basicStudentClass.StudentProfessionId);
                            if (profession != null)
                            {
                                AddIfNotExists(model, field.Id, profession);
                            }
                        }
                        ;
                        break;
                    case "speciality":
                    case "sppooSpeciality":
                        if (basicStudentClass != null)
                        {
                            var speciality = (await _lookupService.GetSPPOOSpecialityOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == basicStudentClass.StudentSpecialityId);
                            if (speciality != null)
                            {
                                AddIfNotExists(model, field.Id, speciality);
                            }
                        }
                        ;
                        break;
                    case "schoolYear":
                        if (basicStudentClass != null)
                        {
                            AddIfNotExists(model, field.Id, basicStudentClass.SchoolYear);
                        }
                        ;
                        break;
                    case "director":
                        if (basicStudentClass != null)
                        {
                            var director = await _context.Directors.FirstOrDefaultAsync(i => i.InstitutionId == _userInfo.InstitutionID && i.CurrentYearId == basicStudentClass.SchoolYear);
                            AddIfNotExists(model, field.Id, director != null ? $"{director.FirstName} {director.LastName}" : null);
                        }
                        break;
                    case "stateExamQualificationGradeText":
                        dippkGrade = await LoadDippkGrade(personId);
                        dippkLoaded = true;

                        if (dippkGrade.HasValue)
                        {
                            AddIfNotExists(model, field.Id, GradeUtils.GetDecimalGradeText(dippkGrade.Value));
                        }

                        break;
                    case "stateExamQualificationGrade":
                        if (!dippkLoaded)
                        {
                            dippkGrade = await LoadDippkGrade(personId);
                        }

                        if (dippkGrade.HasValue)
                        {
                            AddIfNotExists(model, field.Id, dippkGrade.Value);
                        }
                        break;
                    default:
                        break;
                }
            }

            // Зареждане на стойности по подразбиране описани в basicDocument - а
            foreach (DynamicEntityItem field in schemaModel.SelectMany(x => x.Items))
            {
                if (field.DefaultValue != null)
                {
                    AddIfNotExists(model, field.Id, field.DefaultValue);
                }
            }

            // Ministry е задължтелно при създаването на диплома. Възможно е да не е описано поле.
            // Да се прецени в този случай какво трябва да се прави, но за сега ще взимаме МОН.
            AddIfNotExists(model, "ministry", (await _lookupService.GetMinistryOptions()).FirstOrDefault());

            // Добавяне на номера
            //var nextSequence = await _basicDocumentService.GetNextBasicDocumentSequence(basicDocumentId);
            //AddIfNotExists(model, "registrationNumberTotal", nextSequence.RegNumberTotal);
            //AddIfNotExists(model, "registrationNumberYear", nextSequence.RegNumberYear);
            //AddIfNotExists(model, "registrationDate", nextSequence.RegDate.ToString("yyyy-MM-dd"));

            return model.ToJson(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public virtual async Task<string> AutoFillDynamicContent(int basicDocumentId, DiplomaImportParseModel diplomaModel, string contents)
        {
            string basicDocumentSchema = await _context.BasicDocuments
                .Where(x => x.Id == basicDocumentId)
                .Select(x => x.Contents)
                .SingleOrDefaultAsync();

            List<DynamicEntitySection> schemaModel = basicDocumentSchema.IsNullOrWhiteSpace()
                    ? new List<DynamicEntitySection>()
                    : JsonConvert.DeserializeObject<List<DynamicEntitySection>>(basicDocumentSchema);

            ExpandoObject model = contents.IsNullOrWhiteSpace()
                ? new ExpandoObject()
                : JsonConvert.DeserializeObject<ExpandoObject>(contents, new ExpandoObjectConverter());


            InstitutionDropdownViewModel institutiton = null;
            if (int.TryParse(diplomaModel.Institution?.InstitutionCode, out int intstitutionId))
            {
                institutiton = await GetInstitution(intstitutionId, null);
            }
            else
            {
                int? personId = await _context.People
                    .Where(x => x.PersonalIdtype == diplomaModel.Person.PersonalIdType && x.PersonalId == diplomaModel.Person.PersonalId)
                    .Select(x => x.PersonId)
                    .FirstOrDefaultAsync();
                institutiton = await GetInstitution(null, personId);
            }

            foreach (DynamicEntityItem field in schemaModel.SelectMany(x => x.Items))
            {
                switch (field.Id)
                {
                    case "firstName":
                        AddIfNotExists(model, field.Id, diplomaModel.Person.FirstName);
                        break;
                    case "firstNameLatin":
                        AddIfNotExists(model, field.Id, diplomaModel.Person.FirstNameLatin);
                        break;
                    case "middleName":
                        AddIfNotExists(model, field.Id, diplomaModel.Person.MiddleName);
                        break;
                    case "middleNameLatin":
                        AddIfNotExists(model, field.Id, diplomaModel.Person.MiddleNameLatin);
                        break;
                    case "lastName":
                        AddIfNotExists(model, field.Id, diplomaModel.Person.LastName);
                        break;
                    case "lastNameLatin":
                        AddIfNotExists(model, field.Id, diplomaModel.Person.LastNameLatin);
                        break;
                    case "personalIdType": //string
                        AddIfNotExists(model, field.Id, diplomaModel.Person.PersonalIdType);
                        break;
                    case "personalId": //string
                        AddIfNotExists(model, field.Id, diplomaModel.Person.PersonalId);
                        AddIfNotExists(model, "personalIdType", diplomaModel.Person.PersonalIdType);
                        if (Helper.ValidEGN(diplomaModel.Person.PersonalId))
                        {
                            int sexType = Helper.EgnToSexType(diplomaModel.Person.PersonalId);
                            AddIfNotExists(model, "gender", sexType - 1);
                        }
                        break;
                    case "birthPlaceTown": //string town.name
                        AddIfNotExists(model, field.Id, diplomaModel.Person.BirthPlaceTown);
                        break;
                    case "birthPlaceMunicipality": // string municipality.name
                        AddIfNotExists(model, field.Id, diplomaModel.Person.BirthPlaceMunicipality);
                        break;
                    case "birthPlaceRegion": // string region.name
                        AddIfNotExists(model, field.Id, diplomaModel.Person.BirthPlaceRegion);
                        break;
                    case "nationality":
                        var country = await _context.Countries.Where(i => i.Code == diplomaModel.Person.Nationality)
                            .Select(
                            x => new DropdownViewModel
                            {
                                Value = x.CountryId,
                                Text = x.Name,
                                Name = x.Name,
                                Code = x.Code
                            }).FirstOrDefaultAsync();
                        AddIfNotExists(model, field.Id, country);
                        break;
                    case "institution": // InstitutionDropdownViewModel
                        AddIfNotExists(model, field.Id, institutiton);
                        break;
                    case "institutionTown": // string region.name
                        AddIfNotExists(model, field.Id, institutiton?.Town);
                        break;
                    case "institutionMunicipality": // string region.name
                        AddIfNotExists(model, field.Id, institutiton?.Municipality);
                        break;
                    case "institutionRegion": // string region.name
                        AddIfNotExists(model, field.Id, institutiton?.Region);
                        break;
                    case "institutionLocalArea": // string localArea.Name
                        AddIfNotExists(model, field.Id, institutiton?.LocalArea);
                        break;
                    case "eduForm":
                        if (diplomaModel.Education.EducationFormSpecified)
                        {
                            var eduForm = (await _lookupService.GetEducationFormDiplomaOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == diplomaModel.Education.EducationForm);
                            if (eduForm != null)
                            {
                                AddIfNotExists(model, field.Id, eduForm);
                            }
                        }
                        ;
                        break;
                    case "classType":
                        if (diplomaModel.Education.ProfileSpecified)
                        {
                            var classType = (await _lookupService.GetClassTypeDiplomaOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == diplomaModel.Education.Profile);
                            if (classType != null)
                            {
                                AddIfNotExists(model, field.Id, classType);
                            }
                        }
                        ;
                        break;
                    case "sppooProfession":
                        if (diplomaModel.Education.ProfessionSpecified)
                        {
                            var profession = (await _lookupService.GetSPPOOProfessionOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == diplomaModel.Education.Profession);
                            if (profession != null)
                            {
                                AddIfNotExists(model, field.Id, profession);
                            }
                        }
                        ;
                        break;
                    case "sppooSpeciality":
                        if (diplomaModel.Education.SpecialitySpecified)
                        {
                            var speciality = (await _lookupService.GetSPPOOSpecialityOptions(ValidEnum.All)).FirstOrDefault(i => i.Value == diplomaModel.Education.Speciality);
                            if (speciality != null)
                            {
                                AddIfNotExists(model, field.Id, speciality);
                            }
                        }
                        ;
                        break;
                    case "series":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.Series);
                        break;
                    case "factoryNumber":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.FactoryNumber);
                        break;
                    case "registrationNumberTotal":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.RegNumber1);
                        break;
                    case "registrationNumberYear":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.RegNumber2);
                        break;
                    case "registrationDate":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.RegDate);
                        break;
                    case "director":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.Principal);
                        break;
                    case "deputy":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.Deputy);
                        break;
                    case "leadTeacher":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.LeadTeacher);
                        break;
                    case "gpa":
                        if (diplomaModel.Education.GpaSpecified)
                        {
                            AddIfNotExists(model, field.Id, diplomaModel.Education.Gpa);
                        }
                        break;
                    case "gpaText":
                        AddIfNotExists(model, field.Id, diplomaModel.Education.GpaText);
                        break;
                    case "stateExamQualificationGrade":
                        if (diplomaModel.Education.StateExamQualificationGradeSpecified)
                        {
                            AddIfNotExists(model, field.Id, diplomaModel.Education.StateExamQualificationGrade);
                        }
                        break;
                    case "stateExamQualificationGradeText":
                        AddIfNotExists(model, field.Id, diplomaModel.Education.StateExamQualificationGradeText);
                        break;
                    case "schoolYear":
                        AddIfNotExists(model, field.Id, diplomaModel.Education.SchoolYear);
                        break;
                    case "yearGraduated":
                        AddIfNotExists(model, field.Id, diplomaModel.Education.YearGraduated);
                        break;
                    case "flLevel":
                        AddIfNotExists(model, field.Id, diplomaModel.Education.FLGELevel);
                        break;
                    case "itLevel":
                        if (diplomaModel.Education.ITLevelSpecified)
                        {
                            AddIfNotExists(model, field.Id, diplomaModel.Education.ITLevel);
                        }
                        break;
                    case "eduDuration":
                        if (diplomaModel.Education.DurationSpecified)
                        {
                            AddIfNotExists(model, field.Id, diplomaModel.Education.Duration);
                        }
                        break;
                    case "protocolNumber":
                        AddIfNotExists(model, field.Id, diplomaModel.Document.ProtocolNumber);
                        break;
                    case "protocolDate":
                        if (diplomaModel.Document.ProtocolDateSpecified)
                        {
                            AddIfNotExists(model, field.Id, diplomaModel.Document.ProtocolDate);
                        }
                        break;
                    case "description":
                        AddIfNotExists(model, field.Id, diplomaModel.Education.Description);
                        break;
                    case "nkr":
                        if (diplomaModel.Education.NKRSpecified)
                        {
                            AddIfNotExists(model, field.Id, diplomaModel.Education.NKR);
                        }
                        break;
                    case "ekr":
                        if (diplomaModel.Education.EKRSpecified)
                        {
                            AddIfNotExists(model, field.Id, diplomaModel.Education.EKR);
                        }
                        break;
                    default:
                        break;
                }
            }

            // Зареждане на стойности по подразбиране описани в basicDocument-а
            foreach (DynamicEntityItem field in schemaModel.SelectMany(x => x.Items))
            {
                if (field.DefaultValue != null)
                {
                    AddIfNotExists(model, field.Id, field.DefaultValue);
                }
            }

            // Ministry е задължтелно при създаването на диплома. Възможно е да не е описано поле.
            // Да се прецени в този случай какво трябва да се прави, но за сега ще взимаме МОН.
            AddIfNotExists(model, "ministry", (await _lookupService.GetMinistryOptions()).FirstOrDefault());

            return model.ToJson(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public virtual async Task<string> FixDynamicContent(int diplomaId, string contents)
        {
            var diploma = await (
                from d in _context.Diplomas
                where d.Id == diplomaId
                select new
                {
                    d.BasicDocumentId,
                    d.YearGraduated,
                    d.FlgelevelId,
                    FLGELevelName = d.Flgelevel.Name,
                    d.ItlevelId
                }).FirstOrDefaultAsync();

            if (diploma == null)
            {
                return contents;
            }
            string basicDocumentSchema = await _context.BasicDocuments
                .Where(x => x.Id == diploma.BasicDocumentId)
                .Select(x => x.Contents)
                .SingleOrDefaultAsync();

            List<DynamicEntitySection> schemaModel = basicDocumentSchema.IsNullOrWhiteSpace()
                    ? new List<DynamicEntitySection>()
                    : JsonConvert.DeserializeObject<List<DynamicEntitySection>>(basicDocumentSchema);

            ExpandoObject model = contents.IsNullOrWhiteSpace()
                ? new ExpandoObject()
                : JsonConvert.DeserializeObject<ExpandoObject>(contents, new ExpandoObjectConverter());

            foreach (DynamicEntityItem field in schemaModel.SelectMany(x => x.Items))
            {
                switch (field.Id)
                {
                    case "yearGraduated":
                        if (diploma.YearGraduated.HasValue)
                        {
                            AddIfNotExists(model, field.Id, diploma.YearGraduated.Value);
                        }

                        break;
                    case "flLevel":
                        if (diploma.FlgelevelId.HasValue)
                        {
                            AddIfNotExists(model, field.Id, diploma.FLGELevelName);
                        }

                        break;
                    case "itLevel":
                        if (diploma.ItlevelId.HasValue)
                        {
                            AddIfNotExists(model, field.Id, diploma.ItlevelId);
                        }

                        break;

                    default:
                        break;
                }
            }

            return model.ToJson(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public virtual async Task<string> FillDippkDynamicContent(int basicDocumentId, int? personId, string contents)
        {
            string basicDocumentSchema = await _context.BasicDocuments
                .Where(x => x.Id == basicDocumentId)
                .Select(x => x.Contents)
                .SingleOrDefaultAsync();

            List<DynamicEntitySection> schemaModel = basicDocumentSchema.IsNullOrWhiteSpace()
                    ? new List<DynamicEntitySection>()
                    : JsonConvert.DeserializeObject<List<DynamicEntitySection>>(basicDocumentSchema);

            ExpandoObject model = contents.IsNullOrWhiteSpace()
                ? new ExpandoObject()
                : JsonConvert.DeserializeObject<ExpandoObject>(contents, new ExpandoObjectConverter());

            decimal? dippkGrade = null;
            bool dippkLoaded = false;

            foreach (DynamicEntityItem field in schemaModel.SelectMany(x => x.Items))
            {
                switch (field.Id)
                {
                    case "stateExamQualificationGradeText":
                        dippkGrade = await LoadDippkGrade(personId);
                        dippkLoaded = true;

                        if (dippkGrade.HasValue)
                        {
                            AddIfNotExists(model, field.Id, GradeUtils.GetDecimalGradeText(dippkGrade.Value));
                        }

                        break;
                    case "stateExamQualificationGrade":
                        if (!dippkLoaded)
                        {
                            dippkGrade = await LoadDippkGrade(personId);
                        }

                        if (dippkGrade.HasValue)
                        {
                            AddIfNotExists(model, field.Id, dippkGrade.Value);
                        }
                        break;
                    default:
                        break;
                }
            }

            return model.ToJson(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        /// <summary>
        /// Взимаме детайлите на подходящата институция.
        /// Ако логнатия потребител има институция използваме нея.
        /// Ако няма (роля ЧРАО) взимаме детайлите на институцията от EducationalState-а на ученика с позиция Учащ.
        /// </summary>
        /// <param name="userInstitutionId">Институция на логнатия потребител</param>
        /// <param name="personId"></param>
        /// <returns></returns>
        private async Task<InstitutionDropdownViewModel> GetInstitution(int? userInstitutionId, int? personId)
        {
            if (userInstitutionId.HasValue)
            {
                return (await _lookupService
                    .GetInstitutionsAsync(userInstitutionId.ToString(), userInstitutionId, null))
                    .FirstOrDefault();
            }

            if (personId.HasValue)
            {
                int? personEdiStateInstId = await _context.EducationalStates
                    .Where(x => x.PersonId == personId && x.PositionId == (int)PositionType.Student)
                    .Select(x => x.InstitutionId)
                    .FirstOrDefaultAsync();

                if (personEdiStateInstId.HasValue)
                {
                    return (await _lookupService
                        .GetInstitutionsAsync(personEdiStateInstId.ToString(), personEdiStateInstId, null))
                        .FirstOrDefault();
                }
            }

            return null;
        }

        private async Task<List<DiplomaViewModel>> GetPersonDiplomas(string personalId, int personalIdType, bool isManualCreateOrUpdate, int? diplomaId)
        {
            // Всички документи, които не са отбелязани за IsCancelled.
            IQueryable<Diploma> diplomaQuery = _context.Diplomas
                .AsNoTracking()
                .Where(x => x.PersonalId == personalId && x.PersonalIdtype == personalIdType
                    && !x.IsCancelled && x.IsSigned);

            if (isManualCreateOrUpdate && diplomaId.HasValue)
            {
                diplomaQuery = diplomaQuery.Where(x => x.Id != diplomaId.Value);
            }

            return await diplomaQuery
                .Select(x => new DiplomaViewModel
                {
                    Id = x.Id,
                    BasicDocumentId = x.BasicDocumentId,
                    BasicDocumentName = x.BasicDocumentName,
                    Series = x.Series,
                    FactoryNumber = x.FactoryNumber,
                    RegistrationNumberTotal = x.RegistrationNumberTotal,
                    RegistrationNumberYear = x.RegistrationNumberYear,
                    RegistrationDate = x.RegistrationDate,
                    IsSigned = x.IsSigned
                })
                .ToListAsync();
        }

        protected void AddIfNotExists(ExpandoObject model, string key, object value)
        {
            if (model == null) return;

            var dict = (IDictionary<string, object>)model;
            if (!dict.ContainsKey(key))
            {
                model.TryAdd(key, value);
                return;
            }

            if (dict[key] == null && value != null)
            {
                dict[key] = value;
            }
        }

        protected void AddOrUpdate(ExpandoObject model, string key, object value)
        {
            if (model == null) return;

            var dict = (IDictionary<string, object>)model;
            if (!dict.ContainsKey(key))
            {
                model.TryAdd(key, value);
                return;
            }


            if (value != null)
            {
                dict[key] = value;
            }
        }

        public void SetBasicClassIds(ICollection<int> ids)
        {
            BasicClassIds = ids?.ToList() ?? new List<int>();
        }
    }
}
