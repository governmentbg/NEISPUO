namespace MON.API.Controllers
{
    using Hangfire.Server;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MON.DataAccess;
    using MON.Models;
    using MON.Models.Absence;
    using MON.Models.ASP;
    using MON.Models.Dashboards;
    using MON.Models.Grid;
    using MON.Models.Status;
    using MON.Models.StudentModels;
    using MON.Services.Interfaces;
    using MON.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Threading;
    using System.Threading.Tasks;

    public class AdminController : BaseApiController
    {
        private readonly IDashboardService _service;
        private readonly IUserInfo _userInfo;

        public AdminController(IDashboardService service, ILogger<AdminController> logger, IUserInfo userInfo)
        {
            _service = service;
            _logger = logger;
            _userInfo = userInfo;
        }

        [HttpGet]
        public async Task<ActionResult<ServerInfo>> GetServerInfo()
        {
            var framework = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;
            var currentProcess = Process.GetCurrentProcess();

            int availableWorkerThreads = 0;
            int availableIoThreads = 0;
            ThreadPool.GetAvailableThreads(out availableWorkerThreads, out availableIoThreads);

            int minWorkerThreads = 0;
            int minIoThreads = 0;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minIoThreads);

            int maxWorkerThreads = 0;
            int maxIoThreads = 0;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIoThreads);

            var serverInfo = new ServerInfo
            {
                OsPlatform = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                AspDotnetVersion = framework,
                Allocated = GC.GetTotalMemory(false),
                PrivateMemorySize64 = currentProcess.PrivateMemorySize64,
                VirtualMemorySize64 = currentProcess.VirtualMemorySize64,
                WorkingSet64 = currentProcess.WorkingSet64,
                CpuUsage = await GetCpuUsageForProcess(),
                ProcessorCount = Environment.ProcessorCount,
                MinIOThreads = minIoThreads, 
                MinWorkerThreads = minWorkerThreads,
                MaxIOThreads = maxIoThreads,
                MaxWorkerThreads = maxWorkerThreads,
                AvailableWorkerThreads = availableWorkerThreads,
                AvailableIOThreads = availableIoThreads,
            };

            return serverInfo;
        }

        [HttpGet]
        public async Task<ActionResult<DirectorDashboardModel>> GetDirectorDashboardAsync()
        {
            var institutionId = _userInfo.InstitutionID;

            try
            {
                var dashboardData = await _service.GetDirectorDashboardAsync(institutionId);

                return dashboardData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error has occured while getting dashboard data for institutionId:{institutionId}", ex);

                return BadRequest(new
                {
                    message = ex.Message,
                    statusCode = 500
                });
            }
        }

        [HttpGet]
        public async Task<IPagedList<StudentForAdmissionModel>> GetStudentsForAdmission([FromQuery] PagedListInput input)
        {
            return await _service.GetStudentsForAdmission(input);
        }

        [HttpGet]
        public async Task<IPagedList<StudentToBeDischargedModel>> GetStudentsForDischarge([FromQuery] PagedListInput input)
        {
            return await _service.GetStudentsForDischarge(input);
        }

        [HttpGet]
        public async Task<IPagedList<StudentEnvironmentCharacteristicModel>> GetStudentEnvironmentCharacteristics([FromQuery] StudentEnvCharacteristicListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetStudentEnvironmentCharacteristics(input, cancellationToken);
        }

        [HttpGet]
        public async Task<IPagedList<VStudentExternalEvaluation>> GetStudentExernalEvaluationsList([FromQuery] DualFormEmployerListInput input, CancellationToken cancellationToken)
        {
            return await _service.GetStudentExernalEvaluationsList(input, cancellationToken);
        }

        [HttpPost]
        public async Task AdmissionStudentsAsync(List<StudentClassModel> models)
        {
            await _service.AdmissionStudentsAsync(models);
        }

        [HttpPost]
        public async Task DischargeStudentsAsync(List<StudentToBeDischargedModel> models)
        {
            await _service.DischargeStudentsAsync(models);
        }

        [HttpGet]
        public async Task<IPagedList<SopEnrollmentDetailViewModel>> GetSopEnrollmentDetails([FromQuery] PagedListInput input)
        {
            return await _service.GetSopEnrollmentDetails(input);
        }

        [HttpGet]
        public async Task<int> GetInstitutiontSopEnrollmentsCount()
        {
            return await _service.GetInstitutiontSopEnrollmentsCount();
        }

        private async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime; await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime; var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds; var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed); return cpuUsageTotal * 100;
        }

        [HttpGet]
        public async Task<int> GetStudentsCount()
        {
            return await _service.GetStudentsCount();
        }

        [HttpGet]
        public async Task<List<StudentStatsModel>> GetStudentsCountGroupByClassType()
        {
            return await _service.GetStudentsCountGroupByClassType();
        }

        [HttpGet]
        public async Task<List<StudentStatsModel>> GetStudentsCountByClassType(int classTypeId)
        {
            return await _service.GetStudentsCountByClassType(classTypeId);
        }

        [HttpGet]
        public async Task<ClassGroupStatsModel> GetClassGroupStats()
        {
            return await _service.GetClassGroupStats();
        }

        [HttpGet]
        public async Task<DiplomaStatsModel> GetDiplomaStats()
        {
            return await _service.GetDiplomaStats();
        }

        [HttpGet]
        public async Task<List<AbsenceCampaignViewModel>> GetAllActiveCampaigns()
        {
            return await _service.GetAllActiveCampaigns();
        }
    }
}
