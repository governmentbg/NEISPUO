using MON.Models;
using MON.Models.Dropdown;
using MON.Models.Institution;
using MON.Shared.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MON.Services.Interfaces
{
    public interface ILookupService
    {
        Task<IEnumerable<DropdownViewModel>> GetReasonsForEqualization();
        Task<IEnumerable<DropdownViewModel>> GetPinTypesAsync();
        Task<IEnumerable<DropdownViewModel>> GetGendersAsync();
        Task<IEnumerable<DropdownViewModel>> GetGuardianTypesAsync();
        Task<IEnumerable<DropdownViewModel>> GetLanguagesAsync();
        Task<IEnumerable<DropdownViewModel>> GetCountriesAsync(string searchStr, string selectedValue);
        Task<List<DropdownViewModel>> GetCountryOptions();
        Task<IEnumerable<DropdownViewModel>> GetDistrictsAsync(string searchStr, string selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetMunicipalitiesAsync(string searchStr, string selectedValue, int? regionId);
        Task<IEnumerable<DropdownViewModel>> GetCitiesAsync(string searchStr, string selectedValue, int? municipalityId);
        Task<List<DropdownViewModel>> GetClassTypesAsync(int? basicClassId);
        Task<List<ClassGroupDropdownViewModel>> GetClassGroupsAsync(int institutionId, int schoolYear, short? basicClass, short? minBasicClass, int? personId, bool? isInitialEnrollment, bool? filterForTeacher);
        Task<IEnumerable<DropdownViewModel>> GetAddressesAsync(string searchStr, int? selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetRepeaterReasonOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetCommuterOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetStudentRelativeTypeOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetStudentRelativeWorkStatusOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetScholarshipTypeOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetScholarshipAmountOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetSpecialNeedsTypeOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetSpecialNeedsSubTypesOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetResourceSupportSpecialistTypeOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetResourceSupportTypeOptionsAsync();
        Task<IEnumerable<InstitutionDropdownViewModel>> GetInstitutionsAsync(string searchStr, int? selectedValue, int? regionId);
        Task<IEnumerable<DropdownViewModel>> GetStudentTypeOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetSupportPeriodOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetEarlyEvaluationReasonOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetCommonSupportTypeOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetAdditionalSupportTypeOptionsAsync();
        Task<IEnumerable<DropdownViewModel>> GetSubjectOptionsAsync(string searchStr, int? selectedValue, int? pageSize);
        Task<List<DropdownViewModel>> GetSubjectsForLoggedInstitution(string searchStr, int? selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetAdmissionOptionsAsync();
        Task<List<DropdownViewModel>> GetDischargeOptionsAsync(bool? isForDischarge, bool? isForRelocation);
        Task<IEnumerable<DropdownViewModel>> GetEducationFormOptions(ValidEnum valid = ValidEnum.True);
        Task<List<DropdownViewModel>> GetEducationFormDiplomaOptions(ValidEnum valid = ValidEnum.True);
        Task<List<ClassGroupDropdownViewModel>> GetValidForStudentEducationFormOptionsAsync(bool? isNotPresentForm);
        Task<List<ClassGroupDropdownViewModel>> GetEduFormsForLoggedUser(bool? isNotPresentForm);
        Task<List<CurrentClassDropdownViewModel>> GetCurrentStudentClassesForDischarge(int personId, int? selectedValue);
        Task<IEnumerable<NomenclatureViewModel>> GetNomenclatureDataAsync(string tableName);
        Task DeleteNomenclatureAsync(string tableName, int id);
        Task AddNomenclatureAsync(string tableName, NomenclatureViewModel nomenclature);
        Task UpdateNomenclatureAsync(string tableName, NomenclatureViewModel nomenclature);
        Task<IEnumerable<DropdownViewModel>> GetEducationTypeOptionsAsync();
        Task<List<DropdownViewModel>> GetSchoolYears(int? institutionId, int? selectedValue);
        Task<DropdownViewModel> GetRegionById(int? regionId);
        Task<List<DropdownViewModel>> GetSchoolYearsForPerson(int personId);
        Task<IEnumerable<GradeDropdownViewModel>> GetGradesAsync(string searchStr, int? selectedValue, bool? filterSpecialGrade);
        Task<IEnumerable<GradeDropdownViewModel>> GetFirstToThirdClassGradesAsync();
        Task<IEnumerable<AbsenceCampaignDropdownViewModel>> GetAbsenceCampaigns();
        Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetBasicDocumentTypes(string searchStr, bool? schemaSpecified, bool? isValidation,
            bool? isIncludedInRegister, bool? isAppendix, string selectedValue, bool? isRuoDoc, bool? filterByDetailedSchoolType, int[] mainBasicDocuments);

        /// <summary>
        /// Връща IQueryable<BasicDocument.
        /// Ако ролята на логнатия потребител е "Институция (директор)" ще филтрираме само тези документи,
        /// за които има запис в таблица BasicDocumentLimits(свързваща таблица между BasicDocument и DetailedSchoolTypeId).
        /// </summary>
        /// <returns>IQueryable<BasicDocument</returns>
        Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetBasicDocumentTypesWithJsonContent();

        Task<IEnumerable<DropdownViewModel>> GetLocalAreasOptions();
        Task<IEnumerable<DropdownViewModel>> GetSpecialityOptions();
        Task<IEnumerable<DropdownViewModel>> GetValidSpecialityOptions();
        Task<List<DropdownViewModel>> GetSPPOOSpecialityExtendedText(int? relatedObject, string searchStr, int? selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetResourceSupportSpecialistWorkPlaces();
        Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetCurrentUserDiplomaTemplates();
        Task<IEnumerable<DropdownViewModel>> GetLodEvaluationResults();
        Task<List<DropdownViewModel>> GetBasicClassOptions(string searchStr, int? selectedValue, int? minId, int? maxId);
        Task<List<DropdownViewModel>> GetBasicClassValidOptions(int? relatedObject, string searchStr, int? selectedValue, int? minId, int? maxId);
        Task<List<DropdownViewModel>> GetBasicSubjectTypeOptions();
        Task<List<DropdownViewModel>> GetMinistryOptions();
        Task<IEnumerable<DropdownViewModel>> GetLodEvaluationsProfileClasses(int personId, short schoolYear);
        Task<IEnumerable<DropdownViewModel>> GetStudentAwardTypes();
        Task<IEnumerable<DropdownViewModel>> GetNaturalIndicatorsPeriods();
        Task<IEnumerable<DropdownViewModel>> GetResourceSupportDataPeriods();
        Task<IEnumerable<DropdownViewModel>> GetStudentSanctionTypes();
        Task<IEnumerable<DropdownViewModel>> GetAwardCategories();
        Task<IEnumerable<DropdownViewModel>> GetFounders();
        Task<IEnumerable<DropdownViewModel>> GetAwardReasons();
        Task<IEnumerable<DropdownViewModel>> GetScholarshipFinancingOrgans();
        Task<IEnumerable<DropdownViewModel>> GetSupportingEquipment();
        Task<List<DropdownViewModel>> GetRecognitionEducationLevel(string searchStr, int? selectedValue);
        Task<List<DropdownViewModel>> GetSPPOOProfession(int? relatedObject, string searchStr, int? selectedValue);
        Task<List<SPPOOSpecialityDropdownViewModel>> GetSPPOOSpeciality(int? relatedObject, string searchStr, int? selectedValue);
        Task<List<DiplomaTemplateDropdownViewModel>> GetCurrentUserValidationTemplates();
        Task<List<DropdownViewModel>> GetExamTypeOptions();
        Task<List<DropdownViewModel>> GetBuildingAreas(string searchStr, int? selectedValue);
        Task<List<DropdownViewModel>> GetBuildingRooms(string searchStr, int? selectedValue, string buildingAreas);
        Task<List<DropdownViewModel>> GetSpecialEquipment(string searchStr, int? selectedValue, string buildingRooms);
        Task<List<DropdownViewModel>> GetStudentParticipations();
        Task<List<DropdownViewModel>> GetStudentSelfGovernmentPositions();
        Task<List<DropdownViewModel>> GetStudentPositionOptions();
        Task<List<SubjectTypeDropdownViewModel>> GetSubjectTypeOptions(int? basicSubjectTypeId, int? partId, bool? showAll);
        Task<List<ClassGroupDropdownViewModel>> GetClassGroupsForLoggedUser(int schoolYear, int? personId);
        Task<List<DropdownViewModel>> GetBasicClassesLimitForInstitution(int institutionId);
        Task<List<ExternalEvaluationTypeModel>> GetExternalEvaluationTypeOptions();
        Task<List<DropdownViewModel>> GetAvailableArchitecture();
        Task<List<DropdownViewModel>> GetFLLevelOptions();
        Task<List<DropdownViewModel>> GetITLevelOptions();
        Task<IEnumerable<SubjectDetailsDropdownViewModel>> GetSubjectDetailsOptionsAsync();
        Task<List<DropdownViewModel>> GetStudentPositionOptionsByCondition(int? selectedValue, int? institutionId, int? personId);
        Task<List<DropdownViewModel>> GetMonStatusOptions();
        Task<List<DropdownViewModel>> GetAspStatusOptions();
        Task<List<DropdownViewModel>> GetORESTypesOptions();

        void RemoveKey(string cacheKey);
        void ClearCache();
        Task<List<DropdownViewModel>> GetBasicClassesForLoggedUser(string searchStr, int? selectedValue);
        Task<List<DropdownViewModel>> GetClassTypesForLoggedUser(string searchStr, int? selectedValue);
        Task<List<DropdownViewModel>> GetBasicDocumentOptions(ValidEnum valid = ValidEnum.True);
        Task<List<DropdownViewModel>> GetClassTypeOptions(ValidEnum valid = ValidEnum.True);
        Task<List<DropdownViewModel>> GetClassTypeDiplomaOptions(ValidEnum valid = ValidEnum.All);
        Task<List<DropdownViewModel>> GetSPPOOProfessionOptions(ValidEnum valid = ValidEnum.True);
        Task<List<DropdownViewModel>> GetSPPOOSpecialityOptions(ValidEnum valid = ValidEnum.True);

        /// <summary>
        /// Връща детайли за <see cref="DataAccess.Subject"/>. Използва IMemoryCache.
        /// </summary>
        /// <param name="id">Id на предмет</param>
        /// <returns></returns>
        Task<DropdownViewModel> GetSubjectDetails(int id, ValidEnum valid = ValidEnum.True);
       
        /// Връща детайли за <see cref="DataAccess.BasicDocument"/>. Използва IMemoryCache.
        /// </summary>
        /// <param name="id">Id на BasicDocument</param>
        /// <returns></returns>
        Task<BasicDocumentDetailsModel> GetBasicDocumentDetails(int id);
        Task<List<DropdownViewModel>> GetEKRAsync();
        Task<List<DropdownViewModel>> GetNKRAsync();
        Task<List<DropdownViewModel>> GetDocumentEducationTypeOptions(ValidEnum valid = ValidEnum.True);
        Task<List<DropdownViewModel>> GetFlOptions(ValidEnum valid = ValidEnum.True);
        Task<List<DropdownViewModel>> GetVetLevelOptions();
        Task<List<DropdownViewModel>> GetInstitutionTypeOptions();
        Task<List<DropdownViewModel>> GetStaffAsync(string searchStr, int? selectedValue);
        MaintenanceModeModel GetMaintenanceMode();
        Task<IEnumerable<GradeDropdownViewModel>> GetGradeOptions(string searchStr, string selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetSpecialNeedGradeOptions(string searchStr, string selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetOtherGradeOptions(string searchStr, string selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetQualitativeGradeOptions(string searchStr, string selectedValue);
        Task<List<DropdownViewModel>> GetCurriculumPartOptions(string searchStr, string selectedValue);
        Task<IEnumerable<DropdownViewModel>> GetReasonsForReassessment();
        Task<IEnumerable<DropdownViewModel>> GetStudentSessionOptionsAsync();
    }
}
