using Microsoft.Extensions.Logging;
using MON.DataAccess;
using MON.Models;
using MON.Services.Implementations;
using MON.Services.Interfaces;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Infrastructure.Validators
{
    [ExcludeFromCodeCoverage]
    public class RelocationDocumentValidator : MovementDocumentBaseValidator
    {
        public RelocationDocumentValidator(MONContext context,
            IUserInfo userInfo,
            IInstitutionService institutionService,
            EduStateCacheService eduStateCacheService,
            ILogger<RelocationDocumentValidator> logger)
            : base(context, userInfo, institutionService, eduStateCacheService, logger)
        {

        }

        public async Task<ApiValidationResult> ValidateCreation(RelocationDocumentModel model, CancellationToken cancelationtoken = default)
        {
            ValidationResult.IsValid = true;

            if (model == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Model {nameof(RelocationDocumentModel)} cant be null!");
            }

            if (!_userInfo.InstitutionID.HasValue || _userInfo.InstitutionID.Value != model.SendingInstitutionId)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
            }

            bool isDraft = model.Status == (int)DocumentStatus.Draft;
            if (!isDraft)
            {
                bool isStudentInInstitution = await IsStudentInInstitution(model.PersonId, model.SendingInstitutionId ?? int.MinValue);
                if (!isStudentInInstitution)
                {
                    ValidationResult.IsValid = false;
                    ValidationResult.Messages.Add(Messages.NotEnrolledInInstitution);
                }
            }

            return ValidationResult;
        }

        public async Task<ApiValidationResult> ValidateUpdate(RelocationDocument entity, RelocationDocumentModel model, CancellationToken cancelationtoken = default)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(AdmissionDocument)} cant be null!");
                return ValidationResult;
            }

            if (model == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Model {nameof(AdmissionDocumentModel)} cant be null!");
                return ValidationResult;
            }

            if (!_userInfo.InstitutionID.HasValue || _userInfo.InstitutionID.Value != entity.SendingInstitutionId)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
            }

            bool isDraft = model.Status == (int)DocumentStatus.Draft;
            if (!isDraft)
            {
                bool isStudentInInstitution = await IsStudentInInstitution(entity.PersonId, entity.SendingInstitutionId ?? int.MinValue);
                if (!isStudentInInstitution)
                {
                    ValidationResult.IsValid = false;
                    ValidationResult.Messages.Add(Messages.NotEnrolledInInstitution);
                }
            }

            ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.SendingInstitutionId ?? int.MinValue));

            return ValidationResult;
        }

        public ApiValidationResult ValidateDeletion(RelocationDocument entity, CancellationToken cancelationtoken = default)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(AdmissionDocument)} cant be null!");
                return ValidationResult;
            }

            if (!_userInfo.InstitutionID.HasValue || _userInfo.InstitutionID.Value != entity.SendingInstitutionId)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
            }

            ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.SendingInstitutionId ?? default));

            return ValidationResult;
        }

        public async Task<ApiValidationResult> ValidateConfirmation(RelocationDocument entity, CancellationToken cancelationtoken = default)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(AdmissionDocument)} cant be null!");
                return ValidationResult;
            }

            if (!_userInfo.InstitutionID.HasValue || _userInfo.InstitutionID.Value != entity.SendingInstitutionId)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
            }

            bool isStudentInInstitution = await IsStudentInInstitution(entity.PersonId, entity.SendingInstitutionId ?? int.MinValue);
            if (!isStudentInInstitution)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.NotEnrolledInInstitution);
            }

            ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.SendingInstitutionId ?? int.MinValue));

            return ValidationResult;
        }

        /// <summary>
        /// Проверява дали документ за преместване може да се редактира/трие/потвърждава.
        /// Използва се от RelocationDocumentService при зареждане на данните за грида с документи за преместване
        /// за даден ученик. Определя видимостта на бутоните за редакция, триене и потвърждаване.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        public bool CanBeModified(int status, int institutionId)
        {
            bool checkResult = true;

            checkResult = checkResult && CheckDocumentStatusPermisson(status).IsValid;
            checkResult = checkResult && (CheckRolePermission(institutionId)).IsValid;

            return checkResult;
        }
    }
}
