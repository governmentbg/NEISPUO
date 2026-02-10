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
    using MON.Shared;
    using MON.Models.Grid;
    using MON.Shared.Interfaces;
    using System.Linq.Dynamic.Core;
    using MON.Models.Diploma;
    using MON.Shared.Enums;
    using DocumentFormat.OpenXml.Bibliography;

    public class StudentSanctionService : BaseService<StudentSanctionService>, IStudentSanctionService
    {
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;
        private readonly ISchoolBooksService _schoolBooksService;
        private readonly IInstitutionService _institutionService;

        public StudentSanctionService(DbServiceDependencies<StudentSanctionService> dependencies,
            IBlobService blobService,
            ISchoolBooksService schoolBooksService,
            IOptions<BlobServiceConfig> blobServiceConfig,
            IInstitutionService institutionService)
            : base(dependencies)
        {
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
            _schoolBooksService = schoolBooksService;
            _institutionService = institutionService;
        }

        #region Private members
        private async Task ProcessAddedDocs(StudentSanctionModel model, Sanction entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null)
            {
                return;
            }

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.SanctionDocuments.Add(new SanctionDocument
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(StudentSanctionModel model, Sanction sanction)
        {
            if (model.Documents == null || !model.Documents.Any() || sanction == null)
            {
                return;
            }

            HashSet<int> docIdsToDelete = model.Documents
                .Where(x => x.Id.HasValue && x.Deleted == true)
                .Select(x => x.Id.Value).ToHashSet();

            await _context.SanctionDocuments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }
        #endregion

        public async Task<StudentSanctionModel> GetById(int id)
        {
            StudentSanctionModel model = await _context.Sanctions.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new StudentSanctionModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    SanctionTypeId = x.SanctionTypeId,
                    CancelOrderDate = x.CancelOrderDate,
                    CancelOrderNumber = x.CancelOrderNumber,
                    CancelReason = x.CancelReason,
                    OrderDate = x.OrderDate,
                    OrderNumber = x.OrderNumber,
                    RuoOrderDate = x.RuoOrderDate,
                    RuoOrderNumber = x.RuoOrderNumber,
                    EndDate = x.EndDate,
                    StartDate = x.StartDate,
                    Description = x.Description,
                    SourceType = x.SourceType,
                    Documents = x.SanctionDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                }).SingleOrDefaultAsync();

            if (model == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentSanctionRead)
                && !await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentSanctionManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<IPagedList<StudentSanctionViewModel>> List(SanctionsListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(input.PersonId ?? default, DefaultPermissions.PermissionNameForStudentSanctionRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<Sanction> query = _context.Sanctions
                .AsNoTracking()
                .Where(x => x.PersonId == input.PersonId);

            IQueryable<StudentSanctionViewModel> listQuery = query.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.InstitutionSchoolYear.Abbreviation.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.InstitutionSchoolYear.SchoolYearNavigation.Name.Contains(input.Filter))
                .Select(x => new StudentSanctionViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    CancelOrderDate = x.CancelOrderDate,
                    CancelOrderNumber = x.CancelOrderNumber,
                    CancelReason = x.CancelReason,
                    OrderDate = x.OrderDate,
                    OrderNumber = x.OrderNumber,
                    RuoOrderDate = x.RuoOrderDate,
                    RuoOrderNumber = x.RuoOrderNumber,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Description = x.Description,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                    SanctionTypeName = x.SanctionType.Name,
                    SchoolYear = x.SchoolYear,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    SourceType = x.SourceType,
                    Documents = x.SanctionDocuments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
            .OrderBy(input.SortBy.IsNullOrWhiteSpace() ? "Id desc" : input.SortBy);

            int totalCount = listQuery.Count();
            IList<StudentSanctionViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task Create(StudentSanctionModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentSanctionManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            Sanction entry = new Sanction
            {
                PersonId = model.PersonId,
                SanctionTypeId = model.SanctionTypeId,
                InstitutionId = _userInfo.InstitutionID,
                SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(_userInfo.InstitutionID),
                CancelOrderDate = model.CancelOrderDate,
                CancelOrderNumber = model.CancelOrderNumber,
                CancelReason = model.CancelReason,
                OrderDate = model.OrderDate,
                OrderNumber = model.OrderNumber,
                RuoOrderDate = model.RuoOrderDate,
                RuoOrderNumber = model.RuoOrderNumber,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Description = model.Description,
                SourceType = (byte)SanctionSourceType.Manual
            };

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entry);
            _context.Sanctions.Add(entry);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(StudentSanctionModel model)
        {
            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            Sanction entity = await _context.Sanctions
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentSanctionManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            entity.SanctionTypeId = model.SanctionTypeId;
            entity.StartDate = model.StartDate;
            entity.Description = model.Description;
            entity.EndDate = model.EndDate;
            entity.CancelOrderDate = model.CancelOrderDate;
            entity.CancelOrderNumber = model.CancelOrderNumber;
            entity.CancelReason = model.CancelReason;
            entity.OrderDate = model.OrderDate;
            entity.OrderNumber = model.OrderNumber;
            entity.RuoOrderDate = model.RuoOrderDate;
            entity.RuoOrderNumber = model.RuoOrderNumber;
            entity.InstitutionId = model.InstitutionId ?? _userInfo.InstitutionID;
            entity.SchoolYear = model.SchoolYear ?? await _institutionService.GetCurrentYear(model.InstitutionId ?? _userInfo.InstitutionID);

            using var transaction = _context.Database.BeginTransaction();
            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int sanctionId)
        {
            Sanction entity = await _context.Sanctions
                .Include(x => x.SanctionDocuments)
                .SingleOrDefaultAsync(x => x.Id == sanctionId);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!await _authorizationService.HasPermissionForStudent(entity.PersonId, DefaultPermissions.PermissionNameForStudentSanctionManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity != null)
            {
                _context.SanctionDocuments.RemoveRange(entity.SanctionDocuments);
                _context.Sanctions.Remove(entity);
                await SaveAsync();
            }
        }

        public async Task Import(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentSanctionManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            // Импортираме само за текущата за институцията учебна година.
            short schoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Изтриваме старите.
                await _context.Sanctions
                    .Where(x => x.PersonId == personId && x.SchoolYear == schoolYear
                        && x.InstitutionId == _userInfo.InstitutionID
                        && x.SourceType != null
                        && x.SourceType == (byte)SanctionSourceType.SchoolBook)
                    .DeleteAsync();

                List<StudentSanctionModel> toImport = await _schoolBooksService.GetSchoolBooksSanctions(personId, schoolYear, _userInfo.InstitutionID);
                foreach (var sanction in toImport)
                {
                    _context.Sanctions.Add(new Sanction
                    {
                        CancelOrderDate = sanction.CancelOrderDate,
                        CancelOrderNumber = sanction.CancelOrderNumber,
                        CancelReason = sanction.CancelReason,
                        Description = sanction.Description,
                        EndDate = sanction.EndDate,
                        OrderDate = sanction.OrderDate,
                        OrderNumber = sanction.OrderNumber,
                        PersonId = personId,
                        RuoOrderDate = sanction.RuoOrderDate,
                        RuoOrderNumber = sanction.RuoOrderNumber,
                        StartDate = sanction.StartDate,
                        SanctionTypeId = sanction.SanctionTypeId,
                        SchoolYear = sanction.SchoolYear ?? schoolYear,
                        InstitutionId = sanction.InstitutionId ?? _userInfo.InstitutionID,
                        SourceType = (byte)SanctionSourceType.SchoolBook
                    });
                }

                await SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(ex.GetInnerMostException().ToString(), ex);
            }
        }      
    }
}
