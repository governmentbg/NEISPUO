namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class StudentAwardService : BaseService<StudentAwardService>, IStudentAwardService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly IInstitutionService _institutionService;

        public StudentAwardService(DbServiceDependencies<StudentAwardService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IInstitutionService institutionService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _institutionService = institutionService;
        }

        public async Task<StudentAwardModel> GetById(int id)
        {
            StudentAwardModel model = await _context.Awards.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new StudentAwardModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Date = x.Date,
                    Description = x.Description,
                    OrderNumber = x.OrderNumber,
                    AdditionalInformation = x.AdditionalInformation,
                    AwardTypeId = x.AwardTypeId,
                    AwardTypeName = x.AwardType.Name,
                    AwardCategoryId = x.AwardCategoryId,
                    AwardCategoryName = x.AwardCategory.Name,
                    AwardReasonId = x.AwardReasonId,
                    AwardReasonName = x.AwardReason.Name,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Name,
                    FounderId = x.FounderId,
                    FounderName = x.Founder.Name,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                    Documents = x.AwardDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                }).SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentAwardRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentAwardManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<IEnumerable<StudentAwardViewModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentAwardRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await (from x in _context.Awards
                          join lf in _context.Lodfinalizations on new { x.PersonId, x.SchoolYear } equals new { lf.PersonId, lf.SchoolYear } into temp
                          from lodFin in temp.DefaultIfEmpty()
                          where x.PersonId == personId
                          orderby x.SchoolYear descending, x.Id descending
                          select new StudentAwardViewModel
                          {
                              Id = x.Id,
                              PersonId = x.PersonId,
                              Date = x.Date,
                              Description = x.Description,
                              OrderNumber = x.OrderNumber,
                              AdditionalInformation = x.AdditionalInformation,
                              AwardTypeName = x.AwardType.Name,
                              AwardCategoryName = x.AwardCategory.Name,
                              AwardReasonName = x.AwardReason.Name,
                              InstitutionName = x.InstitutionSchoolYear.Name,
                              FounderName = x.Founder.Name,
                              SchoolYear = x.SchoolYear,
                              SchoolYearName = x.InstitutionSchoolYear.SchoolYearNavigation.Name,
                              Documents = x.AwardDocuments.Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                              IsLodFinalized = lodFin != null && lodFin.IsFinalized
                          }
                          ).ToListAsync();
        }

        public async Task Create(StudentAwardModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentAwardManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            short schoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(model?.InstitutionId ?? _userInfo?.InstitutionID);

            var newStudentAward = new Award
            {
                PersonId = model.PersonId,
                Date = model.Date,
                Description = model.Description,
                OrderNumber = model.OrderNumber,
                AdditionalInformation = model.AdditionalInformation,
                AwardTypeId = model.AwardTypeId,
                AwardCategoryId = model.AwardCategoryId,
                AwardReasonId = model.AwardReasonId,
                InstitutionId = model.InstitutionId,
                SchoolYear = schoolYear,
                FounderId = model.FounderId,
            };

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, newStudentAward);
            _context.Awards.Add(newStudentAward);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(StudentAwardModel model)
        {
            Award entity = await _context.Awards
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentAwardManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            short schoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(entity.InstitutionId ?? _userInfo?.InstitutionID);

            entity.Date = model.Date;
            entity.Description = model.Description;
            entity.OrderNumber = model.OrderNumber;
            entity.AdditionalInformation = model.AdditionalInformation;
            entity.AwardTypeId = model.AwardTypeId;
            entity.AwardCategoryId = model.AwardCategoryId;
            entity.AwardReasonId = model.AwardReasonId;
            entity.FounderId = model.FounderId;
            entity.SchoolYear = schoolYear;

            if (entity.InstitutionId != model.InstitutionId)
            {
                entity.InstitutionId = model.InstitutionId;
            }

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int awardId)
        {
            Award entity = await _context.Awards
                .Include(x => x.AwardDocuments)
                .SingleOrDefaultAsync(x => x.Id == awardId);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentAwardManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity != null)
            {
                _context.AwardDocuments.RemoveRange(entity.AwardDocuments);
                _context.Awards.Remove(entity);
                await SaveAsync();
            }
        }

        private async Task ProcessAddedDocs(StudentAwardModel model, Award entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null)
            {
                return;
            }

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.AwardDocuments.Add(new AwardDocument
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(StudentAwardModel model, Award award)
        {
            if (model.Documents == null || !model.Documents.Any() || award == null)
            {
                return;
            }

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.AwardDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }
    }
}
