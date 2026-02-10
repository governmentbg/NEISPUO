namespace MON.Services.Implementations
{
    using Microsoft.EntityFrameworkCore;
    using MON.DataAccess;
    using MON.Models.Configuration;
    using MON.Services.Interfaces;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using MON.Models.StudentModels.Class;
    using MON.Models.UserManagement;
    using System.Collections.Generic;
    using MON.Shared;
    using MON.Models.Enums;
    using Microsoft.Extensions.Logging;
    using MON.Models;
    using MON.Shared.Enums;
    using Z.EntityFramework.Plus;
    using System.Threading;

    public class MovementDocumentBaseService<T> : BaseService<T>
    {
        protected readonly IBlobService _blobService;
        protected readonly BlobServiceConfig _blobServiceConfig;
        protected readonly IInstitutionService _institutionService;
        protected readonly IUserManagementService _userManagementService;
        protected readonly ISignalRNotificationService _signalRNotificationService;
        protected readonly CurriculumService _curriculumService;
        protected readonly EduStateCacheService _eduStateCacheService;

        public MovementDocumentBaseService(DbServiceDependencies<T> dependencies,
            MovementDocumentServiceDependencies<T> movementDocumentDependencies)
            : base(dependencies)
        {
            _ = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _ = movementDocumentDependencies ?? throw new ArgumentNullException(nameof(movementDocumentDependencies));
            _blobService = movementDocumentDependencies.BlobService;
            _blobServiceConfig = movementDocumentDependencies.BlobServiceConfig;
            _institutionService = movementDocumentDependencies.InstitutionService;
            _userManagementService = movementDocumentDependencies.UserManagementService;
            _signalRNotificationService = movementDocumentDependencies.SignalRNotificationService;
            _curriculumService = movementDocumentDependencies.CurriculumService;
            _eduStateCacheService = movementDocumentDependencies.EduStateCacheService;
        }

        /// <summary>
        /// Създаваме запис в InstitutionChanges. Използва се като индикация за инвалидиране на кеша, свързан с институцията.
        /// Използва се основно от дневниците.
        /// </summary>
        protected async Task SaveInstitutionChange(short schoolYear, int classId, CancellationToken cancellationToken = default)
        {
            int institutionId = await _context.ClassGroups
                .AsNoTracking()
                .Where(cg => cg.ClassId == classId)
                .Select(cg => cg.InstitutionId)
                .FirstOrDefaultAsync(cancellationToken);

            await SaveInstitutionChange(institutionId, schoolYear, cancellationToken);
        }

        /// <summary>
        /// Създаваме запис в InstitutionChanges. Използва се като индикация за инвалидиране на кеша, свързан с институцията.
        /// Използва се основно от дневниците.
        /// </summary>
        protected async Task SaveInstitutionChange(int institutionId, short? schoolYear, CancellationToken cancellation = default)
        {
            if (schoolYear.HasValue)
            {
                schoolYear = await _institutionService.GetCurrentYear(institutionId);
            }

            InstitutionChange institutionChange = await _context.InstitutionChanges
                .Where(ic => ic.InstitutionId == institutionId)
                .FirstOrDefaultAsync(cancellation);

            if (institutionChange == null)
            {
                _context.InstitutionChanges.Add(InstitutionChange.From(institutionId, schoolYear.Value));
            }
            else
            {
                institutionChange.Version = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// При оптисване от група/паралелка следва да изтрием(сетнем IsValid = false)
        /// учебния плаn на ученика в старата му група паралелка.
        /// Използва се в транзакция.
        /// </summary>
        /// <param name="studentClass"></param>
        protected async Task DeleteCurriculumStudents(int personId, short schoolYear, int studentClassId, int institutionId, CancellationToken cancellationToken = default)
        {
            bool hasExtSoProviderIntegration = await _institutionService.HasExternalSoProvider(institutionId, schoolYear);
            if (hasExtSoProviderIntegration)
            {
                // Ако за институцията и учебната година име запис за интеграция с външен дневник няма да създаваме учебен план.
                return;
            }

            List<int> deletedCurriculumIds = await _curriculumService.DeleteForStudentClass(new CurriculumStudentUpdateModel
            {
                PersonId = personId,
                SchoolYear = schoolYear,
                StudentClassId = studentClassId
            });

            if (!deletedCurriculumIds.IsNullOrEmpty())
            {
                await _userManagementService.EnrollmentStudentToClassDeleteAsync(new EnrollmentStudentToClassDeleteRequestDto
                {
                    PersonId = personId,
                    CurriculumIds = deletedCurriculumIds.ToArray()
                }, cancellationToken);
            }
        }

        /// <summary>
        /// Добавяне на учебния план на новата паралелка.
        /// Използва се в транзакция.
        /// </summary>
        /// <returns></returns>
        protected async Task CreateCurriculumStudents(int personId, short schoolYear, int institutionId, int studentClassId, CancellationToken cancellationToken = default)
        {
            bool hasExtSoProviderIntegration = await _institutionService.HasExternalSoProvider(institutionId, schoolYear);
            if (hasExtSoProviderIntegration)
            {
                // Ако за институцията и учебната година име запис за интеграция с външен дневник няма да създаваме учебен план.
                return;
            }

            List<int> addedCurriculumIds = await _curriculumService.AddForStudentClass(personId, schoolYear, studentClassId);
            var studentClass = _context.StudentClasses.FirstOrDefault(i => i.Id == studentClassId);
            if (!addedCurriculumIds.IsNullOrEmpty())
            {
                await _userManagementService.EnrollmentStudentToClassCreateAsync(new EnrollmentStudentToClassCreateRequestDto
                {
                    PersonId = personId,
                    CurriculumIds = addedCurriculumIds.ToArray(),
                    BasicClassId = studentClass?.BasicClassId
                }, cancellationToken);
            }
        }

        protected int GetInstitutionId(int? institutionId)
        {
            return institutionId ?? _userInfo.InstitutionID ?? int.MinValue;
        }

        /// <summary>
        /// Извикване на userManagementService при отписване от институция.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        protected async Task EnrollmentSchoolDelete(int personId, int institutionId, CancellationToken cancellationToken = default)
        {
            EnrollmentStudentToSchoolDeleteRequestDto studentDto = new EnrollmentStudentToSchoolDeleteRequestDto
            {
                PersonId = personId,
                InstitutionId = institutionId
            };

            await _userManagementService.EnrollmentSchoolDeleteAsync(studentDto, cancellationToken);
        }

        /// <summary>
        /// Извикване на userManagementService при запис в институция.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        protected async Task EnrollmentSchoolCreate(int personId, int institutionId, CancellationToken cancellationToken = default)
        {
            EnrollmentStudentToSchoolDeleteRequestDto studentDto = new EnrollmentStudentToSchoolDeleteRequestDto
            {
                PersonId = personId,
                InstitutionId = institutionId
            };

            await _userManagementService.EnrollmentSchoolCreateAsync(studentDto, cancellationToken);
        }

        /// <summary>
        /// Отписване от институцията. 
        /// Всички записи в StudentClass, за който IsCurretnt == true, за даден ученик се сетват на IsCurretnt == false и Status = (int)StudentClassStatus.Transferred.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task DischargeFromInstitution(int personId, int? institutionId,
            int? dischargeReasonId, RelocationDocument relocationDocument,
            DischargeDocument dischargeDocument, CancellationToken cancellationToken = default)
        {
            // По някава причина има ситуации, където model.SendingInstitutionId няма стойност.
            // Затова ще я вземем от данните на логнатия потребител.
            institutionId = GetInstitutionId(institutionId);

            List<StudentClass> studentClasses = await _context.StudentClasses
                .Where(x => x.PersonId == personId
                    && x.InstitutionId == institutionId
                    && x.IsCurrent)
                .ToListAsync(cancellationToken);

            foreach (StudentClass studentClass in studentClasses)
            {
                studentClass.IsCurrent = false;
                studentClass.Status = (int)StudentClassStatus.Transferred;
                studentClass.DischargeReasonId = dischargeReasonId;
                if (relocationDocument != null)
                {
                    studentClass.RelocationDocument = relocationDocument;
                    studentClass.DischargeDate = relocationDocument.DischargeDate;
                }
                if (dischargeDocument != null)
                {
                    studentClass.DischargeDocument = dischargeDocument;
                    studentClass.DischargeDate = dischargeDocument.DischargeDate;
                }

                await DeleteCurriculumStudents(studentClass.PersonId, studentClass.SchoolYear, studentClass.Id, studentClass.InstitutionId, cancellationToken);
            }

            await UpdateEduStateOnDischarge(personId, institutionId ?? default, cancellationToken);
        }

        /// <summary>
        /// При наличие на запис в EduState с позиция 3 или 10, но за различна институция от дадената, то записът за дедената институция се изтрива.
        /// При отписване/преместване всички записи в EduState с позиция 3 за дадената институция стават 9.
        /// При отписване/преместване от позиция 7 (когато институцията не е ЦСОП - InstType <> 4), 8 и 10, в EduState записът за дадената институция се изтрива
        /// При отписване/преместване от позиция 7 (когато институцията е ЦСОП - InstType = 4), позицията в EduState става 9
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task UpdateEduStateOnDischarge(int personId, int instituttionId, CancellationToken cancellationToken = default)
        {
            try
            {
                List<EducationalState> eduStates = await _context.EducationalStates
                    .Where(i => i.PersonId == personId
                        && i.PositionId != (int)PositionType.Discharged
                        && i.PositionId != (int)PositionType.Staff)
                    .ToListAsync(cancellationToken);

                foreach (var eduState in eduStates.Where(x => x.InstitutionId == instituttionId))
                {
                    // При наличие на запис в EduState с позиция 3 или 10, но за различна институция от дадената, то записът за дадената институция се изтрива.
                    if (eduStates.Any(x => x.InstitutionId.HasValue && x.InstitutionId != instituttionId
                        && (x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds)))
                    {
                        _context.EducationalStates.Remove(eduState);
                    }

                    switch ((PositionType)eduState.PositionId)
                    {
                        case PositionType.Student:

                            if (eduStates.Any(x => x.InstitutionId != eduState.InstitutionId
                                && (x.PositionId == (int)PositionType.Student || x.PositionId == (int)PositionType.StudentSpecialNeeds)))
                            {
                                // Ако съществува запис в EduState с позиция 3 или 10 но за друга институция
                                // и съществува запис в AdmissionPermissionRequests, за който FirstEducationalStateId == eduState.EducationalStateId
                                // трием EduState-а.
                                // Поискан и пуснат ученик и в последствие записан в искащата институция.

                                if (await _context.AdmissionPermissionRequests.AnyAsync(x => x.PersonId == eduState.PersonId && x.IsPermissionGranted
                                    && x.FirstEducationalStateId == eduState.EducationalStateId, cancellationToken))
                                {
                                    _context.EducationalStates.Remove(eduState);
                                }
                            }
                            else
                            {
                                // При отписване всички записи в EduState с позиция 3 за дадената институция стават 9.
                                eduState.PositionId = (int)PositionType.Discharged;
                            }

                            break;
                        case PositionType.StudentOtherInstitution:
                            InstitutionCacheModel institutionBaseMode = await _institutionService.GetInstitutionCache(eduState.InstitutionId ?? int.MinValue);
                            if (institutionBaseMode != null && institutionBaseMode.InstTypeId.HasValue)
                            {
                                if (institutionBaseMode.IsCSOP)
                                {
                                    // При отписване от позиция 7 (когато институцията е ЦСОП - InstType = 4), позицията в EduState става 9
                                    eduState.PositionId = (int)PositionType.Discharged;
                                }
                                else
                                {
                                    // При отписване от позиция 7 (когато институцията не е ЦСОП - InstType <> 4) в EduState записът за дадената институция се изтрива
                                    _context.EducationalStates.Remove(eduState);
                                }
                            }
                            break;
                        case PositionType.StudentPersDevelopmentSupport:
                        case PositionType.StudentSpecialNeeds:
                            // При отписване от позиция 8 и 10, в EduState записът за дадената институция се изтрива
                            _context.EducationalStates.Remove(eduState);
                            break;
                        default:
                            break;
                    }
                }

                await _eduStateCacheService?.ClearEduStatesForStudent(personId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Грешка при обновяване на EduState", ex);
                throw;
            }

        }

        /// <summary>
        /// При масово записване в група/паралелка в ЦПЛР не минаваме през документ за записване.
        /// Ако за даден ученик липсва запис в EducationalState за дадената институция и позиция = 8
        /// следва да създадем запис.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        protected async Task UpdateEduStateOnCplrEnrollment(int personId, int institutionId, CancellationToken cancellationToken = default)
        {
            if(!await _context.EducationalStates.AnyAsync(x => x.PersonId == personId && x.InstitutionId == institutionId
                && x.PositionId == (int)PositionType.StudentPersDevelopmentSupport))
            {
                _context.EducationalStates.Add(new EducationalState
                {
                    PersonId = personId,
                    InstitutionId = institutionId,
                    PositionId = (int)PositionType.StudentPersDevelopmentSupport
                });

                await SaveAsync(cancellationToken);
                await _eduStateCacheService?.ClearEduStatesForStudent(personId);
            }
        }

        /// <summary>
        /// При масово записване в група/паралелка от тип общежитие не минаваме през документ за записване.
        /// Ако за даден ученик липсва запис в EducationalState за дадената институция
        /// следва да създадем запис с позиция 8.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        protected async Task UpdateEduStateOnAdditionalEnrollment(int personId, int institutionId,
            PositionType position = PositionType.StudentPersDevelopmentSupport,
            CancellationToken cancellationToken = default)
        {
            if (!await _context.EducationalStates.AnyAsync(x => x.PersonId == personId && x.InstitutionId == institutionId))
            {
                _context.EducationalStates.Add(new EducationalState
                {
                    PersonId = personId,
                    InstitutionId = institutionId,
                    PositionId = (int)position
                });

                await SaveAsync(cancellationToken);
                await _eduStateCacheService?.ClearEduStatesForStudent(personId);
            }
        }

        protected async Task<int> UpdateEduStateOnEnrollment(int personId, int institutionId, int position, CancellationToken cancellationToken = default)
        {
            // Изтриване на всички записи в EducationalStates за дадения ученик със статус PositionType.Discharge
            await _context.EducationalStates
                .Where(x => x.PersonId == personId && x.PositionId == (int)PositionType.Discharged)
                .DeleteAsync(cancellationToken);

            var edutState = new EducationalState
            {
                PersonId = personId,
                InstitutionId = institutionId,
                PositionId = position
            };

            _context.EducationalStates.Add(edutState);

            await SaveAsync(cancellationToken);
            await _eduStateCacheService?.ClearEduStatesForStudent(personId);

            return edutState.EducationalStateId;
        }

        protected async Task UpdateAdmissionDocumentOnClassEnrollment(int studentClassid, int? admissionDocumentId, CancellationToken cancellationToken = default)
        {
            if (!admissionDocumentId.HasValue) return;

            await _context.AdmissionDocuments
                .Where(x => x.Id == admissionDocumentId && x.CurrentStudentClassId == null)
                .UpdateAsync(x => new AdmissionDocument
                {
                    CurrentStudentClassId = studentClassid
                }, cancellationToken);
        }

        /// <summary>
        /// Проверяваме за подаден AdmissionDocumentId.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task FixEnrollmentModelMissingAdmissionDocument(StudentClassModel model, int? institutionId, CancellationToken cancellationToken = default)
        {
            if (model == null || !institutionId.HasValue || model.AdmissionDocumentId.HasValue) return;

            model.AdmissionDocumentId = await _context.AdmissionDocuments
                .Where(x => x.PersonId == model.PersonId && x.InstitutionId == institutionId && x.Status == (int)DocumentStatus.Final
                    && !x.StudentClasses.Any())
                .MaxAsync(x => (int?)x.Id, cancellationToken);
        }

        /// <summary>
        /// Проверка и промяна на StudentEduForm.
        /// При InitialEnrollment/преместване в ClassKind = 1 (initialEnrollment) в ДЕТСКА ГРАДИНА
        /// да не се вижда полето за форма на обучение и винаги да се записва -1 
        /// и да не може никога да се променя, с изключение на служебната паралелка
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected async Task FixEduForm(StudentClass entity)
        {
            if (entity == null) return;

            InstitutionCacheModel institution = await _institutionService.GetInstitutionCache(entity.InstitutionId);

            if (institution?.InstTypeId != (int)InstitutionTypeEnum.KinderGarden) return;
            if (entity.ClassTypeId != (int)ClassKindEnum.Basic) return;
            if (entity.IsNotPresentForm == true) return;

            if (entity.StudentEduFormId != -1)
            {
                entity.StudentEduFormId = -1;
            }
        }
    }
}
