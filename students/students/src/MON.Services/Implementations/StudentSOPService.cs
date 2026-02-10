namespace MON.Services.Implementations
{
    using DocumentFormat.OpenXml.Office2010.Excel;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.Enums;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Update;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared;
    using MON.Shared.ErrorHandling;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class StudentSOPService : BaseService<StudentSOPService>, IStudentSOPService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly ILodFinalizationService _lodFinalizationService;

        public StudentSOPService(DbServiceDependencies<StudentSOPService> dependencies,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            ILodFinalizationService lodFinalizationService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _lodFinalizationService = lodFinalizationService;
        }

        public async Task<StudentSopUpdateModel> GetById(int id, CancellationToken cancellationToken)
        {
            StudentSopUpdateModel model = await _context.SpecialNeedsYears
                .Where(x => x.Id == id)
                .Select(x => new StudentSopUpdateModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    SchoolYear = x.SchoolYear,
                    SopDetails = x.SpecialNeeds.Select(i => new SopDetailsViewModel
                    {
                        Id = i.Id,
                        SpecialNeedsTypeId = i.SpecialNeedsTypeId,
                        SpecialNeedsSubTypeId = i.SpecialNeedsSubTypeId,
                        SopTypeName = i.SpecialNeedsType.Name,
                        SopSubTypeName = i.SpecialNeedsSubType.Name
                    }),
                    Documents = x.SpecialNeedsYearAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .FirstOrDefaultAsync(cancellationToken);

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentSopRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentSopManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<List<SopViewModel>> GetListForPerson(int personId, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentSopRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await (from x in _context.SpecialNeedsYears
                          join lf in _context.Lodfinalizations on new { x.PersonId, x.SchoolYear } equals new { lf.PersonId, lf.SchoolYear } into temp
                          from lodFin in temp.DefaultIfEmpty()
                          join apds in _context.AdditionalPersonalDevelopmentSupports.Where(adps => adps.StudentTypeId == (int)PersonalDevelopmentSupportStudentTypeEnum.SOP) on new { x.PersonId, x.SchoolYear } equals new { apds.PersonId, apds.SchoolYear } into temp1
                          from apds in temp1.DefaultIfEmpty()
                          where x.PersonId == personId
                          orderby x.SchoolYear descending
                          select new SopViewModel
                          {
                              Id = x.Id,
                              SchoolYear = x.SchoolYear,
                              SchoolYearName = x.SchoolYearNavigation.Name,
                              SopDetails = x.SpecialNeeds.Select(i => new SopDetailsViewModel
                              {
                                  Id = i.Id,
                                  SopTypeName = i.SpecialNeedsType.Name,
                                  SopSubTypeName = i.SpecialNeedsSubType.Name
                              }),
                              Documents = x.SpecialNeedsYearAttachments.Select(x => x.Document.ToViewModel(_blobServiceConfig)),
                              IsLodFinalized = lodFin != null && lodFin.IsFinalized,
                              RelatedAdditionalPersonalDevelopmentSupportId = apds.Id,
                          })
                          .ToListAsync(cancellationToken);
        }

        public async Task Create(StudentSopUpdateModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentSopManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.SopDetails.IsNullOrEmpty())
            {
                throw new ApiException("Необходимо е въвеждането на поне една Специална образователна потребност!");
            }

            if (await _context.SpecialNeedsYears.AnyAsync(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear))
            {
                throw new ApiException("СОП за този ученик и учебна година вече съществува!");
            }

            var entry = new SpecialNeedsYear
            {
                PersonId = model.PersonId,
                SchoolYear = model.SchoolYear,
                SpecialNeeds = model.SopDetails != null
                    ? model.SopDetails.Select(x => new SpecialNeed { SpecialNeedsTypeId = x.SpecialNeedsTypeId, SpecialNeedsSubTypeId = x.SpecialNeedsSubTypeId }).ToList()
                    : null
            };

            using var transaction = _context.Database.BeginTransaction();
            _context.SpecialNeedsYears.Add(entry);

            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(StudentSopUpdateModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            SpecialNeedsYear entity = await _context.SpecialNeedsYears
                .Include(x => x.SpecialNeeds)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError, new ArgumentNullException(nameof(entity), nameof(SpecialNeedsYear)));
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentSopManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError);
            }

            if (model.SopDetails.IsNullOrEmpty())
            {
                throw new ApiException("Необходимо е въвеждането на поне една Специална образователна потребност!");
            }

            if (await _context.SpecialNeedsYears.AnyAsync(x => x.Id != entity.Id
                && x.PersonId == model.PersonId
                && x.SchoolYear == model.SchoolYear))
            {
                throw new ApiException("СОП за този ученик и учебна година вече съществува!");
            }

            entity.SchoolYear = model.SchoolYear;

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            HashSet<int> existedIds = model.SopDetails != null
                ? model.SopDetails.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.SpecialNeeds.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // За изтриване
                _context.SpecialNeeds.RemoveRange(toDelete);
            }

            if (model.SopDetails != null)
            {
                // За добавяне
                var toAdd = model.SopDetails.Where(x => !x.Id.HasValue);
                if (toAdd.Any())
                {
                    _context.SpecialNeeds.AddRange(toAdd.Select(x => new SpecialNeed
                    {
                        SpecialNeedsYearId = entity.Id,
                        SpecialNeedsTypeId = x.SpecialNeedsTypeId,
                        SpecialNeedsSubTypeId = x.SpecialNeedsSubTypeId
                    }));
                }
            }

            if (existedIds.Any())
            {
                // За редакция
                foreach (var toUpdate in entity.SpecialNeeds.Where(x => existedIds.Contains(x.Id)))
                {
                    var source = model.SopDetails.SingleOrDefault(x => x.Id == toUpdate.Id);
                    if (source == null) continue;

                    toUpdate.SpecialNeedsTypeId = source.SpecialNeedsTypeId;
                    toUpdate.SpecialNeedsSubTypeId = source.SpecialNeedsSubTypeId;
                }
            }

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            SpecialNeedsYear entity = await _context.SpecialNeedsYears
                .Include(x => x.SpecialNeeds)
                .Include(x => x.SpecialNeedsYearAttachments)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentSopManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _context.AdditionalPersonalDevelopmentSupports.AnyAsync(x => x.PersonId == entity.PersonId
                && x.SchoolYear == entity.SchoolYear && x.StudentTypeId == (int)PersonalDevelopmentSupportStudentTypeEnum.SOP))
            {
                throw new ApiException($"Не е позволено изтриването на данни за СОП, свързани със запис за ДПЛР за този ученик и учебна година {entity.SchoolYear}. " +
                    $"Изтриването на данни за СОП се извършва от ЛОД, меню ПЛР, подменю ДПЛР.", 500);
            }

            if (entity.SpecialNeedsYearAttachments != null && entity.SpecialNeedsYearAttachments.Any())
            {
                var docsIds = entity.SpecialNeedsYearAttachments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.SpecialNeedsYearAttachments.RemoveRange(entity.SpecialNeedsYearAttachments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            if (entity.SpecialNeeds != null && entity.SpecialNeeds.Any())
            {
                _context.SpecialNeeds.RemoveRange(entity.SpecialNeeds);
            }

            _context.SpecialNeedsYears.Remove(entity);

            await SaveAsync(cancellationToken);
        }

        private async Task ProcessAddedDocs(StudentSopUpdateModel model, SpecialNeedsYear entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.SpecialNeedsYearAttachments.Add(new SpecialNeedsYearAttachment
                {
                    Document = docModel.ToDocument(result?.Data?.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(StudentSopUpdateModel model, SpecialNeedsYear entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            HashSet<int> blobsToDelete = model.Documents
                .Where(x => x.Id.HasValue && x.Deleted == true && x.BlobId.HasValue)
                .Select(x => x.BlobId.Value).ToHashSet();

            if (blobsToDelete.Count > 0)
            {
                await _context.AdditionalPersonalDevelopmentSupportAttachments
                   .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.SpecialNeedsYearAttachments
                    .Where(x => x.Document.BlobId.HasValue && blobsToDelete.Contains(x.Document.BlobId.Value)).DeleteAsync();
                await _context.Documents.Where(x => x.BlobId.HasValue && blobsToDelete.Contains(x.BlobId.Value)).DeleteAsync();
            }
        }
    }
}
