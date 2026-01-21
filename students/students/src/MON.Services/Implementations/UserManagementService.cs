namespace MON.Services.Implementations
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MON.DataAccess;
    using MON.Models.Configuration;
    using MON.Models.UserManagement;
    using MON.Services.Interfaces;
    using MON.Shared;
    using MON.Shared.Enums;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class UserManagementService : BaseService<UserManagementService>, IUserManagementService
    {
        private readonly UserManagementServiceConfig _config;
        private readonly IHttpClientFactory _clientFactory;

        public UserManagementService(DbServiceDependencies<UserManagementService> dependencies,
            IOptions<UserManagementServiceConfig> config, IHttpClientFactory clientFactory)
            : base(dependencies)
        {
            _config = config.Value;
            _clientFactory = clientFactory;
        }

        public async Task CreateAsync(StudentRequestDto studentDto, CancellationToken cancellationToken = default)
        {
            UserManagementIntegrationResult resultModel = new UserManagementIntegrationResult
            {
                Action = "Create",
                Creator = _userInfo.Username,
                Timestamp = DateTime.UtcNow,
                Payload = studentDto != null ? JsonSerializer.Serialize(studentDto) : null,
            };

            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                StringContent requestContent = GetStudentStringContent(studentDto);
                HttpClient httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

                result = await httpClient.PostAsync($"{_config.Url}/create", requestContent, cancellationToken);
                resultModel.ResponseMessage = $"StatusCode: {result.StatusCode}, IsSuccessStatusCode: {result.IsSuccessStatusCode}, Body: {await result.Content.ReadAsStringAsync()}";
                await LogUserManagementRequest($"{_config.Url}/create", resultModel.Action, resultModel.Payload, resultModel.ResponseMessage, result != null ? (int)result.StatusCode : 0, studentDto.PersonId, !result.IsSuccessStatusCode, cancellationToken);

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = ex.GetInnerMostException().Message;
                resultModel.ResponseMessage = $"StatusCode: {result?.StatusCode}, IsSuccessStatusCode: {result?.IsSuccessStatusCode}, Body: {await result?.Content.ReadAsStringAsync()}";

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
                await LogUserManagementRequest($"{_config.Url}/create", resultModel.Action, resultModel.Payload, $"{resultModel.Message} {resultModel.ResponseMessage}", result != null ? (int)result.StatusCode : 0, studentDto.PersonId, true, cancellationToken);

                _logger.LogError($"An error has occurred while creating a user management student for model: {studentDto}", ex);
                throw;
            }
        }

        private async Task LogUserManagementRequest(string url, string operation, string request, string response, int responseHttpCode, int personId, bool isError = true, CancellationToken cancellationToken = default)
        {
            var failedRequest = new UserManagementApirequest()
            {
                Url = url,
                AuditModuleId = (int)AuditModuleEnum.Students,
                Operation = operation,
                Request = request,
                Response = response,
                ResponseHttpCode = responseHttpCode,
                PersonId = personId,
                CreatedBySysUserId = _userInfo.SysUserID,
                IsError = isError
            };
            _context.UserManagementApirequests.Add(failedRequest);
            await SaveAsync(cancellationToken);
        }

        public async Task UpdateAsync(StudentRequestDto studentDto, CancellationToken cancellationToken = default)
        {
            UserManagementIntegrationResult resultModel = new UserManagementIntegrationResult
            {
                Action = "Update",
                Creator = _userInfo.Username,
                Timestamp = DateTime.UtcNow,
                Payload = studentDto != null ? JsonSerializer.Serialize(studentDto) : null,
            };

            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                StringContent requestContent = GetStudentStringContent(studentDto);
                HttpClient httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

                result = await httpClient.PostAsync($"{_config.Url}/update", requestContent, cancellationToken);
                resultModel.ResponseMessage = $"StatusCode: {result.StatusCode}, IsSuccessStatusCode: {result.IsSuccessStatusCode}, Body: {await result.Content.ReadAsStringAsync()}";
                await LogUserManagementRequest($"{_config.Url}/update", resultModel.Action, resultModel.Payload, resultModel.ResponseMessage, result != null ? (int)result.StatusCode : 0, studentDto.PersonId, !result.IsSuccessStatusCode, cancellationToken);

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = ex.GetInnerMostException().Message;
                resultModel.ResponseMessage = $"StatusCode: {result?.StatusCode}, IsSuccessStatusCode: {result?.IsSuccessStatusCode}, Body: {await result?.Content.ReadAsStringAsync()}";

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
                await LogUserManagementRequest($"{_config.Url}/update", resultModel.Action, resultModel.Payload, $"{resultModel.Message} {resultModel.ResponseMessage}", result != null ? (int)result.StatusCode : 0, studentDto.PersonId, true, cancellationToken);

                _logger.LogError($"An error has occurred while updating a user management student for model: {studentDto}", ex);
                throw;
            }
        }

        public async Task DeleteAsync(StudentDeleteDisableRequestDto studentDto, CancellationToken cancellationToken = default)
        {
            UserManagementIntegrationResult resultModel = new UserManagementIntegrationResult
            {
                Action = "Delete",
                Creator = _userInfo.Username,
                Timestamp = DateTime.UtcNow,
                Payload = studentDto != null ? JsonSerializer.Serialize(studentDto) : null,
            };

            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                StringContent requestContent = GetStudentStringContent(studentDto);
                HttpClient httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

                result = await httpClient.PostAsync($"{_config.Url}/delete", requestContent, cancellationToken);
                resultModel.ResponseMessage = $"StatusCode: {result.StatusCode}, IsSuccessStatusCode: {result.IsSuccessStatusCode}, Body: {await result.Content.ReadAsStringAsync()}";
                await LogUserManagementRequest($"{_config.Url}/delete", resultModel.Action, resultModel.Payload, resultModel.ResponseMessage, result != null ? (int)result.StatusCode : 0, studentDto.PersonId, !result.IsSuccessStatusCode, cancellationToken);

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = ex.GetInnerMostException().Message;
                resultModel.ResponseMessage = $"StatusCode: {result?.StatusCode}, IsSuccessStatusCode: {result?.IsSuccessStatusCode}, Body: {await result?.Content.ReadAsStringAsync()}";

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
                await LogUserManagementRequest($"{_config.Url}/delete", resultModel.Action, resultModel.Payload, $"{resultModel.Message} {resultModel.ResponseMessage}", result != null ? (int)result.StatusCode : 0, studentDto.PersonId, true, cancellationToken);

                _logger.LogError($"An error has occurred while deleting a user management student for model: {studentDto}", ex);
                throw;
            }
        }

        public async Task EnrollmentSchoolCreateAsync(EnrollmentStudentToSchoolDeleteRequestDto studentDto, CancellationToken cancellationToken = default)
        {
            UserManagementIntegrationResult resultModel = new UserManagementIntegrationResult
            {
                Action = "EnrollmentSchoolCreate",
                Creator = _userInfo.Username,
                Timestamp = DateTime.UtcNow,
                Payload = studentDto != null ? JsonSerializer.Serialize(studentDto) : null,
            };

            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                StringContent requestContent = GetStudentStringContent(studentDto);
                HttpClient httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

                result = await httpClient.PostAsync($"{_config.Url}/enrollment-school-create", requestContent, cancellationToken);
                resultModel.ResponseMessage = $"StatusCode: {result.StatusCode}, IsSuccessStatusCode: {result.IsSuccessStatusCode}, Body: {await result.Content.ReadAsStringAsync()}";
                await LogUserManagementRequest($"{_config.Url}/enrollment-school-create", resultModel.Action, resultModel.Payload, resultModel.ResponseMessage, result != null ? (int)result.StatusCode : 0, studentDto.PersonId, !result.IsSuccessStatusCode, cancellationToken);

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);

            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = ex.GetInnerMostException().Message;
                resultModel.ResponseMessage = $"StatusCode: {result?.StatusCode}, IsSuccessStatusCode: {result?.IsSuccessStatusCode}, Body: {await result?.Content.ReadAsStringAsync()}";

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
                await LogUserManagementRequest($"{_config.Url}/enrollment-school-create", resultModel.Action, resultModel.Payload, $"{resultModel.Message} {resultModel.ResponseMessage}", result != null ? (int)result.StatusCode : 0, studentDto.PersonId, true, cancellationToken);

                _logger.LogError($"An error has occurred while enrollment school create for model: {studentDto}", ex);
                throw;
            }
        }

        public async Task EnrollmentSchoolDeleteAsync(EnrollmentStudentToSchoolDeleteRequestDto studentDto, CancellationToken cancellationToken = default)
        {
            UserManagementIntegrationResult resultModel = new UserManagementIntegrationResult
            {
                Action = "EnrollmentSchoolDelete",
                Creator = _userInfo.Username,
                Timestamp = DateTime.UtcNow,
                Payload = studentDto != null ? JsonSerializer.Serialize(studentDto) : null,
            };

            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                StringContent requestContent = GetStudentStringContent(studentDto);
                HttpClient httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

                result = await httpClient.PostAsync($"{_config.Url}/enrollment-school-delete", requestContent, cancellationToken);
                resultModel.ResponseMessage = $"StatusCode: {result.StatusCode}, IsSuccessStatusCode: {result.IsSuccessStatusCode}, Body: {await result.Content.ReadAsStringAsync()}";
                await LogUserManagementRequest($"{_config.Url}/enrollment-school-delete", resultModel.Action, resultModel.Payload, resultModel.ResponseMessage, result != null ? (int)result.StatusCode : 0, studentDto.PersonId, !result.IsSuccessStatusCode, cancellationToken);

                await SaveResult(studentDto?.PersonId ?? 0, resultModel);
            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = ex.GetInnerMostException().Message;
                resultModel.ResponseMessage = $"StatusCode: {result?.StatusCode}, IsSuccessStatusCode: {result?.IsSuccessStatusCode}, Body: {await result?.Content.ReadAsStringAsync()}";

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
                await LogUserManagementRequest($"{_config.Url}/enrollment-school-delete", resultModel.Action, resultModel.Payload, $"{resultModel.Message} {resultModel.ResponseMessage}", result != null ? (int)result.StatusCode : 0, studentDto.PersonId, true, cancellationToken);

                _logger.LogError($"An error has occurred while enrollment school delete for model: {studentDto}", ex);
                throw;
            }
        }

        public async Task EnrollmentStudentToClassCreateAsync(EnrollmentStudentToClassCreateRequestDto studentDto, CancellationToken cancellationToken = default)
        {
            UserManagementIntegrationResult resultModel = new UserManagementIntegrationResult
            {
                Action = "EnrollmentStudentToClassCreate",
                Creator = _userInfo.Username,
                Timestamp = DateTime.UtcNow,
                Payload = studentDto != null ? JsonSerializer.Serialize(studentDto) : null,
            };

            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                StringContent requestContent = GetStudentStringContent(studentDto);
                HttpClient httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

                result = await httpClient.PostAsync($"{_config.Url}/enrollment-class-create", requestContent, cancellationToken);
                resultModel.ResponseMessage = $"StatusCode: {result.StatusCode}, IsSuccessStatusCode: {result.IsSuccessStatusCode}, Body: {await result.Content.ReadAsStringAsync()}";
                await LogUserManagementRequest($"{_config.Url}/enrollment-class-create", resultModel.Action, resultModel.Payload, resultModel.ResponseMessage, result != null ? (int)result.StatusCode : 0, studentDto.PersonId, !result.IsSuccessStatusCode, cancellationToken);

                await SaveResult(studentDto?.PersonId ?? 0, resultModel);

            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = ex.GetInnerMostException().Message;
                resultModel.ResponseMessage = $"StatusCode: {result?.StatusCode}, IsSuccessStatusCode: {result?.IsSuccessStatusCode}, Body: {await result?.Content.ReadAsStringAsync()}";

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
                await LogUserManagementRequest($"{_config.Url}/enrollment-class-create", resultModel.Action, resultModel.Payload, $"{resultModel.Message} {resultModel.ResponseMessage}", result != null ? (int)result.StatusCode : 0, studentDto.PersonId, true, cancellationToken);

                _logger.LogError($"An error has occurred while enrollment student to class create for model: {studentDto}", ex);
                throw;
            }
        }

        public async Task<bool> EnrollmentStudentToClassDeleteAsync(EnrollmentStudentToClassDeleteRequestDto studentDto, CancellationToken cancellationToken = default)
        {
            UserManagementIntegrationResult resultModel = new UserManagementIntegrationResult
            {
                Action = "EnrollmentStudentToClassDelete",
                Creator = _userInfo.Username,
                Timestamp = DateTime.UtcNow,
                Payload = studentDto != null ? JsonSerializer.Serialize(studentDto) : null,
            };

            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                StringContent requestContent = GetStudentStringContent(studentDto);
                HttpClient httpClient = _clientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", _userInfo.AccessToken);

                result = await httpClient.PostAsync($"{_config.Url}/enrollment-class-delete", requestContent, cancellationToken);
                resultModel.ResponseMessage = $"StatusCode: {result.StatusCode}, IsSuccessStatusCode: {result.IsSuccessStatusCode}, Body: {await result.Content.ReadAsStringAsync()}";
                await LogUserManagementRequest($"{_config.Url}/enrollment-class-delete", resultModel.Action, resultModel.Payload, resultModel.ResponseMessage, result != null ? (int)result.StatusCode : 0, studentDto.PersonId, !result.IsSuccessStatusCode, cancellationToken);

                await SaveResult(studentDto?.PersonId ?? 0, resultModel);

                return result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                resultModel.IsError = true;
                resultModel.Message = ex.GetInnerMostException().Message;
                resultModel.ResponseMessage = $"StatusCode: {result?.StatusCode}, IsSuccessStatusCode: {result?.IsSuccessStatusCode}, Body: {await result?.Content.ReadAsStringAsync()}";

                await SaveResult(studentDto?.PersonId ?? 0, resultModel, cancellationToken);
                await LogUserManagementRequest($"{_config.Url}/enrollment-class-delete", resultModel.Action, resultModel.Payload, $"{resultModel.Message} {resultModel.ResponseMessage}", result != null ? (int)result.StatusCode : 0, studentDto.PersonId, true, cancellationToken);

                _logger.LogError($"An error has occurred while enrollment student to class delete for model: {studentDto}", ex);
                throw;
            }
        }

        private async Task SaveResult(int personId, UserManagementIntegrationResult resultModel, CancellationToken cancellationToken = default)
        {
            string str = JsonSerializer.Serialize(resultModel).Truncate(4000);
            await _context.Students.Where(x => x.PersonId == personId).UpdateAsync(x => new Student
            {
                UserManagementIntegrationResult = str
            }, cancellationToken);
        }

        private StringContent GetStudentStringContent(StudentRequestDto studentDto)
        {
            string studentContent = JsonSerializer.Serialize(studentDto);

            return new StringContent(studentContent, Encoding.UTF8, "application/json");
        }

        private StringContent GetStudentStringContent(StudentDeleteDisableRequestDto studentDto)
        {
            string studentContent = JsonSerializer.Serialize(studentDto);

            return new StringContent(studentContent, Encoding.UTF8, "application/json");
        }

        private StringContent GetStudentStringContent(EnrollmentStudentToSchoolDeleteRequestDto studentDto)
        {
            string studentContent = JsonSerializer.Serialize(studentDto);

            return new StringContent(studentContent, Encoding.UTF8, "application/json");
        }

        private StringContent GetStudentStringContent(EnrollmentStudentToClassCreateRequestDto studentDto)
        {
            string studentContent = JsonSerializer.Serialize(studentDto);

            return new StringContent(studentContent, Encoding.UTF8, "application/json");
        }

        private StringContent GetStudentStringContent(EnrollmentStudentToClassDeleteRequestDto studentDto)
        {
            string studentContent = JsonSerializer.Serialize(studentDto);

            return new StringContent(studentContent, Encoding.UTF8, "application/json");
        }
    }
}
