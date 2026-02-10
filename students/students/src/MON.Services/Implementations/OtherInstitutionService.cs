using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class OtherInstitutionService : BaseService<OtherInstitutionService>, IOtherInstitutionService
    {
        private readonly IInstitutionService _institutionService;

        public OtherInstitutionService(DbServiceDependencies<OtherInstitutionService> dependencies,
            IInstitutionService institutionService)
            : base(dependencies)
        {
            _institutionService = institutionService;
        }

        public async Task<OtherInstitutionViewModel> GetByIdAsync(int institutionId)
        {
            var institution = await _context.OtherInstitutions
                .AsNoTracking()
                .Where(x => x.Id == institutionId).Select(x => new OtherInstitutionViewModel
                {
                    Id = x.Id,
                    Reason = x.Reason,
                    ValidFrom = x.ValidFrom,
                    ValidTo = x.ValidTo,
                    PersonId = x.PersonId,
                    Institution = new DropdownViewModel()
                    {
                        Value = x.InstitutionId,
                        Text = x.InstitutionSchoolYear.Name,
                        Name = x.InstitutionSchoolYear.Name
                    }
                }).SingleOrDefaultAsync();

            // Методът се използва при Details и Edit
            if (institution != null
                && !await _authorizationService.HasPermissionForStudent(institution.PersonId, DefaultPermissions.PermissionNameForStudentOtherInstitutionRead)
                && !await _authorizationService.HasPermissionForStudent(institution.PersonId, DefaultPermissions.PermissionNameForStudentOtherInstitutionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return institution;
        }

        public async Task<List<OtherInstitutionViewModel>> GetStudentOtherInstitutions(int personId)
        {
            if (!await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentOtherInstitutionRead)
               && !await _authorizationService.HasPermissionForStudent(personId, DefaultPermissions.PermissionNameForStudentOtherInstitutionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            return await _context.OtherInstitutions
                .AsNoTracking()
                .Where(x => x.PersonId == personId)
                .Select(x => new OtherInstitutionViewModel
                {
                    Id = x.Id,
                    Reason = x.Reason,
                    ValidFrom = x.ValidFrom,
                    ValidTo = x.ValidTo,
                    PersonId = x.PersonId,
                    Institution = new DropdownViewModel()
                    {
                        Value = x.InstitutionId,
                        Text = x.InstitutionSchoolYear.Name,
                        Name = x.InstitutionSchoolYear.Name
                    }
                }).ToListAsync();
        }

        public async Task DeleteAsync(int otherInstitutionId)
        {
            var entity = await _context.OtherInstitutions.SingleOrDefaultAsync(x => x.Id == otherInstitutionId);

            if (!await _authorizationService.HasPermissionForStudent(entity?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentOtherInstitutionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            if (entity != null)
            {
                _context.OtherInstitutions.Remove(entity);
                await SaveAsync();
            }
        }

        public async Task CreateAsync(OtherInstitutionViewModel model)
        {
            if (!await _authorizationService.HasPermissionForStudent(model?.PersonId ?? default, DefaultPermissions.PermissionNameForStudentOtherInstitutionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var toAdd = new OtherInstitution
            {
                PersonId = model.PersonId,
                InstitutionId = model.Institution.Value,
                Reason = model.Reason,
                ValidFrom = model.ValidFrom,
                ValidTo = model.ValidTo,
                SchoolYear = await _institutionService.GetCurrentYear(model.Institution?.Value ?? _userInfo?.InstitutionID)
            };

            _context.OtherInstitutions.Add(toAdd);

            await SaveAsync();
        }

        public async Task UpdateAsync(OtherInstitutionViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), Messages.EmptyModelError);
            }

            if (!await _authorizationService.HasPermissionForStudent(model.PersonId, DefaultPermissions.PermissionNameForStudentOtherInstitutionManage))
            {
                throw new UnauthorizedAccessException(Messages.UnauthorizedMessageError);
            }

            var institution = await _context.OtherInstitutions.Where(i => i.Id == model.Id).SingleOrDefaultAsync();

            institution.InstitutionId = model.Institution.Value;
            institution.Reason = model.Reason;
            institution.ValidTo = model.ValidTo;
            institution.ValidFrom = model.ValidFrom;
            institution.SchoolYear = await _institutionService.GetCurrentYear(model.Institution?.Value ?? _userInfo?.InstitutionID);

            await SaveAsync();
        }
    }
}