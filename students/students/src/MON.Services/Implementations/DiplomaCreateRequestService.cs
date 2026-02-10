using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Models.Diploma;
using MON.Models.Enums;
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

namespace MON.Services.Implementations
{
    public class DiplomaCreateRequestService : BaseService<DiplomaCreateRequestService>, IDiplomaCreateRequestService
    {
        private IInstitutionService _institutionService;
        public DiplomaCreateRequestService(DbServiceDependencies<DiplomaCreateRequestService> dependencies, IInstitutionService institutionService)
            : base(dependencies)
        {
            _institutionService = institutionService;
        }

        public async Task<IPagedList<DiplomaCreateRequestViewModel>> List(PagedListInput input)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaCreateRequestRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            IQueryable<DiplomaCreateRequest> listQuery = _context.DiplomaCreateRequests
                .AsNoTracking()
                .Where(x => !x.Deleted);

            if (_userInfo.InstitutionID.HasValue)
            {
                listQuery = listQuery.Where(x => x.RequestingInstitutionId == _userInfo.InstitutionID.Value);
            }

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                listQuery = listQuery.Where(x => _userInfo.RegionID != null
                    && (x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID 
                    || x.InstitutionSchoolYearNavigation.Town.Municipality.RegionId == _userInfo.RegionID));
            }

