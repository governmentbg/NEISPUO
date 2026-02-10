namespace MON.Services.Infrastructure.Validators
{
    using DocumentFormat.OpenXml.Bibliography;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Enums;
    using MON.Services.Extensions;
    using MON.Services.Implementations;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    public class DischargeDocumentValidator : MovementDocumentBaseValidator
    {

        public DischargeDocumentValidator(MONContext context,
            IUserInfo userInfo,
            IInstitutionService institutionService,
            EduStateCacheService eduStateCacheService,
            ILogger<DischargeDocumentValidator> logger)
            : base(context, userInfo, institutionService, eduStateCacheService, logger)
        {

        }

        public async Task<ApiValidationResult> ValidateCreation(DischargeDocumentModel model)
        {
            ValidationResult.IsValid = true;

            if (model == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Model {nameof(DischargeDocumentModel)} cant be null!");
                return ValidationResult;
            }

            if (!model.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
                return ValidationResult;
            }

            var institutionId = model.InstitutionId ?? int.MinValue;
            ValidationResult.Merge(CheckRolePermission(institutionId));

            if (!model.IsDraft)
            {
                bool isStudentInInstitution = await IsStudentInInstitution(model.PersonId, institutionId, true);
                if (!isStudentInInstitution)
                {
                    ValidationResult.IsValid = false;
                    ValidationResult.Messages.Add(Messages.NotEnrolledInInstitution);
                }

                bool isStudentInHostInstitution = await IsStudentInHostInstitution(model.PersonId, institutionId, new int[] { (int)PositionType.Student, (int)PositionType.StudentSpecialNeeds });
                if (isStudentInHostInstitution)
                {
                    ValidationResult.IsValid = false;
                    ValidationResult.Messages.Add(Messages.EnrolledInHostInstitution);
                }
            }

            return ValidationResult;
        }

        public async Task<ApiValidationResult> ValidateUpdate(DischargeDocument entity, bool fromDraftToConfirmedStatusChange)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(DischargeDocument)} cant be null!");
                return ValidationResult;
            }

            if (!entity.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
                return ValidationResult;
            }

            // Премахване на ограничението за редакция само на чернови.
            // mmitova - 12.08.2022
            //ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.InstitutionId ?? int.MinValue));

            if (fromDraftToConfirmedStatusChange)
            {
                bool isStudentInInstitution = await IsStudentInInstitution(entity.PersonId, entity.InstitutionId ?? int.MinValue, true);
                if (!isStudentInInstitution)
                {
                    ValidationResult.IsValid = false;
                    ValidationResult.Messages.Add(Messages.NotEnrolledInInstitution);
                }

                bool isStudentInHostInstitution = await IsStudentInHostInstitution(entity.PersonId, entity.InstitutionId ?? int.MinValue, new int[] { (int)PositionType.Student, (int)PositionType.StudentSpecialNeeds });
                if (isStudentInHostInstitution)
                {
                    ValidationResult.IsValid = false;
                    ValidationResult.Messages.Add(Messages.EnrolledInHostInstitution);
                }
            }

            return ValidationResult;
        }

        public ApiValidationResult ValidateDeletion(DischargeDocument entity)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(DischargeDocument)} cant be null!");
                return ValidationResult;
            }

            if (!entity.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
                return ValidationResult;
            }

            ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(entity.InstitutionId ?? int.MinValue));

            return ValidationResult;
        }

        public async Task<ApiValidationResult> ValidateConfirmation(DischargeDocument entity)
        {
            ValidationResult.IsValid = true;

            if (entity == null)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add($"Entity {nameof(DischargeDocument)} cant be null!");
                return ValidationResult;
            }

            if (!entity.AuthorizeInstitution(_userInfo?.InstitutionID))
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.UnauthorizedMessageError);
                return ValidationResult;
            }

            int institutionId = entity.InstitutionId ?? int.MinValue;

            ValidationResult.Merge(CheckDocumentStatusPermisson(entity.Status));
            ValidationResult.Merge(CheckRolePermission(institutionId));

            bool isStudentInInstitution = await IsStudentInInstitution(entity.PersonId, institutionId, true);
            if (!isStudentInInstitution)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.NotEnrolledInInstitution);
            }

            bool isStudentInHostInstitution = await IsStudentInHostInstitution(entity.PersonId, institutionId, new int[] { (int)PositionType.Student, (int)PositionType.StudentSpecialNeeds });
            if (isStudentInHostInstitution)
            {
                ValidationResult.IsValid = false;
                ValidationResult.Messages.Add(Messages.EnrolledInHostInstitution);
            }

            return ValidationResult;
        }
    }
}
