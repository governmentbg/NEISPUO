using MON.DataAccess;
using MON.Models;
using MON.Models.StudentModels;
using MON.Services.Interfaces;
using MON.Shared.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MON.Services.Infrastructure.Validators.StudentLod
{
    public class LodEvaluationsValidator
    {
        private readonly MONContext _context;
        private readonly ApiValidationResult _validationResult;
        private readonly ILodFinalizationService _lodFinalizationService;
        private readonly IUserInfo _userInfo;

        public LodEvaluationsValidator(MONContext context,
            IUserInfo userInfo,
            ILodFinalizationService lodFinalizationService)
        {
            _context = context;
            _validationResult = new ApiValidationResult();
            _userInfo = userInfo;
            _lodFinalizationService = lodFinalizationService;
        }

        public async Task<(ApiValidationResult, LodEvaluationGeneral)> ValidateFinalization(LodEvaluationsFinalizationModel model)
        {
            _validationResult.IsValid = true;

            if (model == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Model {nameof(LodEvaluationsFinalizationModel)} can't be null!");
                return (_validationResult, null);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, (short)model.SchoolYear))
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add(Messages.LodIsFinalizedError(model?.SchoolYear));
                return (_validationResult, null);
            }

            LodEvaluationGeneral lodEvaluationGeneral = await _context.LodEvaluationGenerals
                .Where(e => e.PersonId == model.PersonId && e.SchoolYear == model.SchoolYear && e.IsSelfEduForm == model.IsSelfEduForm && e.StudentClassId == model.StudentClassId)
                .FirstOrDefaultAsync();

            if (lodEvaluationGeneral == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява финализиране на оценките, ако липсва запис в LodEvaluationGeneral");
                return (_validationResult, null);
            }

            return (_validationResult, lodEvaluationGeneral);
        }

        public async Task<(ApiValidationResult, LodFirstGradeResult)> ValidateFinalization(LodFirstGradeEvaluationsFinalizationModel model)
        {
            _validationResult.IsValid = true;

            if (model == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Model {nameof(LodFirstGradeEvaluationsFinalizationModel)} can't be null!");
                return (_validationResult, null);
            }

            if (await _lodFinalizationService.IsLodFInalized(model.PersonId, (short)model.SchoolYear))
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add(Messages.LodIsFinalizedError(model?.SchoolYear));
                return (_validationResult, null);
            }

            LodFirstGradeResult lodFirstGradeResult = await _context.LodFirstGradeResults
                .Where(e => e.PersonId == model.PersonId && e.SchoolYear == model.SchoolYear && e.StudentClassId == model.StudentClassId)
                .FirstOrDefaultAsync();

            if (lodFirstGradeResult == null)
            {
                _validationResult.IsValid = false;
                _validationResult.Messages.Add($"Не се позволява финализиране на оценките, ако липсва запис в LodFirstGradeResult");
                return (_validationResult, null);
            }

            return (_validationResult, lodFirstGradeResult);
        }
    }
}
