using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MON.DataAccess;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.Enums;
using MON.Models.Grid;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MON.Services.Implementations
{
    public class AdmissionPermissionRequestService : BaseService<AdmissionPermissionRequestService>, IAdmissionPermissionRequestService
    {
        private readonly IInstitutionService _institutionService;
        private readonly IBlobService _blobService;
        private readonly BlobServiceConfig _blobServiceConfig;

        public AdmissionPermissionRequestService(DbServiceDependencies<AdmissionPermissionRequestService> dependencies,
            IInstitutionService institutionService,
            IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _institutionService = institutionService;
            _blobService = blobService;
            _blobServiceConfig = blobServiceConfig.Value;
        }

        #region Private members
        private async Task ProcessAddedDocs(AdmissionPermissionRequestModel model, AdmissionPermissionRequest entity)
        {
            if (model.Documents == null || !model.Documents.Any() || entity == null) return;

            foreach (DocumentModel docModel in model.Documents.Where(x => x.HasToAdd()))
            {
                var result = await _blobService.UploadFileAsync(docModel.NoteContents, docModel.NoteFileName, docModel.NoteFileType);
                entity.AdmissionPermissionRequestAttachments.Add(new AdmissionPermissionRequestAttachment
                {
                    Document = docModel.ToDocument(result?.Data.BlobId)
                });
            }
        }

        private async Task ProcessDeletedDocs(AdmissionPermissionRequestModel model, AdmissionPermissionRequest entry)
        {
            if (model.Documents == null || !model.Documents.Any() || entry == null) return;

            HashSet<int> docIdsToDelete = model.Documents
            .Where(x => x.Id.HasValue && x.Deleted == true)
            .Select(x => x.Id.Value).ToHashSet();

            await _context.AdmissionPermissionRequestAttachments.Where(x => docIdsToDelete.Contains(x.DocumentId)).DeleteAsync();
            await _context.Documents.Where(x => docIdsToDelete.Contains(x.Id)).DeleteAsync();
        }
        #endregion

        public async Task<AdmissionPermissionRequestModel> GetById(int id)
        {
            AdmissionPermissionRequestModel model = await _context.AdmissionPermissionRequests
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new AdmissionPermissionRequestModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    RequestingInstitutionId = x.RequestingInstitutionId,
                    AuthorizingInstitutionId = x.AuthorizingInstitutionId,
                    Note = x.Note,
                    IsPermissionGranted = x.IsPermissionGranted,
                    Documents = x.AdmissionPermissionRequestAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .SingleOrDefaultAsync();
            // Методът се използва при Details и Edit
            if (model != null
                && !await _authorizationService.HasPermissionForInstitution(_userInfo?.InstitutionID ?? default, DefaultPermissions.PermissionNameForAdmissionPermissionRequestRead)
                && !await _authorizationService.HasPermissionForInstitution(_userInfo?.InstitutionID ?? default, DefaultPermissions.PermissionNameForAdmissionPermissionRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return model;
        }

        public async Task<IPagedList<AdmissionPermissionRequestViewModel>> List(AdmissionDocRequestListInput input)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(input), nameof(AdmissionDocRequestListInput)));
            }

            if (!await _authorizationService.HasPermissionForInstitution(_userInfo?.InstitutionID ?? default, DefaultPermissions.PermissionNameForAdmissionPermissionRequestRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            // Todo: вътрешните селекти да се махнат като синхронизираме продукционната база и се появят необходимите референции
            int userInstitutionId = _userInfo.InstitutionID.Value;

            IQueryable<AdmissionPermissionRequest> query = _context.AdmissionPermissionRequests
                .Where(x => x.RequestingInstitutionId == userInstitutionId || x.AuthorizingInstitutionId == userInstitutionId);
               
            switch (input.ListFilter)
            {
                // Промени в списък с искания за преместване #1251
                // От списъците, филтрирани по Изпратени или Получени да се махнат приключилите.

                case 0: // Изпратени
                    query = query.Where(x => x.RequestingInstitutionId == userInstitutionId)
                        .Where(x => !x.AdmissionDocumentId.HasValue
                            && !(x.IsPermissionGranted
                               && x.AuthorizingInstitutionSchoolYear > x.RequestingInstitutionSchoolYear
                               && _context.EducationalStates.Any(e => e.InstitutionId == x.AuthorizingInstitution.InstitutionId
                                                                && e.PersonId == x.PersonId
                                                               && (e.PositionId == (int)PositionType.Discharged || !e.PositionId.HasValue)))

                        );
                    break;
                case 1: // Получени
                    query = query.Where(x => x.AuthorizingInstitutionId == userInstitutionId)
                         .Where(x => !x.RelocationDocumentId.HasValue && !x.DischargeDocumentId.HasValue
                            && !(x.IsPermissionGranted
                               && x.AuthorizingInstitutionSchoolYear > x.RequestingInstitutionSchoolYear
                               && _context.EducationalStates.Any(e => e.InstitutionId == x.AuthorizingInstitution.InstitutionId
                                                                && e.PersonId == x.PersonId
                                                               && (e.PositionId == (int)PositionType.Discharged || !e.PositionId.HasValue)))

                        );
                    break;
                case 2: // Приключени
                    query = query.Where(x => (x.AdmissionDocumentId.HasValue && x.AuthorizingInstitutionId == userInstitutionId && (x.RelocationDocumentId.HasValue || x.DischargeDocumentId.HasValue))
                            || (x.AdmissionDocumentId.HasValue && x.RequestingInstitutionId == userInstitutionId)
                           || (x.IsPermissionGranted
                               && x.AuthorizingInstitutionSchoolYear > x.RequestingInstitutionSchoolYear
                               && _context.EducationalStates.Any(e => e.InstitutionId == x.AuthorizingInstitution.InstitutionId
                                                                && e.PersonId == x.PersonId
                                                               && (e.PositionId == (int)PositionType.Discharged || !e.PositionId.HasValue))
                               )
                           );
                    break;
                case 3: // Всики(Изпратени и Получени)
                default:
                    break;
            }

            IQueryable<AdmissionPermissionRequestViewModel> listQuery = query
                .Select(x => new AdmissionPermissionRequestViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    RequestingInstitutionId = x.RequestingInstitutionId,
                    RequestingInstitutionName = x.RequestingInstitution.Name,
                    RequestingInstitutionAbbreviation = x.RequestingInstitution.Abbreviation,
                    RequestingInstitutionTown = x.RequestingInstitution.Town.Name,
                    RequestingInstitutionMunicipality = x.RequestingInstitution.Town.Municipality.Name,
                    RequestingInstitutionRegion = x.RequestingInstitution.Town.Municipality.Region.Name,
                    AuthorizingInstitutionId = x.AuthorizingInstitutionId,
                    AuthorizingInstitutionName = x.AuthorizingInstitution.Name,
                    AuthorizingInstitutionAbbreviation = x.AuthorizingInstitution.Abbreviation,
                    Note = x.Note,
                    IsPermissionGranted = x.IsPermissionGranted,
                    PersonName = x.Person.FirstName + " " + x.Person.MiddleName + " " + x.Person.LastName,
                    CreateDate = x.CreateDate,
                    Documents = x.AdmissionPermissionRequestAttachments
                        .Select(d => d.Document.ToViewModel(_blobServiceConfig))
                })
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Note.Contains(input.Filter)
                   || predicate.RequestingInstitutionName.Contains(input.Filter)
                   || predicate.AuthorizingInstitutionName.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "IsPermissionGranted asc, CreateDate desc" : input.SortBy);

            int totalCount = await listQuery.CountAsync();
            IList<AdmissionPermissionRequestViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<int> CountPending()
        {
            if (!_userInfo.InstitutionID.HasValue)
            {
                return 0;
            }

            int institutionId = _userInfo.InstitutionID.Value;
            return await _context.AdmissionPermissionRequests
                .CountAsync(x => x.AuthorizingInstitutionId == institutionId && x.IsPermissionGranted == false);
        }

        public async Task Create(AdmissionPermissionRequestModel model)
        {
            if (!await _authorizationService.HasPermissionForInstitution(_userInfo?.InstitutionID ?? default, DefaultPermissions.PermissionNameForAdmissionPermissionRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError, new ArgumentNullException(nameof(model), nameof(AdmissionPermissionRequestModel)));

            }

            if (!_userInfo.InstitutionID.HasValue || model.RequestingInstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _context.AdmissionPermissionRequests.AnyAsync(x => x.PersonId == model.PersonId
                && x.RequestingInstitutionId == model.RequestingInstitutionId
                && x.AuthorizingInstitutionId == model.AuthorizingInstitutionId
                && !x.IsPermissionGranted))
            {
                throw new ApiException($"Вече е поискано разрешение за създаване на документ за записване, което не е одобрено. {Messages.AdmissionPermissionRequestInfo}");
            }

            var currentEduState = await _context.EducationalStates
                .Where(x => x.PersonId == model.PersonId && x.InstitutionId == model.AuthorizingInstitutionId
                    && (x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds))
                .Select(x => new
                {
                    Id = x.EducationalStateId
                })
                .FirstOrDefaultAsync();

            if (currentEduState == null)
            {
                throw new ApiException("Не е намерен записан ученик с позиция учащ в EduState на текущата институция.");
            }

            short authorizingInstitutionSchoolYear = await _institutionService.GetCurrentYear(model.AuthorizingInstitutionId);
            short requestingInstitutionSchoolYear = await _institutionService.GetCurrentYear(model.RequestingInstitutionId);

            AdmissionPermissionRequest entry = new AdmissionPermissionRequest
            {
                PersonId = model.PersonId,
                RequestingInstitutionId = model.RequestingInstitutionId,
                AuthorizingInstitutionId = model.AuthorizingInstitutionId,
                Note = model.Note,
                FirstEducationalStateId = currentEduState.Id,
                AuthorizingInstitutionSchoolYear = authorizingInstitutionSchoolYear,
                RequestingInstitutionSchoolYear = requestingInstitutionSchoolYear
            };

            using var transaction = _context.Database.BeginTransaction();
            _context.AdmissionPermissionRequests.Add(entry);
            await ProcessAddedDocs(model, entry);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(AdmissionPermissionRequestModel model)
        {
            if (!await _authorizationService.HasPermissionForInstitution(_userInfo?.InstitutionID ?? default, DefaultPermissions.PermissionNameForAdmissionPermissionRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            AdmissionPermissionRequest entity = await _context.AdmissionPermissionRequests
                .Include(x => x.AdmissionPermissionRequestAttachments)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!_userInfo.InstitutionID.HasValue || entity.RequestingInstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.IsPermissionGranted || entity.AdmissionDocumentId.HasValue || entity.DischargeDocumentId.HasValue)
            {
                throw new ApiException($"Разрешението за създаване на документ за записване е използвано и не може да се променя. {Messages.AdmissionPermissionRequestInfo}");
            }

            using var transaction = _context.Database.BeginTransaction();

            entity.Note = model.Note;

            await ProcessAddedDocs(model, entity);
            await ProcessDeletedDocs(model, entity);

            await SaveAsync();
            await transaction.CommitAsync();
        }

        public async Task Delete(int id)
        {
            if (!await _authorizationService.HasPermissionForInstitution(_userInfo?.InstitutionID ?? default, DefaultPermissions.PermissionNameForAdmissionPermissionRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            AdmissionPermissionRequest entity = await _context.AdmissionPermissionRequests
                .Include(x => x.AdmissionPermissionRequestAttachments)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!_userInfo.InstitutionID.HasValue || entity.RequestingInstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.IsPermissionGranted || entity.AdmissionDocumentId.HasValue || entity.DischargeDocumentId.HasValue)
            {
                throw new ApiException($"Разрешението за създаване на документ за записване е използвано и не може да се изтрива. {Messages.AdmissionPermissionRequestInfo}");
            }

            if (entity.AdmissionPermissionRequestAttachments != null && entity.AdmissionPermissionRequestAttachments.Any())
            {
                var docsIds = entity.AdmissionPermissionRequestAttachments.Select(x => x.DocumentId)
                   .ToHashSet();

                _context.AdmissionPermissionRequestAttachments.RemoveRange(entity.AdmissionPermissionRequestAttachments);

                // Изтриване на свързаните student.Document (docs content)
                var docsContentToDelete = await _context.Documents.Where(x => docsIds.Contains(x.Id))
                    .ToListAsync();
                if (docsContentToDelete.Any())
                {
                    _context.Documents.RemoveRange(docsContentToDelete);
                }
            }

            _context.AdmissionPermissionRequests.Remove(entity);

            await SaveAsync();
        }

        public async Task Confirm(int id)
        {
            if (!await _authorizationService.HasPermissionForInstitution(_userInfo?.InstitutionID ?? default, DefaultPermissions.PermissionNameForAdmissionPermissionRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            AdmissionPermissionRequest entity = await _context.AdmissionPermissionRequests
                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!_userInfo.InstitutionID.HasValue || entity.AuthorizingInstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.IsPermissionGranted)
            {
                throw new ApiException($"Резрешението вече е дадено. {Messages.AdmissionPermissionRequestInfo}");
            }

            entity.IsPermissionGranted = true;

            await SaveAsync();
        }
    }
}
