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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class StudentInternationalMobilityService : BaseService<StudentInternationalMobilityService>, IStudentInternationalMobilityService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;

        public StudentInternationalMobilityService(DbServiceDependencies<StudentInternationalMobilityService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
        : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        public async Task<InternationalMobilityModel> GetById(int internationalMobilityId)
        {
            InternationalMobilityModel model = await _context.InternationalMobilities
                .AsNoTracking()
                .Where(x => x.Id == internationalMobilityId).Select(x => new InternationalMobilityModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    ReceivingInstitution = x.ReceivingInstitution,
                    MainObjectives = x.MainObjectives,
                    Project = x.Project,
                    CountryId = x.CountryId,
                    InstitutionId = x.InstitutionId,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    Documents = x.InternationalMobilityDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig)),
                }).SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentInternationalMobilityRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentInternationalMobilityManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return model;
        }

        public async Task<List<InternationalMobilityViewModel>> GetByPersonId(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentInternationalMobilityRead))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var internationalMobilitiesList = await _context.InternationalMobilities
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new InternationalMobilityViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    ReceivingInstitution = x.ReceivingInstitution,
                    MainObjectives = x.MainObjectives,
                    Project = x.Project,
                    Country = x.Country.Name,
                    InstitutionId = x.InstitutionId,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    Documents = x.InternationalMobilityDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                }).ToListAsync();

            return internationalMobilitiesList;
        }

        public async Task Create(InternationalMobilityModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentInternationalProtectionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var internationalMobility = new InternationalMobility
            {
                CountryId = model.CountryId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ReceivingInstitution = model.ReceivingInstitution ?? "", // в базата колоната e not null
                MainObjectives = model.MainObjectives ?? "", // в базата колоната e not null,
                PersonId = model.PersonId,
                Project = model.Project,
                InstitutionId = _userInfo.InstitutionID,
                SchoolYear = model.SchoolYear
            };

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, internationalMobility);
            _context.InternationalMobilities.Add(internationalMobility);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(InternationalMobilityModel model)
        {
            InternationalMobility entity = await _context.InternationalMobilities.SingleOrDefaultAsync(x => x.Id == model.Id);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentInternationalProtectionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            entity.FromDate = model.FromDate;
            entity.ToDate = model.ToDate;
            entity.ReceivingInstitution = model.ReceivingInstitution ?? ""; // в базата колоната e not null;
            entity.CountryId = model.CountryId;
            entity.MainObjectives = model.MainObjectives ?? ""; // в базата колоната e not null;
            entity.Project = model.Project;
            entity.SchoolYear = model.SchoolYear;
            entity.InstitutionId = _userInfo.InstitutionID;

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int internationalMobilityId)
        {
            InternationalMobility entity = await _context.InternationalMobilities
                .Include(x => x.InternationalMobilityDocuments).ThenInclude(x => x.Document)
                .Where(x => x.Id == internationalMobilityId)
                .SingleOrDefaultAsync();

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentInternationalMobilityManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            IEnumerable<InternationalMobilityDocument> internationalMobilityDocumentsToDelete = entity.InternationalMobilityDocuments;
            IEnumerable<Document> documentsToDelete = internationalMobilityDocumentsToDelete.Select(d => d.Document);

            _context.InternationalMobilityDocuments.RemoveRange(internationalMobilityDocumentsToDelete);
            _context.Documents.RemoveRange(documentsToDelete);
            _context.InternationalMobilities.Remove(entity);
            await SaveAsync();
        }

        private async Task ProcessAddedDocs(InternationalMobilityModel model, InternationalMobility entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null)
            {
                return;
            }

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.InternationalMobilityDocuments.Add(new InternationalMobilityDocument
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(InternationalMobilityModel model, InternationalMobility award)
        {
            if (model.Documents == null || !model.Documents.Any() || award == null)
            {
                return;
            }

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.InternationalMobilityDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }
    }
}
