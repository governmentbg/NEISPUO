using MON.Models.DocManagement;
using MON.Models;
using MON.Services.Security.Permissions;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using MON.Shared;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models.Grid;
using MON.Services.Interfaces;
using System;
using Microsoft.Extensions.Logging;

namespace MON.Services.Implementations
{
    public class DocManagementAdditionalCampaignService : BaseService<DocManagementAdditionalCampaignService>
    {
        private readonly ISignalRNotificationService _signalRNotificationService;
        private readonly IInstitutionService _institutionService;
        public DocManagementAdditionalCampaignService(DbServiceDependencies<DocManagementAdditionalCampaignService> dependencies,
            IInstitutionService institutionService,
            ISignalRNotificationService signalRNotificationService) : base(dependencies)
        {
            _signalRNotificationService = signalRNotificationService;
            _institutionService = institutionService;
        }

        #region Private members
        private async Task SendNotification(DocManagementCampaign entity)
        {
            // Todo: Да се развие логиката за изпрашане на нотификации
            try
            {
                await _signalRNotificationService.DocManagementAdditionalCampaignModified(entity.Id, entity.ParentId);
            }
            catch (Exception ex)
            {
                // Ignore
                _logger.LogError($"DocManagementCampaign notification failed:{entity.Id}", ex);
            }
        }
        #endregion

