namespace MON.Services.Infrastructure.Validators
{
    using DocumentFormat.OpenXml.Bibliography;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.EduState;
    using MON.Models.Enums;
    using MON.Services.Implementations;
    using MON.Services.Interfaces;
    using MON.Shared.Enums;
    using MON.Shared.Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    [ExcludeFromCodeCoverage]
    public class MovementDocumentBaseValidator
    {
        protected readonly MONContext _context;
        protected readonly IUserInfo _userInfo;
        protected readonly IInstitutionService _institutionService;
        private readonly EduStateCacheService _eduStateCacheService;
        private readonly ILogger _logger;

        public ApiValidationResult ValidationResult { get; private set; }

        protected MovementDocumentBaseValidator(MONContext context,
            IUserInfo userInfo,
            IInstitutionService institutionService,
            EduStateCacheService eduStateCacheService,
            ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userInfo = userInfo ?? throw new ArgumentNullException(nameof(userInfo));
            _institutionService = institutionService ?? throw new ArgumentNullException(nameof(institutionService));
            _eduStateCacheService = eduStateCacheService ?? throw new ArgumentNullException(nameof(eduStateCacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ValidationResult = new ApiValidationResult();
        }

        /// <summary>
        /// Проверка на статусът на документа. Документи със статус 'Потвърден' не могат да се редактират/трият.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected ApiValidationResult CheckDocumentStatusPermisson(int status)
        {
            var checkResult = new ApiValidationResult();
            if (status == (int)DocumentStatus.Final)
            {
                checkResult.IsValid = false;
                checkResult.Messages.Add("Документ със статус 'Потвърден' не може да се редактира/трие.");
                return checkResult;
            }

            checkResult.IsValid = true;
            return checkResult;
        }

        /// <summary>
        /// Проверява правата в зависимост от ролята на логнатия потребител.
        /// Роли:
        ///     Директор - има права върху документи, чиито institutionId е като неговата.
        /// </summary>
        /// <param name="docInstitutionId">Id на институцията създател на документа.</param>
        /// <returns></returns>
        protected ApiValidationResult CheckRolePermission(int docInstitutionId)
        {
            var checkResult = new ApiValidationResult();

            switch (_userInfo.UserRole)
            {
                case UserRoleEnum.School:
                    if (_userInfo.InstitutionID.HasValue && _userInfo.InstitutionID.Value == docInstitutionId)
                    {
                        checkResult.IsValid = true;
                    }
                    else
                    {
                        checkResult.IsValid = false;
                        checkResult.Messages.Add(Messages.UnauthorizedMessageError);
                    }

                    break;
                case UserRoleEnum.Ruo:
                case UserRoleEnum.Mon:
                case UserRoleEnum.Municipality:
                case UserRoleEnum.FinancingInstitution:
                case UserRoleEnum.Teacher:
                case UserRoleEnum.Student:
                case UserRoleEnum.Parent:
                default:
                    checkResult.IsValid = false;
                    checkResult.Messages.Add("Непозволена за действието потребителска роля.");
                    break;
            }

            return checkResult;
        }


        protected async Task<bool> IsStudentInInstitution(int personId, int institutionId, bool isDischarge = false)
        {
            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(institutionId);
            if (institution == null)
            {
                return false;
            }

            IEnumerable<EduStateModel> eduStates = (await _eduStateCacheService.GetEduStatesForStudent(personId))
                .Where(x => x.InstitutionId == institution.Id);
            InstitutionTypeEnum institutionType = (InstitutionTypeEnum)institution.InstTypeId;

            bool isStudent;

            switch (institutionType)
            {
                case InstitutionTypeEnum.School:
                case InstitutionTypeEnum.KinderGarden:
                    isStudent = eduStates.Any(x => x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds || (!isDischarge || x.PositionId == (int)PositionType.StudentPersDevelopmentSupport));
                    break;
                case InstitutionTypeEnum.PersonalDevelopmentSupportCenter:
                case InstitutionTypeEnum.SpecializedServiceUnit:
                    isStudent = eduStates.Any(x => x.PositionId == (int)PositionType.StudentPersDevelopmentSupport);
                    break;
                case InstitutionTypeEnum.CenterForSpecialEducationalSupport:
                    isStudent = eduStates.Any(x => x.PositionId == (int)PositionType.StudentOtherInstitution);
                    break;
                default:
                    isStudent = false;
                    break;
            }

            return isStudent;
        }


        protected async Task<bool> IsStudentInHostInstitution(int personId, int sendingInstituttionId, int[] positions)
        {
            var instTypes = new int[] { (int)InstitutionTypeEnum.School, (int)InstitutionTypeEnum.KinderGarden };
            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(sendingInstituttionId);
            if (institution == null || !instTypes.Contains(institution.InstTypeId ?? 0))
            {
                return false;
            }

            return (await _eduStateCacheService.GetEduStatesForStudent(personId))
                .Any(x => x.InstitutionId != sendingInstituttionId && positions.Contains(x.PositionId ?? 0));
        }
    }
}