            IQueryable<DiplomaCreateRequestViewModel> query = listQuery
                .Select(x => new DiplomaCreateRequestViewModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    PersonName = x.Person.FirstName + " " + x.Person.MiddleName + " " + x.Person.LastName,
                    PinType = x.Person.PersonalIdtypeNavigation.Name,
                    Pin = x.Person.PersonalId,
                    RequestingInstitutionId = x.RequestingInstitutionId,
                    RequestingInstitutionName = x.InstitutionSchoolYearNavigation.Abbreviation,
                    CurrentInstitutionId = x.CurrentInstitutionId,
                    CurrentInstitutionName = x.CurrentInstitutionId != null ? x.InstitutionSchoolYear.Abbreviation : x.CurrentInstitutionName,
                    BasicDocumentId = x.BasicDocumentId,
                    BasicDocumentName = x.BasicDocument.Name,
                    RegistrationNumber = x.RegistrationNumber,
                    RegistrationNumberYear = x.RegistrationNumberYear,
                    RegistrationDate = x.RegistrationDate,
                    Note = x.Note,
                    IsGranted = x.IsGranted,
                    DiplomaId = x.DiplomaId,
                    CreatorUsername = x.CreatedBySysUser.Username,
                    CreateDate = x.CreateDate,
                    ModifierUsername = x.ModifiedBySysUser.Username,
                    ModifyDate = x.ModifyDate,
                    DiplomaIsSigned = x.Diploma.IsSigned,
                    DiplomaIsCancelled = x.Diploma.IsCancelled,
                    DiplomaIsPublic = x.Diploma.IsPublic,
                    DiplomaIsEditable = x.Diploma.IsEditable
                })
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Note.Contains(input.Filter)
                   || predicate.PersonName.Contains(input.Filter)
                   || predicate.Pin.Contains(input.Filter)
                   || predicate.RequestingInstitutionId.ToString().Contains(input.Filter)
                   || predicate.RequestingInstitutionName.Contains(input.Filter)
                   || predicate.CurrentInstitutionName.Contains(input.Filter)
                   || predicate.BasicDocumentName.Contains(input.Filter)
                   || predicate.RegistrationNumber.Contains(input.Filter)
                   || predicate.RegistrationNumberYear.Contains(input.Filter))
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Id desc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<DiplomaCreateRequestViewModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            foreach (DiplomaCreateRequestViewModel item in items)
            {
                if (item.DiplomaIsCancelled)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.DiplomaIsCancelled),
                        LocalizationKey = "diplomas.isAnulledStatus",
                        Color = "error"
                    });
                }

                if (item.DiplomaIsPublic)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.DiplomaIsPublic),
                        LocalizationKey = "diplomas.isPublicStatus",
                        Color = "success"
                    });
                }

                if (item.DiplomaIsSigned)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.DiplomaIsSigned),
                        LocalizationKey = "diplomas.isSignedStatus",
                        Color = "primary"
                    });
                }

                if (item.DiplomaIsEditable)
                {
                    item.Tags.Add(new TagModel
                    {
                        Id = nameof(item.DiplomaIsEditable),
                        LocalizationKey = "diplomas.isEditableStatus",
                        Color = "info"
                    });
                }
            }

            return items.ToPagedList(totalCount);
        }

        public async Task<DiplomaCreateRequestModel> GetById(int id)
        {
            // Методът се използва при Details и Edit
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaCreateRequestRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaCreateRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DiplomaCreateRequestModel model = await _context.DiplomaCreateRequests
                .AsNoTracking()
                .Where(x => x.Id == id && x.Deleted == false)
                .Select(x => new DiplomaCreateRequestModel
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    RequestingInstitutionId = x.RequestingInstitutionId,
                    CurrentInstitutionId = x.CurrentInstitutionId,
                    CurrentInstitutionName = x.CurrentInstitutionName,
                    BasicDocumentId = x.BasicDocumentId,
                    RegistrationNumber = x.RegistrationNumber,
                    RegistrationNumberYear = x.RegistrationNumberYear,
                    RegistrationDate = x.RegistrationDate,
                    Note = x.Note,
                    IsGranted = x.IsGranted,
                    DiplomaId = x.DiplomaId,
                    ArbitraryCurrentInstitutionName = x.CurrentInstitutionId == null

                })
                .SingleOrDefaultAsync();

            if (model == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            return model;
        } 

        public async Task Create(DiplomaCreateRequestModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaCreateRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!_userInfo.InstitutionID.HasValue || model.RequestingInstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (await _context.DiplomaCreateRequests.AnyAsync(x => x.PersonId == model.PersonId
                && x.Deleted == false
                && x.DiplomaId == null
                && x.RequestingInstitutionId == model.RequestingInstitutionId
                && x.BasicDocumentId == model.BasicDocumentId))
            {
                throw new InvalidOperationException("Вече съществува запис за издаване на документ от този тип.");
            }

            DiplomaCreateRequest entry = new DiplomaCreateRequest
            {
                PersonId = model.PersonId,
                RequestingInstitutionId = _userInfo.InstitutionID ?? throw new ArgumentNullException(nameof(_userInfo.InstitutionID)),
                CurrentInstitutionId = model.ArbitraryCurrentInstitutionName
                    ? null
                    : model.CurrentInstitutionId,
                CurrentInstitutionName = !model.CurrentInstitutionName.IsNullOrWhiteSpace()
                    ? model.CurrentInstitutionName
                    : (model.CurrentInstitutionId.HasValue
                        ? (await _institutionService.GetInstitutionCache(model.CurrentInstitutionId ?? 0))?.Name
                        : ""),
                BasicDocumentId = model.BasicDocumentId,
                RegistrationNumber = model.RegistrationNumber,
                RegistrationNumberYear = model.RegistrationNumberYear,
                RegistrationDate = model.RegistrationDate,
                Note = model.Note,
                IsGranted = true,
                SchoolYear = await _institutionService.GetCurrentYear(_userInfo.InstitutionID)
            };

            _context.DiplomaCreateRequests.Add(entry);

            await SaveAsync();
        }

        public async Task Update(DiplomaCreateRequestModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaCreateRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DiplomaCreateRequest entity = await _context.DiplomaCreateRequests
                .Where(x => x.Deleted == false)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!_userInfo.InstitutionID.HasValue || entity.RequestingInstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.DiplomaId.HasValue)
            {
                throw new InvalidOperationException("Документът е издаден и записът не може да бъде променен!");
            }

            if (await _context.DiplomaCreateRequests.AnyAsync(x => x.Id != entity.Id
                && x.PersonId == model.PersonId
                && x.Deleted == false
                && x.DiplomaId == null
                && x.RequestingInstitutionId == model.RequestingInstitutionId
                && x.BasicDocumentId == model.BasicDocumentId))
            {
                throw new InvalidOperationException("Вече съществува запис за издаване на документ от този тип.");
            }

            entity.CurrentInstitutionId = model.ArbitraryCurrentInstitutionName
                    ? null
                    : model.CurrentInstitutionId;
            entity.CurrentInstitutionName = model.CurrentInstitutionName;
            entity.BasicDocumentId = model.BasicDocumentId;
            entity.RegistrationNumber = model.RegistrationNumber;
            entity.RegistrationNumberYear = model.RegistrationNumberYear;
            entity.RegistrationDate = model.RegistrationDate;
            entity.Note = model.Note;

            await SaveAsync();
        }

        public async Task Delete(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaCreateRequestManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DiplomaCreateRequest entity = await _context.DiplomaCreateRequests
                .SingleOrDefaultAsync(x => x.Id == id && x.Deleted == false);

            if (entity == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            if (!_userInfo.InstitutionID.HasValue || entity.RequestingInstitutionId != _userInfo.InstitutionID.Value)
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (entity.DiplomaId.HasValue)
            {
                throw new InvalidOperationException("Документът е издаден и записът не може да бъде изтрит!");
            }

            entity.Deleted = true;

            await SaveAsync();
        }
    }
}
