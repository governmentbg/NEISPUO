namespace Helpdesk.Services.Implementations
{
    using Helpdesk.DataAccess;
    using Helpdesk.Models;
    using Helpdesk.Services.Interfaces;
    using Helpdesk.Shared;
    using Helpdesk.Shared.Enums;
    using Helpdesk.Shared.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class LookupService : BaseService, ILookupService
    {
        public LookupService(HelpdeskContext context,
            IUserInfo userInfo,
            ILogger<LookupService> logger)
            : base(context, userInfo, logger)
        {
            var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(1) };
            QueryCacheManager.DefaultMemoryCacheEntryOptions = options;
        }

        public async Task<IEnumerable<DropdownViewModel>> GetCategories()
        {
            var dbCategories = await _context.Categories.AsNoTracking()
                .Where(x => x.IsValid && x.ParentId == null)
                .OrderBy(x => x.Code)
                .Select(
                    x => new
                    {
                        x.ForRoles,
                        Category = new DropdownViewModel
                        {
                            Value = x.Id,
                            Text = x.Name
                        }
                    }
                )
                .ToListAsync();

            var categories = dbCategories.Select(category => new
            {
                RoleIds = (category.ForRoles ?? "")
                    .Split("|", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList(),
                category.Category
            });
            return categories.Where(i => _userInfo.IsInRole(UserRoleEnum.Consortium) || i.RoleIds == null || i.RoleIds.Count() == 0 || i.RoleIds.Contains(_userInfo.SysRoleID)).Select(i => i.Category);
        }

        public async Task<IEnumerable<DropdownViewModel>> GetSubcategories(int? parentId)
        {
            var dbSubCategories = await _context.Categories
                .Where(x => x.IsValid && x.ParentId.HasValue && x.ParentId == parentId)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.ForRoles,
                    SubCategory = new DropdownViewModel
                    {

                        Value = x.Id,
                        Text = x.Name,
                        RelatedObjectId = x.ParentId
                    }
                })
                .ToListAsync();

            var subCategories = dbSubCategories.Select(subCategory => new
            {
                RoleIds = (subCategory.ForRoles ?? "")
                    .Split("|", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList(),
                subCategory.SubCategory
            });
            return subCategories.Where(i => _userInfo.IsInRole(UserRoleEnum.Consortium) || i.RoleIds == null || i.RoleIds.Count() == 0 || i.RoleIds.Contains(_userInfo.SysRoleID)).Select(i => i.SubCategory);
        }

        public async Task<CategoryModel> GetCategory(int id)
        {
            var category = await (
                from c in _context.Categories.AsNoTracking()
                where c.Id == id
                select new CategoryModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    SurveySchema = c.SurveySchema
                }).FirstOrDefaultAsync();
            return category;
        }

        public Task<IEnumerable<DropdownViewModel>> GetPriorities()
        {
            return _context.Priorities
                .Where(x => x.IsValid)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Text = x.Name
                })
                .FromCacheAsync();
        }

        public Task<IEnumerable<DropdownViewModel>> GetMONUsers()
        {
            return _context.SysUserSysRoles
                .Where(x => x.SysRoleId == (int)UserRoleEnum.Mon || x.SysRoleId == (int)UserRoleEnum.MonExpert || x.SysRoleId == (int)UserRoleEnum.Consortium)
                .Where(x => x.SysUser.DeletedOn == null)
                .Select(x => new DropdownViewModel
                {
                    Value = x.SysUserId,
                    Text = $"{x.SysUser.Person.FirstName} {x.SysUser.Person.LastName} / {x.SysUser.Username} / {x.SysRole.Name}"
                })
                .FromCacheAsync();
        }

        public Task<IEnumerable<DropdownViewModel>> GetRUOUsers(int regionId)
        {
            return _context.SysUserSysRoles
                .Where(x => (x.SysRoleId == (int)UserRoleEnum.Ruo || x.SysRoleId == (int)UserRoleEnum.RuoExpert) && (x.RegionId == regionId))
                .Where(x => x.SysUser.DeletedOn == null)
                .Select(x => new DropdownViewModel
                {
                    Value = x.SysUserId,
                    Text = $"{x.SysUser.Person.FirstName} {x.SysUser.Person.LastName} / {x.SysUser.Username} / {x.SysRole.Name}"
                })
                .FromCacheAsync();
        }

        public Task<IEnumerable<DropdownViewModel>> GetStatuses()
        {
            return _context.Statuses
                .Where(x => x.IsValid)
                .Select(x => new DropdownViewModel
                {
                    Value = x.Id,
                    Text = x.Name
                })
                .FromCacheAsync();
        }

        public async Task<IEnumerable<DropdownViewModel>> GetUsers()
        {
            IQueryable<SysUserSysRole> query = _context.SysUserSysRoles
                .Where(x => x.SysUser.DeletedOn == null)
                .AsNoTracking();

            if (_userInfo.IsMon || _userInfo.IsMonExpert || _userInfo.IsConsortium || _userInfo.IsCIOO)
            {
                query = query.Where(x => x.SysRoleId == (int)UserRoleEnum.Mon || x.SysRoleId == (int)UserRoleEnum.MonExpert
                    || x.SysRoleId == (int)UserRoleEnum.Ruo || x.SysRoleId == (int)UserRoleEnum.RuoExpert);
            }
            else if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = query.Where(x => x.SysUserId == _userInfo.SysUserID
                    || (x.RegionId.HasValue && x.RegionId.Value == _userInfo.RegionID && (x.SysRoleId == (int)UserRoleEnum.Ruo || x.SysRoleId == (int)UserRoleEnum.RuoExpert)));
            }
            else
            {
                return null;
            }

            return await query.Select(x => new DropdownViewModel
            {
                Value = x.SysUserId,
                Text = $"{x.SysUser.Person.FirstName} {x.SysUser.Person.LastName} / {x.SysUser.Username} / {x.SysRole.Name}"
            }).ToListAsync();
        }
    }
}
