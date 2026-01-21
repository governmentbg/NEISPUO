namespace MON.Services
{
    using Microsoft.Extensions.Options;
    using MON.Models.Configuration;
    using MON.Services.Implementations;
    using MON.Services.Interfaces;

    public class MovementDocumentServiceDependencies<T>
    {
        public MovementDocumentServiceDependencies(IBlobService blobService,
            IOptions<BlobServiceConfig> blobServiceConfig, 
            IInstitutionService institutionService,
            IUserManagementService userManagementService,
            ISignalRNotificationService signalRNotificationService,
            CurriculumService curriculumService,
            EduStateCacheService eduStateCacheService)
        {
            BlobService = blobService;
            BlobServiceConfig = blobServiceConfig.Value;
            InstitutionService = institutionService;
            UserManagementService = userManagementService;
            SignalRNotificationService = signalRNotificationService;
            CurriculumService = curriculumService;
            EduStateCacheService = eduStateCacheService;
        }

        public IBlobService BlobService { get; }
        public BlobServiceConfig BlobServiceConfig { get; }
        public IInstitutionService InstitutionService { get; }
        public IUserManagementService UserManagementService { get; }
        public ISignalRNotificationService SignalRNotificationService { get; }
        public CurriculumService CurriculumService { get; }
        public EduStateCacheService EduStateCacheService { get; }
    }
}
