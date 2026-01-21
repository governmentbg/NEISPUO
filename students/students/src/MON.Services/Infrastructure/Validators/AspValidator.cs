using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Models.ASP;
using MON.Models.Enums;
using MON.Shared.Interfaces;
using System.Threading.Tasks;

namespace MON.Services.Infrastructure.Validators
{
    public class AspValidator
    {
        private readonly MONContext _context;
        private readonly ApiValidationResult _validationResult;
        private readonly IUserInfo _userInfo;

        public AspValidator(MONContext context,
            IUserInfo userInfo)
        {
            _context = context;
            _userInfo = userInfo;
            _validationResult = new ApiValidationResult();
        }

        public ApiValidationResult ValidateEnrolledStudentsExportBasicRules(EnrolledStudentsExportModel model)
        {
            ApiValidationResult checkResult = new ApiValidationResult();

            if (model == null)
            {
                checkResult.Errors.Add(Messages.EmptyModelError, nameof(model), nameof(EnrolledStudentsExportModel));
                return checkResult;
            }

            if (!_userInfo.IsMon)
            {
                checkResult.Errors.Add($"Invalid access error. Only mon or cioo can export this information!");
                return checkResult;
            }

            return checkResult;
        }

        public async Task<ApiValidationResult> ValidateEnrolledStudentsExportCorrections(EnrolledStudentsExportModel model)
        {
            _validationResult.Merge(ValidateEnrolledStudentsExportBasicRules(model));

            if (_validationResult.HasErrors)
            {
                return _validationResult;
            }

            if (model.FileType == (int)AspEnrolledStudentsExportFileType.EnrolledStudentsCorrections && model.Month == null)
            {
                _validationResult.Errors.Add($"The month can't be null, when export for enrolled students corrections is selected!");
            }

            bool existingAspEnrolledStudentsExport = await _context.AspenrolledStudentsExports
               .AsNoTracking()
               .AnyAsync(x => x.SchoolYear == model.SchoolYear);

            if (!existingAspEnrolledStudentsExport)
            {
                _validationResult.Errors.Add($"Exporting a correction file is only possible if there is a main file for the corresponding school year!");
            }

            return _validationResult;
        }
    }
}
