using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MON.API.Helpers;
using MON.DataAccess;
using MON.Models;
using MON.Models.Grid;
using MON.Models.StudentModels;
using MON.Models.StudentModels.History;
using MON.Models.StudentModels.Search;
using MON.Models.StudentModels.Update;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.ErrorHandling;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MON.API.Controllers
{
    public class StudentController : BaseApiController
    {
        private readonly IStudentService _studentService;
        private readonly IHistoryService _historyService; 
        private readonly IAppConfigurationService _configurationService;
        private bool? _validateEgn = null;
        private bool? _validateLnch = null;

        public StudentController(IStudentService studentService,
            IHistoryService historyService,        
            ILogger<StudentController> logger,
            IAppConfigurationService configurationService)
        {
            _studentService = studentService;
            _historyService = historyService;
            _logger = logger;
            _configurationService = configurationService;
        }

        private bool ValidateEgn
        {
            get
            {
                if (_validateEgn == null)
                {
                    string config = _configurationService.GetValueByKey("ValidateEgn").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _validateEgn = result;
                }

                return _validateEgn.Value;
            }
        }

        private bool ValidateLnch
        {
            get
            {
                if (_validateLnch == null)
                {
                    string config = _configurationService.GetValueByKey("ValidateLnch").Result;
                    bool.TryParse(config ?? "", out bool result);
                    _validateLnch = result;
                }

                return _validateLnch.Value;
            }
        }

        /// <summary>
        /// Демонстрация на ApiException
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object Throw()
        {
            throw new InvalidOperationException("This is an unhandled exception");
        }

        [HttpGet]
        public Task<StudentSummaryModel> GetSummaryById(int id, CancellationToken cancellationToken)
        {
            return _studentService.GetSummaryByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public ActionResult<IEnumerable<StudentViewModel>> GetAll()
        {
            IEnumerable<StudentViewModel> students = _studentService.GetAll();
            return Ok(students);
        }

        [HttpGet]
        public async Task<ActionResult<IPagedList<StudentSearchViewModel>>> List([FromQuery] StudentListInput input, CancellationToken cancellationToken)
        {
            return Ok(await _studentService.ListAsync(input, cancellationToken));
        }

        [HttpGet]
        public async Task<ActionResult<StudentViewModel>> GetById(int id)
        {
            StudentViewModel student = await _studentService.GetByIdAsync(id);
            return Ok(student);
        }

        [HttpGet]
        public Task<StudentPersonalDataModel> GetPersonDataById(int id, CancellationToken cancellationToken)
        {
            return _studentService.GetPersonDataByIdAsync(id, cancellationToken);
        }

        [HttpGet]
        public Task<List<EducationViewModel>> GetStudentEducation(int personId)
        {
            return _studentService.GetStudentEducationAsync(personId);
        }

        [HttpGet]
        public Task<StudentAdditionalDetailsModel> GetStudentInternationalProtection(int personId)
        {
            return _studentService.GetStudentInternationalProtectionAsync(personId);
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForStudentSearch)]
        public async Task<ActionResult<IPagedList<StudentSearchViewModel>>> GetBySearch([FromQuery] StudentSearchModelExtended studentSearchModel)
        {
            IPagedList<StudentSearchViewModel> students = await _studentService.GetBySearch(studentSearchModel);
            return Ok(students);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentResourceSupportHistoryModel>>> GetStudentResourceSupportHistory(int personId)
        {
            IEnumerable<StudentResourceSupportHistoryModel> studentResourceSupportModels = await _historyService.GetStudentResourceSupportHistoryRecords(personId);
            return Ok(studentResourceSupportModels);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentResourceSopDetails>>> GetStudentSopHistory(int personId)
        {
            IEnumerable<StudentResourceSopDetails> studenSopSupportModels = await _historyService.GetStudentResourceSopHistoryRecords(personId);
            return Ok(studenSopSupportModels);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentPersonalDataHistoryModel>>> GetStudentPersonalDataHistory(int personId)
        {
            IEnumerable<StudentPersonalDataHistoryModel> studentPersonalDataHistoryModels = await _historyService.GetStudentPersonalDataHistoryRecords(personId);
            return Ok(studentPersonalDataHistoryModels);
        }

        [HttpGet]
        public ActionResult<bool> IsPinValid(int pinTypeId, string pin)
        {
            var result = ValidatePin(pinTypeId, pin);
            return Ok(result);
        }

        [HttpGet]
        public Task<StudentSearchViewModel> CheckPinUniqueness(string pin)
        {
            return _studentService.CheckPinUniqueness(pin);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForStudentCreate)]
        public async Task<ActionResult<int>> PostAsync(StudentCreateModel model)
        {
            _logger.LogInformation($"Saving student:{model}");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid model:{model}");

                string errorStr = GetValidationErrors();
                return BadRequest(new
                {
                    message = "Invalid model validation",
                    errorStr
                });
            }

            var isPinValid = ValidatePin(model.PinTypeId, model.Pin, ValidateEgn, ValidateLnch);

            if (!isPinValid)
            {
                throw new ApiException($"Идентификатор '{model.Pin}' не е валиден за тип идентификатор '{model.PinTypeId}'");
            }

            var id = await _studentService.AddAsync(model);
            return id;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBasicDetailsAsync(StudentCreateModel model)
        {
            await _studentService.UpdateAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInternationalProtection(StudentAdditionalDetailsModel model)
        {
            await _studentService.UpdateInternationalProtection(model);
            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteById(int studentId)
        {
            await _studentService.ArchiveAsync(studentId).ConfigureAwait(false);
            return Ok();
        }

        [HttpDelete]
        public Task DeleteAzureAccount(int studentId)
        {
            return _studentService.DeleteAzureAccount(studentId);
        }

        [HttpGet]
        public async Task<bool> IsStudentInCurrentInstitution(int personId)
        {
            return await _studentService.IsStudentInCurrentInstitution(personId);
        }

        [HttpGet]
        public async Task<bool> CanEditStudentPersonalDetails(int personId)
        {
            return await _studentService.CanEditStudentPersonalDetails(personId);
        }

        [HttpGet]
        public async Task<InitialEnrollmentVisibilityCheckResultModel> InitialEnrollmentSecretBtnVisibilityCheck(int personId)
        {
            return await _studentService.InitialEnrollmentSecretBtnVisibilityCheck(personId);
        }

        [HttpPost]
        public async Task<IPagedList<StudentEmployerDeatilsListModel>> EmployerDetailsList([FromQuery] DualFormEmployerListInput input, [FromBody] GridFilter[] filters, CancellationToken cancellationToken)
        {
            return await _studentService.EmployerDetailsList(input, filters, cancellationToken);
        }

        private bool ValidatePin(int pinTypeId, string pin, bool validateEgn = true, bool validateLnch = true)
        {
            var result = pinTypeId switch
            {
                0 => validateEgn ? EgnValidator.ValidateEGN(pin) : true,
                1 => validateLnch ? LnchValidator.Validate(pin) : true,
                2 => true,
                _ => throw new ApiException("$Invalid pin type"),
            };

            return result;
        }
    }
}
