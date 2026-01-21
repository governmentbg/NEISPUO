namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Enums;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.Enums;
    using MON.Shared.Extensions;
    using MON.Shared.ErrorHandling;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class EqualizationService : BaseService<EqualizationService>, IEqualizationService
    {
        private readonly IAppConfigurationService _configurationService;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IInstitutionService _institutionService;

        public EqualizationService(DbServiceDependencies<EqualizationService> dependencies,
            IAppConfigurationService configurationService,
            IInstitutionService institutionService,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _configurationService = configurationService;
            _institutionService = institutionService;
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        #region Private members
        private async Task ProcessAddedDocs(EqualizationModel model, Equalization entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.EqualizationAttachments.Add(new EqualizationAttachment
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(EqualizationModel model, Equalization entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.EqualizationAttachments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }
        #endregion

        public async Task<EqualizationModel> GetById(int equalizationId)
        {
            string finalSessionStr = StudentSessionCategory.Final.GetEnumDescription();

            EqualizationModel model = await _context.Equalizations
                .Where(x => x.Id == equalizationId)
                .Select(x => new EqualizationModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    ReasonId = x.ReasonId,
                    ReasonName = x.Reason.Name,
                    InClass = x.InClass,
                    BasicClassName = x.InClassNavigation.Name,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    EqualizationDetails = x.EqualizationDetails.Select(e => new EqualizationDetailsModel
                    {
                        Id = e.Id,
                        SubjectId = e.SubjectId,
                        SubjectName = e.Subject.SubjectName,
                        SubjectTypeId = e.SubjectTypeId,
                        SubjectTypeName = e.SubjectType.Name,
                        GradeCategory = e.GradeCategory,
                        Grade = e.Grade,
                        SpecialNeedsGrade = e.SpecialNeedsGrade,
                        OtherGrade = e.OtherGrade,
                        BasicClassId = e.BasicClassId,
                        BasicClassName = e.BasicClass.Description,
                        Term = e.Term,
                        SortOrder = e.Position,
                        Horarium = e.Horarium,
                        SessionId = e.SessionId,
                        SessionName = e.SessionId == (int)StudentSessionCategory.Final ? finalSessionStr : e.Session.Description,
                        Uid = Guid.NewGuid().ToString(),
                    })
                    .ToList(),
                    Documents = x.EqualizationAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .FirstOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentEqualizationRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentEqualizationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<List<EqualizationGridViewModel>> GetListForPerson(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentEqualizationRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var list = await _context.Equalizations
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new EqualizationGridViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    ReasonId = x.ReasonId,
                    ReasonName = x.Reason.Name,
                    InClass = x.InClass,
                    BasicClassName = x.InClassNavigation.Description,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    Reason = x.Reason.Name,
                    EqualizatedGradesCount = x.EqualizationDetails.Count,
                    Documents = x.EqualizationAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .ToListAsync();

            return list;
        }

        public async Task Create(EqualizationModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentEqualizationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var entry = new Equalization
            {
                ReasonId = model.ReasonId,
                PersonId = model.PersonId,
                SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(_userInfo.InstitutionID),
                InClass = model.InClass,
                EqualizationDetails = model.EqualizationDetails?.OrderBy(x => x.SortOrder)
                    .Select(x => new EqualizationDetail
                    {
                        SubjectId = x.SubjectId,
                        SubjectTypeId = x.SubjectTypeId,
                        Position = x.SortOrder,
                        GradeCategory = x.GradeCategory,
                        Grade = x.GradeCategory == (int)GradeCategoryEnum.Normal || x.GradeCategory == (int)GradeCategoryEnum.Qualitative
                            ? x.Grade
                            : null,
                        SpecialNeedsGrade = x.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                            ? x.SpecialNeedsGrade
                            : null,
                        OtherGrade = x.GradeCategory == (int)GradeCategoryEnum.Other
                            ? x.OtherGrade
                            : null,
                        BasicClassId = x.BasicClassId,
                        Term = x.Term,
                        Horarium = x.Horarium,
                        SessionId = x.SessionId
                    })
                    .ToList()
            };

            using var transaction = _context.Database.BeginTransaction();
            _context.Equalizations.Add(entry);
            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(EqualizationModel model)
        {
            Equalization entity = await _context.Equalizations
                .Include(e => e.EqualizationDetails)
                .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentEqualizationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.ReasonId = model.ReasonId;
            entity.InClass = model.InClass;
            entity.SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

            using var transaction = _context.Database.BeginTransaction();

            HashSet<int> existedIds = model.EqualizationDetails != null
                ? model.EqualizationDetails.Where(x => x.Id.Value != 0).Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.EqualizationDetails.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // Оценки за изтриване
                _context.EqualizationDetails.RemoveRange(toDelete);
            }

            if (model.EqualizationDetails != null)
            {
                // Оценки за добавяне
                var toAdd = model.EqualizationDetails.Where(x => !(x.Id.Value != 0));
                if (toAdd.Any())
                {
                    _context.EqualizationDetails.AddRange(toAdd.Select(x => new EqualizationDetail
                    {
                        SubjectId = x.SubjectId,
                        SubjectTypeId = x.SubjectTypeId,
                        EqualizationId = entity.Id,
                        Position = x.SortOrder,
                        BasicClassId = x.BasicClassId,
                        Term = x.Term,
                        GradeCategory = x.GradeCategory,
                        Grade = x.GradeCategory == (int)GradeCategoryEnum.Normal || x.GradeCategory == (int)GradeCategoryEnum.Qualitative
                            ? x.Grade
                            : null,
                        SpecialNeedsGrade = x.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                            ? x.SpecialNeedsGrade
                            : null,
                        OtherGrade = x.GradeCategory == (int)GradeCategoryEnum.Other
                            ? x.OtherGrade
                            : null,
                        Horarium = x.Horarium,
                        SessionId = x.SessionId
                    }));

                }
            }

            if (existedIds.Any())
            {
                // Оценки за редакция
                foreach (var toUpdate in entity.EqualizationDetails.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.EqualizationDetails.SingleOrDefault(x => x.Id.Value != 0 && x.Id.Value == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.SubjectId = source.SubjectId;
                    toUpdate.SubjectTypeId = source.SubjectTypeId;
                    toUpdate.Position = source.SortOrder;
                    toUpdate.BasicClassId = source.BasicClassId;
                    toUpdate.Term = source.Term;
                    toUpdate.GradeCategory = source.GradeCategory;
                    toUpdate.Grade = source.GradeCategory == (int)GradeCategoryEnum.Normal || source.GradeCategory == (int)GradeCategoryEnum.Qualitative
                          ? source.Grade
                          : null;
                    toUpdate.SpecialNeedsGrade = source.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                          ? source.SpecialNeedsGrade
                          : null;
                    toUpdate.OtherGrade = source.GradeCategory == (int)GradeCategoryEnum.Other
                          ? source.OtherGrade
                          : null;
                    toUpdate.Horarium = source.Horarium;
                    toUpdate.SessionId = source.SessionId;
                }
            }

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id)
        {
            Equalization entity = await _context.Equalizations
                .Include(x => x.EqualizationAttachments)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentEqualizationManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.EqualizationAttachments != null && entity.EqualizationAttachments.Any())
            {
                var docsIds = entity.EqualizationAttachments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.EqualizationAttachments.RemoveRange(entity.EqualizationAttachments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            _context.Equalizations.Remove(entity);

            await SaveAsync();
        }

        public async Task<List<DropdownViewModel>> GetEqualizationReasonTypeClasses()
        {
            string equalizationReasonTypeClassesStr = await _configurationService.GetValueByKey("EqualizationReasonTypeClasses");
            Dictionary<int, HashSet<int>> equalizationReasonTypeClasses = JsonConvert.DeserializeObject<Dictionary<int, HashSet<int>>>(equalizationReasonTypeClassesStr ?? "");
            var data = await _context.BasicClasses
                .AsNoTracking()
                .Where(bc => equalizationReasonTypeClasses.First().Value.Any(e => e == bc.BasicClassId))
                 .Select(bc => new DropdownViewModel
                 {
                     Value = bc.BasicClassId,
                     Name = bc.Name,
                     Text = bc.Description
                 }).ToListAsync();

            return data;
        }
    }
}