        public async Task<IPagedList<DocManagementCampaignViewModel>> List(DocManagementApplicationsListInput input, CancellationToken cancellationToken)
        {
            if (input == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if (!await _authorizationService.AuthorizeUser(new string[] { DefaultPermissions.PermissionNameForDocManagementCampaignRead, DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage }, true))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            IQueryable<DocManagementCampaign> query = _context.DocManagementCampaigns
                .AsNoTracking()
                .Where(x => x.ParentId.HasValue && x.InstitutionId.HasValue && !x.IsHidden);

            if (input.CampaignId.HasValue)
            {
                query = query.Where(x => x.ParentId == input.CampaignId.Value);
            }

            if (_userInfo.IsConsortium || _userInfo.IsMon)
            {
                // Всичко
            } else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID);
            } else if(_userInfo.IsSchoolDirector)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID);
            } else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int? institutionId = _userInfo.InstitutionID;

            IQueryable<DocManagementCampaignViewModel> listQuery = query
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                    predicate => predicate.Name.Contains(input.Filter)
                    || predicate.Description.Contains(input.Filter)
                    || predicate.SchoolYearNavigation.Name.Contains(input.Filter)
                    || predicate.InstitutionSchoolYear.Name.Contains(input.Filter)
                    || predicate.InstitutionSchoolYear.Abbreviation.Contains(input.Filter))
                    .Select(x => new DocManagementCampaignViewModel
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        InstitutionId = x.InstitutionId,
                        Name = x.Name,
                        Description = x.Description,
                        SchoolYear = x.SchoolYear,
                        FromDate = x.FromDate,
                        ToDate = x.ToDate,
                        IsManuallyActivated = x.IsManuallyActivated,
                        SchoolYearName = x.SchoolYearNavigation.Name,
                        InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                        CreateDate = x.CreateDate,
                        Creator = x.CreatedBySysUser.Username,
                        ModifyDate = x.ModifyDate,
                        Updater = x.ModifiedBySysUser.Username,
                        HasApplication = institutionId.HasValue && x.DocManagementApplications.Any(a => a.InstitutionId == institutionId && a.SchoolYear == x.SchoolYear)
                    })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "SchoolYear desc, Id desc" : input.SortBy);

            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<DocManagementCampaignViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task<DocManagementCampaignViewModel> GetById(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(new string[] { DefaultPermissions.PermissionNameForDocManagementCampaignRead,
                    DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage }, true))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int? institutionId = _userInfo.InstitutionID;

            var model = await _context.DocManagementCampaigns
                .Where(x => x.Id == id && !x.IsHidden)
                .Select(x => new DocManagementCampaignViewModel
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    InstitutionId = x.InstitutionId,
                    Name = x.Name,
                    Description = x.Description,
                    SchoolYear = x.SchoolYear,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsManuallyActivated = x.IsManuallyActivated,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                    CreateDate = x.CreateDate,
                    Creator = x.CreatedBySysUser.Username,
                    ModifyDate = x.ModifyDate,
                    Updater = x.ModifiedBySysUser.Username,
                    HasApplication = institutionId.HasValue && x.DocManagementApplications.Any(a => a.InstitutionId == institutionId && a.SchoolYear == x.SchoolYear)
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new ApiException("Кампанията не е намерена", new ArgumentNullException(nameof(id)));

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                var institution = await _institutionService.GetInstitutionCache(model.InstitutionId ?? 0)
                    ?? throw new ApiException("Институцията не е намерена", new ArgumentNullException(nameof(model.InstitutionId)));

                if (institution.RegionId != _userInfo.RegionID)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            return model;
        }

        public async Task Create(DocManagementCampaignModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            if(!model.ParentId.HasValue)
            {
                throw new ApiException("Не е избрана основна кампания", new ArgumentNullException(nameof(model.ParentId)));
            }

            if (!model.InstitutionId.HasValue)
            {
                throw new ApiException("Не е избрана институция", new ArgumentNullException(nameof(model.InstitutionId)));
            }

            if (model.ToDate < model.FromDate)
            {
                throw new ApiException("\"От дата\" следва да е преди \"До дата\".");
            }

            var parentCampaign = await _context.DocManagementCampaigns
                .AsNoTracking()
                .Where(x => x.Id == model.ParentId.Value && x.InstitutionId == null)
                .Select(x => new
                {
                    x.Id,
                    x.SchoolYear
                })
                .FirstOrDefaultAsync()
                ?? throw new ArgumentNullException(nameof(DocManagementCampaign));

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                var institution =  await _institutionService.GetInstitutionCache(model.InstitutionId ?? 0)
                    ?? throw new ApiException("Институцията не е намерена", new ArgumentNullException(nameof(model.InstitutionId)));

                if (institution.RegionId != _userInfo.RegionID)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            if (await _context.DocManagementCampaigns.AnyAsync(x => x.ParentId == parentCampaign.Id 
                && x.InstitutionId == model.InstitutionId.Value
                && x.FromDate <= model.ToDate && model.FromDate <= x.ToDate ))
            {
                throw new ApiException(Messages.DocManagementCampaignAlreadyExits);
            }

            DocManagementCampaign entry = new DocManagementCampaign
            {
                Name = model.Name,
                Description = model.Description,
                SchoolYear = parentCampaign.SchoolYear,
                ParentId = model.ParentId,
                InstitutionId = model.InstitutionId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsManuallyActivated = model.IsManuallyActivated
            };

            _context.DocManagementCampaigns.Add(entry);
            await SaveAsync();
            await SendNotification(entry);
        }

        public async Task Update(DocManagementCampaignModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }


            if (!model.InstitutionId.HasValue)
            {
                throw new ApiException("Не е избрана институция", new ArgumentNullException(nameof(model.InstitutionId)));
            }

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                var institution = await _institutionService.GetInstitutionCache(model.InstitutionId ?? 0)
                    ?? throw new ApiException("Институцията не е намерена", new ArgumentNullException(nameof(model.InstitutionId)));

                if (institution.RegionId != _userInfo.RegionID)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            DocManagementCampaign entity = await _context.DocManagementCampaigns
              .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsHidden) ?? throw new ApiException(Messages.EmptyEntityError);

            if (model.ToDate < model.FromDate)
            {
                throw new ApiException("\"От дата\" следва да е преди \"До дата\".");
            }

            if (await _context.DocManagementCampaigns.AnyAsync(x => x.Id != model.Id && x.ParentId == entity.ParentId
                && x.InstitutionId == model.InstitutionId.Value
                && x.FromDate <= model.ToDate && model.FromDate <= x.ToDate))
            {
                throw new ApiException(Messages.DocManagementCampaignAlreadyExits);
            }

            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.FromDate = model.FromDate;
            entity.ToDate = model.ToDate;
            entity.InstitutionId = model.InstitutionId;
            entity.IsManuallyActivated = model.IsManuallyActivated;

            await SaveAsync();
            await SendNotification(entity);
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            DocManagementCampaign entity = await _context.DocManagementCampaigns
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsHidden, cancellationToken)
                ?? throw new ApiException(Messages.EmptyModelError);

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                var institution = await _institutionService.GetInstitutionCache(entity.InstitutionId ?? 0)
                    ?? throw new ApiException("Институцията не е намерена", new ArgumentNullException(nameof(entity.InstitutionId)));

                if (institution.RegionId != _userInfo.RegionID)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            if (entity.FromDate <= Now || entity.IsManuallyActivated)
            {
                throw new ApiException("Кампанията е ръчно активирана или началната дата е настъпила");
            }

            _context.DocManagementCampaigns.Remove(entity);
            await SaveAsync();
        }

        public async Task ToggleManuallyActivation(DocManagementCampaignModel model)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDocManagementAdditionalCampaignManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (model == null)
            {
                throw new ApiException(Messages.EmptyModelError);
            }

            DocManagementCampaign entity = await _context.DocManagementCampaigns
                .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsHidden) ?? throw new ApiException(Messages.EmptyEntityError);

            if(_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                var institution = await _institutionService.GetInstitutionCache(entity.InstitutionId ?? 0)
                    ?? throw new ApiException("Институцията не е намерена", new ArgumentNullException(nameof(entity.InstitutionId)));

                if (institution.RegionId != _userInfo.RegionID)
                {
                    throw new ApiException(Messages.UnauthorizedMessageError, 401);
                }
            }

            entity.IsManuallyActivated = !entity.IsManuallyActivated;

            await SaveAsync();
            await SendNotification(entity);
        }

        public async Task<List<DropdownViewModel>> GetDropdownOptions(CancellationToken cancellationToken)
        {
            return await _context.DocManagementCampaigns
                .Where(x => x.ParentId.HasValue && !x.IsHidden)
                .OrderByDescending(x => x.SchoolYear)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Text = $"{x.SchoolYearNavigation.Name} - {x.Name}"
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DocManagementCampaignViewModel>> GetActive(CancellationToken cancellationToken)
        {
            DateTime now = Now;

            IQueryable<DocManagementCampaign> query = _context.DocManagementCampaigns
                .AsNoTracking()
                .Where(x => x.ParentId.HasValue && !x.IsHidden);

            if (_userInfo.IsConsortium || _userInfo.IsConsortium)
            {
                // Всичко
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => x.InstitutionSchoolYear.Town.Municipality.RegionId == _userInfo.RegionID);
            }
            else if (_userInfo.IsSchoolDirector)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID);
            }
            else
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await query
                .Where(x => x.IsManuallyActivated || (x.FromDate <= now && now < x.ToDate.AddDays(1)))
                .OrderByDescending(x => x.SchoolYear)
                .ThenByDescending(x => x.Id)
                .Select(x => new DocManagementCampaignViewModel
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    InstitutionId = x.InstitutionId,
                    Name = x.Name,
                    Description = x.Description,
                    SchoolYear = x.SchoolYear,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    IsManuallyActivated = x.IsManuallyActivated,
                    SchoolYearName = x.SchoolYearNavigation.Name,
                    InstitutionName = x.InstitutionSchoolYear.Abbreviation,
                    CreateDate = x.CreateDate,
                    Creator = x.CreatedBySysUser.Username,
                    ModifyDate = x.ModifyDate,
                    Updater = x.ModifiedBySysUser.Username,
                })
                .ToListAsync(cancellationToken);
        }
    }
}
