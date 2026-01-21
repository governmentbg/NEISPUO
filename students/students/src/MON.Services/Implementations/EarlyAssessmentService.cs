namespace MON.Services.Implementations
{
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using System.Collections.Generic;
    using MON.Models;
    using Z.EntityFramework.Plus;
    using MON.Models.Configuration;
    using Microsoft.Extensions.Options;

    public class EarlyAssessmentService : BaseService<EarlyAssessmentService>, IEarlyAssessmentService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;

        public EarlyAssessmentService(DbServiceDependencies<EarlyAssessmentService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        #region Private members
        private async Task ProcessAddedDocs(StudentEarlyAssessmentModel model, PersonalEarlyAssessment entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.PersonalEarlyAssessmentAttachments.Add(new PersonalEarlyAssessmentAttachment
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(StudentEarlyAssessmentModel model, PersonalEarlyAssessment entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.PersonalEarlyAssessmentAttachments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }
        #endregion

        public async Task<StudentEarlyAssessmentModel> GetByPerson(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            StudentEarlyAssessmentModel model = await _context.PersonalEarlyAssessments
                .Where(x => x.PersonId == personId)
                .Select(x => new StudentEarlyAssessmentModel
                {
                    PersonId = x.PersonId,
                    AdditionalInfo = x.AdditionalInfo,
                    BgAdditionalTrainingInfo = x.BgAdditionalTrainingInfo,
                    ConclusionInfo = x.ConclusionInfo,
                    LearningDisability = x.PersonalEarlyAssessmentLearningDisability != null
                        ? new StudentEarlyAssessmentLearningDisabilityModel
                        {
                            AgeRange = x.PersonalEarlyAssessmentLearningDisability.AgeRange,
                            Result = x.PersonalEarlyAssessmentLearningDisability.Result,
                            Details = x.PersonalEarlyAssessmentLearningDisability.Details,
                            Score = x.PersonalEarlyAssessmentLearningDisability.Score
                        }
                        : null,
                    DisabilityReasons = x.PersonalEarlyAssessmentDisabilityReasons
                        .Select(r => new StudentEarlyAssessmentDisabilityReasonModel
                        {
                            Id = r.Id,
                            ReasonId = r.ReasonId,
                            Details = r.Details,
                            ReasonName = r.Reason.Name
                        }).ToList(),
                    Documents = x.PersonalEarlyAssessmentAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .SingleOrDefaultAsync();

            // Не ни трябват null заради създаването/редакцията в UI-а
            if (model == null)
            {
                model = new StudentEarlyAssessmentModel
                {
                    PersonId = personId
                };
            }

            if (model.LearningDisability == null)
            {
                model.LearningDisability = new StudentEarlyAssessmentLearningDisabilityModel();
            }

            return model;
        }

        public async Task CreateOrUpdate(StudentEarlyAssessmentModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(StudentEarlyAssessmentModel)));
            }

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentPersonalDevelopmentManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            PersonalEarlyAssessment entity = await _context.PersonalEarlyAssessments
                .Include(x => x.PersonalEarlyAssessmentLearningDisability)
                .Include(x => x.PersonalEarlyAssessmentDisabilityReasons)
                .SingleOrDefaultAsync(x => x.PersonId == model.PersonId);

            if (entity == null)
            {
                entity = new PersonalEarlyAssessment
                {
                    PersonId = model.PersonId
                };
                _context.PersonalEarlyAssessments.Add(entity);
            }

            if (entity.PersonalEarlyAssessmentLearningDisability == null)
            {
                entity.PersonalEarlyAssessmentLearningDisability = new PersonalEarlyAssessmentLearningDisability();
            }

            entity.AdditionalInfo = model.AdditionalInfo;
            entity.BgAdditionalTrainingInfo = model.BgAdditionalTrainingInfo;
            entity.ConclusionInfo = model.ConclusionInfo;
            entity.PersonalEarlyAssessmentLearningDisability.AgeRange = model.LearningDisability?.AgeRange;
            entity.PersonalEarlyAssessmentLearningDisability.Result = model.LearningDisability?.Result;
            entity.PersonalEarlyAssessmentLearningDisability.Details = model.LearningDisability?.Details;
            entity.PersonalEarlyAssessmentLearningDisability.Score = model.LearningDisability?.Score;


            using var transaction = _context.Database.BeginTransaction();

            HashSet<int> selectedDisabilityReasonsIds = model.DisabilityReasons != null
                    ? model.DisabilityReasons.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                    : new HashSet<int>();

            var toDelete = entity.PersonalEarlyAssessmentDisabilityReasons.Where(x => !selectedDisabilityReasonsIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // Оценки за изтриване
                _context.PersonalEarlyAssessmentDisabilityReasons.RemoveRange(toDelete);
            }

            if (model.DisabilityReasons != null)
            {
                // Оценки за добавяне
                var toAdd = model.DisabilityReasons.Where(x => !x.Id.HasValue);
                foreach (var item in toAdd)
                {
                    entity.PersonalEarlyAssessmentDisabilityReasons.Add(new PersonalEarlyAssessmentDisabilityReason
                    {
                        ReasonId = item.ReasonId,
                        Details = item.Details,
                    });
                }
            }

            if (selectedDisabilityReasonsIds.Any())
            {
                // Оценки за редакция
                foreach (PersonalEarlyAssessmentDisabilityReason toUpdate in entity.PersonalEarlyAssessmentDisabilityReasons.Where(x => selectedDisabilityReasonsIds.Contains(x.Id)))
                {
                    var source = model.DisabilityReasons.SingleOrDefault(x => x.Id.HasValue && x.Id.Value == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.ReasonId = source.ReasonId;
                    toUpdate.Details = source.Details;
                }
            }

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }
    }
}
