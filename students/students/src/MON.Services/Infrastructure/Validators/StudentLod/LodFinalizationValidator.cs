using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Models.StudentModels.Lod;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Infrastructure.Validators.StudentLod
{
    public class LodFinalizationValidator
    {
        private readonly MONContext _context;
        private readonly ApiValidationResult _validationResult;
        private readonly IUserInfo _userInfo;

        public LodFinalizationValidator(MONContext context,
            IUserInfo userInfo)
        {
            _context = context;
            _validationResult = new ApiValidationResult();
            _userInfo = userInfo;
        }

        public async Task<ApiValidationResult> ValidateApproval(LodFinalizationModel model)
        {
            _validationResult.IsValid = true;

            if (model == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Model {nameof(LodFinalizationModel)} can't be null!");
                return _validationResult;
            }

            bool anyApproved = await _context.Lodfinalizations
                .AnyAsync(x => model.PersonIds.Contains(x.PersonId) && x.SchoolYear == model.SchoolYear && x.IsApproved);

            if (anyApproved)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява одобрение на ЛОД, ако той вече е одобрен!");
                return _validationResult;

            }

            bool anyFinalized = await _context.Lodfinalizations
                .AnyAsync(x => model.PersonIds.Contains(x.PersonId) && x.SchoolYear == model.SchoolYear && x.IsFinalized);

            if (anyFinalized)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява одобрение на ЛОД, ако той вече е подписан от директор!");
                return _validationResult;
            }

            return _validationResult;
        }

        public async Task<ApiValidationResult> ValidateFinalization(LodFinalizationModel model)
        {
            _validationResult.IsValid = true;

            if (!_userInfo.IsSchoolDirector)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add(Messages.UnauthorizedMessageError);
                return _validationResult;
            }

            if (model == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Model {nameof(LodFinalizationModel)} can't be null!");
                return _validationResult;
            }

            bool anyFinalizationSignatory = await _context.LodFinalizationSignatories
                .AsNoTracking()
                .AnyAsync(x => model.PersonIds.Contains(x.Lodfinalization.PersonId) && x.Lodfinalization.SchoolYear == model.SchoolYear && x.Lodfinalization.IsFinalized);

            if (anyFinalizationSignatory)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява подписване на ЛОД, ако той вече е подписан от директор!");
                return _validationResult;
            }

            return _validationResult;
        }

        public async Task<ApiValidationResult> ValidateApprovalUndo(LodFinalizationModel model)
        {
            _validationResult.IsValid = true;

            if (model == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Model {nameof(LodFinalizationModel)} can't be null!");
                return _validationResult;
            }

            List<int> personIdsInDb = _context.Lodfinalizations
                .Where(x => x.SchoolYear == model.SchoolYear && x.IsApproved)
                .Select(x => x.PersonId).ToList();

            bool anyMissingPersons = personIdsInDb.Count() > 0 ? model.PersonIds.Any(pId => !personIdsInDb.Contains(pId)) : true;
            bool anyNotApprovedLod = await _context.Lodfinalizations
                .AnyAsync(x => (model.PersonIds.Contains(x.PersonId) && x.SchoolYear == model.SchoolYear && !x.IsApproved));

            if (anyNotApprovedLod || anyMissingPersons)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява премахване на одобрение на ЛОД, ако някой от списъка не е одобрен!");
                return _validationResult;
            }

            bool anyLodFinalizationSignatory = await _context.LodFinalizationSignatories
                .AsNoTracking()
                .AnyAsync(x => model.PersonIds.Contains(x.Lodfinalization.PersonId) && x.Lodfinalization.SchoolYear == model.SchoolYear && x.Lodfinalization.IsFinalized);

            if (anyLodFinalizationSignatory)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява премахване на одобрение на ЛОД, ако той вече е подписан от директор!");
                return _validationResult;
            }

            return _validationResult;
        }

        public async Task<ApiValidationResult> ValidateFinalizationUndo(LodFinalizationModel model)
        {
            _validationResult.IsValid = true;

            if (!_userInfo.IsSchoolDirector)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add(Messages.UnauthorizedMessageError);
                return _validationResult;
            }

            if (model == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Model {nameof(LodFinalizationModel)} can't be null!");
                return _validationResult;
            }

            List<int> personIdsInDb = _context.Lodfinalizations
               .AsNoTracking()
               .Where(x => x.SchoolYear == model.SchoolYear && x.IsFinalized)
               .Select(x => x.PersonId).ToList();

            bool anyMissingPersons = personIdsInDb.Count() > 0 ? model.PersonIds.Any(pId => !personIdsInDb.Contains(pId)) : true;
            bool anyNotFinalized = await _context.Lodfinalizations
                .AnyAsync(x => (model.PersonIds.Contains(x.PersonId) && x.SchoolYear == model.SchoolYear && !x.IsFinalized));

            if (anyNotFinalized || anyMissingPersons)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява премахване на подписан ЛОД, ако някой от списъка не е подписан!");
                return _validationResult;
            }

            return _validationResult;
        }
    }
}
