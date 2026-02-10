namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Report.Model.Enums;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class ReassessmentService : BaseService<ReassessmentService>, IReassessmentService
    {
        private readonly IInstitutionService _institutionService;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        public ReassessmentService(DbServiceDependencies<ReassessmentService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IInstitutionService institutionService) 
            : base(dependencies)
        {
            _institutionService = institutionService;
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        public async Task<ReassessmentModel> GetByIdAsync(int reassessmentId)
        {
            ReassessmentModel model = await _context.Reassessments
                .Where(x => x.Id == reassessmentId)
                .Select(x => new ReassessmentModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    ReasonId = x.ReasonId,
                    ReasonName = x.Reason.Name,
                    InClass = x.BasicClassId,
                    BasicClassName = x.BasicClass.Description,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    ReassessmentDetails = x.ReassessmentDetails.Select(e => new ReassessmentDetailsModel
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
                        SortOrder = e.Position,
                        QualitativeGrade = e.GradeCategory == (int)GradeCategoryEnum.Qualitative ? e.Grade : null,
                        Uid = Guid.NewGuid().ToString(),
                    })
                    .ToList(),
                    Documents = x.ReassessmentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .FirstOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentReassessmentRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentReassessmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<List<ReassessmentModel>> GetListForPersonAsync(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentReassessmentRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var list = await _context.Reassessments
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new ReassessmentModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    ReasonId = x.ReasonId,
                    Reason = x.Reason.Name,
                    InClass = x.BasicClassId,
                    BasicClassName = x.BasicClass.Description,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    CreationDate = x.CreateDate,
                    Documents = x.ReassessmentDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .ToListAsync();

            return list;
        }

        public async Task Create(ReassessmentModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentReassessmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            var entry = new Reassessment
            {
                ReasonId = model.ReasonId,
                PersonId = model.PersonId,
                SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(_userInfo.InstitutionID),
                BasicClassId = model.InClass,
                ReassessmentDetails = model.ReassessmentDetails?.OrderBy(x => x.SortOrder)
                    .Select(rd => new ReassessmentDetail
                    {
                        SubjectId = rd.SubjectId,
                        SubjectTypeId = rd.SubjectTypeId,
                        Position = rd.SortOrder,
                        GradeCategory = rd.GradeCategory,
                        Grade = rd.GradeCategory == (int)GradeCategoryEnum.Normal
                            ? rd.Grade
                            : rd.GradeCategory == (int)GradeCategoryEnum.Qualitative
                            ? rd.QualitativeGrade
                            : null,
                        SpecialNeedsGrade = rd.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                            ? rd.SpecialNeedsGrade
                            : null,
                        OtherGrade = rd.GradeCategory == (int)GradeCategoryEnum.Other
                            ? rd.OtherGrade
                            : null,
                    })
                    .ToList()
            };

            using var transaction = _context.Database.BeginTransaction();
            _context.Reassessments.Add(entry);
            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(ReassessmentModel model)
        {
            Reassessment entity = await _context.Reassessments
                .Include(r => r.ReassessmentDetails)
                .SingleOrDefaultAsync(d => d.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentReassessmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.ReasonId = model.ReasonId;
            entity.BasicClassId = model.InClass;
            entity.SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

            using var transaction = _context.Database.BeginTransaction();

            HashSet<int> existedIds = 
                model.ReassessmentDetails?.Where(x => x.Id.Value != 0).Select(x => x.Id.Value).ToHashSet() ?? new HashSet<int>();

            var toDelete = entity.ReassessmentDetails.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // Оценки за изтриване
                _context.ReassessmentDetails.RemoveRange(toDelete);
            }

            if (model.ReassessmentDetails != null)
            {
                // Оценки за добавяне
                var toAdd = model.ReassessmentDetails.Where(x => !(x.Id.Value != 0));
                if (toAdd.Any())
                {
                    _context.ReassessmentDetails.AddRange(toAdd.Select(x => new ReassessmentDetail
                    {
                        SubjectId = x.SubjectId,
                        SubjectTypeId = x.SubjectTypeId,
                        ReassessmentId = entity.Id,
                        Position = x.SortOrder,
                        GradeCategory = x.GradeCategory,
                        Grade = x.GradeCategory == (int)GradeCategoryEnum.Normal 
                            ? x.Grade
                            : x.GradeCategory == (int)GradeCategoryEnum.Qualitative
                            ? x.QualitativeGrade
                            : null,
                        SpecialNeedsGrade = x.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                            ? x.SpecialNeedsGrade
                            : null,
                        OtherGrade = x.GradeCategory == (int)GradeCategoryEnum.Other
                            ? x.OtherGrade
                            : null,
                    }));

                }
            }

            if (existedIds.Any())
            {
                // Оценки за редакция
                foreach (var toUpdate in entity.ReassessmentDetails.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.ReassessmentDetails.SingleOrDefault(x => x.Id.Value != 0 && x.Id.Value == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.SubjectId = source.SubjectId;
                    toUpdate.SubjectTypeId = source.SubjectTypeId;
                    toUpdate.Position = source.SortOrder;
                    toUpdate.GradeCategory = source.GradeCategory;
                    toUpdate.Grade = source.GradeCategory == (int)GradeCategoryEnum.Normal
                            ? source.Grade
                            : source.GradeCategory == (int)GradeCategoryEnum.Qualitative
                            ? source.QualitativeGrade
                            : null;
                    toUpdate.SpecialNeedsGrade = source.GradeCategory == (int)GradeCategoryEnum.SpecialNeeds
                          ? source.SpecialNeedsGrade
                          : null;
                    toUpdate.OtherGrade = source.GradeCategory == (int)GradeCategoryEnum.Other
                          ? source.OtherGrade
                          : null;
                }
            }

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id)
        {
            Reassessment entity = await _context.Reassessments
                .Include(x => x.ReassessmentDocuments)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentReassessmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.ReassessmentDocuments != null && entity.ReassessmentDocuments.Any())
            {
                var docsIds = entity.ReassessmentDocuments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.ReassessmentDocuments.RemoveRange(entity.ReassessmentDocuments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            _context.Reassessments.Remove(entity);

            await SaveAsync();
        }

        #region Private members
        private async Task ProcessAddedDocs(ReassessmentModel model, Reassessment entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.ReassessmentDocuments.Add(new ReassessmentDocument
                {
                    Document = docModel.ToDocument(result?.Data.BlobId),
                });
            }
        }

        private async Task ProcessDeletedDocs(ReassessmentModel model, Reassessment entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.ReassessmentDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }
        #endregion
    }
}
