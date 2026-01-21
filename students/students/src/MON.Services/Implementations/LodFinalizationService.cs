namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Configuration;
    using MON.Models.StudentModels;
    using MON.Models.StudentModels.Lod;
    using MON.Services.Infrastructure.Validators.StudentLod;
    using MON.Services.Interfaces;
    using MON.Services.Security.Permissions;
    using MON.Shared.Enums;
    using MON.Shared.Enums.AspIntegration;
    using MON.Shared.ErrorHandling;
    using MON.Shared.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class LodFinalizationService : BaseService<LodFinalizationService>, ILodFinalizationService
    {
        private const string LodFinalizationCacheTag = "LodFinalization";
        private const string LodApprovalCacheTag = "LodApproval";

        private readonly LodFinalizationValidator _lodFinalizationValidator;
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly IBlobService _blobService;

        public LodFinalizationService(DbServiceDependencies<LodFinalizationService> dependencies,
            ISignalRNotificationService signalRNotificationService,
            IBlobService blobService,
            LodFinalizationValidator lodFinalizationValidator)
            : base(dependencies)
        {
            _blobService = blobService;
            _lodFinalizationValidator = lodFinalizationValidator;
            _signalRNotificationService = signalRNotificationService;
        }

        public async Task ApproveLodAsync(LodFinalizationModel model)
        {
            foreach (var personId in model.PersonIds)
            {
                if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForLodStateManage))
                {
                    throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
                }
            }

            ApiValidationResult validationResult = await _lodFinalizationValidator.ValidateApproval(model);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }


            IEnumerable<Lodfinalization> lodfinalizations = _context.Lodfinalizations
                .Where(x => model.PersonIds.Contains(x.PersonId) && x.SchoolYear == model.SchoolYear);

            if (lodfinalizations.Count() == 0)
            {
                foreach (var personId in model.PersonIds)
                {
                    _context.LodFinalizationSignatories.Add(new LodFinalizationSignatory
                    {
                        Activity = LodFinalizationActivity.Approved.GetEnumDescription(),
                        SysRoleId = _userInfo.SysRoleID,
                        Reason = string.Empty,
                        InstitutionId = _userInfo.InstitutionID,
                        Lodfinalization = new Lodfinalization
                        {
                            PersonId = personId,
                            SchoolYear = model.SchoolYear,
                            ApprovalDate = DateTime.UtcNow,
                            IsApproved = true,
                            InstitutionId = _userInfo.InstitutionID,
                        }
                    });
                }
            }
            else
            {
                foreach (var lodfinalization in lodfinalizations)
                {
                    lodfinalization.IsApproved = true;
                    lodfinalization.ApprovalDate = DateTime.UtcNow;
                    lodfinalization.InstitutionId = _userInfo.InstitutionID;

                    _context.LodFinalizationSignatories.Add(new LodFinalizationSignatory
                    {
                        Activity = LodFinalizationActivity.Approved.GetEnumDescription(),
                        Reason = string.Empty,
                        Lodfinalization = lodfinalization,
                        SysRoleId = _userInfo.SysRoleID,
                        InstitutionId = _userInfo.InstitutionID,
                    });
                }
            }

            await SaveAsync();

            QueryCacheManager.ExpireTag(LodApprovalCacheTag);
        }

        public async Task ApproveLodUndoAsync(LodFinalizationUndoModel model)
        {
            foreach (var personId in model.PersonIds)
            {
                if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForLodStateManage))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            ApiValidationResult validationResult = await _lodFinalizationValidator.ValidateApprovalUndo(model);
            if (!validationResult.IsValid)
            {
                string errors = validationResult.ToString();
                _logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }

            IList<Lodfinalization> lodfinalizations = await _context.Lodfinalizations
                .Where(x => model.PersonIds.Contains(x.PersonId) && x.SchoolYear == model.SchoolYear)
                .ToListAsync();


            foreach (var lodfinalization in lodfinalizations)
            {
                if (lodfinalization.CreatedBySysUserId != _userInfo.SysUserID
                    && lodfinalization.ModifiedBySysUserId != _userInfo.SysUserID
                    && !await _context.SysUserSysRoles.AnyAsync(x => x.SysUserId == lodfinalization.CreatedBySysUserId && x.InstitutionId == _userInfo.InstitutionID))
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }

                lodfinalization.IsApproved = false;
                lodfinalization.ApprovalDate = null;
                lodfinalization.InstitutionId = _userInfo.InstitutionID;

                _context.LodFinalizationSignatories.Add(new LodFinalizationSignatory
                {
                    Activity = LodFinalizationActivity.UnApproved.GetEnumDescription(),
                    Reason = model.Reason,
                    Lodfinalization = lodfinalization,
                    SysRoleId = _userInfo.SysRoleID,
                    InstitutionId = _userInfo.InstitutionID,
                });
            }

            await SaveAsync();

            QueryCacheManager.ExpireTag(LodApprovalCacheTag);
        }

        public Task<bool> IsLodFInalized(int personId, short schoolYear, CancellationToken cancellationToken = default)
        {
            return _context.Lodfinalizations
                .AnyAsync(x => x.PersonId == personId && x.SchoolYear == schoolYear && x.IsFinalized, cancellationToken);
        }

        public Task<bool> IsLodApproved(int personId, short schoolYear, CancellationToken cancellationToken = default)
        {
            return _context.Lodfinalizations
                .AnyAsync(x => x.PersonId == personId && x.SchoolYear == schoolYear && x.IsApproved, cancellationToken);
        }

        public Task<List<short>> GetStudentFinalizedLods(int personId)
        {
            return _context.Lodfinalizations
                .Where(x => x.PersonId == personId && x.IsFinalized)
                .Select(x => x.SchoolYear)
                .OrderBy(x => x)
                .ToListAsync();
        }

        public async Task SignLodAsync(LodSignatureModel model)
        {
            if (await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForLodFinalizationAdministration))
            {
                // Администратор прави всичкo
            }
            else
            {
                if (model.ClassId.HasValue)
                {
                    var classGroup = await _context.ClassGroups
                       .AsNoTracking()
                       .Where(x => x.ClassId == (model.ClassId ?? int.MinValue))
                       .Select(x => new
                       {
                           x.ClassId,
                           x.InstitutionId,
                           x.SchoolYear,
                           x.InstitutionSchoolYear.IsCurrent,
                           HasAny = x.StudentClasses.Any(sc => sc.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear && (sc.IsCurrent || sc.Status == (int)MovementStatusEnum.Enrolled))
                       })
                       .FirstOrDefaultAsync()
                       ?? throw new ApiException(Messages.UnauthorizedMessageError, 401);

                    if (!classGroup.HasAny)
                    {
                        throw new ApiException(Messages.InvalidSchoolYear);
                    }

                    if (classGroup.IsCurrent)
                    {
                        if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForLodStateManage))
                        {
                            throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
                        }
                    }
                    else
                    {
                        if (!await _authorizationService.HasPermissionForClass(model.ClassId.Value, DefaultPermissions.PermissionNameForClassStudentsRead))
                        {
                            throw new ApiException(Messages.UnauthorizedMessageError, 401);
                        }
                    }
                }
                else
                {
                    if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForLodStateManage))
                    {
                        throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
                    }
                }
            }

            DateTime utcNow = DateTime.UtcNow;
            Lodfinalization lodfinalization = await _context.Lodfinalizations.SingleOrDefaultAsync(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear);
            if (lodfinalization == null)
            {
                lodfinalization = new Lodfinalization
                {
                    PersonId = model.PersonId,
                    SchoolYear = model.SchoolYear,
                    FinalizationDate = utcNow,
                    ApprovalDate = utcNow,
                    IsFinalized = true,
                    IsApproved = true
                };
                _context.Lodfinalizations.Add(lodfinalization);
            }

            if (!string.IsNullOrWhiteSpace(model.Signature))
            {
                string personName = await _context.VPersonDetails
                    .Where(x => x.PersonId == model.PersonId)
                    .Select(x => x.FullName)
                    .SingleOrDefaultAsync();
                string fileName = $"Подписан личен картон на {personName ?? lodfinalization.PersonId.ToString()}_{lodfinalization.SchoolYear}.docx";
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                var result = await _blobService.UploadFileAsync(Convert.FromBase64String(model.Signature), fileName, contentType);
                int? blobId = result?.Data?.BlobId;
                if (blobId != null)
                {
                    lodfinalization.Document = new Document
                    {
                        FileName = fileName,
                        ContentType = contentType,
                        Description = "Подписан ЛОД",
                        BlobId = blobId
                    };
                }
            }

            if (!lodfinalization.IsFinalized)
            {
                lodfinalization.IsFinalized = true;
                lodfinalization.FinalizationDate = utcNow;
            }

            if (!lodfinalization.IsApproved)
            {
                lodfinalization.IsApproved = true;
                lodfinalization.ApprovalDate = utcNow;
            }

            lodfinalization.InstitutionId = _userInfo.InstitutionID;

            lodfinalization.LodFinalizationSignatories.Add(new LodFinalizationSignatory
            {
                Activity = LodFinalizationActivity.Finalized.GetEnumDescription(),
                Reason = string.Empty,
                Lodfinalization = lodfinalization,
                SysRoleId = _userInfo.SysRoleID,
                InstitutionId = _userInfo.InstitutionID,
            });

            await SaveAsync();
        }

        public async Task SignLodUndoAsync(LodSignatureUndoModel model)
        {

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForLodStateManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model.ClassId.HasValue && !await _context.StudentClasses.AnyAsync(x => x.PersonId == model.PersonId && x.ClassId == model.ClassId.Value && x.SchoolYear == model.SchoolYear))
            {
                throw new ApiException(Messages.InvalidSchoolYear);
            }

            Lodfinalization lodfinalization = await _context.Lodfinalizations
                .Include(x => x.Document)
                .SingleOrDefaultAsync(x => x.PersonId == model.PersonId && x.SchoolYear == model.SchoolYear);
            if (lodfinalization == null) return;

            if (lodfinalization.CreatedBySysUserId != _userInfo.SysUserID
                    && lodfinalization.ModifiedBySysUserId != _userInfo.SysUserID
                    && !await _context.SysUserSysRoles.AnyAsync(x => x.SysUserId == lodfinalization.CreatedBySysUserId && x.InstitutionId == _userInfo.InstitutionID))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            lodfinalization.IsFinalized = false;
            lodfinalization.FinalizationDate = null;
            lodfinalization.IsApproved = false;
            lodfinalization.ApprovalDate = null;
            lodfinalization.InstitutionId = _userInfo.InstitutionID;

            if (lodfinalization.Document != null)
            {
                lodfinalization.DocumentId = null;
                _context.Documents.Remove(lodfinalization.Document);
            }

            _context.LodFinalizationSignatories.Add(new LodFinalizationSignatory
            {
                Activity = LodFinalizationActivity.UnFinalized.GetEnumDescription(),
                Reason = model.Reason,
                Lodfinalization = lodfinalization,
                SysRoleId = _userInfo.SysRoleID,
                InstitutionId = _userInfo.InstitutionID,
            });

            await SaveAsync();
        }
    }
}
