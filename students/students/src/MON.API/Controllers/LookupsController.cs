
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.Models;
using MON.Models.Dropdown;
using MON.Models.Enums;
using MON.Models.Grid;
using MON.Models.Institution;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class LookupsController : BaseApiController
    {
        private readonly ILookupService _service;
        private readonly ISignalRNotificationService _signalRNotificationService;

        public LookupsController(ILookupService lookupService,
            ISignalRNotificationService signalRNotificationService,
            ILogger<LookupsController> logger)
        {
            _service = lookupService;
            _logger = logger;
            _signalRNotificationService = signalRNotificationService;
        }

        [HttpGet]
        public async Task<ActionResult> GetReasonsForEqualization()
        {
            _logger.LogInformation("Getting all reasons for equalization.");

            try
            {
                IEnumerable<DropdownViewModel> reasons = await _service.GetReasonsForEqualization();
                return Ok(reasons);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting all reasons for equalization.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ClearCache()
        {
            _service.ClearCache();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetPinTypes()
        {
            _logger.LogInformation("Getting all pin types.");
            try
            {
                IEnumerable<DropdownViewModel> pinTypes = await _service.GetPinTypesAsync();
                return Ok(pinTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all pin types.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetCountriesBySearchString(string searchStr, string selectedValue)
        {
            _logger.LogInformation("Getting all countries.");
            try
            {
                IEnumerable<DropdownViewModel> countries = await _service.GetCountriesAsync(searchStr, selectedValue);
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all countries.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetGenders()
        {
            _logger.LogInformation("Getting all genders.");
            try
            {
                IEnumerable<DropdownViewModel> genders = await _service.GetGendersAsync();
                return Ok(genders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting all genders.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetGuardianTypes()
        {
            IEnumerable<DropdownViewModel> guardianTypes = await _service.GetGuardianTypesAsync();
            return Ok(guardianTypes);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetLanguages()
        {
            _logger.LogInformation("Getting all languages.");
            try
            {
                IEnumerable<DropdownViewModel> languages = await _service.GetLanguagesAsync();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting all languages.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetDistricts(string searchStr, string selectedValue)
        {
            _logger.LogInformation("Getting all districts.");
            try
            {
                IEnumerable<DropdownViewModel> districts = await _service.GetDistrictsAsync(searchStr, selectedValue);
                return Ok(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all districts.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetMunicipalities(string searchStr, string selectedValue, int? regionId)
        {
            _logger.LogInformation("Getting all municipalities.");
            try
            {
                IEnumerable<DropdownViewModel> municipalities = await _service.GetMunicipalitiesAsync(searchStr, selectedValue, regionId);
                return Ok(municipalities);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all municipalities.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetCities(string searchStr, string selectedValue, int? municipalityId)
        {
            IEnumerable<DropdownViewModel> cities = await _service.GetCitiesAsync(searchStr, selectedValue, municipalityId);
            return Ok(cities);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassGroupDropdownViewModel>>> GetClassGroups(int institutionId, int schoolYear,
            short? basicClass, short? minBasicClass, int? personId, bool? isInitialEnrollment, bool? filterForTeacher)
        {
            _logger.LogInformation("Getting all class groups.");
            try
            {
                IEnumerable<ClassGroupDropdownViewModel> groups = await _service.GetClassGroupsAsync(institutionId, schoolYear, basicClass, minBasicClass, personId, isInitialEnrollment, filterForTeacher);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting class groups.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<ClassGroupDropdownViewModel>> GetClassGroupsForLoggedUser(int schoolYear, int? personId)
        {
            return _service.GetClassGroupsForLoggedUser(schoolYear, personId);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetBasicClassesLimitForInstitution(int institutionId)
        {
            return _service.GetBasicClassesLimitForInstitution(institutionId);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetAddressesBySearchString(string searchStr, int? selectedValue)
        {
            _logger.LogInformation("Getting addresses.");
            try
            {
                IEnumerable<DropdownViewModel> addresses = await _service.GetAddressesAsync(searchStr, selectedValue);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all addresses.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetRepeaterReasons()
        {
            _logger.LogInformation("Getting repeater reasons.");

            try
            {
                IEnumerable<DropdownViewModel> rerecordedReasons = await _service.GetRepeaterReasonOptionsAsync();
                return Ok(rerecordedReasons);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all repeater reasons.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetCommuterOptions()
        {
            _logger.LogInformation("Getting commuter options.");

            try
            {
                IEnumerable<DropdownViewModel> commuterOptions = await _service.GetCommuterOptionsAsync();
                return Ok(commuterOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all commuter options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetScholarshipTypeOptions()
        {
            _logger.LogInformation("Getting scholarship type options.");

            try
            {
                IEnumerable<DropdownViewModel> scholarshipTypeOptions = await _service.GetScholarshipTypeOptionsAsync();
                return Ok(scholarshipTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all scholarship type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetScholarshipAmountOptions()
        {
            _logger.LogInformation("Getting scholarship amount options.");

            try
            {
                IEnumerable<DropdownViewModel> scholarshipAmountOptions = await _service.GetScholarshipAmountOptionsAsync();
                return Ok(scholarshipAmountOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all scholarship amount options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetStudentRelativeTypeOptions()
        {
            _logger.LogInformation("Getting student relative type options.");

            try
            {
                IEnumerable<DropdownViewModel> relativeTypeOptions = await _service.GetStudentRelativeTypeOptionsAsync();
                return Ok(relativeTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all student relative type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetStudentRelativeWorkStatusOptions()
        {
            _logger.LogInformation("Getting student relative work status options.");

            try
            {
                IEnumerable<DropdownViewModel> relativeTypeOptions = await _service.GetStudentRelativeWorkStatusOptionsAsync();
                return Ok(relativeTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all student relative work status options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSpecialNeedsTypesOptions()
        {
            _logger.LogInformation("Getting special needs types options.");

            try
            {
                IEnumerable<DropdownViewModel> specialNeedsTypes = await _service.GetSpecialNeedsTypeOptionsAsync();
                return Ok(specialNeedsTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all special needs types options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<IEnumerable<GradeDropdownViewModel>> GetGradeOptions(string searchStr, string selectedValue)
        {
            return await _service.GetGradeOptions(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<IEnumerable<DropdownViewModel>> GetSpecialNeedGradeOptions(string searchStr, string selectedValue)
        {
            return await _service.GetSpecialNeedGradeOptions(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<IEnumerable<DropdownViewModel>> GetOtherGradeOptions(string searchStr, string selectedValue)
        {
            return await _service.GetOtherGradeOptions(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<IEnumerable<DropdownViewModel>> GetQualitativeGradeOptions(string searchStr, string selectedValue)
        {
            return await _service.GetQualitativeGradeOptions(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSpecialNeedsSubTypesOptions()
        {
            _logger.LogInformation("Getting special needs sub types options.");

            try
            {
                IEnumerable<DropdownViewModel> specialNeedsTypes = await _service.GetSpecialNeedsSubTypesOptionsAsync();
                return Ok(specialNeedsTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all special needs sub types options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetResourceSupportSpecialistTypesOptions()
        {
            _logger.LogInformation("Getting resource special support specialist types options.");
            try
            {
                IEnumerable<DropdownViewModel> resourceSupportSpecialistTypes = await _service.GetResourceSupportSpecialistTypeOptionsAsync();
                return Ok(resourceSupportSpecialistTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all resource support specialist types options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetResourceSupportTypeOptions()
        {
            _logger.LogInformation("Getting resource support types options.");
            try
            {
                IEnumerable<DropdownViewModel> resourceSupportTypes = await _service.GetResourceSupportTypeOptionsAsync();
                return Ok(resourceSupportTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all resource support types options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstitutionDropdownViewModel>>> GetInstitutionOptions(string searchStr, int? selectedValue, int? regionId)
        {
         _logger.LogInformation("Getting institution options.");
            try
            {
                IEnumerable<InstitutionDropdownViewModel> institutionOptions = await _service.GetInstitutionsAsync(searchStr, selectedValue, regionId);
                return Ok(institutionOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all institution options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetStudentTypeOptions()
        {
            _logger.LogInformation("Getting student type options.");
            try
            {
                IEnumerable<DropdownViewModel> studentTypeOptions = await _service.GetStudentTypeOptionsAsync();
                return Ok(studentTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all student type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSupportPeriodOptions()
        {
            _logger.LogInformation("Getting support period options.");
            try
            {
                IEnumerable<DropdownViewModel> supportPeriodOptions = await _service.GetSupportPeriodOptionsAsync();
                return Ok(supportPeriodOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all support period options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetEarlyEvaluationReasonOptions()
        {
            _logger.LogInformation("Getting evaluatiion reason options.");
            try
            {
                IEnumerable<DropdownViewModel> evaluationReasonsOptions = await _service.GetEarlyEvaluationReasonOptionsAsync();
                return Ok(evaluationReasonsOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all evaluatiion reason options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetCommonSupportTypeOptions()
        {
            _logger.LogInformation("Getting common support type options.");
            try
            {
                IEnumerable<DropdownViewModel> commonSupportTypeOptions = await _service.GetCommonSupportTypeOptionsAsync();
                return Ok(commonSupportTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all common support type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetEducationTypeOptions()
        {
            _logger.LogInformation("Getting education type options.");
            try
            {
                IEnumerable<DropdownViewModel> educationTypeOptions = await _service.GetEducationTypeOptionsAsync();
                return Ok(educationTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all education type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetEducationFormOptions()
        {
            _logger.LogInformation("Getting education type options.");
            try
            {
                IEnumerable<DropdownViewModel> educationTypeOptions = await _service.GetEducationFormOptions();
                return Ok(educationTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all education type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassGroupDropdownViewModel>>> GetValidEducationFormsOptionsForPerson(bool? isNotPresentForm)
        {
            _logger.LogInformation("Getting education type options.");
            try
            {
                IEnumerable<ClassGroupDropdownViewModel> educationTypeOptions = await _service.GetValidForStudentEducationFormOptionsAsync(isNotPresentForm);
                return Ok(educationTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting all education type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<ClassGroupDropdownViewModel>> GetEduFormsForLoggedUser(bool? isNotPresentForm)
        {
            return _service.GetEduFormsForLoggedUser(isNotPresentForm);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetAdditionalSupportTypeOptions()
        {
            _logger.LogInformation("Getting additional support type options.");
            try
            {
                IEnumerable<DropdownViewModel> commonSupportTypeOptions = await _service.GetAdditionalSupportTypeOptionsAsync();
                return Ok(commonSupportTypeOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting additional support type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSubjectOptions(string searchStr, int? selectedValue, int? pageSize)
        {
            _logger.LogInformation("Getting subject options.");
            try
            {
                IEnumerable<DropdownViewModel> subjectOptions = await _service.GetSubjectOptionsAsync(searchStr, selectedValue, pageSize);
                return Ok(subjectOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting subject options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDetailsDropdownViewModel>>> GetSubjecDetailsOptions()
        {
            _logger.LogInformation("Getting subject with subject type options.");
            try
            {
                IEnumerable<SubjectDetailsDropdownViewModel> options = await _service.GetSubjectDetailsOptionsAsync();
                return Ok(options);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting subject with subject type options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetSubjectsForLoggedInstitution(string searchStr, int? selectedValue)
        {
            return _service.GetSubjectsForLoggedInstitution(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetAdmissionReasonTypeOptions()
        {
            _logger.LogInformation("Getting Admission Reason options.");
            try
            {
                IEnumerable<DropdownViewModel> admissionOptions = await _service.GetAdmissionOptionsAsync();
                return Ok(admissionOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occured while getting admission reason options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<CurrentClassDropdownViewModel>> GetCurrentStudentClassesForDischarge(int personId, int? selectedValue)
        {
            return _service.GetCurrentStudentClassesForDischarge(personId, selectedValue);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetDischargeReasonTypeOptions(bool? isForDischarge, bool? isForRelocation)
        {
            return _service.GetDischargeOptionsAsync(isForDischarge, isForRelocation);
        }

        [HttpGet]
        public Task<List<SubjectTypeDropdownViewModel>> GetSubjectTypeOptions(int? basicSubjectTypeId, int? partId, bool? showAll)
        {
            return _service.GetSubjectTypeOptions(basicSubjectTypeId, partId, showAll);
        }


        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForNomenclatureRead)]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetNomenclatureData(string tableName)
        {
            _logger.LogInformation($"Getting nomenclature data for table {tableName}");

            try
            {
                var nomenclatureData = await _service.GetNomenclatureDataAsync(tableName);
                return Ok(nomenclatureData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting nomenclature data for table  {tableName}.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSchoolYears(int? institutionId, int? selectedValue)
        {
            _logger.LogInformation($"Getting school years from inst_basic.CurrentYear");

            try
            {
                IEnumerable<DropdownViewModel> list = await _service.GetSchoolYears(institutionId, selectedValue);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting school years from inst_basic.CurrentYear.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<DropdownViewModel> GetRegionById(int? regionId)
        {
            return _service.GetRegionById(regionId);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSchoolYearsForPerson(int personId)
        {
            _logger.LogInformation($"Getting school years for person {personId}");

            try
            {
                IEnumerable<DropdownViewModel> list = await _service.GetSchoolYearsForPerson(personId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting school years for person {personId}.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeDropdownViewModel>>> GetGradesByBasicClassId(int basicClassId, string searchStr, bool? filterSpecialGrade, int? selectedValue)
        {
            _logger.LogInformation($"Getting Grades");

            try
            {
                IEnumerable<GradeDropdownViewModel> grades;
                if (Enum.IsDefined(typeof(FirstToThirdClassGrade), basicClassId))
                {
                    grades = await _service.GetFirstToThirdClassGradesAsync();
                }
                else
                {
                    grades = await _service.GetGradesAsync(searchStr, selectedValue, filterSpecialGrade);
                }

                return Ok(grades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting Grades", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetLocalAreasOptions()
        {
            _logger.LogInformation($"Getting local areas options.");

            try
            {
                IEnumerable<DropdownViewModel> list = await _service.GetLocalAreasOptions();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting local areas options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSpecialityOptions()
        {
            _logger.LogInformation($"Getting speciality options.");

            try
            {
                IEnumerable<DropdownViewModel> list = await _service.GetSpecialityOptions();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting speciality options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetValidSpecialityOptions()
        {
            _logger.LogInformation($"Getting speciality options.");

            try
            {
                IEnumerable<DropdownViewModel> list = await _service.GetValidSpecialityOptions();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting speciality options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }


        [HttpDelete]
        [Authorize(Policy = DefaultPermissions.PermissionNameForNomenclatureDelete)]
        public async Task<IActionResult> DeleteNomenclature(string tableName, int id)
        {
            _logger.LogInformation($"Deleting nomenclature recordId:{id} for table:{tableName}");

            try
            {
                await _service.DeleteNomenclatureAsync(tableName, id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while deleting nomenclature for table  {tableName}.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForNomenclatureUpdate)]
        public async Task<IActionResult> UpdateNomenclature(string tableName, NomenclatureViewModel nomenclature)
        {
            _logger.LogInformation($"Updating nomenclature with id:{nomenclature.Value} for table:{tableName}");

            try
            {
                await _service.UpdateNomenclatureAsync(tableName, nomenclature);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while updating nomenclature with id:{nomenclature.Value} for table {tableName}.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForNomenclatureCreate)]
        public async Task<IActionResult> AddNomenclature(string tableName, NomenclatureViewModel nomenclature)
        {
            _logger.LogInformation($"Adding nomenclature for table:{tableName}");

            try
            {
                await _service.AddNomenclatureAsync(tableName, nomenclature);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while adding nomenclature for table {tableName}.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetBasicDocumentTypes(string searchStr, bool? schemaSpecified,
            bool? isValidation, bool? isIncludedInRegister, bool? isAppendix, string selectedValue, bool? isRuoDoc,
            bool? filterByDetailedSchoolType, [FromQuery]int[] mainBasicDocuments)
        {
            return _service.GetBasicDocumentTypes(searchStr, schemaSpecified, isValidation, isIncludedInRegister, isAppendix, selectedValue, isRuoDoc, filterByDetailedSchoolType, mainBasicDocuments);
        }


        [HttpGet]
        public Task<IEnumerable<DiplomaTemplateDropdownViewModel>> GetBasicDocumentTypesWithJsonContent()
        {
            return _service.GetBasicDocumentTypesWithJsonContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetResourceSupportSpecialistWorkPlaces()
        {
            _logger.LogInformation($"Getting resource support specialist workplaces.");

            try
            {
                var result = await _service.GetResourceSupportSpecialistWorkPlaces();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting resource support specialist workplaces.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiplomaTemplateDropdownViewModel>>> GetCurrentUserDiplomaTemplates()
        {
            _logger.LogInformation($"Getting diploma templates.");

            try
            {
                IEnumerable<DiplomaTemplateDropdownViewModel> list = await _service.GetCurrentUserDiplomaTemplates();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting diploma templates.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<DiplomaTemplateDropdownViewModel>> GetCurrentUserValidationTemplates()
        {
            return _service.GetCurrentUserValidationTemplates();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetLodEvaluationResults()
        {
            _logger.LogInformation($"Getting lod evaluations results options.");

            try
            {
                IEnumerable<DropdownViewModel> list = await _service.GetLodEvaluationResults();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting lod evaluations results options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiplomaTemplateDropdownViewModel>>> GetBasicClassOptions([FromQuery] BasicClassOptionsInput input)
        {
            _logger.LogInformation($"Getting lod evaluations results options.");

            try
            {
                List<DropdownViewModel> list = await _service.GetBasicClassOptions(input.SearchStr, input.SelectedValue, input.MinId, input.MaxId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting lod evaluations results options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiplomaTemplateDropdownViewModel>>> GetBasicClassValidOptions(int? relatedObject, string searchStr, int? selectedValue, int? minId, int? maxId)
        {
            _logger.LogInformation($"Getting lod evaluations results options.");

            try
            {
                List<DropdownViewModel> list = await _service.GetBasicClassValidOptions(relatedObject, searchStr, selectedValue, minId, maxId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting lod evaluations results options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetBasicClassesForLoggedUser(string searchStr, int? selectedValue)
        {
            return _service.GetBasicClassesForLoggedUser(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiplomaTemplateDropdownViewModel>>> GetBasicSubjectTypeOptions()
        {
            _logger.LogInformation($"Getting lod evaluations results options.");

            try
            {
                List<DropdownViewModel> list = await _service.GetBasicSubjectTypeOptions();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting lod evaluations results options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiplomaTemplateDropdownViewModel>>> GetMinistryOptions()
        {
            _logger.LogInformation($"Getting lod evaluations results options.");

            try
            {
                List<DropdownViewModel> list = await _service.GetMinistryOptions();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting lod evaluations results options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetLodEvaluationsProfileClasses(int personId, short schoolYear)
        {
            _logger.LogInformation($"Getting Lod evaluations ProfileClasses");

            try
            {
                IEnumerable<DropdownViewModel> result = await _service.GetLodEvaluationsProfileClasses(personId, schoolYear);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while Lod evaluations ProfileClasses", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetStudentAwardTypes()
        {
            _logger.LogInformation($"Getting student award types");

            try
            {
                var result = await _service.GetStudentAwardTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting student award types.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetNaturalIndicatorsPeriods()
        {
            _logger.LogInformation($"Getting natural indicators periods");

            try
            {
                var result = await _service.GetNaturalIndicatorsPeriods();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting natural indicators periods.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetResourceSupportDataPeriods()
        {
            try
            {
                var result = await _service.GetResourceSupportDataPeriods();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetStudentSanctionTypes()
        {
            _logger.LogInformation($"Getting student sanction types");

            try
            {
                var result = await _service.GetStudentSanctionTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting student sanction types.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetAwardCategories()
        {
            _logger.LogInformation($"Getting award categories");

            try
            {
                var result = await _service.GetAwardCategories();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting award categories.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetFounders()
        {
            _logger.LogInformation($"Getting founders options");

            try
            {
                var result = await _service.GetFounders();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting founders options.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetAwardReasons()
        {
            _logger.LogInformation($"Getting awards reasons");

            try
            {
                var result = await _service.GetAwardReasons();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting award reasons.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetScholarshipFinancingOrgans()
        {
            _logger.LogInformation($"Getting scholarship financing organs");

            try
            {
                var result = await _service.GetScholarshipFinancingOrgans();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting scholarship financing organs", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropdownViewModel>>> GetSupportingEquipment()
        {
            _logger.LogInformation($"Getting supporting equipment");

            try
            {
                var result = await _service.GetSupportingEquipment();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting supporting equipment", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetRecognitionEducationLevel(string searchStr, int? selectedValue)
        {
            return _service.GetRecognitionEducationLevel(searchStr, selectedValue);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetEducationFormDiplomaOptions()
        {
            return _service.GetEducationFormDiplomaOptions(ValidEnum.All);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetSPPOOProfession(int? relatedObject, string searchStr, int? selectedValue)
        {
            return _service.GetSPPOOProfession(relatedObject, searchStr, selectedValue);
        }

        [HttpGet]
        public Task<List<SPPOOSpecialityDropdownViewModel>> GetSPPOOSpeciality(int? relatedObject, string searchStr, int? selectedValue)
        {
            return _service.GetSPPOOSpeciality(relatedObject, searchStr, selectedValue);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetClassTypeOptions()
        {
            return _service.GetClassTypeOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetClassTypeDiplomaOptions()
        {
            return _service.GetClassTypeDiplomaOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetSPPOOSpecialityExtendedText(int? relatedObject, string searchStr, int? selectedValue)
        {
            return _service.GetSPPOOSpecialityExtendedText(relatedObject, searchStr, selectedValue);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetExamTypeOptions()
        {
            return _service.GetExamTypeOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetBuildingAreas(string searchStr, int? selectedValue)
        {
            return _service.GetBuildingAreas(searchStr, selectedValue);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetBuildingRooms(string searchStr, int? selectedValue, string buildingAreas)
        {
            return _service.GetBuildingRooms(searchStr, selectedValue, buildingAreas);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetSpecialEquipment(string searchStr, int? selectedValue, string buildingRooms)
        {
            return _service.GetSpecialEquipment(searchStr, selectedValue, buildingRooms);
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetAvailableArchitecture()
        {
            return _service.GetAvailableArchitecture();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetFLLevelOptions()
        {
            return _service.GetFLLevelOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetITLevelOptions()
        {
            return _service.GetITLevelOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetStudentSelfGovernmentPositions()
        {
            return _service.GetStudentSelfGovernmentPositions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetStudentParticipations()
        {
            return _service.GetStudentParticipations();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetStudentPositionOptions()
        {
            return _service.GetStudentPositionOptions();
        }
        
        [HttpGet]
        public Task<List<DropdownViewModel>> GetNKROptions()
        {
            return _service.GetNKRAsync();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetEKROptions()
        {
            return _service.GetEKRAsync();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetMonStatusOptions()
        {
            return _service.GetMonStatusOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetAspStatusOptions()
        {
            return _service.GetAspStatusOptions();
        }

        [HttpGet]
        public Task<List<ExternalEvaluationTypeModel>> GetExternalEvaluationTypeOptions()
        {
            return _service.GetExternalEvaluationTypeOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetStudentPositionOptionsByCondition(int? selectedValue, int? institutionId, int? personId)
        {
            return _service.GetStudentPositionOptionsByCondition(selectedValue, institutionId, personId);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Test(int? personId)
        {
            await _signalRNotificationService.StudentHubTest("Ухаааа SignalR работи!", personId);

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> NotifyUser(string username, string message)
        {
            await _signalRNotificationService.NotifyUser(username, message ?? $"Здравей {username}! Системата те призовава.");

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TestFinalizedLodsReload(int personId)
        {
            await _signalRNotificationService.StudentFinalizedLodsReloaded(personId, new List<short> { 2019, 2025 });

            return Ok();
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetORESTypesOptions()
        {
            return await _service.GetORESTypesOptions();
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetClassTypes(int? basicClassId)
        {
            return await _service.GetClassTypesAsync(basicClassId);
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetClassTypesForLoggedUser(string searchStr, int? selectedValue)
        {
            return await _service.GetClassTypesForLoggedUser(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetStaff(string searchStr, int? selectedValue)
        {
            return await _service.GetStaffAsync(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<IEnumerable<AbsenceCampaignDropdownViewModel>> GetAbsenceCampaigns()
        {
            return await _service.GetAbsenceCampaigns();
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetDocumentEducationTypeOptions()
        {
            return await _service.GetDocumentEducationTypeOptions();
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetFlOptions()
        {
            return await _service.GetFlOptions();
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetVetLevelOptions()
        {
            return await _service.GetVetLevelOptions();
        }

        [HttpGet]
        public async Task<List<DropdownViewModel>> GetInstitutionTypeOptions()
        {
            return await _service.GetInstitutionTypeOptions();
        }

        [HttpGet]
        public Task<List<DropdownViewModel>> GetCurriculumPartOptions(string searchStr, string selectedValue)
        {
            return _service.GetCurriculumPartOptions(searchStr, selectedValue);
        }

        [HttpGet]
        public async Task<ActionResult> GetReasonsForReassessment()
        {
            _logger.LogInformation("Getting all reasons for reassessment.");

            try
            {
                IEnumerable<DropdownViewModel> reasons = await _service.GetReasonsForReassessment();
                return Ok(reasons);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting all reasons for reassessment.", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<IEnumerable<DropdownViewModel>> GetStudentSessionOptions()
        {
            return await _service.GetStudentSessionOptionsAsync();
        }
    }
}
