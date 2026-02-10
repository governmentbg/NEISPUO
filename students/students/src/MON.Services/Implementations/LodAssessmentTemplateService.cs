namespace MON.Services.Implementations
{
    using MON.DataAccess;
    using MON.Models;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MON.Shared;
    using System.Linq;
    using System.Text.Json;
    using Microsoft.EntityFrameworkCore;
    using System;
    using Z.EntityFramework.Plus;
    using MON.Models.StudentModels.Lod;
    using System.Linq.Dynamic.Core;
    using MON.Shared.Enums.SchoolBooks;

    public class LodAssessmentTemplateService : BaseService<LodAssessmentTemplateService>, ILodAssessmentTemplateService
    {
        private readonly IInstitutionService _institutionService;
        private readonly ILookupService _lookupService;

        public LodAssessmentTemplateService(DbServiceDependencies<LodAssessmentTemplateService> dependencies,
            IInstitutionService institutionService,
            ILookupService lookupService)
            : base(dependencies)
        {
            _institutionService = institutionService;
            _lookupService = lookupService;
        }

        #region Private members

        /// <summary>
        /// При преглед се използват SubjectName, SubjectTypeName, FlSubjectName, който в JSON-а липсват.
        /// Ще ги заредим отделно.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        private async Task LoadDisplayNames(LodAssessmentTemplateViewModel model)
        {
            if (model == null || model.CurriculumParts.IsNullOrEmpty() || model.CurriculumParts.All(x => x.SubjectAssessments.IsNullOrEmpty()))
            {
                return;
            }

            var subjectsQuery = model.CurriculumParts
                .Where(x => !x.SubjectAssessments.IsNullOrEmpty())
                .SelectMany(p => p.SubjectAssessments.Select(s => s))
                .Union(
                    model.CurriculumParts
                    .Where(x => !x.SubjectAssessments.IsNullOrEmpty())
                    .SelectMany(p => p.SubjectAssessments.Where(x => !x.Modules.IsNullOrEmpty()).SelectMany(s => s.Modules).Select(m => m))
                    .ToHashSet()
                );

            HashSet<int> subjectsIds = subjectsQuery
                .Select(x => x.SubjectId)
                .ToHashSet();
            HashSet<int> subjectTypesIds = subjectsQuery
                .Select(x => x.SubjectTypeId)
                .ToHashSet();
            HashSet<int> flSubjectsIds = subjectsQuery
                .Where(x => x.FlSubjectId.HasValue)
                .Select(x => x.FlSubjectId.Value)
                .ToHashSet();

            Dictionary<int, string> subjectsDict = await _context.Subjects
                .Where(x => subjectsIds.Contains(x.SubjectId))
                .Select(x => new
                {
                    x.SubjectId,
                    x.SubjectName
                })
                .ToDictionaryAsync(x => x.SubjectId, x => x.SubjectName);

            Dictionary<int, string> subjectTypesDict = await _context.SubjectTypes
                .Where(x => subjectTypesIds.Contains(x.SubjectTypeId))
                .Select(x => new
                {
                    x.SubjectTypeId,
                    x.Name
                })
                .ToDictionaryAsync(x => x.SubjectTypeId, x => x.Name);

            Dictionary<int, string> flSubjectsDict = await _context.Fls
                .Where(x => flSubjectsIds.Contains(x.Flid))
                .Select(x => new
                {
                    x.Flid,
                    x.Name
                })
                .ToDictionaryAsync(x => x.Flid, x => x.Name);

            Dictionary<int, string> gradesDict = (await _lookupService.GetGradeOptions(null, null))
                .ToDictionary(x => x.Value, x => x.GradeTypeId == (int)GradeTypeEnum.General ? x.Text : x.Name);

            foreach (var part in model.CurriculumParts.Where(x => !x.SubjectAssessments.IsNullOrEmpty()))
            {
                foreach (LodAssessmentCreateModel subject in part.SubjectAssessments)
                {
                    SetDisplayNames(subject, subjectsDict, subjectTypesDict, flSubjectsDict, gradesDict);

                    if (!subject.Modules.IsNullOrEmpty())
                    {
                        foreach (LodAssessmentCreateModel module in subject.Modules)
                        {
                            SetDisplayNames(module, subjectsDict, subjectTypesDict, flSubjectsDict, gradesDict);

                        }
                    }
                }
            }
        }

        private void SetDisplayNames(LodAssessmentCreateModel subject,
            Dictionary<int, string> subjectsDict,
            Dictionary<int, string> subjectTypesDict,
            Dictionary<int, string> flSubjectsDict,
            Dictionary<int, string> gradesDict
            )
        {
            if (subject == null)
            {
                return;
            }

            if (subjectsDict != null && subjectsDict.TryGetValue(subject.SubjectId, out string subjectName))
            {
                subject.SubjectName = subjectName;
            }

            if (subjectTypesDict != null && subjectTypesDict.TryGetValue(subject.SubjectTypeId, out string subjectTypeName))
            {
                subject.SubjectTypeName = subjectTypeName;
            }

            if (flSubjectsDict != null && subject.FlSubjectId.HasValue && flSubjectsDict.TryGetValue(subject.FlSubjectId.Value, out string flSubjectName))
            {
                subject.FlSubjectName = flSubjectName;
            }

            SetGradeText(subject.FirstTermAssessments, gradesDict);
            SetGradeText(subject.SecondTermAssessments, gradesDict);
            SetGradeText(subject.AnnualTermAssessments, gradesDict);
            SetGradeText(subject.FirstRemedialSession, gradesDict);
            SetGradeText(subject.SecondRemedialSession, gradesDict);
            SetGradeText(subject.AdditionalRemedialSession, gradesDict);
        }

        private void SetGradeText(List<LodAssessmentGradeCreateModel> grades, Dictionary<int, string> gradesDict)
        {
            if (grades.IsNullOrEmpty()) { return; }

            foreach (LodAssessmentGradeCreateModel grade in grades)
            {
                if(gradesDict != null && grade.GradeId.HasValue && gradesDict.TryGetValue(grade.GradeId.Value, out string gradeText))
                {
                    grade.GradeText = gradeText;
                }
            }
        }
        #endregion

        public async Task<IPagedList<LodAssessmentTemplateViewModel>> List(PagedListInput input, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError, 401);
            }

            IQueryable<LodAssessmentTemplateViewModel> listQuery = _context.LodAssessmentTemplates
                .Where(x => x.InstitutionId == _userInfo.InstitutionID.Value)
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Name.Contains(input.Filter)
                    || predicate.Description.Contains(input.Filter)
                    || predicate.BasicClass.Name.Contains(input.Filter)
                    || predicate.BasicClass.RomeName.Contains(input.Filter))
                .Select(x => new LodAssessmentTemplateViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    BasicClassName = x.BasicClass.RomeName,
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            IList<LodAssessmentTemplateViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task<LodAssessmentTemplateViewModel> GetById(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError, 401);
            }

            var entity = await _context.LodAssessmentTemplates
                .Where(x => x.Id == id && x.InstitutionId == _userInfo.InstitutionID.Value)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.BasicClassId,
                    BasicClassName = x.BasicClass.RomeName,
                    x.IsSelfEduForm,
                    x.Data
                })
                .SingleOrDefaultAsync();

            LodAssessmentTemplateViewModel model = entity != null
                ? new LodAssessmentTemplateViewModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    BasicClassId = entity.BasicClassId,
                    BasicClassName = entity.BasicClassName,
                    IsSelfEduForm = entity.IsSelfEduForm,
                    CurriculumParts = entity.Data.IsNullOrWhiteSpace()
                        ? new List<LodAssessmentCurriculumPartModel>()
                        : JsonSerializer.Deserialize<List<LodAssessmentCurriculumPartModel>>(entity.Data)
                }
                : null;

            await LoadDisplayNames(model);

            return model;
        }

        public async Task Create(LodAssessmentTemplateModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError, 401);
            }

            LodAssessmentTemplate entry = new LodAssessmentTemplate
            {
                Name = model.Name,
                Description = model.Description,
                BasicClassId = model.BasicClassId,
                InstitutionId = _userInfo.InstitutionID.Value,
                SchoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID),
                IsSelfEduForm = model.IsSelfEduForm,
                Data = model.CurriculumParts.IsNullOrEmpty() || model.CurriculumParts.All(x => x.SubjectAssessments.IsNullOrEmpty())
                    ? null
                    : JsonSerializer.Serialize(model.CurriculumParts)
            };

            _context.LodAssessmentTemplates.Add(entry);

            await SaveAsync();
        }

        public async Task Update(LodAssessmentTemplateModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError, 401);
            }

            LodAssessmentTemplate entity = await _context.LodAssessmentTemplates
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.InstitutionId == _userInfo.InstitutionID.Value);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentException(nameof(entity), nameof(Ore)));
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.BasicClassId = model.BasicClassId;
            entity.Data = model.CurriculumParts.IsNullOrEmpty() || model.CurriculumParts.All(x => x.SubjectAssessments.IsNullOrEmpty())
                     ? null
                     : JsonSerializer.Serialize(model.CurriculumParts);

            await SaveAsync();
        }

        public async Task Delete(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError, 401);
            }

            await _context.LodAssessmentTemplates
                .Where(x => x.Id == id && x.InstitutionId == _userInfo.InstitutionID.Value)
                .DeleteAsync();
        }

        public async Task<IAsyncEnumerable<DropdownViewModel>> GetDropdownOptions(int? basicClassId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodAssessmentTemplateManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (!_userInfo.InstitutionID.HasValue)
            {
                throw new ApiException(Messages.InvalidInstitutionCodeError, 401);
            }

            return _context.LodAssessmentTemplates
                .Where(x => x.InstitutionId == _userInfo.InstitutionID.Value)
                .Where(basicClassId.HasValue,
                    predicate =>  predicate.BasicClassId == basicClassId)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Text = $"{x.Name} / {x.BasicClass.RomeName} / {x.Description}"
                }).AsAsyncEnumerable();
        }
    }
}
