namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.Models;
    using MON.Models.Diploma;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Dynamic.Core;
    using System;
    using MON.DataAccess;
    using Z.EntityFramework.Plus;
    using MON.Shared.Enums;
    using MON.Shared.Extensions;
    using System.Threading;

    public class DiplomaTemplateService : BaseService<DiplomaTemplateService>, IDiplomaTemplateService
    {
        readonly IInstitutionService _institutionService;

        public DiplomaTemplateService(DbServiceDependencies<DiplomaTemplateService> dependencies, IInstitutionService institutionService)
            : base(dependencies)
        {
            _institutionService = institutionService;
        }


        private void Authorize(Template template)
        {
            _ = template ?? throw new ApiException(Messages.EmptyEntityError);

            if (_userInfo.InstitutionID.HasValue)
            {
                if (template.InstitutionId != _userInfo.InstitutionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else if (_userInfo.RegionID.HasValue)
            {
                if (template.RuoRegId != _userInfo.RegionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }
        }

        public async Task<IPagedList<DiplomaTemplateListModel>> List(PagedListInput input, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<Template> query = _context.Templates;

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
            } else if(_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RuoRegId == _userInfo.RegionID.Value);
            } else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<DiplomaTemplateListModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Name.Contains(input.Filter)
                    || predicate.BasicDocument.Name.Contains(input.Filter)
                    || predicate.BasicClass.Name.Contains(input.Filter))
                .Select(x => new DiplomaTemplateListModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    BasicDocumentTypeName = x.BasicDocument.Name,
                    BasicClassName = x.BasicClass.RomeName,
                    CanBeDeleted = true
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Name asc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            IList<DiplomaTemplateListModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }

        public async Task<BasicDocumentTemplateModel> GetById(int id, CancellationToken cancellationToken)
        {
            var template = await _context.Templates
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.BasicDocumentId,
                    x.InstitutionId,
                    x.RuoRegId,
                    x.Name,
                    x.Description,
                    //x.Principal,
                    //x.Deputy,
                    x.SchoolYear,
                    x.CommissionOrderNumber,
                    x.CommissionOrderData,
                    InstitutionName = x.InstitutionSchoolYear.Name,
                    BasicDocumentName = x.BasicDocument.Name ?? "",
                    BasicDocumentCodeClassName = x.BasicDocument.CodeClassName,
                    x.BasicDocument.IsValidation,
                    x.Contents,
                    x.BasicClassId,
                    x.BasicDocument.BasicClasses,
                    x.BasicDocument.MainBasicDocuments,
                    x.BasicDocument.DetailedSchoolTypes,
                    x.BasicDocument.IsRuoDoc,
                    Parts = x.BasicDocument.BasicDocumentParts
                        .OrderBy(p => p.Position)
                        .Select(p => new
                        {
                            p.Id,
                            p.Name,
                            p.Description,
                            p.Position,
                            p.IsHorariumHidden,
                            p.BasicClassId,
                            p.SubjectTypesList,
                            p.ExternalEvaluationTypesList,
                            p.Code,
                            p.PrintedLines,
                            p.TotalLines,
                        }),
                    Subjects = x.TemplateSubjects
                        .Select(s => new
                        {
                            s.Id,
                            s.TemplateId,
                            s.ParentId,
                            s.BasicDocumentPartId,
                            s.BasicDocumentSubjectId,
                            s.SubjectId,
                            s.SubjectTypeId,
                            s.Position,
                            s.SubjectCanChange,
                            s.Horarium,
                            s.GradeCategory,
                            SubjectName = s.SubjectName ?? s.Subject.SubjectName,
                            SubjectNameShort = s.SubjectName ?? s.Subject.SubjectNameShort,
                            SubjectNomName = s.Subject.SubjectName,
                            SubjectTypeName = s.SubjectType.Name,
                            s.FlSubjectId,
                            FlSubjectName = s.FlSubjectName ?? s.FlSubject.Name,
                            s.FlLevel,
                            s.FlHorarium,
                            s.BasicClassId
                        }),
                    CommisionMembers = x.GraduationCommissionMembers
                            .Select(m => new
                            {
                                m.Id,
                                m.FullName,
                                m.FullNameLatin,
                                m.Position
                            })
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new ApiException(Messages.EmptyEntityError);

            if (_userInfo.InstitutionID.HasValue)
            {
                if (template.InstitutionId != _userInfo.InstitutionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else if (_userInfo.RegionID.HasValue)
            {
                if (template.RuoRegId != _userInfo.RegionID.Value)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            BasicDocumentTemplateModel model = new BasicDocumentTemplateModel
            {
                Id = template.Id,
                BasicDocumentId = template.BasicDocumentId,
                InstitutionId = template.InstitutionId,
                RuoRegId = template.RuoRegId,
                Name = template.Name,
                Description = template.Description,
                //Principal = entity.Principal,
                //Deputy = entity.Deputy,
                CommissionOrderNumber = template.CommissionOrderNumber,
                CommissionOrderData = template.CommissionOrderData,
                SchoolYear = template.SchoolYear,
                InstitutionName = template.InstitutionName,
                BasicDocumentName = template.BasicDocumentName,
                DynamicContent = template.Contents,
                IsValidation = template.IsValidation,
                BasicDocumentCodeClassName = template.BasicDocumentCodeClassName,
                BasicClassId = template.BasicClassId,
                BasicClassIds = !template.BasicClasses.IsNullOrEmpty() ? template.BasicClasses.ToHashSet<int>('|').ToList() : null,
                MainBasicDocumentsStr = template.MainBasicDocuments,
                DetailedSchoolTypesStr = template.DetailedSchoolTypes,
                CommissionMembers = template.CommisionMembers
                    .OrderBy(x => x.Position)
                    .Select(x => new CommissionMemberModel
                    {
                        Id = x.Id,
                        Uid = Guid.NewGuid().ToString(),
                        TemplateId = template.Id,
                        FullName = x.FullName,
                        FullNameLatin = x.FullNameLatin,
                        Position = x.Position,
                    })
                    .ToList(),
                Parts = template.Parts
                    .OrderBy(p => p.Position)
                    .Select(p => new BasicDocumentTemplatePartModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Position = p.Position,
                        IsHorariumHidden = p.IsHorariumHidden,
                        BasicClassId = p.BasicClassId,
                        Code = p.Code,
                        PrintedLines = p.PrintedLines,
                        TotalLines = p.TotalLines,
                        ShowEctsGrade = GlobalConstants.BasicDocumentsWithEctsGrade.Contains(template.BasicDocumentId),
                        SubjectTypes = (p.SubjectTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        ExternalEvaluationTypes = (p.ExternalEvaluationTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        Subjects = template.Subjects
                            .Where(x => x.BasicDocumentPartId == p.Id && x.ParentId == null)
                            .OrderBy(x => x.Position)
                            .Select(s => new BasicDocumentTemplateSubjectModel
                            {
                                Id = s.Id,
                                Uid = Guid.NewGuid().ToString(),
                                TemplateId = s.TemplateId,
                                BasicDocumentPartId = p.Id,
                                BasicDocumentSubjectId = s.BasicDocumentSubjectId,
                                SubjectId = s.SubjectId,
                                SubjectTypeId = s.SubjectTypeId,
                                Position = s.Position,
                                SubjectCanChange = s.SubjectCanChange,
                                Horarium = s.Horarium,
                                GradeCategory = s.GradeCategory,
                                SubjectName = s.SubjectName,
                                SubjectNameShort = s.SubjectNameShort,
                                ShowSubjectNamePreview = false == (s.SubjectName ?? "").Equals((s.SubjectNomName ?? ""), StringComparison.OrdinalIgnoreCase),
                                SubjectTypeName = s.SubjectTypeName,
                                IsHorariumHidden = p.IsHorariumHidden,
                                ShowFlSubject = s.FlSubjectId != null,
                                FlSubjectId = s.FlSubjectId,
                                FlSubjectName = s.FlSubjectName,
                                FlLevel = s.FlLevel,
                                FlHorarium = s.FlHorarium,
                                BasicClassId = s.BasicClassId,
                                IsProfSubjectHeader = false,
                                Modules = template.Subjects.Where(m => m.ParentId != null && m.ParentId == s.Id)
                                    .OrderBy(m => m.Position)
                                    .Select(m => new BasicDocumentTemplateSubjectModel
                                    {
                                        Id = m.Id,
                                        Uid = Guid.NewGuid().ToString(),
                                        TemplateId = m.TemplateId,
                                        BasicDocumentPartId = p.Id,
                                        BasicDocumentSubjectId = m.BasicDocumentSubjectId,
                                        GradeCategory = m.GradeCategory,
                                        SubjectId = m.SubjectId,
                                        SubjectTypeId = m.SubjectTypeId,
                                        Position = m.Position,
                                        SubjectCanChange = m.SubjectCanChange,
                                        Horarium = m.Horarium,
                                        SubjectName = m.SubjectName,
                                        SubjectNameShort = m.SubjectNameShort,
                                        ShowSubjectNamePreview = false == (m.SubjectName ?? "").Equals((m.SubjectNomName ?? ""), StringComparison.OrdinalIgnoreCase),
                                        SubjectTypeName = m.SubjectTypeName,
                                        IsHorariumHidden = p.IsHorariumHidden,
                                        ShowFlSubject = m.FlSubjectId != null,
                                        FlSubjectId = m.FlSubjectId,
                                        FlSubjectName = m.FlSubjectName,
                                        FlLevel = m.FlLevel,
                                        FlHorarium = m.FlHorarium,
                                        BasicClassId = m.BasicClassId,
                                        IsProfSubjectHeader = true
                                    })
                                    .ToList()
                            }).ToList(),
                    })
                    .ToList(),
            };

            // За всяка секция, която има избрани SubjecTypes ще подадем и списъм с DropdownViewModel опциите,
            // които да се използват в SPA-то. Ще спестим ненужното зареждане на SubjecTypes.
            HashSet<int> subjectTypesIds = model.Parts.SelectMany(x => x.SubjectTypes).ToHashSet();
            List<DropdownViewModel> subjectTypeOptons = await _context.SubjectTypes
                .Where(x => subjectTypesIds.Contains(x.SubjectTypeId))
                .Select(x => new DropdownViewModel
                {
                    Value = x.SubjectTypeId,
                    Text = x.Name
                })
                .ToListAsync();

            foreach (BasicDocumentTemplatePartModel part in model.Parts)
            {
                if (!part.HasSubjectTypeLimit) continue;

                part.SubjectTypeOptions = subjectTypeOptons.Where(x => part.SubjectTypes.Contains(x.Value)).ToList();

                if (part.SubjectTypeOptions.Count <= 1)
                {
                    // Само една опция за SubjectType.
                    foreach (var subject in part.Subjects.Where(x => !x.SubjectTypeId.HasValue))
                    {
                        // Всички предмети, които нямат избрани SubjectType го сетваме на единствения възможен..
                        subject.SubjectTypeId = part.SubjectTypeOptions.FirstOrDefault()?.Value;
                    }
                }
            }

            return model;
        }

        public async Task<BasicDocumentTemplateModel> GetForBasicDocument(int basicDocumentId, CancellationToken cancellationToken)
        {
            // DefaultPermissions.PermissionNameForStudentDiplomaManage се проверява защото този метод се извиква през
            // DiplomaServide.GetCreateModel. Модел за създаване на диплома по подаден ИД на BasicDocument.
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesManage)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForStudentDiplomaManage)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForAdminDiplomaManage)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForRuoHrDiplomaManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            InstitutionCacheModel institution = null;
            if (_userInfo.InstitutionID.HasValue)
            {
                institution = await _institutionService.GetInstitutionCache(_userInfo.InstitutionID.Value);
                if (institution == null)
                {
                    throw new ApiException(Messages.InvalidInstitutionCodeError);
                }
            }

            var entity = await _context.BasicDocuments
                .Where(x => x.Id == basicDocumentId)
                .Select(x => new
                {
                    x.Id,
                    Name = x.Name ?? "",
                    x.Contents,
                    x.IsValidation,
                    x.CodeClassName,
                    x.BasicClasses,
                    x.MainBasicDocuments,
                    x.DetailedSchoolTypes,
                    x.IsRuoDoc,
                    BasicDocumentParts = x.BasicDocumentParts.Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Position,
                        p.IsHorariumHidden,
                        p.BasicClassId,
                        p.SubjectTypesList,
                        p.ExternalEvaluationTypesList,
                        p.Code,
                        p.PrintedLines,
                        p.TotalLines,
                        BasicDocumentSubjects = p.BasicDocumentSubjects.Select(s => new
                        {
                            s.Id,
                            s.Position,
                            s.SubjectCanChange,
                            s.SubjectId,
                            s.Subject.SubjectName,
                            s.Subject.SubjectNameShort,
                            s.SubjectTypeId,
                            SubjectTypeName = s.SubjectType.Name
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            short currentSchoolYear = await _context.CurrentYears
                .Where(x => x.IsValid == true)
                .Select(x => x.CurrentYearId)
                .FirstOrDefaultAsync(cancellationToken);

            BasicDocumentTemplateModel model = new BasicDocumentTemplateModel()
            {
                BasicDocumentId = entity.Id,
                BasicDocumentName = entity.Name,
                InstitutionId = institution?.Id ?? 0,
                InstitutionName = institution?.Name,
                SchoolYear = await _institutionService.GetCurrentYear(institution?.Id),
                IsValidation = entity.IsValidation,
                BasicDocumentCodeClassName = entity.CodeClassName,
                BasicClassIds = !entity.BasicClasses.IsNullOrEmpty() ? entity.BasicClasses.ToHashSet<int>('|').ToList() : null,
                MainBasicDocumentsStr = entity.MainBasicDocuments,
                DetailedSchoolTypesStr = entity.DetailedSchoolTypes,
                Parts = entity.BasicDocumentParts
                    .OrderBy(p => p.Position)
                    .Select(p => new BasicDocumentTemplatePartModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Position = p.Position,
                        IsHorariumHidden = p.IsHorariumHidden,
                        BasicClassId = p.BasicClassId,
                        Code = p.Code,
                        PrintedLines = p.PrintedLines,
                        TotalLines = p.TotalLines,
                        ShowEctsGrade = GlobalConstants.BasicDocumentsWithEctsGrade.Contains(entity.Id),
                        SubjectTypes = (p.SubjectTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        ExternalEvaluationTypes = (p.ExternalEvaluationTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        Subjects = p.BasicDocumentSubjects
                            .OrderBy(x => x.Position)
                            .Select(s => new BasicDocumentTemplateSubjectModel
                            {
                                Uid = Guid.NewGuid().ToString(),
                                BasicDocumentPartId = p.Id,
                                BasicDocumentSubjectId = s.Id,
                                SubjectId = s.SubjectId,
                                SubjectName = s.SubjectName,
                                SubjectNameShort = s.SubjectNameShort,
                                SubjectTypeId = s.SubjectTypeId,
                                SubjectTypeName = s.SubjectTypeName,
                                SubjectCanChange = s.SubjectCanChange,
                                Position = s.Position,
                                IsHorariumHidden = p.IsHorariumHidden
                            }).ToList()
                    })
                    .ToList(),
            };

            // За всяка секция, която има избрани SubjecTypes ще подадем и списъм с DropdownViewModel опциите,
            // които да се използват в SPA-то. Ще спестим ненужното зареждане на SubjecTypes.
            HashSet<int> subjectTypesIds = model.Parts.SelectMany(x => x.SubjectTypes).ToHashSet();
            List<DropdownViewModel> subjectTypeOptons = await _context.SubjectTypes
                .Where(x => subjectTypesIds.Contains(x.SubjectTypeId))
                .Select(x => new DropdownViewModel
                {
                    Value = x.SubjectTypeId,
                    Text = x.Name
                })
                .ToListAsync(cancellationToken);

            foreach (BasicDocumentTemplatePartModel part in model.Parts)
            {
                if (!part.HasSubjectTypeLimit) continue;

                part.SubjectTypeOptions = subjectTypeOptons.Where(x => part.SubjectTypes.Contains(x.Value)).ToList();

                if (part.SubjectTypeOptions.Count <= 1)
                {
                    // Само една опция за SubjectType.
                    foreach (var subject in part.Subjects.Where(x => !x.SubjectTypeId.HasValue))
                    {
                        // Всички предмети, които нямат избрани SubjectType го сетваме на единствения възможен..
                        subject.SubjectTypeId = part.SubjectTypeOptions.FirstOrDefault()?.Value;
                    }
                }
            }

            return model;
        }

        public async Task<BasicDocumentTemplateModel> GetForDiploma(int diplomaId)
        {
            var entity = await _context.Diplomas
                .Where(x => x.Id == diplomaId)
                .Select(x => new
                {
                    x.Id,
                    x.TemplateId,
                    x.BasicDocumentId,
                    x.InstitutionId,
                    x.BasicDocument.Name,
                    x.Description,
                    //x.Principal,
                    //x.Deputy,
                    x.SchoolYear,
                    InstitutionName = x.InstitutionSchoolYear.Name,
                    BasicDocumentName = x.BasicDocument.Name ?? "",
                    BasicDocumentCodeClassName = x.BasicDocument.CodeClassName,
                    x.BasicDocument.IsValidation,
                    x.Contents,
                    x.BasicDocument.MainBasicDocuments,
                    x.BasicDocument.DetailedSchoolTypes,
                    Parts = x.BasicDocument.BasicDocumentParts
                        .OrderBy(p => p.Position)
                        .Select(p => new
                        {
                            p.Id,
                            p.Name,
                            p.Description,
                            p.Position,
                            p.IsHorariumHidden,
                            p.BasicClassId,
                            p.SubjectTypesList,
                            p.ExternalEvaluationTypesList,
                            p.Code,
                            p.PrintedLines,
                            p.TotalLines,
                        }),
                    Subjects = x.DiplomaSubjects
                        .Select(s => new
                        {
                            s.Id,
                            s.ParentId,
                            s.DiplomaId,
                            s.BasicDocumentPartId,
                            s.BasicDocumentSubjectId,
                            s.SubjectId,
                            s.SubjectTypeId,
                            s.Position,
                            SubjectCanChange = s.BasicDocumentSubject != null ? s.BasicDocumentSubject.SubjectCanChange : (bool?)null, // В описанието на предметите в BasicDocumentSubject определяма дали могат да се променят
                            s.Horarium,
                            SubjectName = string.IsNullOrWhiteSpace(s.SubjectName) ? s.Subject.SubjectName : s.SubjectName,
                            s.Subject.SubjectNameShort,
                            SubjectNomName = s.Subject.SubjectName,
                            SubjectTypeName = s.SubjectType.Name,
                            s.Grade,
                            s.GradeText,
                            s.GradeCategory,
                            s.FlSubjectId,
                            FlSubjectName = s.FlSubject.Name,
                            s.FlHorarium,
                            s.FlLevel,
                            s.Nvopoints,
                            s.SpecialNeedsGrade,
                            s.OtherGrade,
                            s.QualitativeGrade,
                            s.Ects,
                            s.BasicClassId,
                        }),
                    CommisionMembers = x.GraduationCommissionMembers
                        .Select(m => new
                        {
                            m.Id,
                            m.FullName,
                            m.FullNameLatin,
                            m.Position
                        })
                })
                .SingleOrDefaultAsync();

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }
           
            BasicDocumentTemplateModel model = new BasicDocumentTemplateModel
            {
                Id = entity.TemplateId,
                BasicDocumentId = entity.BasicDocumentId,
                InstitutionId = entity.InstitutionId,
                Name = entity.Name,
                Description = entity.Description,
                //Principal = entity.Principal,
                //Deputy = entity.Deputy,
                SchoolYear = entity.SchoolYear,
                InstitutionName = entity.InstitutionName,
                BasicDocumentName = entity.BasicDocumentName,
                DynamicContent = entity.Contents,
                IsValidation = entity.IsValidation,
                BasicDocumentCodeClassName = entity.BasicDocumentCodeClassName,
                MainBasicDocumentsStr = entity.MainBasicDocuments,
                DetailedSchoolTypesStr = entity.DetailedSchoolTypes,
                CommissionMembers = entity.CommisionMembers
                    .OrderBy(x => x.Position)
                    .Select(x => new CommissionMemberModel
                    {
                        Id = x.Id,
                        Uid = Guid.NewGuid().ToString(),
                        DiplomaId = entity.Id,
                        FullName = x.FullName,
                        FullNameLatin = x.FullNameLatin,
                        Position = x.Position,
                    })
                    .ToList(),
                Parts = entity.Parts
                    .OrderBy(p => p.Position)
                    .Select(p => new BasicDocumentTemplatePartModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Position = p.Position,
                        IsHorariumHidden = p.IsHorariumHidden,
                        BasicClassId = p.BasicClassId,
                        Code = p.Code,
                        PrintedLines = p.PrintedLines,
                        TotalLines = p.TotalLines,
                        ShowEctsGrade = GlobalConstants.BasicDocumentsWithEctsGrade.Contains(entity.BasicDocumentId),
                        SubjectTypes = (p.SubjectTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        ExternalEvaluationTypes = (p.ExternalEvaluationTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        Subjects = entity.Subjects
                            .Where(x => x.ParentId == null && x.BasicDocumentPartId == p.Id)
                            .OrderBy(x => x.Position)
                            .Select(s => new BasicDocumentTemplateSubjectModel
                            {
                                Id = s.Id,
                                Uid = Guid.NewGuid().ToString(),
                                BasicDocumentPartId = p.Id,
                                BasicDocumentSubjectId = s.Id,
                                SubjectId = s.SubjectId ?? default,
                                SubjectTypeId = s.SubjectTypeId,
                                Position = s.Position,
                                ParentId = s.ParentId,
                                SubjectCanChange = s.SubjectCanChange ?? true, // Ако не е посочено приемаме, че може да се променя
                                Horarium = s.Horarium,
                                SubjectName = s.SubjectName,
                                SubjectNameShort = s.SubjectNameShort,
                                ShowSubjectNamePreview = false == (s.SubjectName ?? "").Equals((s.SubjectNomName ?? ""), StringComparison.OrdinalIgnoreCase),
                                SubjectTypeName = s.SubjectTypeName,
                                IsHorariumHidden = p.IsHorariumHidden,
                                GradeCategory = s.GradeCategory,
                                Grade = s.Grade,
                                GradeText = s.GradeText,
                                SpecialNeedsGrade = s.SpecialNeedsGrade,
                                OtherGrade = s.OtherGrade,
                                QualitativeGrade = s.QualitativeGrade,
                                NvoPoints = s.Nvopoints,
                                FlLevel = s.FlLevel,
                                FlSubjectId = s.FlSubjectId,
                                FlSubjectName = s.FlSubjectName,
                                FlHorarium = s.FlHorarium,
                                Ects = s.Ects,
                                ShowFlSubject = s.FlSubjectId != null,
                                BasicClassId= s.BasicClassId,
                                IsProfSubjectHeader = false,
                                Modules = entity.Subjects.Where(m => m.ParentId != null && m.ParentId == s.Id)
                                    .Select(m => new BasicDocumentTemplateSubjectModel
                                    {
                                        Id = m.Id,
                                        Uid = Guid.NewGuid().ToString(),
                                        BasicDocumentPartId = p.Id,
                                        BasicDocumentSubjectId = m.Id,
                                        SubjectId = m.SubjectId ?? default,
                                        SubjectTypeId = m.SubjectTypeId,
                                        Position = m.Position,
                                        ParentId = m.ParentId,
                                        SubjectCanChange = m.SubjectId.HasValue ? true : false,
                                        Horarium = m.Horarium,
                                        SubjectName = m.SubjectName,
                                        SubjectNameShort = m.SubjectNameShort,
                                        ShowSubjectNamePreview = false == (m.SubjectName ?? "").Equals((m.SubjectNomName ?? ""), StringComparison.OrdinalIgnoreCase),
                                        SubjectTypeName = m.SubjectTypeName,
                                        IsHorariumHidden = p.IsHorariumHidden,
                                        GradeCategory = m.GradeCategory,
                                        Grade = m.Grade,
                                        GradeText = m.GradeText,
                                        SpecialNeedsGrade = m.SpecialNeedsGrade,
                                        OtherGrade = m.OtherGrade,
                                        QualitativeGrade = m.QualitativeGrade,
                                        NvoPoints = m.Nvopoints,
                                        FlLevel = m.FlLevel,
                                        FlSubjectId = m.FlSubjectId,
                                        FlSubjectName = m.FlSubjectName,
                                        FlHorarium = m.FlHorarium,
                                        Ects = m.Ects,
                                        ShowFlSubject = m.FlSubjectId != null,
                                        BasicClassId = m.BasicClassId,
                                        IsProfSubjectHeader = false
                                    })
                                    .ToList()
                            }).ToList()
                    })
                    .ToList()
            };

            // За всяка секция, която има избрани SubjectTypes ще подадем и списъм с DropdownViewModel опциите,
            // които да се използват в SPA-то. Ще спестим ненужното зареждане на SubjectTypes.
            HashSet<int> subjectTypesIds = model.Parts.SelectMany(x => x.SubjectTypes).ToHashSet();
            List<DropdownViewModel> subjectTypeOptons = await _context.SubjectTypes
                .Where(x => subjectTypesIds.Contains(x.SubjectTypeId))
                .Select(x => new DropdownViewModel
                {
                    Value = x.SubjectTypeId,
                    Text = x.Name
                })
                .ToListAsync();

            foreach (BasicDocumentTemplatePartModel part in model.Parts)
            {
                if (!part.HasSubjectTypeLimit) continue;

                part.SubjectTypeOptions = subjectTypeOptons.Where(x => part.SubjectTypes.Contains(x.Value)).ToList();

                if (part.SubjectTypeOptions.Count <= 1)
                {
                    // Само една опция за SubjectType.
                    foreach (var subject in part.Subjects.Where(x => !x.SubjectTypeId.HasValue))
                    {
                        // Всички предмети, които нямат избрани SubjectType го сетваме на единствения възможен..
                        subject.SubjectTypeId = part.SubjectTypeOptions.FirstOrDefault()?.Value;
                    }
                }
            }

            return model;
        }

        public async Task Create(BasicDocumentTemplateModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesManage)
                || (!_userInfo.InstitutionID.HasValue && !_userInfo.RegionID.HasValue))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            _ = model ?? throw new ApiException(Messages.EmptyModelError);

            Template entry = new Template
            {
                Name = model.Name.Truncate(255),
                Description = model.Description,
                InstitutionId = _userInfo.InstitutionID,
                RuoRegId = _userInfo.RegionID,
                SchoolYear = model.SchoolYear,
                BasicDocumentId = model.BasicDocumentId,
                //Principal = model.Principal.Truncate(255),
                //Deputy = model.Deputy.Truncate(255),
                CommissionOrderNumber = model.CommissionOrderNumber.Truncate(50),
                CommissionOrderData = model.CommissionOrderData,
                Contents = model.DynamicContent,
                BasicClassId = model.BasicClassId,
            };

            if (!model.Parts.IsNullOrEmpty())
            {
                foreach (var part in model.Parts.Where(x => !x.Subjects.IsNullOrEmpty()))
                {
                    foreach (var subject in part.Subjects)
                    {
                        TemplateSubject subjectToAdd = new TemplateSubject
                        {
                            BasicDocumentPartId = subject.BasicDocumentPartId,
                            BasicDocumentSubjectId = subject.BasicDocumentSubjectId,
                            SubjectId = subject.SubjectId,
                            SubjectName = subject.SubjectName,
                            SubjectTypeId = subject.SubjectTypeId,
                            Position = subject.Position,
                            Horarium = subject.Horarium,
                            SubjectCanChange = subject.SubjectCanChange,
                            GradeCategory = subject.GradeCategory ?? (int)GradeCategoryEnum.Normal,
                            FlSubjectId = subject.FlSubjectId,
                            FlSubjectName= subject.FlSubjectName,
                            FlLevel = subject.FlLevel,
                            FlHorarium = subject.FlHorarium,
                            BasicClassId= subject.BasicClassId,
                        };

                        if (!subject.Modules.IsNullOrEmpty())
                        {
                            foreach (BasicDocumentTemplateSubjectModel module in subject.Modules)
                            {
                                TemplateSubject subjectModuleToAdd = new TemplateSubject
                                {
                                    BasicDocumentPartId = module.BasicDocumentPartId,
                                    BasicDocumentSubjectId = module.BasicDocumentSubjectId,
                                    SubjectId = module.SubjectId,
                                    SubjectTypeId = module.SubjectTypeId,
                                    SubjectName = module.SubjectName,
                                    Position = module.Position,
                                    Horarium = module.Horarium,
                                    SubjectCanChange = module.SubjectCanChange,
                                    GradeCategory = module.GradeCategory ?? (int)GradeCategoryEnum.Normal,
                                    FlSubjectId = module.FlSubjectId,
                                    FlSubjectName = module.FlSubjectName,
                                    FlLevel = module.FlLevel,
                                    FlHorarium = module.FlHorarium,
                                    BasicClassId = module.BasicClassId,
                                };

                                subjectToAdd.InverseParent.Add(subjectModuleToAdd);
                                entry.TemplateSubjects.Add(subjectModuleToAdd);
                            }
                        }

                        entry.TemplateSubjects.Add(subjectToAdd);
                    }
                }
            }

            if (!model.CommissionMembers.IsNullOrEmpty())
            {
                foreach (var member in model.CommissionMembers)
                {
                    entry.GraduationCommissionMembers.Add(new GraduationCommissionMember
                    {
                        FullName = member.FullName.Truncate(1000),
                        FullNameLatin = member.FullNameLatin.Truncate(1000),
                        Position = member.Position
                    });
                }
            }

            _context.Templates.Add(entry);
            await SaveAsync();
        }

        public async Task Update(BasicDocumentTemplateModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            _ = model ?? throw new ApiException(Messages.EmptyModelError);

            Template template = await _context.Templates.FirstOrDefaultAsync(x => x.Id == model.Id);
            Authorize(template);

            template.Name = model.Name;
            template.Description = model.Description;
            template.SchoolYear = model.SchoolYear;
            //entity.Principal = model.Principal.Truncate(255);
            //entity.Deputy = model.Deputy.Truncate(255);
            template.CommissionOrderNumber = model.CommissionOrderNumber.Truncate(50);
            template.CommissionOrderData = model.CommissionOrderData;
            template.Contents = model.DynamicContent;
            template.BasicClassId = model.BasicClassId;

            using var tran = _context.Database.BeginTransaction();
            try
            {
                await _context.TemplateSubjects
                    .Where(x => x.TemplateId == model.Id)
                    .DeleteAsync();

                await _context.GraduationCommissionMembers
                    .Where(x => x.TemplateId == model.Id)
                    .DeleteAsync();

                if (!model.Parts.IsNullOrEmpty())
                {
                    var allSubjectTypeIds = model.Parts.Where(x => !x.Subjects.IsNullOrEmpty()).SelectMany(i => i.Subjects).Select(x => x.SubjectTypeId).ToHashSet();
                    var allDbSubjectTypeIds = await _context.SubjectTypes.Where(i => allSubjectTypeIds.Contains(i.SubjectTypeId)).Select(x => x.SubjectTypeId).ToListAsync();
                    foreach (var part in model.Parts.Where(x => !x.Subjects.IsNullOrEmpty()))
                    {
                        foreach (var subject in part.Subjects)
                        {
                            TemplateSubject subjectToAdd = new TemplateSubject
                            {
                                TemplateId = template.Id,
                                BasicDocumentPartId = subject.BasicDocumentPartId,
                                BasicDocumentSubjectId = subject.BasicDocumentSubjectId,
                                SubjectId = subject.SubjectId != 0 ? subject.SubjectId : null,
                                SubjectName = subject.SubjectName,
                                SubjectTypeId = allSubjectTypeIds.Contains(subject.SubjectTypeId) ? subject.SubjectTypeId : null,
                                Position = subject.Position,
                                Horarium = subject.Horarium,
                                GradeCategory = subject.GradeCategory ?? (int)GradeCategoryEnum.Normal,
                                SubjectCanChange = subject.SubjectCanChange,
                                FlSubjectId = subject.FlSubjectId,
                                FlSubjectName = subject.FlSubjectName,
                                FlLevel = subject.FlLevel,
                                FlHorarium = subject.FlHorarium,
                                BasicClassId= subject.BasicClassId,
                            };

                            if (!subject.Modules.IsNullOrEmpty())
                            {
                                subjectToAdd.InverseParent = subject.Modules
                                    .Select(m => new TemplateSubject
                                    {
                                        TemplateId = template.Id,
                                        BasicDocumentPartId = m.BasicDocumentPartId,
                                        BasicDocumentSubjectId = m.BasicDocumentSubjectId,
                                        SubjectId = m.SubjectId != 0 ? m.SubjectId : null,
                                        SubjectName = m.SubjectName,
                                        SubjectTypeId = m.SubjectTypeId,
                                        Position = m.Position,
                                        Horarium = m.Horarium,
                                        GradeCategory = m.GradeCategory ?? (int)GradeCategoryEnum.Normal,
                                        SubjectCanChange = m.SubjectCanChange,
                                        FlSubjectId = m.FlSubjectId,
                                        FlSubjectName = m.FlSubjectName,
                                        FlLevel = m.FlLevel,
                                        FlHorarium = m.FlHorarium,
                                        BasicClassId = m.BasicClassId,
                                    }).ToList();
                            }

                            _context.TemplateSubjects.Add(subjectToAdd);
                        }
                    }
                }

                if (!model.CommissionMembers.IsNullOrEmpty())
                {
                    foreach (var member in model.CommissionMembers)
                    {
                        _context.GraduationCommissionMembers.Add(new GraduationCommissionMember
                        {
                            TemplateId = template.Id,
                            FullName = member.FullName.Truncate(1000),
                            FullNameLatin = member.FullNameLatin.Truncate(1000),
                            Position = member.Position
                        });
                    }
                }

                await SaveAsync();
                await tran.CommitAsync();
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().Message);
            }
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            Template template = await _context.Templates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            Authorize(template);

            _context.Templates.Remove(template);
            await SaveAsync(cancellationToken);          
        }

        public Task<List<DiplomaTemplateDropdownViewModel>> GetDropdownOptions(string searchStr, int? basicDocumentId, CancellationToken cancellationToken)
        {
            IQueryable<Template> query = _context.Templates.AsQueryable();

            if (_userInfo.InstitutionID.HasValue)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID.Value);
            } else if(_userInfo.RegionID.HasValue)
            {
                query = query.Where(x => x.RuoRegId == _userInfo.RegionID.Value);
            } else
            {
                query = query.Where(x => 1 == 2);
            }

            if (basicDocumentId.HasValue)
            {
                query = query.Where(x => x.BasicDocumentId == basicDocumentId.Value);
            }

            if (searchStr.IsNullOrWhiteSpace())
            {
                searchStr = null;
            }
            else
            {
                searchStr = searchStr.Trim();
            }

            query = query.Where(x => searchStr == null || x.Name.Contains(searchStr));

            return query.Select(x => new DiplomaTemplateDropdownViewModel
            {
                Value = x.Id,
                Text = x.Name,
                BasicClassId= x.BasicClassId,
                BasicClassName = x.BasicClass.RomeName
            }).ToListAsync(cancellationToken);
        }
    }
}
